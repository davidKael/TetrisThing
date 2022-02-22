using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState : MonoBehaviour
{
    int _score;
    bool _isPlayerAlive;





    #region Properties
    internal int Score { get { return _score; } }

    #endregion

    private void Start()
    {
        _isPlayerAlive = true;
    }


    internal void AddToScore(int addedScore)
    {
        int sum = _score += addedScore;
        _score = sum <= 0 ? 0 : sum;
    }



    internal bool IsPlayerInControl()
    {
        return _isPlayerAlive && !GameState.IsGameOver;
    }

    internal void PlayerLost()
    {
        _isPlayerAlive = false;
    }
}
