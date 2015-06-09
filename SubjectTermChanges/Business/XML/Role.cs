using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[FlagsAttribute]
	public enum RoleType : short
	{
		//Note - these must be orders of 2, i.e. 1, 2, 4, 8, 16, 32, etc
		//Note - if new values are added, be sure to update the "All" value to be the "sum" of all of the other values
		None = 0,
		Security = 1,
		Distribution = 2,
		All = Security | Distribution
	};

	public enum RoleLevelType
	{
		Unknown = 0,
		Absolute = 1,		//An 'Absolute' RoleLevel corresponds to 0 == Facility, 5 == Corporate.
							//However, from the FacilityMaster perspective, 6 == Facility and 1 == Corporate 
							//(as retrieved from the FacilityMaster.dbo.GetHierarchyAll sproc).

		Relative = 2		//A 'Relative' RoleLevel corresponds to 0 == current level, 1 == one higher, etc.
							//For example, if the 'base' facility level is '3', and the 'Relative' RoleLevel == 2, 
							//then the target RoleLevel = 3 + 2 = 5.  This calculation requires that the base
							//and relative values be based on the same scale, such as FacilityMaster.  So in this
							//case, the result of '5' means target 'District' levels.
	};

	[Serializable, System.Diagnostics.DebuggerDisplay("Name={Name}, Type={Type}, AppSec Resource={ApplicationSecurityResourceName}")]
	public class Role : IEquatable<Role>, IEquatable<string>, IComparable<Role>, IComparable<string>
	{

		#region private constants
		private const char _DELIMITER = ',';
		private const int _FACILITY_LEVELS = 6;	//The Database OrgLevel value for the lowest heirarchy level member (e.g. Hospital)
		//Note - In some cases, the 'Database OrgLevel' could be smaller depending on the total number of 'levels' for that division.  
		//For example, the Pharmacy division has a level of '5' for its facilities.  This needs to be taken into acount when specifying
		//the 'absolute' org level.

/*
 *            Database   Abs Role
 *              Level      Level
 *    Facility    6          0          (LEAF level) 		
 *    District    5          1
 *    Region      4          2
 *    LOB         3          3
 *    Division    2          4
 *    Corporate   1          5          (CORPORATE level)
 */

		private const int _ABSOLUTE_FACILITY_LEVEL_CORPORATE = 5;
		private const int _ABSOLUTE_FACILITY_LEVEL_LEAF = 0;
		private const int _DATABASE_FACILITY_LEVEL_LEAF = 6;
		private const int _DATABASE_FACILITY_LEVEL_CORPORATE = 1;

	#endregion

		#region private members

		private string _name;
		private string _applicationSecurityResourceName;
		private RoleType _roleType;
		private RoleLevelType _roleLevelType;
		private List<int> _roleLevels;


		#endregion


		#region constructors

		public Role(string name, string applicationResourceName)
		{
			_name = name;
			_applicationSecurityResourceName = applicationResourceName;
			_roleType = RoleType.Security;
			_roleLevelType = RoleLevelType.Unknown;
			_roleLevels = null;
		}

		public Role()
			: this(string.Empty, string.Empty) 		
		{
		}

		public Role(string name)
			: this(name, string.Empty)
		{
		}

       public Role(XmlNode node)
            : this(string.Empty, string.Empty)
        {
            _name = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Name);
        }

		#endregion

       #region Build XML

       public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
       {
           if (bValidate)
           {
           }

           Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, _name);
       }

       #endregion


		#region properties

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string ApplicationSecurityResourceName
		{
			get { return Utility.XMLHelper.GetXMLText(_applicationSecurityResourceName); }
			set { _applicationSecurityResourceName = Utility.XMLHelper.SetXMLText(value); }
		}

		public RoleType RoleType
		{
			get { return _roleType; }
			set { _roleType = value; }
		}

		public RoleLevelType RoleLevelType
		{
			get { return _roleLevelType; }
			set { _roleLevelType = value; }
		}

		public List<int> RoleLevels
		{
			get { return _roleLevels; }
			set { _roleLevels = value; }
		}

		public bool IsDistribution
		{
			get { return (RoleType & RoleType.Distribution) == RoleType.Distribution; }
		}

		public bool IsSecurity
		{
			get { return (RoleType & RoleType.Security) == RoleType.Security; }
		}

		#endregion


		public Role Copy()
		{
			Role role = new	Role();
			role.Name = Name;
			role.ApplicationSecurityResourceName = ApplicationSecurityResourceName;
			role.RoleType = RoleType;
			role.RoleLevelType = RoleLevelType;
			role.RoleLevels = RoleLevels;
			return role;
		}

		#region Generic methods/delegates
		
		public static  Converter<Role, string> StringConverter = delegate(Business.Role role) { return role.Name; };


		#endregion


		#region IEquatable<Role> and IEquatable<string>  Members

		public bool Equals(Role other)
		{
			return _name == other._name;
		}

		public bool Equals(string other)
		{
			return _name == other;
		}

		#endregion


		#region  IComparable<Role> and  IComparable<string> Members

		int IComparable<string>.CompareTo(string other)
		{
			return _name.CompareTo(other);
		}

		int IComparable<Role>.CompareTo(Role other)
		{
			return _name.CompareTo(other._name);
		}

		#endregion

		#region Public Static Methods

		public static List<int> GetLevelsList(string roleLevels)
		{
			if (!string.IsNullOrEmpty(roleLevels))
			{
				string[] sRoleLevels = roleLevels.Split(_DELIMITER);
				if (sRoleLevels.Length > 0)
				{
					int nRoleLevel;
					List<int> listRoleLevels = new List<int>(sRoleLevels.Length);
					foreach (string sRoleLevel in sRoleLevels)
					{
						if (!int.TryParse(sRoleLevel, out nRoleLevel))
							throw new Exception(string.Format("Failed to parse system role level '{0}'", sRoleLevel));
						listRoleLevels.Add(nRoleLevel);
					}
					return listRoleLevels;
				}
			}
			return null;
		}


		public static List<int> GetAllDatabaseLevels()
		{
			List<int> rtn = new List<int>();
			for (int level = _DATABASE_FACILITY_LEVEL_CORPORATE; level <= _DATABASE_FACILITY_LEVEL_LEAF; level++)
				rtn.Add(level);
			return rtn;
		}

		public static List<int> GetAllAbsoluteLevels()
		{
			List<int> rtn = new List<int>();
			for (int level = _ABSOLUTE_FACILITY_LEVEL_LEAF; level <= _ABSOLUTE_FACILITY_LEVEL_CORPORATE; level++)
				rtn.Add(level);
			return rtn;
		}


		public static void SetRole(Role role, string roleType, string roleLevelType, string roleLevels)
		{
			//If the roleType is not specified (older system xml), then assume it is of type equivalent
			//to all of the possiblities, and encompasses all of the available facility levels for distribution.
			if (string.IsNullOrEmpty(roleType))
			{
				role.RoleType = RoleType.All;
				role.RoleLevelType = RoleLevelType.Absolute;
				role.RoleLevels = new List<int>(_FACILITY_LEVELS);
				for (int level = 0; level < _FACILITY_LEVELS; level++)
					role.RoleLevels.Add(level);
			}
			else
			{
				role.RoleType = (RoleType)Enum.Parse(typeof(RoleType), roleType);
				role.RoleLevelType = RoleLevelType.Unknown;
				role.RoleLevels = null;

				if (!string.IsNullOrEmpty(roleLevelType))
				{
					role.RoleLevelType = (RoleLevelType)Enum.Parse(typeof(RoleLevelType), roleLevelType);
					if (role.IsDistribution && !string.IsNullOrEmpty(roleLevels))
					{
						role.RoleLevels = GetLevelsList(roleLevels);
					}
				}
			}
		}

		public static List<int> CombineList(List<int> listBase, List<int> listAdd)
		{
			List<int> listCombine = new List<int>();
			if (listBase != null)
			{
				foreach (int nItem in listBase)
					if (!listCombine.Contains(nItem))
						listCombine.Add(nItem);
			}
			if (listAdd != null)
			{
				foreach (int nItem in listAdd)
					if (!listCombine.Contains(nItem))
						listCombine.Add(nItem);
			}
			listCombine.Sort();
			return listCombine;
		}

		/*
 *            Database   Abs Role
 *              Level      Level
 *    Facility    6          0          (LEAF level) 		
 *    District    5          1
 *    Region      4          2
 *    LOB         3          3
 *    Division    2          4
 *    Corporate   1          5          (CORPORATE level)
 */


		public static int DatabaseToAbsoluteLevel(int databaseLevel)
		{
			return _DATABASE_FACILITY_LEVEL_CORPORATE - databaseLevel + _ABSOLUTE_FACILITY_LEVEL_CORPORATE;
		}

		public static int AbsoluteToDatabaseLevel(int absoluteLevel)
		{
			return _ABSOLUTE_FACILITY_LEVEL_CORPORATE - absoluteLevel + _DATABASE_FACILITY_LEVEL_CORPORATE;
		}

		public static List<int> AbsoluteToDatabaseLevel(List<int> listBase)
		{
			List<int> listBaseDatabase = new List<int>(listBase.Count);
			foreach (int nLevel in listBase)
			{
				listBaseDatabase.Add(AbsoluteToDatabaseLevel(nLevel));
			}
			return listBaseDatabase;
		}

		public static List<int> RelativeToDatabaseLevel(int nFacilityLevelDatabase, List<int> listRelative)
		{
			List<int> listDatabase = new List<int>(listRelative.Count);
			foreach (int nLevel in listRelative)
			{
				int newLevel = nFacilityLevelDatabase - nLevel;
				//Assumption: we want to treat anything "lower" in the facility hiearchy than a leaf-level facility (e.g., Hospital, Nursing Center) the SAME as a leaf-level facility.
				if (newLevel > _DATABASE_FACILITY_LEVEL_LEAF)
					newLevel = _DATABASE_FACILITY_LEVEL_LEAF;
				//Assumption: we want to treat anything "higher" in the facility hiearchy than the top-most facility (i.e., Corporate) the SAME as Corporate.
				if (newLevel < _DATABASE_FACILITY_LEVEL_CORPORATE)
					newLevel = _DATABASE_FACILITY_LEVEL_CORPORATE;
				listDatabase.Add(newLevel);
			}
			return listDatabase;
		}


		public static Role FindRole(List<Role> roles, string name)
		{
			Predicate<Role> p = delegate(Role r) { return (r.Name == name); };
			return roles.Find(p);
		}

        public static List<Role> FromNames(List<string> roleNames)
        {
            List<Role> roles = new List<Role>();
            if (roleNames != null)
            {
                foreach (string roleName in roleNames)
                    roles.Add(new Role(roleName));
            }
            return roles;
        }
 
		#endregion

	}
}
