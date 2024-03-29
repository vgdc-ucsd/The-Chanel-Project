using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DuelSettings
{
    // General
    public bool SameSettingsForBothPlayers = true;
    public PlayerSettings Player;
    public PlayerSettings Enemy;
    
    // Board
    public int BoardRows = 3;
    public int BoardCols = 4;

    // Misc
    public bool EnemyGoesFirst = false;

    // Dev/Debug settings
    public bool EnablePVPMode = false;
    public bool RestrictPlacement = true;
    public bool UnlimitedMana = false;
    public bool ShowEnemyHand = false;
}
