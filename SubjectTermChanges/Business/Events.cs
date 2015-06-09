using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	public static class Events
	{

		//Generate the Notifications collection (of N) based on an existing Template
		public static List<Event> Create(string templateDef, bool IsManagedItem)
		{
			if (templateDef == null)
				return null;
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

			return Create(xmlTemplateDoc, IsManagedItem);
		}


		//Generate the Notifications collection (of N) based on an existing Template
		public static List<Event> Create(XmlDocument xmlTemplateDoc, bool IsManagedItem)
		{
			List<Event> notifications = new List<Event>();

			XmlNodeList nodeNotifications = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Events, XMLNames._E_Event));
			if (nodeNotifications != null)
			{
				foreach (XmlNode nodeNotification in nodeNotifications)
				{
					Event notification = new Event(nodeNotification, IsManagedItem);
					notifications.Add(notification);
				}
			}

			return notifications;
		}


		public static List<string> ReplaceEmbeddedTermNames(Template template, List<Event> notifications)
		{
			List<string> sErrors = new List<string>();
			foreach (Event notification in notifications)
			{
				if (!Term.ValidID(notification.BaseDateTermID))
					if (!string.IsNullOrEmpty((notification as Event).BaseDateTermName))
					{
						try
						{
							(notification as Event).BaseDateTermID = template.FindTerm((notification as Event).BaseDateTermName).ID;
						}
						catch (Exception)
						{
							sErrors.Add(string.Format("Unable to find Event term '{0}'", (notification as Event).BaseDateTermName));
						}
					}

				if (!Term.ValidID(notification.OffsetTermID))
					if (!string.IsNullOrEmpty((notification as Event).OffsetTermName))
					{
						try
						{
							(notification as Event).OffsetTermID = template.FindTerm((notification as Event).OffsetTermName).ID;
						}
						catch (Exception)
						{
							sErrors.Add(string.Format("Unable to find Event term '{0}'", (notification as Event).BaseDateTermName));
						}
					}

				foreach (Message message in notification.Messages)
				{
					message.Text = Term.SubstituteTermNames(template, message.Text);
				}
			}
			return sErrors;
		}

		//This is similar to Save(Template template, EventType eventType, bool bValidate), but it saves all event types (such as when copying a template)
		//Note - THIS SHOULD ONLY BE CALLED WHEN COPYING A TEMPLATE, and IT ASSUMES THAT NO MANAGED ITEMS EXIST FOR THIS TEMPLATE
		public static bool Save(Template template, bool bValidate)
		{
			//Convert the xml into an xmldocument
			//Note - the xml will contain other events that should not be modified by this save.
			return true;
		}


		//This method is called whenever the user clicks 'Save'.  However, only certain
		//portions of the template events may have been updated.  For instance, if the user
		//is editing BasicTerms, then only the Renewal term event portion should be updated.
		public static bool Save(XmlDocument xmlTemplateDoc, Template template, EventType eventType, bool bValidate)
		{
			//Convert the xml into an xmldocument
			//Note - the xml will contain other events that should not be modified by this save.
            bool bWriteToDatabase = false;
            if (xmlTemplateDoc == null)
            {
                xmlTemplateDoc = new XmlDocument();
                xmlTemplateDoc.PreserveWhitespace = false;
                xmlTemplateDoc.LoadXml(template.TemplateDef);
                bWriteToDatabase = true;
            }

			//Convert the xml events into a database Events collection for examination.
			List<Event> databaseEvents = Create(xmlTemplateDoc, template is ManagedItem);

			//Remove the 'EventType' Events from the database Events collection
			for (int i = 0; i < databaseEvents.Count; i++)
			{
				if (databaseEvents[i].EventType == eventType)
				{
					databaseEvents.RemoveAt(i);
					i--;
				}
			}

			//Add back in the 'EventType' events from the memory collection. 
			for (int i = 0; i < template.Events.Count; i++)
			{
				if (eventType != EventType.WorkflowRevertToDraft)   //do not add back a WorkflowRevertToDraft event in the //TemplateDef/Events node.   These will be in the //TemplateDef/Workflows/WorkFlow/Events node.
 					if (template.Events[i].EventType == eventType)
						databaseEvents.Add(template.Events[i]);
			}

			//Convert the version of "Events" stored in memory to an xmldocument
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement nodeEvents = xmlDoc.CreateElement(XMLNames._E_Events);

			if (databaseEvents != null)
				foreach (Event databaseEvent in databaseEvents)
				{
					XmlElement nodeEvent = xmlDoc.CreateElement(XMLNames._E_Event);
					databaseEvent.Build(xmlDoc, nodeEvent, bValidate);
					nodeEvents.AppendChild(nodeEvent);
				}

			//Replace the "terms" portion of the complete xml with this version from memory
			XmlNode importedNodeEvents = xmlTemplateDoc.ImportNode(nodeEvents, true);
			//Find the "Notifications" child node
			XmlNode documentChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_Events));
			if (documentChildNode != null)
			    xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeEvents, documentChildNode);
			else
			    xmlTemplateDoc.DocumentElement.AppendChild(importedNodeEvents);

			//Store the entire xml back to the database
            if (bWriteToDatabase)
                Template.UpdateTemplateDef(template, xmlTemplateDoc.OuterXml);
			return true;
		}


		//Generate the Terms collection (of Term) based on an xml document
		//public static string Save(Template template, EventType eventType, ref string sXml, bool bValidate)
		public static bool Save(Template template, ref string sXml, bool bValidate)
		{
			//Convert the xml into an xmldocument
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(sXml);

			if (template.Events == null)
				throw new NullReferenceException("template.Events is null.   Contact customer support.");
			//Convert the objects stored in memory to an xmldocument
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement nodeNotifications = xmlDoc.CreateElement(XMLNames._E_Events);
			foreach (Event notification in template.Events)
			{
				XmlElement nodeNotification = xmlDoc.CreateElement(XMLNames._E_Event);
				notification.Build(xmlDoc, nodeNotification, bValidate);
				nodeNotifications.AppendChild(nodeNotification);
			}
			//Replace the impacted portion of the complete xml with this version from memory
			XmlNode importedNodeNotifications = xmlTemplateDoc.ImportNode(nodeNotifications, true);
			//Find the "Events" child node
			XmlNode documentChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Events));
			if (documentChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeNotifications, documentChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeNotifications);
			//Return the revised template XML
			sXml = xmlTemplateDoc.OuterXml;
			return true;
		}



		private static bool IncludeEvent(ProcessScheduledEvent processScheduledEvent, bool includeExecuted, int gracePeriod)
		{
			if (includeExecuted)
				return true;
			if (!(processScheduledEvent.Executed))
				return true;
			if (gracePeriod > 0)
			{
				return processScheduledEvent.EventDate > DateTime.Today.AddDays(-1 * gracePeriod);
			}
			else
			{
				return false;
			}
		}


		//This call is used to generate an xml format for database updates
		//If gracePeriod > 0,
		//   include any events that have occurred < gracePeriod days ago (even if already executed)
		public static string GetXml(Guid managedItemID, List<ProcessScheduledEvent> processScheduledEvents, bool includeDates, bool includeExecuted, int gracePeriod)
		{
			if (processScheduledEvents == null)
				return null;

			using (Utility.StringWriterWithEncoding sw = new Utility.StringWriterWithEncoding(Encoding.UTF8))
			{
				using (System.Xml.XmlTextWriter writer = new XmlTextWriter(sw))
				{
					writer.WriteStartDocument();
					writer.WriteStartElement(XMLNames._E_Events);
					foreach (ProcessScheduledEvent processScheduledEvent in processScheduledEvents)
					{
						if (IncludeEvent(processScheduledEvent, includeExecuted, gracePeriod))
						{
							writer.WriteStartElement(XMLNames._E_Event);
							writer.WriteAttributeString(XMLNames._A_ManagedItemNumber, managedItemID.ToString());
							writer.WriteAttributeString(XMLNames._A_ScheduledEventId, processScheduledEvent.ScheduledEventId.ToString());
							writer.WriteAttributeString(XMLNames._A_ID, processScheduledEvent.EventId.ToString());
							writer.WriteAttributeString(XMLNames._A_Executed, (processScheduledEvent.Executed ? "1" : "0"));
							if (includeDates)
								writer.WriteAttributeString(XMLNames._A_ExecutionDate, processScheduledEvent.EventDate.ToString());
							writer.WriteEndElement();  // Event
						}
					}
					writer.WriteEndElement();  //Events
					writer.WriteEndDocument();
					writer.Flush();
					return sw.ToString();
				}
			}
		}


		public static List<string> TermReferences(Template template, string termName, Guid termID, List<Event> events)
		{
			
			List<string> rtn = new List<string>();
			foreach (Event eachEvent in events)
			{
				List<string> references = eachEvent.TermReferences(template, termName, termID);
				rtn.AddRange(references);

			}
			return Utility.ListHelper.EliminateDuplicates<string>(rtn);
		}

	}
}
