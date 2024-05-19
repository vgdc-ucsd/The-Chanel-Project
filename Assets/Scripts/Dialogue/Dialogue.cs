using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
    //public string CharacterName;
    [TextAreaAttribute]
    public string Line = "";
    public float TextSpeed = 20; // The time between when each character gets displayed in milliseconds
    //public Sprite CharacterImage;

    public Dialogue() {
        Line = "";
        TextSpeed = 20;
    }
}
