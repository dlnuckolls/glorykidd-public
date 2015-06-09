using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	public class Extensions
	{
		//Generate the Notifications collection (of N) based on an existing Template
		public static List<Extension> Create(string templateDef)
		{
			if (templateDef == null)
				return null;
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);
			return Create(xmlTemplateDoc);
		}


		//Generate the Notifications collection (of N) based on an existing Template
		public static List<Extension> Create(XmlDocument xmlTemplateDoc)
		{
			List<Extension> extensions = new List<Extension>();
			XmlNodeList nodeExtensions = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Extensions, XMLNames._E_Extension));
			if (nodeExtensions != null)
			{
				foreach (XmlNode nodeExtension in nodeExtensions)
				{
					Extension extension = new Extension(nodeExtension);
					extensions.Add(extension);
				}
			}
			return extensions;
		}


		//Generate the Terms collection (of Term) based on an xml document
		public static bool Save(XmlDocument xmlTemplateDoc, Template template, bool bValidate)
		{
            //Convert the xml into an xmldocument
            bool bWriteToDatabase = false;
            if (xmlTemplateDoc == null)
            {
                xmlTemplateDoc = new XmlDocument();
                xmlTemplateDoc.PreserveWhitespace = false;
                xmlTemplateDoc.LoadXml(template.TemplateDef);
                bWriteToDatabase = true;
            }

            //Convert the objects stored in memory to an xmldocument
            XmlDocument xmlDoc = new XmlDocument();
			XmlElement nodeExtensions = xmlDoc.CreateElement(XMLNames._E_Extensions);

			if (template.Extensions != null)
				foreach (Extension extension in template.Extensions)
				{
					XmlElement nodeExtension = xmlDoc.CreateElement(XMLNames._E_Extension);
					extension.Build(xmlDoc, nodeExtension, bValidate);
					nodeExtensions.AppendChild(nodeExtension);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeExtensions = xmlTemplateDoc.ImportNode(nodeExtensions, true);
			//Find the "Extensions" child node
			XmlNode documentChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_Extensions));
			if (documentChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeExtensions, documentChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeExtensions);

            //Store the entire xml back to the database
            if (bWriteToDatabase)
                Template.UpdateTemplateDef(template, xmlTemplateDoc.OuterXml);
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
			XmlElement nodeExtensions = xmlDoc.CreateElement(XMLNames._E_Extensions);

			if (template.Extensions != null)
				foreach (Extension extension in template.Extensions)
				{
					XmlElement nodeExtension = xmlDoc.CreateElement(XMLNames._E_Extension);
					extension.Build(xmlDoc, nodeExtension, bValidate);
					nodeExtensions.AppendChild(nodeExtension);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeExtensions = xmlTemplateDoc.ImportNode(nodeExtensions, true);
			//Find the "Extensions" child node
			XmlNode documentChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_Extensions));
			if (documentChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeExtensions, documentChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeExtensions);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

			return true;
		}


	}
}
