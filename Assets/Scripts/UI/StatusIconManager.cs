using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
            icon.gameObject.SetActive(true);
            icon.SetStatus(effect, i, effect.duration);
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
    public StatusIcon AddIcon(StatusEffect effect, int duration)
    {
        StatusIcon icon = Instantiate(statusIconTemplate, transform);
        icon.gameObject.SetActive(true);
        icon.SetStatus(effect, icons.Count, duration);
        icons.Add(icon);
        return icon;
    }
    public bool RemoveIcon(StatusEffect statusEffect)
    {
        StatusIcon statusIcon = GetStatusIcon(statusEffect);
        if (statusIcon == null) return false;

        int index = icons.IndexOf(statusIcon);
        icons.Remove(statusIcon);
        Destroy(statusIcon.gameObject);
        for (int i = index; i < icons.Count; ++i) {
            icons[i].ShiftStatus(-1);
        }
        return true;
    }
    public void UpdateIcon(StatusEffect effect, int amount)
    {
        foreach (StatusIcon statusIcon in icons) {
            int currentDuration = 0;
            if (statusIcon.statusEffect.GetType() == effect.GetType()) {
                if (!statusIcon.durationText.text.Equals("")) {
                    currentDuration = Math.Max(int.Parse(statusIcon.durationText.text) + amount, 0);
                    statusIcon.durationText.text = currentDuration.ToString();
                }
            }
        }
    }
    public StatusIcon GetStatusIcon(StatusEffect statusEffect) {
        foreach (StatusIcon statusIcon in icons) {
            if (statusIcon.statusEffect.GetType() == statusEffect.GetType()) {
                return statusIcon;
            }
        }
        return null;
    }
}
