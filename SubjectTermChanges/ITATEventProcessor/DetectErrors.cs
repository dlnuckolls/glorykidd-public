using System;
using System.Data;
using System.Configuration;
using System.Web;
using Kindred.Knect.ITAT.Business;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT
{
	public class DetectErrors
	{
		public List<string> ProcessScheduledEvents2(string system, DateTime dtStart, DateTime dtStop)
		{
			List<string> listOut = new List<string>();
			listOut.Add("SystemID,ManagedItemNumber,ManagedItemID,EventID,ScheduledEventID,EventDate");
//			DataSet dsEvents = ManagedItem.GetAllNonExecutedEvents2(system, dtStart, dtStop);
			DataSet dsEvents = ManagedItem.GetAllNonExecutedEvents(dtStop);
			if (dsEvents.Tables.Count > 0)
			{
				if (dsEvents.Tables[0].Rows.Count > 0)
				{
					string configKey = Utility.EnvironmentHelper.GetEnvironment(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine) + Kindred.Knect.ITAT.Data.DataNames._AC_ApplicationWebServer;
					string sEnvironment = System.Configuration.ConfigurationManager.AppSettings[configKey];

					Guid previousManagedItemId = new Guid(dsEvents.Tables[0].Rows[0][Kindred.Knect.ITAT.Data.DataNames._C_ManagedItemID].ToString());
					List<Kindred.Knect.ITAT.Business.ProcessScheduledEvent> scheduledEvents = new List<ProcessScheduledEvent>();

					//Create a listing of all the ScheduledEvent's for a given managedItem
					for (int i = 0; i < dsEvents.Tables[0].Rows.Count; i++)
					{
						DataRow row = dsEvents.Tables[0].Rows[i];
						Guid currentManagedItemId = new Guid(row[Kindred.Knect.ITAT.Data.DataNames._C_ManagedItemID].ToString());

						if ((currentManagedItemId != previousManagedItemId) && (scheduledEvents.Count > 0))
						{
							listOut.AddRange(ProcessManagedItemEvents2(sEnvironment, previousManagedItemId, scheduledEvents));
							previousManagedItemId = currentManagedItemId;
							scheduledEvents = new List<ProcessScheduledEvent>();
						}

						Guid scheduledEventId = new Guid(row[Kindred.Knect.ITAT.Data.DataNames._C_ScheduledEventID].ToString());
						Guid eventId = new Guid(row[Kindred.Knect.ITAT.Data.DataNames._C_EventID].ToString());
						DateTime eventDate = DateTime.Parse(row[Kindred.Knect.ITAT.Data.DataNames._C_ScheduledEventDate].ToString());
						scheduledEvents.Add(new ProcessScheduledEvent(scheduledEventId, eventId, eventDate, false));
					}
					if (scheduledEvents.Count > 0)
						listOut.AddRange(ProcessManagedItemEvents2(sEnvironment, previousManagedItemId, scheduledEvents));
				}
			}
			//Log.Info(string.Format("Processing Complete.  {0} errors logged.", errorCount));
			return listOut;
		}

		//Returns the number of errors that occur while processing this managed item's events
		private List<string> ProcessManagedItemEvents2(string environment, Guid managedItemId, List<ProcessScheduledEvent> procScheduledEvents)
		{
			List<string> listOut = new List<string>();
			int nEventsNotFound = 0;

			ManagedItem managedItem = ManagedItem.Get(managedItemId, false);
			if (managedItem != null)
			{
				ITATSystem system = null;
				try
				{
					system = ITATSystem.Get(managedItem.SystemID);
				}
				catch
				{
				}
				if (system != null)
				{
					foreach (ProcessScheduledEvent procScheduledEvent in procScheduledEvents)
					{
						Event eventDef = managedItem.FindEvent(procScheduledEvent.EventId);
						if (eventDef != null)
						{
							ScheduledEvent executedEvent = ScheduledEvent.Find(managedItem, procScheduledEvent.ScheduledEventId);
							if (executedEvent == null)
							{
								listOut.Add(string.Format("{0},{1},{2},{3},{4},{5}",
									managedItem.SystemID.ToString(),
									managedItem.ItemNumber.Trim(),
									managedItem.ManagedItemID.ToString(),
									procScheduledEvent.EventId.ToString(),
									procScheduledEvent.ScheduledEventId.ToString(),
									procScheduledEvent.EventDate.ToString("MM/dd/yyyy")));
							}
						}
						else
						{
							nEventsNotFound++;
							//Log.Error(string.Format("ManagedItem {0} EventID {1} not found", managedItem.ManagedItemID.ToString(), procScheduledEvent.EventId.ToString()));
						}
					}
				}
			}
			return listOut;
		}

	}
}
