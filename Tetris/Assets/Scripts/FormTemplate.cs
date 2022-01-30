using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFormation", menuName = "Create new formation")]
public class FormTemplate : ScriptableObject
{
    public Color Color;


    public List<Vector2Int> FormationPositions;

}
