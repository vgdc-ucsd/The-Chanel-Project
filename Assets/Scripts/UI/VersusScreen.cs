using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private Encounter currentEncounter;

    void Start()
    {
        if(PersistentData.Instance.VsState == VsScreenState.Win) {
            Background.sprite = WinBG;
            Logo.sprite = WinLogo;
        }
        else if(PersistentData.Instance.VsState == VsScreenState.Lose) {
            Background.sprite = LoseBG;
            Logo.sprite = LoseLogo;
        }
        else {
            PersistentData.Instance.SetEncounterStats();

            Background.sprite = VsBG;
            Logo.sprite = VsLogo;
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
        yield return new WaitForSeconds(2f);
        LoadScene(state);
    }
}
