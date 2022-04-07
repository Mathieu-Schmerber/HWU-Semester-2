using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;

public class Blast : APoolableObject
{
	public enum Size
	{
		SMALL,
		MEDIUM,
		BIG
	}

	[SerializeField] private ParticleSystem _small;
	[SerializeField] private ParticleSystem _medium;
	[SerializeField] private ParticleSystem _big;

	private float _maxLifetime;
	private Dictionary<Size, ParticleSystem> _blasts;

	private void Awake()
	{
		_blasts = new Dictionary<Size, ParticleSystem>()
		{
			{Size.SMALL, _small},
			{Size.MEDIUM, _medium},
			{Size.BIG, _big}
		};
		_maxLifetime = _blasts.Values.Max(x => x.GetComponentsInChildren<ParticleSystem>(true, true).Max(x => x.main.startLifetime.constantMax));
	}

	public override void Init(object data)
	{
		Size size = (Size)data;
		_blasts[size].gameObject.SetActive(true);
		_blasts[size].Play(true);
		Invoke(nameof(Release), _maxLifetime);
	}

	protected override void OnReleasing()
	{
		base.OnReleasing();
		_small.gameObject.SetActive(false);
		_medium.gameObject.SetActive(false);
		_big.gameObject.SetActive(false);
	}

	public static void Spawn(Vector3 position, Size size) => ObjectPooler.Get(PoolIdEnum.BLAST, position, Quaternion.identity, size);
}
