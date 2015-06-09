using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public class DateOffset
	{
		//Note - this class is kept for the purposes of backward compatibility only.
		#region private constants
		const char _postfix = '*';
		const char _delimiter = ',';
		#endregion

		#region private fields
		private int _offset;
		private bool _executed;
		#endregion

		#region properties
		
		public int Offset
		{
			get { return _offset; }
			set { _offset = value; }
		}

		public bool Executed
		{
			get { return _executed; }
			set { _executed = value; }
		}
		#endregion


		#region constructors

		//Note - This call is kept for backward compatibility.  It expects a string in the format '30' or '30*'.
		public DateOffset(string rawOffset)
		{
			_executed = IsExecuted(rawOffset);
			string sOffset = _executed ? TrimExecuted(rawOffset) : rawOffset;
			if (!int.TryParse(sOffset,out _offset))
				throw new Exception(string.Format("Failed to parse date offset '{0}'",rawOffset));
		}

		#endregion


		#region static methods

		//Note - This call is kept for backward compatibility.  It expects a string in the format '15,30,90' or '15,30*,90*'.
		public static List<DateOffset> GetOffsets(string sRawOffsets)
		{
			string[] sarrRawOffsets = sRawOffsets.Split(_delimiter);
			if (sarrRawOffsets.Length > 0)
			{
				List<DateOffset> dateOffsets = new List<DateOffset>(sarrRawOffsets.Length);
				foreach (string sRawOffset in sarrRawOffsets)
				{
					dateOffsets.Add(new DateOffset(sRawOffset));
				}
				return dateOffsets;
			}
			else
				return new List<DateOffset>();
		}
		#endregion

		#region private methods
		//Note - This call is kept for backward compatibility.  It expects a string in the format '30' or '30*'.
		private bool IsExecuted(string dateOffset)
		{
			if (string.IsNullOrEmpty(dateOffset))
				return false;
			return dateOffset[dateOffset.Length - 1] == _postfix;
		}

		//Note - This call is kept for backward compatibility.
		private string TrimExecuted(string dateOffset)
		{
			return dateOffset.TrimEnd(_postfix);
		}

		#endregion
	}
}
