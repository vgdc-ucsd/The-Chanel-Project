using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public static MenuScript Instance { get; set; }
    public Encounter CurrentEncounter;

    private static int TITLE_INDEX = 0;
    private static int MAP_INDEX = 1;
    private static int INVENTORY_INDEX = 6;

    public void Awake()
    {
        if (Instance != this && Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    // For Debugging & Swapping Scenes Easily
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadMap();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            LoadInventory();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMap()
    {
        SceneManager.LoadScene(MAP_INDEX);
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene(TITLE_INDEX);
    }

    public void LoadInventory()
    {
        SceneManager.LoadScene(INVENTORY_INDEX);
    }
}
