using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable, System.Diagnostics.DebuggerDisplay("{EventType} Event; Name={Name}, ScheduledEvents.Count={ScheduledEvents.Count}, ID={ID}")]
	public class Event
	{
        public const string _LABEL_BASE_DATE_TERM_NAME = "BaseDateTermName";
        public const string _LABEL_OFFSET_TERM_NAME = "OffsetTermName";
        public const string _LABEL_OFFSET_DEFAULT_VALUE = "OffsetDefaultValue";
        public const string _LABEL_BASE_DATE_OFFSET = "BaseDateOffset";

		#region private members

		private Guid _id;
		private bool? _isRef;
		private string _name;
		private string _description;
		private EventType _eventType;

		private string _baseDateTermName;
		private Guid _baseDateTermID;	//Identifies the basic term required for calculating the date for a ScheduledEvent.

		private string _offsetTermName;  
		private Guid _offsetTermID;		//Identifies the basic term whose 'DisplayValue' is used to calculate the 'Base Offset Days'.  This has meaning only if the BaseDateTerm is defined.

		private string _baseDateTermPart;  // for certain term types (e.g. Renewal) that have multiple "parts", this denotes the part of term (e.g.  Effective Date, Expiration Date)

		private int _offsetDefaultValue;  //Used to calculate the 'Base Offset Days' in cases where the DisplayVaue of the OffsetTerm is not an int.
		private List<Message> _messages;
		private List<ScheduledEvent> _scheduledEvents;

		private bool _isManagedItem = false;

		#endregion

		#region Properties

		public Guid ID
		{
			get { return _id; }
		}

		public bool? IsRef
		{
			get { return _isRef; }
			set { _isRef = value; }
		}

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Description
		{
			get { return Utility.XMLHelper.GetXMLText(_description); }
			set { _description = Utility.XMLHelper.SetXMLText(value); }
		}

		public EventType EventType
		{
			get { return _eventType; }
			set { _eventType = value; }
		}

		public string BaseDateTermName
		{
			get { return Utility.XMLHelper.GetXMLText(_baseDateTermName); }
		}

		public string OffsetTermName
		{
			get { return _offsetTermName; }
		}

		public Guid BaseDateTermID
		{
			get { return _baseDateTermID; }
			set { 
					_baseDateTermID = value;
					_baseDateTermName = null;
				}
		}

		public string BaseDateTermPart
		{
			get { return _baseDateTermPart; }
			set { _baseDateTermPart = value; }
		}


		public Guid OffsetTermID
		{
			get { return _offsetTermID; }
			set { 
					_offsetTermID = value;
					_offsetTermName = null;
				}
		}

		public int OffsetDefaultValue
		{
			get { return _offsetDefaultValue; }
			set { _offsetDefaultValue = value; }
		}

		public string BaseDateOffset
		{
			get
			{
				//Create a comma-delimited string based on the contents of the _scheduledEvents collection.
				return ScheduledEvent.GetOffsetsDisplay(_scheduledEvents);
			}
			set
			{
				SetOffsets(value);
			}
		}

		public List<Message> Messages
		{
			get { return _messages; }
			set { _messages = value; }
		}

		public List<ScheduledEvent> ScheduledEvents
		{
			get { return _scheduledEvents; }
		}

		#endregion

		#region Constructors

		public Event(EventType eventType, bool IsManagedItem)
		{
			_id = Guid.NewGuid();
			_eventType = eventType;
			_messages = new List<Message>();
			_scheduledEvents = new List<ScheduledEvent>();
			_isManagedItem = IsManagedItem;
		}

		public Event(XmlNode scheduledEventNode, bool IsManagedItem)
		{
			_isManagedItem = IsManagedItem;

			string idString = Utility.XMLHelper.GetAttributeString(scheduledEventNode, XMLNames._A_ID);
			if (string.IsNullOrEmpty(idString))
				_id = Guid.NewGuid();
			else
				_id = new Guid(idString);
			_isRef = Utility.XMLHelper.GetAttributeBool(scheduledEventNode, XMLNames._A_IsRef);
			_name = Utility.XMLHelper.GetAttributeString(scheduledEventNode, XMLNames._A_Name);
			_description = Utility.XMLHelper.GetAttributeString(scheduledEventNode, XMLNames._A_Description);
			_eventType = (EventType)Enum.Parse(typeof(EventType), Utility.XMLHelper.GetAttributeString(scheduledEventNode, XMLNames._A_EventType));
			_baseDateTermName = Utility.XMLHelper.GetAttributeString(scheduledEventNode, XMLNames._A_BaseDateTermName);
			_offsetTermName = Utility.XMLHelper.GetAttributeString(scheduledEventNode, XMLNames._A_OffsetTermName);

			_baseDateTermID = Term.CreateID(scheduledEventNode, XMLNames._A_BaseDateTermID);
			_baseDateTermPart = Utility.XMLHelper.GetAttributeString(scheduledEventNode, XMLNames._A_BaseDateTermPart);
			_offsetTermID = Term.CreateID(scheduledEventNode, XMLNames._A_OffsetTermID);

			_offsetDefaultValue = 0;
			string sOffsetDefaultValue = Utility.XMLHelper.GetAttributeString(scheduledEventNode, XMLNames._A_OffsetDefaultValue);
			if (!string.IsNullOrEmpty(sOffsetDefaultValue))
			{
				try { _offsetDefaultValue += int.Parse(sOffsetDefaultValue); }
				catch { }
			}

			XmlNodeList listScheduledEvents = scheduledEventNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_ScheduledEvents, XMLNames._E_ScheduledEvent));
			if (listScheduledEvents != null && listScheduledEvents.Count > 0)
			{
				_scheduledEvents = new List<ScheduledEvent>(listScheduledEvents.Count);
				foreach (XmlNode nodeScheduledEvent in listScheduledEvents)
				{
					ScheduledEvent scheduledEvent = new ScheduledEvent(nodeScheduledEvent, _id, IsManagedItem);
					_scheduledEvents.Add(scheduledEvent);
				}
			}
			else
			{
				_scheduledEvents = new List<ScheduledEvent>();
			}

			XmlNodeList listMessages = scheduledEventNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Messages,XMLNames._E_Message));
			if (listMessages != null && listMessages.Count > 0)
			{
				_messages = new List<Message>(listMessages.Count);
				foreach (XmlNode nodeMessage in listMessages)
				{
					Message message = new Message(nodeMessage);
					_messages.Add(message);
				}
			}
			else
			{
				_messages = new List<Message>();
			}
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
			}

			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ID, _id.ToString());
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_IsRef, _isRef);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Name, _name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Description, _description);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_EventType, _eventType.ToString());
			Term.StoreID(xmlDoc, termNode, XMLNames._A_BaseDateTermName, _baseDateTermName, XMLNames._A_BaseDateTermID, _baseDateTermID);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_BaseDateTermPart, _baseDateTermPart);
			Term.StoreID(xmlDoc, termNode, XMLNames._A_OffsetTermName, _offsetTermName, XMLNames._A_OffsetTermID, _offsetTermID);

			Utility.XMLHelper.AddAttributeInt(xmlDoc, termNode, XMLNames._A_OffsetDefaultValue, _offsetDefaultValue);

			XmlNode elementScheduledEvents = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_ScheduledEvents, XMLNames._M_NameSpaceURI);
			termNode.AppendChild(elementScheduledEvents);
			foreach (ScheduledEvent scheduledEvent in _scheduledEvents)
			{
				if (scheduledEvent != null)
				{
					XmlNode elementScheduledEvent = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_ScheduledEvent, XMLNames._M_NameSpaceURI);
					scheduledEvent.Build(xmlDoc, elementScheduledEvent, bValidate);
					elementScheduledEvents.AppendChild(elementScheduledEvent);
				}
			}

			XmlNode elementMessages = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Messages, XMLNames._M_NameSpaceURI);
			termNode.AppendChild(elementMessages);
			foreach (Message message in _messages)
			{
				if (message != null)
				{
					XmlNode elementMessage = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Message, XMLNames._M_NameSpaceURI);
					message.Build(xmlDoc, elementMessage, bValidate);
					elementMessages.AppendChild(elementMessage);
				}
			}

		}

		#endregion


		internal List<string> TermReferences(Template template, string termName, Guid termID)
		{
		    List<string> rtn = new List<string>();

		    foreach (Message message in _messages)
		    {
				List<string> references = message.TermReferences(template, termName, string.Format("Event \"{0}\"",Name));
				foreach (string reference in references)
					rtn.Add(reference);
		    }

			if (Term.MatchReference(template, BaseDateTermID, BaseDateTermName, termID, termName))
				rtn.Add(string.Format("Term \"{0}\" is in use by Event \"{1}\".", termName, Name));

			if (Term.MatchReference(template, OffsetTermID, OffsetTermName, termID, termName))
				rtn.Add(string.Format("Term \"{0}\" is in use by Event \"{1}\".", termName, Name));

			return rtn;
		}


		public Event Copy(bool IsManagedItem)
		{
			Event newEvent = new Event(EventType, IsManagedItem);

			newEvent.IsRef = IsRef;
			newEvent.Name = Name;
			newEvent.Description = Description;
			newEvent.BaseDateTermID = BaseDateTermID;
			newEvent.OffsetTermID = OffsetTermID;
			newEvent.BaseDateTermPart = BaseDateTermPart;
			newEvent.OffsetDefaultValue = OffsetDefaultValue;

			newEvent.Messages.Clear();
			foreach (Message message in Messages)
			{
				newEvent.Messages.Add(message);
			}

			newEvent.ScheduledEvents.Clear();
			foreach (ScheduledEvent scheduledEvent in ScheduledEvents)
			{
				ScheduledEvent newScheduledEvent = new ScheduledEvent(scheduledEvent.DateOffset, newEvent.ID, IsManagedItem);
				newEvent.ScheduledEvents.Add(newScheduledEvent);
			}

			return newEvent;
		}


		private void SetOffsets(string offsetsString)
		{
			if (string.IsNullOrEmpty(offsetsString))
			{
				//Create a new scheduledEvents collection of one member, not executed.
				_scheduledEvents = GetScheduledEvents(XMLNames._M_ZeroBaseOffset);
			}
			else
			{
				//Create a new scheduledEvents collection, then compare it to the current _scheduledEvents collection.  
				//This is in order to preserve the executed status of existing _scheduledEvents.
				List<ScheduledEvent> newEvents = GetScheduledEvents(offsetsString);
				_scheduledEvents = Consolidate(_scheduledEvents, newEvents);
			}
		}



		//This call expects a string in the format '15,30,90' (no special char's).
		private List<ScheduledEvent> GetScheduledEvents(string dateOffsets)
		{
			if (string.IsNullOrEmpty(dateOffsets))
				return null;
			List<ScheduledEvent> scheduledEvents = new List<ScheduledEvent>();
			int[] offsets = ScheduledEvent.GetOffsets(dateOffsets);
			if (offsets == null)
				return null;
			foreach (int offset in offsets)
			{
				scheduledEvents.Add(new ScheduledEvent(offset, _id, _isManagedItem));
			}
			return scheduledEvents;
		}



		private List<ScheduledEvent> Consolidate(List<ScheduledEvent> oldEvents, List<ScheduledEvent> newEvents)
		{
			if (oldEvents == null || oldEvents.Count == 0)
				return newEvents;

			List<ScheduledEvent> consolidatedEvents = new List<ScheduledEvent>(newEvents.Count);
			foreach (ScheduledEvent newEvent in newEvents)
			{
				consolidatedEvents.Add(newEvent);
			}

			foreach (ScheduledEvent oldEvent in oldEvents)
			{
				ScheduledEvent consolidatedEvent = consolidatedEvents.Find(p => p.DateOffset == oldEvent.DateOffset);
				if (consolidatedEvent != null)
				{
					consolidatedEvent.ID = oldEvent.ID;
				}
			}
			return consolidatedEvents;
		}

        //Look at the destinationEvents.  If a matching EventID is found in the sourceEvents, then migrate the ScheduledEvents.
        //This means that if a matching ScheduledEvent is found in the source ScheduledEvents, then replace the corresponding 
        //destination ScheduledEvent with that matching ScheduledEvent.  This will effectively preserve the ScheduledEventID 
        //from the source ScheduledEvent.
        public static void Migrate(List<Event> sourceEvents, List<Event> destinationEvents) 
        {
            foreach (Event destinationEvent in destinationEvents)
            {
                Event matchingSourceEvent = sourceEvents.Find(ev => ev.ID == destinationEvent.ID);
                if (matchingSourceEvent != null)
                {
                    List<ScheduledEvent> iterationScheduledEvents = new List<ScheduledEvent>(destinationEvent.ScheduledEvents);
                    foreach (ScheduledEvent destinationScheduledEvent in iterationScheduledEvents)
                    {
                        ScheduledEvent equivalentSourceScheduledEvent = ScheduledEvent.FindEquivalent(matchingSourceEvent.ScheduledEvents, destinationScheduledEvent);
                        if (equivalentSourceScheduledEvent != null)
                        {
                            destinationEvent.ScheduledEvents.Remove(destinationScheduledEvent);
                            destinationEvent.ScheduledEvents.Add(equivalentSourceScheduledEvent);
                        }
                    }
                }

                //At this point, there could be new ScheduledEvents from the template that did not have a valid ID.
                //Ensure that all ScheduledEvents have a valid ID.
                foreach (ScheduledEvent destinationScheduledEvent in destinationEvent.ScheduledEvents)
                {
                    destinationScheduledEvent.Migrate();
                }
            }
        }

        public void Load(Template template, string value, string pattern, char? delimiter)
        {
            if (!string.IsNullOrEmpty(value))
            {
                //TODO: May also need to account for List<DurationUnit> _initialDurationUnits and _renewalDurationUnits
                string[] values = value.Split(delimiter.Value);
                string[] sections = pattern.Split(delimiter.Value);
                if (values.Length != sections.Length)
                    return;
                Term term = null;
                for (int i = 0; i < sections.Length; i++)
                {
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        switch (sections[i])
                        {
                            //Expecting values[i] to be of the form 'Term Name^Effective Date' or 'Term Name^Expiration Date'
                            case _LABEL_BASE_DATE_TERM_NAME:
                                string termName = RenewalTerm.GetLoadBaseDateTermName(values[i]);
                                term = template.FindBasicTerm(termName);
                                if (term != null)
                                {
                                    switch (term.TermType)
                                    {
                                        case TermType.Date:
                                            BaseDateTermID = term.ID;
                                            break;

                                        case TermType.Renewal:
                                            switch (RenewalTerm.GetLoadBaseDateTermDisplayType(values[i]))
                                            {
                                                case DisplayedDate.EffectiveDate:
                                                    _baseDateTermPart = XMLNames._TPS_EffectiveDate;
                                                    break;

                                                case DisplayedDate.ExpirationDate:
                                                    _baseDateTermPart = XMLNames._TPS_ExpirationDate;
                                                    break;
                                            }
                                            BaseDateTermID = term.ID;
                                            break;
                                    }
                                }
                                break;

                            case _LABEL_OFFSET_TERM_NAME:
                                term = template.FindBasicTerm(values[i]);
                                if (term != null)
                                    OffsetTermID = term.ID;
                                break;

                            case _LABEL_OFFSET_DEFAULT_VALUE:
                                int offsetDefaultValue;
                                if (int.TryParse(values[i], out offsetDefaultValue))
                                    OffsetDefaultValue = offsetDefaultValue;
                                break;

                            case _LABEL_BASE_DATE_OFFSET:
                                BaseDateOffset = values[i];
                                break;
                        }
                    }
                }
            }
        }

	}
}
