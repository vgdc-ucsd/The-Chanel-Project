using UnityEngine;

public class DebugTool : MonoBehaviour
{

    SpellBomb bombSpell;
    SpellPortal swapSpell;
    private void Awake()
    {
        bombSpell = ScriptableObject.CreateInstance<SpellBomb>();
        swapSpell = ScriptableObject.CreateInstance<SpellPortal>();
    }
    public void DebugAction()
    {

    }
}