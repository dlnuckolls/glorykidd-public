using System;
using System.Collections.Generic;
using System.Text;
using Kindred.Common.Batch;
using Kindred.Knect.ITAT.Business;
using Kindred.Common.Logging;
using System.Data;
using Kindred.Knect.ITAT;

namespace Kindred.Knect.ITAT
{
	class Process : BatchBase
	{
		private const string _ARG_UPDATE_SCHEDULED_EVENTS = "UpdateScheduledEvents";
        private const string _ARG_PROCESS_SCHEDULED_EVENTS = "ProcessScheduledEvents";
        private const string _ARG_DETECT_SCHEDULED_EVENT_ERRORS = "DetectScheduledEventErrors";
        private const string _ARG_UPDATE_SECURITY_MODULE = "UpdateSecurityModule";
        private const string _ARG_APPLY_RETRO = "ApplyRetro";
        private const string _CONFIG_SYSTEM = "system";
		private const string _CONFIG_START_DATE = "startdate";
		private const string _CONFIG_END_DATE = "enddate";
		private const string _CONFIG_OUTPUT_FILE = "outputfile";

		static int Main(string[] args)
		{
            int nReturn = (new Process()).Run(args);
            //Console.ReadLine();
            return nReturn;
        }

		Kindred.Knect.ITAT.Business.FacilityCollection _allFacilities;

		public override void InitializeStorageAreas()
		{
			base.InitializeStorageAreas();
			_allFacilities = Kindred.Knect.ITAT.Business.FacilityCollection.GetAll(false);
		}

