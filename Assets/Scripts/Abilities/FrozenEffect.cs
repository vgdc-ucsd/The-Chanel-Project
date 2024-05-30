using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/FrozenEffect")]
public class FrozenEffect : StatusEffect
{
    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnBeginTurn; }
    }

    public override void Activate(UnitCard c, ActivationInfo info)
    {
        duration--;
        if (duration < 1)
        {
            RemoveEffect(c, info);
        }
        c.CanMove = false;
        c.CanAttack = false;
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
    }

    public override void AddEffect(UnitCard c, ActivationInfo info)
    {
        duration = initialDuration;
        bool hasEffect = false;
        foreach (StatusEffect s in c.StatusEffects)
        {
            if (s.GetType() == this.GetType())
            {
                s.duration += initialDuration;
                hasEffect = true;
                Debug.Log(s.duration);
                AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
                return;
            }
        }
        if (!hasEffect) {
            c.Abilities.Add(this);
            c.StatusEffects.Add(this);
        }
        AnimationManager.Instance.UpdateCardInfoAnimation(info.Duel, c);
        Debug.Log(duration);
    }

    public override void RemoveEffect(UnitCard c, ActivationInfo info)
    {
        c.CanMove = true;
        c.CanAttack = true;
        base.RemoveEffect(c, info);
    }


    public override void ReapplyEffect(UnitCard c, ActivationInfo info)
    {
    }
}