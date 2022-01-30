using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoxFormation 
{
    internal Dictionary<Vector2Int, Box> boxes = new Dictionary<Vector2Int, Box>();
    internal List<Vector2Int> ghosts = new List<Vector2Int>();
    internal Vector2Int offset = new Vector2Int(5, 21);
    internal bool IsPlaced { get { return _isPlaced; } set { _isPlaced = value; if(_isPlaced) GameState.IsGameOver = IsPlacedOverLimit(); } }
    bool _isPlaced = false;
    FormTemplate _form;



    internal BoxFormation(FormTemplate form)
    {
        _form = form;

        List<Vector2Int> startPos = new List<Vector2Int>(_form.FormationPositions);

      
        for (int i = 0; i< startPos.Count; i++)
        {
            startPos[i] += offset;
        }


        RefreshBoxes(startPos);

    }

    internal void InstaDrop()
    {
        RefreshBoxes(ghosts);
        IsPlaced = true;

    }

    void RefreshBoxes(List<Vector2Int> newPositions)
    {
        if(newPositions.Count > 0)
        {
            if (boxes.Count > 0)
            {
                List<Vector2Int> removables = new List<Vector2Int>();

                foreach (KeyValuePair<Vector2Int, Box> item in boxes)
                {

                    if (!newPositions.Contains(item.Key))
                    {

                        removables.Add(item.Key);

                    }

                }

                if (removables.Count > 0)
                {
                    foreach (Vector2Int removable in removables)
                    {
                        Box.All[removable].ResetBox();
                        boxes.Remove(removable);
                    }
                }
            }

            foreach (Vector2Int newPos in newPositions)
            {

                if (!boxes.ContainsKey(newPos))
                {
                    boxes.Add(newPos, Box.All[newPos]);
                    boxes[newPos].ActivateBox(_form.Color);


                    

                }
            }

            foreach(Vector2Int ghostpos in ghosts)
            {
                if (!boxes.Keys.Any(pos => pos == ghostpos))
                {
                    Box.All[ghostpos].ResetBox();

                }
            }

            ghosts = new List<Vector2Int>(GetGhostPositions());

            for(int i = 0; i < ghosts.Count; i++)
            {
                if(!boxes.Keys.Any(pos => pos == ghosts[i]))
                {
                    Box.All[ghosts[i]].TurnGhost(_form.Color);
                }
                
            }

            
        }

    }

    List<Vector2Int> GetGhostPositions()
    {
        List<Vector2Int> ghostPositions = new List<Vector2Int>(boxes.Keys);

        

        while (true)
        {
            if (ghostPositions.Any(ghostPos => IsPosBlockedOnVertical(ghostPos)))
            {
                break;
                

            }
            else
            {
                for(int i = 0; i < ghostPositions.Count; i++)
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

        if(velocity != Vector2Int.zero)
        {
            List<Vector2Int> nextPositions = new List<Vector2Int>();

            foreach (Vector2Int pos in boxes.Keys)
            {
                nextPositions.Add(pos + velocity);
            }

            if (!IsPlaced)
            {

                RefreshBoxes(nextPositions);
            }


        }




    }

    bool IsBlockedOnHorizontal(int horizontalVel)
    {
           //not out side the x-axis?                                                     /is aktive?                                                                                             //not already in formation
        return boxes.Keys.Any(pos => pos.x + horizontalVel > 9 || pos.x + horizontalVel < 0) || horizontalVel != 0 && boxes.Keys.Any(pos => Box.All[new Vector2Int(pos.x + horizontalVel, pos.y)].isActive && !boxes.ContainsKey(new Vector2Int(pos.x + horizontalVel, pos.y)));
    }


    bool IsBlocked(Vector2Int velocity)
    {
                //not outside on y-axis                              //is active                                                   //is not already in formation     
        return boxes.Keys.Any(pos => pos.y + velocity.y < 0) || (boxes.Keys.Any(pos => Box.All[pos + velocity].isActive && !boxes.ContainsKey(pos + velocity)));
    }

    bool IsPosBlockedOnVertical(Vector2Int pos)
    {
        //not outside on y-axis                                          //is active                                                   
        return (pos.y - 1 < 0) || (Box.All[new Vector2Int(pos.x, pos.y - 1)].isActive && !boxes.ContainsKey(new Vector2Int(pos.x, pos.y -1)));
    }


    bool IsPlacedOverLimit()
    {
        return boxes.Keys.Any(pos => pos.y >= 20);
    }

}
