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

    internal BoxFormation()
    {
        List<Vector2Int> startPos = new List<Vector2Int>();

        int formationID = RandomIndex();

        foreach(Vector2Int pos in AllFormTypes[formationID])
        {
            startPos.Add(pos + offset);
        }


        RefreshBoxes(startPos);

    }

    void RefreshBoxes(List<Vector2Int> newPositions)
    {
        if(newPositions.Any(pos => pos.y < 0) || (Box.All.Values.Any(box => box.isActive && newPositions.Contains(box.pos) && !boxes.ContainsKey(box.pos))))
        {
            IsPlaced = true;
            return;
        }
        else
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
                    boxes[newPos].ActivateBox();
                }
            }

        }

       

    }

    internal void Fall()
    {
        List<Vector2Int> falledPos = new List<Vector2Int>();
        foreach (Vector2Int pos in boxes.Keys)
        {
            falledPos.Add(new Vector2Int(pos.x, pos.y - 1));
        }

        RefreshBoxes(falledPos);
    }


    List<List<Vector2Int>> AllFormTypes = new List<List<Vector2Int>>()
    {
        //Box
        new List<Vector2Int>()
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        //Line
        new List<Vector2Int>()
        {
            new Vector2Int(0, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
            new Vector2Int(0, 3)
        },

        //L
        new List<Vector2Int>()
        {
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        //Reversed L
        new List<Vector2Int>()
        {
            new Vector2Int(-1, 0),
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 1)
        },

        //Z
        new List<Vector2Int>()
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 1)
        },

        //Reversed Z
        new List<Vector2Int>()
        {
            new Vector2Int(0, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

        //T
        new List<Vector2Int>()
        {
            new Vector2Int(0, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1)
        },

    };

    int RandomIndex()
    {
        System.Random random = new System.Random();

        return random.Next(0, AllFormTypes.Count);
    }


}
