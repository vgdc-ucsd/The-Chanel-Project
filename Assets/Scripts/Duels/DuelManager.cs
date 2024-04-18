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
    public CharStatus PlayerStatus, EnemyStatus;
    public Board CurrentBoard;
    public DuelInstance MainDuel;
    private MctsAI ai;

    void Awake() {
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

        UI.SetupBoard();

        if (Settings.EnablePVPMode || Settings.ShowEnemyHand) {
            UI.EnemyHand.gameObject.SetActive(true);
        }

        PlayerStatus = new CharStatus(Team.Player);
        EnemyStatus = new CharStatus(Team.Enemy);

        UI.SetupStatus();

        CurrentBoard = new Board(Settings.BoardRows, Settings.BoardCols);
        ai = new MctsAI(PlayerStatus, EnemyStatus);
        MainDuel = new DuelInstance(PlayerStatus, EnemyStatus, true);
        MainDuel.InitBoard(CurrentBoard);

        DuelEvents.Instance.UpdateUI();
        DuelEvents.Instance.UpdateHand();
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
            Debug.LogError("Could not start duel, the DuelSeetings are uninitialized");
            return;
        }
    }

    public void EndTurnPlayer() {
        if(Settings.EnablePVPMode) {
            // TODO
        }
        else {
            // TODO await events to be finished (no spamming end turn)
            MainDuel.ProcessBoard(CurrentBoard, Team.Player);
            // TODO begin MCTS
            // TODO await animations
            // TODO await MCTS
            ai.MakeMove();
        }

        DuelEvents.Instance.UpdateUI();
        DuelEvents.Instance.UpdateHand();
    }

    public void TryPlaceCard(UnitCard card, BoardCoords pos) {
        // Check out of bounds
        if (CurrentBoard.IsOutOfBounds(pos)) return;
        if (CurrentBoard.IsOccupied(pos)) return;

        // TODO
        //if (currentTeam != card.team) {
        //    Debug.Log($"Tried to play {card.team} card while on {currentTeam} turn");
        //    return;
        //}
        CharStatus charStatus;
        if(card.CurrentTeam == Team.Player) charStatus = MainDuel.PlayerStatus;
        else charStatus = MainDuel.EnemyStatus;
       
        if (!charStatus.CanUseMana(card.ManaCost))
        {
            Debug.Log("Not enough Mana"); //TODO: UI feedback
            return;
        }
        //if(card.team == Team.Enemy) MirrorAttacks(card); // this should only be called once per enemy card

        CurrentBoard.PlayCard(card, pos, charStatus, true);
        DuelEvents.Instance.UpdateUI();
    }
}
