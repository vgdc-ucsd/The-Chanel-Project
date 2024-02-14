using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public DuelController() {
        settings = DuelManager.Instance.Settings;
        ui = DuelManager.Instance.UI;
        playerDeck = DuelManager.Instance.PlayerDeck;
        enemyDeck = DuelManager.Instance.PlayerDeck;

        isEnemyTurn = settings.EnemyGoesFirst;
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
    // This only does the data, for UI see PlaceCard in CardInteractable
    // TODO convert coordinates
    public void PlayCard(Card card, int r, int c) {
        // Check out of bounds
        if(r < 0 || r >= board.Rows || c < 0 || c >= board.Cols) {
            Debug.Log("row " + r + ", col " + c + " out of bounds");
            return;
        }
        board.CardSlots[r, c] = card;
    }

    private void ProcessBoard() {
        // Process all cards
        for(int i = 0; i < board.Rows; i++) {
            for(int j = 0; j < board.Cols; j++) {
                if(board.CardSlots[i, j] != null) {
                    ProcessCard(board.CardSlots[i, j], BoardCoords.FromRowCol(i, j));
                }
            }
        }

        // Update cards that were modified
        foreach(Card c in modifiedCards) {
            if(c.Health <= 0) {
                int cardRow = c.TileInteractableRef.location.x;
                int cardCol = c.TileInteractableRef.location.y;
                c.TileInteractableRef.occupied = false;
                board.CardSlots[cardRow, cardCol] = null;
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
        // Player cards only attack on player's turn
        if(card.BelongsToPlayer && !isEnemyTurn) {
            foreach(Vector2Int atk in card.AttackDirections) {
                ProcessAttack(card, atk, pos);
            }
        }
        // Enemy cards only attack on enemy's turn
        else if(!card.BelongsToPlayer && isEnemyTurn) {
            foreach(Vector2Int atk in card.AttackDirections) {
                ProcessAttack(card, atk, pos);
            }
        }
        
    }

    bool OnEnemyEdge(BoardCoords pos)
    {
        return (pos.y == settings.BoardRows - 1);
    }
    bool BeyondEnemyEdge(BoardCoords pos)
    {
        return (pos.y > settings.BoardRows - 1);
    }

    bool OnPlayerEdge(BoardCoords pos)
    {
        return (pos.y == 0);
    }
    bool BeyondPlayerEdge(BoardCoords pos)
    {
        return (pos.y < 0);
    }



    private void ProcessAttack(Card card, Vector2Int atk, BoardCoords pos) {
        BoardCoords atkDest = pos + new BoardCoords(atk);

        // Attack targeting enemy
        if(BeyondEnemyEdge(atkDest) && card.BelongsToPlayer) {
            enemyStatus.DealDamage(card.Attack);
            return;
        }
        // Attack targeting player
        if(BeyondPlayerEdge(atkDest) && !card.BelongsToPlayer) {
            playerStatus.DealDamage(card.Attack);
            return;
        }
        // Check for out of bounds 
        if(board.IsOutOfBounds(atkDest)) return;
        // Check for empty tile
        if(board.GetCardAtPos(atkDest) == null) return;
        
        // Deal damage
        Card target = board.GetCardAtPos(atkDest);
        if(card.BelongsToPlayer != target.BelongsToPlayer) {
            target.Health -= card.Attack;
            modifiedCards.Add(target);
        }
    }

    public void EndTurn() {
        ProcessBoard(); 
        EnemyTurn();
        DrawCardPlayer(1);
    }

    private void EnemyTurn() {
        isEnemyTurn = true;
        DrawCardEnemy(1);

        //TODO AI
        //ai.

        List<BoardCoords> legalTiles = new List<BoardCoords>();

        for(int i = 0; i < board.Rows; i++) {
            for(int j = 0; j < board.Cols; j++) {
                if (board.CardSlots[i, j] == null) { 
                    if(settings.RestrictPlacement && i < board.Rows-1) { // can't place in row closest to player
                        legalTiles.Add(BoardCoords.FromRowCol(i, j));
                    }
                    else {
                        legalTiles.Add(BoardCoords.FromRowCol(i, j));
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
            ci.PlaceCard(tile);
            cardObject.transform.SetParent(tile.transform);

            PlayCard(c, tile.location.x, tile.location.y);
        }

        ProcessBoard();
        isEnemyTurn = false;
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
