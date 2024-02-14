using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// The main manager for duels/combat, handles all things related to duels
public class DuelManager : MonoBehaviour
{
    public static DuelManager instance;
    //GAME SETTINGS
    public static bool restrictPlacement = false;

    // Sets the size of the board
    public int BoardRows;
    public int BoardCols;

    // The decks of cards used in the duel
    public Deck PlayerDeck;
    public Deck EnemyDeck;

    // "Blank" objects that are intantiated many times
    public GameObject TemplateTile;
    public GameObject TemplateCard;

    // Interface GameObjects
    public GameObject BoardContainer;
    public GameObject Hand;
    private HandInterface handInterface;

    [SerializeField]
    CharStatus playerStatus, enemyStatus;

    // Health

    // Stores information on the current game state
    private Board board;
    private List<Card> modifiedCards = new List<Card>();


    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        board = new Board(BoardRows, BoardCols);
        CheckProperInitialization();
        DuelEvents.instance.UpdateUI();
        InitializeBoard();
        InitializeHand();
    }

    // Diplays health values in the UI



    

    // Create new board
    private void InitializeBoard() {
        BoardContainer.GetComponent<BoardInterface>().CreateBoard(board, TemplateTile);
    }

    // Initalize and draw starting hand
    private void InitializeHand() {
        handInterface = Hand.GetComponent<HandInterface>();
        handInterface.PlayerDeck = PlayerDeck;
        handInterface.TemplateCard = TemplateCard;
        handInterface.Draw(3);
    }

    // Updates the board with the card played at the desired index
    // This only does the data, for UI see PlaceCard in CardInteractable
    public void PlayCard(Card card, int r, int c) {
        // Check out of bounds
        if(r < 0 || r >= board.Rows || c < 0 || c >= board.Cols) {
            Debug.Log("row " + r + ", col " + c + " out of bounds");
            return;
        }
        board.CardSlots[r, c] = card;
    }

    public void EndTurn() {
        ProcessBoard(true); // player turn
        EnemyTurn();
        handInterface.Draw(1);
    }

    public void EnemyTurn() {
        GameObject[,] tileObjects = BoardContainer.GetComponent<BoardInterface>().Tiles;
        List<TileInteractable> legalTiles = new List<TileInteractable>();

        foreach(GameObject g in tileObjects) {
            TileInteractable t = g.GetComponent<TileInteractable>();
            if (t.occupied == false)
            { // can't place in row closest to player
                legalTiles.Add(g.GetComponent<TileInteractable>());
            }

            /*
            if(t.occupied == false && t.location.x < board.Rows-1) { // can't place in row closest to player
                legalTiles.Add(g.GetComponent<TileInteractable>());
            }
            */
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
            GameObject cardObject = Instantiate(TemplateCard);

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
    }

    private void ProcessBoard(bool playerAttack) {
        // Process all cards
        for(int i = 0; i < board.Rows; i++) {
            for(int j = 0; j < board.Cols; j++) {
                if(board.CardSlots[i, j] != null) {
                    ProcessCard(playerAttack, board.CardSlots[i, j], BoardCoords.FromRowCol(i, j));
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
                Destroy(c.CardInteractableRef.gameObject);
            }
            //else {
            //    c.CardInteractableRef.SetCardInfo();
            //}
        }
        modifiedCards.Clear();

        DuelEvents.instance.UpdateUI();
    }

    private void ProcessCard(bool playerAttack, Card card, BoardCoords pos) {
        // Player cards only attack on player's turn
        if(card.BelongsToPlayer && playerAttack) {
            foreach(Vector2Int atk in card.AttackDirections) {
                ProcessAttack(card, atk, pos);
            }
        }
        // Enemy cards only attack on enemy's turn
        else if(!card.BelongsToPlayer && !playerAttack) {
            foreach(Vector2Int atk in card.AttackDirections) {
                ProcessAttack(card, atk, pos);
            }
        }
        
    }

    bool OnEnemyEdge(BoardCoords pos)
    {
        return (pos.y == BoardRows - 1);
    }
    bool BeyondEnemyEdge(BoardCoords pos)
    {
        return (pos.y > BoardRows - 1);
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

    private void CheckProperInitialization() {
        if(PlayerDeck == null || EnemyDeck == null) {
            Debug.Log("Could not start duel, decks are uninitalized");
            return;
        }
        /*
        if(PlayerHealthDisplay == null || EnemyHealthDisplay == null) {
            Debug.Log("Could not start duel, health displays are uninitalized");
            return;
        }
        */
        if(board == null) {
            Debug.Log("Cannot create board, board is uninitialized");
            return;
        }
        if(TemplateTile == null) {
            Debug.Log("Cannot create board, TemplateTile is uninitialized");
            return;
        }
        if(BoardContainer == null) {
            Debug.Log("Cannot create board, BoardContainer is uninitialized");
            return;
        }

        if(BoardContainer.GetComponent<BoardInterface>() == null) {
            Debug.Log("No BoardInterface found on BoardContainer, creating new BoardInterface");
            BoardContainer.AddComponent<BoardInterface>();
        }
        if(BoardContainer.GetComponent<GridLayoutGroup>() == null) {
            Debug.Log("No GridLayoutGroup found on BoardContainer, creating new GridLayoutGroup");
            BoardContainer.AddComponent<GridLayoutGroup>();
        }
        if(BoardContainer.GetComponent<RectTransform>() == null) {
            Debug.Log("No RectTransform found on BoardContainer, creating new RectTransform");
            BoardContainer.AddComponent<RectTransform>();
        }
        if(TemplateTile.GetComponent<TileInteractable>() == null) {
            Debug.Log("No TileInteractable found on TemplateTile, creating new TileInteractable");
            TemplateTile.AddComponent<TileInteractable>();
        }

        if(Hand == null) {
            Debug.Log("Could not create hand, Hand (GameObject) is uninitialized");
            return;
        }
        if(PlayerDeck == null) {
            Debug.Log("Could not create hand, PlayerHand is uninitialized");
            return;
        }
        if(TemplateCard == null) {
            Debug.Log("Could not create hand, TemplateCard is uninitialized");
            return;
        }
        if(TemplateCard.GetComponent<CardInteractable>() == null) {
            Debug.Log("Could not create hand, TemplateCard is has no CardInteractable");
            return;
        }
        if(TemplateCard.GetComponent<CardInteractable>().TemplateArrowPlayer == null ||
            TemplateCard.GetComponent<CardInteractable>().TemplateArrowEnemy == null) {
            Debug.Log("Could not create hand, TemplateCard is has no TemplateArrow");
            return;
        }

        if(Hand.GetComponent<HandInterface>() == null) {
            Debug.Log("No HandInterface found on Hand, creating new HandInterface");
            Hand.AddComponent<HandInterface>();
        }
        if(TemplateCard.GetComponent<CardInteractable>().duelManager == null) {
            Debug.Log("No DuelManager found on TemplateCard, adding this as a DuelManager");
            TemplateCard.GetComponent<CardInteractable>().duelManager = this;
        }
    }
}
