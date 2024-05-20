using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavButtons : MonoBehaviour
{
    public void PlayGame()
    {
        MenuScript.Instance.PlayGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void LoadMap()
    {
        MenuScript.Instance.LoadMap();
    }

    public void LoadTitle()
    {
        MenuScript.Instance.LoadTitle();
    }

    public void LoadInventory()
    {
        MenuScript.Instance.LoadInventory();
    }
}
