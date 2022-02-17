using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerGrid : GridBase
{

    [SerializeField] int _score;
    [SerializeField] TextMeshProUGUI _scoreText;

    #region Methods
    /// <summary>
    /// called to move down all active boxes on grid
    /// </summary>
    /// <param name="lowestRow">lowest row to move down</param>
    /// <param name="amount">how many rows to move</param>
    internal void MoveDownAll(int lowestRow, int amount)
    {
        for (int row = lowestRow; row < 20; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                Boxes[new Vector2Int(col, row)].MoveDown(amount);
            }
        }
    }
    
    /// <summary>
    /// Clear grid from given Formation
    /// </summary>
    /// <param name="boxFormation"></param>
    internal void DeleteFormFromGrid(BoxFormation boxFormation)
    {
        boxFormation.Positions.ForEach(pos => Boxes[pos].ResetBox());
        boxFormation.ghosts.ForEach(ghostPos => Boxes[ghostPos].ResetBox());
    }
    
    internal void AddToScore(int addedScore)
    {
        _score += addedScore;
        _scoreText.text = _score.ToString();
    }

    #endregion

}
