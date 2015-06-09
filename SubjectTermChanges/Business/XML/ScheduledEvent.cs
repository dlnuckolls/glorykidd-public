using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlTypes;

namespace Kindred.Knect.ITAT.Business
{

	// A ScheduledEvent means an Event Offset instance

	[Serializable, System.Diagnostics.DebuggerDisplay("DateOffset={DateOffset}, ID={ID}")]
	public class ScheduledEvent
	{
		public enum ExecutedCalculationType
		{
			UsePreviousValue = 0,
			SetAsNotExecuted,
			SetAsExecuted,
			UseGracePeriod
		}

		#region private constants
		const char _delimiter = ',';
		#endregion

		#region private members

		private Guid _id;
		private int _dateOffset;	//In days.  The value is 'subtracted' from the target date, so in
									//order to cause an event to occur after a target date, use 'negative' values.
		private Guid _eventId;
		#endregion

		#region Properties

		public Guid ID
		{
			get { return _id; }
			set { _id = value; }
		}

		public int DateOffset
		{
			get { return _dateOffset; }
			set { _dateOffset = value; }
		}

		public Guid EventId
		{
			get { return _eventId; }
			set { _eventId = value; }
		}

		#endregion

		#region Constructors


		public ScheduledEvent(int dateOffset, Guid eventId, bool useId)
		{
			if (useId)
			{
				_id = Guid.NewGuid();
			}
			else
			{
				_id = Guid.Empty;
			}
			_dateOffset = dateOffset;
			_eventId = eventId;
		}

		public ScheduledEvent(XmlNode termNode, Guid eventId, bool useId)
		{
			if (useId)
			{
                string idString = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_ID);
                if (!string.IsNullOrEmpty(idString))
					_id = new Guid(idString);
				else
					_id = Guid.NewGuid();
			}
			else
			{
				_id = Guid.Empty;
			}
			_dateOffset = Utility.XMLHelper.GetAttributeInt(termNode, XMLNames._A_DateOffset).Value;
			_eventId = eventId;
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
			}

			if (_id != Guid.Empty)
				Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ID, _id.ToString());
			Utility.XMLHelper.AddAttributeInt(xmlDoc, termNode, XMLNames._A_DateOffset, _dateOffset);
		}

		#endregion

        public void Migrate()
        {
            //Note - regenerate the ID to ensure that no 'duplicates' exist across ManagedItems.
            _id = Guid.NewGuid();
        }

		#region Private Static Methods


		private static ScheduledEvent Find(List<ScheduledEvent> scheduledEvents, int offset)
		{
			if (scheduledEvents == null)
				return null;
			Predicate<ScheduledEvent> p = delegate(ScheduledEvent scheduledEvent) { return (scheduledEvent.DateOffset == offset); };
			return scheduledEvents.Find(p);
		}

		private static ScheduledEvent Find(List<ScheduledEvent> scheduledEvents, Guid scheduledEventID)
		{
			if (scheduledEvents == null)
				return null;
			Predicate<ScheduledEvent> p = delegate(ScheduledEvent scheduledEvent) { return (scheduledEvent.ID.Equals(scheduledEventID)); };
			return scheduledEvents.Find(p);
		}


		public static DataTable BuildSqlUdt(Guid managedItemId, IEnumerable<ProcessScheduledEvent> processScheduledEvents)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ScheduledEventId", typeof(SqlGuid));
			dt.Columns.Add("ScheduledEventDate", typeof(SqlDateTime));
			dt.Columns.Add("ManagedItemId", typeof(SqlGuid));
			dt.Columns.Add("EventId", typeof(SqlGuid));
			dt.Columns.Add("Executed", typeof(SqlBoolean));
			if (processScheduledEvents != null)
			{
				foreach (ProcessScheduledEvent pse in processScheduledEvents)
				{
					if (pse.ScheduledEventId == Guid.Empty)
						throw new ArgumentException(string.Format("The ScheduledEventId == Guid.Empty for Managed Item '{0}', Event Date = '{1}'.  This should not happen.", managedItemId, pse.EventDate));
					dt.Rows.Add(pse.ScheduledEventId, pse.EventDate, managedItemId, pse.EventId, pse.Executed);
				}
			}
			return dt;
		}


		#endregion

		#region Public Static Methods


		public static string GetOffsetsDisplay(List<ScheduledEvent> scheduledEvents)
		{
			if (scheduledEvents == null)
				return "";
			if (scheduledEvents.Count == 0)
				return "";
			int[] offsetsList = GetOffsets(scheduledEvents);
			StringBuilder sb = new StringBuilder();
			foreach (int offset in offsetsList)
			{
				sb.AppendFormat("{0}{1}", offset, _delimiter);
			}
			return sb.ToString(0, sb.Length - 1);   //remove trailing delimeter
		}

		public static int[] GetOffsets(string dateOffsets)
		{
			if (string.IsNullOrEmpty(dateOffsets))
				return null;
			string[] stringOffsets = dateOffsets.Split(_delimiter);
			List<int> offsets = new List<int>();
			int nOffset;
			foreach (string stringOffset in stringOffsets)
			{
				if (int.TryParse(stringOffset, out nOffset))
					if (!offsets.Contains(nOffset))
						offsets.Add(nOffset);
			}
			offsets.Sort();
			return offsets.ToArray();
		}

		public static int[] GetOffsets(List<ScheduledEvent> scheduledEvents)
		{
			if (scheduledEvents == null)
				return null;
			if (scheduledEvents.Count == 0)
				return null;
			List<int> offsets = new List<int>(scheduledEvents.Count);
			foreach (ScheduledEvent scheduledEvent in scheduledEvents)
			{
				offsets.Add(scheduledEvent.DateOffset);
			}
			offsets.Sort();
			return offsets.ToArray();
		}


		public static ScheduledEvent Find(Template template, Guid scheduledEventID)
		{
			if (template == null)
				return null;

			ScheduledEvent scheduledEvent = null;

			foreach (Event eachEvent in template.Events)
			{
				scheduledEvent = Find(eachEvent.ScheduledEvents, scheduledEventID);
				if (scheduledEvent != null)
					return scheduledEvent;
			}

			foreach (Event eachEvent in template.Workflow.Events)
			{
				scheduledEvent = Find(eachEvent.ScheduledEvents, scheduledEventID);
				if (scheduledEvent != null)
					return scheduledEvent;
			}

			RenewalTerm renewalTerm = template.FindBasicTerm(TermType.Renewal) as RenewalTerm;
			if (renewalTerm != null)
				return Find(renewalTerm.RenewalEvent.ScheduledEvents, scheduledEventID);

			return null;
		}

		public static DateTime GetEventDate(DateTime baseDate, int dateOffset)
		{
			return baseDate.AddDays(-1 * dateOffset);
		}

        public static ScheduledEvent FindEquivalent(List<ScheduledEvent> scheduledEvents, ScheduledEvent scheduledEvent)
        {
            if (scheduledEvents == null)
                return null;

            return scheduledEvents.Find(se => se.DateOffset == scheduledEvent.DateOffset);
        }
        
        #endregion

 	}
}
