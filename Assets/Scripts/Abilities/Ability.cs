using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivationCondition {
    OnProcess,
    OnDeath,
    OnDraw,
    OnPlay,
    OnMove
}

[Serializable]
public abstract class Ability : ScriptableObject
{
    public abstract void Activate(Board b, Card c);
    public abstract ActivationCondition Condition{ get; }
}
