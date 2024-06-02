using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDrawFlash : MonoBehaviour
{
    public Image blankCard;

    void Update()
    {
        if (transform.childCount <= 0 && DuelManager.Instance.MainDuel.PlayerStatus.CanDrawCard() && DrawCardButton.Instance.CanInteract) {
            float alpha = (float)(Math.Sin(Time.time*1.5f)/7) + 1.0f/6.0f;
            Debug.Log(alpha);
            blankCard.color = new Color(1, 1, 1, alpha);
        }
        else {
            blankCard.color = Color.clear;
        }
    }
}
