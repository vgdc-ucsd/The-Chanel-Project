using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Unity.VisualScripting; // for testing

public class MctsAI
{
    private class Node
    {
        public int NumWins = 0;
        public int TotalGames = 0;
        public List<Node> Children = new List<Node>();
        public DuelInstance State = null;
        private Node parent;

        public Node(DuelInstance state, Node parent) {
            NumWins = 0;
            TotalGames = 0;
            this.State = state.Clone();
            this.parent = parent;
        }

        public void BackProp(int win) {
            // win is either 1 or -1, 0 on tie

            TotalGames++;
            NumWins += win;
            if(parent != null) {
                parent.BackProp(win);
            }
        }

        public float Score() {
            if(TotalGames != 0) return (float)NumWins / TotalGames;
            else return 0;
        }
    }

    const int MAX_TURNS = 100;
    const int CHILD_COUNT = 3;
    const int MAX_ITERATIONS = 100;

    private List<Node> nodes;

    public IEnumerator MCTS(DuelInstance state) {
        float maxTime = 0.008f; // <= 120 fps
        float iterations = 0;

        Node root = new Node(state, null); // no parent since this is the root node
        float startTime;

        while(iterations < MAX_ITERATIONS) {
            // Advance to next frame if taking too long
            startTime = Time.time;
            while(Time.time - startTime < maxTime) {
                // Selection
                Node selection = GreedySelection(root);

                // Expansion
                selection.Children = RandomExpand(selection);

                // Simulation
                foreach(Node child in selection.Children) {
                    int result = RandomRollout(child);
                    
                    // Backpropagation
                    child.BackProp(result);
                }

                iterations++;
                if(iterations > MAX_ITERATIONS) break;
            }
            yield return null;
        }

        DuelInstance move = FindBestMove(root).State;

        // TODO cleanup
        for(int i = 0; i < move.DuelBoard.Cols; i++) {
            for(int j = 0; j < move.DuelBoard.Rows; j++) {
                BoardCoords pos = new BoardCoords(i,j);
                if (move.DuelBoard.IsOccupied(pos)) {
                    Card c = move.DuelBoard.GetCard(pos);
                    if(c.CardInteractableRef != null) {
                        UnitCardInteractable uci = (UnitCardInteractable) c.CardInteractableRef;
                        uci.card = (UnitCard)c;
                    }
                }
            }
        }

        DuelManager.Instance.EnemyMove(move);
    }

    private Node GreedySelection(Node root) {
        Node parent = root;
        Node selection = root;

        while(parent.Children.Count != 0) {
            parent = FindBestMove(parent);
        }

        return selection;
    }

    private List<Node> RandomExpand(Node parent) {
        // determines how many new children will be created
        List<Node> children = new List<Node>();

        for(int i = 0; i < CHILD_COUNT; i++) {
            Node child = new Node(parent.State, parent);
            PickRandomMove(child.State, Team.Enemy);
            children.Add(child);
        }

        return children;
    }

    private int RandomRollout(Node n) {
        DuelInstance duel = n.State.Clone();
        for(int i = 0; i < MAX_TURNS; i++) {
            // Player move
            PickRandomMove(duel, Team.Player);
            duel.ProcessBoard(Team.Player);

            // On player win
            if(duel.Winner == Team.Player) {
                return -1;
            }

            // Enemy move
            PickRandomMove(duel, Team.Enemy);
            duel.ProcessBoard(Team.Enemy);
            
            // One Enemy win
            if(duel.Winner == Team.Enemy) {
                return 1;
            }
        }

        return 0; // Tie
    }

    private Node FindBestMove(Node parent) {
        Node selection = null;
        float max = -2;
        
        foreach(Node child in parent.Children) {
            if(child.Score() > max) {
                max = child.Score();
                selection = child;
            }
        }

        return selection;
    }

    private void PickRandomMove(DuelInstance duel, Team team) {
        CharStatus status = duel.GetStatus(team);
        List<Card> playableCards = GetPlayableCards(status);
        List<BoardCoords> legalTiles = GetLegalTiles(duel.DuelBoard); // TODO behind spawn line

        int loopCount = 0;
        // If any cards can be played, always play them
        while(playableCards.Count > 0 && legalTiles.Count > 0) {

            // pick random card
            int randomCardIndex = Random.Range(0, playableCards.Count);
            Card randomCard = playableCards[randomCardIndex].Clone();
            

            // pick random tile
            int randomTileIndex = Random.Range(0, legalTiles.Count);
            BoardCoords randomTile = legalTiles[randomTileIndex];

            // play card
            if (randomCard is UnitCard uc)
            {
                duel.DuelBoard.PlayCard(uc, randomTile, status, duel);
            }
            else if (randomCard is SpellCard)
            {
                if (randomCard is ISpellTypeTile sct)
                {
                    sct.CastSpell(duel, duel.DuelBoard.GetRandomTile());
                }
                else if (randomCard is ISpellTypeUnit scu)
                {
                    scu.CastSpell(duel, duel.DuelBoard.GetRandomCard());
                }
            }

            // find legal cards and playable tiles
            playableCards = GetPlayableCards(status);
            legalTiles = GetLegalTiles(duel.DuelBoard);
            loopCount++;
            if (loopCount > 1000)
            {
                UnityEngine.Debug.LogError("Failed to find random move");
                break;
            }
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
            if (c == null)
            {
                UnityEngine.Debug.LogError("card is null");
            }

            if (c.ManaCost <= status.Mana) results.Add(c);
            
        }
        return results;
    }
}
