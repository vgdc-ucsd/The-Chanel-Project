using UnityEngine;

// Abilty Description:
// When this card attacks another card, overkill damage is directly applied to the opponent's health
[CreateAssetMenu(menuName = "Abilites/SunWukongAbility")]
public class SunWukongAbility : Ability
{
    public override ActivationCondition Condition { get{return ActivationCondition.OnDealDamage;} }

    public override void Activate(UnitCard c, ActivationInfo Info)
    {
        if (Info.OverkillDamage > 0) {
            AnimationManager.Instance.AbilityActivateAnimation(Info.Duel, c);
            if(c.CurrentTeam == Team.Player) {
                AnimationManager.Instance.DamagePlayerAnimation(Info.Duel, Info.Duel.EnemyStatus, Info.OverkillDamage);
                Team winner = Info.Duel.EnemyStatus.TakeDamage(Info.OverkillDamage);
                if (winner != Team.Neutral) Info.Duel.Winner = winner;
            }
            else if(c.CurrentTeam == Team.Enemy) {
                AnimationManager.Instance.DamagePlayerAnimation(Info.Duel, Info.Duel.PlayerStatus, Info.OverkillDamage);
                Team winner = Info.Duel.PlayerStatus.TakeDamage(Info.OverkillDamage);
                if (winner != Team.Neutral) Info.Duel.Winner = winner;
            }
        }
    }
}
