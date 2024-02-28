using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopCardInteract : MonoBehaviour, IPointerClickHandler
{
    public Card card;


    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Purchased Card");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Inspect Card");
        }
    }
}
