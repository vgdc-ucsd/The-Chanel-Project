using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

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

        if(origin != null) origin.position = dest;
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

    private IEnumerator CardDeath(UnitCard card, float duration) {
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

        // Reset stats
        card.ResetStats();
        card.UnitCardInteractableRef.UpdateCardInfo();

        // Hide arrows
        for (int i = 0; i < card.UnitCardInteractableRef.transform.childCount; i++)
        {
            if (card.UnitCardInteractableRef.transform.GetChild(i).name.Contains("Arrow"))
            {
                card.UnitCardInteractableRef.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        yield return SimpleTranslate(cardTransform, discardPile.position, duration, InterpolationMode.Linear);
    }

    private IEnumerator SpellDiscard(SpellCard card) {
        Transform cardTransform = card.SpellCardInteractableRef.transform;
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

        card.SpellCardInteractableRef.CanInteract = false;
        cardTransform.SetParent(discardPile);
        yield return SimpleTranslate(cardTransform, discardPile.position, 0.5f, InterpolationMode.Linear);
    }

    public IEnumerator OrganizeCards(Team team) {
        if (team == Team.Player) UIManager.Instance.Hand.OrganizeCards();
        else UIManager.Instance.EnemyHand.OrganizeCards();

        yield return null;
    }

    private IEnumerator DrawCards(List<Card> cards, Team team) {
        int childIndex = 0;

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

        if(discardPile.childCount>=1 && drawPile.childCount==0) {
            yield return ShuffleDiscardIntoDeckAnimation(discardPile, drawPile);
            yield return null;
        }

        // draw all cards
        if(drawPile.childCount >= cards.Count) {
            
            foreach(Card c in cards) {
                GameObject cardObject;
                // Draw hidden enemy card
                if(team == Team.Enemy && !DuelManager.Instance.Settings.EnablePVPMode) {
                    cardObject = Instantiate(UIManager.Instance.TemplateCardBack);
                    UIManager.Instance.EnemyHand.cardObjects.Add(cardObject); // need to do manually since not generating interactable
                }
                // Draw visible card
                else {
                    c.CardInteractableRef = UIManager.Instance.GenerateCardInteractable(c);
                    cardObject = c.CardInteractableRef.gameObject;
                }
                cardObject.transform.position = drawPile.position;
                Destroy(drawPile.GetChild(childIndex).gameObject);
                childIndex++;
            }
        }
        // draw then shuffle then draw
        else if(drawPile.childCount < cards.Count && drawPile.childCount+discardPile.childCount >= cards.Count) {
            int initalCount = drawPile.childCount;
            int afterCount = cards.Count-initalCount;
            for(int i = 0; i < initalCount; i++) {
                Card c = cards[i];
                GameObject cardObject;
                // Draw hidden enemy card
                if(team == Team.Enemy && !DuelManager.Instance.Settings.EnablePVPMode) {
                    cardObject = Instantiate(UIManager.Instance.TemplateCardBack);
                    UIManager.Instance.EnemyHand.cardObjects.Add(cardObject); // need to do manually since not generating interactable
                }
                // Draw visible card
                else {
                    c.CardInteractableRef = UIManager.Instance.GenerateCardInteractable(c);
                    cardObject = c.CardInteractableRef.gameObject;
                }
                cardObject.transform.position = drawPile.position;
                Destroy(drawPile.GetChild(childIndex).gameObject);
                childIndex++;
            }

            yield return ShuffleDiscardIntoDeckAnimation(discardPile, drawPile);

            childIndex = 0;
            for(int i = 0; i < afterCount; i++) {
                Card c = cards[i];
                GameObject cardObject;
                // Draw hidden enemy card
                if(team == Team.Enemy && !DuelManager.Instance.Settings.EnablePVPMode) {
                    cardObject = Instantiate(UIManager.Instance.TemplateCardBack);
                    UIManager.Instance.EnemyHand.cardObjects.Add(cardObject); // need to do manually since not generating interactable
                }
                // Draw visible card
                else {
                    c.CardInteractableRef = UIManager.Instance.GenerateCardInteractable(c);
                    cardObject = c.CardInteractableRef.gameObject;
                }
                cardObject.transform.position = drawPile.position;
                Destroy(drawPile.GetChild(childIndex).gameObject);
                childIndex++;
            }
        }
        // no draw
        else {
            Debug.LogError("trying to draw too many cards " + team + " " + cards.Count);
        }
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

    private IEnumerator DamageFlash(UnitCard c, float duration) {
        float damageTime = duration * 0.25f;
        float restoreTime = duration * 0.75f;

        InterpolationMode mode = InterpolationMode.Linear;
        float startTime = Time.time;
        float elapsedTime = Time.time - startTime;

        TextMeshProUGUI text = c.UnitCardInteractableRef.CardHealth;

        // black to red
        Color from = Color.black;
        Color to = Color.red;
        while(elapsedTime < damageTime) {
            if(text == null) break;
            float t = elapsedTime / damageTime;
            elapsedTime = Time.time - startTime;
            Color col = Interpolation.Interpolate(from, to, t, mode);
            text.color = col;
            yield return null;
        }

        yield return UpdateCardInfo(c);

        // red to black
        startTime = Time.time;
        elapsedTime = Time.time - startTime;
        from = Color.red;
        to = Color.black;
        while(elapsedTime < restoreTime) {
            if(text == null) break;
            float t = elapsedTime / restoreTime;
            elapsedTime = Time.time - startTime;
            Color col = Interpolation.Interpolate(from, to, t, mode);
            text.color = col;
            yield return null;
        }

        text.color = Color.black;
    }

    private IEnumerator DamageFlashPlayer(PlayerUI status, int newHealth, float duration) {
        float damageTime = duration * 0.25f;
        float restoreTime = duration * 0.75f;

        InterpolationMode mode = InterpolationMode.Linear;
        float startTime = Time.time;
        float elapsedTime = Time.time - startTime;

        TextMeshProUGUI text = status.HealthText;

        // black to red
        Color from = Color.black;
        Color to = Color.red;
        while(elapsedTime < damageTime) {
            if(text == null) break;
            float t = elapsedTime / damageTime;
            elapsedTime = Time.time - startTime;
            Color col = Interpolation.Interpolate(from, to, t, mode);
            text.color = col;
            yield return null;
        }

        status.HealthText.text = newHealth.ToString();

        // red to black
        startTime = Time.time;
        elapsedTime = Time.time - startTime;
        from = Color.red;
        to = Color.black;
        while(elapsedTime < restoreTime) {
            if(text == null) break;
            float t = elapsedTime / restoreTime;
            elapsedTime = Time.time - startTime;
            Color col = Interpolation.Interpolate(from, to, t, mode);
            text.color = col;
            yield return null;
        }

        text.color = Color.black;
    }

    private IEnumerator DrawArrows(UnitCardInteractable uci) {
        uci.DrawArrows();
        yield return null;
    }

    private IEnumerator BounceScale(Transform obj, float from, float to, float duration) {
        float bounceTime = duration * 0.8f;
        float restoreTime = duration * 0.2f;
        float bounceFactor = 1.05f;

        InterpolationMode mode = InterpolationMode.Linear;
        float startTime = Time.time;
        float elapsedTime = Time.time - startTime;

        obj.localScale = new Vector3(from, from, 1);
        Vector3 startScale = new Vector3(from, from, 1);
        Vector3 endScale = new Vector3(to*bounceFactor, to*bounceFactor, 1);

        // bounce up
        while(elapsedTime < bounceTime) {
            if(obj == null) break;
            float t = elapsedTime / bounceTime;
            elapsedTime = Time.time - startTime;
            obj.localScale = Interpolation.Interpolate(startScale, endScale, t, mode);
            yield return null;
        }

        // bounce down
        startTime = Time.time;
        elapsedTime = Time.time - startTime;
        startScale = endScale;
        endScale = new Vector3(to, to, 1);
        while(elapsedTime < restoreTime) {
            if(obj == null) break;
            float t = elapsedTime / restoreTime;
            elapsedTime = Time.time - startTime;
            obj.localScale = Interpolation.Interpolate(startScale, endScale, t, mode);
            yield return null;
        }

        obj.localScale = endScale;
    }

    private IEnumerator ShuffleDiscardIntoDeckAnimation(Transform discard, Transform draw) { // only call from other coroutines in this script
        foreach(Transform t in discard) {
            GameObject cardBack = Instantiate(UIManager.Instance.TemplateCardBack);
            cardBack.transform.position = t.position;
            cardBack.transform.SetParent(draw);
            cardBack.transform.localScale = Vector3.one;
            Destroy(t.gameObject);
            yield return SimpleTranslate(cardBack.transform, draw.position, 0.2f, InterpolationMode.Linear);
        }
    }

    private IEnumerator Shake(Transform obj, float intensity, float duration) {
        Vector3 originalPos = obj.localPosition;
        float startTime = Time.time;

        while(Time.time - startTime < duration) {
            // Screen shake
            obj.localPosition = originalPos + Random.insideUnitSphere * intensity;
            yield return null;
        }

        obj.localPosition = originalPos;
    }

    private IEnumerator DrawCardsCoroutine(List<Card> cards, Team team) {
        yield return DrawCards(cards, team);
        yield return OrganizeCards(team);
    }

    private IEnumerator ShakeCard(Card c, float intensity, float duration) {
        Transform obj = c.CardInteractableRef.transform;
        yield return Shake(obj, intensity, duration);
    }

    private IEnumerator UpdateUI(DuelInstance duel) {
        UIManager.Instance.UpdateStatus(duel);
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
        float duration = 0.5f;
        IEnumerator ie = CardDeath(card, duration);
        QueueableAnimation qa = new QueueableAnimation(ie, duration);
        duel.Animations.Enqueue(qa);
    }

    public void SpellDiscardAnimation(DuelInstance duel, SpellCard card) {
        IEnumerator ie = SpellDiscard(card);
        QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
        duel.Animations.Enqueue(qa);
    }

    public void DrawCardsAnimation(DuelInstance duel, List<Card> cards, Team team) {
        IEnumerator ie = DrawCardsCoroutine(cards, team);
        QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
        duel.Animations.Enqueue(qa);
    }

    public void PlaceUnitCardAnimation(DuelInstance duel, UnitCard c, BoardCoords pos) {
        float speed = 0.5f; // time of animation in seconds
        IEnumerator ie = PlaceUnitCard(c, pos, speed);
        QueueableAnimation qa = new QueueableAnimation(ie, speed);
        duel.Animations.Enqueue(qa);
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

    public void DamageCardAnimation(DuelInstance duel, UnitCard c) {
        IEnumerator shake = ShakeCard(c, 2.0f, 0.2f);
        QueueableAnimation shakeAnim = new QueueableAnimation(shake, 0.0f);
        duel.Animations.Enqueue(shakeAnim);

        float duration = 0.75f;
        IEnumerator ie = DamageFlash(c, duration);
        QueueableAnimation qa = new QueueableAnimation(ie, duration);
        duel.Animations.Enqueue(qa);
    }

    public void DamagePlayerAnimation(DuelInstance duel, CharStatus status) {
        float duration = 0.75f;
        PlayerUI ui;
        if(status.CharTeam == Team.Player) {
            ui = UIManager.Instance.Player;
        }
        else {
            ui = UIManager.Instance.Enemy;
        }

        IEnumerator shake = Shake(ui.transform, 2.0f, 0.2f);
        QueueableAnimation shakeAnim = new QueueableAnimation(shake, 0.0f);
        duel.Animations.Enqueue(shakeAnim);

        IEnumerator ie = DamageFlashPlayer(ui, status.Health, duration);
        QueueableAnimation qa = new QueueableAnimation(ie, duration);
        duel.Animations.Enqueue(qa);
    }

    public void DrawArrowsAnimation(DuelInstance duel, UnitCard uc) {
        if(uc.UnitCardInteractableRef != null) {
            IEnumerator ie = DrawArrows(uc.UnitCardInteractableRef);
            QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
            duel.Animations.Enqueue(qa);
        }
    }

    public void StartManaHover(int cost, Team team) {
        if(team == Team.Player) {
            UIManager.Instance.Player.HoverMana(cost, DuelManager.Instance.MainDuel.GetStatus(Team.Player));
        }
    }

    public void StopManaHover(Team team) {
        if(team == Team.Player) {
            UIManager.Instance.Player.UnhoverMana(DuelManager.Instance.MainDuel.GetStatus(Team.Player));
        }
    }

    public void BounceScaleAnimation(DuelInstance duel, Transform t, float from, float to, float duration) {
        IEnumerator ie = BounceScale(t, from, to, duration);
        QueueableAnimation qa = new QueueableAnimation(ie, duration/6f);
        duel.Animations.Enqueue(qa);
    }

    public void UpdateUIAnimation(DuelInstance duel) {
        IEnumerator ie = UpdateUI(duel);
        QueueableAnimation qa = new QueueableAnimation(ie, 0.0f);
        duel.Animations.Enqueue(qa);
    }
}
