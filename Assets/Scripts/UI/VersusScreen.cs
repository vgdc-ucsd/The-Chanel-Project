using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusScreen : MonoBehaviour
{
    void Start()
    {
        Invoke("LoadScene", 2f);
    }

    void LoadScene()
    {
        MenuScript.Instance.LoadDuel();
    }
}
