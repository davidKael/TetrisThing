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

    }
    #endregion
}
