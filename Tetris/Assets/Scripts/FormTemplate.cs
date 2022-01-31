using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFormation", menuName = "Create new formation")]
public class FormTemplate : ScriptableObject
{
    public Color Color;
    
    public List<Vector2Int> FormationPositions;
    public List<Vector2Int> RotationForm1;
    public List<Vector2Int> RotationForm2;
    public List<Vector2Int> RotationForm3;
    public bool IsRotatable()
    {
        return AmountOfRotations > 0;
    }

    public int AmountOfRotations = 0;

   
}
