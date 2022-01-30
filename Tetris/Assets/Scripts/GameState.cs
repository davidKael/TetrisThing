using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    internal static bool IsGameOver = false;


    internal void StartGame()
    {
        IsGameOver = false;
    }



    private void Start()
    {
       
        StartGame();
    }
}
