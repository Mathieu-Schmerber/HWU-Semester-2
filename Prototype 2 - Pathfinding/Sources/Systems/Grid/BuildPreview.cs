using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPreview : MonoBehaviour
{
    [SerializeField] private Material _previewMaterial;

	public void ConvertMaterials()
	{
		foreach (Renderer renderer in GetComponentsInChildren<MeshRenderer>())
		{
			Material[] materials = renderer.materials;

			for (int i = 0; i < materials.Length; i++)
				renderer.material = _previewMaterial;
		}
	}
}
