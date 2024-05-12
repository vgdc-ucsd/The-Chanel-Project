using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SycamoreEvent : MonoBehaviour
{
    public void EndEvent() {
        EventManager.Instance.FinishEvent();
    }
}
