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
    private bool[] enabledAttacks = new bool[8]; // each of the 8 basic directions

    private Vector2Int[] basicAttackDirections = {
        new Vector2Int(-1, 1), // Up Left, 0
        new Vector2Int(0, 1), // Up, 1
        new Vector2Int(1, 1), // Up Right, 2
        new Vector2Int (-1, 0), // Left, 3
        new Vector2Int(1, 0), // Right, 4
        new Vector2Int(-1, -1), // Down Left, 5 
        new Vector2Int(0, -1), // Down, 6
        new Vector2Int(1, -1), // Down Right, 7
    };

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
                ShowToggle(basicAttackDirections[0], 0, c);
                // Middle Row, Left Column
                ShowToggle(basicAttackDirections[3], 3, c);
                // Bottom Row, Left Column
                ShowToggle(basicAttackDirections[5], 5, c);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
                // Top Row, Middle Column
                ShowToggle(basicAttackDirections[1], 1, c);
                // Middle Row, Middle Column (always false)
                GUILayout.Toggle(false, "");
                // Bottom Row, Middle Column
                ShowToggle(basicAttackDirections[6], 6, c);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
                // Top Row, Right Column
                ShowToggle(basicAttackDirections[2], 2, c);                
                // Middle Row, Right Column
                ShowToggle(basicAttackDirections[4], 4, c);
                // Bottom Row, Right Column
                ShowToggle(basicAttackDirections[7], 7, c);
            GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        int newDamage = EditorGUILayout.IntField("Base Damage", c.BaseDamage);
        if(newDamage != c.BaseDamage) {
            c.BaseDamage = newDamage;
            foreach(Attack atk in c.Attacks) {
                atk.damage = newDamage;
            }
        }

        // Loops through enabledAttacks and adds or removes them from the target card
        for(int i = 0; i < 8; i++) {
            Attack atk = new Attack(basicAttackDirections[i], c.BaseDamage);
            bool hasAtk = HasAttackWithSameDirection(c.Attacks, atk);
            if(enabledAttacks[i]) {
                if(!hasAtk) c.Attacks.Add(atk);
            }
            else {
                if(hasAtk) c.Attacks.Remove(atk);
            }
        }
    }

    // Displays a toggle on the Unity Inspector
    private void ShowToggle(Vector2Int atkDir, int index, UnitCard c) {
        enabledAttacks[index] = GUILayout.Toggle(c.Attacks.Contains(new Attack(atkDir, c.BaseDamage)), "");
    }

    private bool HasAttackWithSameDirection(List<Attack> attacks, Attack atk) {
        foreach(Attack a in attacks) {
            if(a.direction == atk.direction) return true;
        }

        return false;
    }
}
