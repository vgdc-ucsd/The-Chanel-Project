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
        Board board = DuelManager.Instance.MainDuel.DuelBoard; 
        // bombSpell.CastSpell(new BoardCoords(0, 0));
        if (board.IsOccupied(new BoardCoords(0, 0)) &&
            board.IsOccupied(new BoardCoords(1, 1)))
        {
            swapSpell.CastSpell(DuelManager.Instance.MainDuel, board.GetCard(0,0),board.GetCard(1,1));
            Debug.Log("Swapping");
        }
    }
}