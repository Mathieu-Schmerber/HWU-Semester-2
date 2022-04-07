using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a Tooltip type.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IDrawableTooltip
{
	public void Draw(object value);
}
