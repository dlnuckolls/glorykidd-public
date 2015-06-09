using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;

//20090429
namespace Kindred.Knect.ITAT.Business
{
    public enum SecurityModel
    {
        Basic = 0,
        Advanced
    }

	[Serializable]
	public class TermGroup
	{
        public const string _TERM_GROUP_BASIC_NAME = "Profile";

        public enum TermGroupType
        {
            None,
            BasicSecurity,
            AdvancedBasicTerm,
            AdvancedComplexList
        }

		#region private members

        private readonly Guid _id;
        private string _name;
        private string _description;
        private readonly TermGroupType _type;
        private int _order = 0; //Used to assist in sorting - not saved back to the xml

		#endregion

		#region Properties

        public Guid ID
        {
            get { return _id; }
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

        public TermGroupType Type
        {
            get { return _type; }
        }

        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }

        #endregion

		#region Constructors 

        public TermGroup(string name, string description, SecurityModel securityModel, TermGroupType type)
        {
            _id = Guid.NewGuid();
            switch (securityModel)
            {
                case SecurityModel.Advanced:
                    _name = name;
                    break;
                case SecurityModel.Basic:
                    _name = _TERM_GROUP_BASIC_NAME;
                    break;
            }
            _description = description;
            _type = type;
        }

        public TermGroup(XmlNode node)
		{
            _id = new Guid(Utility.XMLHelper.GetAttributeString(node, XMLNames._A_ID));
            _name = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Name);
            _description = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Description);
			//TODO: can remove the try/catch when v1.6 is complete if desired
			try { _type = (TermGroupType)Enum.Parse(typeof(TermGroupType), Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Type)); }
			catch { _type = TermGroupType.BasicSecurity; }
        }

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
            if (bValidate)
            {
            }

            Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_ID, _id.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, _name);
            Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Description, _description);
            Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Type, _type.ToString());
        }

		#endregion

        public static bool ValidName(string name, SecurityModel securityModel)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            switch (securityModel)
            {
                case SecurityModel.Advanced:
                    return !name.Equals(_TERM_GROUP_BASIC_NAME);
                case SecurityModel.Basic:
                    return (name.Equals(_TERM_GROUP_BASIC_NAME));
                default:
                    return false;
            }
		}

		#region base class overrides

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion

		#region Equality overrides

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			TermGroup objTermGroup = obj as TermGroup;
			if (objTermGroup == null)
				return false;
			return objTermGroup.ID.Equals(_id);
		}

		public bool Equals(TermGroup other)
		{
            if ((object)other == null)
                return false;
			return ID.Equals(other.ID);
		}
		
		public static bool Equals(TermGroup a, TermGroup b)
		{
			if ((object)a == null)
			    return ((object)b == null);
			if ((object)b == null)
			    return false;
            return a.ID.Equals(b.ID);
		}

		public static bool operator==(TermGroup a, TermGroup b)
		{
			return TermGroup.Equals(a, b);
		}

		public static bool operator !=(TermGroup a, TermGroup b)
		{
			return !TermGroup.Equals(a, b);
        }



		#endregion


	}


}
