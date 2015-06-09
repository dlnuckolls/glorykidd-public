using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable, System.Diagnostics.DebuggerDisplay("Name={Name}, ID={ID}, Status={Status}")]
	public class State
	{

		#region private members
		
		private string _name;
        private Guid _id;
		private string _status;
		private bool? _isDraft;
		private bool? _isBase;
		private bool? _isExit;
		private bool? _requiresValidation;
		private List<Action> _actions;
        private List<StateTermGroup> _stateTermGroups;

		#endregion

        private static char ACTION_DELIMITER = '|'; 

		#region Properties

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}

        public Guid ID
        {
            get { return _id; }
        }

		public string Status
		{
			get { return Utility.XMLHelper.GetXMLText(_status); }
			set { _status = Utility.XMLHelper.SetXMLText(value); }
		}

		public bool? IsDraft
		{
			get { return _isDraft; }
			set { _isDraft = value; }
		}

		public bool? IsBase
		{
			get { return _isBase; }
			set { _isBase = value; }
		}

		public bool? IsExit
		{
			get { return _isExit; }
			set { _isExit = value; }
		}

		public bool? RequiresValidation
		{
			get { return _requiresValidation; }
			set { _requiresValidation = value; }
		}

 		public List<Action> Actions
		{
			get { return _actions; }
			set { _actions = value; }
		}

        public List<StateTermGroup> StateTermGroups
        {
            get { return _stateTermGroups; }
            set { _stateTermGroups = value; }
        }

        public List<string> AccessorRoleNames
        {
            get {
                    List<Role> roles = new List<Role>();
                    foreach (StateTermGroup stateTermGroup in _stateTermGroups)
                    {
                        roles.AddRange(stateTermGroup.Viewers);
                        roles.AddRange(stateTermGroup.Editors);
                    }
                    return roles.ConvertAll<string>(Business.Role.StringConverter); 
                }
        }

        public List<string> EditorRoleNames
        {
            get
            {
                List<Role> roles = new List<Role>();
                foreach (StateTermGroup stateTermGroup in _stateTermGroups)
                {
                    roles.AddRange(stateTermGroup.Editors);
                }
                return roles.ConvertAll<string>(Business.Role.StringConverter);
            }
        }

		#endregion

		#region Constructors

		public State(Template template)
		{
            _id = Guid.NewGuid();
            _actions = new List<Action>();
            _stateTermGroups = new List<StateTermGroup>();
            foreach (TermGroup termGroup in template.TermGroups)
            {
                StateTermGroup stateTermGroup = new StateTermGroup(termGroup.ID);
                _stateTermGroups.Add(stateTermGroup);
            }
		}

		public State(XmlNode termNode, Template template, Workflow workflow)
		{
			_name = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Name);

            _id = Guid.Empty;
            string id = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_ID);
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

			_status = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_Status);
			_isDraft = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_IsDraft);
			_isBase = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_IsBase);
			_isExit = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_IsExit);
			_requiresValidation = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_RequiresValidation);

            _stateTermGroups = new List<StateTermGroup>();
            XmlNodeList listStateTermGroups = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_StateTermGroups, XMLNames._E_StateTermGroup));
            bool bInitializeStateTermGroups = false;
            try
            {
                bInitializeStateTermGroups = listStateTermGroups.Count == 0;
            }
            catch
            {
                bInitializeStateTermGroups = true;
            }

            if (bInitializeStateTermGroups)
            {
                //Read in the old version - list of editor roles
                List<Role> editors = new List<Role>();
                XmlNodeList listEditors = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Editors, XMLNames._E_Editor));
                if (listEditors != null)
                {
                    foreach (XmlNode nodeEditor in listEditors)
                    {
                        Role role = new Role();
                        role.Name = Utility.XMLHelper.GetAttributeString(nodeEditor, XMLNames._A_Role);
                        editors.Add(role);
                    }
                }
                StateTermGroup stateTermGroup = new StateTermGroup(template.BasicSecurityTermGroupID);
                stateTermGroup.Editors = editors;
                ITATSystem system = ITATSystem.Get(template.SystemID);
                stateTermGroup.Viewers = system.Roles;
                stateTermGroup.AttachmentRemovers = editors;
                stateTermGroup.ScannedAttachmentRemovers = Role.FromNames(system.AllowedRoles(Business.XMLNames._AF_EditAttachment));
                _stateTermGroups.Add(stateTermGroup);
            }
            else
            {
                foreach (XmlNode nodeStateTermGroup in listStateTermGroups)
                {
                    StateTermGroup stateTermGroup = new StateTermGroup(nodeStateTermGroup);
                    _stateTermGroups.Add(stateTermGroup);
                }
            }
 
			XmlNodeList listActions = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Actions,XMLNames._E_Action));
			if (listActions != null)
			{
				_actions = new List<Action>(listActions.Count);
				foreach (XmlNode nodeAction in listActions)
				{
                    Action action = new Action(nodeAction, workflow);
					_actions.Add(action);
				}
			}
			else
			{
				_actions = new List<Action>();
			}

		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
				Utility.XMLHelper.ValidateString(XMLNames._A_Name, _name);
				Utility.XMLHelper.ValidateString(XMLNames._A_Status, _status);
			}

			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Name, _name);
            Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ID, _id.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Status, _status);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_IsDraft, _isDraft);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_IsBase, _isBase);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_IsExit, _isExit);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_RequiresValidation, _requiresValidation);

            XmlNode elementStateTermGroups = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_StateTermGroups, XMLNames._M_NameSpaceURI);
            foreach (StateTermGroup stateTermGroup in _stateTermGroups)
            {
                XmlNode elementStateTermGroup = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_StateTermGroup, XMLNames._M_NameSpaceURI);
                stateTermGroup.Build(xmlDoc, elementStateTermGroup, bValidate);
                elementStateTermGroups.AppendChild(elementStateTermGroup);
            }
            termNode.AppendChild(elementStateTermGroups);

			XmlNode elementActions = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Actions, XMLNames._M_NameSpaceURI);
			foreach (Action action in _actions)
			{
				XmlNode elementAction = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Action, XMLNames._M_NameSpaceURI);
				action.Build(xmlDoc, elementAction, bValidate);
				elementActions.AppendChild(elementAction);
			}
			termNode.AppendChild(elementActions);

		}

		#endregion

		public Action FindAction(string targetStateName)    //TODO: For the StateID change, look at removing this call
		{
			Predicate<Action> p = delegate(Action a) { return (a.TargetState == targetStateName); };
			return _actions.Find(p);
		}

        public Action FindAction(Guid targetStateID)
        {
            Predicate<Action> p = delegate(Action a) { return (a.TargetStateID == targetStateID); };
            return _actions.Find(p);
        }

        public Action FindAction(Guid targetStateID, string buttonText)
        {
            Predicate<Action> p = delegate(Action a) { return (a.TargetStateID == targetStateID && a.ButtonText == buttonText); };
            return _actions.Find(p);
        }

        public StateTermGroup GetTermGroup(Guid termGroupID)
        {
            Predicate<StateTermGroup> p = delegate(StateTermGroup a) { return (a.TermGroupID == termGroupID); };
            return _stateTermGroups.Find(p);
        }

        public void AddTermGroup(Guid termGroupID, bool copyRoles)
        {
            StateTermGroup stateTermGroup = new StateTermGroup(termGroupID);
            if (copyRoles && _stateTermGroups.Count > 0)
            {
                stateTermGroup.Editors = _stateTermGroups[0].Editors;
                stateTermGroup.Viewers = _stateTermGroups[0].Viewers;
                stateTermGroup.AttachmentRemovers = _stateTermGroups[0].AttachmentRemovers;
                stateTermGroup.ScannedAttachmentRemovers = _stateTermGroups[0].ScannedAttachmentRemovers;
            }
            _stateTermGroups.Add(stateTermGroup);
        }

        public void DeleteTermGroup(Guid termGroupID)
        {
            StateTermGroup stateTermGroup = GetTermGroup(termGroupID);
            if (stateTermGroup != null)
                _stateTermGroups.Remove(stateTermGroup);
        }
        
        public State Copy(Template sourceTemplate, Workflow destinationWorkflow)
		{
            State state = new State(sourceTemplate);
			state.Name = Name;
			state.Status = Status;
			state.IsDraft = IsDraft;
			state.IsBase = IsBase;
			state.IsExit = IsExit;
			state.RequiresValidation = RequiresValidation;
            state.StateTermGroups.Clear();
            foreach (StateTermGroup stateTermGroup in StateTermGroups)
			{
                state.StateTermGroups.Add(stateTermGroup.Copy());
			}
			state.Actions.Clear();
			foreach (Action action in Actions)
			{
                state.Actions.Add(action.Copy(destinationWorkflow));
			}
			return state;
		}

        public List<Guid> AccessibleTermGroups(List<string> userRoles)
        {
            List<Guid> termGroupIDs = new List<Guid>();
            foreach (StateTermGroup stateTermGroup in _stateTermGroups)
            {
                if (Utility.ListHelper.HaveAMatch(stateTermGroup.Viewers.ConvertAll<string>(Business.Role.StringConverter), userRoles))
                {
                    termGroupIDs.Add(stateTermGroup.TermGroupID);
                }
                else
                {
                    if (Utility.ListHelper.HaveAMatch(stateTermGroup.Editors.ConvertAll<string>(Role.StringConverter), userRoles))
                    {
                        termGroupIDs.Add(stateTermGroup.TermGroupID);
                    }
                }
            }
            return termGroupIDs;
        }

        public void SetGroupRoles(StateTermGroup.StateTermGroupRoleType stateTermGroupRoleType, List<Role> roles) 
        {
            foreach (StateTermGroup stateTermGroup in _stateTermGroups)
            {
                switch (stateTermGroupRoleType)
                {
                    case StateTermGroup.StateTermGroupRoleType.Viewer:
                        stateTermGroup.Viewers = new List<Role>(roles);
                        break;

                    case StateTermGroup.StateTermGroupRoleType.Editor:
                        stateTermGroup.Editors = new List<Role>(roles);
                        break;

                    case StateTermGroup.StateTermGroupRoleType.AttachmentRemover:
                        stateTermGroup.AttachmentRemovers = new List<Role>(roles);
                        break;

                    case StateTermGroup.StateTermGroupRoleType.ScannedAttachmentRemover:
                        stateTermGroup.ScannedAttachmentRemovers = new List<Role>(roles);
                        break;

                    default:
                        throw new Exception(string.Format("StateTermGroupRoleType {0} is not handled", stateTermGroupRoleType.ToString()));
                }
            }
        }

        public static string ParseStateName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Split(ACTION_DELIMITER)[0];
        }

        public static string ParseButtonText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Split(ACTION_DELIMITER)[1];
        }

        public static string CombineStateNameAndButtonText(string targetState, string buttonText)
        {
            if (string.IsNullOrEmpty(targetState))
                return targetState;
            return string.Format("{0}{1}{2}", targetState, ACTION_DELIMITER, buttonText);
        }

        public static string ValidName(string description, string value)
        {
            if (value.IndexOf(ACTION_DELIMITER) != -1)
                return string.Format("The {0} must not contain the character '{1}'", description, ACTION_DELIMITER);
            return string.Empty;
        }

	}
}
