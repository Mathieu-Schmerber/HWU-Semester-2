using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EntityTooltip : Tooltip
{
	[SerializeField] private GameObject _costPanel;

	private readonly string[] COST_CODES = { "$WCOST", "$MCOST" };

	public override void OnStartBinding()
	{
		_costPanel.SetActive(false);
		_unbindData.ForEach(x => x.Key.transform.parent.gameObject.SetActive(false));
	}

	public override void OnValueChanging(Text[] textsObject, string code, object value)
	{
		bool costWritten = COST_CODES.Contains(code);

		if (costWritten)
			_costPanel.SetActive(true);
		textsObject.ForEach(x => x.transform.parent.gameObject.SetActive(true));
	}
}
