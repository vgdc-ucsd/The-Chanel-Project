using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// The main manager for duels/combat, handles all things related to duels
public class DuelManager : MonoBehaviour
{
    // Singleton
    public static DuelManager Instance;
    public EffectsLibrary Effects;

    // Set through inspector
    public DuelSettings Settings;
    public Deck PlayerDeck;
    public Deck EnemyDeck;

    // Game Logic
    public DuelInstance MainDuel;
    private MctsAI ai;
    private bool awaitingAI;
    public Team currentTeam;


    void Awake() {
        // Singleton
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Tried to create more than one instance of the DuelManager singleton");
            Destroy(this);
            return;
        }
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PersistentData.Instance == null) {
            Debug.LogWarning("Could not load encounter data");
        }
        else {
            Settings = PersistentData.Instance.CurrentEncounter.Settings;
            EnemyDeck = PersistentData.Instance.CurrentEncounter.EnemyDeck;
        }

        CheckProperInitialization();

        // DuelInstance Setup
        CharStatus PlayerStatus = new CharStatus(Team.Player, ScriptableObject.Instantiate(PlayerDeck));
        CharStatus EnemyStatus = new CharStatus(Team.Enemy, ScriptableObject.Instantiate(EnemyDeck));
        PlayerStatus.Deck.Init();
        EnemyStatus.Deck.Init();
        Board board = new Board(Settings.BoardRows, Settings.BoardCols);
        MainDuel = new DuelInstance(PlayerStatus, EnemyStatus, board);

        // AI setup
        ai = new MctsAI();
        awaitingAI = false;

        // Set Starting Team
        if (Settings.EnemyGoesFirst) currentTeam = Team.Enemy;
        else currentTeam = Team.Player;

        // UI Setup
        UIManager.Instance.Initialize();
        UIManager.Instance.UpdateStatus(MainDuel);
        if (Settings.EnablePVPMode || Settings.ShowEnemyHand) {
            UIManager.Instance.EnemyHand.gameObject.SetActive(true);
        }

        // Draw staring cards
        AnimationManager.Instance.Enqueue(MainDuel.DrawStartingCards());
    }

    private void Update()
    {
        if(MainDuel.Animations.Count != 0) {
            AnimationManager.Instance.Enqueue(MainDuel.Animations);
            MainDuel.Animations.Clear();
        }

        //Debug.Log(MainDuel.PlayerStatus.Cards.ToCommaSeparatedString());
    }

    private void CheckProperInitialization() {
        UIManager.Instance.CheckProperInitialization();

        if(PlayerDeck == null || EnemyDeck == null) {
            Debug.LogError("Could not start duel, decks are uninitalized");
            return;
        }
        if(Settings == null) {
            Debug.LogError("Could not start duel, the DuelSettings are uninitialized");
            return;
        }
    }

    // triggered by button
    public void DrawCardPlayer()
    {
        if (Settings.EnablePVPMode)
            throw new System.NotImplementedException();

        MainDuel.DrawCardWithMana(Team.Player);

        AnimationManager.Instance.UpdateUIAnimation(MainDuel);
    }

    // triggered by button
    public void EndTurnPlayer() {
        if(awaitingAI) return; // await AI
        if(!AnimationManager.Instance.DonePlaying()) return; // await animations

        if(Settings.EnablePVPMode) {
            if (currentTeam == Team.Player) {
                MainDuel.ProcessBoard(Team.Player);
                currentTeam = Team.Enemy;
            }
            else {
                MainDuel.ProcessBoard(Team.Enemy);
                currentTeam = Team.Player;
            }
        }
        else {
            EnablePlayerControl(false);
            MainDuel.ProcessBoard(Team.Player);
            currentTeam = Team.Enemy;
            AnimationManager.Instance.UpdateUIAnimation(MainDuel);
            StartCoroutine(ai.MCTS(MainDuel));
            awaitingAI = true;
        }
    }


    public void EnemyMove(DuelInstance state) {
        state.ProcessBoard(Team.Enemy);
        MainDuel = state;
        //state.DebugBoard();
        AnimationManager.Instance.DrawCardsAnimation(MainDuel, new List<Card>(), Team.Enemy);
        AnimationManager.Instance.UpdateUIAnimation(MainDuel);
        AnimationManager.Instance.RestorePlayerControlAnimation(MainDuel);

        awaitingAI = false;
        currentTeam = Team.Player;

        foreach (CharStatus status in new CharStatus[] { state.EnemyStatus, state.PlayerStatus })
        {
            foreach (Card c in status.Cards)
            {
                if (c is UnitCard uc && uc.UnitCardInteractableRef != null)
                    uc.UnitCardInteractableRef.card = uc;
                else if (c is SpellCard sc && sc.SpellCardInteractableRef != null)
                    sc.SpellCardInteractableRef.card = sc;
            }
        }

        /*
        Debug.Log($"Player Draw Pile: {MainDuel.PlayerStatus.Deck.DrawPile().ToCommaSeparatedString()}");
        Debug.Log($"Player Discard Pile: {MainDuel.PlayerStatus.Deck.DiscardPile().ToCommaSeparatedString()}");
        Debug.Log($"Enemy Draw Pile: {MainDuel.EnemyStatus.Deck.DrawPile().ToCommaSeparatedString()}");
        Debug.Log($"Enemy Discard Pile: {MainDuel.EnemyStatus.Deck.DiscardPile().ToCommaSeparatedString()}");
        */
    }

    public void EnablePlayerControl(bool enable) {
        UIManager.Instance.HighlightEndTurnButton(enable);
        List<Card> cards = new List<Card>();
        cards.AddRange(MainDuel.GetStatus(Team.Player).Cards);
        cards.AddRange(MainDuel.DuelBoard.CardSlots);

        foreach (Card card in cards) {
            if (card != null && card.CardInteractableRef != null) {
                card.CardInteractableRef.CanInteract = enable;
            }
        }
    }

    [Serializable]
    public class EffectsLibrary
    {
        public FireEffect FireEffectTemplate;
        public PoisonEffect PoisonEffectTemplate;
    }
}
