using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    public TMP_Text durationText;
    public Image image;
    public static int spacing = 25;

    public void SetStatus(StatusEffect effect, int position)
    {
        image.sprite = effect.icon;
        if (effect.duration != -1)
            durationText.text = effect.duration.ToString();
        else
            durationText.text = "";
        image.rectTransform.anchoredPosition = new Vector2(
            image.rectTransform.anchoredPosition.x + position * spacing,
            image.rectTransform.anchoredPosition.y);
    }
}
