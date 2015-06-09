using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class Workflow
	{
		#region private members

		private string _name;
		private Guid _id;
		private bool? _useFunction;				
		private int? _daysAfterWorkflowEntry;	
		private List<State> _states;
		private DateTime? _dateExitedBaseState;	//Note - this only has meaning to the ManagedItem.
		private List<Event> _events;
		private bool _isManagedItem = false;

		#endregion

		#region Properties

		public Guid ID
		{
			get { return _id; }
		}

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value.TrimEnd()); }
		}

		public bool? UseFunction
		{
			get { return _useFunction; }
			set { _useFunction = value; }
		}

		public int? DaysAfterWorkflowEntry
		{
			get { return _daysAfterWorkflowEntry; }
			set { _daysAfterWorkflowEntry = value; }
		}

		public List<State> States
		{
			get { return _states; }
            set { _states = value; }
		}

		public DateTime? DateExitedBaseState
		{
			get { return _dateExitedBaseState; }
			set { _dateExitedBaseState = value; }
		}

		public List<Event> Events
		{
			get { return _events; }
			set { _events = value; }
		}

		public Event RevertEvent
		{
			get
			{
				Event e = Events.Find(ev => ev.EventType == EventType.WorkflowRevertToDraft);
				if (e == null)
				{
					e = new Event(EventType.WorkflowRevertToDraft, _isManagedItem);
					e.Name = EventType.WorkflowRevertToDraft.ToString();
					Events.Add(e);
				}
				return e;
			}
			set
			{
				for (int nIndex = 0; nIndex < Events.Count; nIndex++)
				{
					if (Events[nIndex].EventType == EventType.WorkflowRevertToDraft)
						Events.RemoveAt(nIndex);
				}
				if (value != null)
					Events.Add(value);
			}
		}

		public Message RevertMessage
		{
			get
			{
				if (RevertEvent.Messages.Count == 0)
					RevertEvent.Messages.Add(new Message());
				return RevertEvent.Messages[0];
			}
			set
			{
				//TODO Note - this method assumes there is only one message for the RevertEvent
				if (RevertEvent.Messages.Count == 0)
					RevertEvent.Messages.Add(new Message());
				RevertEvent.Messages[0] = value;
			}
		}

        public Event RetroRevertEvent
        {
            get
            {
				Event e = Events.Find(ev => ev.EventType == EventType.RetroRevertToDraft);
				if (e == null)
				{
					e = new Event(EventType.RetroRevertToDraft, _isManagedItem);
					e.Name = EventType.RetroRevertToDraft.ToString();
					Events.Add(e);
				}
				return e;
            }
            set
            {
                for (int nIndex = 0; nIndex < Events.Count; nIndex++)
                {
                    if (Events[nIndex].EventType == EventType.RetroRevertToDraft)
                        Events.RemoveAt(nIndex);
                }
                if (value != null)
                    Events.Add(value);
            }
        }

        public Message RetroRevertMessage
        {
            get
            {
                if (RetroRevertEvent.Messages.Count == 0)
                    RetroRevertEvent.Messages.Add(new Message());
                return RetroRevertEvent.Messages[0];
            }
            set
            {
                //TODO Note - this method assumes there is only one message for the RevertEvent
                if (RetroRevertEvent.Messages.Count == 0)
                    RetroRevertEvent.Messages.Add(new Message());
                RetroRevertEvent.Messages[0] = value;
            }
        }

		#endregion

		#region Constructors

		public Workflow(string sName, bool IsManagedItem, Template template)
		{
			_isManagedItem = IsManagedItem;
			_name = sName;
			_id = Guid.NewGuid();
			_states = new List<State>();
            State baseState = new State(template);
			baseState.Name = "BaseState";
			baseState.Status = "BaseState Status";
			baseState.IsBase = true;
			baseState.IsDraft = true;
			baseState.IsExit = true;
			_states.Add(baseState);
			_events = new List<Event>();
		}

		public Workflow(XmlNode nodeWorkflow, bool IsManagedItem, Template template)
		{
			_isManagedItem = IsManagedItem;
			_name = Utility.XMLHelper.GetAttributeString(nodeWorkflow, XMLNames._A_Name);
			if (string.IsNullOrEmpty(_name))
				_name = "New Workflow";
			_id = Guid.Empty;
			string id = Utility.XMLHelper.GetAttributeString(nodeWorkflow, XMLNames._A_ID);
			if (!string.IsNullOrEmpty(id))
			{
				try
				{
					_id = new Guid(id);
				}
				catch
				{
				}
			}
			//If an ID was not assigned before, assign it now....
			if (_id == Guid.Empty)
				_id = Guid.NewGuid();

			_useFunction = Utility.XMLHelper.GetAttributeBool(nodeWorkflow, XMLNames._A_UseFunction);
			_daysAfterWorkflowEntry = Utility.XMLHelper.GetAttributeInt(nodeWorkflow, XMLNames._A_DaysAfterWorkflowEntry);

			//DateExitedBaseState
			XmlNode nodeDateExitedBaseState = nodeWorkflow.SelectSingleNode(XMLNames._E_DateExitedBaseState);
			if (nodeDateExitedBaseState != null)
			{
				_dateExitedBaseState = Utility.XMLHelper.GetValueDate(nodeDateExitedBaseState);
			}

			XmlNodeList listStates = nodeWorkflow.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_States, XMLNames._E_State));

			if (listStates != null)
			{
				_states = new List<State>(listStates.Count);
				foreach (XmlNode nodeState in listStates)
				{
                    State state = new State(nodeState, template, this);
					_states.Add(state);
				}

                foreach (State state in _states)
                {
                    foreach (Action action in state.Actions)
                    {
                        if (!action.TargetStateIDDefined)
                        {
                            State targetState = FindState(action.TargetState);
                            if (targetState == null)
                                throw new Exception(string.Format("Target State not found for action {0} of Workflow {1}, State {2}", action.ButtonText, _name, state.Name));
                            action.TargetStateID = targetState.ID;
                        }
                    }
                }
			}

			//RR - 12/10/2007 - added code (revertToDraftEventSet) to prevent loading of multiple RevertToDraft events for a workflow
			_events = new List<Event>();
			bool revertToDraftEventSet = false;
			XmlNodeList nodeNotifications = nodeWorkflow.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Events, XMLNames._E_Event));
			if (nodeNotifications != null)
			{
				foreach (XmlNode nodeNotification in nodeNotifications)
				{
					Event notification = new Event(nodeNotification, IsManagedItem);
					if (notification.EventType == EventType.WorkflowRevertToDraft)
					{
						if (!revertToDraftEventSet)
						{
							_events.Add(notification);
							revertToDraftEventSet = true;
						}
					}
					else
						_events.Add(notification);
				}
			}
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode nodeWorkflow, bool bValidate)
		{
			if (bValidate)
			{
			}

			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeWorkflow, XMLNames._A_Name, _name);
			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeWorkflow, XMLNames._A_ID, _id.ToString());
			Utility.XMLHelper.AddAttributeBool(xmlDoc, nodeWorkflow, XMLNames._A_UseFunction, _useFunction);
			Utility.XMLHelper.AddAttributeInt(xmlDoc, nodeWorkflow, XMLNames._A_DaysAfterWorkflowEntry, _daysAfterWorkflowEntry);

			if (_dateExitedBaseState.HasValue)
			{
				XmlNode elementDateExitedBaseState = xmlDoc.CreateElement(XMLNames._E_DateExitedBaseState);
				Utility.XMLHelper.SetValueDate(xmlDoc, elementDateExitedBaseState, _dateExitedBaseState);
				nodeWorkflow.AppendChild(elementDateExitedBaseState);
			}

			XmlElement nodeStates = xmlDoc.CreateElement(XMLNames._E_States);
			nodeWorkflow.AppendChild(nodeStates);

			foreach (State state in _states)
			{
				XmlElement nodeState = xmlDoc.CreateElement(XMLNames._E_State);
				state.Build(xmlDoc, nodeState, bValidate);
				nodeStates.AppendChild(nodeState);
			}

			XmlElement nodeEvents = xmlDoc.CreateElement(XMLNames._E_Events);
			foreach (Event eachEvent in _events)
			{
				XmlElement nodeEvent = xmlDoc.CreateElement(XMLNames._E_Event);
				eachEvent.Build(xmlDoc, nodeEvent, bValidate);
				nodeEvents.AppendChild(nodeEvent);
			}
			nodeWorkflow.AppendChild(nodeEvents);
		}

		#endregion

		#region public methods

		public State FindBaseState()
		{
			return _states.Find(IsBaseState);
		}

		public List<State> FindExitStates()
		{
			return _states.FindAll(IsExitState);
		}

		public List<State> FindDraftStates()
		{
			return _states.FindAll(IsDraftState);
		}

        public State FindState(string stateName) 
		{
			Predicate<State> p = delegate(State s) { return s.Name == stateName; };
			return _states.Find(p);
		}

        public State FindState(Guid stateID)
        {
            Predicate<State> p = delegate(State s) { return s.ID == stateID; };
            return _states.Find(p);
        }

		#endregion

		#region predicate methods

		private static bool IsBaseState(State state)
		{
			return (state.IsBase ?? false);
		}

		private static bool IsExitState(State state)
		{
			return (state.IsExit ?? false);
		}

		private static bool IsDraftState(State state)
		{
			return (state.IsDraft ?? false);
		}

		#endregion

		public List<string> StateReferences(Guid stateID)
		{
			List<string> rtn = new List<string>();

			foreach (State state in this.States)
				foreach (Action action in state.Actions)
                    if (action.TargetStateID == stateID)
						rtn.Add(string.Format("State: \"{0}\",  Button: \"{1}\"", state.Name, action.ButtonText));
			
			return rtn;
		}


		public bool GetRevertToBaseStateEventDate(DateTime managedItemEffectiveDate, out DateTime eventDate)
		{
			eventDate = DateTime.MinValue;
			if (_useFunction ?? false)
			{
				if (_dateExitedBaseState.HasValue)
				{
					eventDate = _dateExitedBaseState.Value.AddDays(_daysAfterWorkflowEntry ?? 0);
					if (managedItemEffectiveDate > eventDate)
						eventDate = managedItemEffectiveDate;
					return true;
				}
			}
			return false;
		}

		public Workflow Copy(string newName, bool IsManagedItem, Template template)
		{
            Workflow workflow = new Workflow(newName, IsManagedItem, template);
			workflow.States.Clear();
			workflow.UseFunction = UseFunction;
			workflow.DaysAfterWorkflowEntry = DaysAfterWorkflowEntry;
			workflow.DateExitedBaseState = DateExitedBaseState;

			//TODO - Does it make sense here to make copies of Events?
			foreach (Event eachEvent in Events)
			{
				workflow.Events.Add(eachEvent.Copy(IsManagedItem));
			}

			foreach (State state in States)
			{
                workflow.States.Add(state.Copy(template, workflow));
			}

            foreach (State state in workflow.States)
            {
                foreach (Action action in state.Actions)
                {
                    action.TargetStateID = workflow.FindState(action.TargetState).ID;
                }
            }

			return workflow;
		}

        public string GetNewWorkflowName(Template template, string oldName)
        {
            string startingWorkflowName = string.Concat("Copy of ", oldName);
            string workflowName = startingWorkflowName;
            int counter = 1;
            while (template.WorkflowExists(workflowName))
            {
                workflowName = string.Format("{0} {1}", startingWorkflowName, counter++);
            }
            return workflowName;
        }

        public void RemoveStateTermRoles()
        {
            foreach (State state in _states)
            {

            }
        }

	}
}
