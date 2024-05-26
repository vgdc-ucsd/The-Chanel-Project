using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClioEvent : MonoBehaviour
{
    public void HelpOut() {
        if(EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        Debug.Log("help out");


        EventManager.Instance.FinishEvent();
    }

    public void Misery() {
        if(EventManager.Instance.OptionSelected) return;
        else EventManager.Instance.OptionSelected = true;
        Debug.Log("misery");



        EventManager.Instance.FinishEvent();
    }
}
