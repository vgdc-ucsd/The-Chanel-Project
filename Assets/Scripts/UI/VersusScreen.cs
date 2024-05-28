using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VersusScreen : MonoBehaviour
{
    public GameObject enemyImageObject;
    public TextMeshProUGUI enemyName;
    private Encounter currentEncounter;

    void Start()
    {
        PersistentData.Instance.SetEncounterStats();
        currentEncounter = PersistentData.Instance.CurrentEncounter;
        enemyName.text = currentEncounter.EncounterName;
        enemyImageObject.GetComponent<Image>().sprite = currentEncounter.EnemyArt;
        Invoke("LoadScene", 2f);
    }

    void LoadScene()
    {
        MenuScript.Instance.LoadDuel();
    }
}
