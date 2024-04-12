using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

// Lets this function as the editor in the inspector for the Card script
[CustomEditor(typeof(UnitCard))]
public class CardEditor : Editor
{
    // Keeps track of which attacks, from the list of all attacks, are enabled for this card
    private bool[] enabledAttacks = new bool[AttackDirections.AllAttackDirections.Count];

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UnitCard c = (UnitCard)target;
   
        GUILayout.Label("Attack Directions (Right Facing)");

        // Changes margins so it forms a tightly packed grid
        GUIStyle style = new GUIStyle();
        style.margin.right = 1000;

        GUILayout.BeginHorizontal(style);
            GUILayout.BeginVertical();
                // Top Row, Left Column
                ShowToggle(AttackDirections.UpLeft, c);
                // Middle Row, Left Column
                ShowToggle(AttackDirections.Left, c);
                // Bottom Row, Left Column
                ShowToggle(AttackDirections.DownLeft, c);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
                // Top Row, Middle Column
                ShowToggle(AttackDirections.Up, c);
                // Middle Row, Middle Column (always false)
                GUILayout.Toggle(false, "");
                // Bottom Row, Middle Column
                ShowToggle(AttackDirections.Down, c);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
                // Top Row, Right Column
                ShowToggle(AttackDirections.UpRight, c);                
                // Middle Row, Right Column
                ShowToggle(AttackDirections.Right, c);
                // Bottom Row, Right Column
                ShowToggle(AttackDirections.DownRight, c);
            GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        // Loops through enabledAttacks and adds or removes them from the target card
        for(int i = 0; i < AttackDirections.AllAttackDirections.Count; i++) {
            Vector2Int atk = AttackDirections.AllAttackDirections[i];
            bool hasAtk = c.AttackDirections.Contains(atk);
            if(enabledAttacks[i]) {
                if(!hasAtk) c.AttackDirections.Add(atk);
            }
            else {
                if(hasAtk) c.AttackDirections.Remove(atk);
            }
        }
    }

    // Displays a toggle on the Unity Inspector
    private void ShowToggle(Vector2Int atkDir, UnitCard c) {
        enabledAttacks[AttackDirections.AllAttackDirections.IndexOf(atkDir)] = GUILayout.Toggle(c.AttackDirections.Contains(atkDir), "");
    }


}
