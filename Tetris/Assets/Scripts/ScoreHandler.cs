using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreHandler : MonoBehaviour
{

    static internal void PlaceForm(BoxFormation bf)
    {
        List<int> rows = new List<int>();

        foreach(Box newBox in bf.boxes.Values)
        {
            if(!Box.All.Values.Any(box => !box.IsActive && box.pos.y == newBox.pos.y))
            {
                if (!rows.Contains(newBox.pos.y))
                {
                    rows.Add(newBox.pos.y);
                    
                }
            }
        }

        rows.Sort();
        /*
        if(rows.Count > 0)
        {
            int lowestRow = rows[0];
           
            for(int row = 0; row < rows.Count; row++)
            {
                for(int col = 0; col < 10; col++)
                {

                    Box.All[new Vector2Int(col, rows[row])].ResetBox(true);
                }
                if(rows[row] < lowestRow)
                {
                    lowestRow = rows[row];
                }
            }

            Box.MoveDownAll(lowestRow, rows.Count);
            rows.Clear();
        }
        */

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
