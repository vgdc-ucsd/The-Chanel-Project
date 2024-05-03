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

    // Set through inspector
    public DuelSettings Settings;
    public Deck PlayerDeck;
    public Deck EnemyDeck;
    public UIManager UI;
    public AnimationManager AM;

    // Game Logic
    public DuelInstance MainDuel;
    private MctsAI ai;
    private bool awaitingAI;

    void Awake() {
        // Singleton
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Tried to create more than one instance of the DuelManager singleton");
            Destroy(this);
        }
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckProperInitialization();

        // DuelInstance Setup
        CharStatus PlayerStatus = new CharStatus(Team.Player, PlayerDeck);
        CharStatus EnemyStatus = new CharStatus(Team.Enemy, EnemyDeck);
        Board board = new Board(Settings.BoardRows, Settings.BoardCols);
        MainDuel = new DuelInstance(PlayerStatus, EnemyStatus, board);

        // Draw staring cards
        AM.Enqueue(MainDuel.DrawStartingCards());

        // AI setup
        ai = new MctsAI(PlayerStatus, EnemyStatus);
        awaitingAI = false;

        // UI Setup
        UI.Initialize(PlayerStatus, EnemyStatus);
        if (Settings.EnablePVPMode || Settings.ShowEnemyHand) {
            UI.EnemyHand.gameObject.SetActive(true);
        }
        //DuelEvents.Instance.UpdateUI();
        //DuelEvents.Instance.UpdateHand();
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
        if(AM == null) {
            Debug.LogError("Could not start duel, the AnimationManager is uninitalized");
            return;
        }
        if(Settings == null) {
            Debug.LogError("Could not start duel, the DuelSettings are uninitialized");
            return;
        }
    }

    public void EndTurnPlayer() {
        // TODO can only end turn if awaitingAI is false

        if(Settings.EnablePVPMode) {
            // TODO
        }
        else {
            AM.Enqueue(MainDuel.ProcessBoard(Team.Player));
            StartCoroutine(ai.MCTS(MainDuel));
            awaitingAI = true;
        }
    }

    public void EnemyMove(DuelInstance state) {
        MainDuel = state;
        AM.Enqueue(state.ProcessBoard(Team.Enemy));
        awaitingAI = false;
        //DuelEvents.Instance.UpdateUI();
        //if(team == Team.Player) DuelEvents.Instance.UpdateHand();
    }
}
