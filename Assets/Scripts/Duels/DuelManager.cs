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

    // Game Logic
    public DuelInstance MainDuel;
    private MctsAI ai;
    private bool awaitingAI;
    private Team currentTeam;

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
        AnimationManager.Instance.Enqueue(MainDuel.DrawStartingCards());

        // AI setup
        ai = new MctsAI();
        awaitingAI = false;

        // Set Starting Team
        if (Settings.EnemyGoesFirst) currentTeam = Team.Enemy;
        else currentTeam = Team.Player;

        // UI Setup
        UI.Initialize();
        UI.UpdateStatus(MainDuel);
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
        if(Settings == null) {
            Debug.LogError("Could not start duel, the DuelSettings are uninitialized");
            return;
        }
    }

    public void EndTurnPlayer() {
        if(awaitingAI) return; // await AI
        if(!AnimationManager.Instance.DonePlaying()) return; // await animations

        if(Settings.EnablePVPMode) {
            if (currentTeam == Team.Player) {
                MainDuel.ProcessBoard(Team.Player);
                AnimationManager.Instance.Enqueue(MainDuel.Animations);
                currentTeam = Team.Enemy;
            }
            else {
                MainDuel.ProcessBoard(Team.Enemy);
                AnimationManager.Instance.Enqueue(MainDuel.Animations);
                currentTeam = Team.Player;
            }
        }
        else {
            MainDuel.ProcessBoard(Team.Player);
            AnimationManager.Instance.Enqueue(MainDuel.Animations);
            StartCoroutine(ai.MCTS(MainDuel));
            awaitingAI = true;
        }
    }

    public void EnemyMove(DuelInstance state) {
        state.ProcessBoard(Team.Enemy);
        MainDuel = state;
        //state.DebugBoard();
        AnimationManager.Instance.Enqueue(state.Animations);
        UI.UpdateStatus(state);
        awaitingAI = false;
    }
}
