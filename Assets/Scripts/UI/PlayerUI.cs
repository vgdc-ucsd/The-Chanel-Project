using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ManaText;

    public void UpdateUI(CharStatus status) {
        HealthText.text = status.Health.ToString();
        ManaText.text = status.Mana.ToString();
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
