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

        List<Vector2Int> startPos = new List<Vector2Int>(Form.BasePositions);

        for (int i = 0; i < startPos.Count; i++)
        {
            startPos[i] += startOffset;
        }

        RefreshBoxes(startPos, Vector2Int.zero + startOffset);
    }

    internal void InstaDrop()
    {
        RefreshBoxes(ghosts, Vector2Int.zero);
        IsPlaced = true;
    }

    void RefreshBoxes(List<Vector2Int> newPositions, Vector2Int nexRotCenter)
    {
        if (newPositions.Count > 0)
        {
            if (boxPositions.Count > 0)
            {
                List<Vector2Int> removables = new List<Vector2Int>();

                foreach (Vector2Int pos in boxPositions)
                {

                    if (!newPositions.Contains(pos))
                    {

                        removables.Add(pos);

                    }

                }

                if (removables.Count > 0)
                {
                    foreach (Vector2Int removable in removables)
                    {
                        _grid.Boxes[removable].ResetBox();
                        boxPositions.Remove(removable);
                    }
                }
            }

            foreach (Vector2Int newPos in newPositions)
            {

                if (!boxPositions.Contains(newPos))
                {
                    boxPositions.Add(newPos);
                    _grid.Boxes[newPos].ActivateBox(Form.Color);
                }
            }

            foreach (Vector2Int ghostpos in ghosts)
            {
                if (!boxPositions.Any(pos => pos == ghostpos))
                {
                    _grid.Boxes[ghostpos].ResetBox();

                }
            }

            ghosts = new List<Vector2Int>(GetGhostPositions());

            for (int i = 0; i < ghosts.Count; i++)
            {
                if (!boxPositions.Any(pos => pos == ghosts[i]))
                {
                    _grid.Boxes[ghosts[i]].TurnGhost(Form.Color);
                }

            }

            centerPos = nexRotCenter;

        }

    }

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

    internal void Move(Vector2Int velocity)
    {
        if (IsBlockedOnHorizontal(velocity.x))
        {
            velocity.x = 0;
        }

        if (IsBlocked(velocity))
        {
            velocity = Vector2Int.zero;
            ghosts.Clear();
            IsPlaced = true;
        }

        if (velocity != Vector2Int.zero)
        {
            List<Vector2Int> nextPositions = new List<Vector2Int>();

            if (!IsPlaced)
            {
                Vector2Int newCenter = Vector2Int.zero;

                foreach (Vector2Int pos in boxPositions)
                {
                    nextPositions.Add(pos + velocity);
                }

                newCenter = centerPos + velocity;
                RefreshBoxes(nextPositions, newCenter);
            }

        }
    }

    bool IsBlockedOnHorizontal(int horizontalVel)
    {
        //not out side the x-axis?                                                     /is aktive?                                                                                             //not already in formation
        return boxPositions.Any(pos => pos.x + horizontalVel > 9 || pos.x + horizontalVel < 0) || horizontalVel != 0 && boxPositions.Any(pos => _grid.Boxes[new Vector2Int(pos.x + horizontalVel, pos.y)].IsActive && !boxPositions.Contains(new Vector2Int(pos.x + horizontalVel, pos.y)));
    }


    bool IsBlocked(Vector2Int velocity)
    {
        //not outside on y-axis                              //is active                                                   //is not already in formation     
        return boxPositions.Any(pos => (pos.y + velocity.y < 0) || pos.x + velocity.x > 9 || pos.x + velocity.x < 0) || (boxPositions.Any(pos => _grid.Boxes[pos + velocity].IsActive && !boxPositions.Contains(pos + velocity)));
    }

    bool IsPosBlockedOnVertical(Vector2Int pos)
    {
        //not outside on y-axis                                          //is active                                                   
        return (pos.y - 1 < 0) || (_grid.Boxes[new Vector2Int(pos.x, pos.y - 1)].IsActive && !boxPositions.Contains(new Vector2Int(pos.x, pos.y - 1)));
    }


    bool IsPlacedOverLimit()
    {
        return boxPositions.Any(pos => pos.y >= 20);
    }


    internal bool Rotate()
    {
        List<Vector2Int> wantedPositions = new List<Vector2Int>();

        if (Form.IsRotatable())
        {
            switch (rotation + 1 > Form.AmountOfRotations ? 0 : rotation + 1)
            {
                case 1:
                    wantedPositions = new List<Vector2Int>(Form.RotationPositions1);
                    break;
                case 2:
                    wantedPositions = new List<Vector2Int>(Form.RotationPositions2);
                    break;
                case 3:
                    wantedPositions = new List<Vector2Int>(Form.RotationPositions3);
                    break;
                default:
                    wantedPositions = new List<Vector2Int>(Form.BasePositions);
                    break;
            }


            if (IsRoomForRotation(wantedPositions, out Vector2Int? wallKickVelocity))
            {
                rotation = rotation + 1 > Form.AmountOfRotations ? 0 : rotation + 1;

                for (int i = 0; i < wantedPositions.Count; i++)
                {
                    Vector2Int newPos = wantedPositions[i] + centerPos + (Vector2Int)wallKickVelocity;
                    wantedPositions[i] = newPos;
                }

                if (wantedPositions.Count > 0)
                {
                    RefreshBoxes(wantedPositions, centerPos + (Vector2Int)wallKickVelocity);
                    return true;
                }
            }
        }
        return false;
    }

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


}


