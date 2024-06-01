using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
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
        if(PersistentData.Instance.PossibleEvents.Count == 0) {
            Debug.LogWarning("No available events!");
            FinishEvent();
            return;
        }

        GameObject selectedEvent;

        if(PersistentData.Instance.FirstEvent) {
            PersistentData.Instance.FirstEvent = false;
            selectedEvent = Instantiate(PersistentData.Instance.OutlawEvent);
        }
        else {
            int randomIndex = Random.Range(0, PersistentData.Instance.PossibleEvents.Count);
            selectedEvent = Instantiate(PersistentData.Instance.PossibleEvents[randomIndex].gameObject);
            PersistentData.Instance.CompletedEvents.Add(PersistentData.Instance.PossibleEvents[randomIndex]);
            PersistentData.Instance.PossibleEvents.RemoveAt(randomIndex);
        }

        selectedEvent.transform.SetParent(this.transform);
    }

    public void FinishEvent(int nextScene = MenuScript.MAP_INDEX) {
        if(MenuScript.Instance != null) MenuScript.Instance.LoadScene(nextScene);
        else Debug.LogWarning("Couldn't return to map");
    }
}
