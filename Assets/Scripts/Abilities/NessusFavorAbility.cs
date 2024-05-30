using UnityEngine;

[CreateAssetMenu(menuName = "Abilites/NessusFavorAbility")]
public class NessusFavorAbility : StatusEffect
{

    public override ActivationCondition Condition
    {
        get { return ActivationCondition.OnFinishAttack; }
    }

    // "Activate" is actually the finish condition, removes the extra attacks
    public override void Activate(UnitCard c, ActivationInfo info)
    {
        foreach (UnitCard card in info.DamagedCards)
        {
            PoisonEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.PoisonEffectTemplate);
            effect.AddEffect(card, info);
        }
        RemoveEffect(c, info);
    }

    public override void AddEffect(UnitCard c, ActivationInfo info)
    {
        base.AddEffect(c, info);
        duration = -1;


        ReapplyEffect(c, info);
    }

    public override void ReapplyEffect(UnitCard c, ActivationInfo info)
    {
    }
}
