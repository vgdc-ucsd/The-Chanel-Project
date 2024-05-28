using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ActivationCondition {
    OnProcess, // applies effect every turn during attack phase
    OnBeginTurn,// applies effect at start of every turn of the card's team
    OnBeginOppositeTurn,
    OnEndTurn, // applies after process
    OnDeath,
    OnDraw,
    OnPlay,
    OnMove,
    OnReceiveDamage,
    OnAttacksHitMe, // triggers when any Attacker hits you, gives Attacker reference
    OnAttack,       // triggers just before Attack hits
    OnDealDamage,   // triggers once after landing each separate attack
    OnFinishAttack, // triggers once per turn after landing at least one attack
    OnTrigger,      // can only be triggered externally
}

public struct ActivationInfo {
    public ActivationInfo(DuelInstance duel) {
        Duel = duel;
        TargetCard = null;
        OverkillDamage = 0;
        TotalDamage = 0;
        DamagedCards = new List<UnitCard>();
    }

    public DuelInstance Duel;
    public UnitCard TargetCard;
    public int OverkillDamage;
    public int TotalDamage;
    public List<UnitCard> DamagedCards;
}

[Serializable]
public abstract class Ability : ScriptableObject
{
    public abstract void Activate(UnitCard c, ActivationInfo info);
    public abstract ActivationCondition Condition{ get; }
}


public abstract class StatusEffect : Ability
{
    public int duration;
    public Sprite icon;

    public StatusEffect Clone()
    {
        StatusEffect copy = ScriptableObject.Instantiate(this);
        copy.duration = duration;
        copy.icon = icon;
        CloneExtras(copy);
        return copy;
    }

    protected virtual void CloneExtras(StatusEffect copy) { }

    public virtual void AddEffect(UnitCard c, ActivationInfo info)
    {
        foreach (StatusEffect s in c.StatusEffects)
        {
            if (s.GetType() == this.GetType())
            {
                return;
            }
        }
        c.Abilities.Add(this);
        c.StatusEffects.Add(this);
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
    }


    public void RemoveEffect(UnitCard c, ActivationInfo info)
    {
        c.Abilities.Remove(this);
        c.StatusEffects.Remove(this);

        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
        c.RecalculateStats(info); // very poorly optimized, consider recalculating stats once per turn
    }

    public abstract void ReapplyEffect(UnitCard c, ActivationInfo info);

}
