using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	public static class DetailedDescriptions
	{
		//Generate the DetailedDescriptions collection (of N) based on an existing Template
		public static List<DetailedDescription> Create(string templateDef)
		{
			if (templateDef == null)
				return null;
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

			return Create(xmlTemplateDoc);
		}

		//Generate the DetailedDescriptions collection (of N) based on an existing Template
		public static List<DetailedDescription> Create(XmlDocument xmlTemplateDoc)
		{
			List<DetailedDescription> detailedDescriptions = new List<DetailedDescription>();

			XmlNodeList nodeDetailedDescriptions = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_DetailedDescriptions, XMLNames._E_DetailedDescription));
			if (nodeDetailedDescriptions != null)
			{
				foreach (XmlNode nodeDetailedDescription in nodeDetailedDescriptions)
				{
					DetailedDescription detailedDescription = new DetailedDescription(nodeDetailedDescription);
					detailedDescriptions.Add(detailedDescription);
				}
			}
			return detailedDescriptions;
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
			XmlElement nodeDetailedDescriptions = xmlDoc.CreateElement(XMLNames._E_DetailedDescriptions);

			if (template.DetailedDescriptions != null)
				foreach (DetailedDescription detailedDescription in template.DetailedDescriptions)
				{
					XmlElement nodeDetailedDescription = xmlDoc.CreateElement(XMLNames._E_DetailedDescription);
					detailedDescription.Build(xmlDoc, nodeDetailedDescription, bValidate);
					nodeDetailedDescriptions.AppendChild(nodeDetailedDescription);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeDetailedDescriptions = xmlTemplateDoc.ImportNode(nodeDetailedDescriptions, true);
			//Find the "DetailedDescriptions" child node
			XmlNode detailedDescriptionChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_DetailedDescriptions));
			if (detailedDescriptionChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeDetailedDescriptions, detailedDescriptionChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeDetailedDescriptions);

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
			XmlElement nodeDetailedDescriptions = xmlDoc.CreateElement(XMLNames._E_DetailedDescriptions);

			if (template.DetailedDescriptions != null)
				foreach (DetailedDescription detailedDescription in template.DetailedDescriptions)
				{
					XmlElement nodeDetailedDescription = xmlDoc.CreateElement(XMLNames._E_DetailedDescription);
					detailedDescription.Build(xmlDoc, nodeDetailedDescription, bValidate);
					nodeDetailedDescriptions.AppendChild(nodeDetailedDescription);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeDetailedDescriptions = xmlTemplateDoc.ImportNode(nodeDetailedDescriptions, true);
			//Find the "DetailedDescriptions" child node
			XmlNode detailedDescriptionChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_DetailedDescriptions));
			if (detailedDescriptionChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeDetailedDescriptions, detailedDescriptionChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeDetailedDescriptions);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

			return true;
		}

	}
}
