public interface ISpellTypeAny {
    public bool CastSpell(DuelInstance duel);
}
public interface ISpellTypeTile {
    public bool CastSpell(DuelInstance duel, BoardCoords pos);
}
public interface ISpellTypeUnit 
{
    public bool CastSpell(DuelInstance duel, UnitCard unit);
}

public interface ISpellTypeAlly : ISpellTypeUnit { }

public interface ISpellTypeEnemy : ISpellTypeUnit { }

public interface ISpellTypeTwoUnits {
    public bool CastSpell(DuelInstance duel, UnitCard unit1, UnitCard unit2);
}
