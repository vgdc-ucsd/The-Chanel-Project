using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavButtons : MonoBehaviour
{
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


    public void LoadMap()
    {
        SceneManager.LoadScene(MenuScript.MAP_INDEX);
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene(MenuScript.TITLE_INDEX);
    }

    public void LoadInventory()
    {
        SceneManager.LoadScene(MenuScript.INVENTORY_INDEX);
    }
}
