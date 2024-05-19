using Unity.VisualScripting;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public UnitCardInteractable UCITemplate;
    public SpellCardInteractable SCITemplate;


    public static int DECK_SIZE = 12;

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