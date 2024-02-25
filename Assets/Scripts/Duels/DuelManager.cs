using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// The main manager for duels/combat, handles all things related to duels
public class DuelManager : MonoBehaviour
{
    // Singleton
    public static DuelManager Instance;

    // Game Settings
    public DuelSettings Settings;

    // The decks of cards used in the duel
    public Deck PlayerDeck;
    public Deck EnemyDeck;

    public CharStatus PlayerStatus, EnemyStatus;

    // Script for managing UI
    public UIManager UI;

    // Script that handles logic for the duels
    public DuelController DC;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckProperInitialization();
        DC = new DuelController(PlayerStatus,EnemyStatus);
        UI.SetupBoard();
        UI.SetupHand();
        DuelEvents.Instance.UpdateUI();

        if (Settings.EnablePVPMode || Settings.ShowEnemyHand) {
            UI.EnemyHand.gameObject.SetActive(true);
        }

        DC.StartDuel();
    }

    private void CheckProperInitialization() {
        UI.CheckProperInitialization();

        if(PlayerDeck == null || EnemyDeck == null) {
            Debug.LogError("Could not start duel, decks are uninitalized");
            return;
        }
        if(UI == null) {
            Debug.LogError("Could not start duel, the UIManager is uninitalized");
            return;
        }
        if(Settings == null) {
            Debug.LogError("Could not start duel, the DuelSeetings are uninitialized");
            return;
        }
    }

    public void EndTurn() {
        DC.EndTurn();
    }
}
