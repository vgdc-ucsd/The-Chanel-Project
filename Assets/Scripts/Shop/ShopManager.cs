using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    // Collection of Cards Player Unlocked
    public Deck cardCollection;
    public Deck playerDeck; // use persistant data

    [HideInInspector]
    public List<Card> availableShopCards;

    public Transform shopContainer;

    //TEST, Text Reference for Player Gold
    public TextMeshProUGUI goldText;
    public TMP_Text dText;

    Coroutine spendGoldCor;

    private List<CardInteractable> cardInteractables;
    public GameObject cardSlotTemplate;

    // Inspect GameObject/Screen
    public GameObject inspectScreen;
    public CardInfoPanel infoPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        PersistentData.Instance.GenerateShopOffers();
        goldText.text = PersistentData.Instance.Inventory.Gold.ToString();

        // Adds cards that the player doesn't have into the shop
        cardInteractables = new List<CardInteractable>();

        foreach (Card c in PersistentData.Instance.ShopOffers)
        {
            GameObject cardSlot = Instantiate(cardSlotTemplate);
            cardSlot.transform.SetParent(shopContainer, false);
            c.CurrentTeam = Team.Neutral;
            CardInteractable ci = UIManager.Instance.GenerateCardInteractable(c);
            ci.transform.SetParent(cardSlot.transform, false);
            ci.mode = CIMode.Shop;
            ci.CanInteract = true;
            cardInteractables.Add(ci);
            ci.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text = c.ShopCost.ToString();
        }
    }
    
    // OnClick Function from ShopCardInteractable
    public bool purchase(CardInteractable ci)
    {
        Card card = ci.GetCard();
        if(card.ShopCost <= PersistentData.Instance.Inventory.Gold)
        {
            PersistentData.Instance.Inventory.InactiveCards.Add(card.Clone());
            PersistentData.Instance.Inventory.Gold -= card.ShopCost;
            if (spendGoldCor != null) StopCoroutine(spendGoldCor);
            spendGoldCor = StartCoroutine(SpendGoldAnim(card.ShopCost));
            Destroy(ci.gameObject);
            return true;
        }
        else
        {
            //TODO: Set up Proper Response to Insufficient Funds
            Debug.Log("Insufficient Funds");
            return false;
        }
    }

    // Inspect Function for ShopCardInteractable
    public void inspect(CardInteractable ci)
    {
        inspectScreen.SetActive(true);
        infoPanel.InitializeCardInfoPanel(ci.GetCard());
    }

    public void toInventory()
    {
        MenuScript.Instance.OpenInventory();
    }

    public void toMap()
    {
        SceneManager.LoadScene(MenuScript.MAP_INDEX);
    }


    private IEnumerator SpendGoldAnim(int gold)
    {
        goldText.text = PersistentData.Instance.Inventory.Gold.ToString();
        dText.text = "-" + gold.ToString();
        dText.enabled = true;
        float elapsedTime = 0;
        float endTime = 1;
        float startTime = Time.time;
        while (elapsedTime < endTime)
        {
            elapsedTime = Time.time - startTime;
            dText.color = Interpolation.Interpolate(Color.red, Color.clear,
                    elapsedTime / endTime, InterpolationMode.Linear);
            yield return null;
        }
        dText.enabled = false;
    }

    // TODO: Port Function to Inventory
    // NOT NEEDED FOR SHOP, BUT WONDERFUL FOR INVENTORY :)
    // Quicksort to sort 'shopCards' list by name (in ascending order)

    /*public List<Card> SortCardsByName(List<Card> cards, int leftIndex, int rightIndex)
    {
        var i = leftIndex;
        var j = rightIndex;
        var pivot = cards[leftIndex];

        while (i <= j)
        {
            while (String.Compare(cards[i].name, pivot.name) < 0)
            // cards[i].name < pivot.name)
            {
                i++;
            }

            while (String.Compare(cards[j].name, pivot.name) > 0)
            {
                j--;
            }

            if (i <= j)
            {
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
                i++;
                j--;
            }
        }

        if (leftIndex < j)
            SortCardsByName(cards, leftIndex, j);

        if (i < rightIndex)
            SortCardsByName(cards, i, rightIndex);

        return cards;
    }*/
}
