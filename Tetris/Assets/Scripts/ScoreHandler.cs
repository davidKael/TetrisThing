using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreHandler
{

    static internal void PlaceForm(BoxFormation bf, PlayerGrid grid)
    {
        List<int> rows = new List<int>();

        foreach(Vector2Int placedPos in bf.boxPositions)
        {
            if(!grid.Boxes.Values.Any(box => !box.IsActive && box.pos.y == placedPos.y))
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

}
