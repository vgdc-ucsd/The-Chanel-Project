using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Spell", menuName = "Cards/BombSpell")]
public class SpellBomb : Spell , ISpellTypeTile
{
    int damage = 2;
    int area = 2;
    public void CastSpell(BoardCoords pos)
    {
        List<UnitCard> damagedCards = DuelManager.Instance.MainDuel.DuelBoard.GetCardsInSquare(pos, area);
        Debug.Log(damagedCards.ToCommaSeparatedString());
        foreach(UnitCard card in damagedCards)
        {
            DuelManager.Instance.MainDuel.DealDamage(card, damage);
        }
    }
}