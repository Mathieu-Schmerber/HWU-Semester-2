using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResizeGridShader : MonoBehaviour
{
	[SerializeField] private string _parameterName = "_Tiling";

	private void Awake() => Resize();

	[Button("Adjust grid")]
    private void Resize()
	{
		Renderer host = GetComponentsInChildren<Renderer>().Where(x => x.sharedMaterial.HasProperty(_parameterName)).FirstOrDefault();

		if (host == null)
			return;

		Material shader = host.sharedMaterial;
		Vector3 scale = new Vector3(host.transform.parent.localScale.x, host.transform.parent.localScale.z, 0);

		shader.SetVector(_parameterName, scale);
	}
}
