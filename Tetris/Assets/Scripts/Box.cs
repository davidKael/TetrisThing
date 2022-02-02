using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    internal bool IsActive = false;
    SpriteRenderer sr;
    internal Vector2Int pos;
    internal static Dictionary<Vector2Int, Box> All = new Dictionary<Vector2Int, Box>();

    public GameObject EffectPrefab;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ResetBox();
        pos = new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
        All.Add(pos, this);
    }

    internal void ResetBox(bool isRemoved = false)
    {
        IsActive = false;
        sr.enabled = false;
        sr.color = Color.white;
        if (isRemoved)
        {
            Instantiate(EffectPrefab, transform.position, Quaternion.identity);
        }

        
    }

    internal void ActivateBox(Color color)
    {

        sr.color = color;
        sr.enabled = true;
        IsActive = true;
    }

    internal bool IsOutside()
    {
        return pos.y > 20;
    }
    
    internal void TurnGhost(Color color)
    {
        
        sr.color = new Color(color.r, color.g, color.b, 0.25f);
        sr.enabled = true;
    }

    void MoveDown(int rows)
    {
        if (IsActive)
        {
            Box.All[new Vector2Int(pos.x, pos.y - rows)].ActivateBox(sr.color);
            ResetBox();
        }
        
    }

    internal static void MoveDownAll(int lowestRow, int amount)
    {
        for (int row = lowestRow; row < 20; row++)
        {
            for (int col = 0; col < 10; col++)
            {
               // Debug.Log($"row={row},col={col}");
                All[new Vector2Int(col, row)].MoveDown(amount);
            }
        }
    }
}
