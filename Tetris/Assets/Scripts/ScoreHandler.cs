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
    static internal void PlaceForm(BoxFormation form, PlayerGrid grid)
    {
        List<int> rows = new List<int>();

        foreach (Vector2Int placedPos in form.Positions)
        {
            if(!grid.Boxes.Values.Any(box => !box.IsActive && box.Position.y == placedPos.y))
            {
                if (!rows.Contains(placedPos.y))
                {
                    rows.Add(placedPos.y);
                    
                }
            }
        }

        rows.Sort();

        for (int row = 0; row < rows.Count; row++)
        {
            for (int col = 0; col < 10; col++)
            {

                grid.Boxes[new Vector2Int(col, rows[row])].ResetBox(true);
            }

        }

        int rowsMoved = 0;
        for (int row = 0; row < rows.Count; row++)
        {
            grid.MoveDownAll(rows[row] -rowsMoved, 1);
            rowsMoved++;
        }

        if(rows.Count > 0)
        {
            grid.AddToScore(CalculateScore(rows.Count));
        }

    }

    static int CalculateScore(int rows, int level = 1)
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
