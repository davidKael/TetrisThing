using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBase : MonoBehaviour
{
    #region Fields
    internal Dictionary<Vector2Int, Box> Boxes = new Dictionary<Vector2Int, Box>();
    [SerializeField] internal bool ShowGhosts = true;
    #endregion

    #region Unity Methods
    private void Start()
    {
        SetUpBoxes();
    }
    #endregion

    /// <summary>
    /// Gets all boxes and store them in list
    /// </summary>
    protected virtual void SetUpBoxes()
    {
        foreach (Box box in transform.GetComponentsInChildren<Box>())
        {
            Boxes.Add(box.Position, box);
            box.Grid = this;
        }
    }

    internal virtual void ResetGrid()
    {
        foreach (Box box in Boxes.Values)
        {
            box.ResetBox();
        }
    }
}
