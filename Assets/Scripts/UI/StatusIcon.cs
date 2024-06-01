using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    public StatusEffect statusEffect;
    public TMP_Text durationText;
    public Image image;
    public Image imageOutline;

    public static int spacing = 55;

    public void SetStatus(StatusEffect se, int position)
    {
        statusEffect = se;
        image.sprite = statusEffect.icon;
        imageOutline.sprite = image.sprite;
        if (statusEffect.duration != -1)
            durationText.text = statusEffect.duration.ToString();
        else
            durationText.text = "";
        RectTransform transform = GetComponent<RectTransform>();
        transform.anchoredPosition = new Vector2(
            transform.anchoredPosition.x + position * spacing,
            transform.anchoredPosition.y);
    }
    public void ShiftStatus(int direction)
    {
        RectTransform transform = GetComponent<RectTransform>();
        transform.anchoredPosition += new Vector2(direction * spacing, 0);
    }
}
