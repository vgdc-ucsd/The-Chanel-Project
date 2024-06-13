using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    [HideInInspector] public Transform center;
    public Encounter encounter;

    PersistentData pd;

    public void Start() {
        pd = PersistentData.Instance;
        center = transform;
    }

    // The next time an encounter is loaded, player will fight this character
    public void FightCharacter() {
        pd.SetNextEncounter(encounter);
    }

    // Removes this character from current run, player should no longer encounter them
    public void KillCharacter() {
        if (pd.possibleEncounters.Contains(encounter)) pd.possibleEncounters.Remove(encounter);
        if (pd.completedEncounters.Contains(encounter)) pd.completedEncounters.Remove(encounter);
    }

    // Player will only encounter this character later when every other character has been met
    public void AcknowledgeCharacter() {
        if (pd.possibleEncounters.Contains(encounter)) pd.possibleEncounters.Remove(encounter);
        if (!pd.completedEncounters.Contains(encounter)) pd.completedEncounters.Add(encounter);
    }
}
