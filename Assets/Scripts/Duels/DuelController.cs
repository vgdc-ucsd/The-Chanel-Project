using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelController
{
    private bool isEnemyTurn;
    public DuelSettings Settings;
    private Board board;
    private PlayerSettings playerSettings, enemySettings;
    private CharStatus playerStatus, enemyStatus;
    private UIManager UI;
    private List<Card> modifiedCards = new List<Card>();
    private Deck playerDeck;
    private Deck enemyDeck;

    public DuelController(DuelSettings settings, UIManager UI, Deck playerDeck, Deck enemyDeck) {
        Settings = settings;

        isEnemyTurn = settings.EnemyGoesFirst;
        board = new Board(settings.BoardRows, settings.BoardCols);
        this.playerDeck = playerDeck;
        this.enemyDeck = enemyDeck;
        playerSettings = settings.Player;
        enemySettings = settings.Enemy;

        if(settings.SameSettingsForBothPlayers) {
            enemySettings = playerSettings;
        }
        playerStatus = new CharStatus(playerSettings);
        enemyStatus = new CharStatus(enemySettings);

        UI.Player.Status = playerStatus;
        UI.Enemy.Status = enemyStatus;
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

        DuelEvents.instance.UpdateUI();
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
        return (pos.y == Settings.BoardRows - 1);
    }
    bool BeyondEnemyEdge(BoardCoords pos)
    {
        return (pos.y > Settings.BoardRows - 1);
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
        int index = UnityEngine.Random.Range(0, playerDeck.CardList.Count);
        Card c = playerDeck.CardList[index];
        DuelEvents.instance.DrawCardPlayer(c);
        DuelEvents.instance.UpdateUI();
    }

    private void EnemyTurn() {
        return;
    }
}
