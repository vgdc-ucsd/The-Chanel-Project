using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ManaText;
    public Image[] ManaSprites;
    public Sprite ManaBlack;
    public Sprite ManaBlue;

    public void UpdateUI(CharStatus status) {
        HealthText.text = status.Health.ToString();
        ManaText.text = status.Mana.ToString();

        for(int i = 0; i < ManaSprites.Length; i++) {
            if(i < status.ManaCapacity) {
                ManaSprites[i].sprite = ManaBlue;
                if(i < status.Mana) {
                    ManaSprites[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                else {
                    ManaSprites[i].color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                }
            }
            else {
                ManaSprites[i].sprite = ManaBlack;
                ManaSprites[i].color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
        }

        
        if(status.Health <= 0 && !DuelManager.Instance.Settings.DisableWinning) {
            if(status.CharTeam == Team.Player) UIManager.Instance.PlayerWin();
            else UIManager.Instance.PlayerLose();

            AnimationManager.Instance.ClearQueue();
        }
    }

    public void CheckProperInitialization() {
        if(HealthText == null) {
            Debug.LogError("PlayerUIError, HealthText is uninitialized");
            return;
        }
        if(ManaText == null) {
            Debug.LogError("PlayerUIError, ManaText is uninitialized");
            return;
        }
    }
}
