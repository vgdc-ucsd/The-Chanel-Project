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
    public ActivationInfo(DuelInstance duel) {
        Duel = duel;
        TargetCard = null;
        OverkillDamage = 0;
        TotalDamage = 0;
    }

    public DuelInstance Duel;
    public UnitCard TargetCard;
    public int OverkillDamage;
    public int TotalDamage;
}

[Serializable]
public abstract class Ability : ScriptableObject
{
    public abstract void Activate(UnitCard c, ActivationInfo info);
    public abstract ActivationCondition Condition{ get; }
}