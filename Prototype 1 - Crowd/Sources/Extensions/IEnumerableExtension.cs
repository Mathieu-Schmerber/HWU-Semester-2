using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class IEnumerableExtension
{
	private static readonly Random random = new Random();

	public static T Random<T>(this IList<T> list) => list[random.Next(0, list.Count)];
}
