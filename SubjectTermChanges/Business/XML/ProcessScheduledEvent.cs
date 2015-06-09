using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable, System.Diagnostics.DebuggerDisplay("ID:{ScheduledEventId}, Date:{EventDate}, EventID:{EventId}")]
	public class ProcessScheduledEvent 
	{

		#region  private fields
        private readonly Guid _scheduledEventId;
        private readonly Guid _eventId;
        private readonly DateTime _eventDate;
		private bool _executed;  
		#endregion


		#region Properties

        public Guid ScheduledEventId
		{
			get { return _scheduledEventId; }
		}

        public Guid EventId
		{
			get { return _eventId; }
		}

		public DateTime EventDate
		{
			get { return _eventDate; }
		}

		public bool Executed
		{
			get { return _executed; }
			set { _executed = value; }
		}

		#endregion


		#region constructors
		
		public ProcessScheduledEvent(Guid scheduledEventId, Guid eventId, DateTime eventDate, bool executed)
		{
			_scheduledEventId = scheduledEventId;
			_eventId = eventId;
			_eventDate = eventDate;
			_executed = executed;
		}

		#endregion


	}

}
