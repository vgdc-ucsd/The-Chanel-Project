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
    [SerializeField] private Color defaultColor, highlightColor;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerInputController.Instance.InteractTile(location);
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        if (isHighlighted) {
            image.color = highlightColor;
        }
        else
        {
            image.color = defaultColor;
        }
    }

    public bool IsOccupied()
    {
        return DuelManager.Instance.MainDuel.DuelBoard.IsOccupied(location);
    }
}
