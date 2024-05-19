using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavButtons : MonoBehaviour
{
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
