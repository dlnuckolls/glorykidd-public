using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using Kindred.Common.Security;
using System.Data;
using System.Collections.ObjectModel;

namespace Kindred.Knect.ITAT.Business
{
	public sealed class SecurityHelper
	{

		#region Constructor

		public SecurityHelper(ITATSystem system)
		{
			_system = system;
		}

        public SecurityHelper()
        {
            _system = null;
        }

		#endregion

		#region private members

		private ADPerson _person;
		private List<string> _userRoles;
		private FacilityCollection _userFacilities;
		private FacilityCollection _allUserFacilities;
		private ITATSystem _system;

		#endregion

		#region Private Properties

		public ADPerson Person
		{
			get
			{
				if (_person == null)
					_person = SecurityHelper.GetLoggedOnPerson();

				return _person;
			}
		}

		#endregion

		#region Public Properties

        public static string LoggedOnUser
        {
            get { return GetLoggedOnUser(); }
        }

		public ITATSystem System
		{
			set { _system = value; }
		}

		public List<string> UserRoles
		{
			get
			{
				if (_userRoles == null)
					_userRoles = GetUserRoles();
				return _userRoles;
			}
		}

		public FacilityCollection UserFacilities
		{
			get
			{
				if (_userFacilities == null)
					_userFacilities = GetUserFacilities();
				return _userFacilities;
			}
		}

		public FacilityCollection AllUserFacilities
		{
			get
			{
				if (_allUserFacilities == null)
					_allUserFacilities = GetAllUserFacilities();
				return _allUserFacilities;
			}
		}

		public Guid PersonId
		{
			get
			{
				return (new Guid(Person.PersonId));
			}
		}

		#endregion


		#region Public Methods


		/// <summary>
		/// Returns true if the user is an Application Admin or System Admin or belongs to one or more of the allowedRoles, AND one or more of the user's facilities is in facilityHierarchy.
		///  -- Similar to CanPerformFunction(string[] allowedRoles) with the additional filter on facilities.
		/// </summary>
		/// <param name="allowedRoles">a list of roles that are pemitted to perform this function</param>
		/// <param name="facilityHierarchy">A list of facilities starting with the pertinent facility and going UPWARDS in the facfility hierarchy to Corporate.</param>
		/// <returns>bool indicating whether the user is allowed to perform the requested function</returns>
		public bool CanPerformFunction(List<string> allowedRoles, List<int> facilityHierarchy)
		{
			if ((Utility.ListHelper.FacilityListsOverlap(AllUserFacilities, facilityHierarchy)) && (Utility.ListHelper.HaveAMatch<string>(allowedRoles, UserRoles)))
				return true;
			else
				return false;
		}

		public bool CanPerformFunction(List<Role> allowedRoles, List<int> facilityHierarchy)
		{
			return CanPerformFunction(allowedRoles.ConvertAll<string>(Role.StringConverter), facilityHierarchy);
		}


		/// <summary>
		/// Returns true if the user is an Application Admin or System Admin or belongs to one or more of the allowedRoles.
		/// </summary>
		/// <param name="allowedRoles">a list of roles that are permitted to perform this function</param>
		/// <returns>bool indicating whether the user is allowed to perform the requested function</returns>
		public bool CanPerformFunction(List<string> allowedRoles)
		{
			 return Utility.ListHelper.HaveAMatch<string>(allowedRoles, UserRoles);
		}


		public bool CanPerformFunction(List<Role> allowedRoles)
		{
			return CanPerformFunction(allowedRoles.ConvertAll<string>(Role.StringConverter));
		}

		public static List<string> ResourceNamesFromRoles(List<Role> roles)
		{
			//transform role names into security group names
			List<string> resourceNames = new List<string>();
			foreach (Role role in roles)
				if (!resourceNames.Contains(role.ApplicationSecurityResourceName))
					resourceNames.Add(role.ApplicationSecurityResourceName);
			return resourceNames;
		}

		public static List<string> ResourceNamesFromRoleNames(Business.ITATSystem system, List<string> roleNames)
		{
			//transform role names into security group names
			List<string> resourceNames = new List<string>();
			foreach (string roleName in roleNames)
			{
				Role role = Role.FindRole(system.Roles, roleName);
                if (role != null)
                {
                    if (!resourceNames.Contains(role.ApplicationSecurityResourceName))
                        resourceNames.Add(role.ApplicationSecurityResourceName);
                }
			}
			return resourceNames;
		}



