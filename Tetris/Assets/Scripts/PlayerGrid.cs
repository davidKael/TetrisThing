using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrid : MonoBehaviour
{
    internal Dictionary<Vector2Int, Box> Boxes = new Dictionary<Vector2Int, Box>();
    [SerializeField]internal bool ShowGhosts = true;

    private void Start()
    {
        SetUpBoxes();
    }

    void SetUpBoxes()
    {
        foreach(Box box in transform.GetComponentsInChildren<Box>())
        {
            Boxes.Add(box.pos, box);
            box.grid = this;
        }
    }


    internal void MoveDownAll(int lowestRow, int amount)
    {
        for (int row = lowestRow; row < 20; row++)
        {
            for (int col = 0; col < 10; col++)
            {
                // Debug.Log($"row={row},col={col}");
                Boxes[new Vector2Int(col, row)].MoveDown(amount);
            }
        }
    }

    internal void DeleteFormFromGrid(BoxFormation boxFormation)
    {
        boxFormation.boxPositions.ForEach(pos => Boxes[pos].ResetBox());
        boxFormation.ghosts.ForEach(ghostPos => Boxes[ghostPos].ResetBox());
    }


}
