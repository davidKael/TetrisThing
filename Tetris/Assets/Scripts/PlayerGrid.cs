using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrid : MonoBehaviour
{
    #region Fields
    internal Dictionary<Vector2Int, Box> Boxes = new Dictionary<Vector2Int, Box>();
    [SerializeField]internal bool ShowGhosts = true;
    #endregion

    #region Unity Methods
    private void Start()
    {
        SetUpBoxes();
    }
    #endregion

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
    
    /// <summary>
    /// Gets all boxes and store them in list
    /// </summary>
    protected void SetUpBoxes()
    {
        foreach (Box box in transform.GetComponentsInChildren<Box>())
        {
            Boxes.Add(box.Position, box);
            box.Grid = this;
        }            
    }

    internal void ResetGrid()
    {
        foreach(Box box in Boxes.Values)
        {
            box.ResetBox();
        }
    }
    #endregion

}
