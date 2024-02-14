using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDuelAI
{
    /* public void EnemyTurn() {
        GameObject[,] tileObjects = BoardContainer.GetComponent<BoardInterface>().Tiles;
        List<TileInteractable> legalTiles = new List<TileInteractable>();

        foreach(GameObject g in tileObjects) {
            TileInteractable t = g.GetComponent<TileInteractable>();
            if (t.occupied == false)
            { // can't place in row closest to player
                legalTiles.Add(g.GetComponent<TileInteractable>());
            }

       
            //if(t.occupied == false && t.location.x < board.Rows-1) { // can't place in row closest to player
            //    legalTiles.Add(g.GetComponent<TileInteractable>());
            //}
           
        }

        if(legalTiles.Count >= 1) {
            int index = UnityEngine.Random.Range(0, PlayerDeck.CardList.Count);
            Card c = PlayerDeck.CardList[index];
            c = ScriptableObject.Instantiate(c);
            c.BelongsToPlayer = false;
            List<Vector2Int> mirroredAttacks = new List<Vector2Int>();
            foreach(Vector2Int v in c.AttackDirections) {
                mirroredAttacks.Add(new Vector2Int(v.x, -v.y));
            }
            c.AttackDirections = mirroredAttacks;
            GameObject cardObject = Instantiate(TemplateCard.gameObject);

            CardInteractable ci = cardObject.GetComponent<CardInteractable>();
            ci.card = c;
            ci.SetCardInfo();

            TileInteractable randomTile = legalTiles[Random.Range(0, legalTiles.Count)];
            c.TileInteractableRef = randomTile;
            ci.PlaceCard(randomTile);
            cardObject.transform.SetParent(randomTile.transform);

            PlayCard(c, randomTile.location.x, randomTile.location.y);
        }

        ProcessBoard(false); // enemy attack
    } */
}
