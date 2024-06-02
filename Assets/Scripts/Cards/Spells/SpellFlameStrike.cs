using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Flame Strike Spell Card", menuName = "Cards/SpellFlameStrike")]
public class SpellFlameStrike : SpellCard , ISpellTypeTile
{
    public bool CastSpell(DuelInstance duel, BoardCoords pos)
    {
        // Board board = duel.DuelBoard;
        // if (board.GetCardsInRow(pos.y).Count < 1) return false;
        // List<UnitCard> targets = new List<UnitCard>();
        // foreach (UnitCard uc in board.GetCardsInRow(pos.y)) {
        //     if (uc.CurrentTeam != CurrentTeam) {
        //         targets.Add(uc);
        //     }
        // }
        // if (targets.Count < 1) return false;

        StartCast(duel, pos);

        List<UnitCard> damagedCards = duel.DuelBoard.GetCardsInRow(pos.y);
        foreach(UnitCard card in damagedCards)
        {
            ActivationInfo info = new ActivationInfo(duel);
            FireEffect effect = ScriptableObject.Instantiate(DuelManager.Instance.Effects.FireEffectTemplate);
            effect.AddEffect(card, info);
        }



        FinishCast(duel);
        return true;

    }
}