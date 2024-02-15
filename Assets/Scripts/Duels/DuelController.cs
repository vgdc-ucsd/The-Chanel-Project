using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player, Enemy
}

public class DuelController
{
    private bool isEnemyTurn;
    private DuelSettings settings;
    private Board board;
    private PlayerSettings playerSettings, enemySettings;
    private CharStatus playerStatus, enemyStatus;
    private UIManager ui;
    private List<Card> modifiedCards = new List<Card>();
    private Deck playerDeck;
    private Deck enemyDeck;
    private BasicDuelAI ai;
    private Team currentTeam;

    public DuelController() {
        settings = DuelManager.Instance.Settings;
        ui = DuelManager.Instance.UI;
        playerDeck = DuelManager.Instance.PlayerDeck;
        enemyDeck = DuelManager.Instance.PlayerDeck;

        isEnemyTurn = settings.EnemyGoesFirst;
        if (settings.EnemyGoesFirst) currentTeam = Team.Enemy;
        else currentTeam = Team.Player;
        board = new Board(settings.BoardRows, settings.BoardCols);
        playerSettings = settings.Player;
        enemySettings = settings.Enemy;

        if(settings.SameSettingsForBothPlayers) {
            enemySettings = playerSettings;
        }
        playerStatus = new CharStatus(playerSettings);
        enemyStatus = new CharStatus(enemySettings);

        ui.Player.Status = playerStatus;
        ui.Enemy.Status = enemyStatus;

        ai = new BasicDuelAI();
    }

    public void StartDuel() {
        DrawCardPlayer(playerSettings.StartingCards);
        DrawCardEnemy(enemySettings.StartingCards);
    }

    // Updates the board with the card played at the desired index
    // ATTEMPTS to place a card at the specified pos
    // if successful, places a card on UI.

    // ALL attempts to play a card should go through this method, it will update everything needed.
    public void PlayCard(Card card, BoardCoords pos) {
        // Check out of bounds
        if (board.IsOutOfBounds(pos)) { 
            Debug.Log(pos + " out of bounds");
            return;
        }
        CharStatus charStatus = CurrentCharStatus();
        if (!charStatus.CanUseMana(card.ManaCost))
        {
            Debug.Log("Not enough Mana"); //TODO: UI feedback
            return;
        }
        charStatus.UseMana(card.ManaCost);
        card.CardInteractableRef.PlaceCard(pos);
        board.PlaceCard(card, pos);
        DuelEvents.Instance.UpdateUI();
    }

    private void ProcessBoard() {
        // Process all cards
        for(int i = 0; i < board.Cols; i++) {
            for(int j = 0; j < board.Rows; j++) {
                BoardCoords pos = new BoardCoords(i,j);
                if (board.IsOccupied(pos)) {
                    ProcessCard(board.GetCard(pos), pos);
                }
            }
        }

        // Update cards that were modified
        foreach(Card c in modifiedCards) {
            if(c.Health <= 0) {
                c.TileInteractableRef.occupied = false;
                board.RemoveCard(c.TileInteractableRef.location);
                MonoBehaviour.Destroy(c.CardInteractableRef.gameObject);
            }
            //else {
            //    c.CardInteractableRef.SetCardInfo();
            //}
        }
        modifiedCards.Clear();

        DuelEvents.Instance.UpdateUI();
    }

    private void ProcessCard(Card card, BoardCoords pos) {
        if (card == null)
        {
            Debug.Log("Tried to process null card!");
            return;
        }
        // Player cards only attack on player's turn
        if (card.team == currentTeam) {
            foreach(Vector2Int atk in card.AttackDirections) {
                ProcessAttack(card, atk, pos);
            }
        }
        
    }

    


