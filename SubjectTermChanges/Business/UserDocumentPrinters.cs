using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
    public static class UserDocumentPrinters
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
            List<Role> userDocumentPrinters = new List<Role>();

            XmlNode nodeUserDocumentPrinters = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_UserDocumentPrinters));
            if (nodeUserDocumentPrinters == null)
            {
                ITATSystem system = ITATSystem.Get(template.SystemID);
                userDocumentPrinters = new List<Role>(system.Roles);
            }
            else
            {
                XmlNodeList nodeUserDocumentPrintersList = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_UserDocumentPrinters, XMLNames._E_Role));
                foreach (XmlNode nodeUserDocumentPrinter in nodeUserDocumentPrintersList)
                {
                    Role userDocumentPrinter = new Role(nodeUserDocumentPrinter);
                    userDocumentPrinters.Add(userDocumentPrinter);
                }
            }
            return userDocumentPrinters;
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
            XmlElement nodeUserDocumentPrinters = xmlDoc.CreateElement(XMLNames._E_UserDocumentPrinters);

            if (template.UserDocumentPrinters != null)
                foreach (Role userDocumentPrinter in template.UserDocumentPrinters)
                {
                    XmlElement nodeUserDocumentPrinter = xmlDoc.CreateElement(XMLNames._E_Role);
                    userDocumentPrinter.Build(xmlDoc, nodeUserDocumentPrinter, bValidate);
                    nodeUserDocumentPrinters.AppendChild(nodeUserDocumentPrinter);
                }

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeUserDocumentPrinters = xmlTemplateDoc.ImportNode(nodeUserDocumentPrinters, true);
            //Find the "Comments" child node
            XmlNode userDocumentPrinterChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_UserDocumentPrinters));
            if (userDocumentPrinterChildNode != null)
                xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeUserDocumentPrinters, userDocumentPrinterChildNode);
            else
                xmlTemplateDoc.DocumentElement.AppendChild(importedNodeUserDocumentPrinters);

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
            XmlElement nodeUserDocumentPrinters = xmlDoc.CreateElement(XMLNames._E_UserDocumentPrinters);

            if (template.UserDocumentPrinters != null)
                foreach (Role userDocumentPrinter in template.UserDocumentPrinters)
                {
                    XmlElement nodeUserDocumentPrinter = xmlDoc.CreateElement(XMLNames._E_Role);
                    userDocumentPrinter.Build(xmlDoc, nodeUserDocumentPrinter, bValidate);
                    nodeUserDocumentPrinters.AppendChild(nodeUserDocumentPrinter);
                }

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeUserDocumentPrinters = xmlTemplateDoc.ImportNode(nodeUserDocumentPrinters, true);
            //Find the "Comments" child node
            XmlNode userDocumentPrinterChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_UserDocumentPrinters));
            if (userDocumentPrinterChildNode != null)
                xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeUserDocumentPrinters, userDocumentPrinterChildNode);
            else
                xmlTemplateDoc.DocumentElement.AppendChild(importedNodeUserDocumentPrinters);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

            return true;
        }
    }
}
