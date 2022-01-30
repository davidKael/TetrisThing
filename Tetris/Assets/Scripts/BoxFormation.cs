using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoxFormation 
{
    internal Dictionary<Vector2Int, Box> boxes = new Dictionary<Vector2Int, Box>();
    internal Vector2Int offset = new Vector2Int(4, 20);
    internal bool IsPlaced = false;

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
        }

           



       

    }

    internal void Move(Vector2Int velocity)
    {


        
        if (IsAbleToMoveOnHorizontal(velocity))
        {

            velocity.x = 0;
        }

       

        if (IsAbleToMoveOnVerticalAndHorizontal(velocity))
        {
            velocity.y = 0;

 

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

    bool IsAbleToMoveOnHorizontal(Vector2Int velocity)
    {
           //not out side the x-axis?                                                     /is aktive?                                                                                             //not already in formation
        return boxes.Keys.Any(pos => pos.x + velocity.x > 9 || pos.x + velocity.x < 0) || velocity.x != 0 && boxes.Keys.Any(pos => Box.All[new Vector2Int(pos.x + velocity.x, pos.y)].isActive && !boxes.ContainsKey(new Vector2Int(pos.x + velocity.x, pos.y)));
    }


    bool IsAbleToMoveOnVerticalAndHorizontal(Vector2Int velocity)
    {
                //not outside on y-axis                              //is active                                                   //is not already in formation     
        return boxes.Keys.Any(pos => pos.y + velocity.y < 0) || (boxes.Keys.Any(pos => Box.All[pos + velocity].isActive && !boxes.ContainsKey(pos + velocity)));
    }


    


}
