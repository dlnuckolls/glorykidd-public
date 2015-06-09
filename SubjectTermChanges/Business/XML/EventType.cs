using System;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{

	public enum EventType
	{
		None = -1,
		Custom = 0,
		Workflow,		//Note - this is not a scheduled event.  It is associated with Action messages.
		WorkflowRevertToDraft,
		Renewal,
		RetroRevertToDraft
	}

	public static class EventTypeHelper
	{

		public static string Value(EventType notificationType)
		{
			switch (notificationType)
			{
				case EventType.Custom:
					return XMLNames._NOTIFICATION_TYPE_ASTER;
				case EventType.Workflow:
					return XMLNames._NOTIFICATION_TYPE_W;
				case EventType.WorkflowRevertToDraft:
					return XMLNames._NOTIFICATION_TYPE_E;
				case EventType.Renewal:
					return XMLNames._NOTIFICATION_TYPE_R;
				case EventType.RetroRevertToDraft:
					return XMLNames._NOTIFICATION_TYPE_RETROREVERT;
				default:
					return string.Empty;
			}
		}

		public static EventType GetNotificationType(string value)
		{
			switch (value)
			{
				case XMLNames._NOTIFICATION_TYPE_W:
					return EventType.Workflow;
				case XMLNames._NOTIFICATION_TYPE_E:
					return EventType.WorkflowRevertToDraft;
				case XMLNames._NOTIFICATION_TYPE_R:
					return EventType.Renewal;
				case XMLNames._NOTIFICATION_TYPE_RETROREVERT:
					return EventType.RetroRevertToDraft;
				case XMLNames._NOTIFICATION_TYPE_ASTER:
				default:
					return EventType.Custom;
			}

		}


	}
}
