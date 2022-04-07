using Nawlian.Lib.Systems.Pooling;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldGainText : APoolableObject
{
	private TextMesh _text;

	private void Awake()
	{
		_text = GetComponent<TextMesh>();
	}

	public override void Init(object data)
	{
		_text.text = $"+{(int)data}<color=\"orange\">¤</color>";
		Tween.LocalScale(transform, Vector3.one, Vector3.one * 1.3f, .4f, 0, Tween.EaseBounce, completeCallback: () => Release());
	}

	public static void Spawn(Vector3 position, int gain) 
		=> ObjectPooler.Get(PoolIdEnum.GOLD_GAIN_TEXT, position, Quaternion.Euler(0, 0, Random.Range(-30, 30)), gain);
}
