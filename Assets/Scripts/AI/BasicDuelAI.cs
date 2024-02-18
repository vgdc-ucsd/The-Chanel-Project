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

        if(legalTiles.Count >= 1) {
            // TODO make it pick a card from the enemy's hand and remove the card from their hand after
            int index = Random.Range(0, DuelManager.Instance.EnemyDeck.CardList.Count);
            Card c = DuelManager.Instance.EnemyDeck.CardList[index];
            
            if(c.ManaCost <= status.Mana) {
                c = ScriptableObject.Instantiate(c);
                c.team = Team.Enemy;
                List<Vector2Int> mirroredAttacks = new List<Vector2Int>();
                foreach(Vector2Int v in c.AttackDirections) {
                    mirroredAttacks.Add(new Vector2Int(v.x, -v.y));
                }
                c.AttackDirections = mirroredAttacks;
                GameObject cardObject = MonoBehaviour.Instantiate(DuelManager.Instance.UI.TemplateCard.gameObject);

                CardInteractable ci = cardObject.GetComponent<CardInteractable>();
                ci.card = c;
                ci.SetCardInfo();

                BoardCoords randomTile = legalTiles[Random.Range(0, legalTiles.Count)];
                TileInteractable tile = DuelManager.Instance.UI.BoardContainer.Tiles[randomTile.ToRowColV2().x, randomTile.ToRowColV2().y];
                c.TileInteractableRef = tile;

                cardObject.transform.SetParent(tile.transform);
                dc.PlayCard(c, randomTile);
            }
        }
    }
}
