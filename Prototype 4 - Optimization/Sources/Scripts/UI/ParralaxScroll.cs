using Nawlian.Lib.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParralaxScroll : MonoBehaviour
{
	[SerializeField] private float _scrollFactor;
    [SerializeField] private PlayerController _player;
    private RawImage _image;

	private void Awake()
	{
		_image = GetComponent<RawImage>();
	}

	private void Update()
    {
		if (_player.Velocity.magnitude > 0)
			_image.uvRect = new Rect(_image.uvRect.position + (_player.Velocity.magnitude * -_player.transform.up.ToVector2XY()) * _scrollFactor, _image.uvRect.size);
    }
}