		public override int ProcessData()
		{
            try
            {
                //If more than one Args present, then not able to process Config data
                if (Args.Length > 0)
                {
                    if (Args.Length > 1)
                    {
                        if (GetConfig(_CONFIG_SYSTEM).Length > 0 ||
                            GetConfig(_CONFIG_START_DATE).Length > 0 ||
                            GetConfig(_CONFIG_END_DATE).Length > 0 ||
                            GetConfig(_CONFIG_OUTPUT_FILE).Length > 0)
                        {
                            Log.Error(string.Format("{0:D} Arguments were defined along with Config data.  No config data expected for multiple arguments.", Args.Length));
                            return 1;
                        }
                        int ret = 0;
                        foreach (string arg in Args)
                        {
                            switch (arg)
                            {
                                //Command Line = ProcessScheduledEvents
                                case _ARG_PROCESS_SCHEDULED_EVENTS:
                                    ret = ProcessScheduledEvents();
                                    break;

                                case _ARG_UPDATE_SCHEDULED_EVENTS:
                                    Log.Error(string.Format("Received {0:D} command line arguments - but argument '{1}' is not supported in this situation", Args.Length, arg));
                                    return 1;

                                case _ARG_DETECT_SCHEDULED_EVENT_ERRORS:
                                    Log.Error(string.Format("Received {0:D} command line arguments - but argument '{1}' is not supported in this situation", Args.Length, arg));
                                    return 1;

                                //Command Line = UpdateSecurityModule
                                case _ARG_UPDATE_SECURITY_MODULE:
                                    new SecurityModule().Process(null);
                                    break;

                                //Command Line = ApplyRetro
                                case _ARG_APPLY_RETRO:
                                    new RetroModule().Process(null);
                                    break;

                                default:
                                    Log.Error(string.Format("Received undefined command line argument '{0}'", arg));
                                    return 1;
                            }
                            if (ret != 0)
                                break;
                        }
                        return ret;
                    }
                    else
                    {
                        string systems = null;
                        switch (Args[0])
                        {
                            //Command Line = ProcessScheduledEvents
                            case _ARG_PROCESS_SCHEDULED_EVENTS:
                                return ProcessScheduledEvents();

                            //Command Line = UpdateScheduledEvents system="HCMS"
                            case _ARG_UPDATE_SCHEDULED_EVENTS:
                                string sSystem = GetConfig(_CONFIG_SYSTEM);
                                if (string.IsNullOrEmpty(sSystem))
                                {
                                    Log.Error(string.Format("Received command line argument '{0}' without command line config setting '{1}'", _ARG_UPDATE_SCHEDULED_EVENTS, _CONFIG_SYSTEM));
                                    return 1;
                                }
                                return UpdateScheduledEvents(sSystem);

                            //Command Line = DetectScheduledEventErrors system="HCMS" startdate="1/1/2001" enddate="1/1/2010" outputfile="C:\ScheduledEventErrors.txt"
                            case _ARG_DETECT_SCHEDULED_EVENT_ERRORS:
                                //Command Line = DetectScheduledEventErrors enddate="1/1/2010" outputfile="C:\ScheduledEventErrors.txt"
                                try
                                {
                                    DateTime dtStop = DateTime.Parse(GetConfig(_CONFIG_END_DATE));
                                    List<string> listOut = new DetectErrors().ProcessScheduledEvents2("", dtStop, dtStop);
                                    System.IO.File.WriteAllLines(GetConfig(_CONFIG_OUTPUT_FILE), listOut.ToArray());
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(string.Format("Got an error while performing '{0}' - Error: {1}  Trace: {2}", _ARG_DETECT_SCHEDULED_EVENT_ERRORS, ex.Message, ex.StackTrace));
                                    return 1;
                                }
                                return 0;

                            //Command Line = UpdateSecurityModule system="RCMS"
                            case _ARG_UPDATE_SECURITY_MODULE:
                                systems = GetConfig(_CONFIG_SYSTEM);
                                if (string.IsNullOrEmpty(systems))
                                    new SecurityModule().Process(null);
                                else
                                    new SecurityModule().Process(systems.Split(','));
                                return 0;

                            //Command Line = ApplyRetro system="Hope Fund"
                            case _ARG_APPLY_RETRO:
                                systems = GetConfig(_CONFIG_SYSTEM);
                                if (string.IsNullOrEmpty(systems))
                                    new RetroModule().Process(null);
                                else
                                    new RetroModule().Process(systems.Split(','));
                                return 0;

                            default:
                                Log.Error(string.Format("Received undefined command line argument '{0}'", Args[0]));
                                return 1;
                        }
                    }
                }
                else
                {
                    Log.Error("No command line arguments provided");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Unhandled exception: {0} Trace: {1}", ex.Message, ex.StackTrace));
                return 1;
            }
		}

		private int UpdateScheduledEvents(string sSystem)
		{
			//Get a list of all ManagedItems for this system that are found in the ScheduledEvents table.
			//Go to each one, update its events.
			try
			{
				DataSet dsManagedItemIDs = Kindred.Knect.ITAT.Data.ManagedItem.GetSystemEventManagedItems(sSystem);
				if (dsManagedItemIDs == null)
				{
					Log.Info("GetSystemEventManagedItems returned null");
					return 1;
				}
				if (dsManagedItemIDs.Tables.Count == 0)
				{
					Log.Info("GetSystemEventManagedItems returned no results");
					return 1;
				}
				if (dsManagedItemIDs.Tables[0].Rows.Count == 0)
				{
					Log.Info("GetSystemEventManagedItems returned no results");
					return 1;
				}
				Log.Info(string.Format("Now updating scheduled events for {0} ManagedItems for System '{1}'", dsManagedItemIDs.Tables[0].Rows.Count.ToString(), sSystem));
				int nCount = 0;
				foreach (DataRow row in dsManagedItemIDs.Tables[0].Rows)
				{
					Guid managedItemId = new Guid(row[Kindred.Knect.ITAT.Data.DataNames._C_ManagedItemID].ToString());
					ManagedItem managedItem = ManagedItem.Get(managedItemId, false);
					if (managedItem.UpdateScheduledEvents(ScheduledEvent.ExecutedCalculationType.UseGracePeriod))
						nCount++;
				}
				Log.Info(string.Format("Successfully updated {0} ManagedItems for System '{1}'", nCount.ToString(), sSystem));
			}
			catch (Exception ex)
			{
				Log.Error(string.Format("UpdateScheduledEvents exception: {0}  trace: {1}",ex.Message, ex.StackTrace));
				return 1;
			}

			return 0;
		}

		private int ProcessScheduledEvents()
		{
			DataSet dsEvents = ManagedItem.GetAllNonExecutedEvents(DateTime.Today);
			int errorCount = 0;
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
							errorCount += ProcessManagedItemEvents(sEnvironment, previousManagedItemId, scheduledEvents);
							previousManagedItemId = currentManagedItemId;
							scheduledEvents = new List<ProcessScheduledEvent>();
						}

						Guid scheduledEventId = new Guid(row[Kindred.Knect.ITAT.Data.DataNames._C_ScheduledEventID].ToString());
						Guid eventId = new Guid(row[Kindred.Knect.ITAT.Data.DataNames._C_EventID].ToString());
						DateTime eventDate = DateTime.Parse(row[Kindred.Knect.ITAT.Data.DataNames._C_ScheduledEventDate].ToString());
						scheduledEvents.Add(new ProcessScheduledEvent(scheduledEventId, eventId, eventDate, false));
					}
					if (scheduledEvents.Count > 0)
						errorCount += ProcessManagedItemEvents(sEnvironment, previousManagedItemId, scheduledEvents);
				}
			}
			Log.Info(string.Format("Processing Complete.  {0} errors logged.", errorCount));
			return errorCount;
		}

		//Returns the number of errors that occur while processing this managed item's events
		private int ProcessManagedItemEvents(string environment, Guid managedItemId, List<ProcessScheduledEvent> procScheduledEvents)
		{
			int nMessagesSent = 0;
			int nMessagesFailed = 0;
			int nEventsNotFound = 0;

			ManagedItem managedItem = ManagedItem.Get(managedItemId, false);
			if (managedItem != null)
			{
				int nEvents = procScheduledEvents.Count;

				ITATSystem system = ITATSystem.Get(managedItem.SystemID);
				foreach (ProcessScheduledEvent procScheduledEvent in procScheduledEvents)
				{
					string sError = "";
					Event eventDef = managedItem.FindEvent(procScheduledEvent.EventId);
					if (eventDef != null)
					{
						ScheduledEvent executedEvent = ScheduledEvent.Find(managedItem, procScheduledEvent.ScheduledEventId);
						if (executedEvent != null)
						{
							if (!procScheduledEvent.Executed)
							{
								procScheduledEvent.Executed = true;
								Message message = eventDef.Messages[0];
								if (message != null)
								{
									switch (eventDef.EventType)
									{
										case EventType.Custom:
											{
												//TODO - Is this baseDateTerm test really required?
												Term baseDateTerm = managedItem.FindTerm(eventDef.BaseDateTermID, eventDef.BaseDateTermName);
												if (baseDateTerm == null)
												{
													Log.Debug(string.Format("Unable to find term {0} for Scheduled Event {1} for item {3}.", managedItem.FindTermName(eventDef.BaseDateTermID, eventDef.BaseDateTermName), procScheduledEvent.ScheduledEventId, managedItem.ItemNumber));
													continue;
												}
												if (message.AllStatusesValid || (message.NotificationStatuses.Contains(managedItem.State.Status)))
												{
													sError = message.Send(managedItem, system, environment, managedItem.OwningFacilityIDs);
													if (sError.Length > 0)
													{
														string sEmailError = string.Format("Failed to send notification message for ManagedItem {0}, due {1}.", managedItem.ItemNumber.Trim(), procScheduledEvent.EventDate.ToString("MM/dd/yyyy"));
														Log.Error(string.Format("{0} - Error: {1}", sEmailError, sError));
														Business.EmailHelper.SendNotificationToOwner(system, managedItem, message.Text, sEmailError);
														nMessagesFailed++;
													}
													else
													{
														nMessagesSent++;
													}
												}
												else
                                                    Log.Debug(string.Format("Message not sent for ManagedItem {0} because managed item status ({1}) is not valid", managedItem.ItemNumber, managedItem.State.Status));
												break;
											}

										case EventType.Renewal:
											{
												if (message.AllStatusesValid || (message.NotificationStatuses.Contains(managedItem.State.Status)))
												{
													sError = message.Send(managedItem, system, environment, managedItem.OwningFacilityIDs);
													if (sError.Length > 0)
													{
														string sEmailError = string.Format("Failed to send renewal message for ManagedItem {0} due {1}.", managedItem.ItemNumber.Trim(), procScheduledEvent.EventDate.ToString("MM/dd/yyyy"));
														Log.Error(string.Format("{0} - Error: {1}", sEmailError, sError));
                                                        Business.EmailHelper.SendNotificationToOwner(system, managedItem, message.Text, sEmailError);
														nMessagesFailed++;
													}
													else
													{
														nMessagesSent++;
													}
												}
												else
													Log.Debug(string.Format("Message not sent because managed item status ({0}) is not valid", managedItem.State.Status));
												break;
											}

										case EventType.Workflow:
											{
												//Not a scheduled event - should not occur
												throw new Exception(string.Format("Encountered a Workflow event - as if it were scheduled, EventID = {0}", procScheduledEvent.EventId.ToString()));
											}

										//TODO - Test WorkflowRevertEvent
										case EventType.WorkflowRevertToDraft:
											{
												sError = message.Send(managedItem, system, environment, managedItem.OwningFacilityIDs);
												if (sError.Length > 0)
												{
													string sEmailError = string.Format("Failed to send RevertToDraft message for ManagedItem {0}, due {1}.", managedItem.ItemNumber.Trim(), procScheduledEvent.EventDate.ToString("MM/dd/yyyy"));
													Log.Error(string.Format("{0} - Error: {1}", sEmailError, sError));
                                                    Business.EmailHelper.SendNotificationToOwner(system, managedItem, message.Text, sEmailError);
													nMessagesFailed++;
												}
												else
												{
													nMessagesSent++;
													managedItem.State = managedItem.Workflow.FindBaseState();
                                                }
												break;
											}
									}
								}
								else
									Log.Debug("Message not sent because eventDef.Messages[0] is null.");
							}
							else
								Log.Debug(string.Format("Message not sent because scheduled event {0} was already executed.", executedEvent.ID.ToString()));
						}
						else
							Log.Debug(string.Format("Message not sent because scheduled event {0} was not found.", procScheduledEvent.ScheduledEventId.ToString()));
					}
					else
					{
						nEventsNotFound++;
						Log.Error(string.Format("ManagedItem {0} EventID {1} not found", managedItem.ManagedItemID.ToString(), procScheduledEvent.EventId.ToString()));
					}
				}
                managedItem.Update(false, Business.Retro.AuditType.Saved);

				Log.Info(string.Format("Processed ManagedItem: {0}-{1},  {2} Events, {3} Events Not Found,\n\t{4} Emails sent, {5} Emails errored out",
					system.Name,
					managedItem.ItemNumber.Trim(),
					nEvents.ToString(),
					nEventsNotFound.ToString(),
					nMessagesSent.ToString(),
					nMessagesFailed.ToString()));

				Data.ManagedItem.UpdateProcessScheduledEvents(managedItem.ManagedItemID, ScheduledEvent.BuildSqlUdt(managedItem.ManagedItemID, procScheduledEvents), false);

			}
			return (nEventsNotFound);
		}

		public override void DisplayCounters()
		{
		}

	}
}
