using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public GameObject[] Events;
    public bool OptionSelected = false;

    public static EventManager Instance;

    void Awake() {
        OptionSelected = false;

        // Singleton
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Tried to create more than one instance of the EventManager singleton");
            Destroy(this);
        }
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        int randomIndex = Random.Range(0, Events.Length);
        GameObject selectedEvent = Instantiate(Events[randomIndex]);
        selectedEvent.transform.SetParent(this.transform);
    }

    public void FinishEvent(bool returnToMap = true) {
        if (returnToMap) {
            if(MenuScript.Instance != null) MenuScript.Instance.LoadMap();
            else Debug.LogWarning("Couldn't return to map");
        }
    }
}
