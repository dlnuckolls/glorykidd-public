using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class Action
	{
		#region private members

		private string _buttonText;
		private string _targetState = null;
        private Guid _targetStateID;
		private List<Message> _messages;
		private List<string> _performers;
        private bool _targetStateIDDefined;
        private Workflow _workflow; //This is needed in order to access the TargetState Name
        private bool _requiresConfirmation;
        private string _confirmationText;

		#endregion

		#region Properties

		public string ButtonText
		{
			get { return Utility.XMLHelper.GetXMLText(_buttonText); }
			set { _buttonText = Utility.XMLHelper.SetXMLText(value); }
		}

		public string TargetState
		{
			get
			{
				if (!string.IsNullOrEmpty(_targetState))
					return _targetState;
				else
				{
					if (_targetStateID == Guid.Empty)
						return null;
					State state = _workflow.FindState(_targetStateID);
					if (state == null)
						return null;
					return state.Name;
				}
			}
		}

        public Guid TargetStateID
        {
            get { return _targetStateID; }
            set { 
                    _targetStateID = value;
                    _targetStateIDDefined = true;
                    _targetState = null;
                }
        }

        public bool TargetStateIDDefined
        {
            get { return _targetStateIDDefined; }
        }

        public bool RequiresConfirmation
        {
            get { return _requiresConfirmation; }
            set { _requiresConfirmation = value; }
        }

        public string ConfirmationText
        {
			get { return Utility.XMLHelper.GetXMLText(_confirmationText); }
			set { _confirmationText = Utility.XMLHelper.SetXMLText(value); }
        }

        public List<Message> Messages
		{
			get { return _messages; }
			set { _messages = value; }
		}

		public List<string> Performers
		{
			get { return _performers; }
			set { _performers = value; }
		}

		#endregion

		#region Constructors

        public Action(Workflow workflow)
		{
            _workflow = workflow;
            _messages = new List<Message>(1);
			_messages.Add(new Message());
			_performers = new List<string>();
		}

        public Action(XmlNode termNode, Workflow workflow)
		{
            _workflow = workflow;
			_buttonText = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_ButtonText);
            _requiresConfirmation = Utility.XMLHelper.GetAttributeBool(termNode, XMLNames._A_RequiresConfirmation) ?? false;
            _confirmationText = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_ConfirmationText);

            try
            {
                string targetStateID = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_TargetStateID);
                _targetStateID = new Guid(targetStateID);
                _targetStateIDDefined = true;
            }
            catch
            {
                _targetStateIDDefined = false;
                try
                {
                    _targetState = Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_TargetState);
                }
                catch
                {
                }
            }
			 
			XmlNodeList listMessages = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Messages,XMLNames._E_Message));

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
				_messages = new List<Message>(1);
				_messages.Add(new Message());
			}

			XmlNodeList listPerformers = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Performers,XMLNames._E_Performer));
			if (listPerformers != null && listPerformers.Count > 0)
			{
				_performers = new List<string>(listPerformers.Count);
				foreach (XmlNode nodePerformer in listPerformers)
				{
					string sRole = Utility.XMLHelper.GetAttributeString(nodePerformer, XMLNames._A_Role);
					_performers.Add(sRole);
				}
			}
			else
			{
				_performers = new List<string>();
			}
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
				Utility.XMLHelper.ValidateString(XMLNames._A_ButtonText, _buttonText);
			}

            Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_TargetStateID, _targetStateID.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ButtonText, _buttonText);

            if (_requiresConfirmation)
            {
                Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_RequiresConfirmation, _requiresConfirmation);
                Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_ConfirmationText, _confirmationText);
            }

			if (_messages != null)
			{
				XmlNode elementMessages = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Messages, XMLNames._M_NameSpaceURI);
				foreach (Message message in _messages)
				{
					XmlNode elementMessage = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Message, XMLNames._M_NameSpaceURI);
					message.Build(xmlDoc, elementMessage, bValidate);
					elementMessages.AppendChild(elementMessage);
				}
				termNode.AppendChild(elementMessages);
			}

			if (_performers != null)
			{
				XmlNode elementPerformers = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Performers, XMLNames._M_NameSpaceURI);
				foreach (string performer in _performers)
				{
					XmlNode elementPerformer = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Performer, XMLNames._M_NameSpaceURI);
					Utility.XMLHelper.AddAttributeString(xmlDoc, elementPerformer, XMLNames._A_Role, performer);
					elementPerformers.AppendChild(elementPerformer);
				}
				termNode.AppendChild(elementPerformers);
			}
		}

		#endregion

		public Action Copy(Workflow destinationWorkflow)
		{
            Action action = new Action(destinationWorkflow);
			action.ButtonText = ButtonText;
            //Note - After all of the new States have been created, go back and assign the TargetStateID's by
            //matching on the equivalent State Names between the two workflows.
            action._targetState = TargetState;
			action.Messages.Clear();
			foreach (Message message in Messages)
			{
				action.Messages.Add(message.Copy());
			}
			action.Performers.Clear();
			foreach (string performer in Performers)
			{
				action.Performers.Add(performer);
			}

			return action;
		}
 	}
}
