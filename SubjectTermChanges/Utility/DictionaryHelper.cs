using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kindred.Knect.ITAT.Utility
{
	public static class DictionaryHelper<TKey, TValue>
	{

		public static Dictionary<TKey, TValue> Merge(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
		{
			foreach (TKey key in dict2.Keys)
			{
				if (!dict1.ContainsKey(key))
					dict1.Add(key, dict2[key]);
			}
			return dict1;
		}

	}
}
