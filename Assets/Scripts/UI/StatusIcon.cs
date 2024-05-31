using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusIcon : MonoBehaviour
{
    public TMP_Text durationText;
    public Image image;
    public Image imageOutline;

    public static int spacing = 55;

    public void SetStatus(StatusEffect effect, int position)
    {
        image.sprite = effect.icon;
        imageOutline.sprite = image.sprite;
        if (effect.duration != -1)
            durationText.text = effect.duration.ToString();
        else
            durationText.text = "";
        RectTransform transform = GetComponent<RectTransform>();
        transform.anchoredPosition = new Vector2(
            transform.anchoredPosition.x + position * spacing,
            transform.anchoredPosition.y);
    }
}