		public static Kindred.Common.Security.NameEmailPair[] GetEmailRecipients(ITATSystem system, List<string> roleNames)
		{
			//transform role names into security group names
			List<string> applicationResourceNames = SecurityHelper.ResourceNamesFromRoleNames(system, roleNames);
			ReadOnlyCollection<string> groupNames = Kindred.Common.Security.SecurityInfo.GetResourceGroups(system.ApplicationSecurityName, applicationResourceNames);
			Kindred.Common.Security.NameEmailPair[] emailRecipients = Kindred.Common.Security.EmailHelper.GetEmailRecipients(groupNames);
			return emailRecipients;
		}


		public static Kindred.Common.Security.NameEmailPair[] GetEmailRecipients(ITATSystem system, List<string> roleNames, List<int> facIDs)
		{
			//transform role names into security group names
			List<string> applicationResourceNames = SecurityHelper.ResourceNamesFromRoleNames(system, roleNames);
			ReadOnlyCollection<string> groupNames = Kindred.Common.Security.SecurityInfo.GetResourceGroups(system.ApplicationSecurityName, applicationResourceNames);
			Kindred.Common.Security.NameEmailPair[] emailRecipients = Kindred.Common.Security.EmailHelper.GetEmailRecipients(groupNames, facIDs);
			return emailRecipients;
		}

		#endregion


		#region public static methods

		/// <summary>
		/// This function returns the AD person object for the user.
		/// </summary>
		/// <returns>The AD person object for the user.</returns>
		public static ADPerson GetLoggedOnPerson()
		{
			try
			{
                string userLogin = GetLoggedOnUser();
				if (string.IsNullOrEmpty(userLogin))
					return null;
				else
					return ADPerson.GetPerson(userLogin);
			}
			catch
			{
				return null;
			}
		}


		/// <summary>
		/// This function returns the PersonID for the logged on user.
		/// </summary>
		/// <returns>The (Guid) PersonID for the user in the People database.</returns>
		public static Guid GetLoggedOnPersonId()
		{
			if (HttpContext.Current == null)
				return Guid.Empty;
			return GetPersonId(HttpContext.Current.User.Identity.Name);
		}


		/// <summary>
		/// This function returns the PersonID for the speciifed user.
		/// </summary>
		/// <returns>The (Guid) PersonID for the user in the People database.</returns>
		public static Guid GetPersonId(string loginId)
		{
			if (loginId.Contains("\\"))
				loginId = loginId.Substring(loginId.LastIndexOf("\\") + 1);
			try
			{
				Kindred.Common.Security.ADPerson p = Kindred.Common.Security.ADPerson.GetPerson(loginId);
				if (p == null)
					return Guid.Empty;
				else
					return new Guid(p.PersonId);
			}
			catch
			{
				return Guid.Empty;
			}
		}


		#endregion


		#region private methods

        private static string GetLoggedOnUser()
        {
            string userLogin = HttpContext.Current.User.Identity.Name;
            userLogin = userLogin.Substring(userLogin.LastIndexOf("\\") + 1);
            return userLogin;
        }

		private List<string> GetUserRoles()
		{
			if ((_system == null) || string.IsNullOrEmpty(_system.Name))
			{
				throw new Exception("System not set");
			}
			List<string> rtn = new List<string>();
			List<string> systemRoles = _system.Roles.ConvertAll<string>(Role.StringConverter);
			KeyedCollection<string, ISecurityResource> securityResources = SecurityInfo.GetUserAccessTypes(Person.SAMLogonName, _system.ApplicationSecurityName, systemRoles);
			foreach (string role in systemRoles)
			{
				if (securityResources.Contains(role))
				{
					ISecurityResource securityResource = securityResources[role];
					if (!string.IsNullOrEmpty(securityResource.AccessType))
						rtn.Add(role);
				}
			}
			return rtn;
		}


		private FacilityCollection GetUserFacilities()
		{
			List<int> facilityIdList = new List<int>(Person.FacilityIdListInt);
			return FacilityCollection.FacilityList(facilityIdList, false);
		}

		private FacilityCollection GetAllUserFacilities()
		{
			List<int> facilityIdList = new List<int>(Person.FacilityIdListInt);
			return FacilityCollection.FacilityList(facilityIdList, true);
		}



		#endregion

		}
}
