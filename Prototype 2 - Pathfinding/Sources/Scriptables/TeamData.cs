using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Team")]
public class TeamData : ScriptableObject
{
    public string Name;
    public bool Playable;
    public Color PrimaryColor;
    public Color SecondaryColor;
    public Gradient ColorGradient;
}
