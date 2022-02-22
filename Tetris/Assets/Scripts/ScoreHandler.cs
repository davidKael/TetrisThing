using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreHandler
{
    #region Methods
    /// <summary>
    /// calculates which rows got full after a formation was placed, then updates the whole grid
    /// </summary>
    /// <param name="form">form that was placed</param>
    /// <param name="grid">on which grid it was placed</param>


    internal static int CalculateScore(int rows, int level = 1)
    {
        int sum;
        
        switch (rows)
        {
            case 1:
                sum = 100 * level;
                break;
            case 2:
                sum = 300 * level;
                break;
            case 3:
                sum = 500 * level;
                break;
            case 4:
                sum = 800 * level;
                break;
            default:
                sum = 0;
                break;
        }
        return sum;
    }
    #endregion
}
