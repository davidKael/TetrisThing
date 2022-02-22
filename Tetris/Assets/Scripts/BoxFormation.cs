using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoxFormation
{
    #region Properties
    internal FormTemplate Form { get; }
    internal List<Vector2Int> Positions { get; private set; } = new List<Vector2Int>();
    internal List<Vector2Int> ghosts { get; private set; } = new List<Vector2Int>();
    internal bool IsPlaced { get { return _isPlaced; }  }
    #endregion

    #region Fields
    PlayerGrid _grid;
    bool _isPlaced = false;
    int _rotation = 0;
    Vector2Int _startOffset = new Vector2Int(4, 21);
    Vector2Int _centerPos;
    Vector2Int[] _wallKicks = 
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
    #endregion

    #region Constructors
    internal BoxFormation(FormTemplate form, PlayerGrid grid)
    {
        Form = form;
        _grid = grid;
        List<Vector2Int> startPos = Form.BasePositions.ConvertAll(pos => pos += _startOffset);
        RefreshBoxes(startPos, Vector2Int.zero + _startOffset);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Called to instantly place form at ghostposition
    /// </summary>
    internal void InstaDrop()
    {
        RefreshBoxes(ghosts, Vector2Int.zero);
        _grid.PlaceForm(this, out _isPlaced);
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
            if (Positions.Count > 0)
            {
                //finds and removes all unwanted positions
                Positions.Where(pos => !newPositions.Contains(pos)).ToList().ForEach(unwanted =>
                {
                    _grid.Boxes[unwanted].ResetBox();
                    Positions.Remove(unwanted);
                });

            }

            //Actives all new positions and adds them to boxPositions
            newPositions.Where(pos => !Positions.Contains(pos)).ToList().ForEach(newPos =>
            {
                Positions.Add(newPos);
                _grid.Boxes[newPos].ActivateBox(Form.Color);
            });

            //Resets all old ghost-boxes except for those that are placed at any of the new positions
            ghosts.ForEach(ghostPos => { if (!Positions.Contains(ghostPos)) _grid.Boxes[ghostPos].ResetBox(); });

            //gets all new ghost-positions
            ghosts = new List<Vector2Int>(GetGhostPositions());


            if (_grid.ShowGhosts)
            {
                //turns all relevant boxes into ghosts except for those that are placed at any of the new positions
                ghosts.ForEach(ghostPos => { if (!Positions.Contains(ghostPos)) _grid.Boxes[ghostPos].TurnGhost(Form.Color); });
            }

            _centerPos = nexRotCenter;
        }

    }

    /// <summary>
    /// Calculates where all the ghostpositions are
    /// </summary>
    /// <returns></returns>
    List<Vector2Int> GetGhostPositions()
    {
        List<Vector2Int> ghostPositions = new List<Vector2Int>(Positions);

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
            _grid.PlaceForm(this, out _isPlaced);
        }
        else
        {
            List<Vector2Int> nextPositions = Positions.ConvertAll(pos => pos + velocity);
            Vector2Int newCenter = _centerPos + velocity;
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
        return Positions.Any(pos => pos.x + horizontalVel > 9 || pos.x + horizontalVel < 0) || horizontalVel != 0 && Positions.Any(pos => _grid.Boxes[new Vector2Int(pos.x + horizontalVel, pos.y)].IsActive && !Positions.Contains(new Vector2Int(pos.x + horizontalVel, pos.y)));
    }

    /// <summary>
    /// Checks if formation is is blocked on any axis
    /// </summary>
    /// <param name="velocity"></param>
    /// <returns></returns>
    bool IsBlocked(Vector2Int velocity)
    {
        //not outside on y-axis                              //is active                                                   //is not already in formation     
        return Positions.Any(pos => (pos.y + velocity.y < 0) || pos.x + velocity.x > 9 || pos.x + velocity.x < 0) || (Positions.Any(pos => _grid.Boxes[pos + velocity].IsActive && !Positions.Contains(pos + velocity)));
    }

    /// <summary>
    /// Checks if a single position is blocked on the Y-axis
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool IsPosBlockedOnVertical(Vector2Int pos)
    {
        //not outside on y-axis                                          //is active                                                   
        return (pos.y - 1 < 0) || (_grid.Boxes[new Vector2Int(pos.x, pos.y - 1)].IsActive && !Positions.Contains(new Vector2Int(pos.x, pos.y - 1)));
    }

    /// <summary>
    /// Checks if any position is over grid border
    /// </summary>
    /// <returns></returns>


    /// <summary>
    /// To rotate BoxFormation 
    /// </summary>
    /// <returns></returns>
    internal bool Rotate()
    {
        List<Vector2Int> wantedPositions = GetNextFormRotation(_rotation);

        if (Form.IsRotatable())
        {
            //checks if the rotation is able and if there is any wallKickVelocity needed
            if (IsRoomForWantedRotation(wantedPositions, out Vector2Int? wallKickVelocity))
            {
                //updates which rotation the form currently has and makes sure its within limits of the amount of rotations the form has 
                _rotation = _rotation + 1 > Form.AmountOfRotations ? 0 : _rotation + 1;

                //applies the wallKickVelocity to the new rotation and updates all wanted new positions
                wantedPositions = wantedPositions.ConvertAll(newPos => newPos + _centerPos + (Vector2Int)wallKickVelocity);

                if (wantedPositions.Count > 0)
                {
                    //To Update form to new positions and rotation
                    RefreshBoxes(wantedPositions, _centerPos + (Vector2Int)wallKickVelocity);
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
    bool IsRoomForWantedRotation(List<Vector2Int> orgWantedRotPos, out Vector2Int? wallKickVelocity)
    {
        int wallfloorKickTried = 0;

        while (true)
        {
            List<Vector2Int> newWallKickPositions = new List<Vector2Int>(orgWantedRotPos);

            for (int i = 0; i < newWallKickPositions.Count; i++)
            {
                newWallKickPositions[i] += _wallKicks[wallfloorKickTried];

            }

            if (newWallKickPositions.Any(pos => IsBlocked(pos)))
            {
                if (++wallfloorKickTried >= _wallKicks.Length)
                {

                    wallKickVelocity = null;
                    break;
                }
            }
            else
            {
                wallKickVelocity = _wallKicks[wallfloorKickTried];
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


