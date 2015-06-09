using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
    public static class DocumentPrinters
	{
		//Generate the Comments collection (of N) based on an existing Template
        public static List<Role> Create(string templateDef, Template template)
		{
			if (templateDef == null)
				return null;
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);
            return Create(xmlTemplateDoc, template);
		}

		//Generate the Comments collection (of N) based on an existing Template
        public static List<Role> Create(XmlDocument xmlTemplateDoc, Template template)
		{
            List<Role> documentPrinters = new List<Role>();

            XmlNode nodeDocumentPrinters = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_DocumentPrinters));
            if (nodeDocumentPrinters == null)
            {
                ITATSystem system = ITATSystem.Get(template.SystemID);
                documentPrinters = new List<Role>(system.Roles);
            }
            else
			{
                XmlNodeList nodeDocumentPrintersList = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_DocumentPrinters, XMLNames._E_Role));
                foreach (XmlNode nodeDocumentPrinter in nodeDocumentPrintersList)
				{
                    Role documentPrinter = new Role(nodeDocumentPrinter);
					documentPrinters.Add(documentPrinter);
				}
			}
			return documentPrinters;
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
			XmlElement nodeDocumentPrinters = xmlDoc.CreateElement(XMLNames._E_DocumentPrinters);

			if (template.DocumentPrinters != null)
                foreach (Role documentPrinter in template.DocumentPrinters)
				{
					XmlElement nodeDocumentPrinter = xmlDoc.CreateElement(XMLNames._E_Role);
					documentPrinter.Build(xmlDoc, nodeDocumentPrinter, bValidate);
					nodeDocumentPrinters.AppendChild(nodeDocumentPrinter);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeDocumentPrinters = xmlTemplateDoc.ImportNode(nodeDocumentPrinters, true);
			//Find the "Comments" child node
			XmlNode documentPrinterChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_DocumentPrinters));
			if (documentPrinterChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeDocumentPrinters, documentPrinterChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeDocumentPrinters);

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
			XmlElement nodeDocumentPrinters = xmlDoc.CreateElement(XMLNames._E_DocumentPrinters);

			if (template.DocumentPrinters != null)
                foreach (Role documentPrinter in template.DocumentPrinters)
				{
					XmlElement nodeDocumentPrinter = xmlDoc.CreateElement(XMLNames._E_Role);
					documentPrinter.Build(xmlDoc, nodeDocumentPrinter, bValidate);
					nodeDocumentPrinters.AppendChild(nodeDocumentPrinter);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeDocumentPrinters = xmlTemplateDoc.ImportNode(nodeDocumentPrinters, true);
			//Find the "Comments" child node
			XmlNode documentPrinterChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_DocumentPrinters));
			if (documentPrinterChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeDocumentPrinters, documentPrinterChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeDocumentPrinters);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

			return true;
		}
	}
}
