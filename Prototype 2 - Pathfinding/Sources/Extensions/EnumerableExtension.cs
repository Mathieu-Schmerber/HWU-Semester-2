using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Extends any IEnumarable implementation.
/// </summary>
public static class EnumerableExtension
{
	private static readonly Random random = new Random();

	/// <summary>
	/// Get random element of an enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static T Random<T>(this IList<T> list) => list[random.Next(0, list.Count)];

	/// <summary>
	/// Executes an action for each element of an enumerable.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="source"></param>
	/// <param name="action"></param>
	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T item in source)
			action(item);
	}
}
