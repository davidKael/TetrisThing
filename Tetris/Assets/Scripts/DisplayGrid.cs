using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayGrid : PlayerGrid
{

    internal void DisplayForm(FormTemplate newForm)
    {
        ResetGrid();

        List<Vector2Int> positions = newForm.BasePositions.ConvertAll(pos => pos += new Vector2Int(1, 1));
        positions.ForEach(pos => Boxes[pos].ActivateBox(newForm.Color));
    }

}
