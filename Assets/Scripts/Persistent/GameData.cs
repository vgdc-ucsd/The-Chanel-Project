using Unity.VisualScripting;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public UnitCardInteractable UCITemplate;
    public SpellCardInteractable SCITemplate;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

}