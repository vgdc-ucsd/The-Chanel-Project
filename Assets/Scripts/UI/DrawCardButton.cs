using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class DrawCardButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) {
        if (DuelManager.Instance.MainDuel.GetStatus(Team.Player).Deck.DrawPileIsEmpty()) return;
        AnimationManager.Instance.StartManaHover(DuelManager.Instance.Settings.DrawCardManaCost, Team.Player);
    }

    public void OnPointerExit(PointerEventData eventData) {
        AnimationManager.Instance.StopManaHover(Team.Player);
    }
}
