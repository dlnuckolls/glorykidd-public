using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	public static class TermGroups
	{
		//Generate the Comments collection (of N) based on an existing Template
        public static List<TermGroup> Create(string templateDef)
		{
			if (templateDef == null)
				return null;
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

			return Create(xmlTemplateDoc);
		}

		//Generate the Comments collection (of N) based on an existing Template
        public static List<TermGroup> Create(XmlDocument xmlTemplateDoc)
		{
            List<TermGroup> termGroups = new List<TermGroup>();

			XmlNodeList nodeTermGroups = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_TermGroups, XMLNames._E_TermGroup));
			if (nodeTermGroups != null)
			{
                int order = 0;
				foreach (XmlNode nodeTermGroup in nodeTermGroups)
				{
                    TermGroup termGroup = new TermGroup(nodeTermGroup);
                    termGroup.Order = order++;
                    termGroups.Add(termGroup);
				}
			}
			//termGroups.Sort();
            //If the TermGroups are not defined, then assume that Basic Security applies
            if (termGroups.Count == 0)
            {
                termGroups.Add(new TermGroup(null, String.Empty, SecurityModel.Basic, TermGroup.TermGroupType.BasicSecurity));
            }
			return termGroups;
		}

		//Generate the Terms collection (of Term) based on an xml document
		public static bool Save(XmlDocument xmlTemplateDoc, Template template, bool bValidate)
		{
            //Convert the xml into an xmldocument
            //XmlDocument xmlTemplateDoc = new XmlDocument();
            //xmlTemplateDoc.PreserveWhitespace = false;
            //xmlTemplateDoc.LoadXml(template.TemplateDef);

            //Convert the objects stored in memory to an xmldocument
            XmlDocument xmlDoc = new XmlDocument();
			XmlElement nodeTermGroups = xmlDoc.CreateElement(XMLNames._E_TermGroups);

			if (template.TermGroups != null)
                foreach (TermGroup termGroup in template.TermGroups)
				{
					XmlElement nodeTermGroup = xmlDoc.CreateElement(XMLNames._E_TermGroup);
					termGroup.Build(xmlDoc, nodeTermGroup, bValidate);
					nodeTermGroups.AppendChild(nodeTermGroup);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeTermGroups = xmlTemplateDoc.ImportNode(nodeTermGroups, true);
			//Find the "Comments" child node
			XmlNode termGroupChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_TermGroups));
			if (termGroupChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeTermGroups, termGroupChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeTermGroups);

			return true;
		}

		public static bool Save(Template template, ref string sXml, bool bValidate)
		{
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(sXml);

			//Convert the objects stored in memory to an xmldocument
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement nodeTermGroups = xmlDoc.CreateElement(XMLNames._E_TermGroups);

			if (template.TermGroups != null)
                foreach (TermGroup termGroup in template.TermGroups)
				{
					XmlElement nodeTermGroup = xmlDoc.CreateElement(XMLNames._E_TermGroup);
					termGroup.Build(xmlDoc, nodeTermGroup, bValidate);
					nodeTermGroups.AppendChild(nodeTermGroup);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeTermGroups = xmlTemplateDoc.ImportNode(nodeTermGroups, true);
			//Find the "Comments" child node
			XmlNode termGroupChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_TermGroups));
			if (termGroupChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeTermGroups, termGroupChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeTermGroups);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

			return true;
		}
	}
}
