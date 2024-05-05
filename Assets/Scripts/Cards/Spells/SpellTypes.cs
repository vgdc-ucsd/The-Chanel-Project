public interface ISpellTypeAny {
    public void CastSpell();
}
public interface ISpellTypeTile {
    public void CastSpell(BoardCoords pos);
}
public interface ISpellTypeUnit 
{
    public void CastSpell(UnitCard unit);
}
public interface ISpellTypeTwoUnits {
    public void CastSpell(UnitCard unit1, UnitCard unit2);
}
