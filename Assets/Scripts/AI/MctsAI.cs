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

/*     public IEnumerator MakeMove() {
        Board board = DuelManager.Instance.CurrentBoard;
        //MCTS(board); coroutine
        //testCloneSpeed(DuelManager.Instance.MainDuel);

        StartCoroutine(MCTS());

        //testMove(board);
        DuelManager.Instance.MainDuel.ProcessBoard(board, Team.Enemy);
    } */

    public IEnumerator MCTS(DuelInstance state) {
        float maxTime = 0.008f; // <= 120 fps
        float maxIterations = 100;
        float iterations = 0;

        Node root = new Node(state, null); // no parent since this is the root node
        float startTime = Time.time;

        while(iterations < maxIterations) {
            // Advance to next frame if taking too long
            while(Time.time - startTime < maxTime) {
                // Selection
                Node selection = GreedySelection(root);

                // Expansion
                List<Node> children = RandomExpand(selection);

                // Simulation
                foreach(Node child in children) {
                    int result = RandomRollout(child);
                    
                    // Backpropagation
                    child.BackProp(result);
                }

                iterations++;
            }

            startTime = Time.time;
            yield return null;
        }

        DuelInstance move = FindBestMove(root).State;
        DuelManager.Instance.EnemyMove(move);
    }

    private Node GreedySelection(Node root) {
        return null;
    }

    private List<Node> RandomExpand(Node parent) {
        return null;
    }

    private int RandomRollout(Node n) {
        return 0;
    }

    private void Simulate(DuelInstance duel, Board board) {
        for(int i = 0; i < MAX_GAMES; i++) {
            PickRandomMove(duel, board, Team.Player);
            //duel.ProcessBoard(duel.Team.Player)

        }
    }

    private Node FindBestMove(Node root) {
        return null;
    }

    private void PickRandomMove(DuelInstance duel, Board board, Team team) {
        CharStatus status;
        if(team == Team.Player) {
            status = duel.PlayerStatus;
        }
        else {
            status = duel.EnemyStatus;
        }

        List<Card> playableCards = GetPlayableCards(status);
        List<BoardCoords> legalTiles = GetLegalTiles(board); // TODO behind spawn line

        while(playableCards.Count > 0 && legalTiles.Count > 0) {
            int randomCardIndex = Random.Range(0, playableCards.Count);
            Card randomCard = playableCards[randomCardIndex];
            
            int randomTileIndex = Random.Range(0, legalTiles.Count);
            BoardCoords randomTile = legalTiles[randomCardIndex];

            // play card

            playableCards = GetPlayableCards(status);
            legalTiles = GetLegalTiles(board);
        }
    }

    private void testMove(Board board)
    {
        // Check what moves are available
        List<BoardCoords> legalTiles = GetLegalTiles(board);
        if(legalTiles.Count == 0) return;

        // pick random tile
        BoardCoords randomTile = legalTiles[Random.Range(0, legalTiles.Count)];
        
        // pick random card
        int index = Random.Range(0, enemyStatus.Cards.Count);
        UnitCard cardToPlay = (UnitCard)enemyStatus.Cards[index]; // TODO
        //UnitCard cardToPlay = null;
        // TODO

        // play card if enough mana
        if(enemyStatus.Mana >= cardToPlay.ManaCost) { // temp (doesnt deduct mana)
            //board.PlayCard(cardToPlay, randomTile, enemyStatus);
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

    private List<Card> GetPlayableCards(CharStatus status) {
        List<Card> results = new List<Card>();
        if(status.Mana == 0) return results;
        foreach(Card c in status.Cards) {
            if(c.ManaCost <= status.Mana) results.Add(c);
        }
        return results;
    }
}
