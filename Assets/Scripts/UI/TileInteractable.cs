using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// A tile that a card can be played on
public class TileInteractable : MonoBehaviour, IPointerDownHandler
{

    [HideInInspector] public Vector2Int locationRC; //DEPRECATED - DO NOT USE
    [HideInInspector] public BoardCoords location;
    [HideInInspector] public bool isHighlighted = false;
    public Sprite HighlightedSprite;
    public Sprite NormalSprite;
    [SerializeField] private Color defaultColor, highlightColor;
    public Image image;

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerInputController.Instance.InteractTile(location);
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        if (isHighlighted) {
            image.sprite = HighlightedSprite;
        }
        else
        {
            image.sprite = NormalSprite;
        }
    }

    public bool IsOccupied()
    {
        return DuelManager.Instance.MainDuel.DuelBoard.IsOccupied(location);
    }
}
