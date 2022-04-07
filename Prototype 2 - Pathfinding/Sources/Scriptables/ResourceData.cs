using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Resource")]
public class ResourceData : ScriptableObject
{
	public string Name;

	public GameObject GFX;
	public ToggleableValue<int> WoodGain;
	public ToggleableValue<int> MetalGain;
}