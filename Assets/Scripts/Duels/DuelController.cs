using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player, Enemy
}

public class DuelController
{
    private DuelSettings settings;
    public Board board;
    private PlayerSettings playerSettings, enemySettings;
    private CharStatus playerStatus, enemyStatus;
    private UIManager ui;
    private List<Card> modifiedCards = new List<Card>();
    private Deck playerDeck;
    private Deck enemyDeck;
    private BasicDuelAI ai;
    private Team currentTeam;

    public DuelController(CharStatus player, CharStatus enemy) {
        settings = DuelManager.Instance.Settings;
        ui = DuelManager.Instance.UI;
        playerDeck = DuelManager.Instance.PlayerDeck;
        enemyDeck = DuelManager.Instance.PlayerDeck;

        playerStatus = player;
        enemyStatus = enemy;
        playerStatus.Init(Team.Player);
        enemyStatus.Init(Team.Enemy);

        if (settings.EnemyGoesFirst) currentTeam = Team.Enemy;
        else currentTeam = Team.Player;
        board = new Board(settings.BoardRows, settings.BoardCols);

        
        playerSettings = settings.Player;
        enemySettings = settings.Enemy;
        /* moved to CharStatus awake
        if(settings.SameSettingsForBothPlayers) {
            enemySettings = playerSettings;
        }
        */

        ui.Player.Status = playerStatus;
        ui.Enemy.Status = enemyStatus;

        ai = new BasicDuelAI(enemyStatus, this);
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
        card.CardInteractableRef.PlaceCard(pos); // these should not
        board.PlaceCard(card, pos);              // be two different methods
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

        // Moved implementation to Card.DealDamage
        /*
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
        */

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
            foreach(Attack atk in card.Attacks) {
                ProcessAttack(atk);
            }
        }
        
    }

    private void ProcessAttack(Attack atk) {
        if (atk == null)
        {
            Debug.Log("Tried to process null attack");
            return;
        }
        Card card = atk.card;
        BoardCoords atkDest = card.pos + new BoardCoords(atk.direction);
        // Attack targeting enemy
        if(board.BeyondEnemyEdge(atkDest) && card.team == Team.Player) {
            enemyStatus.DealDamage(atk.damage);
            return;
        }
        // Attack targeting player
        if(board.BeyondPlayerEdge(atkDest) && card.team == Team.Enemy) {
            playerStatus.DealDamage(atk.damage);
            return;
        }
        // Check for out of bounds 
        if(board.IsOutOfBounds(atkDest)) return;
        // Check for empty tile
        if(board.GetCard(atkDest) == null) return;
        
        // Deal damage
        Card target = board.GetCard(atkDest);
        if(card.team != target.team) {
            atk.Hit(target);
            // Remove this?
            modifiedCards.Add(target); 
        }
    }

    // Use EndTurn() for ending both player and enemy turn, for control and AI purposes
    public void EndTurn() {
        ProcessBoard(); 
        if (currentTeam == Team.Player)
        {
            currentTeam = Team.Enemy;
            DrawCardEnemy(1);
            enemyStatus.RegenMana();
            EnemyTurn();
        }
        else
        {
            currentTeam = Team.Player;
            DrawCardPlayer(1);
            playerStatus.RegenMana();
        }

        DuelEvents.Instance.UpdateUI();
    }

    private void EnemyTurn() {
        DrawCardEnemy(1);
        ai.MakeMove(board);
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
