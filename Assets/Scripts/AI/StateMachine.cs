// Extremely simple base AI, huge W.I.P.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // Enemy variables
    private CharStatus status;

    // State variables
    // Current states: "attack", "defend" | potential state ideas: "pressure", "heal", "last resort"
    public string state = "attack";
    public float lowHealthThreshold = 50f;

    // Cards for the enemy to use
    List<Card> attackCards = new List<Card>();
    List<Card> defenseCards = new List<Card>();

    public StateMachine(CharStatus status) {
        this.status = status;
    }

    // Update is called once per frame
    void Update()
    {
        if (status.Health <= lowHealthThreshold) {
            state = "defend";
        } else {
            state = "attack";
        }
    }
}
