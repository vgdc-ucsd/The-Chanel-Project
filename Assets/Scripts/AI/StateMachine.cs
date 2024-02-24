// Extremely simple base AI, huge W.I.P.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Enemy variables
    public float health = 100f;
    public float mana = 1f;

    // State variables
    public string state = "attack";
    public float lowHealth = 50f;

    // Cards for the enemy to use
    List<string> attackCards = new List<string>() {"Card1", "Card2", "Card3", "..."};
    List<string> defenseCards = new List<string>() { "Card1", "Card2", "Card3", "..." };

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("works!");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("works!");
        if (health <= lowHealth)
        {
            // use a card from defenseCards
        } else {
            // use a card from attackCards
        }
    }
}
