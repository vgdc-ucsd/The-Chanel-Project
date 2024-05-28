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
    public const int REWARD_INDEX = 8;

    public int PREV_INDEX;

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
        PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
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
        PersistentData.Instance.Init();
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
        PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
    }

    public void LoadNextScene()
    {
        PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadMap()
    {
        PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(MAP_INDEX);
    }

    public void LoadTitle()
    {
        PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(TITLE_INDEX);
    }

    public void LoadInventory()
    {
        if (SceneManager.GetActiveScene().buildIndex == MAP_INDEX || SceneManager.GetActiveScene().buildIndex == SHOP_INDEX)
        {
            PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(INVENTORY_INDEX);
        }
        else
        {
            Debug.Log("Cannot Load Inventory from this Scene");
        }
    }

    // Debug Only
    public void LoadShop()
    {
        PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(SHOP_INDEX);
    }

    public void LoadDuel()
    {
        PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(DUEL_INDEX);
    }

    //Previous scene loaded from
    public void LoadPrev()
    {
        SceneManager.LoadScene(PREV_INDEX);
        PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(PREV_INDEX);
    }

    public void LoadPrevFromInventory()
    {
        if (PREV_INDEX == MAP_INDEX || PREV_INDEX == SHOP_INDEX)
        {
            SceneManager.LoadScene(PREV_INDEX);
            PREV_INDEX = SceneManager.GetActiveScene().buildIndex;
        }
        else
        {
            Debug.Log("Cannot go back to this scene from Inventory");
        }

        Debug.Log(PREV_INDEX);
    }
}
