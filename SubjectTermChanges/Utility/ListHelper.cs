using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Kindred.Knect.ITAT.Utility
{
	public static class ListHelper
	{

		/// <summary>
		/// Returns true if the 2 lists are identical, except for order.   
		/// In other words, if each item from list1 is in list2, and each item from list2 is in list1, then returns true.  Otherwise, returns false.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="list1"></param>
		/// <param name="list2"></param>
		/// <returns></returns>
		public static bool ListsMatch<T, U>(List<T> list1, List<U> list2)  where U : IEquatable<T>, IComparable<T>
		{
			//first handle null or obvious cases
			if ((list1 == null) && list2 == null)
				return true;
			if ((list1 == null) || list2 == null)
				return false;
			int count1 = list1.Count;
			int count2 = list2.Count;
			if ((count1 == 0) && (count2 == 0))
				return true;
			if (count1 != count2)
				return false;

            List<T> listCompare1 = new List<T>(list1);
            List<U> listCompare2 = new List<U>(list2);
            //here, we can assume that neither list is null, and that they have the same number of elements
            listCompare1.Sort();
            listCompare2.Sort();

            for (int i = 0; i < listCompare1.Count; i++)
                if (!listCompare1[i].Equals(listCompare2[i]))
					return false;

			//if we get to this point, then every item in sortedList1 matched the corresponding item in sortedList2
			return true;
		}



		/// <summary>
		/// Compares 2 lists of the same type.  If the 2 lists have ANY value in common, returns true.  Otherwise, returns false.
		/// </summary>
		/// <typeparam name="T">any value type</typeparam>
		/// <param name="array1">First array of type T</param>
		/// <param name="array2">Second array of type T</param>
		/// <returns>bool indicating whether the 2 arrays have any value in common.</returns>
		public static bool HaveAMatch<T>(IList<T> list1, IList<T> list2) where T:IComparable
		{
			for (int i = 0; i < list1.Count; i++)
				for (int j = 0; j < list2.Count; j++)
					if (list1[i].Equals(list2[j]))
						return true;
			return false;
		}


		public static bool FacilityListsOverlap(Dictionary<int, Data.FacilityDataRow> list1, List<int> list2)
		{
			for (int i = 0; i < list2.Count; i++)
				if (list1.ContainsKey(list2[i]))
						return true;
			return false;
		}

		public static string ToDelimitedString(List<int> list, char delimiter, bool includeEndDelimiters)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if (includeEndDelimiters)
				sb.Append(delimiter);
			for (int i = 0; i < list.Count - 1; i++)
			{
				sb.Append(list[i].ToString());
				sb.Append(delimiter);
			}
			if (list.Count > 0)
				sb.Append(list[list.Count - 1].ToString());
			if (includeEndDelimiters)
				sb.Append(delimiter);
			return sb.ToString();
		}

		public static List<int> FromDelimitedString(string inputString, char delimiter, bool includeEndDelimiters)
		{
			List<int> rtn = new List<int>();
			if (string.IsNullOrEmpty(inputString))
				return rtn;
			if (inputString.Length > 1)
			{
				if (includeEndDelimiters)
				{
					if ((inputString[0] == delimiter) && (inputString[inputString.Length - 1] == delimiter))
						inputString = inputString.Substring(1, inputString.Length - 2);
					else
						throw new Exception("includeEndDelimiters == true, but the first and last character of inputString do not match.");
				}
				if (!string.IsNullOrEmpty(inputString))
				{
					char[] separator = new char[1] { delimiter };
					string[] strings = inputString.Split(separator);
					for (int i = 0; i < strings.Length; i++)
					{
						try
						{
							rtn.Add(int.Parse(strings[i]));
						}
						catch
						{
							throw new Exception(string.Format("Unable to convert '{0}' to an int.   (input string = \"{2}\")", strings[i], inputString));
						}
					}
					rtn.Sort();
				}
			}
			return rtn;
		}

		public static List<T> EliminateDuplicates<T>(List<T> list) where T:IComparable
		{
			list.Sort();
			for (int i = list.Count - 1; i >0; i--)
				if (list[i-1].Equals(list[i]))
					list.RemoveAt(i);
			return list;
		}
    }
}
