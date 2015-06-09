using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Kindred.Knect.ITAT.Business
{
    public static class ITATDocuments
    {

        public static List<ITATDocument> Create(string templateDef, Template template)
        {
            if (templateDef == null)
                return null;

            XmlDocument xmlTemplateDoc = new XmlDocument();
            xmlTemplateDoc.PreserveWhitespace = false;
            xmlTemplateDoc.LoadXml(templateDef);
            return Create(xmlTemplateDoc, template);
        }


        ////This method will return a bool value that indicates if the XML has a single document (contract) or addtional documents (2.0 enhancement supports multiple documents)
        ////This will enable us to quickly find out if we are dealing with the old structure(single document) or new structure(multidocument)
        public static bool isNewStructure(XmlDocument xmlTemplateDoc)
        {
            XmlNode newRootNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Documents));
            if (newRootNode == null)
                return false;

            return true;
        }


        public static List<ITATDocument> Create(XmlDocument xmlTemplateDoc, Template template)
        {       
            XmlNodeList nodeDocuments = null;
            bool isNewDocStructure = isNewStructure(xmlTemplateDoc);


            if (isNewDocStructure)
            {
                nodeDocuments = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Documents, XMLNames._E_Document));
            }
            else
            {
                nodeDocuments = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Document));
            }


            if (nodeDocuments == null)
            {
                return new List<ITATDocument>();
            }

            List<ITATDocument> rtn = new List<ITATDocument>(nodeDocuments.Count);
            foreach (XmlNode nodeDocument in nodeDocuments)
            {
                ITATDocument document = new ITATDocument(xmlTemplateDoc, nodeDocument, template, isNewDocStructure);
                rtn.Add(document);               
                
            }

            return rtn;
        }



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
            XmlElement rootITATDocuments = xmlDoc.CreateElement(XMLNames._E_Documents);


            if (template.Documents != null)
                foreach (ITATDocument document in template.Documents)
                {
                    XmlElement elementDocument = xmlDoc.CreateElement(XMLNames._E_Document);
                    document.Build(xmlTemplateDoc, xmlDoc, elementDocument, bValidate);
                    rootITATDocuments.AppendChild(elementDocument);

                }

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedRootITATDocument = xmlTemplateDoc.ImportNode(rootITATDocuments, true);
            //Find the "Terms" child node
            XmlNode documentsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Documents));
            XmlNode documentChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Document));

            if (documentChildNode!=null)
            {
                xmlTemplateDoc.DocumentElement.RemoveChild(documentChildNode);
                xmlTemplateDoc.DocumentElement.AppendChild(importedRootITATDocument);
            }                
            else if (documentsChildNode != null)
                xmlTemplateDoc.DocumentElement.ReplaceChild(importedRootITATDocument, documentsChildNode);
            else 
                xmlTemplateDoc.DocumentElement.AppendChild(importedRootITATDocument);

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
            XmlElement rootITATDocuments = xmlDoc.CreateElement(XMLNames._E_Documents);

            //unload the documents so that they get reloaded with the latest xml on the Documents accessor.
            template.UnloadDocuments();

            if (template.Documents != null)
                foreach (ITATDocument document in template.Documents)
                {
                    XmlElement elementITATDocument = xmlDoc.CreateElement(XMLNames._E_Document);
                    document.Build(xmlTemplateDoc, xmlDoc, elementITATDocument, bValidate);
                    rootITATDocuments.AppendChild(elementITATDocument);
                }

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedRootITATDocument = xmlTemplateDoc.ImportNode(rootITATDocuments, true);
            //Find the "Terms" child node
            XmlNode documentsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Documents));
            XmlNode documentChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Document));

            if (documentChildNode != null)
            {
                xmlTemplateDoc.DocumentElement.RemoveChild(documentChildNode);
                xmlTemplateDoc.DocumentElement.AppendChild(importedRootITATDocument);
            }
            else if (documentsChildNode != null)
                xmlTemplateDoc.DocumentElement.ReplaceChild(importedRootITATDocument, documentsChildNode);
            else
                xmlTemplateDoc.DocumentElement.AppendChild(importedRootITATDocument);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

            return true;
        }



        public static List<string> TermReferences(Template template, string termName)
        {
            List<string> rtn = new List<string>();
            foreach (ITATDocument document in template.Documents)
            {                
                rtn.AddRange(document.Clause.TermReferences(template, termName));              

            }
            return Utility.ListHelper.EliminateDuplicates<string>(rtn);

        }

        public static void UpdateGeneratedDocument(XmlDocument xmlTemplateDoc, string documentId, Guid ITATDocumentID)
        {
            XmlNodeList nodeDocuments = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Documents, XMLNames._E_Document));

            if (nodeDocuments != null && nodeDocuments.Count > 0)
                foreach (XmlElement nodeDocument in nodeDocuments)
                    if (ITATDocument.UpdateGeneratedDocument(nodeDocument, documentId, ITATDocumentID))
                        return;
        }
    }
}
