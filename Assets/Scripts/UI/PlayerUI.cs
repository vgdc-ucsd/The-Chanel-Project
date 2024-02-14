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
    public CharStatus Status;

    void Awake() {
        DuelEvents.Instance.onUpdateUI += UpdateUI;
    }

    private void UpdateUI() {
        HealthText.text = Status.Health.ToString();
        ManaText.text = Status.Mana.ToString();
    }

    public void CheckProperInitialization() {
        if(Status == null) {
            Debug.LogError("PlayerUI Error, Status is uninitalized");
            return;
        }
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
