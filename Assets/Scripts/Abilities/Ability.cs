using System;
using UnityEngine;

public enum ActivationCondition {
    OnProcess, // applies effect every turn
    OnDeath,
    OnDraw,
    OnPlay,
    OnMove,
    OnReceiveDamage,
    OnDealDamage, // triggers once after landing each separate attack
    OnFinishAttack, // triggers once per turn after landing at least one attack
    OnTrigger, // can only be triggered externally
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