using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public List<Encounter> Encounters;

    public static EncounterManager Instance;

     void Awake() {
        // Singleton
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Tried to create more than one instance of the EncounterManager singleton");
            Destroy(this);
        }
        else Instance = this;
    }
}
