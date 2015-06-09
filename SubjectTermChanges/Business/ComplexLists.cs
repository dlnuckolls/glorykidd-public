using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public static class ComplexLists
	{
		//Generate the Terms collection (of Term) based on an existing Template
        public static List<Term> Create(string templateDef, Template template)
		{
			if (templateDef == null)
				return null;

			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

            return Create(xmlTemplateDoc, template);
		}

		//Generate the Terms collection (of Term) based on an existing Template
		public static List<Term> Create(XmlDocument xmlTemplateDoc, Template template)
		{
			XmlNodeList nodeComplexLists = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_ComplexLists, XMLNames._E_ComplexList));
			if (nodeComplexLists == null)
			{
				return new List<Term>();
			}

			List<Term> rtn = new List<Term>(nodeComplexLists.Count);
			foreach (XmlNode nodeComplexList in nodeComplexLists)
			{
                ComplexList complexList = new ComplexList(nodeComplexList, template, false);
				rtn.Add(complexList);
			}

			return rtn;
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
			XmlElement rootComplexList = xmlDoc.CreateElement(XMLNames._E_ComplexLists);

			if (template.ComplexLists != null)
				foreach (ComplexList complexList in template.ComplexLists)
				{
					XmlElement elementComplexList = xmlDoc.CreateElement(XMLNames._E_ComplexList);
					complexList.Build(xmlDoc, elementComplexList, bValidate);
					rootComplexList.AppendChild(elementComplexList);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedRootComplexList = xmlTemplateDoc.ImportNode(rootComplexList, true);
			//Find the "Terms" child node
			XmlNode complexListsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_ComplexLists));
			if (complexListsChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedRootComplexList, complexListsChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedRootComplexList);

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
			XmlElement rootComplexList = xmlDoc.CreateElement(XMLNames._E_ComplexLists);

			if (template.ComplexLists != null)
				foreach (ComplexList complexList in template.ComplexLists)
				{
					XmlElement elementComplexList = xmlDoc.CreateElement(XMLNames._E_ComplexList);
					complexList.Build(xmlDoc, elementComplexList, bValidate);
					rootComplexList.AppendChild(elementComplexList);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedRootComplexList = xmlTemplateDoc.ImportNode(rootComplexList, true);
			//Find the "Terms" child node
			XmlNode complexListsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_ComplexLists));
			if (complexListsChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedRootComplexList, complexListsChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedRootComplexList);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

			return true;
		}

		public static List<string> TermReferences(Template template, string termName, List<Term> complexLists)
		{
			List<string> rtn = new List<string>();
			foreach (ComplexList complexList in complexLists)
			{
				rtn.AddRange(Business.Term.TermReferences(template, termName, complexList.StandardHeader, complexList.Name, ""));
				rtn.AddRange(Business.Term.TermReferences(template, termName, complexList.AlternateHeader, complexList.Name, ""));
			}
			return Utility.ListHelper.EliminateDuplicates<string>(rtn);
		}

		//Note - this call will not work with embedded term id's.
		public static List<string> TermReferences(Template template, string sHTMLText)
		{
			string decodedText = System.Web.HttpUtility.HtmlDecode(sHTMLText);
			//match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
			List<string> rtn = new List<string>();
			//Assume that the caller has already decoded the text.
			if (!string.IsNullOrEmpty(decodedText))
			{
				MatchCollection matches = Regex.Matches(decodedText, XMLNames._M_TermImageTemplate);
				foreach (Match match in matches)
				{
                    rtn.Add(match.Groups[1].Value);     //Note - 20111105 - Changed from match.Groups[2].Value
                }
			}
			return rtn;
		}

		//TODO - This code is similar to the 'SubstituteTerms' code found in ITATClause
		//May not be able to assume that all "img" nodes are complex fields.
		public static int ReplaceComplexTermNames(string oldTermName, string newTermName, ref string sHTMLText)
		{
			List<string> names = new List<string>();

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.PreserveWhitespace = false;
			xmlDoc.LoadXml(sHTMLText);

			XmlElement root = xmlDoc.DocumentElement;

			int nCount = 0;
			ReplaceComplexTermName(root, oldTermName, newTermName, ref nCount);

			sHTMLText = xmlDoc.OuterXml;

			return nCount;
		}

		private static bool ReplaceComplexTermName(XmlNode node, string oldTermName, string newTermName, ref int nCount)
		{
			if (node.Attributes != null)
			{
				if (node.Attributes["src"] != null)
				{
					string sSource = node.Attributes["src"].Value;
					string[] sParts = sSource.Split('=');
					if (sParts[1] == oldTermName)
					{
						sSource = string.Concat(sParts[0],"=",newTermName);
						node.Attributes["src"].Value = sSource;
						nCount++;
					}
				}
			}

			foreach (XmlNode nodeChild in node.ChildNodes)
			{
				ReplaceComplexTermName(nodeChild, oldTermName, newTermName, ref nCount);
			}
			return true;
		}

		public static void SubstituteTermNames(Template template)
		{
			foreach (ComplexList complexList in template.ComplexLists)
			{
				complexList.StandardHeader = Term.SubstituteTermNames(template, complexList.StandardHeader);
				complexList.AlternateHeader = Term.SubstituteTermNames(template, complexList.AlternateHeader);
			}
		}
	
	}
}
