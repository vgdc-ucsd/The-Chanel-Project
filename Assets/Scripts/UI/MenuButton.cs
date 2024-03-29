using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public Sprite BookOpenSprite;
    public Sprite BookClosedSprite;

    private Image img;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        img.sprite = BookClosedSprite;
            
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.sprite = BookOpenSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.sprite = BookClosedSprite;
    }
}
