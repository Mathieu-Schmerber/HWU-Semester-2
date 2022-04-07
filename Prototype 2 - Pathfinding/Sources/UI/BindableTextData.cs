using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tool used to bind data to any GUI object
/// </summary>
public abstract class BindableTextData : MonoBehaviour
{
	private string _pattern = @"\B\$";
	private Regex _regex;
	protected Dictionary<Text, string> _unbindData = new Dictionary<Text, string>();

	protected virtual void Awake()
	{
		_regex = new Regex(_pattern);
		foreach (Text textObj in GetComponentsInChildren<Text>(includeInactive: true))
			_unbindData.Add(textObj, textObj.text);
	}

	public void Bind<T>(T valueObj) where T : class
	{
		Type type = valueObj.GetType();

		OnStartBinding();
		foreach (PropertyInfo property in type.GetProperties())
		{
			string code = type.GetDisplayNameByPropertyName(property.Name);

			if (code != null && _regex.IsMatch(code))
			{
				object value = property.GetValue(valueObj);
				Text[] concerned = _unbindData.Where(x => x.Value.Contains(code)).Select(x => x.Key).ToArray();

				if (value == null)
					OnValueNull(concerned, code);
				else
				{
					OnValueChanging(concerned, code, value);
					concerned.ForEach(x => x.text = _unbindData[x].Replace(code, FormatValue(value)));
					OnValueChanged(concerned, code, value);
				}
			}
		}
	}

	public virtual string FormatValue(object value) => value.ToString();
	public virtual void OnValueNull(Text[] textsObject, string code) { }
	public virtual void OnValueChanging(Text[] textsObject, string code, object value) { }
	public virtual void OnValueChanged(Text[] textsObject, string code, object value) { }
	public virtual void OnStartBinding() { }
}