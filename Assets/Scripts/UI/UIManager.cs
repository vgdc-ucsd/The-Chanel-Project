using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // "Blank" objects that are intantiated many times
    public TileInteractable TemplateTile;
    //public CardInteractable TemplateCard;
    public UnitCardInteractable TemplateUnitCard;
    public SpellCardInteractable TemplateSpellCard;
    public GameObject TemplateCardBack;

    // Interface GameObjects
    public BoardInterface BoardContainer;
    public HandInterface Hand;
    public HandInterface EnemyHand;

    // Player UIs
    public PlayerUI Player;
    public PlayerUI Enemy;

    // Card piles
    public Transform PlayerDraw;
    public Transform EnemyDraw;
    public Transform PlayerDiscard;
    public Transform EnemyDiscard;

    private List<GameObject> PlayerDrawObjects;
    private List<GameObject> EnemyDrawObjects;
    private List<GameObject> PlayerDiscardObjects;
    private List<GameObject> EnemyDiscardObjects;

    // Card View Panel
    public GameObject DeckViewObject; // root object
    public GameObject DeckViewContainer; // object that can scale (no raycast blocker)

    public CardInfoPanel InfoPanel;
    public Canvas MainCanvas;
    public Image EndTurnButton;
    public Image EnemyArt;
    public Sprite PlayerUnitCardBorder;
    public Sprite PlayerSpellCardBorder;
    public Sprite EnemyUnitCardBorder;
    public Sprite EnemySpellCardBorder;

    public static UIManager Instance;

    void Awake() {
        // Singleton
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Tried to create more than one instance of the UIManager singleton");
            Destroy(this);
        }
        else Instance = this;
    }

    public void Initialize() {
        BoardContainer.CreateBoard();

        PlayerDrawObjects = new List<GameObject>();
        PlayerDiscardObjects = new List<GameObject>();
        EnemyDrawObjects = new List<GameObject>();
        EnemyDiscardObjects = new List<GameObject>();

        foreach(Card c in DuelManager.Instance.PlayerDeck.CardList) {
            GameObject cardBack = Instantiate(TemplateCardBack);
            cardBack.transform.SetParent(PlayerDraw);
            cardBack.transform.localPosition = Vector3.zero;
            cardBack.transform.localScale = Vector3.one;
            PlayerDrawObjects.Add(cardBack);
        }
        foreach(Card c in DuelManager.Instance.EnemyDeck.CardList) {
            GameObject cardBack = Instantiate(TemplateCardBack);
            cardBack.transform.SetParent(EnemyDraw);
            cardBack.transform.localPosition = Vector3.zero;
            cardBack.transform.localScale = Vector3.one;
            EnemyDrawObjects.Add(cardBack);
        }

        if(PersistentData.Instance.CurrentEncounter.EnemyArt != null) {
            EnemyArt.sprite = PersistentData.Instance.CurrentEncounter.EnemyArt;
        }
    }

    public void UpdateStatus(DuelInstance state) {
        Player.UpdateUI(state.PlayerStatus);
        Enemy.UpdateUI(state.EnemyStatus);
    }

    /* !! README !!
     * The GenerateCardInteractable method used in inventory screen has been moved
     * to InventoryUI, please make changes there for inventory CardInteractables.
     */
    public CardInteractable GenerateCardInteractable(Card c) {
        if(c is UnitCard) {
            UnitCardInteractable ci = Instantiate(TemplateUnitCard);
            ci.card = (UnitCard) c;
            ci.SetCardInfo();
            ci.mode = CIMode.Duel;
            ci.ResetArrows();

            if (c.CurrentTeam == Team.Player) {
                ci.handInterface = Hand;
                ci.image.sprite = PlayerUnitCardBorder;
            }
            else if (c.CurrentTeam == Team.Enemy) {
                ci.handInterface = EnemyHand;
                ci.image.sprite = EnemyUnitCardBorder;
            }
            else {
                ci.image.sprite = PlayerUnitCardBorder;
                return ci;
            }

            ci.handInterface.cardObjects.Add(ci.gameObject);
            return ci;
        }
        else if(c is SpellCard) {
            SpellCardInteractable ci = Instantiate(TemplateSpellCard);
            ci.card = (SpellCard)c;
            ci.SetCardInfo();
            ci.mode = CIMode.Duel;

            if(c.CurrentTeam == Team.Player) {
                ci.image.sprite = PlayerSpellCardBorder;
                ci.handInterface = Hand;
            }
            else if (c.CurrentTeam == Team.Enemy) {
                ci.image.sprite = EnemySpellCardBorder;
                ci.handInterface = EnemyHand;
            }
            else {
                ci.image.sprite = PlayerSpellCardBorder;
                return ci;
            }

            ci.handInterface.cardObjects.Add(ci.gameObject);
            return ci;
        }
        else {
            Debug.LogError("Unidentified card type");
            return null;
        }
    }

    public TileInteractable FindTileInteractable(BoardCoords bc) {
        if(bc.ToRowColV2().x >= 0 && bc.ToRowColV2().x < BoardContainer.Tiles.GetLength(0)
        && bc.ToRowColV2().y >= 0 && bc.ToRowColV2().y < BoardContainer.Tiles.GetLength(1)) {
            TileInteractable tile = BoardContainer.Tiles[bc.ToRowColV2().x, bc.ToRowColV2().y];
            return tile;
        }
        else {
            Debug.LogWarning("No tile exists at x=" + bc.x + ", y=" + bc.y);
            return null;
        }
    }

    public void EnablePlayerControlUI(bool enable) {
        if(enable) {
            EndTurnButton.color = Color.white;
            DrawCardButton.Instance.CanInteract = true;
        }
        else {
            EndTurnButton.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            DrawCardButton.Instance.CanInteract = false;
            DrawCardButton.Instance.StopCardHover();
        }
    }

    public void ShowDeckView(List<Card> cards) {
        DeckViewObject.SetActive(true);

        for(int i = 0; i < Mathf.Min(12 , cards.Count); i++) {
            Card c = cards[i].Clone();
            c.CurrentTeam = Team.Neutral;
            CardInteractable ci = GenerateCardInteractable(c);
            ci.CanInteract = false;
            ci.transform.SetParent(DeckViewContainer.transform.GetChild(i));
            ci.transform.localScale = Vector3.one;
            ci.transform.localPosition = Vector3.zero;
        }

        foreach(Transform obj in DeckViewContainer.transform) {
            obj.localScale = Vector3.zero;
            AnimationManager.Instance.BounceScaleAnimation(
                DuelManager.Instance.MainDuel,
                obj,
                0.0f,
                1.0f,
                0.2f
            );
        }
    }

    public void HideDeckview() {
        foreach(Transform obj in DeckViewContainer.transform) {
            if(obj.childCount != 0) {
                Destroy(obj.GetChild(0).gameObject);
            }
        }
        DeckViewObject.SetActive(false);
    }

    public void ShowPlayerDraw() {
        ShowDeckView(DuelManager.Instance.MainDuel.PlayerStatus.drawPileCards);
    }

    public void ShowPlayerDiscard() {
        ShowDeckView(DuelManager.Instance.MainDuel.PlayerStatus.discardPileCards);
    }

    public void ShowEnemyDiscard() {
        ShowDeckView(DuelManager.Instance.MainDuel.EnemyStatus.Deck.DiscardPile());
    }

    public void PlayerWin() {
        if (PersistentData.Instance.CurrentEncounter.boss.finalBoss) {
            SceneManager.LoadScene(MenuScript.TITLE_INDEX);
        }
        else {
            PersistentData.Instance.EncountersFinished++;
            PersistentData.Instance.VsState = VsScreenState.Win;
            SceneManager.LoadScene(MenuScript.VERSUS_INDEX);
        }
    }

    public void PlayerLose() {
        PersistentData.Instance.VsState = VsScreenState.Lose;
        SceneManager.LoadScene(MenuScript.VERSUS_INDEX);
    }

    public void CheckProperInitialization() {
        if(TemplateTile == null) {
            Debug.LogError("Cannot create board, TemplateTile is uninitialized");
            return;
        }
        if(TemplateUnitCard == null) {
            Debug.LogError("Could not create hand, TemplateCard is uninitialized");
            return;
        }
        if(BoardContainer == null) {
            Debug.LogError("Cannot create board, BoardContainer is uninitialized");
            return;
        }
        BoardContainer.CheckProperInitialization();

        if(Hand == null) {
            Debug.LogError("Could not create hand, Hand is uninitialized");
            return;
        }

        if (EnemyHand == null) {
            Debug.LogError("Could not create hand, EnemyHand is uninitialized");
            return;
        }
    }
}
