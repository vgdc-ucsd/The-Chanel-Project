using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject HighlightText;
    public TextMeshProUGUI HighlightTMP;
    public TextMeshProUGUI TMP;

    void Start() {
        HighlightText.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightText.SetActive(false);
    }
}
