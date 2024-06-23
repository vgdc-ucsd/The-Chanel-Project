using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Unity.VisualScripting; // for testing

public class MctsAI
{

    public static MctsAI Instance;
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

    const int MAX_TURNS = 3;
    const int CHILD_COUNT = 4;
    const int MAX_ITERATIONS = 200;

    const int WEIGHTED_MAX_TURNS = 3;
    const int INITIAL_CHILD_COUNT = 20;

    const int MAX_CHILD_PER_NODE = 500;

    // probability that the AI will consider these actions
    float aiMovementChance = 0.3f;
    float aiPushChance = 0.7f;
    float aiDrawChance = 0.1f;

    // probability that the AI will anticipate the player doing these actions
    float playerMovementChance = 0.3f;
    float playerPushChance = 0.7f;
    float playerDrawChance = 0.1f;

    // how much the AI likes enemy/player cards positioned at corresponding y value
    // multiplied by mana cost of card
    static float[] ENEMY_CARD_POSITIONING_WEIGHTS = { 30, 18, 16, 14 };
    static float[] PLAYER_CARD_POSITIONING_WEIGHTS = { 10, 12, 25, 80 };

    const int CARD_WEIGHTS = 2;

    const float STATUS_DAMAGE_WEIGHT = 80; // weight bias/penalty per pt of damage taken by player/enemy


    private List<Node> nodes;

    public MctsAI(float aggression, float defense)
    {
        aiPushChance = aggression;
        playerPushChance = defense;
    }

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

                List<Node> curBatch = new List<Node>();

                // Expansion
                if (selection == root)
                {
                    curBatch = RandomExpand(selection, INITIAL_CHILD_COUNT);
                    selection.Children.AddRange(curBatch);
                }
                else
                {
                    curBatch = RandomExpand(selection, CHILD_COUNT);
                    selection.Children.AddRange(curBatch);
                }

                // Simulation
                foreach(Node child in curBatch) {
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
        //UnityEngine.Debug.Log($"Iterations: {iterations}");

        // Some Scores
        System.Random rnd = new System.Random();
        /*
        if (root.Children.Count > 0)
        {
            UnityEngine.Debug.Log($"Score: {root.Children[rnd.Next(0, root.Children.Count)].Score()}");
            UnityEngine.Debug.Log($"Score: {root.Children[rnd.Next(0, root.Children.Count)].Score()}");
            UnityEngine.Debug.Log($"Score: {root.Children[rnd.Next(0, root.Children.Count)].Score()}");
            UnityEngine.Debug.Log($"Score: {root.Children[rnd.Next(0, root.Children.Count)].Score()}");
            UnityEngine.Debug.Log($"Score: {root.Children[rnd.Next(0, root.Children.Count)].Score()}");
        }
        */
        
        //UnityEngine.Debug.Log("Children: " + root.Children.Count);

        Node bestMove = FindBestMove(root);
        DuelInstance move = bestMove.State;

        UpdateUnitCardInteractableRefs(move);

        //UnityEngine.Debug.Log("Final Move Score " + bestMove.Score());

        DuelManager.Instance.EnemyMove(move);
    }

    private Node GreedySelection(Node root) {
        Node parent = root;
        Node selection = root;

        while(parent.Children.Count != 0) {
            Node curMove = FindBestMove(parent);
            if (curMove == null || curMove.Children == null)
            {
                UnityEngine.Debug.LogError("null node");
            }
            if (parent.Children.Count < MAX_CHILD_PER_NODE){
                return parent;
            }
            parent = curMove;
            selection = curMove;
        }

        return selection;
    }

    private List<Node> RandomExpand(Node parent, int childCount) {
        List<Node> children = new List<Node>();

        for(int i = 0; i < childCount; i++) {
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
                score -= 80;
            }

            // Enemy move
            PickRandomMove(duel, Team.Enemy);
            duel.ProcessBoard(Team.Enemy);

            // One Enemy win
            if (duel.Winner == Team.Enemy)
            {
                score += 80;
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
            float cardValue = CARD_WEIGHTS * card.ManaCost;
            if (card.CurrentTeam == Team.Player) 
                score -= PLAYER_CARD_POSITIONING_WEIGHTS[card.Pos.y] + cardValue;
            else if (card.CurrentTeam == Team.Enemy) 
                score += ENEMY_CARD_POSITIONING_WEIGHTS[card.Pos.y] + cardValue;
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

        float movementChance, pushChance, drawChance;

        if (team == Team.Enemy)
        {
            movementChance = aiMovementChance;
            pushChance = aiPushChance;
            drawChance = aiDrawChance;
        }
        else
        {
            movementChance = playerMovementChance;
            pushChance = playerPushChance;
            drawChance = playerDrawChance;
        }


        int loopCount = 0;
        // If any cards can be played, always play them
        while((playableCards.Count > 0 && legalTiles.Count > 0) || moveableCards.Count > 0) {
            
            // roll to draw a card after every action
            if (status.CanDrawCard() && Random.value < drawChance)
            {
                duel.DrawCardWithMana(team);
            }

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
                else if (randomCard is SpellCard sc)
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
            
            // if out of legal playable cards, use all mana to draw cards

            while (playableCards.Count == 0 && status.CanDrawCard() )
            {
                duel.DrawCardWithMana(team);
            }


            loopCount++;
            if (loopCount > 1000)
            {
                UnityEngine.Debug.LogWarning("Failed to find random move");
                break;
            }
        }

        while (playableCards.Count == 0 && status.CanDrawCard())
        {
            duel.DrawCardWithMana(team);
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
    public List<BoardCoords> GetLegalTiles(Board b) {
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

    public List<Card> GetPlayableCards(DuelInstance duel, CharStatus status) {
        UnityEngine.Debug.Log("b1");
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
        UnityEngine.Debug.Log("b2");
        return results;
    }

    public List<UnitCard> GetMovableCards(Board board, Team team) {
        List<UnitCard> results = new List<UnitCard>();
        foreach(UnitCard uc in board.CardSlots) {
            if (uc != null) {
                if(uc.CurrentTeam == team && board.GetEmptyAdjacentTiles(uc.Pos).Count > 0 && uc.CanMove) results.Add(uc);
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
