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
            if (parent != null) {
                parent.BackProp(win);
            }
        }

        public float Score() {
            if (TotalGames != 0) return (float)NumWins / TotalGames;
            else return 0;
        }
    }

    const int MAX_TURNS = 100;
    const int CHILD_COUNT = 6;
    const int MAX_ITERATIONS = 120;

    const int WEIGHTED_MAX_TURNS = 10;

    // how much the AI likes enemy/player cards positioned at corresponding y value
    // multiplied by mana cost of card
    static float[] ENEMY_CARD_POSITIONING_WEIGHTS = { 30, 20, 3, 2 };
    static float[] PLAYER_CARD_POSITIONING_WEIGHTS = { -8, -10, -20, -50 };

    const float STATUS_DAMAGE_WEIGHT = 50; // weight bias/penalty per pt of damage taken by player/enemy


    private List<Node> nodes;

    public IEnumerator MCTS(DuelInstance state) {
        float maxTime = 0.008f; // <= 120 fps
        float iterations = 0;

        Node root = new Node(state, null); // no parent since this is the root node
        float startTime;

        while(iterations < MAX_ITERATIONS) {
            // Advance to next frame if taking too long
            startTime = Time.realtimeSinceStartup;
            while(Time.realtimeSinceStartup - startTime < maxTime) {
                // Selection
                Node selection = GreedySelection(root);

                // Expansion
                selection.Children = RandomExpand(selection);

                // Simulation
                foreach(Node child in selection.Children) {
                    //int result = RandomRollout(child);
                    int result = (int)WeightedRollout(child);
                    // Backpropagation
                    child.BackProp(result);
                }

                iterations++;
                if(iterations > MAX_ITERATIONS) break;
            }
            yield return null;
        }

        DuelInstance move = FindBestMove(root).State;

        UpdateUnitCardInteractableRefs(move);

        DuelManager.Instance.EnemyMove(move);
    }

    private Node GreedySelection(Node root) {
        Node parent = root;
        Node selection = root;
        



        while(parent.Children.Count != 0) {
            parent = FindBestMove(parent);
            if (parent == null || parent.Children == null)
            {
                UnityEngine.Debug.LogError("null node");
            }
        }

        return selection;
    }

    private List<Node> RandomExpand(Node parent) {
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

    private float WeightedRollout(Node n)
    {
        float score = 0;
        DuelInstance duel = n.State.Clone();
        for (int i = 0; i < WEIGHTED_MAX_TURNS; i++)
        {
            // Player move
            PickRandomMove(duel, Team.Player);
            duel.ProcessBoard(Team.Player);

            // On player win
            if (duel.Winner == Team.Player)
            {
                score -= 10000;
            }

            // Enemy move
            PickRandomMove(duel, Team.Enemy);
            duel.ProcessBoard(Team.Enemy);

            // One Enemy win
            if (duel.Winner == Team.Enemy)
            {
                score += 10000;
                
            }
        }

        score += EvaluatePosition(duel, n.State);


        return score; // Tie
    }

    private float EvaluatePosition(DuelInstance duel, DuelInstance originalState)
    {
        float score = 0f;
        foreach (UnitCard card in duel.DuelBoard.GetAllCards())
        {
            if (card.CurrentTeam == Team.Player) 
                score += PLAYER_CARD_POSITIONING_WEIGHTS[card.Pos.y] * card.ManaCost * card.Health;
            else if (card.CurrentTeam == Team.Enemy) 
                score += ENEMY_CARD_POSITIONING_WEIGHTS[card.Pos.y] * card.ManaCost * card.Health;
        }
        score += (originalState.GetStatus(Team.Player).Health - duel.GetStatus(Team.Player).Health) * STATUS_DAMAGE_WEIGHT;
        score -= (originalState.GetStatus(Team.Enemy).Health - duel.GetStatus(Team.Enemy).Health) * STATUS_DAMAGE_WEIGHT;

        return score;
    }

    private Node FindBestMove(Node parent) {
        Node selection = null;
        float max = float.MinValue;
        
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
        List<Card> playableCards = GetPlayableCards(duel, status);
        List<BoardCoords> legalTiles = GetLegalTiles(duel.DuelBoard); // TODO behind spawn line
        List<UnitCard> moveableCards = GetMovableCards(duel.DuelBoard, team);
        float movementChance = 0.4f;
        float pushChance = 0.5f;

        int loopCount = 0;
        // If any cards can be played, always play them
        while((playableCards.Count > 0 && legalTiles.Count > 0) || moveableCards.Count > 0) {
            
            // remove cards if they are no longer movable
            for(int i = 0; i < moveableCards.Count; i++) {
                if(duel.DuelBoard.GetEmptyAdjacentTiles(moveableCards[i].Pos).Count == 0) {
                    moveableCards.Remove(moveableCards[i]);
                    i--;
                }
            }

            if(moveableCards.Count > 0) {
                // pick random card to move
                int randomCardIndex = Random.Range(0, moveableCards.Count);
                UnitCard randomCard = moveableCards[randomCardIndex];


                // decide to move or not

                if (Random.Range(0.0f, 1.0f) < pushChance)
                {
                    if (!duel.DuelBoard.IsOccupied(randomCard.Pos + new BoardCoords(0, -1)))
                    {
                        duel.DuelBoard.MoveCard(randomCard, randomCard.Pos + new BoardCoords(0, -1), duel);
                    }
                }

                else if (Random.Range(0.0f, 1.0f) < movementChance) {
                    // pick random tile
                    List<BoardCoords> availableTiles = duel.DuelBoard.GetEmptyAdjacentTiles(randomCard.Pos);
                    int randomTileIndex = Random.Range(0, availableTiles.Count);
                    BoardCoords randomTile = availableTiles[randomTileIndex];

                    // move to random tile
                    duel.DuelBoard.MoveCard(randomCard, randomTile, duel);
                }

                

                moveableCards.Remove(randomCard);
            }

            // find legal cards and playable tiles
            playableCards = GetPlayableCards(duel, status);
            legalTiles = GetLegalTiles(duel.DuelBoard);

            if(playableCards.Count > 0 && legalTiles.Count > 0) {
                // pick random card to play
                int randomCardIndex = Random.Range(0, playableCards.Count);
                Card randomCard = playableCards[randomCardIndex];

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
                    bool success = false;
                    if (randomCard is ISpellTypeTile sct)
                    {
                        success = sct.CastSpell(duel, duel.DuelBoard.GetRandomTile());
                    }
                    else if (randomCard is ISpellTypeAlly sca)
                    {
                        success = sca.CastSpell(duel, duel.DuelBoard.GetRandomCardOfTeam(randomCard.CurrentTeam));
                    }
                    else if (randomCard is ISpellTypeEnemy sce)
                    {
                        success = sce.CastSpell(duel, duel.DuelBoard.GetRandomCardOfTeam(CharStatus.OppositeTeam(randomCard.CurrentTeam)));
                    }
                    else if (randomCard is ISpellTypeUnit scu)
                    {
                        success = scu.CastSpell(duel, duel.DuelBoard.GetRandomCard());
                    }


                }
            }
            
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

        for(int i = 0; i < b.Rows; i++) {
            for(int j = 0; j < b.Cols; j++) {
                BoardCoords pos = BoardCoords.FromRowCol(new Vector2Int(i, j));
                if (!b.IsOccupied(pos) )
                { 
                    if (DuelManager.Instance.Settings.RestrictPlacement && i < 2) { // can't place in two rows closest to player
                        legalTiles.Add(pos);
                    }
                }
            }
        }

        return legalTiles;
    }

    private List<Card> GetPlayableCards(DuelInstance duel, CharStatus status) {
        List<Card> results = new List<Card>();
        if(status.Mana == 0) return results;
        foreach(Card c in status.Cards) {
            if (c == null)
            {
                UnityEngine.Debug.LogError("card is null");
            }

            if (c is ISpellTypeUnit)
            { 
                if (c is ISpellTypeAlly)
                {
                    if (duel.DuelBoard.GetCardsOfTeam(c.CurrentTeam).Count == 0) continue;
                }

                if (c is ISpellTypeEnemy)
                {
                    if (duel.DuelBoard.GetCardsOfTeam(CharStatus.OppositeTeam(c.CurrentTeam)).Count == 0) continue;
                }
            }


            if (c.ManaCost <= status.Mana) results.Add(c);
            
        }
        return results;
    }

    private List<UnitCard> GetMovableCards(Board board, Team team) {
        List<UnitCard> results = new List<UnitCard>();
        foreach(UnitCard uc in board.CardSlots) {
            if (uc != null) {
                if(uc.CurrentTeam == team && board.GetEmptyAdjacentTiles(uc.Pos).Count > 0) results.Add(uc);
            }
        }
        return results;
    }

    private void UpdateUnitCardInteractableRefs(DuelInstance move) {
        // Loops over every card and updates its cardinteractable to reference itself

        foreach(UnitCard uc in move.DuelBoard.CardSlots) {
            if(uc != null && uc.CardInteractableRef != null) {
                UnitCardInteractable uci = uc.UnitCardInteractableRef;
                uci.card = uc;
            }
        }
    }
}
