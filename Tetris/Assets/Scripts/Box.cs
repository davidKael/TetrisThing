using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    internal bool IsActive = false;
    SpriteRenderer sr;
    internal Vector2Int pos;
    public PlayerGrid grid;

    public GameObject EffectPrefab;
    [SerializeField] ParticleSystem ps;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ResetBox();
        pos = new Vector2Int(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
        
    }

    internal void ResetBox(bool isRemoved = false)
    {
        IsActive = false;
        sr.enabled = false;
        sr.color = Color.white;
        if (isRemoved)
        {

            // Instantiate(EffectPrefab, transform.position, Quaternion.identity);
            ps.Play();
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

    internal void MoveDown(int rows)
    {
        if (IsActive)
        {
            grid.Boxes[new Vector2Int(pos.x, pos.y - rows)].ActivateBox(sr.color);
            ResetBox();
        }
        
    }

}
