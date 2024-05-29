using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public enum VsScreenState {
    Win,
    Lose,
    VS
}

public class VersusScreen : MonoBehaviour
{
    public Sprite VsBG;
    public Sprite WinBG;
    public Sprite LoseBG;
    public Sprite VsLogo;
    public Sprite WinLogo;
    public Sprite LoseLogo;

    public Image Logo;
    public Image Background;

    public GameObject enemyImageObject;
    public TextMeshProUGUI enemyName;
    public FMODUnity.StudioEventEmitter AudioPlayer;
    private FMODUnity.StudioEventEmitter characterAudio;

    void Start()
    {
        if(PersistentData.Instance.VsState == VsScreenState.Win) {
            Background.sprite = WinBG;
            Logo.sprite = WinLogo;
            characterAudio = PersistentData.Instance.CurrentEncounter.WinAudio;
        }
        else if(PersistentData.Instance.VsState == VsScreenState.Lose) {
            Background.sprite = LoseBG;
            Logo.sprite = LoseLogo;
            characterAudio = PersistentData.Instance.CurrentEncounter.LoseAudio;
        }
        else {
            Background.sprite = VsBG;
            Logo.sprite = VsLogo;
            characterAudio = PersistentData.Instance.CurrentEncounter.EncounterAudio;

        }

        enemyName.text = PersistentData.Instance.CurrentEncounter.EncounterName;
        enemyImageObject.GetComponent<Image>().sprite = PersistentData.Instance.CurrentEncounter.EnemyArt;

        StartCoroutine(LoadSceneCoroutine(PersistentData.Instance.VsState));
    }

    void LoadScene(VsScreenState state)
    {
        if(state == VsScreenState.Win) {
            SceneManager.LoadScene(MenuScript.REWARD_INDEX);
        }
        else if(state == VsScreenState.Lose) {
            SceneManager.LoadScene(MenuScript.TITLE_INDEX);
        }
        else {
            SceneManager.LoadScene(MenuScript.DUEL_INDEX);
        }
    }

    private IEnumerator LoadSceneCoroutine(VsScreenState state) {
        if(characterAudio != null) {
            AudioPlayer = characterAudio;
            AudioPlayer.Play();
            AudioPlayer.EventDescription.getLength(out int audioLength);
            float waitTime = audioLength/1000f; // milliseconds to seconds
            yield return new WaitForSeconds(waitTime);
        }
        else {
            Debug.LogWarning("Missing audio for this encounter");
            yield return new WaitForSeconds(2f);
        }
        LoadScene(state);
    }
}
