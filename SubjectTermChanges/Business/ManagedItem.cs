using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Transactions;
using Kindred.Common.Logging;
using System.IO;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public partial class ManagedItem : Template
	{
		#region private members

		private Guid _managedItemID;
		private string _itemNumber;
		private State _state;
		private bool _attachmentsLoaded;
        private bool _isOrphaned;
        private List<Attachment> _attachments;
        private bool _isEditLanguage;
		private bool _processScheduledEventsLoaded;
		private List<ProcessScheduledEvent> _processScheduledEvents;

		#endregion


		#region  Properties

		// Overrode Template.ID to throw an exception, forcing developers to reference either 
		// TemplateID or ManagedItemID properties, to avoid confusion.
		[Obsolete("Do not reference ManagedItem.ID.  Use TemplateID or ManagedItemID properties instead.", true)]
		public new Guid ID
		{
			get {return base.ID; }
			set {base.ID = value; }
		}

		public Guid TemplateID
		{
			get { return base.ID; }
			set { base.ID = value; }
		}

		public Guid ManagedItemID
		{
			get { return _managedItemID; }
			set { _managedItemID = value; }
		}

		public string ItemNumber
		{
			get { return _itemNumber; }
			set { _itemNumber = value; }
		}

		public bool IsEditLanguage
		{
			get { return _isEditLanguage; }
			set { _isEditLanguage = value; }
		}

        public State State
		{
			get { return _state; }
			set
			{
				if (_state != null)
					if (value.ID != _state.ID)
					{
						//If going from BaseState to any other state (except ExitState), need to set DateExitedBaseState
						if ((_state.IsBase ?? false) && !(value.IsBase ?? false))
						{
							Workflow.DateExitedBaseState = DateTime.Today;
						}
						//If going to BaseState or ExitState, need to clear DateExitedBaseState
						if ((value.IsBase ?? false) || (value.IsExit ?? false))
						{
							Workflow.DateExitedBaseState = null;
						}
					}
				_state = value;
			}
		}

		public List<int> OwningFacilityIDs
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
							return facilityTerm.OwningFacilityIDs;
						}
					}
				}
				return null;
			}
		}

		public List<Attachment> Attachments
		{
			get
			{
				if (!_attachmentsLoaded)
					LoadAttachments();
				return _attachments;
			}
            set{	_attachments = value;}
		}


        public bool IsOrphaned
        {
            get { return _isOrphaned; }
            set { _isOrphaned = value; }
        }

        //NOTE - These are from the ScheduledEvent table
		public List<ProcessScheduledEvent> ProcessScheduledEvents
		{
			get
			{
				if (!_processScheduledEventsLoaded)
					LoadProcessScheduledEvents();
				return _processScheduledEvents;
			}
		}


         #endregion


		#region static "Constructors"

		public ManagedItem(Guid templateID, Guid managedItemID)
			: base(templateID, DefType.ManagedItem)
		{
			_managedItemID = managedItemID;
		}


        public ManagedItem(Guid templateID, Guid managedItemID, string managedItemNumber, bool isOrphaned, Guid stateID, string sTemplateDef)
            : base(templateID, DefType.ManagedItem)
        {
            _managedItemID = managedItemID;
            _templateDef = sTemplateDef;

            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(_templateDef);

            LoadWorkflow(xmlTemplateDoc);
            LoadBasicTerms(xmlTemplateDoc);
            LoadComplexLists(xmlTemplateDoc);

            LoadComments(xmlTemplateDoc);
            LoadAttachments();
            LoadTermGroups(xmlTemplateDoc);
            LoadDocuments(xmlTemplateDoc);
            State = Workflow.FindState(stateID);
            ItemNumber = managedItemNumber;
            IsOrphaned = isOrphaned;
        }

        public static ManagedItem Get(Guid managedItemId, bool loadComplexLists, string state, Guid stateID, string status, string templateDef, string itemNumber, Guid templateID, bool orphaned)
		{
			ManagedItem managedItem = new ManagedItem(templateID, managedItemId);
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

			if (loadComplexLists)
			{
				//Need Workflow in order to access the states
				//Need BasicTerms in order to be able to modify the Primary FacilityID's
				managedItem.LoadWorkflow(xmlTemplateDoc);
				managedItem.LoadBasicTerms(xmlTemplateDoc);
				managedItem.LoadComplexLists(xmlTemplateDoc);
			}
			else
			{
				managedItem.LoadWithoutComplexLists(xmlTemplateDoc);
			}
			managedItem.LoadComments(xmlTemplateDoc);
			managedItem.LoadAttachments();
            if (!managedItem.TermGroupsLoaded)
                managedItem.LoadTermGroups(xmlTemplateDoc);
            managedItem.LoadDocuments(xmlTemplateDoc);
            if (stateID.Equals(Guid.Empty))
			    managedItem.State = managedItem.Workflow.FindState(state);
            else
                managedItem.State = managedItem.Workflow.FindState(stateID);
            managedItem.ItemNumber = itemNumber;
            managedItem.IsOrphaned = orphaned;
            
			return managedItem;
		}


		/// <summary>
		/// Retrieves a ManagedItem instance from the database.
		/// </summary>
		/// <param name="id"></param>
		public static ManagedItem Get(Guid managedItemID, bool loadComplexLists)
		{
			using (DataSet ds = Data.ManagedItem.GetManagedItem(managedItemID))
			{
				DataRow row = ds.Tables[0].Rows[0];
				string state = (string)row[Data.DataNames._C_State];
                Guid stateID = Guid.Empty;
                try
                {
                    stateID = (Guid)row[Data.DataNames._C_StateID];
                }
                catch
                {
                }
				string status = (string)row[Data.DataNames._C_Status];
				string templateDef = (string)row[Data.DataNames._C_TemplateDef];
				Guid templateID = new Guid(row[Data.DataNames._C_TemplateID].ToString());
				string itemNumber = row[Data.DataNames._C_ManagedItemNumber].ToString();
                bool isOrphaned = bool.Parse(row[Data.DataNames._C_IsOrphaned].ToString());
                return Get(managedItemID, loadComplexLists, state, stateID, status, templateDef, itemNumber, templateID, isOrphaned);
			}
		}

        /// <summary>
        /// Retrieves ManagedItem data from the database.
        /// </summary>
        /// <param name="id"></param>
        public static DataTable GetRawManagedItem(Guid managedItemID)
        {
            return Data.ManagedItem.GetManagedItem(managedItemID).Tables[0];
        }

		/// <summary>
		/// Retrieves a ManagedItem instance from the database.  Only retrieves the minimum data.
		/// </summary>
		/// <param name="id"></param>
		public static ManagedItem GetBasic(Guid managedItemID)
		{
			string status = "";
			string templateDef = "";
			string itemNumber = "";
			string templateID = "";
            Guid stateID = Guid.Empty;
			using (DataSet ds = Data.ManagedItem.GetManagedItem(managedItemID))
			{
				DataRow row = ds.Tables[0].Rows[0];
				status = (string)row[Data.DataNames._C_Status];
                templateDef = (string)row[Data.DataNames._C_TemplateDef];
				templateID = row[Data.DataNames._C_TemplateID].ToString();
				itemNumber = row[Data.DataNames._C_ManagedItemNumber].ToString();
                try
                {
                    stateID = (Guid)row[Data.DataNames._C_StateID];
                }
                catch
                {
                    throw new Exception(string.Format("StateID not defined for ManagedItem {0}", itemNumber));
                }
			}

			ManagedItem managedItem = new ManagedItem(new Guid(templateID), managedItemID);

            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(templateDef);

            managedItem.LoadWorkflow(xmlTemplateDoc);
            managedItem.LoadBasicTerms(xmlTemplateDoc);

            //populate external terms
            foreach (Term term in managedItem.FindAllBasicTerms(TermType.External))
            {
                ExternalTerm extTerm = term as ExternalTerm;
                if (extTerm == null)
                    throw new Exception(string.Format("The term '{0}' is not an external term.", term.Name));
                if (!extTerm.ValuesLoaded)
                    extTerm.LoadValues();
            }

            managedItem.State = managedItem.Workflow.FindState(stateID);
            managedItem.ItemNumber = itemNumber;
            return managedItem;
		}

		/// <summary>
		/// Creates a new instance of a ManagedItem using the specified template where the system uses the OwningFacility concept
		/// </summary>
		/// <param name="template">The template that this ManagedItem is based on</param>
		/// <param name="facilityIds">One or more facilities that this ManagedItem is associated with</param>
		public static ManagedItem Create(bool updateDatabase, Guid templateID, int? primaryFacilityID)
		{
			ManagedItem managedItem = new ManagedItem(templateID, Guid.NewGuid());
			managedItem.LoadWorkflow();

			//Load BasicTerms since we will need to update the TemplateDef based on the selected PrimaryID's
			managedItem.LoadBasicTerms();

			managedItem.SetDefaultValues();
			managedItem.LoadAttachments();
			managedItem.State = managedItem.Workflow.FindBaseState();

			if (primaryFacilityID.HasValue)
			{
				List<int> facilityIDs = new List<int>(1);
				facilityIDs.Add(primaryFacilityID.Value);
				managedItem.PrimaryFacility.SelectedFacilityIDs = new List<int>(facilityIDs);
				managedItem.PrimaryFacility.OwningFacilityIDs = new List<int>(facilityIDs);
			}

			ITATSystem system = ITATSystem.Get(managedItem.SystemID);
			string sFirstPart = GetFirstPart(system, managedItem);
			string sPrefix = string.Format("{0}{1}", sFirstPart, system.ManagedItemNumberSystemId);
			if (sPrefix.Length > 13)
				throw new Exception("The ManagedItem Number length will exceed 20 characters");
            int nSequenceNumber = 0;
            if (updateDatabase)
                nSequenceNumber = Data.ManagedItem.GetSequenceNumber(managedItem.SystemID, sPrefix);
            else
                nSequenceNumber = 1;
			if (nSequenceNumber <= 0)
			{
				if (primaryFacilityID.HasValue)
					throw new Exception(string.Format("SequenceNumber not found for SystemID {0}, FacilityID {1}", managedItem.SystemID.ToString(), primaryFacilityID.Value.ToString()));
				else
					throw new Exception(string.Format("SequenceNumber not found for SystemID {0}", managedItem.SystemID.ToString()));
			}

			managedItem.ItemNumber = string.Format("{0}{1}", sPrefix, nSequenceNumber.ToString("D7"));
			return managedItem;
		}

        public static void RemoveGeneratedDocument(ManagedItem sourceManagedItem, string documentID, Utility.DocumentStorageType documentStorageType)
        {
            ILog log = LogManager.GetLogger(sourceManagedItem.GetType());
            try
            {
                Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(documentStorageType);
                documentStorageObject.DeleteDocument(documentID);
                log.Debug(string.Format("Deleted Generated document {0} from ManagedItem {1} during CreateRetro call", documentID, sourceManagedItem.ItemNumber));
            }
            catch
            {
                //Create a record in the Document table - for later clean up
                Data.Document.DeleteDocument(sourceManagedItem.ManagedItemID, "Generated Document", "Generated Document", documentID, "Generated Document", false, false);
                log.Error(string.Format("Failed to fully delete Generated document {0} from ManagedItem {1} during CreateRetro call", documentID, sourceManagedItem.ItemNumber));
            }
        }

        /// <summary>
        /// Creates a new instance of a ManagedItem using the specified template where the system uses the OwningFacility concept
        /// </summary>
        /// <param name="template">The template that this ManagedItem is based on</param>
        /// <param name="facilityIds">One or more facilities that this ManagedItem is associated with</param>
        public static ManagedItem CreateRetro(Template template, bool useAlternateTemplate, Guid sourceManagedItemID, bool hasOwningFacility, Utility.DocumentStorageType documentStorageType, out bool returnToDraft)
        {
            ManagedItem sourceManagedItem = Business.ManagedItem.Get(sourceManagedItemID, true);
            ManagedItem newManagedItem = new ManagedItem(template.ID, sourceManagedItem.ManagedItemID);
            newManagedItem.RetroCopy(template, useAlternateTemplate);
            returnToDraft = newManagedItem.Migrate(sourceManagedItem);
            if (returnToDraft)
            {
                //Changed for Multiple Documents
                foreach (ITATDocument doc in sourceManagedItem.Documents)
                {
                    if (!string.IsNullOrEmpty(doc.GeneratedDocumentID))
                    {
                        RemoveGeneratedDocument(sourceManagedItem, doc.GeneratedDocumentID, documentStorageType);
                        newManagedItem.GetITATDocument(doc.ITATDocumentID).GeneratedDocumentID = null;
                    }
                }
            }
            return newManagedItem;
        }

        private void SetDefaultValues()
		{
			foreach (Term t in this.BasicTerms)
				t.SetDefaultValue();
		}

		#endregion


		#region Private Methods

		//TODO:  Allow for non-numeric hard-coded value (including empty string)
		private static string GetFirstPart(ITATSystem system, ManagedItem managedItem)
		{
			//If ManagedItemNumberSystemType is an empty string, then return an empty string
			if (string.IsNullOrEmpty(system.ManagedItemNumberSystemType))
				return string.Empty;

			//If ManagedItemNumberSystemType is a special value meaning SAP Alias, return the Owning Facility's SAP Alias
			if (system.ManagedItemNumberSystemType == XMLNames._SYSTEM_MINST_Fac_SAP4)
			{
				if (managedItem.PrimaryFacility.OwningFacilityIDs == null)
					throw new Exception(string.Format("Tried to create a sequence number for a ManagedItem which does not have any OwningFacilites defined, ManagedItem {0}", managedItem._managedItemID.ToString()));
				if (managedItem.PrimaryFacility.OwningFacilityIDs.Count == 0)
					throw new Exception(string.Format("Tried to create a sequence number for a ManagedItem which does not have any OwningFacilites defined, ManagedItem {0}", managedItem._managedItemID.ToString()));

				string sSAPID = "";
				using (DataSet ds = Data.ManagedItem.GetFacilityInfo(FacilityTerm.GetXml(managedItem.PrimaryFacility.OwningFacilityIDs)))
				{
					if (ds.Tables.Count > 0)
						if (ds.Tables[0].Rows.Count > 0)
							//This approach assumes that we are only interested in the first facilityID
							sSAPID = ds.Tables[0].Rows[0][Data.DataNames._SP_FacilitySAPID].ToString();
					if (string.IsNullOrEmpty(sSAPID))
						throw new Exception(string.Format("SAPID not found for managedItem {0}", managedItem._managedItemID.ToString()));
				}
				return sSAPID.Substring(0, 4);
			}

			//Otherwise, return the hard-coded ManagedItemNumberSystemType 
			return system.ManagedItemNumberSystemType;
		}

  		//Note - The caller is expected to set TransactionScope for this call
		private void UpdateLoaded(bool bValidate, string sXml, Retro.AuditType auditType)
		{
			string sTerm1 = null;
			string sTerm2 = null;
			DateTime? dtTerm3 = null;
            string sTerm4 = null;
            string sTerm5 = null;
            DateTime? dtTerm6 = null;
            DateTime? dtTerm7 = null;

			//Save values of External Terms to the ExternalTermValue table
			foreach (Term t in BasicTerms)
				if (t.TermType == TermType.External)
				{
					ExternalTerm extTerm = t as ExternalTerm;
					if (!extTerm.ValuesLoaded)
						extTerm.LoadValues();
					extTerm.SaveValues(true);
				}

            GetTerms(ref sTerm1, ref sTerm2, ref dtTerm3, ref sTerm4, ref sTerm5, ref dtTerm6, ref dtTerm7);
			string sKeyWords = GetKeyWords();

            if (!_isOrphaned)
                _isOrphaned = auditType == Retro.AuditType.Orphaned;

            if (PrimaryFacility == null)
                Data.ManagedItem.UpdateManagedItem(ManagedItemID, ItemNumber, State.Status, State.Name, State.ID, sTerm1, sTerm2, dtTerm3, sTerm4, sTerm5, dtTerm6, dtTerm7, sKeyWords, FacilityTerm.GetXml(null), FacilityTerm.GetXml(null), _isOrphaned);
			else
                Data.ManagedItem.UpdateManagedItem(ManagedItemID, ItemNumber, State.Status, State.Name, State.ID, sTerm1, sTerm2, dtTerm3, sTerm4, sTerm5, dtTerm6, dtTerm7, sKeyWords, FacilityTerm.GetXml(PrimaryFacility.SelectedFacilityIDs), FacilityTerm.GetXml(OwningFacilityIDs), _isOrphaned);

			Business.BasicTerms.Save(this, ref sXml, bValidate);

            //Update the Scheduled Events, but only if this method is being called from the web app (not from the scheduled job)
            if (System.Web.HttpContext.Current != null)
                UpdateScheduledEvents(ScheduledEvent.ExecutedCalculationType.UsePreviousValue);
			//Update all the other portions of the xml, if they have been loaded (assumes they were modified)...
            if (ComplexListsLoaded)
            {
                UpdateTermGroupNames();
                Business.ComplexLists.Save(this, ref sXml, bValidate);
            }
			if (WorkflowLoaded)
				Business.Workflows.Save(this, ref sXml, bValidate);

            if (TermDependenciesLoaded)
                Business.TermDependency.Save(this, ref sXml, bValidate);
			if (EventsLoaded)
				Business.Events.Save(this, ref sXml, bValidate);
			if (ExtensionsLoaded)
				Business.Extensions.Save(this, ref sXml, bValidate);
			if (CommentsLoaded)
				Business.Comments.Save(this, ref sXml, bValidate);
            if (TermGroupsLoaded)
                Business.TermGroups.Save(this, ref sXml, bValidate);
            if (DocumentsLoaded)
                Business.ITATDocuments.Save(this, ref sXml, bValidate);
            //Save the attachments, but only if this method is being called from the web app (not from the scheduled job)
            if (System.Web.HttpContext.Current != null)
            {
                SaveAttachments();
            }

            Business.Template.UpdateTemplateDef(this, sXml);
            Guid personId = Business.SecurityHelper.GetLoggedOnPersonId();

            byte[] templateDefZipped = Utility.CompressionHelper.Compress(sXml);
            Data.ManagedItem.InsertManagedItemAudit(ManagedItemID, personId, DateTime.Now, State.Name, State.ID, State.Status, templateDefZipped, (int)auditType);

            Data.ManagedItem.UpdateManagedItemStateRole(ManagedItemID, Utility.ListHelper.EliminateDuplicates(State.AccessorRoleNames));
        }

		#endregion


		#region Public Methods

		//This is called when the Managedtem is first saved to the database - this is where the new row is created.
		public void FirstSave(bool updateDatabase, bool bValidate)
		{
			string sTerm1 = null;
			string sTerm2 = null;
			DateTime? dtTerm3 = null;
            string sTerm4 = null;
            string sTerm5 = null;
            DateTime? dtTerm6 = null;
            DateTime? dtTerm7 = null;

            GetTerms(ref sTerm1, ref sTerm2, ref dtTerm3, ref sTerm4, ref sTerm5, ref dtTerm6, ref dtTerm7);
			//Note - if the PrimaryFacility.SelectedFacilityIDs collection is null then the ManagedItemFacility table will not receive any entries.
			//Note - if the OwningFacilityIDs collection is null then none of the PrimaryFacility.SelectedFacilityIDs will be marked as 'Owning'.

			using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
			{
                if (updateDatabase)
                {
                    if (PrimaryFacility == null)
                        Data.ManagedItem.InsertManagedItem(ManagedItemID, TemplateID, ItemNumber, State.Status, State.Name, State.ID, sTerm1, sTerm2, dtTerm3, sTerm4, sTerm5, dtTerm6, dtTerm7, FacilityTerm.GetXml(null), FacilityTerm.GetXml(OwningFacilityIDs), false);
                    else
                        Data.ManagedItem.InsertManagedItem(ManagedItemID, TemplateID, ItemNumber, State.Status, State.Name, State.ID, sTerm1, sTerm2, dtTerm3, sTerm4, sTerm5, dtTerm6, dtTerm7, FacilityTerm.GetXml(PrimaryFacility.SelectedFacilityIDs), FacilityTerm.GetXml(OwningFacilityIDs), false);
                }

				//Update the just inserted TemplateDef with the Primary FacilityID's
				string sTemplateDef = TemplateDef;
				Business.BasicTerms.Save(this, ref sTemplateDef, bValidate);
                Business.ComplexLists.Save(this, ref sTemplateDef, bValidate);
                if (updateDatabase)
                    Business.Template.UpdateTemplateDef(this, sTemplateDef);
                byte[] templateDefZipped = Utility.CompressionHelper.Compress(sTemplateDef);
                if (updateDatabase)
                {
                    Data.ManagedItem.InsertManagedItemAudit(ManagedItemID, Business.SecurityHelper.GetLoggedOnPersonId(), DateTime.Now, State.Name, State.ID, State.Status, templateDefZipped, (int)Retro.AuditType.Created);
                    Data.ManagedItem.UpdateManagedItemStateRole(ManagedItemID, Utility.ListHelper.EliminateDuplicates(State.AccessorRoleNames));
                }
                transScope.Complete();
			}
		}


		public string[] SaveAttachments()
		{
			//Delete all documents from DocumentCache table for this managed item
			//Delete all documents from Document table for this managed item
			//Add references to the attachments in Document table
			List<string> log = new List<string>();
            using (System.Transactions.TransactionScope ts = new TransactionScope(Data.Common.TransactionScopeOption))
            {
                log.Add(string.Format("For ManagedItem {0} now deleting all attachments", _managedItemID.ToString()));
                Data.Document.DeleteCachedDocumentsByManagedItem(_managedItemID);
                Data.Document.DeleteDocumentsByManagedItem(_managedItemID);
                if (Attachments.Count > 0)
                {
                    foreach (Attachment att in _attachments)
                    {
                        Data.Document.InsertDocument(_managedItemID, att.Name, att.Description, att.DocumentStoreID, att.DocumentType.Name, att.IsScanned);
                        //Note - Added more logging here
                        log.Add(string.Format("For ManagedItem {0} added attachment '{1}', DocumentStoreID = '{2}', IsScanned = {3}", _managedItemID.ToString(), att.Name, att.DocumentStoreID, att.IsScanned.ToString()));
                    }
                }
                ts.Complete();
            }
            return log.ToArray();
		}


        public void UpdateGeneratedDocument(string documentId, Guid ITATDocumentID)
		{
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.LoadXml(this.TemplateDef);
            ITATDocuments.UpdateGeneratedDocument(xmlTemplateDoc, documentId, ITATDocumentID);
            Business.Template.UpdateTemplateDef(this, xmlTemplateDoc.OuterXml);
        }

        private Retro.AuditType GetAuditTypeAsOrphaned(Retro.AuditType defaultAuditType)
        {
            if (!_isOrphaned && RetroModel == Retro.RetroModel.OnWithEditLanguage)
            {
                _isOrphaned = true;
                return Retro.AuditType.Orphaned;
            }
            return defaultAuditType;
        }

        //TODO: This code is in the process of being migrated.  Here is the skinny:
        //  1.  'SaveLoaded' is called from the Template Admin screens.  It is needed here when handling 'Edit Language' scenarios.
        //      When called from the Template Admin screens, no 'AuditType' is supplied.
        //  2.  'UpdateLoaded' is the workhorse.  It is the only code that actually writes to the database.
        public override bool SaveLoaded(bool bValidate, bool bWriteToDatabase, string message)
        {
            Retro.AuditType auditType = GetAuditTypeAsOrphaned(Retro.AuditType.Saved);
            Update(bValidate, auditType);
            if (auditType == Retro.AuditType.Orphaned && !string.IsNullOrEmpty(message))
            {
                ILog log = LogManager.GetLogger(this.GetType());
                log.Debug(string.Format("ManagedItem {0} ({1}) (System {2}) Orphaned due to changes at '{3}'", ItemNumber, ManagedItemID.ToString(), SystemID.ToString(), string.IsNullOrEmpty(message) ? "NOT PROVIDED" : message));
            }
            return true;
        }

        public void Update(bool bValidate, Retro.AuditType auditType, bool transactionFurnished)
		{
            if (transactionFurnished)
            {
                UpdateLoaded(bValidate, TemplateDef, auditType);
            }
            else
            {
                using (TransactionScope transScope = new TransactionScope(Data.Common.TransactionScopeOption))
                {
                    UpdateLoaded(bValidate, TemplateDef, auditType);
                    transScope.Complete();
                }
            }
        }

        public void Update(bool bValidate, Retro.AuditType auditType)
        {
            Update(bValidate, auditType, false);
        }

        //These three 'Save Overrides' are needed because the corresponding Template Admin screens do not call SaveLoaded, which is also overridden.
        public override bool SaveDocuments(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            if (Business.ITATDocuments.Save(xmlTemplateDoc, this, bValidate))
            {
                Retro.AuditType auditType = GetAuditTypeAsOrphaned(Retro.AuditType.Saved);
                byte[] templateDefZipped = Utility.CompressionHelper.Compress(TemplateDef);
                Data.ManagedItem.InsertManagedItemAudit(ManagedItemID, Business.SecurityHelper.GetLoggedOnPersonId(), DateTime.Now, State.Name, State.ID, State.Status, templateDefZipped, (int)auditType);
                if (auditType == Retro.AuditType.Orphaned)
                {
                    ILog log = LogManager.GetLogger(this.GetType());
                    log.Debug(string.Format("ManagedItem {0} ({1}) (System {2}) Orphaned due to changes at 'Document'", ItemNumber, ManagedItemID.ToString(), SystemID.ToString()));
                }
                return true;
            }
            else
                return false;
        }

        public override bool SaveEvents(XmlDocument xmlTemplateDoc, EventType eventType, bool bValidate)
        {
            if (Business.Events.Save(xmlTemplateDoc, this, eventType, bValidate))
            {
                Retro.AuditType auditType = GetAuditTypeAsOrphaned(Retro.AuditType.Saved);
                byte[] templateDefZipped = Utility.CompressionHelper.Compress(TemplateDef);
                Data.ManagedItem.InsertManagedItemAudit(ManagedItemID, Business.SecurityHelper.GetLoggedOnPersonId(), DateTime.Now, State.Name, State.ID, State.Status, templateDefZipped, (int)auditType);
                if (auditType == Retro.AuditType.Orphaned)
                {
                    ILog log = LogManager.GetLogger(this.GetType());
                    log.Debug(string.Format("ManagedItem {0} ({1}) (System {2}) Orphaned due to changes at 'Events'", ItemNumber, ManagedItemID.ToString(), SystemID.ToString()));
                }
                return true;
            }
            else
                return false;
        }

        //TODO: Need to verify that editing Extensions could orphan a MI
        public override bool SaveExtensions(XmlDocument xmlTemplateDoc, bool bValidate)
        {
            if (Business.Extensions.Save(xmlTemplateDoc, this, bValidate))
            {
                Retro.AuditType auditType = GetAuditTypeAsOrphaned(Retro.AuditType.Saved);
                byte[] templateDefZipped = Utility.CompressionHelper.Compress(TemplateDef);
                Data.ManagedItem.InsertManagedItemAudit(ManagedItemID, Business.SecurityHelper.GetLoggedOnPersonId(), DateTime.Now, State.Name, State.ID, State.Status, templateDefZipped, (int)auditType);
                if (auditType == Retro.AuditType.Orphaned)
                {
                    ILog log = LogManager.GetLogger(this.GetType());
                    log.Debug(string.Format("ManagedItem {0} ({1}) (System {2}) Orphaned due to changes at 'Extensions'", ItemNumber, ManagedItemID.ToString(), SystemID.ToString()));
                }
                return true;
            }
            else
                return false;
        }

		private bool GetTerms(ref string sTerm1, ref string sTerm2, ref DateTime? dtTerm3, ref string sTerm4, ref string sTerm5, ref DateTime? dtTerm6, ref DateTime? dtTerm7)
		{
			if (BasicTerms != null)
			{
				foreach (Term term in BasicTerms)
				{
					if (term.SystemTerm)
					{
						if (term.UseDBField ?? false)
						{
							switch (term.DBFieldName)
							{
								case Data.DataNames._C_Term1:
									if (sTerm1 != null)
										throw new Exception("Duplicate Term1 encountered in GetTerms");
									sTerm1 = term.Keyword;
									break;

								case Data.DataNames._C_Term2:
									if (sTerm2 != null)
										throw new Exception("Duplicate Term2 encountered in GetTerms");
									sTerm2 = term.Keyword;
									break;

								case Data.DataNames._C_Term3:
									if (dtTerm3 != null)
										throw new Exception("Duplicate Term3 encountered in GetTerms");
									switch (term.TermType)
									{
										case TermType.Date:
											Business.DateTerm dateterm = term as Business.DateTerm;
											if (dateterm.Value.HasValue)
												dtTerm3 = dateterm.Value.Value;
											break;
										case TermType.Renewal:
											Business.RenewalTerm renewalterm = term as Business.RenewalTerm;
											switch (renewalterm.DisplayedDate)
											{
												case DisplayedDate.EffectiveDate:
													{
														if (renewalterm.EffectiveDate.HasValue)
															dtTerm3 = renewalterm.EffectiveDate.Value;
														break;
													}
												case DisplayedDate.ExpirationDate:
												default:
													{
														if (renewalterm.ExpirationDate.HasValue)
															dtTerm3 = renewalterm.ExpirationDate.Value;
														break;
													}
											}
											break;
										default:
											throw new Exception(string.Format("Term3 must be assigned to a Date or Renewal term.  It is currently assigned to a term of type {0}.", term.TermType.ToString()));
									}
									break;

                                case Data.DataNames._C_Term4:
                                    if (sTerm4 != null)
                                        throw new Exception("Duplicate Term4 encountered in GetTerms");
                                    sTerm4 = term.Keyword;
                                    break;

                                case Data.DataNames._C_Term5:
                                    if (sTerm5 != null)
                                        throw new Exception("Duplicate Term5 encountered in GetTerms");
                                    sTerm5 = term.Keyword;
                                    break;

                                case Data.DataNames._C_Term6:
                                    if (dtTerm6 != null)
                                        throw new Exception("Duplicate Term6 encountered in GetTerms");
                                    switch (term.TermType)
                                    {
                                        case TermType.Date:
                                            Business.DateTerm dateterm = term as Business.DateTerm;
                                            if (dateterm.Value.HasValue)
                                                dtTerm6 = dateterm.Value.Value;
                                            break;
                                        case TermType.Renewal:
                                            Business.RenewalTerm renewalterm = term as Business.RenewalTerm;
                                            switch (renewalterm.DisplayedDate)
                                            {
                                                case DisplayedDate.EffectiveDate:
                                                    {
                                                        if (renewalterm.EffectiveDate.HasValue)
                                                            dtTerm6 = renewalterm.EffectiveDate.Value;
                                                        break;
                                                    }
                                                case DisplayedDate.ExpirationDate:
                                                default:
                                                    {
                                                        if (renewalterm.ExpirationDate.HasValue)
                                                            dtTerm6 = renewalterm.ExpirationDate.Value;
                                                        break;
                                                    }
                                            }
                                            break;
                                        default:
                                            throw new Exception(string.Format("Term6 must be assigned to a Date or Renewal term.  It is currently assigned to a term of type {0}.", term.TermType.ToString()));
                                    }
                                    break;

                                case Data.DataNames._C_Term7:
                                    if (dtTerm7 != null)
                                        throw new Exception("Duplicate Term7 encountered in GetTerms");
                                    switch (term.TermType)
                                    {
                                        case TermType.Date:
                                            Business.DateTerm dateterm = term as Business.DateTerm;
                                            if (dateterm.Value.HasValue)
                                                dtTerm7 = dateterm.Value.Value;
                                            break;
                                        case TermType.Renewal:
                                            Business.RenewalTerm renewalterm = term as Business.RenewalTerm;
                                            switch (renewalterm.DisplayedDate)
                                            {
                                                case DisplayedDate.EffectiveDate:
                                                    {
                                                        if (renewalterm.EffectiveDate.HasValue)
                                                            dtTerm7 = renewalterm.EffectiveDate.Value;
                                                        break;
                                                    }
                                                case DisplayedDate.ExpirationDate:
                                                default:
                                                    {
                                                        if (renewalterm.ExpirationDate.HasValue)
                                                            dtTerm7 = renewalterm.ExpirationDate.Value;
                                                        break;
                                                    }
                                            }
                                            break;
                                        default:
                                            throw new Exception(string.Format("Term7 must be assigned to a Date or Renewal term.  It is currently assigned to a term of type {0}.", term.TermType.ToString()));
                                    }
                                    break;

								default:
									throw new Exception(string.Format("Non-defined DBFieldName '{0}' encountered", term.DBFieldName));
							}
						}
					}
				}
			}
			if (!string.IsNullOrEmpty(sTerm1))
				if (sTerm1.Length > Data.DataNames._FL_Term1)
					throw new Exception(string.Format("The length of Term1 ({0}) exceeds the allowed length of {1}", sTerm1.Length.ToString(), Data.DataNames._FL_Term1.ToString()));
			if (!string.IsNullOrEmpty(sTerm2))
				if (sTerm2.Length > Data.DataNames._FL_Term2)
					throw new Exception(string.Format("The length of Term2 ({0}) exceeds the allowed length of {1}", sTerm2.Length.ToString(), Data.DataNames._FL_Term2.ToString()));
            if (!string.IsNullOrEmpty(sTerm4))
                if (sTerm4.Length > Data.DataNames._FL_Term4)
                    throw new Exception(string.Format("The length of Term4 ({0}) exceeds the allowed length of {1}", sTerm4.Length.ToString(), Data.DataNames._FL_Term4.ToString()));
            if (!string.IsNullOrEmpty(sTerm5))
                if (sTerm5.Length > Data.DataNames._FL_Term5)
                    throw new Exception(string.Format("The length of Term5 ({0}) exceeds the allowed length of {1}", sTerm5.Length.ToString(), Data.DataNames._FL_Term5.ToString()));

			return true;
		}


		private string GetKeyWords()
		{
			string sKeyWords = "";
			foreach (Term term in BasicTerms)
			{
				if (term.KeywordSearchable ?? false)
					if (term.Keyword != null)
						sKeyWords = string.Concat(sKeyWords, XMLNames._M_Delimiter, term.Keyword);
			}

			foreach (Term term in ComplexLists)
			{
				if (term.KeywordSearchable ?? false)
					if (term.Keyword != null)
						sKeyWords = string.Concat(sKeyWords, XMLNames._M_Delimiter, term.Keyword);
			}

			return sKeyWords.Trim(XMLNames._M_Delimiter);
		}

		private void LoadAttachments()
		{
			if (_attachments == null)
				_attachments = new List<Attachment>();
			DataSet dsAttachments = Data.Document.GetDocumentList(_managedItemID, false);
			foreach (DataRow row in dsAttachments.Tables[0].Rows)
			{
                DocumentType documentType = new DocumentType((string)row[Data.DataNames._C_DocumentType]);
                _attachments.Add(new Attachment((Guid)row[Data.DataNames._C_DocumentID], 
                    (string)row[Data.DataNames._C_DocumentName],
                    (string)row[Data.DataNames._C_DocumentDescription],
                    (string)row[Data.DataNames._C_DocumentStoreID], 
                    documentType,
                    (bool)row[Data.DataNames._C_IsScanned]));
			}
			_attachmentsLoaded = true;
		}

		private void LoadProcessScheduledEvents()
		{
			DataTable dtProcessScheduledEvents = Data.ManagedItem.GetScheduledEvents(this.ManagedItemID);
			if (_processScheduledEvents == null)
				_processScheduledEvents = new List<ProcessScheduledEvent>();
			foreach (DataRow row in dtProcessScheduledEvents.Rows)
			{
                _processScheduledEvents.Add(new ProcessScheduledEvent((Guid)row[Data.DataNames._C_ScheduledEventID], (Guid)row[Data.DataNames._C_EventID], (DateTime)row[Data.DataNames._C_ScheduledEventDate], (bool)row[Data.DataNames._C_Executed])); 
			}
			_processScheduledEventsLoaded = true;
		}


        public TermGroup GetDefaultTermGroup(List<string> userRoles, bool isAdmin)
        {
            foreach (TermGroup termGroup in TermGroups)
            {
                if (CanAccessTermGroup(termGroup, userRoles))
                    return termGroup;
            }
            if (isAdmin)
                return TermGroups[0];
            else
                return (TermGroup)null;
        }

        public bool CanAccessTermGroup(TermGroup termGroup, List<string> userRoles)
        {
			return CanAccessTermGroup(termGroup.ID, userRoles);
        }

        public List<Guid> AccessibleTermGroupIDs(List<string> userRoles)
        {
            List<Guid> termGroupIDs = new List<Guid>();
            foreach (TermGroup termGroup in TermGroups)
            {
                if (CanAccessTermGroup(termGroup, userRoles) || userRoles == null)
                    termGroupIDs.Add(termGroup.ID);
            }
            return termGroupIDs;
        }

		public bool CanAccessTermGroup(Guid termGroupId, List<string> userRoles)
		{
            if (userRoles == null)
                return false;
			StateTermGroup stateTermGroup = State.GetTermGroup(termGroupId);
			if (stateTermGroup == null)
			{
				string tgName = termGroupId.ToString();
				TermGroup tg = FindTermGroup(termGroupId);
				if (tg != null)
				    tgName = tg.Name;
				throw new Exception(string.Format("stateTermGroup is null.  State={0}.  TermGroup={1}.", State.Name, tgName));
			}
			if (Utility.ListHelper.HaveAMatch(stateTermGroup.Editors.ConvertAll<string>(Role.StringConverter), userRoles))
			{
				return true;
			}
			if (Utility.ListHelper.HaveAMatch(stateTermGroup.Viewers.ConvertAll<string>(Role.StringConverter), userRoles))
			{
				return true;
			}
			return false;
		}




        public void ApplyTermDependencies(bool? canEditProfile, Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup)
		{

			if (canEditProfile.HasValue)
            {
                foreach (Term term in BasicTerms)
                {
                    term.Runtime.Reset(canEditProfile.Value && (term.Editable ?? false), (term.Required ?? false));
                }
                foreach (ComplexList complexList in ComplexLists)
                {
                    complexList.Runtime.Reset(canEditProfile.Value && (complexList.Editable ?? false), (complexList.Required ?? false));
                }
                foreach (TermDependency td in this.TermDependencies)
                {
                    switch (td.Target)
                    {
                        case DependencyTarget.Term:
                            if (td.IsActive == true)
                            {
                                foreach (Guid dependentTermID in td.DependentTermIDs)
                                {
                                    Term dependentTerm = FindTerm(dependentTermID);
                                    if (dependentTerm != null)
                                        if (td.ShouldApplyAction(this))
                                        {
                                            dependentTerm.ApplyDependencyAction(canEditProfile.Value, td.Action);
                                        }
                                }
                            }
                            break;

                        case DependencyTarget.Workflow:
                            if (td.ShouldApplyAction(this))
                                if (td.Action.SetValue != ActiveWorkflowID.ToString())
                                {
                                    ActiveWorkflowID = new Guid(td.Action.SetValue);
                                    State = Workflow.FindBaseState();
                                }
                            break;
                    }
                }
            }
            else
            {
                foreach (Term term in BasicTerms)
                {
                    term.Runtime.Reset(canEditTermGroup[term.TermGroupID] && (term.Editable ?? false), (term.Required ?? false));
                }
                foreach (ComplexList complexList in ComplexLists)
                {
					complexList.Runtime.Reset(canEditTermGroup[complexList.TermGroupID] && (complexList.Editable ?? false), (complexList.Required ?? false));
                }
                foreach (TermDependency td in this.TermDependencies)
                {
                    switch (td.Target)
                    {
                        case DependencyTarget.Term:
                            if (td.IsActive == true)
                            {
                                foreach (Guid dependentTermID in td.DependentTermIDs)
                                {
                                    Term dependentTerm = FindTerm(dependentTermID);
                                    if (dependentTerm != null)
                                        if (td.ShouldApplyAction(this))
                                        {
                                            dependentTerm.ApplyDependencyAction(canEditTermGroup[dependentTerm.TermGroupID], td.Action);
                                        }
                                }
                            }
                            break;

                        case DependencyTarget.Workflow:
                            if (td.ShouldApplyAction(this))
                                if (td.Action.SetValue != ActiveWorkflowID.ToString())
                                {
                                    ActiveWorkflowID = new Guid(td.Action.SetValue);
                                    State = Workflow.FindBaseState();
                                }
                            break;
                    }
                }
            }
		}


		//replace instances of the <img> tag that indicates a term placeholder with the value of the term
		public override void SubstituteTerms()
		{
            foreach (ITATDocument document in Documents)
            {
                foreach (ITATClause clause in document.Clause.Children)
                    clause.SubstituteTerms(this);

            }
		}

		public static DataSet GetAllNonExecutedEvents(DateTime eventDate)
		{
			return Data.ManagedItem.GetAllNonExecutedEvents(eventDate);
		}

 		#endregion

        #region Retro

        private bool Migrate(ManagedItem sourceManagedItem)
        {
            _itemNumber = sourceManagedItem._itemNumber;
            _isOrphaned = sourceManagedItem._isOrphaned;
            //Changed for Multiple Documents
             this.Documents = sourceManagedItem.Documents;
            CopyExtensions(sourceManagedItem.Extensions);
            _attachments = new List<Attachment>(sourceManagedItem.Attachments);

            foreach (Term term in BasicTerms) 
            {
                term.MigrateReset();
                Term sourceTerm = sourceManagedItem.FindTerm(term.ID, term.Name);
                if (sourceTerm != null && !(sourceTerm is ComplexList) && term.TermType == sourceTerm.TermType)
                    term.Migrate(sourceTerm);
                else
                {
                    term.Migrate(null);
                }
            }

            foreach (Term term in sourceManagedItem.BasicTerms)
            {
                if (!term.Runtime.Migrated)
                    term.Delete();
            }

            for (int index = 0;index < ComplexLists.Count;index++)
            {
                ComplexList term = (ComplexList)ComplexLists[index];
                Term sourceTerm = sourceManagedItem.FindTerm(term.ID, term.Name);
                if (sourceTerm != null && (sourceTerm is ComplexList))
                {
                    if (ComplexList.ModifyItems(sourceTerm as ComplexList, term))
                        term = (ComplexList)term.RetroCopy(false, this);
                    term.Migrate(sourceTerm);
                    ComplexLists[index] = term;
                }
                else
                    term.Migrate(null);
            }

            foreach (Term term in sourceManagedItem.ComplexLists)
            {
                if (!term.Runtime.Migrated)
                    term.Delete();
            }

            int count = Comments.Count; //Cause the setting of _commentsLoaded
            Comments = new List<Comment>(sourceManagedItem.Comments);

            Event.Migrate(sourceManagedItem.Events, Events);
            Event.Migrate(sourceManagedItem.Workflow.Events, Workflow.Events);

            //The ActiveWorkflow is determined by the source template (that was used to create 'this').
            //The ActiveWorkflow will not change.
            //If the sourceManagedItem happens to have the same ActiveWorkflow as the source template, and a match
            //can be made on the state, then keep the state.  Otherwise, set the state to the base state.

            State currentState = null;
            if (ActiveWorkflowID.Equals(sourceManagedItem.ActiveWorkflowID) || Workflow.Name.Equals(sourceManagedItem.Workflow.Name))
            {
                currentState = Workflow.FindState(sourceManagedItem.State.ID);
                if (currentState == null)
                    currentState = Workflow.FindState(sourceManagedItem.State.Name);
            }

            State = currentState == null ? Workflow.FindBaseState() : currentState;
            return currentState == null;
        }

        private Retro.RetroData GetRetroData(string managedItemNumber, Guid managedItemID, Guid managedItemAuditId)
        {
			Retro.RetroData rtn = new Retro.RetroData();
            DataTable tabManagedItemAudit = Data.ManagedItem.GetManagedItemAudit(managedItemAuditId);
            rtn.AuditDate = (DateTime)tabManagedItemAudit.Rows[0][Data.DataNames._C_DateOfChange];
            rtn.TemplateRetroDate = null;
            try
            {
                if (tabManagedItemAudit.Rows[0][Data.DataNames._C_TemplateDefZipped] == DBNull.Value)
                {
                    rtn.TemplateDef = tabManagedItemAudit.Rows[0][Data.DataNames._C_TemplateDef].ToString();
                }
                else
                {
                    byte[] templateDefZipped = (byte[])tabManagedItemAudit.Rows[0][Data.DataNames._C_TemplateDefZipped];
                    rtn.TemplateDef = Utility.CompressionHelper.Decompress(templateDefZipped);
                }
                if (string.IsNullOrEmpty(rtn.TemplateDef))
                    throw new Exception();
            }
            catch
            {
                throw new Exception(string.Format("TemplateDef not found for manageditem {0}, managedItemID {1}, managedItemAuditId{2}", managedItemNumber, managedItemID.ToString(), managedItemAuditId.ToString()));
            }

            rtn.StateID = (Guid)tabManagedItemAudit.Rows[0][Data.DataNames._C_StateID];

            DataTable dtTemplateAuditTypes = Data.Template.GetTemplateRetroDetails(TemplateID);
            try
            {
                rtn.AuditType = (Retro.AuditType)Enum.Parse(typeof(Retro.AuditType), dtTemplateAuditTypes.Rows[0][Data.DataNames._C_AuditTypeName].ToString());
            }
            catch
            {
                rtn.AuditType = Retro.AuditType.None;
            }

            try
            {
                rtn.TemplateRetroDate = (DateTime)dtTemplateAuditTypes.Rows[0][Data.DataNames._C_RetroDate];
            }
            catch
            {
            }

			return rtn;
        }


        public bool CouldBeOrphaned(Guid managedItemAuditId)
		{
			if (managedItemAuditId == Guid.Empty)
				return false;
            if (_isOrphaned)
                return false;
            if (RetroModel != Retro.RetroModel.OnWithEditLanguage)
                return false;
            Retro.RetroData retroData = GetRetroData(_itemNumber, _managedItemID, managedItemAuditId);
            return retroData.TemplateRetroDate.HasValue && retroData.AuditDate < retroData.TemplateRetroDate.Value;
        }

        public void Rollback(Guid managedItemAuditId, string sEnvironment, bool? orphanTheItem)
        {
			bool retroRollback;
            Retro.RetroData retroData = GetRetroData(_itemNumber, _managedItemID, managedItemAuditId);
			if (retroData.TemplateRetroDate.HasValue && retroData.AuditDate < retroData.TemplateRetroDate.Value)
            {
                //May require retro
                switch (RetroModel)
                {
                    case Retro.RetroModel.Off:
                        retroRollback = false;
                        break;

                    case Retro.RetroModel.OnWithEditLanguage:
                        if (_isOrphaned)
                        {
                            retroRollback = false;
                        }
                        else
                        {
                            if (!orphanTheItem.HasValue)
                                throw new Exception(string.Format("The 'orphan' value was required but not supplied for the rollback of ManagedItem '{0}'", ManagedItemID.ToString()));
                            retroRollback = !(orphanTheItem.Value);
                            _isOrphaned = orphanTheItem.Value;
                            if (_isOrphaned)
                            {
								retroData.AuditType = Retro.AuditType.Orphaned;
                                ILog log = LogManager.GetLogger(this.GetType());
                                log.Debug(string.Format("ManagedItem {0} ({1}) (System {2}) Orphaned due to Rollback", ItemNumber, ManagedItemID.ToString(), SystemID.ToString()));
                            }
                        }
                        break;

                    case Retro.RetroModel.OnWithoutEditLanguage:
                        retroRollback = true;
                        break;

                    default:
                        throw new Exception(string.Format("RetroModel case '{0}' not handled for ManagedItem '{1}'", RetroModel.ToString(),ManagedItemID.ToString()));
                }
            }
            else
                retroRollback = false;

            ITATSystem itatSystem = null;
            Business.ManagedItem newManagedItem = null;
            if (retroRollback)
            {
                Template currentTemplate = new Template(TemplateID, Business.DefType.Final);
                itatSystem = ITATSystem.Get(SystemID);
                bool returnToDraft;
                newManagedItem = Business.ManagedItem.CreateRetro(currentTemplate, false, ManagedItemID, itatSystem.HasOwningFacility.Value, itatSystem.DocumentStorageType, out returnToDraft);
                newManagedItem.Update(false, retroData.AuditType);    //Update also calls UpdateManagedItemStateRole
                if (returnToDraft)
                {
                    //No action required here as of last spec update
                }
                newManagedItem.UpdateScheduledEvents(ScheduledEvent.ExecutedCalculationType.UseGracePeriod);
            }
            else
            {
                newManagedItem = new ManagedItem(TemplateID, ManagedItemID, ItemNumber, IsOrphaned, retroData.StateID, retroData.TemplateDef);
                //Ensure that the database is updated with the older External term values
                foreach (Term t in newManagedItem.BasicTerms)
                    if (t.TermType == TermType.External)
                    {
                        (t as ExternalTerm).SaveValues(false);
                    }
				newManagedItem.Update(false, retroData.AuditType);    //Update also calls UpdateManagedItemStateRole
            }
            
           
            //Changed for Multiple Documents
            foreach (ITATDocument doc in Documents)
            {
                if (!string.IsNullOrEmpty(doc.GeneratedDocumentID) && newManagedItem.GetITATDocument(doc.ITATDocumentID).GeneratedDocumentID != doc.GeneratedDocumentID)
                {
                    if (itatSystem == null)
                        itatSystem = ITATSystem.Get(SystemID);
                    //Changed for Multiple Documents
                    RemoveGeneratedDocument(newManagedItem, doc.GeneratedDocumentID, itatSystem.DocumentStorageType);
                }

            }



        }

        #endregion Retro

        #region Load

        public static DataTable QueryExcelFile(string fileName, string tableName, out string error)
        {
            return Data.Excel.QueryFile(fileName, tableName, out error);
        }

        public static DataTable QueryFile(string[] fileRows, int rowMain, int rowSecondary, char delimiter, out string error)
        {
            return Data.Excel.QueryFile(fileRows, rowMain, rowSecondary, delimiter, out error);
        }

        public static void DeleteManagedItem(Guid managedItemID)
        {
            Data.ManagedItem.DeleteManagedItem(managedItemID);
        }

        public static DataTable GetManagedItemByNumber(string managedItemNumber)
        {
            return Data.ManagedItem.GetManagedItemByNumber(managedItemNumber);
        }

        public static DataTable GetManagedItemDocuments(Guid managedItemID)
        {
            return Data.ManagedItem.GetManagedItemDocuments(managedItemID);
        }

        public bool UpdateManagedItemTemplate(Guid templateID)
        {
            return Data.ManagedItem.UpdateManagedItemTemplate(ManagedItemID, templateID);
        }

        #endregion Load

        #region Scheduled Events

		public bool UpdateScheduledEvents(ScheduledEvent.ExecutedCalculationType executedCalculationType)
        {
            DateTime cutoffDate = DateTime.Today;
            if (executedCalculationType == ScheduledEvent.ExecutedCalculationType.UseGracePeriod)
            {
                ITATSystem system = ITATSystem.Get(this.SystemID);
                cutoffDate = cutoffDate.AddDays(-1 * system.RetroEventGracePeriodDays);
            }

            //Create a collection of all the Events from all of the event object sources 
            //within this ManagedItem.  From that collection, create a collection of objects
            //that store the newly calculated event dates for each of the ScheduledEvents.
            //Once the new collection is created, pass the information to the database in xml
            //form and update the ScheduledEvent table.  In that table, copy all of the
            //executed events for this ManagedItem to the ScheduledEventArchive table, delete
            //all of the events for this ManagedItem, and re-add only the non-executed events to the
            //ScheduledEvent table.

            List<ProcessScheduledEvent> procScheduledEvents = new List<ProcessScheduledEvent>();
            List<Event> tempEvents = new List<Event>(Events.Count);

            //Create a temporary collection of Events that contains all of the events from whatever sources there are.
            foreach (Business.Event eachEvent in Events)
                if (eachEvent.EventType != EventType.WorkflowRevertToDraft)
                    tempEvents.Add(eachEvent);
            if (WorkflowRevertEvent != null && (Workflow.UseFunction ?? false))
                tempEvents.Add(WorkflowRevertEvent);

            foreach (Business.Event eachEvent in tempEvents)
            {
                switch (eachEvent.EventType)
                {
                    case EventType.Custom:
                        procScheduledEvents.AddRange(CustomScheduledEvents(eachEvent, executedCalculationType, cutoffDate));
                        break;
                    case EventType.Renewal:
						procScheduledEvents.AddRange(RenewalScheduledEvents(eachEvent, executedCalculationType, cutoffDate));
                        break;
                    case EventType.WorkflowRevertToDraft:
						procScheduledEvents.AddRange(WorkflowRevertToDraftScheduledEvents(eachEvent, executedCalculationType, cutoffDate));
                        break;
                    case EventType.Workflow:
                    case EventType.RetroRevertToDraft:
                    default:
                        break;
                }
            }
            Data.ManagedItem.UpdateProcessScheduledEvents(ManagedItemID, ScheduledEvent.BuildSqlUdt(ManagedItemID, procScheduledEvents), true);
            return true;
        }

        private List<ProcessScheduledEvent> WorkflowRevertToDraftScheduledEvents(Event eachEvent, ScheduledEvent.ExecutedCalculationType executedCalculationType, DateTime cutoffDate)
        {
            List<ProcessScheduledEvent> scheduledEvents = new List<ProcessScheduledEvent>();
            DateTime renewalTermEffectiveDate = DateTime.MinValue;
            RenewalTerm renewalTerm = FindBasicTerm(TermType.Renewal) as RenewalTerm;
            if (renewalTerm != null)
                if (renewalTerm.EffectiveDate.HasValue)
                    renewalTermEffectiveDate = renewalTerm.EffectiveDate.Value;

            DateTime dtRevertEvent;
            //Currently, WorkflowRevertToDraft event forces State to be the 'IsBase' state.
            //TODO - Just added this logic to inhibit the creation of the RevertToBaseStateEvent if the current State is an Exit state.
            if (Workflow.GetRevertToBaseStateEventDate(renewalTermEffectiveDate, out dtRevertEvent))
                if (!State.IsExit ?? false)
                    foreach (ScheduledEvent scheduledEvent in Workflow.RevertEvent.ScheduledEvents)
                        scheduledEvents.Add(new ProcessScheduledEvent(scheduledEvent.ID, eachEvent.ID, dtRevertEvent, false));  

            return scheduledEvents;
        }


		private List<ProcessScheduledEvent> RenewalScheduledEvents(Event eachEvent, ScheduledEvent.ExecutedCalculationType executedCalculationType, DateTime cutoffDate)
        {
            List<ProcessScheduledEvent> processScheduledEvents = new List<ProcessScheduledEvent>();
            RenewalTerm renewalTerm = FindBasicTerm(TermType.Renewal) as RenewalTerm;
            if (renewalTerm != null)
            {
                if (renewalTerm.SendNotification ?? true)
                {
                    if (renewalTerm.RenewalEvent != null && renewalTerm.RenewalEvent.ScheduledEvents != null)
                    {
                        if (renewalTerm.ExpirationDate.HasValue)
                        {
                            int baseOffsetDays = GetBaseOffsetDays(eachEvent, renewalTerm);
                            foreach (ScheduledEvent scheduledEvent in eachEvent.ScheduledEvents)
                            {
                                DateTime eventDate = ScheduledEvent.GetEventDate(renewalTerm.ExpirationDate.Value.AddDays(-1 * baseOffsetDays), scheduledEvent.DateOffset);
                                ProcessScheduledEvent existingPSE = ProcessScheduledEvents.Find(pse => pse.ScheduledEventId == scheduledEvent.ID);
								processScheduledEvents.Add(new ProcessScheduledEvent(scheduledEvent.ID, scheduledEvent.EventId, eventDate, ShouldBeMarkedExecuted(existingPSE, executedCalculationType, eventDate, cutoffDate)));
                            }
                        }
                    }
                }
            }
            return processScheduledEvents;
        }

		private List<ProcessScheduledEvent> CustomScheduledEvents(Event eachEvent, ScheduledEvent.ExecutedCalculationType executedCalculationType, DateTime cutoffDate)
        {
            List<ProcessScheduledEvent> processScheduledEvents = new List<ProcessScheduledEvent>();
            Term term = FindTerm(eachEvent.BaseDateTermID, eachEvent.BaseDateTermName);
            int baseOffsetDays = GetBaseOffsetDays(eachEvent);
            if (term != null)
            {
                switch (term.TermType)
                {
                    case TermType.Date:
                        DateTerm dateTerm = term as DateTerm;
                        if (dateTerm == null)
                            throw new Exception(string.Format("Term '{0}' is not a Date Term.", term.Name));
                        if (dateTerm.Value.HasValue)
                        {
                            foreach (ScheduledEvent scheduledEvent in eachEvent.ScheduledEvents)
                            {
                                DateTime eventDate = ScheduledEvent.GetEventDate(dateTerm.Value.Value.AddDays(-1 * baseOffsetDays), scheduledEvent.DateOffset);
                                ProcessScheduledEvent existingPSE = ProcessScheduledEvents.Find(pse => pse.ScheduledEventId == scheduledEvent.ID);
								processScheduledEvents.Add(new ProcessScheduledEvent(scheduledEvent.ID, scheduledEvent.EventId, eventDate, ShouldBeMarkedExecuted(existingPSE, executedCalculationType, eventDate, cutoffDate)));
                            }
                        }
                        break;

                    case TermType.Renewal:
                        RenewalTerm renewalTerm = term as RenewalTerm;
                        if (renewalTerm == null)
                            throw new Exception(string.Format("Term '{0}' is not a Renewal Term.", term.Name));
                        DateTime? dtBaseDate = null;
                        switch (eachEvent.BaseDateTermPart)
                        {
                            case XMLNames._TPS_EffectiveDate:
                                if (renewalTerm.EffectiveDate.HasValue)
                                    dtBaseDate = renewalTerm.EffectiveDate.Value;
                                break;
                            case XMLNames._TPS_ExpirationDate:
                            case "":
                            case null:
                            default:
                                if (renewalTerm.ExpirationDate.HasValue)
                                    dtBaseDate = renewalTerm.ExpirationDate.Value;
                                break;
                        }
                        if (dtBaseDate.HasValue)
                        {
                            foreach (ScheduledEvent scheduledEvent in eachEvent.ScheduledEvents)
                            {
                                DateTime eventDate = ScheduledEvent.GetEventDate(dtBaseDate.Value.AddDays(-1 * baseOffsetDays), scheduledEvent.DateOffset);
                                ProcessScheduledEvent existingPSE = ProcessScheduledEvents.Find(pse => pse.ScheduledEventId == scheduledEvent.ID);
								processScheduledEvents.Add(new ProcessScheduledEvent(scheduledEvent.ID, scheduledEvent.EventId, eventDate, ShouldBeMarkedExecuted(existingPSE, executedCalculationType, eventDate, cutoffDate)));

                            }
                        }
                        break;

                    default:
                        throw new Exception(string.Format("Tried to assign basic term '{0}' of type {1} to Custom Event '{2}'", term.Name, term.TermType.ToString(), eachEvent.Name));
                }
            }
            return processScheduledEvents;
        }

        private bool ShouldBeMarkedExecuted(ProcessScheduledEvent existingPSE, ScheduledEvent.ExecutedCalculationType executedCalculationType, DateTime eventDate, DateTime cutoffDate)
        {
            switch (executedCalculationType)
            {
                case ScheduledEvent.ExecutedCalculationType.UsePreviousValue:
                    //Note - use previous value only if the eventDate has not been changed!
                    if (existingPSE == null || !Utility.DateHelper.SameDay(existingPSE.EventDate,eventDate))
                        return (eventDate < cutoffDate);
                    return existingPSE.Executed;
                case ScheduledEvent.ExecutedCalculationType.SetAsNotExecuted:
                    return false;
                case ScheduledEvent.ExecutedCalculationType.SetAsExecuted:
                    return true;
                case ScheduledEvent.ExecutedCalculationType.UseGracePeriod:
                    return (eventDate < cutoffDate);
                default:
                    throw new ArgumentOutOfRangeException("Invalid value for executedCalculationType argument: " + executedCalculationType.ToString());
            }
        }

        private int GetBaseOffsetDays(Event eachEvent, RenewalTerm renewalTerm)
        {
            int baseOffsetDays = 0;
            Term offsetTerm = FindTerm(renewalTerm.RenewalEvent.OffsetTermID, renewalTerm.RenewalEvent.OffsetTermName);
            if (offsetTerm != null)
            {
                try { baseOffsetDays += int.Parse(offsetTerm.DisplayValue("")); }
                catch { baseOffsetDays += eachEvent.OffsetDefaultValue; }
            }
            else
            {
                baseOffsetDays += renewalTerm.RenewalEvent.OffsetDefaultValue;
            }
            return baseOffsetDays;
        }


        private int GetBaseOffsetDays(Event eachEvent)
        {
            int baseOffsetDays = 0;
            Term offsetTerm = FindTerm(eachEvent.OffsetTermID, eachEvent.OffsetTermName);
            if (offsetTerm != null)
            {
                try { baseOffsetDays += int.Parse(offsetTerm.DisplayValue("")); }
                catch { baseOffsetDays += eachEvent.OffsetDefaultValue; }
            }
            else
            {
                baseOffsetDays += eachEvent.OffsetDefaultValue;
            }
            return baseOffsetDays;
        }

        #endregion

        #region Compression

        public static int GetManagedItemAuditListCount()
        {
            return Data.ManagedItem.GetManagedItemAuditListCount();
        }

        public static DataTable GetManagedItemAuditList(int maxCount)
        {
            return Data.ManagedItem.GetManagedItemAuditList(maxCount);
        }

        public static void UpdateManagedItemAuditList(DataTable dtTemplateDefZipped)
        {
            Data.ManagedItem.UpdateManagedItemAuditList(dtTemplateDefZipped);
        }

        #endregion Compression

        #region Store

        public static ManagedItemStore CreateStore(
            ILog log, 
            Template template, 
            string managedItem, 
            Guid managedItemID, 
            string managedItemNumber, 
            string status, 
            string state, 
            Dictionary<string /* Term Name */, Term> templateTerms,
            List<string> basicTermsToProcess,
            List<string> complexListTermsToProcess)
        {
            ManagedItemStore managedItemStore = new ManagedItemStore(template.ID, template.Name, managedItemNumber, status, state);
            bool basicTermsProcessed = false;
            bool complexListsProcessed = false;
            bool workflowsProcessed = false;
            int basicTermsToProcessCount = basicTermsToProcess.Count;
            int complexListTermsToProcessCount = complexListTermsToProcess.Count;

            XmlTextReader txtReader = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(managedItem)));
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = true;
            readerSettings.IgnoreComments = true;
            readerSettings.IgnoreProcessingInstructions = true;

            using (XmlReader reader = XmlReader.Create(txtReader, readerSettings))
            {
                reader.MoveToContent();     //Reader initialized and positioned at the 'TemplateDef' node.

                while (!(basicTermsProcessed && complexListsProcessed && workflowsProcessed))
                {
                    if (!reader.Read())     //At start element of a TemplateDef child node.
                    {
                        throw new Exception(string.Format("CreateStore call stopped unexpectedly on failed reader.Read call for ManagedItemNumber '{0}', ManagedItemID '{1}'", managedItemNumber, managedItemID.ToString()));
                    }

                    //Ensure completion before quitting...
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == XMLNames._E_TemplateDef)
                    {
                        string error = string.Empty;
                        if (basicTermsToProcessCount > 0 && !basicTermsProcessed)
                            error += "Basic Terms were not processed.  ";
                        if (complexListTermsToProcessCount > 0 && !complexListsProcessed)
                            error += "Complex Lists were not processed.  ";
                        if (!workflowsProcessed)
                            error += "WorkFlows were not processed.  ";
                        if (!string.IsNullOrEmpty(error))
                            throw new Exception(string.Format("CreateStore call at end element before processing completed for ManagedItemNumber '{0}', ManagedItemID '{1}'.  {2}", managedItemNumber, managedItemID.ToString(), error));
                        break;
                    }

                    switch (reader.Name)
                    {
                        case XMLNames._E_Workflows:
                            Guid activeWorkflowID = new Guid(reader.GetAttribute(XMLNames._A_ActiveWorkflowID));
                            //NOTE - The search for active workflow may fail if the template information does not match the manageditem's information.
                            Workflow activeWorkflow = template.FindWorkflow(activeWorkflowID);
                            if (activeWorkflow != null)
                                managedItemStore.ActiveWorkflow = activeWorkflow.Name;
                            else
                                managedItemStore.ActiveWorkflow = string.Empty;
                            using (reader.ReadSubtree()) { }
                            workflowsProcessed = true;
                            break;

                        case XMLNames._E_Terms:
                            if (basicTermsToProcess.Count > 0)
                            {
                                ReadTermStore(
                                    template.Name,
                                    managedItemID,
                                    reader.ReadSubtree(),
                                    managedItemStore,
                                    templateTerms,
                                    basicTermsToProcess);
                            }
                            else
                            {
                                using (reader.ReadSubtree()) { }
                            }
                            basicTermsProcessed = true;
                            break;

                        case XMLNames._E_ComplexLists:
                            if (complexListTermsToProcess.Count > 0)
                            {
                                ReadComplexListStore(
                                    reader.ReadSubtree(),
                                    managedItemStore,
                                    templateTerms,
                                    complexListTermsToProcess);
                            }
                            else
                            {
                                using (reader.ReadSubtree()) { }
                            }
                            complexListsProcessed = true;
                            break;

                        case XMLNames._E_Events:
                        case XMLNames._E_Document:
                        case XMLNames._E_Comments:
                        case XMLNames._E_TermDependencies:
                        case XMLNames._E_DetailedDescriptions:
                        case XMLNames._E_TermGroups:
                        case XMLNames._E_DocumentPrinters:
                        default:
                            using (reader.ReadSubtree()) { }
                            break;
                    }

                }
            }

            return managedItemStore;
        }

        private static void ReadTermStore(
            string templateName,
            Guid managedItemID,
            XmlReader reader,
            ManagedItemStore managedItemStore,
            Dictionary<string /* Term Name */, Term> templateTerms,
            List<string> basicTermsToProcess)
        {
            string termName = string.Empty;
            bool attachmentsProcessed = false;

            using (reader)
            {
                reader.MoveToContent(); //Positioned at 'Terms' element.
                reader.Read();          //Positioned at first term or end element if no terms.
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    termName = Term.GetTermName(reader);
                    if (basicTermsToProcess.Contains(termName))
                    {
                        //Note - we use the current template definition as a filter for the reported terms.
                        if (templateTerms.ContainsKey(termName))
                        {
                            TermStore termStore = null;
                            switch (reader.Name)
                            {
                                case XMLNames._E_Text:
                                    termStore = TextTerm.CreateStore(termName, reader.ReadSubtree());
                                    break;

                                case XMLNames._E_Date:
                                    termStore = Term.CreateStore(termName, reader.ReadSubtree(), TermType.Date);
                                    break;

                                case XMLNames._E_MSO:
                                    termStore = MSOTerm.CreateStore(termName, reader.ReadSubtree());
                                    break;

                                case XMLNames._E_Renewal:
                                    termStore = RenewalTerm.CreateStore(termName, reader.ReadSubtree());
                                    break;

                                case XMLNames._E_Facility:
                                    termStore = FacilityTerm.CreateStore(termName, reader.ReadSubtree());
                                    break;

                                case XMLNames._E_PickList:
                                    termStore = PickListTerm.CreateStore(termName, reader.ReadSubtree());
                                    break;

                                case XMLNames._E_External:
                                    termStore = ExternalTerm.CreateStore(termName, templateTerms[termName] as ExternalTerm, managedItemID);
                                    break;

                                case XMLNames._E_Link:
                                    using (reader.ReadSubtree()) { }
                                    break;

                                //This will not be processed by older ManagedItems.
                                case XMLNames._E_PlaceHolderAttachments:
                                    attachmentsProcessed = true;
                                    termStore = Attachment.CreateStore(termName, managedItemID);
                                    break;

                                default:
                                    throw new Exception(string.Format("In ReadTermStore: unable to identify term type '{0}'", reader.Name));
                            }
                            if (termStore != null)
                                managedItemStore.AddTerm(termStore);
                        }
                        else
                        {
                            //Not found in the current template.
                            using (reader.ReadSubtree()) { }
                        }

                        basicTermsToProcess.Remove(termName);
                        if (basicTermsToProcess.Count == 0)
                        {
                            //All of the DataStoreDefinition terms have been processed, so quit early.
                            break;
                        }
                    }
                    else
                    {
                        //Not found in the DataStoreDefinition.
                        using (reader.ReadSubtree()) { }
                    }

                    reader.Read();      //Position the reader just past the term node.
                }

            }
            termName = XMLNames._A_Attachments;
            if (!attachmentsProcessed && basicTermsToProcess.Contains(termName))
            {
                TermStore termStore = Attachment.CreateStore(termName, managedItemID);
                if (termStore != null)
                    managedItemStore.AddTerm(termStore);
            }
        }

        private static void ReadComplexListStore(
            XmlReader reader,
            ManagedItemStore managedItemStore,
            Dictionary<string /* Term Name */, Term> templateTerms,
            List<string> complexListTermsToProcess)
        {
            string termName = string.Empty;

            using (reader)
            {
                reader.MoveToContent(); //Positioned at 'ComplexLists' element.
                reader.Read();          //Positioned at first ComplexList term or end element if no terms.
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    termName = Term.GetTermName(reader);
                    if (complexListTermsToProcess.Contains(termName))
                    {
                        //Note - we use the current template definition as a filter for the reported terms.
                        if (templateTerms.ContainsKey(termName))
                        {
                            List<string> templateFieldNames = new List<string>();
                            ComplexList complexList = templateTerms[termName] as ComplexList;
                            foreach (ComplexListField complexListField in complexList.Fields)
                            {
                                templateFieldNames.Add(complexListField.Name);
                            }
                            TermStore termStore = ComplexList.CreateStore(termName, reader.ReadSubtree(), templateFieldNames);
                            managedItemStore.AddTerm(termStore);
                        }
                        else
                        {
                            //Not found in the current template.
                            using (reader.ReadSubtree()) { }
                        }

                        complexListTermsToProcess.Remove(termName);
                        if (complexListTermsToProcess.Count == 0)
                        {
                            //All of the DataStoreDefinition terms have been processed, so quit early.
                            break;
                        }
                    }
                    else
                    {
                        //Not found in the DataStoreDefinition.
                        using (reader.ReadSubtree()) { }
                    }

                    reader.Read();      //Position the reader just past the term node.
                }
            }
        }
 
        #endregion Store
    }
}
