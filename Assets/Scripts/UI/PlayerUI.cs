using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ManaText;
    public Image[] ManaSprites;
    public Sprite ManaBlack;
    public Sprite ManaBlue;
    public Sprite ManaRed;
    public Sprite Highlighted;
    public RectTransform HealthIndicator;
    public bool IsEnemy = false;

    private Coroutine ManaEffect;
    private List<Transform> shakeTransforms;
    private List<Vector3> originalPositions;
    private List<Image> flickerSprites;
    private List<Color> originalColors;

    void Start() {
        shakeTransforms = new List<Transform>();
        originalPositions = new List<Vector3>();
        flickerSprites = new List<Image>();
        originalColors = new List<Color>();

        float distance = 90f;
        if(IsEnemy) HealthIndicator.localPosition = new Vector3(
            distance,
            HealthIndicator.localPosition.y,
            HealthIndicator.localPosition.z
        );
        else HealthIndicator.localPosition = new Vector3(
            -distance,
            HealthIndicator.localPosition.y,
            HealthIndicator.localPosition.z
        );
    }

    public void UpdateUI(CharStatus status) {
        HealthText.text = status.Health.ToString();
        ManaText.text = status.Mana.ToString();

        UpdateMana(status);
        
        if(status.Health <= 0 && !DuelManager.Instance.Settings.DisableWinning) {
            if(status.CharTeam == Team.Enemy) UIManager.Instance.PlayerWin();
            else UIManager.Instance.PlayerLose();

            AnimationManager.Instance.ClearQueue();
        }
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

    public void HoverMana(int cost, CharStatus status) {
        // Can afford
        if(cost <= status.Mana) {
            for(int i = 0; i < cost; i++) {
                ManaSprites[i].sprite = Highlighted;
                shakeTransforms.Add(ManaSprites[i].transform);
                originalPositions.Add(ManaSprites[i].transform.localPosition);
            }
            ManaEffect = StartCoroutine(ManaShake(shakeTransforms, originalPositions));
        }
        // Can't afford
        else {
            for(int i = 0; i < cost; i++) {
                ManaSprites[i].sprite = ManaRed;
                flickerSprites.Add(ManaSprites[i]);
                originalColors.Add(ManaSprites[i].color);
            }
            ManaEffect = StartCoroutine(ManaFlicker(flickerSprites));
        }
    }

    public void UnhoverMana(CharStatus status) {
        UpdateMana(status);
        StopManaEffect();
    }

    private void UpdateMana(CharStatus status) {
        for(int i = 0; i < ManaSprites.Length; i++) {
            if(i < status.ManaCapacity) {
                ManaSprites[i].sprite = ManaBlue;
                if(i < status.Mana) {
                    ManaSprites[i].color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                else {
                    ManaSprites[i].color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                }
            }
            else {
                ManaSprites[i].sprite = ManaBlack;
                ManaSprites[i].color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
        }
    }

    private IEnumerator ManaShake(List<Transform> shakeTransforms, List<Vector3> originalPositions) {
        float intensity = 0.75f;

        while(true) {
            for(int i = 0; i < shakeTransforms.Count; i++) {
                shakeTransforms[i].localPosition = originalPositions[i] + Random.insideUnitSphere * intensity;
            }
            yield return null;
        }
    }

    private IEnumerator ManaFlicker(List<Image> flickerSprites) {
        float startTime = Time.time;
        float duration = 1.0f;
        bool direction = true;
        Color white = Color.white;
        Color flicker = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Color col = Color.white;

        while(true) {
            float elapsedTime = Time.time - startTime;
            float t = elapsedTime/duration;

            if(direction) {
                col = Interpolation.Interpolate(white, flicker, t, InterpolationMode.Linear);

                if(elapsedTime > duration) {
                    direction = false;
                    startTime = Time.time;
                }
            }
            else {
                col = Interpolation.Interpolate(flicker, white, t, InterpolationMode.Linear);
                
                if(elapsedTime > duration) {
                    direction = true;
                    startTime = Time.time;
                }
            }

            for(int i = 0; i < flickerSprites.Count; i++) {
                flickerSprites[i].color = col;
            }

            yield return null;
        }
    }

    private void StopManaEffect() {
        if(ManaEffect != null) StopCoroutine(ManaEffect);

        for(int i = 0; i < shakeTransforms.Count; i++) {
            shakeTransforms[i].localPosition = originalPositions[i];
        }

        for(int i = 0; i < flickerSprites.Count; i++) {
            flickerSprites[i].color = originalColors[i];
        }

        shakeTransforms.Clear();
        originalPositions.Clear();
        flickerSprites.Clear();
        originalColors.Clear();
    }
}