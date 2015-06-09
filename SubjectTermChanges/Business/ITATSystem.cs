using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using Kindred.Knect.ITAT.Utility;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class ITATSystem
	{

		#region private members
		private Guid _id;
		private string _name;
		private string _applicationSecurityName;
		private bool? _hasContent;
		private bool? _trackAudit;
		private string _managedItemName;
		private string _introPage;
		private string _defaultDateFormat;
		private string _managedItemNumberSystemId;
		private string _managedItemNumberSystemType;	//Has values such as "Fac:SAP" (only valid if a primaryFacility defined), "12345" (hard-coded)
		private FacilityTerm _primaryFacility;
		private List<Status> _statuses;
		private List<Role> _roles;
		private Dictionary<string, ApplicationFunction> _applicationFunctions;    //publicly accessed via the AllowedRoles() method
		private List<Term> _terms;
		private List<DocumentType> _documentTypes;
		private string _XMLVersion;
		private bool? _hasOwningFacility;
		private Utility.DocumentStorageType _documentStorageType;
		private string _documentStorageRootPath;
		private string _overrideEmail;
		private string _ownerEmail;
		private string _headerRowSize;
        private string _searchResultsHeaderRowSize;
        private bool? _allowAttachments;
		private List<ExternalInterfaceConfig> _externalInterfaces;
		private bool _allowRetro;
        private bool _viewersAddComments;
        private bool _viewersEditAttachments;
		private bool _viewersCannotSeeComplexLists;
        private int _retroEventGracePeriodDays;
        private bool _supportMultipleDocuments;
        private bool _allowNotificationFilterFacility;
        private bool _allowActionConfirmation;

		#endregion

		#region Properties

		public Guid ID
		{
			get { return _id; }
		}

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
		}

		public string XMLVersion
		{
			get
			{
				return Utility.XMLHelper.GetXMLText(_XMLVersion);
			}
		}

		public string ApplicationSecurityName
		{
			get { return Utility.XMLHelper.GetXMLText(_applicationSecurityName); }
		}

		public bool? HasContent
		{
			get { return _hasContent; }
		}

		public bool? TrackAudit
		{
			get { return _trackAudit; }
		}

		public bool? HasOwningFacility
		{
			get { return _hasOwningFacility; }
		}

		public bool? AllowAttachments
		{
			get { return _allowAttachments; }
		}

        public bool? ViewersAddComments
        {
            get { return _viewersAddComments; }
        }

        public bool? ViewersEditAttachments
        {
            get { return _viewersEditAttachments; }
        }

		public bool ViewersCannotSeeComplexLists
		{
			get { return _viewersCannotSeeComplexLists; }
		}
        
		public string IntroPage
		{
			get { return _introPage; }
		}

		public string DefaultDateFormat
		{
			get { return _defaultDateFormat; }
		}

		public string ManagedItemNumberSystemId
		{
			get { return _managedItemNumberSystemId; }
		}

		public string ManagedItemNumberSystemType
		{
			get { return _managedItemNumberSystemType; }
		}

		public FacilityTerm PrimaryFacility
		{
			get { return _primaryFacility; }
			set { _primaryFacility = value; }
		}

		public List<DocumentType> DocumentTypes
		{
			get { return _documentTypes; }
			set { _documentTypes = value; }
		}

		public List<Status> Statuses
		{
			get { return _statuses; }
			set { _statuses = value; }
		}

		public List<Role> Roles
		{
			get { return _roles; }
			set { _roles = value; }
		}

		public List<Term> Terms
		{
			get { return _terms; }
			set { _terms = value; }
		}

		public string ManagedItemName
		{
			get { return Utility.XMLHelper.GetXMLText(_managedItemName); }
			set { _managedItemName = Utility.XMLHelper.SetXMLText(value); }
		}

		public Utility.DocumentStorageType DocumentStorageType
		{
			get { return _documentStorageType; }
		}

		public string DocumentStorageRootPath
		{
			get { return _documentStorageRootPath; }
		}

		public string OverrideEmail
		{
			get { return _overrideEmail; }
		}

		public string OwnerEmail
		{
			get { return _ownerEmail; }
		}

		public int HeaderRowSize
		{
			get
			{
				if (!string.IsNullOrEmpty(_headerRowSize))
				{
					int nRowSize;
					if (int.TryParse(_headerRowSize, out nRowSize))
						if (nRowSize >= 1)
							return nRowSize;
				}
				return 1;
			}
		}

        public int SearchResultsHeaderRowSize
        {
            get
            {
                if (!string.IsNullOrEmpty(_searchResultsHeaderRowSize))
                {
                    int nRowSize;
                    if (int.TryParse(_searchResultsHeaderRowSize, out nRowSize))
                        if (nRowSize >= 2)
                            return nRowSize;
                }
                return 2;
            }
        }

		public List<ExternalInterfaceConfig> ExternalInterfaces
		{
		   get { return _externalInterfaces; }
		}

		public bool AllowRetro
		{
			get { return _allowRetro; }
		}

        public int RetroEventGracePeriodDays
		{
            get { return _retroEventGracePeriodDays; }
		}

        public bool? SupportMultipleDocuments
        {
            get { return _supportMultipleDocuments; }
        }

        public bool AllowNotificationFilterFacility
        {
            get { return _allowNotificationFilterFacility; }
        }

        //_A_AllowActionConfirmation
        public bool AllowActionConfirmation
        {
            get { return _allowActionConfirmation; }
        }

		#endregion


		#region Constructors

		private ITATSystem()
		{
			_roles = new List<Role>();
			_statuses = new List<Status>();
			_documentTypes = new List<DocumentType>();
			_externalInterfaces = new List<ExternalInterfaceConfig>();
			_applicationFunctions = new Dictionary<string, ApplicationFunction>();
		}

		#endregion


		#region Public Static members

		/// <summary>
		/// Returns an instance of the ITATSystem class (from the Cache or the database), or null if no "system" exists with this systemID
		/// </summary>
		/// <param name="systemID">Database ID of the "system"</param>
		public static ITATSystem Get(Guid systemID, List<string> validation)
		{
			//This code is also used by ITATEventProcessor, therefore the System.Web.HttpContext will not be defined....
			//If validation is not null, force a read from the database, and a validation check.
			ITATSystem rtn = validation == null ? ITATSystem.LoadFromCache(systemID) : null;
			//if rtn != null, then the system is loaded from the server's cache
			if (rtn == null)
			{
				rtn = ITATSystem.LoadFromDatabase(systemID, validation);
				if (System.Web.HttpContext.Current != null)
				{
					if (rtn != null)
					{
						//System loaded from Database
						rtn.RefreshCache();
					}
					else
					{
						//System NOT LOADED
					}
				}
			}
			return rtn;
		}

		public static ITATSystem Get(Guid systemID)
		{
			return Get(systemID, null);
		}

		public static ITATSystem Get(string systemName)
		{
            DataTable dt = Data.ITATSystem.GetSystemList().Tables[0];
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if ((string)dt.Rows[i][Data.DataNames._C_ITATSystemName] == systemName)
					return Get((Guid)dt.Rows[i][Data.DataNames._C_ITATSystemID]);
			}
			return null;
		}

		public static ITATSystem CreateNew(Guid systemID, string systemName, string systemDef)
		{
			ITATSystem rtn = new ITATSystem();
			rtn._id = systemID;
			rtn._name = systemName;
			rtn.UpdateSystemDef(systemDef);
			return rtn;
		}

		public static ITATSystem CreateNew(Guid systemID, string systemName, System.IO.Stream inputStream)
		{
			ITATSystem rtn = new ITATSystem();
			rtn._id = systemID;
			rtn._name = systemName;
			rtn.UpdateSystemDef(inputStream);
			return rtn;
		}

		public static DataSet GetSystemList()
		{
            return Data.ITATSystem.GetSystemList();
		}

		public static void ClearSystemCache(Guid systemId)
		{
			string cacheId = ITATSystem.CacheID(systemId);
			System.Web.HttpContext.Current.Cache.Remove(cacheId);
		}



		#endregion


		#region Public Instance Methods

		public void UpdateSystemDef(string systemDef)
		{
			XmlDocument rawXML = new XmlDocument();
			rawXML.PreserveWhitespace = false;
			rawXML.LoadXml(systemDef);
			ParseSystemDef(rawXML, null);
			Data.ITATSystem.UpdateSystemDef(_id, _name, systemDef);
			RefreshCache();
		}

		public void UpdateSystemDef(System.IO.Stream inputStream)
		{
			XmlDocument rawXML = new XmlDocument();
			rawXML.Load(inputStream);
			ParseSystemDef(rawXML, null);
			Data.ITATSystem.UpdateSystemDef(_id, _name, rawXML.OuterXml);
			RefreshCache();
		}

		public List<string> AllowedRoles(string applicationFunctionName)
		{
			if (_applicationFunctions.ContainsKey(applicationFunctionName))
				return _applicationFunctions[applicationFunctionName].AllowedRoles;
			else
				return new List<string>();
		}

		public void SetTermsVisible(bool bVisible)
		{
			foreach (Term term in _terms)
			{
				term.Runtime.Visible = bVisible;
			}
		}

		public bool RoleExists(string sRole)
		{
			Predicate<Role> p = delegate(Role role)
					{
						return (role.Name == sRole);
					};
			return Roles.Exists(p);
		}

		public List<Status> ViewableStatuses(List<string> userRoles)
		{
			List<Status> rtn = new List<Status>();
			foreach (Status status in _statuses)
			{
				if (status.ViewableByAllRoles)
					rtn.Add(status);
				else
					if (status.ViewingRoles.Count > 0)
						if (Utility.ListHelper.HaveAMatch<string>(status.ViewingRoles, userRoles))
							rtn.Add(status);
			}
			return rtn;
		}


		public void GetFacilityLevels(string roleName, ref List<int> absoluteLevels, ref List<int> relativeLevels)
		{
			absoluteLevels = new List<int>();
			relativeLevels = new List<int>();

			Role role = Role.FindRole(_roles, roleName);
			if (role != null)
			{
				if (role.IsDistribution)
				{
					switch (role.RoleLevelType)
					{
						case RoleLevelType.Absolute:
							absoluteLevels = Role.CombineList(absoluteLevels, role.RoleLevels);
							break;

						case RoleLevelType.Relative:
							relativeLevels = Role.CombineList(relativeLevels, role.RoleLevels);
							break;

						case RoleLevelType.Unknown:
						default:
							throw new Exception(string.Format("Encountered an unhandled RoleLevelType '{0}' where Absolute or Relative were expected", role.RoleLevelType.ToString()));
					}
				}
			}
		}


        public bool UserIsInRole(string applicationFunctionName)
        {
            SecurityHelper securityHelper = new SecurityHelper(this);
            if (Utility.ListHelper.HaveAMatch(this.AllowedRoles(applicationFunctionName), securityHelper.UserRoles))
            {
                return true;
            }
            return false;

        }

		#endregion


		#region private static methods

		private static ITATSystem LoadFromCache(Guid systemID)
		{
			//Try to get ITATSystem instance from the Cache.  If so, return it.
			//This code is also used by ITATEventProcessor, therefore the System.Web.HttpContext will not be defined....
			if (System.Web.HttpContext.Current == null)
				return null;
			try
			{
				return (ITATSystem)System.Web.HttpContext.Current.Cache[CacheID(systemID)];
			}
			catch (Exception ex)
			{
				throw new XmlException(String.Format("Error retrieving ITATSystem instance from the cache for system={0},   Error: {1}", systemID.ToString("D"), ex.Message));
			}
		}

		private static ITATSystem LoadFromDatabase(Guid systemID, List<string> validation)
		{
			ITATSystem itatSystem;
			//Try to create ITATSystem instance from the database.  If so, add it to the Cache and return it.
			//try
			//{
				DataSet ds = Data.ITATSystem.GetSystem(systemID);
				if (ds.Tables[0].Rows.Count == 0)
					return null;

				itatSystem = new ITATSystem();
				itatSystem._id = systemID;
				itatSystem._name = (string)ds.Tables[0].Rows[0][Data.DataNames._C_ITATSystemName];

				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.PreserveWhitespace = false;
				string s = (string)ds.Tables[0].Rows[0][Data.DataNames._C_ITATSystemDef];
				xmlDoc.LoadXml((string)ds.Tables[0].Rows[0][Data.DataNames._C_ITATSystemDef]);
				itatSystem.ParseSystemDef(xmlDoc, validation);

				return itatSystem;
			//}
			//catch (Exception ex)
			//{
			//    throw new XmlException(String.Format("Error retrieving ITATSystem instance from the database for system={0},   Error: {1}", systemID.ToString("D"), ex.Message));
			//}
		}

		private static string CacheID(Guid systemID)
		{
			return string.Concat("ITATSystem.", systemID.ToString("D"));
		}




		#endregion



		#region private instance methods

		private string CacheID()
		{
			return ITATSystem.CacheID(_id);
		}

		private void ParseSystemDef(XmlDocument rawXML, List<string> validation)
		{
			if (rawXML == null)
				throw new NullReferenceException("ITATSystem.ParseSystemDef: ITATSystem._rawXML is null");
			if (rawXML.DocumentElement.Name != XMLNames._E_SystemDef)
				throw new XmlException("ITATSystem.ParseSystemDef: The root element of ITATSystem._rawXML must be \"SystemDef\".");
			GetRootLevelAttributes(rawXML, validation);
			GetIntroPage(rawXML, validation);
			GetTerms(rawXML, validation);
			GetPrimaryFacility(rawXML, validation);
			GetRoles(rawXML, validation);
			GetApplicationFunctions(rawXML, validation);
			GetStatuses(rawXML, validation);
			GetDocumentStorageConfig(rawXML, validation);
			GetDocumentTypes(rawXML, validation);
			GetValidation(validation);
			GetExternalInterfaces(rawXML, validation);
		}


		private void AddValidation(string sError, List<string> validation)
		{
			if (validation == null)
				return;
			if (string.IsNullOrEmpty(sError))
				return;
			validation.Add(sError);
		}

		private void GetDocumentStorageConfig(XmlDocument rawXML, List<string> validation)
		{
			XmlNode node = rawXML.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_SystemDef, XMLNames._E_DocumentStoreConfig));
			if (node != null)
			{
				_documentStorageRootPath = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_RootPath);
				try
				{
					_documentStorageType = (Utility.DocumentStorageType)Enum.Parse(typeof(Utility.DocumentStorageType), Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Type));
				}
				catch
				{
					throw new Exception("Invalid DocumentStorageType specified in the System XML");
				}				
			}
		}


		private void GetIntroPage(XmlDocument rawXML, List<string> validation)
		{
			XmlNode node = rawXML.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_SystemDef, XMLNames._E_IntroPage));
			if (node != null)
				_introPage = node.InnerXml;
			else
				AddValidation("Missing Intro Page", validation);
		}

		
		private void GetTerms(XmlDocument rawXML, List<string> validation)
		{
			XmlNode termsNode = rawXML.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_SystemDef, XMLNames._E_Terms));
            if (termsNode != null)
            {
                _terms = BasicTerms.Create(termsNode, null, null);
            }
            else
                AddValidation("Missing Basic Terms", validation);
		}


		private void GetExternalInterfaces(XmlDocument rawXML, List<string> validation)
		{
		    _externalInterfaces.Clear();
		    XmlNodeList externalInterfaceNodes = rawXML.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_SystemDef, XMLNames._E_ExternalInterfaces, XMLNames._E_ExternalInterface));
		    if (!Utility.XMLHelper.ListNullOrEmpty(externalInterfaceNodes))
		        foreach (XmlNode externalInterfaceNode in externalInterfaceNodes)
		            _externalInterfaces.Add(new ExternalInterfaceConfig(externalInterfaceNode));
		}


		private void GetPrimaryFacility(XmlDocument rawXML, List<string> validation)
		{
			foreach (Term term in _terms)
			{
				if (term.TermType == TermType.Facility)
				{
					Business.FacilityTerm facilityTerm = term as Business.FacilityTerm;
					if (facilityTerm.IsPrimary.HasValue)
					{
						if (facilityTerm.IsPrimary.Value)
						{
							_primaryFacility = facilityTerm;
							break;
						}
					}
				}
			}
		}

		private void GetRoles(XmlDocument rawXML, List<string> validation)
		{
			_roles.Clear();
			XmlNodeList roleNodes = rawXML.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_SystemDef,XMLNames._E_Roles,XMLNames._E_Role));
			if (Utility.XMLHelper.ListNullOrEmpty(roleNodes))
				AddValidation("Missing Roles", validation);
			else
			{
				foreach (XmlNode roleNode in roleNodes)
				{
					string name = Utility.XMLHelper.GetAttributeString(roleNode, XMLNames._A_Name);
					string appResourceName = Utility.XMLHelper.GetAttributeString(roleNode, XMLNames._A_ApplicationSecurityResourceName);
					//Next 2 lines for backward-compatibility:  If the Role node does not have a Name attribute, assume that the Name and ApplicationResourceName are the same
					if (string.IsNullOrEmpty(name))
						name = appResourceName;
					Role role = new Role(name, appResourceName);
					string roleType = Utility.XMLHelper.GetAttributeString(roleNode, XMLNames._A_RoleType);
					string roleLevelType = Utility.XMLHelper.GetAttributeString(roleNode, XMLNames._A_RoleLevelType);
					string roleLevels = Utility.XMLHelper.GetAttributeString(roleNode, XMLNames._A_RoleLevels);
					Role.SetRole(role, roleType, roleLevelType, roleLevels);
					_roles.Add(role);
				}
			}
		}

		private void GetApplicationFunctions(XmlDocument rawXML, List<string> validation)
		{
			_applicationFunctions.Clear();
			XmlNodeList appFunctionNodes = rawXML.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_SystemDef, XMLNames._E_ApplicationFunctions, XMLNames._E_ApplicationFunction));
			foreach (XmlNode appFunctionNode in appFunctionNodes)
			{
				ApplicationFunction applicationFunction = new ApplicationFunction();
				applicationFunction.Name = Utility.XMLHelper.GetAttributeString(appFunctionNode, XMLNames._A_ApplicationFunctionName);
				foreach (XmlNode appFunctionRoleNode  in appFunctionNode.SelectNodes("Role"))
				{
					string role =  Utility.XMLHelper.GetAttributeString(appFunctionRoleNode, "Name");
					applicationFunction.AllowedRoles.Add(role);
				}
				_applicationFunctions.Add(applicationFunction.Name, applicationFunction);
			}
		}


		private void GetStatuses(XmlDocument rawXML, List<string> validation)
		{
			_statuses.Clear();
			XmlNodeList statusNodes = rawXML.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_SystemDef,XMLNames._E_Statuses,XMLNames._E_Status));
			if (Utility.XMLHelper.ListNullOrEmpty(statusNodes))
				AddValidation("Missing Statuses", validation);
			else
			{
				foreach (XmlNode statusNode in statusNodes)
				{
					_statuses.Add(new Status(statusNode));
				}
			}
		}

		private void GetDocumentTypes(XmlDocument rawXML, List<string> validation)
		{

			_documentTypes.Clear();
			XmlNodeList documentNodes = rawXML.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_SystemDef,XMLNames._E_DocumentTypes,XMLNames._E_DocumentType));
			if (Utility.XMLHelper.ListNullOrEmpty(documentNodes))
				AddValidation("Missing DocumentTypes", validation);
			else
			{
				foreach (XmlNode documentNode in documentNodes)
				{
					_documentTypes.Add(new DocumentType(Utility.XMLHelper.GetAttributeString(documentNode, XMLNames._A_Name)));
				}
			}
		}



		private void GetRootLevelAttributes(XmlDocument rawXML, List<string> validation)
		{
			XmlElement root = rawXML.DocumentElement;
			_hasContent = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_HasContent);
			_applicationSecurityName = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_ApplicationSecurityName);
			_trackAudit = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_TrackAudit);
			_managedItemName = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_ManagedItemName);
			_defaultDateFormat = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_DefaultDateFormat);		//
			_managedItemNumberSystemId = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_ManagedItemNumberSystemID);
			_managedItemNumberSystemType = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_ManagedItemNumberSystemType);
			_XMLVersion = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_XMLVersion);
			_hasOwningFacility = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_HasOwningFacility);
			_overrideEmail = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_OverrideEmail);
			_ownerEmail = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_OwnerEmail);
			_allowAttachments = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_AllowAttachments);
			_headerRowSize = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_HeaderRowSize);
            _searchResultsHeaderRowSize = Utility.XMLHelper.GetAttributeString(root, XMLNames._A_SearchResultsHeaderRowSize);
			_allowRetro = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_AllowRetro) ?? false;
            _viewersAddComments = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_ViewersAddComments) ?? false;
            _viewersEditAttachments = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_ViewersEditAttachments) ?? false;
			_viewersCannotSeeComplexLists = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_ViewersCannotSeeComplexLists) ?? false;
            _retroEventGracePeriodDays = Utility.XMLHelper.GetAttributeInt(root, XMLNames._A_RetroEventGracePeriodDays) ?? 0;
            _supportMultipleDocuments = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_SupportMultipleDocuments) ?? false;
            _allowNotificationFilterFacility = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_AllowNotificationFilterFacility) ?? false;
            _allowActionConfirmation = Utility.XMLHelper.GetAttributeBool(root, XMLNames._A_AllowActionConfirmation) ?? false;
        }

		private void GetValidation(List<string> validation)
		{
			if (validation == null)
				return;
			if (_terms != null)
			{
				int nPrimaryFacilityCount = 0;
				int nTerm1Count = 0;
				int nTerm2Count = 0;
				int nTerm3Count = 0;
                int nTerm4Count = 0;
                int nTerm5Count = 0;
                int nTerm6Count = 0;
                int nTerm7Count = 0;
                int nTermXCount = 0;

				foreach (Term term in _terms)
				{
					if (!(term.SystemTerm))
						AddValidation(string.Format("Term '{0}', type '{1}' not a system term", term.Name, term.TermType.ToString()), validation);
					switch (term.TermType)
					{
						case TermType.Facility:
							Business.FacilityTerm facilityTerm = term as Business.FacilityTerm;
							if (facilityTerm.IsPrimary ?? false)
								nPrimaryFacilityCount++;
							if ((HasOwningFacility ?? false) && (facilityTerm.MultiSelect ?? false))
								AddValidation(string.Format("System Facility Term '{0}' cannot be multi-select if each Managed Item will have an Owning Facility.", term.Name), validation);
							break;
						case TermType.ComplexList:
							AddValidation(string.Format("System Term '{0}' is a ComplexList and is not suported", term.Name), validation);
							break;
					}

					if ((term.SystemTerm) && (term.UseDBField ?? false))
					{
						if (term.TermType == TermType.Link)
							AddValidation(string.Format("Encountered term '{0}' of type '{1}' - not allowed as a DBField term", term.Name, term.TermType.ToString()), validation);
						switch (term.DBFieldName)
						{
							case Data.DataNames._C_Term1:
								nTerm1Count++;
								break;

							case Data.DataNames._C_Term2:
								nTerm2Count++;
								break;

							case Data.DataNames._C_Term3:
								nTerm3Count++;
								switch (term.TermType)
								{
									case TermType.Text:
									case TermType.MSO:
									case TermType.Facility:
									case TermType.PickList:
									case TermType.Link:
										AddValidation(string.Format("Encountered term '{0}' of type '{1}' assigned to Term3 - which is Date fields only", term.Name, term.TermType.ToString()), validation);
										break;
								}
								break;

                            case Data.DataNames._C_Term4:
                                nTerm4Count++;
                                break;

                            case Data.DataNames._C_Term5:
                                nTerm5Count++;
                                break;

                            case Data.DataNames._C_Term6:
                                nTerm6Count++;
								switch (term.TermType)
								{
									case TermType.Text:
									case TermType.MSO:
									case TermType.Facility:
									case TermType.PickList:
									case TermType.Link:
										AddValidation(string.Format("Encountered term '{0}' of type '{1}' assigned to Term6 - which is Date fields only", term.Name, term.TermType.ToString()), validation);
										break;
								}
                                break;

                            case Data.DataNames._C_Term7:
                                nTerm7Count++;
								switch (term.TermType)
								{
									case TermType.Text:
									case TermType.MSO:
									case TermType.Facility:
									case TermType.PickList:
									case TermType.Link:
										AddValidation(string.Format("Encountered term '{0}' of type '{1}' assigned to Term7 - which is Date fields only", term.Name, term.TermType.ToString()), validation);
										break;
								}
                                break;

							default:
								nTermXCount++;
								break;
						}
					}
				}
				if (nPrimaryFacilityCount > 1)
					AddValidation(string.Format("Encountered {0} Primary Facility terms - should just be 1 max",nPrimaryFacilityCount.ToString()), validation);
				if (nTerm1Count > 1)
					AddValidation(string.Format("Encountered {0} 'Term1' terms - should just be 1 max", nTerm1Count.ToString()), validation);
				if (nTerm2Count > 1)
					AddValidation(string.Format("Encountered {0} 'Term2' terms - should just be 1 max", nTerm2Count.ToString()), validation);
				if (nTerm3Count > 1)
					AddValidation(string.Format("Encountered {0} 'Term3' terms - should just be 1 max", nTerm3Count.ToString()), validation);
                if (nTerm4Count > 1)
                    AddValidation(string.Format("Encountered {0} 'Term4' terms - should just be 1 max", nTerm4Count.ToString()), validation);
                if (nTerm5Count > 1)
                    AddValidation(string.Format("Encountered {0} 'Term5' terms - should just be 1 max", nTerm5Count.ToString()), validation);
                if (nTerm6Count > 1)
                    AddValidation(string.Format("Encountered {0} 'Term6' terms - should just be 1 max", nTerm6Count.ToString()), validation);
                if (nTerm7Count > 1)
                    AddValidation(string.Format("Encountered {0} 'Term7' terms - should just be 1 max", nTerm7Count.ToString()), validation);
                if (nTermXCount > 1)
					AddValidation(string.Format("Encountered {0} terms that are not Term1, term2 or Term3", nTermXCount.ToString()), validation);
			}
		}

        public bool TermIsMultiSelectSearch(string systemTermDBStorage)
        {
            Term term = _terms.Find(t => t.DBFieldName == systemTermDBStorage && t.SystemTerm);
            if (term != null)
                return term.MultiSelectSearch;
            return false;
        }

		private void RefreshCache()
		{
			System.Web.HttpContext.Current.Cache.Insert(CacheID(), this, null, DateTime.Now.AddHours(4), System.Web.Caching.Cache.NoSlidingExpiration);
		}

		public ExternalInterfaceConfig FindExternalInterfaceConfig(string interfaceConfigName)
		{
		    Predicate<ExternalInterfaceConfig> p = delegate(ExternalInterfaceConfig eic) { return (interfaceConfigName == eic.Name); };
		    return _externalInterfaces.Find(p);
		}

		#endregion

	}
}
