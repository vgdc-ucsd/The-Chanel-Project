using System;
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

public struct ActivationInfo {
    public ActivationInfo(bool mainDuel) {
        IsMainDuel = mainDuel;
        OverkillDamage = 0;
        TotalDamage = 0;
    }

    public bool IsMainDuel;
    public int OverkillDamage;
    public int TotalDamage;
}

[Serializable]
public abstract class Ability : ScriptableObject
{
    public abstract void Activate(Board b, UnitCard c, ActivationInfo info);
    public abstract ActivationCondition Condition{ get; }
}