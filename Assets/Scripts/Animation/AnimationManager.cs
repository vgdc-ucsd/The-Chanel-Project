using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    public Transform EnemyHandLocation;

    private Queue<QueueableAnimation> animations = new Queue<QueueableAnimation>();
    private bool activelyPlaying = false;

    void Awake() {
        // Singleton
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Tried to create more than one instance of the AnimationManager singleton");
            Destroy(this);
        }
        else Instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(!activelyPlaying && animations.Count > 0) {
            StartCoroutine(PlayAnimations());
        }
    }

    // Plays the animation immediately
    public void Play(IEnumerator animation) {
        StartCoroutine(animation);
    }

    // Plays the animation when the delay from the previous animation has finished
    public void Enqueue(QueueableAnimation qa) {
        animations.Enqueue(qa);
    }
    public void Enqueue(Queue<QueueableAnimation> qa) {
        while(qa.Count > 0) animations.Enqueue(qa.Dequeue());
    }

    public void ClearQueue() {
        animations.Clear();
    }

    private IEnumerator PlayAnimations() {
        activelyPlaying = true;
        
        // While the animation queue is not empty
        while(animations.Count > 0) {
            float startTime = Time.time;
            // Dequeues and plays the animation
            QueueableAnimation current = animations.Dequeue();
            if(current != null && current.Animation != null) {
                Play(current.Animation);

                // Waits until the delay is done
                while(Time.time - startTime < current.Delay) {
                    yield return null;
                }
            }
        }
        activelyPlaying = false;
    }

    public bool DonePlaying() {
        return animations.Count == 0;
    }

    // **************************************************************
    //                 Animation logic (IEnumerator)
    // **************************************************************

    // Translates the transform at origin to the position at dest
    public IEnumerator SimpleTranslate(Transform origin, Vector3 dest, float duration, InterpolationMode mode) {
        if(origin == null) yield break;
        float startTime = Time.time;
        Vector3 startPos = origin.position;
        float elapsedTime = Time.time - startTime;

        // Interpolates between two positions until elapsedTime reaches duration
        while(elapsedTime < duration) {
            if(origin == null) yield break;
            float t = elapsedTime / duration;
            origin.position = Interpolation.Interpolate(startPos, dest, t, mode);
            elapsedTime = Time.time - startTime;
            yield return null;
        }

        origin.position = dest;
    }

    // Animation that plays when a card makes an attack in a direction
    // Pulls backwards briefly then launches forwards
    private IEnumerator CardAttack(UnitCard card, Vector2 atkDir, float duration) {

        InterpolationMode mode = InterpolationMode.Linear;
        Transform cardTransform = card.CardInteractableRef.transform;
        Vector3 startPos = cardTransform.position;

        // windup
        float windupDuration = duration * 0.4f;
        float windupOffset = 10f;
        Vector3 windupPos = startPos + new Vector3(windupOffset*(-atkDir.x), windupOffset*(-atkDir.y), 0);

        // launch
        float launchDuration = duration * 0.3f;
        float launchOffset = 40f;
        Vector3 launchPos = startPos + new Vector3(launchOffset*atkDir.x, launchOffset*atkDir.y, 0);

        // recover
        float recoverDuration = duration * 0.3f;

        // animation
        yield return SimpleTranslate(cardTransform, windupPos, windupDuration, mode);
        yield return SimpleTranslate(cardTransform, launchPos, launchDuration, mode);
        yield return SimpleTranslate(cardTransform, startPos, recoverDuration, mode);
        if(card == null) yield break;

        cardTransform.position = startPos;
    }

    private IEnumerator CardDeath(UnitCard card) {
        Transform cardTransform = card.UnitCardInteractableRef.transform;
        Transform drawPile;
        Transform discardPile;

        if(card.CurrentTeam == Team.Player) {
            drawPile = UIManager.Instance.PlayerDraw;
            discardPile = UIManager.Instance.PlayerDiscard;
        }
        else {
            drawPile = UIManager.Instance.EnemyDraw;
            discardPile = UIManager.Instance.EnemyDiscard;
        }

        card.UnitCardInteractableRef.CanInteract = false;
        cardTransform.SetParent(discardPile);
        // TODO reset health and attack to card default
        // TODO hide arrows
        yield return SimpleTranslate(cardTransform, discardPile.position, 0.5f, InterpolationMode.Linear);
        
        if(discardPile.childCount==1 && drawPile.childCount==0) {
            ShuffleDiscardIntoDeckAnimation(discardPile, drawPile);
        }
    }

    public void CardDeathImmediate(UnitCard card)
    {
        StartCoroutine(CardDeath(card));
    }

    public IEnumerator OrganizeCards(List<Card> cards, Team team) {
        int drawPileCount = -1; // this is necessary because drawPile.childCount only updates once per frame
        int childIndex = 0;

        foreach(Card c in cards) {
            if(c.CardInteractableRef == null) {

                Transform drawPile;
                Transform discardPile;

                if(team == Team.Player) {
                    drawPile = UIManager.Instance.PlayerDraw;
                    discardPile = UIManager.Instance.PlayerDiscard;
                }
                else {
                    drawPile = UIManager.Instance.EnemyDraw;
                    discardPile = UIManager.Instance.EnemyDiscard;
                }
                if(drawPileCount == -1) drawPileCount = drawPile.childCount;

                // Draw hidden enemy card
                if(c.CurrentTeam == Team.Enemy && !DuelManager.Instance.Settings.EnablePVPMode) {
                    GameObject cardBack = Instantiate(UIManager.Instance.TemplateCardBack);
                    cardBack.transform.position = drawPile.position;
                    UIManager.Instance.EnemyHand.cardObjects.Add(cardBack);
                    if(drawPileCount == 0 && discardPile.childCount > 0) {
                        ShuffleDiscardIntoDeckAnimation(discardPile, drawPile);
                        drawPileCount = discardPile.childCount;
                    }
                    Destroy(drawPile.GetChild(childIndex).gameObject);
                    drawPileCount--;
                    childIndex++;
                }
                // Draw visible card
                else {
                    c.CardInteractableRef = UIManager.Instance.GenerateCardInteractable(c);
                    c.CardInteractableRef.transform.position = drawPile.position;
                    if(drawPileCount == 0 && discardPile.childCount > 0) {
                        ShuffleDiscardIntoDeckAnimation(discardPile, drawPile);
                        drawPileCount = discardPile.childCount;
                    }
                    Destroy(drawPile.GetChild(childIndex).gameObject);
                    drawPileCount--;
                    childIndex++;
                }
            }
        }

        if (team == Team.Player) UIManager.Instance.Hand.OrganizeCards();
        else UIManager.Instance.EnemyHand.OrganizeCards();

        yield return null;
    }

    public IEnumerator PlaceUnitCard(UnitCard c, BoardCoords pos, float speed) {
        UnitCardInteractable unitRef = c.UnitCardInteractableRef;

        // make a new card interactable if there is none
        // TODO remove since they should be generated when the enemy draws/organizes the card ?
        if(unitRef == null) {
            unitRef = (UnitCardInteractable) UIManager.Instance.GenerateCardInteractable(c);
            c.UnitCardInteractableRef = unitRef;
        }

        // Show card place animation if card belongs to enemy
        if(!DuelManager.Instance.Settings.EnablePVPMode && c.CurrentTeam == Team.Enemy) {
            // set card
            TileInteractable tile = BoardInterface.Instance.GetTile(pos);
            unitRef.inHand = false;
            unitRef.transform.localEulerAngles = Vector3.zero;
            unitRef.transform.localScale = Vector3.one;
            if(unitRef.handInterface != null) {
                unitRef.handInterface.cardObjects.Remove(unitRef.gameObject);
            }
            unitRef.transform.SetParent(tile.transform);
            unitRef.transform.localScale = Vector3.one;
            unitRef.DrawArrows(); 
            unitRef.CardCost.enabled = false;
            unitRef.gameObject.SetActive(true);

            // remove card from hand
            int randomIndex = Random.Range(0, UIManager.Instance.EnemyHand.cardObjects.Count);
            GameObject cardBack = UIManager.Instance.EnemyHand.cardObjects[randomIndex];
            UIManager.Instance.EnemyHand.cardObjects.Remove(cardBack);
            unitRef.transform.position = cardBack.transform.position;
            Destroy(cardBack);

            // translation animation
            yield return SimpleTranslate(unitRef.transform, tile.transform.position, speed, InterpolationMode.Linear);
        }
        else {
            unitRef.UIPlaceCard(pos);
        }
    }

    public IEnumerator MoveCard(UnitCard uc, Transform targetPos, float speed) {
        yield return SimpleTranslate(uc.UnitCardInteractableRef.transform, targetPos.position, speed, InterpolationMode.Linear);
        uc.UnitCardInteractableRef.UpdateCardPos();
    }

    private IEnumerator UpdateCardInfo(Card c) {
        if(c.CardInteractableRef != null) {c.CardInteractableRef.UpdateCardInfo();}
        yield return null;
    }

    // **************************************************************
    //              public animation methods (void)
    // **************************************************************

    public void AttackAnimation(DuelInstance duel, UnitCard card, Attack atk) {
        float animDuration = 0.3f;
        IEnumerator anim = CardAttack(
            card, 
            atk.direction,
            animDuration
        );
        QueueableAnimation qa = new QueueableAnimation(anim, animDuration);
        duel.Animations.Enqueue(qa);
    }

    public void DeathAnimation(DuelInstance duel, UnitCard card) {
        IEnumerator ie = CardDeath(card);
        QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
        duel.Animations.Enqueue(qa);
    }

    public void OrganizeCardsAnimation(DuelInstance duel, List<Card> cards, Team team) {
        IEnumerator ie = OrganizeCards(cards, team);
        QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
        duel.Animations.Enqueue(qa);
    }

    public void PlaceUnitCardAnimation(DuelInstance duel, UnitCard c, BoardCoords pos) {
        float speed = 0.5f; // time of animation in seconds
        IEnumerator ie = PlaceUnitCard(c, pos, speed);
        QueueableAnimation qa = new QueueableAnimation(ie, speed);
        duel.Animations.Enqueue(qa);
    }

    private void ShuffleDiscardIntoDeckAnimation(Transform discard, Transform draw) { // only call from other coroutines in this script
        while(discard.childCount > 0) {
            discard.GetChild(0).SetParent(draw);
        }

        for(int i = 0; i < discard.childCount; i++) {
            GameObject cardBack = Instantiate(UIManager.Instance.TemplateCardBack);
            Transform child = discard.GetChild(i);
            cardBack.transform.position = child.position;
            cardBack.transform.SetParent(draw);
            Destroy(child);
            IEnumerator ie = SimpleTranslate(cardBack.transform, draw.position, 0.5f, InterpolationMode.Linear);
            QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
            animations.Enqueue(qa);
        }
    }

    public void MoveCardAnimation(DuelInstance duel, UnitCard uc, BoardCoords targetPos) {
        if(uc.CurrentTeam == Team.Enemy && !DuelManager.Instance.Settings.EnablePVPMode) {
            float speed = 0.5f;
            Transform targetTransform = BoardInterface.Instance.GetTile(targetPos).transform;
            IEnumerator ie = MoveCard(uc, targetTransform, speed);
            QueueableAnimation qa = new QueueableAnimation(ie, speed);
            duel.Animations.Enqueue(qa);
        }
    }

    public void UpdateCardInfoAnimation(DuelInstance duel, UnitCard c) {
        IEnumerator ie = UpdateCardInfo(c);
        QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
        duel.Animations.Enqueue(qa);
    }
}
