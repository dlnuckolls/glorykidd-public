using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable, System.Diagnostics.DebuggerDisplay("Name={Name}")]
	public class Status
	{

		#region private members
		
		private string _name;
		private bool _viewableByAllRoles;
		List<string> _viewingRoles;  //roles that can view and search this status

		#endregion

		#region Properties

		/// <summary>
		/// Name of this status
		/// </summary>
		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}


		/// <summary>
		/// Flag indicating whether this status can be viewed and searched by all roles.  If true, then the ViewingRoles collection is ignored.
		/// </summary>
		public bool ViewableByAllRoles
		{
			get { return _viewableByAllRoles; }
			set { _viewableByAllRoles = value; }
		}

        /// <summary>
		/// Roles that can view and search this status
		/// </summary>
		public List<string> ViewingRoles
		{
			get { return _viewingRoles; }
			set { _viewingRoles = value; }
		}

		#endregion

		#region Constructors

		public Status()
		{
			_viewingRoles = new List<string>();
		}

		public Status(XmlNode statusNode)
		{
			_name = Utility.XMLHelper.GetAttributeString(statusNode, XMLNames._A_Name);
			_viewableByAllRoles = Utility.XMLHelper.GetAttributeBool(statusNode, XMLNames._A_AllRoles) ?? false;

			XmlNodeList listRoles = statusNode.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Roles, XMLNames._E_Role));
			if (listRoles != null)
			{
				_viewingRoles = new List<string>(listRoles.Count);
				foreach (XmlNode nodeRole in listRoles)
				{
					string roleName = Utility.XMLHelper.GetAttributeString(nodeRole, XMLNames._A_Name);
					_viewingRoles.Add(roleName);
				}
			}
			else
			{
				_viewingRoles = new List<string>();
			}

		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
				Utility.XMLHelper.ValidateString("Name", _name);
			}
			Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_Name, _name);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, termNode, XMLNames._A_AllRoles, _viewableByAllRoles);

			XmlNode elementRoles = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Roles, XMLNames._M_NameSpaceURI);
			foreach (string roleName in _viewingRoles)
			{
				XmlNode elementRole = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Role, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeString(xmlDoc, elementRole, XMLNames._A_Name, roleName);
				elementRoles.AppendChild(elementRole);
			}
			termNode.AppendChild(elementRoles);

		}

		#endregion


		//This call is used to generate an xml format for database updates
		public static string GetXml(List<string> statuses)
		{
			if (statuses == null)
				return null;

			if (statuses.Count == 0)
				return null;

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<?xml version='1.0' encoding='UTF-8' ?><Statuses />");
			XmlElement elementRoot = xmlDoc.DocumentElement;

			foreach (string status in statuses)
			{
				XmlNode nodeStatus = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Status, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeString(xmlDoc, nodeStatus, XMLNames._A_Name, status);
				elementRoot.AppendChild(nodeStatus);
			}

			return xmlDoc.OuterXml;
		}



	}
}
