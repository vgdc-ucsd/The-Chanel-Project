using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DrawCardButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{

    private static GameObject hoveredCard;
    private Vector3 basePosition;
    private Transform playerDrawPile;
    private float scaleFactor = 1.1f;

    public void OnPointerEnter(PointerEventData eventData) {
        if (DuelManager.Instance.MainDuel.GetStatus(Team.Player).Deck.DrawPileIsEmpty()) return;

        AnimationManager.Instance.StartManaHover(DuelManager.Instance.Settings.DrawCardManaCost, Team.Player);

        playerDrawPile = UIManager.Instance.PlayerDraw;
        hoveredCard = playerDrawPile.GetChild(0).gameObject;

        basePosition = hoveredCard.transform.position;
        hoveredCard.transform.position = hoverPosition();
        hoveredCard.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        hoveredCard.transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData) {
        AnimationManager.Instance.StopManaHover(Team.Player);

        if (hoveredCard == null) return;

        hoveredCard.transform.position = basePosition;
        hoveredCard.transform.localScale = Vector3.one;

        hoveredCard = null;
    }

    private Vector3 hoverPosition() {
        return new Vector3(
            hoveredCard.transform.position.x,
            hoveredCard.transform.position.y + (50f*UIManager.Instance.MainCanvas.scaleFactor),
            hoveredCard.transform.position.z
        );
    }
}
