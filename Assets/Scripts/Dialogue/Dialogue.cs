using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string CharacterName;
    [TextArea(10, 100)]
    public string Line;
    public float TextSpeed = 20; // The time between when each character gets displayed in milliseconds
    public Sprite CharacterImage;
}
