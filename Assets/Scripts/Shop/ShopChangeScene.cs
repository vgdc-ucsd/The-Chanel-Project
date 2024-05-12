using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Configures everything scene wise for Shop
 * Can Remove Later and Merge into Shop Manager if few functions needed
 */
public class ShopChangeScene : MonoBehaviour
{
    // Scene Change back to Map
    public void leaveShop()
    {
        SceneManager.LoadScene(1);
    }

    // Scene Change for Deck/Inventory
    public void openDeck()
    {
        SceneManager.LoadScene(6);
    }
}
