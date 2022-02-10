using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    #region FieldsInEditor
    [SerializeField] ParticleSystem _particle;
    #endregion

    #region Properties
    internal bool IsActive { get; private set; } = false;
    internal Vector2Int Position { get; private set; }
    #endregion

    #region Fields
    internal PlayerGrid Grid;
    SpriteRenderer sr;
    #endregion

    #region UnityMethods
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ResetBox();
        Position = new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
    }
    #endregion

    #region Methods

    /// <summary>
    /// Called to reset box to default values
    /// </summary>
    /// <param name="isRemoved">if removed particle plays</param>
    internal void ResetBox(bool isRemoved = false)
    {
        IsActive = false;
        sr.enabled = false;
        sr.color = Color.white;
        if (isRemoved)
        {
            _particle.Play();
        } 

    }

    /// <summary>
    /// called to activate box
    /// </summary>
    /// <param name="color">set the box's new color</param>
    internal void ActivateBox(Color color)
    {
        sr.color = color;
        sr.enabled = true;
        IsActive = true;
    }

    /// <summary>
    /// Checks if box is outside of grid
    /// </summary>
    /// <returns></returns>
    internal bool IsOutside()
    {
        return Position.y > 20;
    }
    
    /// <summary>
    /// Make the box visible but transparent
    /// </summary>
    /// <param name="color">Set what color</param>
    internal void TurnGhost(Color color)
    { 
        sr.color = new Color(color.r, color.g, color.b, 0.25f);
        sr.enabled = true;
    }

    /// <summary>
    /// Pass box value on to a box below and reset itself
    /// </summary>
    /// <param name="rows">how many rows the value drops</param>
    internal void MoveDown(int rows)
    {
        if (IsActive)
        {
            Grid.Boxes[new Vector2Int(Position.x, Position.y - rows)].ActivateBox(sr.color);
            ResetBox();
        }
        
    }
    #endregion
}
