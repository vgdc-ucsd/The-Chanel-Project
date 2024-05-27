using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusIconManager : MonoBehaviour
{

    public UnitCardInteractable ci;
    public List<StatusIcon> icons;
    public StatusIcon statusIconTemplate;
    public void RefreshIcons()
    {
        ClearIcons();
        int i = 0;
        foreach (StatusEffect effect in ci.card.StatusEffects)
        {
            StatusIcon icon = Instantiate(statusIconTemplate, transform);
            icon.SetStatus(effect, i);
            icons.Add(icon);
            i++;
        }
    }
    public void ClearIcons()
    {
        foreach (StatusIcon icon in icons)
        {
            Destroy(icon.gameObject);
        }
        icons.Clear();
    }
}
