using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public static StartMenu Instance;
    public PersistentData startDataTemplate;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlayGame()
    {
        if (PersistentData.Instance != null)
        {
            Destroy(PersistentData.Instance.gameObject);
            PersistentData.Instance = null;
            Debug.Log("old persistent data destroyed");
        }
        Instantiate(startDataTemplate);

        PersistentData.Instance.Init();
        PersistentData.Instance.mapInfo.nodePoints = new();
        PersistentData.Instance.mapInfo.nodeTypes = new();
        PersistentData.Instance.mapInfo.nodeConnections = new();
        PersistentData.Instance.mapInfo.nodeVisited = new();
        PersistentData.Instance.mapInfo.lastVisitedNode = new(0, 0);
        SceneManager.LoadScene(1);
    }
}
