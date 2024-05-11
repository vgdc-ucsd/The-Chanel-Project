using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ExpandedInfo : MonoBehaviour
{
    public Transform cardPos;
    public TextMeshProUGUI descriptionText;
    [SerializeField] GameObject cardTemplate;
    [SerializeField] float scale = 3.3f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SendCardDetails(UnitCard card)
    {
        if (cardPos.childCount > 0)
        {
            Destroy(cardPos.GetChild(0).gameObject);
        }

        RectTransform cardObject = Instantiate(cardTemplate, cardPos).GetComponent<RectTransform>();

        // Set name, health, attack text appropriately
        cardObject.Find("Name").GetComponent<TextMeshProUGUI>().text = card.name;
        cardObject.Find("Health").GetComponent<TextMeshProUGUI>().text = "Health: " + card.Health.ToString();
        cardObject.Find("Attack").GetComponent<TextMeshProUGUI>().text = "Attack: " + card.BaseDamage.ToString();

        // Set card info appropriately
        cardObject.GetComponent<InventoryItemInteractable>().card = card;
        cardObject.GetComponent<InventoryItemInteractable>().enabled = false;

        cardObject.localScale = new Vector3(scale, scale, scale);
        cardObject.gameObject.SetActive(true);
    }
}
