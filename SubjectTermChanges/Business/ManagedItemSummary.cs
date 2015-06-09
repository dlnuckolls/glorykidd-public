using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace Kindred.Knect.ITAT.Business
{
	public class ManagedItemSummary
	{

		#region private fields
		private ManagedItem _managedItem;
		#endregion


		#region constructors

		public ManagedItemSummary(ManagedItem managedItem)
		{
			_managedItem = managedItem;
		}

		#endregion


		#region public methods

		public string CreateXml(string systemManagedItemName, List<string> userRoles)
		{
			//Initialize writers
			using (Utility.StringWriterWithEncoding sw = new Utility.StringWriterWithEncoding(Encoding.UTF8))
			{
				using (System.Xml.XmlTextWriter writer = new XmlTextWriter(sw))
				{
					//"write" XML using writer
					writer.WriteStartDocument();
					WriteSummaryDocument(writer, systemManagedItemName, userRoles);
					writer.WriteEndDocument();
					writer.Flush();
					return sw.ToString();
				}
			}
		}

		#endregion


		#region private methods

		private void WriteSummaryDocument(XmlWriter writer, string systemManagedItemName, List<string> userRoles)
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
			writer.WriteAttributeString("top", "72");
			writer.WriteAttributeString("right", "36");
			writer.WriteAttributeString("bottom", "72");
			writer.WriteAttributeString("left", "36");
			writer.WriteEndElement();  // </margin>

			WriteSummaryDocumentBody(writer, systemManagedItemName, userRoles);
			WriteSummaryPageHeaders(writer, "odd");
			WriteSummaryPageHeaders(writer, "even");
			WriteSummaryPageFooters(writer, systemManagedItemName, "odd");
			WriteSummaryPageFooters(writer, systemManagedItemName, "even");

			writer.WriteEndElement();   // </section>
			writer.WriteEndElement();   // </sections>
			writer.WriteEndElement();   // </document>
		}

		private void WriteSummaryDocumentBody(XmlWriter writer, string systemManagedItemName, List<string> userRoles)
		{
			//<paragraphs keepwithnext="false">
			writer.WriteStartElement("paragraphs");
			writer.WriteAttributeString("keepwithnext", "false");

			//write document header
			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "textparagraph");
			writer.WriteAttributeString("horizontalalignment", "center");
			writer.WriteAttributeString("spacingbefore", "0");
			writer.WriteAttributeString("spacingafter", "12");

			writer.WriteStartElement("fragment");
			writer.WriteAttributeString("font", "timesbold");
			writer.WriteAttributeString("fontsize", "14pt");
			writer.WriteString(string.Format("{0} SUMMARY", systemManagedItemName.ToUpper()));

			writer.WriteEndElement();   // </fragment>
			writer.WriteEndElement();   // </paragraph>

			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "horizontalline");
			writer.WriteAttributeString("linewidth", "0.1");
			writer.WriteEndElement();  // </paragraph>

			PdfHelper.EmitPDFXML(writer, string.Format("{0} Number", systemManagedItemName), _managedItem.ItemNumber);
			PdfHelper.EmitPDFXML(writer, "Status", _managedItem.State.Status);
			PdfHelper.EmitPDFXML(writer, "Workflow State", _managedItem.State.Name);
			PdfHelper.EmitPDFXML(writer, "Template", _managedItem.Name);

			//WriteClause(writer);
			List<Guid> accessibleTermGroups = _managedItem.State.AccessibleTermGroups(userRoles);
            _managedItem.BasicTerms.Sort(Term.TermGroupOrderTermOrderComparison);

            foreach (Term term in _managedItem.BasicTerms)
				if (accessibleTermGroups.Contains(term.TermGroupID))
					term.EmitPDFXML(writer);

			foreach (ComplexList complexList in _managedItem.ComplexLists)
			{
				if (accessibleTermGroups.Contains(complexList.TermGroupID))
					if (complexList.ShowOnItemSummary)
						if (complexList.Runtime.Enabled)
							complexList.EmitPDFXML(writer);
			}

			WriteSummaryPageAttachments(writer);
			WriteSummaryPageComments(writer);

			writer.WriteEndElement();   // </paragraphs>
		}


		private void WriteSummaryPageComments(XmlWriter writer)
		{
			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "table");
			writer.WriteAttributeString("spacingbefore", "1");
			writer.WriteAttributeString("spacingafter", "1");
			writer.WriteStartElement("rows");

			writer.WriteStartElement("row");
			writer.WriteAttributeString("minheight", "0");

			writer.WriteStartElement("cell");
			writer.WriteAttributeString("preferredwidth", "180");
			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "textparagraph");
			writer.WriteAttributeString("horizontalalignment", "left");
			writer.WriteStartElement("fragment");
			writer.WriteAttributeString("font", "timesbold");
			writer.WriteAttributeString("fontsize", "10");
			writer.WriteString("Comments");
			writer.WriteEndElement();			// </fragment>
			writer.WriteEndElement();			// </paragraph>
			writer.WriteEndElement();			// </cell>

			writer.WriteStartElement("cell");
			writer.WriteAttributeString("preferredwidth", "380");
			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "textparagraph");
			writer.WriteAttributeString("horizontalalignment", "left");
			bool firstComment = true;
			foreach (Comment c in _managedItem.Comments)
			{
				if (!firstComment)
				{
					PdfHelper.WriteLineBreak(writer, false);
					PdfHelper.WriteLineBreak(writer, false);
				}
				string commentHeader = string.Format("{0} ({1}) - {2:MMMM dd, yyyy h:mm tt}", c.UserName, c.UserID, c.Created);
				PdfHelper.AddPDFXMLText(writer, commentHeader, true);
				PdfHelper.WriteLineBreak(writer, false);
				PdfHelper.AddPDFXMLText(writer, c.Text, false);
				firstComment = false;
			}
			writer.WriteEndElement();			// </paragraph>
			writer.WriteEndElement();			// </cell>

			writer.WriteEndElement();   // </row>
			writer.WriteEndElement();   // </rows>
			writer.WriteEndElement();   // </table>
		}


		private void WriteSummaryPageAttachments(XmlWriter writer)
		{
			PdfHelper.AddPDFXMLHeader(writer);
			if (_managedItem.Attachments.Count == 0)
			{
				PdfHelper.AddPDFXMLRow(writer, "Attachments", string.Empty, string.Empty);
			}
			else
			{
				bool firstAttachment = true;
				foreach (Attachment att in _managedItem.Attachments)
				{
					if (firstAttachment)
						PdfHelper.AddPDFXMLRow(writer, "Attachments", att.DocumentType.Name, att.Description);
					else
						PdfHelper.AddPDFXMLRow(writer, string.Empty, att.DocumentType.Name, att.Description);
					firstAttachment = false;
				}
			}
			PdfHelper.AddPDFXMLFooter(writer);
		}


		private void WriteSummaryPageHeaders(XmlWriter writer, string pageType)
		{
			// <????header>   where ???? = odd or even
			writer.WriteStartElement(pageType + "header");
			writer.WriteAttributeString("verticalalignment", "middle");

			writer.WriteEndElement();   // </????header>   where ???? = odd or even
		}

		private void WriteSummaryPageFooters(XmlWriter writer, string systemManagedItemName, string pageType)
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
			writer.WriteString(string.IsNullOrEmpty(_managedItem.ItemNumber) ? "###########" : _managedItem.ItemNumber);
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
