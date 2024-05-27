using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public static MenuScript Instance { get; set; }

    public const int TITLE_INDEX = 0;
    public const int MAP_INDEX = 1;
    public const int DUEL_INDEX = 2;
    public const int SHOP_INDEX = 3;
    public const int EVENT_INDEX = 4;
    public const int INVENTORY_INDEX = 6;
    public const int VERSUS_INDEX = 7;

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
        if (Input.GetKeyDown(KeyCode.W))
        {
            UIManager.Instance.PlayerWin();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            LoadShop();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
        PersistentData.Instance.mapInfo.nodePoints = new();
        PersistentData.Instance.mapInfo.nodeTypes = new();
        PersistentData.Instance.mapInfo.nodeConnections = new();
        PersistentData.Instance.mapInfo.lastVisitedNode = new(0, 0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
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

    // Debug Only
    public void LoadShop()
    {
        SceneManager.LoadScene(SHOP_INDEX);
    }

    public void LoadDuel()
    {
        SceneManager.LoadScene(DUEL_INDEX);
    }
}
