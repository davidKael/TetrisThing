using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    internal bool isActive = false;
    SpriteRenderer sr;
    internal Vector2Int pos;
    internal static Dictionary<Vector2Int, Box> All = new Dictionary<Vector2Int, Box>();

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ResetBox();
        pos = new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
        All.Add(pos, this);
    }

    internal void ResetBox()
    {
        isActive = false;
        
        sr.enabled = false;
        sr.color = Color.white;
    }

    internal void ActivateBox(Color color)
    {

        sr.color = color;
        sr.enabled = true;
        isActive = true;
    }

    internal bool IsOutside()
    {
        return pos.y > 20;
    }
    
}
