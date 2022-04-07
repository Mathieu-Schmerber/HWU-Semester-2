using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour
{
	[SerializeField] private GameObject _buildItemPrefab;
	[SerializeField] private GridLayoutGroup _list;
	[SerializeField] private Text _woodAmount;
	[SerializeField] private Text _metalAmount;

	private int _prevWood;
	private int _prevMetal;
	private EntityData[] _builds;

	private void Awake()
	{
		_builds = ResourceLoader.GetEntities(x => x.IsBuild.Enabled);
	}

	private void Start()
	{
		foreach (EntityData item in _builds)
		{
			GameObject instance = Instantiate(_buildItemPrefab, _list.transform);

			instance.GetComponent<BuildItemUI>().Init(item);
		}
	}

	private void Update()
	{
		if (GameManager.Instance.WoodAmount != _prevWood)
		{
			_woodAmount.text = GameManager.Instance.WoodAmount.ToString();
			iTween.PunchScale(_woodAmount.transform.parent.gameObject, new Vector3(.8f, .8f, .8f), .5f);
		}
		if (GameManager.Instance.MetalAmount != _prevMetal)
		{
			_metalAmount.text = GameManager.Instance.MetalAmount.ToString();
			iTween.PunchScale(_metalAmount.transform.parent.gameObject, new Vector3(.8f, .8f, .8f), .5f);
		}

		_prevWood = GameManager.Instance.WoodAmount;
		_prevMetal = GameManager.Instance.MetalAmount;
	}
}