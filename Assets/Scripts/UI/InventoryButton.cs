using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image BookImage;
    public Sprite OpenBook;
    public Sprite ClosedBook;

    private Vector3 openScale = new Vector3(1.5f, 1.5f, 1.5f) * 0.3f;
    private Vector3 closedScale = new Vector3(1.0f, 1.0f, 1.0f) * 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        BookImage.sprite = ClosedBook;
        BookImage.transform.localScale = closedScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BookImage.sprite = OpenBook;
        BookImage.transform.localScale = openScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BookImage.sprite = ClosedBook;
        BookImage.transform.localScale = closedScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MenuScript.Instance.OpenInventory();
    }
}
