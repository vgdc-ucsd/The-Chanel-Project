using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Queue<QueueableAnimation> animations = new Queue<QueueableAnimation>();
    private bool activelyPlaying = false;
    
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
    public void QueueAnimation(QueueableAnimation qa) {
        animations.Enqueue(qa);
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
    public IEnumerator CardAttack(Transform card, Vector2 atkDir, float duration) {
        if(card == null) yield break;

        InterpolationMode mode = InterpolationMode.Linear;
        Vector3 startPos = card.position;

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
        yield return SimpleTranslate(card, windupPos, windupDuration, mode);
        yield return SimpleTranslate(card, launchPos, launchDuration, mode);
        yield return SimpleTranslate(card, startPos, recoverDuration, mode);
        if(card == null) yield break;

        card.position = startPos;
    }
}
