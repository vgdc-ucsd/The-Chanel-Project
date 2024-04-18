using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivationCondition {
    OnProcess,
    OnDeath,
    OnDraw,
    OnPlay,
    OnMove,
    OnReceiveDamage,
    OnDealDamage
}

[Serializable]
public abstract class Ability : ScriptableObject
{
    public abstract void Activate(Board b, UnitCard c);
    public abstract ActivationCondition Condition{ get; }
}
