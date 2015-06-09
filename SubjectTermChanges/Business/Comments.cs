using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	public static class Comments
	{
		//Generate the Comments collection (of N) based on an existing Template
		public static List<Comment> Create(string templateDef)
		{
			if (templateDef == null)
				return null;
			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

			return Create(xmlTemplateDoc);
		}

		//Generate the Comments collection (of N) based on an existing Template
		public static List<Comment> Create(XmlDocument xmlTemplateDoc)
		{
			List<Comment> comments = new List<Comment>();

			XmlNodeList nodeComments = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Comments, XMLNames._E_Comment));
			if (nodeComments != null)
			{
				foreach (XmlNode nodeComment in nodeComments)
				{
					Comment comment = new Comment(nodeComment);
					comments.Add(comment);
				}
			}
			comments.Sort();
			return comments;
		}

		public static bool Save(Template template, ref string sXml, bool bValidate)
		{
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(sXml);

            //Convert the objects stored in memory to an xmldocument
            XmlDocument xmlDoc = new XmlDocument();
			XmlElement nodeComments = xmlDoc.CreateElement(XMLNames._E_Comments);

			if (template.Comments != null)
				foreach (Comment comment in template.Comments)
				{
					XmlElement nodeComment = xmlDoc.CreateElement(XMLNames._E_Comment);
					comment.Build(xmlDoc, nodeComment, bValidate);
					nodeComments.AppendChild(nodeComment);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeComments = xmlTemplateDoc.ImportNode(nodeComments, true);
			//Find the "Comments" child node
			XmlNode commentChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_Comments));
			if (commentChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeComments, commentChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeComments);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

			return true;
		}

	}
}
