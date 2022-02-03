using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFormation", menuName = "Create new formation")]
public class FormTemplate : ScriptableObject
{
    public Color Color;  
    public List<Vector2Int> BasePositions;
    public List<Vector2Int> RotationPositions1;
    public List<Vector2Int> RotationPositions2;
    public List<Vector2Int> RotationPositions3;
    public int AmountOfRotations = 0;

    public bool IsRotatable()
    {
        return AmountOfRotations > 0;
    }


 
}
