using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDuelAI
{
    private CharStatus status;
    private DuelController dc;
    private DuelSettings settings;

    public BasicDuelAI(CharStatus status, DuelController dc) {
        this.status = status;
        this.dc = dc;
        settings = DuelManager.Instance.Settings;
    }

    public void MakeMove(Board b) {
        List<BoardCoords> legalTiles = GetLegalTiles(b);

        // Pick a random card from the deck
        // TODO make it pick a card from the enemy's hand and remove the card from their hand after
        int index = Random.Range(0, DuelManager.Instance.EnemyDeck.CardList.Count);
        Card c = DuelManager.Instance.EnemyDeck.CardList[index];

        if(legalTiles.Count >= 1 && c.ManaCost <= status.Mana) {
            // Make a specific instance of the chosen card
            c = ScriptableObject.Instantiate(c);
            c.team = Team.Enemy;

            // Create a new CardInteractable
            CardInteractable ci = DuelManager.Instance.UI.GenerateCardInteractable(c);

            // Set TileInteractableRef
            BoardCoords randomTile = legalTiles[Random.Range(0, legalTiles.Count)];
            TileInteractable ti = DuelManager.Instance.UI.FindTileInteractable(randomTile);

            // Play card
            ci.transform.SetParent(ti.transform);
            dc.PlayCard(c, randomTile);
        }
    }

    // Returns a list of spaces on the board that are unrestricted and not occupied by another card
    private List<BoardCoords> GetLegalTiles(Board b) {
        List<BoardCoords> legalTiles = new List<BoardCoords>();

        for(int i = 0; i < b.Cols; i++) {
            for(int j = 0; j < b.Rows; j++) {
                BoardCoords pos = new BoardCoords(i, j);
                if (!b.IsOccupied(pos) )
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

        return legalTiles;
    }
}
