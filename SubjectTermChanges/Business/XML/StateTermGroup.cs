using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class StateTermGroup
	{
        public enum StateTermGroupRoleType
        {
            Editor,
            Viewer,
            AttachmentRemover,
            ScannedAttachmentRemover
        }

		#region private members
		
        private Guid _termGroupID;
		private List<Role> _editors;
        private List<Role> _viewers;
        private List<Role> _attachmentRemovers;
        private List<Role> _scannedAttachmentRemovers;

		#endregion

		#region Properties

        public Guid TermGroupID
        {
            get { return _termGroupID; }
            set { _termGroupID = value; }
        }
        
        public List<Role> Editors
		{
			get { return _editors; }
			set { _editors = value; }
		}

        public List<Role> Viewers
        {
            get { return _viewers; }
            set { _viewers = value; }
        }

        public List<Role> AttachmentRemovers
        {
            get { return _attachmentRemovers; }
            set { _attachmentRemovers = value; }
        }

        public List<Role> ScannedAttachmentRemovers
        {
            get { return _scannedAttachmentRemovers; }
            set { _scannedAttachmentRemovers = value; }
        }
        
        #endregion

		#region Constructors

        public StateTermGroup(Guid termGroupID)
		{
            _termGroupID = termGroupID;
			_editors = new List<Role>();
			_viewers = new List<Role>();
			_attachmentRemovers = new List<Role>();
			_scannedAttachmentRemovers = new List<Role>();
		}

        public StateTermGroup(XmlNode termNode)
		{
            _termGroupID = new Guid(Utility.XMLHelper.GetAttributeString(termNode, XMLNames._A_TermGroupID));
            _editors = ReadRoles(termNode, XMLNames._E_Editors, XMLNames._E_Editor);
            _viewers = ReadRoles(termNode, XMLNames._E_Viewers, XMLNames._E_Viewer);
            _attachmentRemovers = ReadRoles(termNode, XMLNames._E_AttachmentRemovers, XMLNames._E_AttachmentRemover);
            _scannedAttachmentRemovers = ReadRoles(termNode, XMLNames._E_ScannedAttachmentRemovers, XMLNames._E_ScannedAttachmentRemover);
		}

        private List<Role> ReadRoles(XmlNode termNode, string collectionName, string editorName)
        {
            List<Role> roles = null;
            XmlNodeList listRoles = termNode.SelectNodes(Utility.XMLHelper.GetXPath(false, collectionName, editorName));
            if (listRoles != null)
            {
                roles = new List<Role>(listRoles.Count);
                foreach (XmlNode nodeRole in listRoles)
                {
                    Role role = new Role();
                    role.Name = Utility.XMLHelper.GetAttributeString(nodeRole, XMLNames._A_Role);
                    roles.Add(role);
                }
            }
            else
            {
                roles = new List<Role>();
            }
            return roles;
        }

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode termNode, bool bValidate)
		{
			if (bValidate)
			{
			}

            Utility.XMLHelper.AddAttributeString(xmlDoc, termNode, XMLNames._A_TermGroupID, _termGroupID.ToString());

            BuildRoles(xmlDoc, ref termNode, _editors, XMLNames._E_Editors, XMLNames._E_Editor);
            BuildRoles(xmlDoc, ref termNode, _viewers, XMLNames._E_Viewers, XMLNames._E_Viewer);
            BuildRoles(xmlDoc, ref termNode, _attachmentRemovers, XMLNames._E_AttachmentRemovers, XMLNames._E_AttachmentRemover);
            BuildRoles(xmlDoc, ref termNode, _scannedAttachmentRemovers, XMLNames._E_ScannedAttachmentRemovers, XMLNames._E_ScannedAttachmentRemover);
		}

        private void BuildRoles(XmlDocument xmlDoc, ref XmlNode termNode, List<Role> listRoles, string collectionName, string editorName)
        {
            XmlNode elementRoles = xmlDoc.CreateNode(XmlNodeType.Element, collectionName, XMLNames._M_NameSpaceURI);
            foreach (Role role in listRoles)
            {
                XmlNode elementRole = xmlDoc.CreateNode(XmlNodeType.Element, editorName, XMLNames._M_NameSpaceURI);
                Utility.XMLHelper.AddAttributeString(xmlDoc, elementRole, XMLNames._A_Role, role.Name);
                elementRoles.AppendChild(elementRole);
            }
            termNode.AppendChild(elementRoles);
        }

		#endregion

        public StateTermGroup Copy()
        {
            StateTermGroup stateTermGroup = new StateTermGroup(TermGroupID);
            foreach (Role role in Editors)
            {
                stateTermGroup.Editors.Add(role.Copy());
            }
            foreach (Role role in Viewers)
            {
                stateTermGroup.Viewers.Add(role.Copy());
            }
            foreach (Role role in AttachmentRemovers)
            {
                stateTermGroup.AttachmentRemovers.Add(role.Copy());
            }
            foreach (Role role in ScannedAttachmentRemovers)
            {
                stateTermGroup.ScannedAttachmentRemovers.Add(role.Copy());
            }
            return stateTermGroup;
        }
	}
}