    private void ProcessAttack(Card card, Vector2Int atk, BoardCoords pos) {
        BoardCoords atkDest = pos + new BoardCoords(atk);
        // Attack targeting enemy
        if(board.BeyondEnemyEdge(atkDest) && card.team == Team.Player) {
            enemyStatus.DealDamage(card.Attack);
            return;
        }
        // Attack targeting player
        if(board.BeyondPlayerEdge(atkDest) && card.team == Team.Enemy) {
            playerStatus.DealDamage(card.Attack);
            return;
        }
        // Check for out of bounds 
        if(board.IsOutOfBounds(atkDest)) return;
        // Check for empty tile
        if(board.GetCard(atkDest) == null) return;
        
        // Deal damage
        Card target = board.GetCard(atkDest);
        if(card.team != target.team) {
            target.Health -= card.Attack;
            modifiedCards.Add(target);
        }
    }

    //Use EndTurn() for ending both player and enemy turn, for control and AI purposes
    public void EndTurn() {
        ProcessBoard(); 
        if (currentTeam == Team.Player)
        {
            isEnemyTurn = true;
            currentTeam = Team.Enemy;
            DrawCardEnemy(1);
            enemyStatus.RegenMana();
            EnemyTurn();
        }
        else
        {
            isEnemyTurn = false;
            currentTeam = Team.Player;
            DrawCardPlayer(1);
            playerStatus.RegenMana();
        }
        DuelEvents.Instance.UpdateUI();
    }

    private void EnemyTurn() {
        DrawCardEnemy(1);

        //TODO AI
        //ai.

        List<BoardCoords> legalTiles = new List<BoardCoords>();

        for(int i = 0; i < board.Cols; i++) {
            for(int j = 0; j < board.Rows; j++) {
                BoardCoords pos = new BoardCoords(i, j);
                if (!board.IsOccupied(pos) )
                { 
                    if (settings.RestrictPlacement && j > 0) { // can't place in row closest to player
                        legalTiles.Add(pos);
                    }
                    else {
                        legalTiles.Add(pos);
                    }
                }
            }
        }

        if(legalTiles.Count >= 1) {
            // TODO make it pick a card from the enemy's hand and remove the card from their hand after
            int index = Random.Range(0, playerDeck.CardList.Count);
            Card c = playerDeck.CardList[index];
            c = ScriptableObject.Instantiate(c);
            c.BelongsToPlayer = false;
            c.team = Team.Enemy;
            List<Vector2Int> mirroredAttacks = new List<Vector2Int>();
            foreach(Vector2Int v in c.AttackDirections) {
                mirroredAttacks.Add(new Vector2Int(v.x, -v.y));
            }
            c.AttackDirections = mirroredAttacks;
            GameObject cardObject = MonoBehaviour.Instantiate(ui.TemplateCard.gameObject);

            CardInteractable ci = cardObject.GetComponent<CardInteractable>();
            ci.card = c;
            ci.SetCardInfo();

            BoardCoords randomTile = legalTiles[Random.Range(0, legalTiles.Count)];
            TileInteractable tile = ui.BoardContainer.Tiles[randomTile.ToRowColV2().x, randomTile.ToRowColV2().y];
            c.TileInteractableRef = tile;

            cardObject.transform.SetParent(tile.transform);
            PlayCard(c, randomTile);
        }

        EndTurn();
    }

    public CharStatus CurrentCharStatus()
    {
        if (currentTeam == Team.Enemy)
        {
            return enemyStatus;
        }
        else return playerStatus;
    }

    private void DrawCardPlayer(int count) {
        for(int i = 0; i < count; i++) {
            int index = Random.Range(0, playerDeck.CardList.Count);
            Card c = playerDeck.CardList[index];
            DuelEvents.Instance.DrawCardPlayer(c);
        }

        DuelEvents.Instance.UpdateUI();
    }

    private void DrawCardEnemy(int count) {
        for(int i = 0; i < count; i++) {
            int index = Random.Range(0, enemyDeck.CardList.Count);
            Card c = enemyDeck.CardList[index];
            DuelEvents.Instance.DrawCardEnemy(c);
        }

        DuelEvents.Instance.UpdateUI();
    }
}
