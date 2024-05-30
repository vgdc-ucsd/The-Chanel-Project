
using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "BossData")]
public class BossData : ScriptableObject
{
    public int stages = 2;
    public bool clearBoard = true;
    public bool buffCards = true;
    public bool finalBoss;
}