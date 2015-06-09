using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Transactions;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class Template
	{
		#region private members
		private Guid _id;
		private Guid _systemID;
		private string _name;
		private string _description;
		private TemplateStatusType _status;
		private bool _canGenerateDocument;
        private List<Term> _basicTerms;
        private List<TermGroup> _termGroups;
        private Dictionary<Guid /* TermGroupID */, List<Term>> _termGroupTerms;
        private List<Term> _complexLists;
		private List<Workflow> _workflows;
		private List<Extension> _extensions;
		private List<Event> _events;
		private List<Comment> _comments;
		private List<TermDependency> _termDependencies;
		private List<DetailedDescription> _detailedDescriptions;
        private List<Role> _documentPrinters;
        private List<Role> _userDocumentPrinters;
        private List<ITATDocument> _documents;

		private List<string> _sourceTerms;
		private Guid _activeWorkflowID;
		
		[NonSerialized]
		protected string _templateDef;
		private DefType _defType = DefType.NotAssigned;

		private bool _allowAttachments;
		private bool _allowComments = true;
		private bool _useDetailedDescription = false;		

		private bool _summaryLoaded = false;
		private bool _basicTermsLoaded = false;
		private bool _documentsLoaded = false;
		private bool _complexListsLoaded = false;
		private bool _workflowLoaded = false;
		private bool _msoDefined = false;
		private bool _renewalDefined = false;
		private bool _extensionsLoaded = false;
		private bool _eventsLoaded = false;
		private bool _commentsLoaded = false;
        private bool _documentPrintersLoaded = false;
        private bool _userDocumentPrintersLoaded = false;
        private bool _detailedDescriptionsLoaded = false;
		private bool _termDependenciesLoaded = false;
        private bool _termGroupsLoaded;

		private string _version;
		private string _XMLVersion;

		private SecurityModel _securityModel = SecurityModel.Basic;
		private Retro.RetroModel _retroModel;

        private DateTime? _retroDate;
        private bool _canGenerateUserDocuments;




		#endregion


		#region Properties

		public Guid ID
		{
			get { return _id; }
			set { _id = value; }
		}

		public Guid SystemID
		{
			get {
					if (!_summaryLoaded)
						LoadSummary();
					return _systemID; 
				}
		}

		public Guid ActiveWorkflowID
		{
			get { return _activeWorkflowID; }
			set {_activeWorkflowID = value;	}
		}

		public DefType DefType
		{
			get
			{
				return _defType;
			}
		}

		public string TemplateDef
		{
            get
            {
                if (_templateDef == null)
                {
                    _templateDef = GetTemplateDef(this, this.ID, _defType);
                    if (_templateDef == null)
                    {
                        //If this template is based on a newly created ManagedItem, then the first TemplateDef comes from the Final version
                        if (IsManagedItem)
                            _templateDef = GetTemplateDef(this, this.ID, DefType.Final);
                        if (_templateDef == null)
                        {
                            string sException = string.Format("Unable to obtain the {0} TemplateDef", _defType.ToString());
                            throw new Exception(sException);
                        }
                    }
                }
                return _templateDef;
            }
        }

		public bool MSODefined
		{
			get {
					Refresh();
					return _msoDefined; 
				}
		}

		public bool RenewalDefined
		{
			get {
					Refresh();
					return _renewalDefined; 
				}
		}

		public bool ExternalTermDefined(string interfaceConfigName)
		{
			if (_basicTerms == null)
				return false;
			foreach (Term term in _basicTerms)
				if (term.TermType == TermType.External)
					if (term.Name == interfaceConfigName)  // ASSUMPTION: the Name of an External Term is the same as the InterfaceConfigName
						return true;
			return false;
		}

		public bool IsManagedItem
		{
			get
			{
				if (_defType == DefType.ManagedItem)
				{
					if ((this is ManagedItem))
						return true;
					else
						throw new Exception(string.Format("Failed to cast TemplateID {0} to ManagedItem", ID.ToString()));
				}
				if ((this is ManagedItem))
				{
					ManagedItem managedItem = this as ManagedItem;
					throw new Exception(string.Format("Incorrect DefType assignment '{0}' for ManagedItemID {1}, TemplateID {2}", _defType.ToString(), managedItem.ManagedItemID.ToString(),ID.ToString()));
				}

				return false;
			}
		}

		public string Name
		{
			get
			{
				if (!_summaryLoaded)
					LoadSummary();
				return Utility.XMLHelper.GetXMLText(_name);
			}
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}

        public Guid BasicSecurityTermGroupID
        {
            get
            {
                if (_securityModel == SecurityModel.Basic)
                    return FindTermGroup(TermGroup.TermGroupType.BasicSecurity).ID;
                else
                    throw new Exception(string.Format("Requested a Basic Term Group when the Security Model is {0}", _securityModel.ToString()));
            }
        }

        public string Description
		{
			get
			{
				if (!_summaryLoaded)
					LoadSummary();
				return Utility.XMLHelper.GetXMLText(_description);
			}
			set { _description = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Version
		{
			get { return Utility.XMLHelper.GetXMLText(_version); }
			set { _version = Utility.XMLHelper.SetXMLText(value); }
		}

		public string XMLVersion
		{
			get { return Utility.XMLHelper.GetXMLText(_XMLVersion); }
		}

        public DateTime? RetroDate
        {
            get { return _retroDate; }
            set { _retroDate = value; }
        }

		public TemplateStatusType Status
		{
			get
			{
				if (!_summaryLoaded)
					LoadSummary();
				return _status;
			}
			set { _status = value; }
		}

		public bool CanGenerateDocument
		{
			get
			{
				if (!_summaryLoaded)
					LoadSummary();
				return _canGenerateDocument;
			}
			set { _canGenerateDocument = value; }
		}

        public bool CanGenerateUserDocuments
        {
            get
            {
                if (!_summaryLoaded)
                    LoadSummary();
                return _canGenerateUserDocuments;
            }
            set { _canGenerateUserDocuments = value; }
        }


		public bool AllowAttachments
		{
			get { return _allowAttachments; }
			set { _allowAttachments = value; }
		}

		public bool UseDetailedDescription
		{
			get { return _useDetailedDescription; }
			set { _useDetailedDescription = value; }
		}

		public bool AllowComments
		{
		    get { return _allowComments; }
		    set { _allowComments = value; }
		}

		public bool BasicTermsLoaded
		{
			get { return _basicTermsLoaded; }
		}

		public bool DocumentsLoaded
		{
			get { return _documentsLoaded; }
		}

		public bool ExtensionsLoaded
		{
			get { return _extensionsLoaded; }
		}

		public bool EventsLoaded
		{
			get { return _eventsLoaded; }
		}

		public bool DetailedDescriptionsLoaded
		{
			get { return _detailedDescriptionsLoaded; }
		}

		public bool ComplexListsLoaded
		{
			get { return _complexListsLoaded; }
		}

		public bool WorkflowLoaded
		{
			get { return _workflowLoaded; }
		}

		public bool CommentsLoaded
		{
			get { return _commentsLoaded; }
		}

        public bool DocumentPrintersLoaded
        {
            get { return _documentPrintersLoaded; }
        }

        public bool UserDocumentPrintersLoaded
        {
            get { return _userDocumentPrintersLoaded; }
        }

		public bool TermDependenciesLoaded
		{
			get { return _termDependenciesLoaded; }
		}

        public bool TermGroupsLoaded
        {
            get { return _termGroupsLoaded; }
        }

		public List<Term> BasicTerms
		{
			get
			{
				if (!_basicTermsLoaded)
					LoadBasicTerms();
				return _basicTerms;
			}
		}

        public List<Term> HeaderTerms
        {
            get
            {
                if (!_basicTermsLoaded)
                    LoadBasicTerms();
                Predicate<Term> p = delegate(Term t) { return (t.IsHeader); };
                return BasicTerms.FindAll(p);
            }
        }

		public List<TermDependency> TermDependencies
		{
			get
			{
				if (!_termDependenciesLoaded)
					LoadTermDependencies();
				return _termDependencies;
			}
		}

		//This must return a list of names, not ID's
		public List<string> SourceTerms
		{
			get
			{
				if (_sourceTerms == null)
				{
					_sourceTerms = new List<string>();
					foreach (TermDependency td in TermDependencies)
					{
						foreach (TermDependencyCondition condition in td.Conditions)
						{
							if (!_sourceTerms.Contains(condition.SourceTerm))
							{
								_sourceTerms.Add(condition.SourceTerm);
							}
						}
					}
                }
				return _sourceTerms;
			}
		}

		public List<Term> ComplexLists
		{
			get
			{
				if (!_complexListsLoaded)
					LoadComplexLists();
				return _complexLists;
			}
		}

		public List<ITATDocument> Documents
		{
			get
			{
                if (!_documentsLoaded)
                {
                    LoadDocuments();
                    _documentsLoaded = true;
                }
				return _documents;
			}
			set { _documents = value; }
		}

		public List<Extension> Extensions
		{
			get
			{
				if (!_extensionsLoaded)
					LoadExtensions();
				return _extensions;
			}
		}

		public List<Event> Events
		{
			get
			{
				if (!_eventsLoaded)
					LoadEvents(this is ManagedItem);
				return _events;
			}
		}

		public List<Comment> Comments
		{
			get
			{
				if (!_commentsLoaded)
					LoadComments();
				return _comments;
			}
            set{ _comments = value;}

		}

        public List<Role> DocumentPrinters
        {
            get
            {
                if (!_documentPrintersLoaded)
                    LoadDocumentPrinters();
                return _documentPrinters;
            }
            set { _documentPrinters = value; }

        }


        public List<Role> UserDocumentPrinters
        {
            get
            {
                if (!_userDocumentPrintersLoaded)
                    LoadUserDocumentPrinters();
                return _userDocumentPrinters;
            }
            set { _userDocumentPrinters = value; }

        }

        public List<TermGroup> TermGroups
        {
            get
            {
                if (!_termGroupsLoaded)
                    LoadTermGroups();
                return _termGroups;
            }
            set
            {
                _termGroups = value;
            }
        }

        public Dictionary<Guid /* TermGroupID */, List<Term>> TermGroupTerms
        {
            get
            {
                if (!_termGroupsLoaded)
                    LoadTermGroups();
                return _termGroupTerms;
            }
            //Note - this accessor is intended for use from the Profile screen.
            set
            {
                _termGroupTerms = value;
                RefreshTermGroupTerms();
            }
        }

		public List<DetailedDescription> DetailedDescriptions
		{
			get
			{
				if (!_detailedDescriptionsLoaded)
					LoadDetailedDescriptions();
				return _detailedDescriptions;
			}
			set 
			{ 
				_detailedDescriptions = value;
			}
		}
 
		public Workflow Workflow
		{
			get
			{
				if (!_workflowLoaded)
				{
					LoadWorkflow();
				}
				Workflow workflow = Business.Workflows.ActiveWorkflow(this);
				if (workflow == null)
					throw new Exception(string.Format("The Active Workflow is not defined - tried to locate with Guid '{0}'",this.ActiveWorkflowID.ToString()));
				return workflow;
			}
		}

		public List<Workflow> Workflows
		{
			get
			{
				if (!_workflowLoaded)
				{
					LoadWorkflow();
				}
				return _workflows;
			}
		}

		public FacilityTerm PrimaryFacility
		{
			get
			{
				if (BasicTerms == null)
					return null;
				foreach (Term term in BasicTerms)
				{
					if (term.TermType == TermType.Facility)
					{
						FacilityTerm facilityTerm = term as Business.FacilityTerm;
						if (facilityTerm.IsPrimary ?? false)
						{
							return facilityTerm;
						}
					}
				}
				return null;
			}
		}

		public RenewalTerm RenewalTerm
		{
			get
			{
				return BasicTerms.Find(t => t.TermType == TermType.Renewal) as RenewalTerm;
				//if (BasicTerms == null)
				//    return null;
				//foreach (Term term in BasicTerms)
				//{
				//    if (term.TermType == TermType.Renewal)
				//    {
				//        return term as Business.RenewalTerm;
				//    }
				//}
				//return null;
			}
		}

		public Message RenewalTermMessage
		{
			get
			{
				Event re = RenewalEvent;
				if (re == null)
					return null;
				if (re.Messages == null)
					re.Messages = new List<Message>();
				if (re.Messages.Count == 0)
					re.Messages.Add(new Message());
				return re.Messages[0]; //GetEventMessage(EventType.Renewal);
			}
			//set 
			//{
			//    SetEventMessage(EventType.Renewal, value);
			//}
		}


		public Message WorkflowRevertMessage
		{
			get
			{
				return Workflow.RevertEvent.Messages[0];
			}
			set
			{
				//TODO Note - this method assumes there is only one message for the RevertEvent
				Workflow.RevertEvent.Messages[0] = value;
			}
		}

		
		//public Event RenewalEvent
		//{
		//    get
		//    {
		//        if (this.RenewalTerm == null)
		//            return null;
		//        return this.RenewalTerm.RenewalEvent;  //GetEvent(EventType.Renewal);
		//    }
		//}


		public Event RenewalEvent
		{
			get { return Events.Find(ev => ev.EventType == EventType.Renewal); }
		}

		public Event WorkflowRevertEvent
		{
			get
			{
				return Workflow.RevertEvent;
			}
			set
			{
				Workflow.RevertEvent = value;
			}
		}

		//This accessor is only needed when initializing Workflows from the old structure.
		public Event OldWorkflowRevertEvent
		{
			get
			{
				return GetEvent(EventType.WorkflowRevertToDraft);
			}
			set
			{
				if (value == null)
					DeleteEvent(EventType.WorkflowRevertToDraft);
				else
					throw new Exception("Invalid use of limited accessor OldWorkflowRevertEvent");
			}
		}

		public SecurityModel SecurityModel
		{
			get 
            {
                if (!_summaryLoaded)
                    LoadSummary();                
                return _securityModel;
            }
			set 
            {
                if (_securityModel != value)
                {
                    switch (value)
                    {
                        case SecurityModel.Basic:
                            DownGradeSecurity();
                            break;
                        case SecurityModel.Advanced:
                            UpgradeSecurity();
                            break;
                    }
                    _securityModel = value;
                }
            }
		}

		public Retro.RetroModel RetroModel
		{
			get 
            {
                if (!_summaryLoaded)
                    LoadSummary();
                return _retroModel;
            }
			set { _retroModel = value; }
		}



		#endregion


		#region constructors

		public Template(Guid id, DefType defType)
		{
			_id = id;
			_defType = defType;
		}

		#endregion


		#region private methods

		private void LoadVersion()
		{
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(TemplateDef);
			XmlElement root = xmlTemplateDoc.DocumentElement;
			_XMLVersion = Utility.XMLHelper.GetAttributeString(root,XMLNames._A_XMLVersion);
			_version = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_Version);
		}

        private static string GetVersion(string templateDef)
        {
            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(templateDef);
            XmlElement root = xmlTemplateDoc.DocumentElement;
            return Utility.XMLHelper.GetAttributeString(root, XMLNames._A_Version);
        }


		private void LoadSummary()
		{
			LoadVersion();
			using (DataSet ds = Data.Template.GetTemplateSummary(_id, TemplateDef))
			{
				DataRow row = ds.Tables[0].Rows[0];
				_name = (string)row[Data.DataNames._C_TemplateName];
				_description = (string)row[Data.DataNames._C_Description];
				_canGenerateDocument = bool.Parse((string)row[Data.DataNames._C_GenerateDocument]);
                bool canGenerateUserDocuments;

                if (bool.TryParse((string)row[Data.DataNames._C_GenerateUserDocuments], out canGenerateUserDocuments))
                    _canGenerateUserDocuments = canGenerateUserDocuments;
                else
                    _canGenerateUserDocuments = false;
                
				_status = (TemplateStatusType)((short)row[Data.DataNames._C_Status]);
				_allowAttachments = bool.Parse((string)row[Data.DataNames._C_Attachments]);
                bool useDetailedDescription;
                if (bool.TryParse((string)row[Data.DataNames._C_UseDetailedDescription], out useDetailedDescription))
                    _useDetailedDescription = useDetailedDescription;
                else
                    _useDetailedDescription = false;
                bool allowComments;
                if (bool.TryParse((string)row[Data.DataNames._C_AllowComments], out allowComments))
                    _allowComments = allowComments;
                else
                    _allowComments = false;

                try
                {
                    _securityModel = (SecurityModel)Enum.Parse(typeof(SecurityModel), (string)row[Data.DataNames._C_SecurityModel]);
                }
                catch
                {
                    _securityModel = SecurityModel.Basic;
                }

                try
                {
					_retroModel = (Retro.RetroModel)Enum.Parse(typeof(Retro.RetroModel), (string)row[Data.DataNames._C_RetroModel]);
				}
                catch
                {
                    _retroModel = Retro.RetroModel.Off;
                }

                _systemID = (Guid)row[Data.DataNames._C_SystemID];
				_summaryLoaded = true;
			}
		}

		//If the template is null, then use the templateID
		//The caller is expected to set the TransactionScope
		public static string GetTemplateDef(Template template, Guid templateID, DefType defType)
		{
			Guid localTemplateID = template == null ? templateID : template.ID;
			string sTemplateDef = null;
			DataSet ds;
			try
			{
				switch (defType)
				{
					case DefType.NotAssigned:
						throw new Exception(string.Format("DefType not assigned for TemplateID {0}",templateID.ToString()));
					case DefType.Draft:
						ds = Data.Template.GetDraftTemplateDef(localTemplateID);
						sTemplateDef = (string)ds.Tables[0].Rows[0][Data.DataNames._C_DraftTemplateDef];
						break;
					case DefType.Final:
						ds = Data.Template.GetFinalTemplateDef(localTemplateID);
						sTemplateDef = (string)ds.Tables[0].Rows[0][Data.DataNames._C_FinalTemplateDef];
						break;
					case DefType.ManagedItem:
						if (template == null)
							throw new Exception("Failed to provide Template object for GetTemplateDef call");
						if (!template.IsManagedItem)
							throw new Exception(string.Format("Failed to cast TemplateID {0} to ManagedItem", localTemplateID.ToString()));
						ManagedItem managedItem = template as ManagedItem;
						sTemplateDef = Data.ManagedItem.GetTemplateDef(managedItem.ManagedItemID);
						break;
				}
				if (string.IsNullOrEmpty(sTemplateDef))
					return null;
			}
			catch
			{
				return null;
			}
			return sTemplateDef;
		}

        public static bool DraftTemplateDefValid(Guid templateID)
        {
            try
            {
                DataSet dsDraft = Data.Template.GetDraftTemplateDef(templateID);
                string sXml = dsDraft.Tables[0].Rows[0][Data.DataNames._C_DraftTemplateDef].ToString();
                return sXml.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        public static bool FinalTemplateDefValid(Guid templateID)
        {
            try
            {
                DataSet dsDraft = Data.Template.GetFinalTemplateDef(templateID);
                string sXml = dsDraft.Tables[0].Rows[0][Data.DataNames._C_FinalTemplateDef].ToString();
                return sXml.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private Event GetEvent(EventType messageEventType)
        {
            if (Events.Count != 0)
            {
                foreach (Event eachEvent in Events)
                {
                    if (eachEvent.EventType == messageEventType)
                        return eachEvent;
                }
            }
            Business.Event newEvent = new Event(messageEventType, this is ManagedItem);
            newEvent.Name = messageEventType.ToString();
            Events.Add(newEvent);
            return newEvent;
        }

        public Guid AddTermGroup(string name, string description, SecurityModel securityModel, Business.TermGroup.TermGroupType termGroupType)
        {
            TermGroup termGroup = new TermGroup(name, description, securityModel, termGroupType);
            TermGroups.Add(termGroup);
            foreach (Workflow workflow in Workflows)
            {
                foreach (State state in workflow.States)
                {
                    state.AddTermGroup(termGroup.ID, true);
                }
            }
            return termGroup.ID;
        }

        public TermGroup GetTermGroup(Guid termGroupID)
        {
            Predicate<TermGroup> p = delegate(TermGroup a) { return (a.ID == termGroupID); };
            return TermGroups.Find(p);
        }

       

        //It is assumed that the term group being deleted has been checked for term content by 'TermGroupInUse'
        public void DeleteTermGroup(Guid termGroupID)
        {
            foreach (Workflow workflow in Workflows)
            {
                foreach (State state in workflow.States)
                {
                    state.DeleteTermGroup(termGroupID);
                }
            }
            TermGroup termGroup = GetTermGroup(termGroupID);
            if (termGroup != null)
                _termGroups.Remove(termGroup);
        }

        //Use this call as a check when deleting a term group
        public bool TermGroupInUse(Guid termGroupID)
        {
            foreach (Term term in BasicTerms)
                if (term.TermGroupID == termGroupID)
                    return true;
            foreach (Term term in ComplexLists)
                if (term.TermGroupID == termGroupID)
                    return true;
            return false;
        }

        //Intended for use whenever a Complex List name is changed
        public void UpdateTermGroupNames()
        {
            if (SecurityModel == SecurityModel.Advanced)
                foreach (Term term in ComplexLists)
                {
                    TermGroup termGroup = GetTermGroup(term.TermGroupID);
                    termGroup.Name = term.Name;
                }
        }

        private bool DeleteEvent(EventType messageEventType)
        {
            for (int i = 0; i < Events.Count; i++)
            {
                try
                {
                    if (Events[i].EventType == messageEventType)
                    {
                        Events.RemoveAt(i);
                        i--;
                    }
                }
                catch { }
            }
            return true;
        }

        private Message GetEventMessage(EventType messageEventType)
        {
            //Note - this call assumes there is only one Event for the stated EventType.
            //It also assumes there is only one message for that Event.
            //If the message does not already exist, create it.
            Event messageEvent = GetEvent(messageEventType);
            if (messageEvent.Messages.Count == 0)
            {
                messageEvent.Messages.Add(new Message());
            }

            return messageEvent.Messages[0];
        }

        private bool SetEventMessage(EventType messageEventType, Message message)
        {
            Event messageEvent = GetEvent(messageEventType);
            if (message == null)
            {
                //Note - message == null is interpreted as a command to delete the event.
                return DeleteEvent(messageEventType);
            }
            else
            {
                //Add the message - assume it is to be the ONLY message.
                if (messageEvent.Messages == null)
                    messageEvent.Messages = new List<Message>(1);
                int nCount = messageEvent.Messages.Count;
                for (int i = 0; i < nCount; i++)
                    messageEvent.Messages.RemoveAt(0);
                messageEvent.Messages.Add(message);
            }

            return true;
        }

        #endregion


        #region public methods

        public string GetManagedItemName()
        {
            ITATSystem system = ITATSystem.Get(this.SystemID);
            return system.ManagedItemName;
        }

        public void DumpTemplateDef()
        {
            _templateDef = null;
        }

        private static void GetNewVersion(Template template, ref string templateDef)
        {
            //Update the Version attribute with a new guid for storing in the database
            //TODO - Old Templates - Remove the test for 'null' when the database is cleaned of older templates.
            string newVersion = Guid.NewGuid().ToString();
            if (!string.IsNullOrEmpty(template._version))
                templateDef = templateDef.Replace(template._version, newVersion);
            template._version = newVersion;
        }

        public static bool UpdateTemplateDef(Template template, string templateDef)
        {
            //Note - It is assumed that the caller will update the ManagedItemAudit table, as needed.
            switch (template._defType)
            {
                case DefType.NotAssigned:
                    throw new Exception(string.Format("DefType not assigned for TemplateID {0}", template.ID.ToString()));
                case DefType.Draft:
                    GetNewVersion(template, ref templateDef);
                    using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
                    {
                        Data.Template.UpdateDraftTemplateDef(template.ID, templateDef);
                        Data.Template.InsertTemplateAudit(template.ID, Business.SecurityHelper.GetLoggedOnPersonId(), (short)template.Status, templateDef, (int)Retro.AuditType.Saved);
                        transScope.Complete();
                    }
                    break;
                case DefType.Final:
                    Data.Template.UpdateFinalTemplateDef(template.ID, templateDef);
                    break;
                case DefType.ManagedItem:
                    if (!template.IsManagedItem)
                        throw new Exception(string.Format("Failed to cast TemplateID {0} to ManagedItem", template.ID.ToString()));
                    ManagedItem managedItem = template as ManagedItem;
                    Data.ManagedItem.UpdateTemplateDef(managedItem.ManagedItemID, templateDef);
                    break;
            }
            template.DumpTemplateDef();
            return true;
        }


        protected bool LoadWithoutComplexLists(XmlDocument xmlTemplateDoc)
        {
            LoadSummary();

            _basicTerms = Business.BasicTerms.Create(xmlTemplateDoc, this as ManagedItem, this);  //note: if "this" is a Template and not a ManagedItem, "this as ManagedItem" equals null.
            _basicTermsLoaded = true;

            //Need to load events before loading workflow, since the WorkflowRevertToDraft message
            //might be in the events collection (old way).
            LoadEvents(xmlTemplateDoc);

            _workflowLoaded = true;
            _workflows = Business.Workflows.Create(xmlTemplateDoc, this);

            return true;
        }

        protected bool LoadComplexLists(XmlDocument xmlTemplateDoc)
        {
            LoadSummary();

            _complexLists = Business.ComplexLists.Create(xmlTemplateDoc, this);
            _complexListsLoaded = true;

            return true;
        }

        //This is called if a term name has been changed
        public List<string> ReplaceEmbeddedAlteredTermNames()
        {
            List<string> sErrors = new List<string>();
            //If the Template XML version is current enough, there will not be any embedded term names.
            if (TemplateXMLVersionIndex(XMLVersion) >= TemplateXMLVersionIndex(XMLNames._M_TemplateXMLVersion_T2007OCT17))
                return sErrors;
            sErrors.AddRange(Business.BasicTerms.ReplaceEmbeddedTermNames(this, BasicTerms));
            Business.ComplexLists.SubstituteTermNames(this);
            sErrors.AddRange(Business.Events.ReplaceEmbeddedTermNames(this, Events));

            foreach (ITATDocument document in Documents)
            {
                Business.ITATClause.SubstituteTermNames(this, document.Clause);
            }


            foreach (ITATDocument document in Documents)
            {
                Business.ITATClause.ReplaceEmbeddedTermNames(this, document.Clause, sErrors);
            }
            
            sErrors.AddRange(Business.TermDependency.ReplaceEmbeddedTermNames(this, TermDependencies));
            Business.Workflows.SubstituteTermNames(this);
            return sErrors;
        }

        #region Public Searches

        public bool TermGroupExists(string termGroupName)
        {
            if (string.IsNullOrEmpty(termGroupName))
                return false;
            Predicate<TermGroup> p = delegate(TermGroup tg) { return (tg.Name == termGroupName); };
            return TermGroups.Exists(p);
        }


        public TermGroup FindTermGroup(Guid termGroupId)
        {
            if (termGroupId == Guid.Empty)
                return null;
            Predicate<TermGroup> p = delegate(TermGroup tg) { return (tg.ID == termGroupId); };
            return TermGroups.Find(p);
        }

        public TermGroup FindTermGroup(TermGroup.TermGroupType type)
        {
            Predicate<TermGroup> p = delegate(TermGroup tg) { return (tg.Type == type); };
            return TermGroups.Find(p);
        }

        public TermGroup FindTermGroup(string termGroupName)
        {
            if (string.IsNullOrEmpty(termGroupName))
                return null;
            Predicate<TermGroup> p = delegate(TermGroup tg) { return (tg.Name == termGroupName); };
            return TermGroups.Find(p);
        }

        public int FindTermGroupIndex(Guid termGroupID, List<TermGroup> termGroups)
        {
            Predicate<TermGroup> p = delegate(TermGroup t) { return (t.ID == termGroupID); };
            return termGroups.FindIndex(p);
        }

        public List<TermGroup> GetTermGroups(TermGroup.TermGroupType type)
        {
            Predicate<TermGroup> p = delegate(TermGroup tg) { return (tg.Type == type); };
            return TermGroups.FindAll(p);
        }

        public List<Term> GetTermsByTermGroupID(Guid termGroupID)
        {
            Predicate<Term> p = delegate(Term t) { return (t.TermGroupID == termGroupID); };
            return BasicTerms.FindAll(p);
        }

        public bool BasicTermExists(string termName)
        {
            if (string.IsNullOrEmpty(termName))
                return false;
            Predicate<Term> p = delegate(Term t) { return (t.Name == termName); };
            return BasicTerms.Exists(p);
        }

        public bool BasicTerm_FirstChar_Exists(string termName, char cFirst)
        {
            if (string.IsNullOrEmpty(termName))
                return false;
            string sNamePart = Term.NameAtFirstChar(termName, cFirst);
            if (sNamePart == termName)
                return false;
            Predicate<Term> p = delegate(Term t) { return (Term.NameAtFirstChar(t.Name, cFirst) == sNamePart); };
            return BasicTerms.Exists(p);
        }


        //Returns true if a term exists with Name == termName AND the term's Index IS NOT EQUAL to index
        //Returns true if the suggested term name is already in use by another term
        public bool TermNameInUse(string termName, Guid termID)
        {
            if (string.IsNullOrEmpty(termName))
                return false;
            Predicate<Term> p = delegate(Term t) { return (t.Name == termName); };
            //The FindIndex search stops on the first match.  If there are more than one
            //match, and the search happened to stop on the current index, then the other match
            //would not be detected.  Therefore check for >= 2 matches first.
            List<Term> termMatches = BasicTerms.FindAll(p);
            if (termMatches == null)
                return false;
            if (termMatches.Count == 0)
                return false;
            if (termMatches.Count >= 2)
                return true;

            return !termMatches[0].ID.Equals(termID);
        }

        //TermName_FirstChar_InUse

        //20070810_DEG Bug #131 Returns true if a term exists with Name(up to first quote) == termName(up to first quote) AND the term's Index IS NOT EQUAL to index
        public bool TermName_FirstChar_InUse(string termName, char cFirst, Guid termID)
        {
            if (string.IsNullOrEmpty(termName))
                return false;
            string sNamePart = Term.NameAtFirstChar(termName, cFirst);
            if (sNamePart == termName)
                return false;
            Predicate<Term> p = delegate(Term t) { return (Term.NameAtFirstChar(t.Name, cFirst) == sNamePart); };
            //The FindIndex search stops on the first match.  If there are more than one
            //match, and the search happened to stop on the current index, then the other match
            //would not be detected.  Therefore check for >= 2 matches first.
            List<Term> termMatches = BasicTerms.FindAll(p);
            if (termMatches == null)
                return false;
            if (termMatches.Count == 0)
                return false;
            if (termMatches.Count >= 2)
                return true;

            return !termMatches[0].ID.Equals(termID);
        }

        public Term FindBasicTerm(string termName)
        {
            if (string.IsNullOrEmpty(termName))
                return null;
            Predicate<Term> p = delegate(Term t) { return (t.Name == termName); };
            return BasicTerms.Find(p);
        }

        public Term FindTerm(Guid termID, string termName)
        {
            if (Term.ValidID(termID))
            {
                return FindTerm(termID);
            }
            else if (!string.IsNullOrEmpty(termName))
            {
                return FindTerm(termName);
            }
            else
                return null;
        }

        public string FindTermName(Guid termID, string termName)
        {
            if (!string.IsNullOrEmpty(termName))
                return termName;
            if (Term.ValidID(termID))
            {
                Term t = FindTerm(termID);
                if (t != null)
                    return t.Name;
            }
            return null;
        }

        public bool MatchTerm(Guid termID, string termName, string termNameMatch)
        {
            Term term = FindTerm(termID, termName);
            if (term == null)
                return false;
            return term.Name == termNameMatch;
        }

        public Term FindTerm(Guid termID)
        {
            Term term = null;
            Predicate<Term> p = delegate(Term t) { return (t.ID == termID); };
            term = BasicTerms.Find(p);
            if (term != null)
                return term;
            return ComplexLists.Find(p);
        }

        public int FindTermIndex(Guid termID, List<Term> terms)
        {
            Predicate<Term> p = delegate(Term t) { return (t.ID == termID); };
            return terms.FindIndex(p);
        }

        private bool ReplaceTerm(Term term, List<Term> terms)
        {
            int termIndex = FindTermIndex(term.ID, terms);
            if (termIndex >= 0)
            {
                terms.Insert(termIndex, term);
                terms.RemoveAt(termIndex + 1);
                return true;
            }
            return false;
        }

        public Workflow FindWorkflow(Guid workflowID)
        {
            Predicate<Workflow> p = delegate(Workflow w) { return (w.ID == workflowID); };
            return Workflows.Find(p); ;
        }

        public Workflow FindWorkflow(string workflowName)
        {
            Predicate<Workflow> p = delegate(Workflow w) { return (w.Name == workflowName); };
            return Workflows.Find(p); ;
        }

        public bool WorkflowExists(string workflowName)
        {
            if (string.IsNullOrEmpty(workflowName))
                return false;
            Predicate<Workflow> p = delegate(Workflow w) { return (w.Name == workflowName); };
            return Workflows.Exists(p);
        }


        public Term FindBasicTerm(string termName, TermType termType)
        {
            if (string.IsNullOrEmpty(termName))
                return null;
            if (termType == TermType.ComplexList)
                return null;

            Predicate<Term> p = delegate(Term t) { return ((t.Name == termName) && (t.TermType == termType)); };
            return BasicTerms.Find(p);
        }


        public Term FindBasicTerm(TermType termType)
        {
            if (termType == TermType.ComplexList)
                return null;
            Predicate<Term> p = delegate(Term t) { return t.TermType == termType; };
            return BasicTerms.Find(p);
        }

        public String FindDetailedDescription(DetailedDescription.DetailedDescriptionType detailedDescriptionType)
        {
            if (detailedDescriptionType == DetailedDescription.DetailedDescriptionType.None)
                return null;
            Predicate<DetailedDescription> p = delegate(DetailedDescription t) { return t.DescriptionType == detailedDescriptionType; };
            return DetailedDescriptions.Find(p).Text;
        }

        public List<Term> FindAllBasicTerms(TermType termType)
        {
            if (termType == TermType.ComplexList)
                return null;
            Predicate<Term> p = delegate(Term t) { return t.TermType == termType; };
            return BasicTerms.FindAll(p);
        }


        public List<Term> FindAllBasicTerms(TermGroup tg)
        {
            List<Term> rtn = new List<Term>();
            foreach (Term t in BasicTerms)
                if (t.TermGroupID == tg.ID)
                    rtn.Add(t);
            return rtn;
        }


        public bool ComplexListTermNameInUse(string termName, Guid? termID)
        {
            if (string.IsNullOrEmpty(termName))
                return false;
            if (!termID.HasValue)
                return ComplexLists.Exists(t => t.Name == termName);
            else
            {
                List<Term> complexLists = ComplexLists.FindAll(t => t.Name == termName);
                if (complexLists == null || complexLists.Count == 0)
                    return false;
                if (complexLists.Count >= 2)
                    return true;
                return !complexLists[0].ID.Equals(termID);
            }
        }

        //Note - This call could be done away with - just use the TermID for identification!
        public bool ComplexListTermExistsButNotAtIndex(string termName, int index)
        {
            if (string.IsNullOrEmpty(termName))
                return false;
            Predicate<Term> p = delegate(Term t) { return (t.Name == termName); };
            //The FindIndex search stops on the first match.  If there are more than one
            //match, and the search happened to stop on the current index, then the other match
            //would not be detected.  Therefore check for >= 2 matches first.
            List<Term> termMatches = _complexLists.FindAll(p);
            if (termMatches == null)
                return false;
            if (termMatches.Count >= 2)
                return true;

            int matchingTermIndex = _complexLists.FindIndex(p);
            if (matchingTermIndex == -1)
                return false;
            if (matchingTermIndex == index)
                return false;
            return true;
        }

        public Term FindComplexList(string termName)
        {
            if (string.IsNullOrEmpty(termName))
                return null;
            Predicate<Term> p = delegate(Term t) { return (t.Name == termName); };
            return ComplexLists.Find(p);
        }

        public Term FindTerm(string termName)
        {
            Term t = FindBasicTerm(termName);
            if (t != null)
                return t;
            return FindComplexList(termName);
        }

        public TermDependency FindTermDependency(Guid id)
        {
            Predicate<TermDependency> p = delegate(TermDependency td)
            {
                return (td.ID == id);
            };
            return TermDependencies.Find(p);
        }


        public bool TermDependencyExists(Guid id)
        {
            Predicate<TermDependency> p = delegate(TermDependency td)
            {
                return (td.ID == id);
            };
            return TermDependencies.Exists(p);
        }

        public bool DocumentNameExists(string ITATDocumentName)
        {
            if (string.IsNullOrEmpty(ITATDocumentName))
                return false;
            Predicate<ITATDocument> p = delegate(ITATDocument doc) { return (doc.DocumentName == ITATDocumentName); };
            return Documents.Exists(p);
        }

        #endregion Public Searches

        private void UpgradeSecurity()
        {
            if (_securityModel == SecurityModel.Advanced)
                return;
            if (TermGroups.Count != 1)
                throw new Exception(string.Format("When converting from Basic to Advanced security, encountered {0:D} termgroups where one was expected", TermGroups.Count));
            if (TermGroups[0].Type != TermGroup.TermGroupType.BasicSecurity)
                throw new Exception(string.Format("When converting from Basic to Advanced security, encountered term group type '{0}' where type '{1}' was expected", TermGroups[0].Type.ToString(), TermGroup.TermGroupType.BasicSecurity.ToString()));

            Guid oldBasicTermGroupID = TermGroups[0].ID;

            Guid newBasicTermGroupID = AddTermGroup(TermGroup._TERM_GROUP_BASIC_NAME, String.Empty, SecurityModel.Advanced, TermGroup.TermGroupType.AdvancedBasicTerm);
            foreach (Term term in _basicTerms)
            {
                term.TermGroupID = newBasicTermGroupID;
            }

            foreach (ComplexList complexList in _complexLists)
            {
                Guid newComplexListTermGroupID = AddTermGroup(complexList.Name, String.Empty, SecurityModel.Advanced, TermGroup.TermGroupType.AdvancedComplexList);
                complexList.TermGroupID = newComplexListTermGroupID;
            }

            DeleteTermGroup(oldBasicTermGroupID);

        }

        private void DownGradeSecurity()
        {
            if (_securityModel == SecurityModel.Basic)
                return;
            while (TermGroups.Count > 0)
                DeleteTermGroup(_termGroups[0].ID);

            Guid basicTermGroupID = AddTermGroup("", String.Empty, SecurityModel.Basic, TermGroup.TermGroupType.BasicSecurity);

            foreach (Term term in BasicTerms)
                term.TermGroupID = basicTermGroupID;

            foreach (Term term in ComplexLists)
                term.TermGroupID = basicTermGroupID;
        }

        public bool TermIsDependent(Term t)
        {
            foreach (TermDependency td in _termDependencies)
                foreach (Guid dependentTermID in td.DependentTermIDs)
                {
                    if (dependentTermID.Equals(t.ID))
                        return true;
                }
            return false;
        }

        public bool AssignTermDependency(Guid termDependencyID, TermDependency termDependency)
        {
            for (int nIndex = 0; nIndex < TermDependencies.Count; nIndex++)
            {
                if (TermDependencies[nIndex].ID.Equals(termDependencyID))
                {
                    TermDependencies[nIndex] = termDependency;
                    return true;
                }
            }
            return false;
        }

        public bool RemoveTermDependency(Guid termDependencyID)
        {
            for (int nIndex = 0; nIndex < TermDependencies.Count; nIndex++)
            {
                if (TermDependencies[nIndex].ID.Equals(termDependencyID))
                {
                    TermDependencies.RemoveAt(nIndex);
                    return true;
                }
            }
            return false;
        }

        public List<string> TermReferences(Template template, string termName)
        {
            List<string> rtn = new List<string>();
            if (!string.IsNullOrEmpty(termName))
            {
                Guid termID = template.FindTerm(termName).ID;
                rtn.AddRange(Business.ITATDocuments.TermReferences(template, termName));
                rtn.AddRange(Business.Events.TermReferences(template, termName, termID, Events));
                rtn.AddRange(Business.BasicTerms.TermReferences(template, termName, termID));
                rtn.AddRange(Business.TermDependency.TermReferences(template, termName, termID));
                rtn.AddRange(Business.Workflows.TermReferences(template, termName, termID));
                rtn.AddRange(Business.ComplexLists.TermReferences(template, termName, ComplexLists));
            }
            return rtn;
        }

        protected void LoadBasicTerms()
        {
            _basicTerms = Business.BasicTerms.Create(TemplateDef, this as ManagedItem, this as Template);   //note: if "this" is a Template and not a ManagedItem, "this as ManagedItem" equals null.
            Refresh();
            _basicTermsLoaded = true;
        }

        protected void LoadBasicTerms(XmlDocument xmlTemplateDoc)
        {
            _basicTerms = Business.BasicTerms.Create(xmlTemplateDoc, this as ManagedItem, this);   //note: if "this" is a Template and not a ManagedItem, "this as ManagedItem" equals null.
            _basicTermsLoaded = true;
        }

        protected void LoadComplexLists()
        {
            _complexLists = Business.ComplexLists.Create(TemplateDef, this);
            Refresh();
            _complexListsLoaded = true;
        }

        private void LoadTermDependencies()
        {
            _termDependencies = Business.TermDependency.Create(this, TemplateDef);
            _termDependenciesLoaded = true;
        }

        protected void LoadEvents(XmlDocument xmlTemplateDoc)
        {
            _events = Business.Events.Create(xmlTemplateDoc, this is ManagedItem);
            _eventsLoaded = true;
        }

        protected void LoadEvents(bool bIsManagedItem)
        {
            _events = Business.Events.Create(TemplateDef, bIsManagedItem);
            _eventsLoaded = true;
        }

        protected void LoadExtensions()
        {
            _extensions = Business.Extensions.Create(TemplateDef);
            _extensionsLoaded = true;
        }

        protected void CopyExtensions(List<Extension> extensions)
        {
            _extensions = new List<Extension>(extensions);
            _extensionsLoaded = true;
        }

        protected void LoadWorkflow()
        {
            //Need to load events before loading workflow, since the WorkflowRevertToDraft message
            //might be in the events collection (old way).
            if (!_eventsLoaded)
                LoadEvents(this is ManagedItem);
            _workflowLoaded = true;
            _workflows = Business.Workflows.Create(TemplateDef, this);
        }

        protected void LoadWorkflow(XmlDocument xmlTemplateDoc)
        {
            //Need to load events before loading workflow, since the WorkflowRevertToDraft message
            //might be in the events collection (old way).
            if (!_eventsLoaded)
                LoadEvents(this is ManagedItem);
            _workflowLoaded = true;
            _workflows = Business.Workflows.Create(xmlTemplateDoc, this);
        }


        protected void LoadDocuments()
        {
            _documents = Business.ITATDocuments.Create(TemplateDef, this);
            _documentsLoaded = true;
        }

        protected void LoadDocuments(XmlDocument xmlTemplateDoc)
        {
            _documents = Business.ITATDocuments.Create(xmlTemplateDoc, this);
            _documentsLoaded = true;
        }

        protected void LoadComments()
        {
            _comments = Business.Comments.Create(TemplateDef);
            _commentsLoaded = true;
        }

        protected void LoadDocumentPrinters()
        {
            _documentPrinters = Business.DocumentPrinters.Create(TemplateDef, this);
            _documentPrintersLoaded = true;
        }


        protected void LoadUserDocumentPrinters()
        {
            _userDocumentPrinters = Business.UserDocumentPrinters.Create(TemplateDef, this);
            _userDocumentPrintersLoaded = true;
        }

        protected void LoadTermGroups()
        {
            _termGroups = Business.TermGroups.Create(TemplateDef);
            _termGroupsLoaded = true;
            LoadTermGroupTerms();
        }

        private void LoadTermGroupTerms()
        {
            int basicTermsCount = 0;
            try
            {
                basicTermsCount = BasicTerms.Count;
            }
            catch
            {
            }
            int complexListsCount = 0;
            try
            {
                complexListsCount = ComplexLists.Count;
            }
            catch
            {
            }
            _termGroupTerms = new Dictionary<Guid, List<Term>>(basicTermsCount + complexListsCount);
            foreach (Term term in BasicTerms)
            {
                if (!_termGroupTerms.ContainsKey(term.TermGroupID))
                {
                    _termGroupTerms.Add(term.TermGroupID, new List<Term>());
                }
                _termGroupTerms[term.TermGroupID].Add(term);
            }
            foreach (ComplexList term in ComplexLists)
            {
                if (!_termGroupTerms.ContainsKey(term.TermGroupID))
                {
                    _termGroupTerms.Add(term.TermGroupID, new List<Term>());
                }
                _termGroupTerms[term.TermGroupID].Add(term);
            }
        }

        //This call will transfer any updates made to the terms contained within the TermGroupTerms
        //collection back to the BasicTerms, ComplexList terms.
        private void RefreshTermGroupTerms()
        {
            foreach (KeyValuePair<Guid, List<Term>> kvp in _termGroupTerms)
            {
                foreach (Term term in kvp.Value)
                {
                    if (term.TermType == TermType.ComplexList)
                    {
                        if (!ReplaceTerm(term, ComplexLists))
                        {
                            throw new Exception(string.Format("Failed to locate term {0} of type {1} within the BasicTerms or the ComplexLists collection", term.Name, term.TermType.ToString()));
                        }
                    }
                    else
                    {
                        if (!ReplaceTerm(term, BasicTerms))
                        {
                            throw new Exception(string.Format("Failed to locate term {0} of type {1} within the BasicTerms or the ComplexLists collection", term.Name, term.TermType.ToString()));
                        }
                    }
                }
            }
        }

        protected void LoadDetailedDescriptions()
        {
            _detailedDescriptions = Business.DetailedDescriptions.Create(TemplateDef);
            _detailedDescriptionsLoaded = true;
        }

        protected void LoadDocumentPrinters(XmlDocument xmlTemplateDoc)
        {
            _documentPrinters = Business.DocumentPrinters.Create(xmlTemplateDoc, this);
            _documentPrintersLoaded = true;
        }


        protected void LoadUserDocumentPrinters(XmlDocument xmlTemplateDoc)
        {
            _userDocumentPrinters = Business.UserDocumentPrinters.Create(xmlTemplateDoc, this);
            _userDocumentPrintersLoaded = true;
        }

        protected void LoadComments(XmlDocument xmlTemplateDoc)
        {
            _comments = Business.Comments.Create(xmlTemplateDoc);
            _commentsLoaded = true;
        }

        protected void LoadTermGroups(XmlDocument xmlTemplateDoc)
        {
            _termGroups = Business.TermGroups.Create(xmlTemplateDoc);
            _termGroupsLoaded = true;
            LoadTermGroupTerms();
        }

        //Refresh is meant to be called whenever there is a change in the makeup of
        //the Template object.  Functionality is added here as needed.
        public void Refresh()
        {
            _msoDefined = false;
            _renewalDefined = false;

            if (_basicTerms != null)
            {
                foreach (Term term in _basicTerms)
                {
                    switch (term.TermType)
                    {
                        case TermType.MSO:
                            _msoDefined = true;
                            break;
                        case TermType.Renewal:
                            _renewalDefined = true;
                            break;
                    }
                }
            }
        }

        public bool SetDetailedDescriptions(Dictionary<Business.DetailedDescription.DetailedDescriptionType, String> detailedDescriptions)
        {
            _detailedDescriptions = new List<DetailedDescription>();
            if (detailedDescriptions != null)
            {
                foreach (KeyValuePair<Business.DetailedDescription.DetailedDescriptionType, String> kvp in detailedDescriptions)
                {
                    _detailedDescriptions.Add(new DetailedDescription(kvp.Key, kvp.Value));
                }
            }

            return true;
        }

        //This call is intended to save all loaded portions of the template.
        //This is called from the Template Admin pages (for Template editing, or for ManagedItem EditLanguage).
        public virtual bool SaveLoaded(bool bValidate, bool bWriteToDatabase, string message)
        {
            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(TemplateDef);

            if (BasicTermsLoaded)
            {
                SaveBasicTerms(xmlTemplateDoc, bValidate, Retro.AuditType.None);	//Handles EventType.Renewal
            }
            if (ComplexListsLoaded)
            {
                SaveComplexLists(xmlTemplateDoc, bValidate, Retro.AuditType.None);
            }
            if (TermDependenciesLoaded)
            {
                SaveTermDependencies(xmlTemplateDoc, bValidate);
            }
            if (DocumentsLoaded)
            {
                SaveDocuments(xmlTemplateDoc, bValidate);
            }
            if (WorkflowLoaded)
            {
                SaveWorkflow(xmlTemplateDoc, bValidate);	//Handles EventType.WorkflowRevertToDraft
            }
            if (ExtensionsLoaded)
            {
                SaveExtensions(xmlTemplateDoc, bValidate);
            }
            if (EventsLoaded)
            {
                SaveEvents(xmlTemplateDoc, EventType.Renewal, bValidate);
                SaveEvents(xmlTemplateDoc, EventType.WorkflowRevertToDraft, bValidate);
                SaveEvents(xmlTemplateDoc, EventType.Custom, bValidate);
                SaveEvents(xmlTemplateDoc, EventType.Workflow, bValidate);
            }
            if (DetailedDescriptionsLoaded)
            {
                SaveDetailedDescriptions(xmlTemplateDoc, bValidate);
            }
            if (TermGroupsLoaded)
            {
                SaveTermGroups(xmlTemplateDoc, bValidate);
            }
            if (DocumentPrintersLoaded)
            {
                SaveDocumentPrinters(xmlTemplateDoc, bValidate);
            }

            if (UserDocumentPrintersLoaded)
            {
                SaveUserDocumentPrinters(xmlTemplateDoc, bValidate);
            }

            

            if (bWriteToDatabase)
                return Template.UpdateTemplateDef(this, xmlTemplateDoc.OuterXml);
            else
            {
                _templateDef = xmlTemplateDoc.OuterXml;
                return true;
            }
        }


        //Replaces the "terms" portion of the xml stored in the database with the version
        //currently stored in memory
        public virtual bool SaveBasicTerms(XmlDocument xmlTemplateDoc, bool bValidate, Retro.AuditType auditType)
        {
            if (bValidate)
            {
                bool bFoundFacilitySystemTerm = false;
                foreach (Term t in _basicTerms)
                {
                    if (t.SystemTerm && t.TermType == TermType.Facility)
                    {
                        //RULE: There can be at most one facility system term
                        if (bFoundFacilitySystemTerm)
                            return false;
                        else
                            bFoundFacilitySystemTerm = true;
                    }
                }
            }
            if (Business.BasicTerms.Save(xmlTemplateDoc, this, bValidate))
                return SaveEvents(xmlTemplateDoc, EventType.Renewal, bValidate);
            return false;
        }

        public virtual bool SaveComplexLists(XmlDocument xmlTemplateDoc, bool bValidate, Retro.AuditType auditType)
        {
            UpdateTermGroupNames();
            return Business.ComplexLists.Save(xmlTemplateDoc, this, bValidate);
        }

        public virtual bool SaveTermDependencies(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            return Business.TermDependency.Save(xmlTemplateDoc, this, bValidate);
        }

        public virtual bool SaveDocuments(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            return Business.ITATDocuments.Save(xmlTemplateDoc, this, bValidate);
        }

        public virtual bool SaveEvents(XmlDocument xmlTemplateDoc, EventType eventType, bool bValidate)
        {
            return Business.Events.Save(xmlTemplateDoc, this, eventType, bValidate);
        }

        public virtual bool SaveWorkflow(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            return Business.Workflows.Save(xmlTemplateDoc, this, bValidate);
        }

        public virtual bool SaveExtensions(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            return Business.Extensions.Save(xmlTemplateDoc, this, bValidate);
        }

        public virtual bool SaveDetailedDescriptions(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            return Business.DetailedDescriptions.Save(xmlTemplateDoc, this, bValidate);
        }

        public virtual bool SaveDocumentPrinters(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            return Business.DocumentPrinters.Save(xmlTemplateDoc, this, bValidate);
        }

        public virtual bool SaveUserDocumentPrinters(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            return Business.UserDocumentPrinters.Save(xmlTemplateDoc, this, bValidate);
        }

        public virtual bool SaveTermGroups(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            return Business.TermGroups.Save(xmlTemplateDoc, this, bValidate);
        }

        public bool UpdateTemplateSummary(string sXML)
        {
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
            //string sXML = GetTemplateDef(this, this.ID, DefType.Draft);
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(sXML);

            //Convert the objects stored in memory to an xmldocument
            XmlElement root = xmlTemplateDoc.DocumentElement;

            Utility.XMLHelper.SetAttributeString(root, XMLNames._A_Description, _description);
            Utility.XMLHelper.SetAttributeBool(root, XMLNames._A_CanGenerateDocument, _canGenerateDocument);
            if (!Utility.XMLHelper.SetAttributeBool(root, XMLNames._A_CanGenerateUserDocuments, _canGenerateUserDocuments))
                Utility.XMLHelper.AddAttributeBool(xmlTemplateDoc, root, XMLNames._A_CanGenerateUserDocuments, _canGenerateUserDocuments);
            Utility.XMLHelper.SetAttributeBool(root, XMLNames._A_Attachments, _allowAttachments);
            if (!Utility.XMLHelper.SetAttributeBool(root, XMLNames._A_UseDetailedDescription, _useDetailedDescription))
                Utility.XMLHelper.AddAttributeBool(xmlTemplateDoc, root, XMLNames._A_UseDetailedDescription, _useDetailedDescription);
            Utility.XMLHelper.SetAttributeBool(root, XMLNames._A_AllowComments, _allowComments);
            Utility.XMLHelper.SetAttributeString(root, XMLNames._A_XMLVersion, _XMLVersion);
            Utility.XMLHelper.SetAttributeString(root, XMLNames._A_Version, _version);
            if (!Utility.XMLHelper.SetAttributeString(root, XMLNames._A_RetroModel, _retroModel.ToString()))
                Utility.XMLHelper.AddAttributeString(xmlTemplateDoc, root, XMLNames._A_RetroModel, _retroModel.ToString());
            if (!Utility.XMLHelper.SetAttributeString(root, XMLNames._A_SecurityModel, _securityModel.ToString()))
                Utility.XMLHelper.AddAttributeString(xmlTemplateDoc, root, XMLNames._A_SecurityModel, _securityModel.ToString());

            //Store the entire xml back to the database
            using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
            {
                UpdateTemplateDef(this, xmlTemplateDoc.OuterXml);
                Data.Template.UpdateTemplateSummary(_id, _name, (short)_status, _retroDate);
                transScope.Complete();
            }

            DumpTemplateDef();
            return true;
        }

        public static int TemplateXMLVersionIndex(string sXMLVersion)
        {
            for (int nIndex = 0; nIndex < XMLNames._M_TemplateXMLVersions.Length; nIndex++)
            {
                if (XMLNames._M_TemplateXMLVersions[nIndex] == sXMLVersion)
                    return nIndex;
            }
            return -1;
        }

        public static string CurrentTemplateXMLVersion()
        {
            return XMLNames._M_TemplateXMLVersions[XMLNames._M_TemplateXMLVersions.Length - 1];
        }

        public static Guid AddTemplate(Guid ITATSystemID, string name, string description, bool generatesDocument, bool generateUserDocuments, short status, bool hasAttachments, bool allowComments, bool useDetailedDescription, List<string> validation)
        {
            Guid templateGuid = Guid.Empty;

            ITATSystem itatSystem = ITATSystem.Get(ITATSystemID, validation);
            if (validation != null)
                if (validation.Count > 0)
                    return templateGuid;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='UTF-8' ?><TemplateDef />");
            XmlElement elementTemplateDef = xmlDoc.DocumentElement;

            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTemplateDef, XMLNames._A_Description, description);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, elementTemplateDef, XMLNames._A_CanGenerateDocument, generatesDocument);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, elementTemplateDef, XMLNames._A_CanGenerateUserDocuments, generateUserDocuments);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, elementTemplateDef, XMLNames._A_Attachments, hasAttachments);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, elementTemplateDef, XMLNames._A_AllowComments, allowComments);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, elementTemplateDef, XMLNames._A_UseDetailedDescription, useDetailedDescription);
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTemplateDef, XMLNames._A_XMLVersion, CurrentTemplateXMLVersion());
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTemplateDef, XMLNames._A_Version, Guid.NewGuid().ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTemplateDef, XMLNames._A_SecurityModel, SecurityModel.Basic.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTemplateDef, XMLNames._A_RetroModel, Retro.RetroModel.Off.ToString());

            XmlElement elementEvents = xmlDoc.CreateElement(XMLNames._E_Events);
            elementTemplateDef.AppendChild(elementEvents);

            //Add the new TermGroups collection
            XmlElement elementTermGroups = xmlDoc.CreateElement(XMLNames._E_TermGroups);
            XmlElement elementTermGroup = xmlDoc.CreateElement(XMLNames._E_TermGroup);

            Guid basicTermGroupID = Guid.NewGuid();
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTermGroup, XMLNames._A_ID, basicTermGroupID.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTermGroup, XMLNames._A_Name, TermGroup._TERM_GROUP_BASIC_NAME);
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTermGroup, XMLNames._A_Description, "");
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementTermGroup, XMLNames._A_Type, TermGroup.TermGroupType.BasicSecurity.ToString());
            elementTermGroups.AppendChild(elementTermGroup);
            elementTemplateDef.AppendChild(elementTermGroups);

            XmlElement elementWorkFlows = xmlDoc.CreateElement(XMLNames._E_Workflows);
            XmlElement elementWorkFlow = xmlDoc.CreateElement(XMLNames._E_Workflow);
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementWorkFlow, XMLNames._A_Name, "New Workflow");
            string sActiveWorkflowID = Guid.NewGuid().ToString();
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementWorkFlow, XMLNames._A_ID, sActiveWorkflowID);

            XmlElement elementStates = xmlDoc.CreateElement(XMLNames._E_States);
            XmlElement elementState = xmlDoc.CreateElement(XMLNames._E_State);
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementState, XMLNames._A_Name, "BaseState");
            //TODO - Should we choose from the list of system Statuses here?
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementState, XMLNames._A_Status, "BaseState Status");
            Utility.XMLHelper.AddAttributeBool(xmlDoc, elementState, XMLNames._A_IsDraft, true);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, elementState, XMLNames._A_IsBase, true);
            Utility.XMLHelper.AddAttributeBool(xmlDoc, elementState, XMLNames._A_IsExit, true);

            //Add the StateTermGroup collection
            XmlElement elementStateTermGroups = xmlDoc.CreateElement(XMLNames._E_StateTermGroups);
            XmlElement elementStateTermGroup = xmlDoc.CreateElement(XMLNames._E_StateTermGroup);

            Utility.XMLHelper.AddAttributeString(xmlDoc, elementStateTermGroup, XMLNames._A_TermGroupID, basicTermGroupID.ToString());
            elementStateTermGroup.AppendChild(xmlDoc.CreateElement(XMLNames._E_Editors));
            elementStateTermGroup.AppendChild(xmlDoc.CreateElement(XMLNames._E_Viewers));
            elementStateTermGroup.AppendChild(xmlDoc.CreateElement(XMLNames._E_AttachmentRemovers));
            elementStateTermGroup.AppendChild(xmlDoc.CreateElement(XMLNames._E_ScannedAttachmentRemovers));

            elementStateTermGroups.AppendChild(elementStateTermGroup);
            elementState.AppendChild(elementStateTermGroups);

            elementStates.AppendChild(elementState);
            elementWorkFlow.AppendChild(elementStates);
            elementWorkFlows.AppendChild(elementWorkFlow);

            Utility.XMLHelper.AddAttributeString(xmlDoc, elementWorkFlows, XMLNames._A_ActiveWorkflowID, sActiveWorkflowID);
            elementTemplateDef.AppendChild(elementWorkFlows);

            XmlElement elementTerms = xmlDoc.CreateElement(XMLNames._E_Terms);
            elementTemplateDef.AppendChild(elementTerms);

            XmlElement elementDocuments = xmlDoc.CreateElement(XMLNames._E_Documents);
            elementTemplateDef.AppendChild(elementDocuments);


            XmlElement elementDocument = xmlDoc.CreateElement(XMLNames._E_Document);
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementDocument, XMLNames._A_ID, Guid.NewGuid().ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementDocument, XMLNames._A_Name, XMLNames._E_Document);
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementDocument, XMLNames._A_Document_DefaultDocument, bool.TrueString);
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementDocument, XMLNames._A_Document_WorkflowEnabled, bool.TrueString);

            XmlElement elementHeader = xmlDoc.CreateElement(XMLNames._E_Header);
            elementDocument.AppendChild(elementHeader);
            XmlElement elementBody = xmlDoc.CreateElement(XMLNames._E_Body);
            Utility.XMLHelper.AddAttributeString(xmlDoc, elementBody, XMLNames._A_Name, XMLNames._E_Document);
            elementDocument.AppendChild(elementBody);
            XmlElement elementFooter = xmlDoc.CreateElement(XMLNames._E_Footer);
            elementDocument.AppendChild(elementFooter);
            elementDocuments.AppendChild(elementDocument);


            XmlElement elementComments = xmlDoc.CreateElement(XMLNames._E_Comments);
            elementTemplateDef.AppendChild(elementComments);

            bool bValidate = false;
            foreach (Term term in itatSystem.Terms)
                term.TermGroupID = basicTermGroupID;

            Term newTerm = new PlaceHolderAttachments(false, null, false);
            newTerm.TermGroupID = basicTermGroupID;
            itatSystem.Terms.Add(newTerm);

            newTerm = new PlaceHolderComments(false, null, false);
            newTerm.TermGroupID = basicTermGroupID;
            itatSystem.Terms.Add(newTerm);

            Business.BasicTerms.SaveTerms(itatSystem.Terms, xmlDoc, elementTerms, bValidate);

            using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
            {
                templateGuid = Data.Template.InsertTemplate(ITATSystemID, name);
                Data.Template.UpdateTemplateSummary(templateGuid, name, status, null);
                Data.Template.UpdateDraftTemplateDef(templateGuid, xmlDoc.OuterXml);
                Data.Template.InsertTemplateAudit(templateGuid, Business.SecurityHelper.GetLoggedOnPersonId(), (short)status, xmlDoc.OuterXml, (int)Retro.AuditType.Created);
                transScope.Complete();
            }

            return templateGuid;
        }


        public static Guid CopyTemplate(ITATSystem itatSystem, Guid copyFrom, string newName, string newDescription)
        {
            string contentType;
            //new templates default to Status = Inactive
            Guid newTemplateId = Data.Template.CopyTemplate(copyFrom, newName, newDescription, (short)TemplateStatusType.Inactive);
            Template newTemplate = new Template(newTemplateId, DefType.Draft);

            //get XML for new Template
            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(newTemplate.TemplateDef);

            //Set the retro option to 'Off' initially...
            string rootNodeXPath = string.Format("/{0}", XMLNames._E_TemplateDef);
            XmlNode nodeParent = xmlTemplateDoc.SelectSingleNode(rootNodeXPath);
            ((XmlElement)nodeParent).SetAttribute(XMLNames._A_RetroModel, Retro.RetroModel.Off.ToString());

            //give each event an new ID (to avoid cross-referencing the original template's events)
            string eventNodeXPath = string.Format("/{0}/{1}/{2}", XMLNames._E_TemplateDef, XMLNames._E_Events, XMLNames._E_Event);
            XmlNodeList eventNodes = xmlTemplateDoc.SelectNodes(eventNodeXPath);
            foreach (XmlElement eventNode in eventNodes)
                eventNode.SetAttribute(XMLNames._A_ID, Guid.NewGuid().ToString());

            //give each extension an new ObjectID (to avoid cross-referencing the original template's extensions)
            string extensionNodeXPath = string.Format("/{0}/{1}/{2}", XMLNames._E_TemplateDef, XMLNames._E_Extensions, XMLNames._E_Extension);
            XmlNodeList extensionNodes = xmlTemplateDoc.SelectNodes(extensionNodeXPath);
            foreach (XmlElement extensionNode in extensionNodes)
            {
                //get document from DocumentStore
                string documentId = extensionNode.GetAttribute(XMLNames._A_ObjectID);
                string filename = extensionNode.GetAttribute(XMLNames._A_FileName);
                Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(itatSystem.DocumentStorageType);
                documentStorageObject.RootPath = itatSystem.DocumentStorageRootPath;
                byte[] document = documentStorageObject.GetDocument(itatSystem.ID, documentId, out contentType);
                //re-save the document to the DocumentStore and get a new DocumentId.   Note that extensions MUST be PDF documents
                extensionNode.SetAttribute(XMLNames._A_ObjectID, documentStorageObject.SaveDocument(filename, "pdf", document));
            }

            //re-save the XML back to the database with the updated values
            Template.UpdateTemplateDef(newTemplate, xmlTemplateDoc.OuterXml);
            return newTemplateId;
        }

        //This call will make copies of all the pertinent pieces of the template, minimizing the need to
        //extract data from the database or to perform additional parsing of the xml.
        //Only those pieces deemed to be needed for Retro are initially copied over, saving additional xml parsing.
        public void RetroCopy(Template sourceTemplate, bool parseEvents)
        {
            //Note - do not make these assignments here - they should already be defined.
            //_defType
            //_id

            //Summary Data
            _name = sourceTemplate.Name;
            _status = sourceTemplate.Status;
            _retroDate = sourceTemplate.RetroDate;
            _templateDef = sourceTemplate.TemplateDef;

            //Attributes
            _systemID = sourceTemplate.SystemID;
            _description = sourceTemplate.Description;
            _canGenerateDocument = sourceTemplate.CanGenerateDocument;
            _canGenerateUserDocuments = sourceTemplate.CanGenerateUserDocuments;
            _allowComments = sourceTemplate.AllowComments;
            _useDetailedDescription = sourceTemplate.UseDetailedDescription;
            _securityModel = sourceTemplate.SecurityModel;
            _retroModel = sourceTemplate.RetroModel;
            _version = sourceTemplate.Version;
            _XMLVersion = sourceTemplate.XMLVersion;
            _allowAttachments = sourceTemplate.AllowAttachments;

            //Collections
            _basicTerms = new List<Term>(sourceTemplate.BasicTerms.Count);
            foreach (Term term in sourceTemplate.BasicTerms)
                _basicTerms.Add(term.RetroCopy(term.SystemTerm, this));
            _complexLists = new List<Term>(sourceTemplate.ComplexLists);

            _termGroups = new List<TermGroup>(sourceTemplate.TermGroups);
            if (parseEvents)
            {
                XmlDocument xmlTemplateDoc = new XmlDocument();
                xmlTemplateDoc.PreserveWhitespace = false;
                xmlTemplateDoc.LoadXml(sourceTemplate.TemplateDef);
                _events = Business.Events.Create(xmlTemplateDoc, this is ManagedItem);
            }
            else
            {
                _events = new List<Event>(sourceTemplate.Events);
            }

            _workflows = new List<Workflow>(sourceTemplate.Workflows);

            //_workflows = new List<Workflow>(sourceTemplate.Workflows.Count);
            //foreach (Workflow workflow in sourceTemplate.Workflows)
            //    _workflows.Add(workflow.Copy(workflow.Name, sourceTemplate is ManagedItem, this));
            //Workflow sourceActiveworkflow = sourceTemplate.Workflows.Find(workflow => workflow.ID == sourceTemplate.ActiveWorkflowID);
            //_activeWorkflowID = Workflows.Find(workflow => workflow.Name == sourceActiveworkflow.Name).ID;

            //Note - Extensions not copied here, since the source may be a template and not the manageditem.
            //_extensions = new List<Extension>(sourceTemplate.Extensions);

            //_document = ITATDocument.Create(TemplateDef);
            //_comments = new List<Comment>(sourceTemplate.Comments);
            //_termDependencies = new List<TermDependency>(sourceTemplate.TermDependencies);
            //_detailedDescriptions = new List<DetailedDescription>(sourceTemplate.DetailedDescriptions);
            //_documentPrinters = new List<Role>(sourceTemplate.DocumentPrinters);

            //Derived
            //_sourceTerms = sourceTemplate.SourceTerms;
            _activeWorkflowID = sourceTemplate.ActiveWorkflowID;
            //_termGroupTerms = sourceTemplate.TermGroupTerms;

            //Loaded indicators
            _summaryLoaded = sourceTemplate._summaryLoaded;
            _basicTermsLoaded = sourceTemplate._basicTermsLoaded;
            _complexListsLoaded = sourceTemplate._complexListsLoaded;
            _termGroupsLoaded = sourceTemplate._termGroupsLoaded;
            _eventsLoaded = sourceTemplate._eventsLoaded;
            _workflowLoaded = sourceTemplate._workflowLoaded;

            //_msoDefined = sourceTemplate._msoDefined;
            //_renewalDefined = sourceTemplate._renewalDefined;
            //_extensionsLoaded = sourceTemplate._extensionsLoaded;
            //_commentsLoaded = sourceTemplate._commentsLoaded;
            //_documentPrintersLoaded = sourceTemplate._documentPrintersLoaded;
            //_detailedDescriptionsLoaded = sourceTemplate._detailedDescriptionsLoaded;
            //_termDependenciesLoaded = sourceTemplate._termDependenciesLoaded;
            //_documentLoaded = sourceTemplate._documentLoaded;
        }

        public static DataSet GetTemplateList(Guid ITATSystemID, List<string> userRoles)
        {
            return Data.Template.GetTemplateList(ITATSystemID, userRoles);
        }

        public static DataTable GetActiveTemplates(Guid ITATSystemID)
        {
            return Data.Template.GetActiveTemplates(ITATSystemID);
        }

        public static DataSet GetTemplateListWithStatus(Guid ITATSystemID, short[] arrayTemplateStatus, List<string> userRoles)
        {
            List<short> listTemplateStatus = new List<short>(arrayTemplateStatus.Length);
            foreach (short templateStatus in arrayTemplateStatus)
                listTemplateStatus.Add(templateStatus);
            return GetTemplateListWithStatus(ITATSystemID, listTemplateStatus, userRoles);
        }

        public static DataSet GetTemplateListWithStatus(Guid ITATSystemID, List<short> templateStatuses, List<string> userRoles)
        {
            return Data.Template.GetTemplateListWithStatus(ITATSystemID, GetTemplateStatusListXML(templateStatuses), userRoles);
        }

        public static string GetTemplateStatusListXML(List<short> statuses)
        {
            if (statuses == null)
                return null;
            if (statuses.Count == 0)
                return null;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<?xml version='1.0' encoding='UTF-8' ?><Statuses />");
            XmlElement elementRoot = xmlDoc.DocumentElement;
            foreach (short status in statuses)
            {
                XmlNode nodeStatus = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Status, XMLNames._M_NameSpaceURI);
                Utility.XMLHelper.AddAttributeInt(xmlDoc, nodeStatus, XMLNames._A_Code, status);
                elementRoot.AppendChild(nodeStatus);
            }
            return xmlDoc.OuterXml;
        }

        public static bool PromoteTemplate(Guid TemplateID, TemplateStatusType status, Retro.AuditType auditType)
        {
            bool promoted = true;
            using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
            {
                string sDraftTemplateXML = GetTemplateDef(null, TemplateID, DefType.Draft);
                string sDraftTemplateXML_NoVersion = null;
                if (!string.IsNullOrEmpty(sDraftTemplateXML))
                    sDraftTemplateXML_NoVersion = sDraftTemplateXML.Replace(GetVersion(sDraftTemplateXML), "");

                string sFinalTemplateXML = GetTemplateDef(null, TemplateID, DefType.Final);
                string sFinalTemplateXML_NoVersion = null;
                if (!string.IsNullOrEmpty(sFinalTemplateXML))
                    sFinalTemplateXML_NoVersion = sFinalTemplateXML.Replace(GetVersion(sFinalTemplateXML), "");

                bool attemptPromotion = true;
                if (!string.IsNullOrEmpty(sDraftTemplateXML_NoVersion) && !string.IsNullOrEmpty(sFinalTemplateXML_NoVersion))
                    attemptPromotion = !sDraftTemplateXML_NoVersion.Equals(sFinalTemplateXML_NoVersion);

                if (attemptPromotion)
                {
                    Data.Template.UpdateFinalTemplateDef(TemplateID, sDraftTemplateXML);
                    Data.Template.InsertTemplateAudit(TemplateID, Business.SecurityHelper.GetLoggedOnPersonId(), (short)status, sDraftTemplateXML, (int)auditType);
                }
                else
                    promoted = false;

                transScope.Complete();
            }

            return promoted;
        }

        public static void DemoteTemplate(Guid TemplateID, TemplateStatusType status)
        {

            using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
            {
                string sFinalTemplateXML = GetTemplateDef(null, TemplateID, DefType.Final);
                Data.Template.UpdateDraftTemplateDef(TemplateID, sFinalTemplateXML);
                Data.Template.InsertTemplateAudit(TemplateID, Business.SecurityHelper.GetLoggedOnPersonId(), (short)status, sFinalTemplateXML, (int)Retro.AuditType.Saved);
                transScope.Complete();
            }


        }

        //This call could be located somewhere else.  This is not specifically
        //related to Template - this is mostly used by the GUI layer.
        public static string GetSessionParameter(Guid sessionParameterID)
        {
            try
            {
                return Data.Common.GetSessionParameter(sessionParameterID).Tables[0].Rows[0]["Value"] as string;
            }
            catch 
            { 
                return null; 
            }
        }

        //This call could be located somewhere else.  This is not specifically
        //related to Template - this is mostly used by the GUI layer.
        public static Guid InsertSessionParameter(string Value, int purgeKeepDays)
        {
            return Data.Common.InsertSessionParameter(Value, purgeKeepDays);
        }

        public static bool FinalVersionExists(Guid TemplateID)
        {
            string templateDef = GetTemplateDef(null, TemplateID, DefType.Final);
            return templateDef != null;
        }

        //replace instances of the <img> tag that indicates a term placeholder with the value of the term
        public virtual void SubstituteTerms()
        {
            foreach (ITATDocument document in Documents)
            {
                foreach (ITATClause clause in document.Clause.Children)
                {
                    //if (string.IsNullOrEmpty(clause.DependsOnTermName) && (FindBasicTerm(clause.DependsOnTermName).DisplayValue("") != clause.DependsOnValue))
                    //   Document.Clause.Children.Remove(clause);
                    //else
                    clause.SubstituteTerms(this);
                }

            }

        }


        public List<TermDependency> FilterDependency(DependencyTarget target)
        {
            List<TermDependency> termdependencies = new List<TermDependency>();
            foreach (TermDependency termDependency in TermDependencies)
            {
                if (termDependency.Target == target)
                    termdependencies.Add(termDependency);
            }
            return termdependencies;
        }

        public Event FindEvent(Guid eventID)
        {
            Event workflowRevertEvent = WorkflowRevertEvent;
            if (workflowRevertEvent != null)
                if (workflowRevertEvent.ID == eventID)
                    return workflowRevertEvent;
            Predicate<Event> p = delegate(Event e) { return (e.ID == eventID); };
            return _events.Find(p);
        }

        public Event FindEvent(string eventName)
        {
            Predicate<Event> p = delegate(Event e) { return (e.Name == eventName); };
            return _events.Find(p);
        }


        //Changed for Multiple Documents
        public ITATDocument GetITATDocument(Guid ITATDocumentID)
        {
            Predicate<ITATDocument> p = delegate(ITATDocument a) { return (a.ITATDocumentID == ITATDocumentID); };
            return Documents.Find(p);
        }

        public ITATDocument GetDefaultITATDocument()
        {
            Predicate<ITATDocument> p = delegate(ITATDocument a) { return (a.DefaultDocument == true); };
            return Documents.Find(p);

        }


        public bool isNewDocumentStructure()
        {
            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(TemplateDef);


            XmlNode newRootNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Documents));
            if (newRootNode == null)
                return false;

            return true;

        }

        #endregion



        public static string NewTemplateName(Guid itatSystemId, string oldName)
        {
            string baseNewName = string.Concat("Copy of ", oldName);
            string newName = baseNewName;
            //TODO: if there already exists a template called newName, append (2) or (3) or ... to the end to make it unique
            DataRowCollection rows = Business.Template.GetTemplateList(itatSystemId, null).Tables[0].Rows;
            if (rows.Count > 0)
            {
                List<string> templateNames = new List<string>(rows.Count);
                for (int i = 0; i < rows.Count; i++)
                    templateNames.Add((string)rows[i][Data.DataNames._C_TemplateName]);
                int suffixIndex = 2;
                while (templateNames.Contains(newName))
                    newName = string.Format("{0} ({1})", baseNewName, suffixIndex++);
            }
            return newName;
        }

        public void Rollback(Guid templateID, Guid templateAuditID)
        {
            using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
            {
                DataTable dtTemplateAudit = Data.Template.GetTemplateAudit(templateAuditID);
                _templateDef = dtTemplateAudit.Rows[0][Data.DataNames._C_TemplateDef].ToString();
                Data.Template.UpdateDraftTemplateDef(templateID, _templateDef);
                PromoteTemplate(templateID, (TemplateStatusType)Enum.Parse(typeof(TemplateStatusType), dtTemplateAudit.Rows[0][Data.DataNames._C_Status].ToString()), Retro.AuditType.RollBack);
            }
        }

        public List<Retro.AuditType> GetRetroAuditTypes()
        {
            DataTable dtAuditTypes = Data.Template.GetTemplateRetroDetails(ID);
            List<Retro.AuditType> auditTypes = new List<Retro.AuditType>(dtAuditTypes.Rows.Count);
            foreach (DataRow drAuditType in dtAuditTypes.Rows)
            {
                auditTypes.Add((Retro.AuditType)Enum.Parse(typeof(Retro.AuditType), drAuditType[Data.DataNames._C_AuditTypeName].ToString()));
            }
            return auditTypes;
        }

        public DataTable GetTemplateRetroDetails()
        {
            return Data.Template.GetTemplateRetroDetails(ID);
        }

        public DataTable GetTemplateAudit(Guid templateAuditID)
        {
            return Data.Template.GetTemplateAudit(templateAuditID);
        }

        public void UnloadDocuments()
        {
            _documentsLoaded = false;
        }

	}
}
