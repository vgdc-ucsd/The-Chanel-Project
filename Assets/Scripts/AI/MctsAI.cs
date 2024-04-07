using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MctsAI
{
    private CharStatus playerStatus, enemyStatus;

    public MctsAI(CharStatus player, CharStatus enemy) {
        playerStatus = player;
        enemyStatus = enemy;
    }

    public void MakeMove() {
        // TODO MCTS
        Board board = DuelManager.Instance.CurrentBoard;
        DuelManager.Instance.MainDuel.ProcessBoard(board, Team.Enemy);
    }
}
