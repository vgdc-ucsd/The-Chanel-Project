using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MctsAI
{
    private CharStatus playerStatus, enemyStatus;

    public MctsAI(CharStatus player, CharStatus enemy) {
        playerStatus = player;
        enemyStatus = enemy;
    }

    public void MakeMove() {
        // TODO MCTS
        Board board = DuelManager.Instance.CurrentBoard;
        testMove(board);
        DuelManager.Instance.MainDuel.ProcessBoard(board, Team.Enemy);
    }

    private void testMove(Board board)
    {
        // Check what moves are available
        List<BoardCoords> legalTiles = GetLegalTiles(board);
        if(legalTiles.Count == 0) return;

        // pick random tile
        BoardCoords randomTile = legalTiles[Random.Range(0, legalTiles.Count)];
        
        // pick random card
        int index = Random.Range(0, enemyStatus.cards.Count);
        Card cardToPlay = enemyStatus.cards[index];

        // play card if enough mana
        if(enemyStatus.Mana >= cardToPlay.ManaCost) { // temp (doesnt deduct mana)
            board.PlaceCard(cardToPlay, randomTile);
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
                    if (DuelManager.Instance.Settings.RestrictPlacement && j > 0) { // can't place in row closest to player
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
