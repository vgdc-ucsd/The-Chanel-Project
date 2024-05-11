using System;
using Unity.VisualScripting;
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
        OverkillDamage = 0;
        TotalDamage = 0;
    }

    public DuelInstance Duel;
    public int OverkillDamage;
    public int TotalDamage;
}

[Serializable]
public abstract class Ability : ScriptableObject
{
    public abstract void Activate(UnitCard c, ActivationInfo info);
    public abstract ActivationCondition Condition{ get; }
}


public abstract class StatusEffect : Ability
{
    public virtual void AddEffect(UnitCard c)
    {
        c.Abilities.Add(this);
        c.StatusEffects.Add(this);
    }


    public void RemoveEffect(UnitCard c)
    {
        c.Abilities.Remove(this);
        c.StatusEffects.Remove(this);

        c.RecalculateStats(); // very poorly optimized, consider recalculating stats once per turn
    }

    public abstract void ReapplyEffect(UnitCard c);
}