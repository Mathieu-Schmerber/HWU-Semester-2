using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private float _lifetime;

    private Renderer _renderer;

	private void Awake()
	{
		_renderer = GetComponent<Renderer>();
	}

	private void Start()
	{
		Material mat = _renderer.material;
		Color color = mat.color;
		Color target = new Color(color.r, color.g, color.b, 0);

		Tween.ShaderColor(mat, "_BaseColor", target, _lifetime, 0, completeCallback: () => Destroy(transform.parent.gameObject));
	}
}
