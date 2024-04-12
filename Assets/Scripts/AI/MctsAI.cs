using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics; // for testing

public class MctsAI
{
    private class Node
    {
        public int NumWins = 0;
        public int TotalGames = 0;
        public DuelInstance State = null;
        private Node parent;
        private List<Node> children = new List<Node>();

        public Node(DuelInstance state, Node parent) {
            NumWins = 0;
            TotalGames = 0;
            this.State = state.Clone();
            this.parent = parent;
        }

        public void BackProp(int win) {
            // win is either 1 or 0
            // maybe add case for tie

            TotalGames++;
            NumWins += win;
            if(parent != null) {
                parent.BackProp(win);
            }
        }
    }

    const int MAX_GAMES = 100;

    private CharStatus playerStatus, enemyStatus;
    private List<Node> nodes;

    public MctsAI(CharStatus player, CharStatus enemy) {
        playerStatus = player;
        enemyStatus = enemy;
    }

    public void MakeMove() {
        Board board = DuelManager.Instance.CurrentBoard;
        //MCTS(board); coroutine
        //testCloneSpeed(DuelManager.Instance.MainDuel);
        //testMove(board);
        DuelManager.Instance.MainDuel.ProcessBoard(board, Team.Enemy);
    }

    private IEnumerable MCTS(Board board) {
        // Selection

        // Expansion

        // Simulation

        // Backpropagation
        yield return null;
    }

    private void Simulate(DuelInstance duel) {
        for(int i = 0; i < MAX_GAMES; i++) {
            PickRandomMove(duel, Team.Player);
            //duel.ProcessBoard(duel.Team.Player)

        }
    }

    private void PickRandomMove(DuelInstance duel, Team team) {

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
        UnitCard cardToPlay = enemyStatus.cards[index];

        // play card if enough mana
        if(enemyStatus.Mana >= cardToPlay.ManaCost) { // temp (doesnt deduct mana)
            board.PlaceCard(cardToPlay, randomTile);
        }
    }

    private void testCloneSpeed(DuelInstance di) {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        di.Clone();
        sw.Stop();
        UnityEngine.Debug.Log("time: " + sw.Elapsed);
    }

    private void testSimulationSpeed() {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        //di.Clone();
        sw.Stop();
        UnityEngine.Debug.Log("time: " + sw.Elapsed);
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
