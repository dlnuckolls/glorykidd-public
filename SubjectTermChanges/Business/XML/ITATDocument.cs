using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Diagnostics;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class ITATDocument
	{

        #region private members
        private ITATDocumentHeader _header;
        private ITATClause _clause;
        private ITATDocumentFooter _footer;
        private bool? _defaultDocument = false;
        private bool? _workflowEnabled = false;
        private string _documentName;
        private string _generatedDocumentID = default(string);
        private Guid _ITATDocumentID;
        #endregion

        #region Properties

        public Guid ITATDocumentID
        {
            get { return _ITATDocumentID; }
            set { _ITATDocumentID = value; }
        }


        public string DocumentName
        {
            get { return _documentName; }
            set { _documentName = value; }
        }

        public ITATDocumentHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

        public ITATClause Clause
        {
            get { return _clause; }
            set { _clause = value; }
        }

        public ITATDocumentFooter Footer
        {
            get { return _footer; }
            set { _footer = value; }
        }

        public bool? DefaultDocument
        {
            get { return _defaultDocument; }
            set { _defaultDocument = value; }
        }

        public bool? WorkflowEnabled
        {
            get { return _workflowEnabled; }
            set { _workflowEnabled = value; }
        }

        public string GeneratedDocumentID
        {
            get
            {
                return _generatedDocumentID;
            }
            set
            {
                _generatedDocumentID = value;
            }
        }


        #endregion


        #region Constructors
        public ITATDocument()
        {
            _ITATDocumentID = Guid.NewGuid();
            _documentName = string.Empty;
            _defaultDocument = false;
            _workflowEnabled = true;
            _clause = new ITATClause();
            _clause.Name = XMLNames._E_Document;
            _header = new ITATDocumentHeader();
            _footer = new ITATDocumentFooter();           
            
        }


        public ITATDocument(XmlDocument xmlTemplateDoc, XmlNode node, Template template, bool isNewDocStructure)
        {
            if (isNewDocStructure)
            {
                _ITATDocumentID = new Guid(Utility.XMLHelper.GetAttributeString(node, XMLNames._A_ID));
                _documentName = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Name);
                _defaultDocument = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Document_DefaultDocument);
                _workflowEnabled = Utility.XMLHelper.GetAttributeBool(node, XMLNames._A_Document_WorkflowEnabled);
                _generatedDocumentID = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_GeneratedDocumentID);
            }
            else
            {
                _ITATDocumentID = Guid.NewGuid();
                _documentName = Utility.XMLHelper.GetAttributeString(node.SelectSingleNode(XMLNames._E_Body), XMLNames._A_Name);
                _defaultDocument = true;
                _workflowEnabled = true;
                _generatedDocumentID = Utility.XMLHelper.GetAttributeString(xmlTemplateDoc.DocumentElement, XMLNames._A_GeneratedDocument); ;
            }


            XmlNode nodeHeader = node.SelectSingleNode(XMLNames._E_Header);
            _header = new ITATDocumentHeader(nodeHeader);

            XmlNode nodeFooter = node.SelectSingleNode(XMLNames._E_Footer);
            _footer = new ITATDocumentFooter(nodeFooter);

            XmlNode nodeBody = node.SelectSingleNode(XMLNames._E_Body);
            _clause = new ITATClause();
            _clause.Name = Utility.XMLHelper.GetAttributeString(nodeBody, XMLNames._A_Name);
            _clause.ChildNumberingScheme = ChildNumberingSchemeHelper.GetSchemeType(Utility.XMLHelper.GetAttributeString(nodeBody, XMLNames._A_ChildNumberingScheme));
            _clause.Create(nodeBody);
        }
        #endregion

        #region Build XML

        public void Build(XmlDocument xmlTemplateDoc, XmlDocument xmlDoc, XmlNode nodeDocument, bool bValidate)
        {
            Utility.XMLHelper.AddAttributeString(xmlDoc, nodeDocument, XMLNames._A_ID, _ITATDocumentID.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, nodeDocument, XMLNames._A_Name, _documentName);
            Utility.XMLHelper.AddAttributeString(xmlDoc, nodeDocument, XMLNames._A_Document_DefaultDocument, _defaultDocument.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, nodeDocument, XMLNames._A_Document_WorkflowEnabled, _workflowEnabled.ToString());
            Utility.XMLHelper.AddAttributeString(xmlDoc, nodeDocument, XMLNames._A_GeneratedDocumentID, _generatedDocumentID);
            xmlTemplateDoc.DocumentElement.RemoveAttribute(XMLNames._A_GeneratedDocument);

            XmlElement nodeHeader = xmlDoc.CreateElement(XMLNames._E_Header);
            _header.Build(xmlDoc, nodeHeader, bValidate);
            nodeDocument.AppendChild(nodeHeader);


            XmlElement nodeFooter = xmlDoc.CreateElement(XMLNames._E_Footer);
            _footer.Build(xmlDoc, nodeFooter, bValidate);
            nodeDocument.AppendChild(nodeFooter);

            XmlElement nodeBody = xmlDoc.CreateElement(XMLNames._E_Body);
            _clause.Build(xmlDoc, nodeBody, _clause, bValidate);
            Utility.XMLHelper.AddAttributeString(xmlDoc, nodeBody, XMLNames._A_Name, _clause.Name);
            Utility.XMLHelper.AddAttributeString(xmlDoc, nodeBody, XMLNames._A_ChildNumberingScheme, ChildNumberingSchemeHelper.GetSchemeText(_clause.ChildNumberingScheme));
            nodeDocument.AppendChild(nodeBody);

        }


        public static bool UpdateGeneratedDocument(XmlElement node, string documentId, Guid ITATDocumentID)
        {
            string itatDocumentID = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_ID);

            if (!string.IsNullOrEmpty(itatDocumentID))
            {
                if (new Guid(itatDocumentID).Equals(ITATDocumentID))
                {
                    node.SetAttribute(Business.XMLNames._A_GeneratedDocumentID, documentId);
                    return true;
                }
            }

            return false;
        }

        #endregion


        /// <summary>
        /// Returns an ITATClause object from within the Document.Clause hierarchy
        /// </summary>
        /// <param name="clausePath">XPath-like string (delimited by pathSeparator) indicating the path of the desired clause</param>
        /// <returns>an ITATClause object (or null if no matching clause is found)</returns>
        public ITATClause FindClause(string clausePath, char pathSeparator)
        {
            //pathSegments[0] is always the Document "root" clause.			
            ITATClause rtn = this._clause;
            if (string.IsNullOrEmpty(clausePath))
                return rtn;
            string[] pathSegments = clausePath.Split(pathSeparator);
            if (pathSegments.Length == 1)  //Document "root" clause selected which is _template.Document.Clause
                return rtn;
            for (int i = 1; i < pathSegments.Length; i++)
            {
                if (rtn == null)
                    break;
                rtn = rtn.ChildClause(pathSegments[i]);
            }
            return rtn;

        }


        #region "Preview" methods

        public string CreateXml(bool isPreview, string systemManagedItemName, string managedItemNumber, Template template, Guid ITATDocumentID)
        {
            //Initialize writers
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    Clause.SubstituteImages(template);

                    //"write" XML using writer
                    writer.WriteStartDocument();
                    WriteDocument(writer, systemManagedItemName, managedItemNumber, template, ITATDocumentID);
                    writer.WriteEndDocument();
                    return sw.ToString();
                }
            }
        }



        private void WriteDocument(XmlWriter writer, string systemManagedItemName, string managedItemNumber, Template template, Guid ITATDocumentID)
        {
            //document (root node)
            writer.WriteStartElement("document");
            writer.WriteAttributeString("xmlns", "", null, XMLNames._M_TallComponents_URL);

            //	<sections>
            writer.WriteStartElement("sections");

            //	<section>
            writer.WriteStartElement("section");

            //	<margin bottom="" top="" left="" right="" />
            writer.WriteStartElement("margin");
            writer.WriteAttributeString("top", PdfHelper.TOP_MARGIN.ToString());
            writer.WriteAttributeString("right", PdfHelper.RIGHT_MARGIN.ToString());
            writer.WriteAttributeString("bottom", PdfHelper.BOTTOM_MARGIN.ToString());
            writer.WriteAttributeString("left", PdfHelper.LEFT_MARGIN.ToString());
            writer.WriteEndElement();  // </margin>

            WriteDocumentBody(writer, template, ITATDocumentID);
            WritePageHeaders(writer, "odd");
            WritePageHeaders(writer, "even");
            WritePageFooters(writer, systemManagedItemName, managedItemNumber, "odd");
            WritePageFooters(writer, systemManagedItemName, managedItemNumber, "even");

            writer.WriteEndElement();   // </section>
            writer.WriteEndElement();   // </sections>
            writer.WriteEndElement();   // </document>
        }


        private void WriteDocumentBody(XmlWriter writer, Template template, Guid ITATDocumentID)
        {
            //<paragraphs keepwithnext="false">
            writer.WriteStartElement("paragraphs");
            writer.WriteAttributeString("keepwithnext", "false");

            //Changed for Multiple Documents
            ITATDocument document = template.GetITATDocument(ITATDocumentID);
            
            int paragraphNumber = 0;
            for (int i = 0; i < document.Clause.Children.Count; i++)
            {
                ITATClause childClause = Clause.Children[i];
                if (childClause.ShouldShow(template))
                {
                    paragraphNumber++;
                    Clause.Children[i].WriteClause(writer, 0, Clause.ChildNumberingScheme, paragraphNumber, template);
                }
            }
            writer.WriteEndElement();   // </paragraphs>
        }




        private void WritePageHeaders(XmlWriter writer, string pageType)
        {
            // <????header>   where ???? = odd or even
            writer.WriteStartElement(pageType + "header");
            writer.WriteAttributeString("verticalalignment", "middle");

            writer.WriteEndElement();   // </????header>   where ???? = odd or even
        }


        private void WritePageFooters(XmlWriter writer, string systemManagedItemName, string managedItemNumber, string pageType)
        {
            // <????footer>
            writer.WriteStartElement(pageType + "footer");
            writer.WriteAttributeString("verticalalignment", "middle");

            // <paragraph type="table">
            writer.WriteStartElement("paragraph");
            writer.WriteAttributeString("type", "table");

            writer.WriteStartElement("rows");
            writer.WriteStartElement("row");

            writer.WriteStartElement("cell");
            writer.WriteAttributeString("preferredwidth", "200");
            writer.WriteStartElement("paragraph");
            writer.WriteAttributeString("type", "textparagraph");
            writer.WriteAttributeString("horizontalalignment", "left");
            //<fragment>Item Number: ###########</fragment>
            writer.WriteStartElement("fragment");
            writer.WriteAttributeString("type", "LineBreakFragment");
            writer.WriteEndElement(); //fragment
            writer.WriteStartElement("fragment");
            writer.WriteAttributeString("font", "timesroman");
            writer.WriteAttributeString("fontsize", "12");
            writer.WriteString(systemManagedItemName + " Number: ");
            writer.WriteString(string.IsNullOrEmpty(managedItemNumber) ? "###########" : managedItemNumber);
            writer.WriteEndElement();   // </fragment>
            writer.WriteEndElement();   // </paragraph>
            writer.WriteEndElement();   // </cell>

            //<<fragment hascontextfields="true">Page #p of #P</fragment>
            writer.WriteStartElement("cell");
            writer.WriteAttributeString("preferredwidth", "140");
            writer.WriteStartElement("paragraph");
            writer.WriteAttributeString("type", "textparagraph");
            writer.WriteAttributeString("horizontalalignment", "center");
            writer.WriteStartElement("fragment");
            writer.WriteAttributeString("type", "LineBreakFragment");
            writer.WriteEndElement(); //fragment
            writer.WriteStartElement("fragment");
            writer.WriteAttributeString("font", "timesroman");
            writer.WriteAttributeString("fontsize", "12");
            writer.WriteAttributeString("hascontextfields", "true");
            writer.WriteString("Page #p of #P");
            writer.WriteEndElement();   // </fragment>
            writer.WriteEndElement();   // </paragraph>
            writer.WriteEndElement();   // </cell>

            //<<fragment hascontextfields="true">{Date}</fragment>
            writer.WriteStartElement("cell");
            writer.WriteAttributeString("preferredwidth", "200");
            writer.WriteStartElement("paragraph");
            writer.WriteAttributeString("type", "textparagraph");
            writer.WriteAttributeString("horizontalalignment", "right");
            writer.WriteStartElement("fragment");
            writer.WriteAttributeString("type", "LineBreakFragment");
            writer.WriteEndElement(); //fragment
            writer.WriteStartElement("fragment");
            writer.WriteAttributeString("font", "timesroman");
            writer.WriteAttributeString("fontsize", "12");
            writer.WriteString(DateTime.Today.ToString("dddd, MMMM dd, yyyy"));
            writer.WriteEndElement();   // </fragment>
            writer.WriteEndElement();   // </paragraph>
            writer.WriteEndElement();   // </cell>

            writer.WriteEndElement();   // </row>
            writer.WriteEndElement();   // </rows>

            writer.WriteEndElement();   // </paragraph>

            writer.WriteEndElement();   // </????footer>
        }

        #endregion
    }
}
