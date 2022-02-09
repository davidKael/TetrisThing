using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoxFormation
{
    internal FormTemplate Form { get; }
    internal List<Vector2Int> boxPositions = new List<Vector2Int>();
    internal Vector2Int startOffset = new Vector2Int(4, 21);

    internal bool IsPlaced { get { return _isPlaced; } set { _isPlaced = value; if (_isPlaced) GameState.IsGameOver = IsPlacedOverLimit(); } }
    bool _isPlaced = false;

    PlayerGrid _grid;

    internal List<Vector2Int> ghosts = new List<Vector2Int>();
    Vector2Int centerPos;
    int rotation = 0;


    Vector2Int[] wallKicks = new Vector2Int[]
    {
        new Vector2Int(0, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),

        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),

        new Vector2Int(2, 0),
        new Vector2Int(-2, 0),

        new Vector2Int(2, 1),
        new Vector2Int(-2, 1),

        new Vector2Int(0, 2),

        new Vector2Int(1, 2),
        new Vector2Int(-1, 2),

        new Vector2Int(2, 2),
        new Vector2Int(-2, 2),
    };

    internal BoxFormation(FormTemplate form, PlayerGrid grid)
    {
        Form = form;
        _grid = grid;
        List<Vector2Int> startPos = Form.BasePositions.ConvertAll(pos => pos += startOffset);
        RefreshBoxes(startPos, Vector2Int.zero + startOffset);
    }


    #region Methods
    /// <summary>
    /// Called to instantly place form at ghostposition
    /// </summary>
    internal void InstaDrop()
    {
        RefreshBoxes(ghosts, Vector2Int.zero);
        IsPlaced = true;
    }

    /// <summary>
    /// Update form and grid with the new positions of the form
    /// </summary>
    /// <param name="newPositions">where form will move</param>
    /// <param name="nexRotCenter">where the new center of the form will be</param>
    void RefreshBoxes(List<Vector2Int> newPositions, Vector2Int nexRotCenter)
    {
        if (newPositions.Count > 0)
        {
            if (boxPositions.Count > 0)
            {
                //finds and removes all unwanted positions
                boxPositions.Where(pos => !newPositions.Contains(pos)).ToList().ForEach(unwanted =>
                {
                    _grid.Boxes[unwanted].ResetBox();
                    boxPositions.Remove(unwanted);
                });

            }

            //Actives all new positions and adds them to boxPositions
            newPositions.Where(pos => !boxPositions.Contains(pos)).ToList().ForEach(newPos =>
            {
                boxPositions.Add(newPos);
                _grid.Boxes[newPos].ActivateBox(Form.Color);
            });

            //Resets all old ghost-boxes except for those that are placed at any of the new positions
            ghosts.ForEach(ghostPos => { if (!boxPositions.Contains(ghostPos)) _grid.Boxes[ghostPos].ResetBox(); });

            //gets all new ghost-positions
            ghosts = new List<Vector2Int>(GetGhostPositions());


            if (_grid.ShowGhosts)
            {
                //turns all relevant boxes into ghosts except for those that are placed at any of the new positions
                ghosts.ForEach(ghostPos => { if (!boxPositions.Contains(ghostPos)) _grid.Boxes[ghostPos].TurnGhost(Form.Color); });
            }

            centerPos = nexRotCenter;
        }
    }

    /// <summary>
    /// Calculates where all the ghostpositions are
    /// </summary>
    /// <returns></returns>
    List<Vector2Int> GetGhostPositions()
    {
        List<Vector2Int> ghostPositions = new List<Vector2Int>(boxPositions);

        while (true)
        {
            if (ghostPositions.Any(ghostPos => IsPosBlockedOnVertical(ghostPos)))
            {
                break;


            }
            else
            {
                for (int i = 0; i < ghostPositions.Count; i++)
                {
                    ghostPositions[i] += new Vector2Int(0, -1);
                }

            }

        }

        return ghostPositions;

    }

    /// <summary>
    /// Called when form tries to move
    /// </summary>
    /// <param name="velocity">direction to where formation wants to move</param>
    internal void Move(Vector2Int velocity)
    {
        if (IsBlockedOnHorizontal(velocity.x))
        {
            velocity.x = 0;
        }

        if (IsBlocked(velocity))
        {
            velocity = Vector2Int.zero;
            IsPlaced = true;
        }

        if (!IsPlaced)
        {
            List<Vector2Int> nextPositions = boxPositions.ConvertAll(pos => pos + velocity);
            Vector2Int newCenter = centerPos + velocity;
            RefreshBoxes(nextPositions, newCenter);
        }
    }

    /// <summary>
    /// Checks if formation is blocked on X-axis
    /// </summary>
    /// <param name="horizontalVel"></param>
    /// <returns></returns>
    bool IsBlockedOnHorizontal(int horizontalVel)
    {
        //not out side the x-axis?                                                     /is aktive?                                                                                             //not already in formation
        return boxPositions.Any(pos => pos.x + horizontalVel > 9 || pos.x + horizontalVel < 0) || horizontalVel != 0 && boxPositions.Any(pos => _grid.Boxes[new Vector2Int(pos.x + horizontalVel, pos.y)].IsActive && !boxPositions.Contains(new Vector2Int(pos.x + horizontalVel, pos.y)));
    }

    /// <summary>
    /// Checks if formation is is blocked on any axis
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns></returns>
    bool IsBlocked(Vector2Int velocity)
    {
        //not outside on y-axis                              //is active                                                   //is not already in formation     
        return boxPositions.Any(pos => (pos.y + velocity.y < 0) || pos.x + velocity.x > 9 || pos.x + velocity.x < 0) || (boxPositions.Any(pos => _grid.Boxes[pos + velocity].IsActive && !boxPositions.Contains(pos + velocity)));
    }

    /// <summary>
    /// Checks if a single position is blocked on the Y-axis
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool IsPosBlockedOnVertical(Vector2Int pos)
    {
        //not outside on y-axis                                          //is active                                                   
        return (pos.y - 1 < 0) || (_grid.Boxes[new Vector2Int(pos.x, pos.y - 1)].IsActive && !boxPositions.Contains(new Vector2Int(pos.x, pos.y - 1)));
    }

    /// <summary>
    /// Checks if any position is over grid border
    /// </summary>
    /// <returns></returns>
    bool IsPlacedOverLimit()
    {
        return boxPositions.Any(pos => pos.y >= 20);
    }

    /// <summary>
    /// To rotate BoxFormation 
    /// </summary>
    /// <returns></returns>
    internal bool Rotate()
    {
        List<Vector2Int> wantedPositions = GetNextFormRotation(rotation);

        if (Form.IsRotatable())
        {
            //checks if the rotation is able and if there is any wallKickVelocity needed
            if (IsRoomForRotation(wantedPositions, out Vector2Int? wallKickVelocity))
            {
                //updates which rotation the form currently has and makes sure its within limits of the amount of rotations the form has 
                rotation = rotation + 1 > Form.AmountOfRotations ? 0 : rotation + 1;

                //applies the wallKickVelocity to the new rotation and updates all wanted new positions
                wantedPositions = wantedPositions.ConvertAll(newPos => newPos + centerPos + (Vector2Int)wallKickVelocity);

                if (wantedPositions.Count > 0)
                {
                    //To Update form to new positions and rotation
                    RefreshBoxes(wantedPositions, centerPos + (Vector2Int)wallKickVelocity);
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if there is room for rotation
    /// </summary>
    /// <param name="orgWantedRotPos">postions where new position would end up</param>
    /// <param name="wallKickVelocity">velocity needed to rotate</param>
    /// <returns></returns>
    bool IsRoomForRotation(List<Vector2Int> orgWantedRotPos, out Vector2Int? wallKickVelocity)
    {
        int wallfloorKickTried = 0;

        while (true)
        {
            List<Vector2Int> newWallKickPositions = new List<Vector2Int>(orgWantedRotPos);

            for (int i = 0; i < newWallKickPositions.Count; i++)
            {
                newWallKickPositions[i] += wallKicks[wallfloorKickTried];

            }

            if (newWallKickPositions.Any(pos => IsBlocked(pos)))
            {
                if (++wallfloorKickTried >= wallKicks.Length)
                {

                    wallKickVelocity = null;
                    break;
                }
            }
            else
            {
                wallKickVelocity = wallKicks[wallfloorKickTried];
                break;
            }


        }
        return wallKickVelocity != null;


    }

    /// <summary>
    /// Get next premade rotation of form
    /// </summary>
    /// <param name="currRot"></param>
    /// <returns></returns>
    List<Vector2Int> GetNextFormRotation(int currRot) 
    {
        //this is all the premade rotations for this BoxFormaiton 
        //this switch finds which the next rotation should be
        switch (currRot + 1)
        {
            case 1:
                return new List<Vector2Int>(Form.RotationPositions1);
                
            case 2:
                return new List<Vector2Int>(Form.RotationPositions2);
                
            case 3:
                return new List<Vector2Int>(Form.RotationPositions3);
                
            default:
                return new List<Vector2Int>(Form.BasePositions);
               
        }
    }
    #endregion
}


