using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class ITATClause
	{

		#region private members
		private string _name;
		private bool? _indentFirstParagraph;
		private bool? _indentSubsequentParagraphs;
		private bool? _hangingIndent;
		private bool? _breakParagraphs;
		private bool? _pageBreakBefore;
		private bool? _suppressSpacingBefore;
		private ChildNumberingSchemeType _childNumberingScheme;
		private string _text;       //Rich text markup from RTE
		private List<ITATClause> _children;
		private ITATClause _parent;
		private string _dependsOnTermName;
		private Guid _dependsOnTermID;		
		private string _dependsOnOperator;
		private string _dependsOnValue;


		#endregion


		#region Properties

		public string Name
		{
			get { return Utility.XMLHelper.GetXMLText(_name); }
			set { _name = Utility.XMLHelper.SetXMLText(value); }
		}

		public bool? IndentFirstParagraph
		{
			get { return _indentFirstParagraph; }
			set { _indentFirstParagraph = value; }
		}

		public bool? IndentSubsequentParagraphs
		{
			get { return _indentSubsequentParagraphs; }
			set { _indentSubsequentParagraphs = value; }
		}

		public bool? HangingIndent
		{
			get { return _hangingIndent; }
			set { _hangingIndent = value; }
		}

		public bool? BreakParagraphs
		{
			get { return _breakParagraphs; }
			set { _breakParagraphs = value; }
		}

		public bool? PageBreakBefore
		{
			get { return _pageBreakBefore; }
			set { _pageBreakBefore = value; }
		}

		public bool? SuppressSpacingBefore
		{
			get { return _suppressSpacingBefore; }
			set { _suppressSpacingBefore = value; }
		}

		public ChildNumberingSchemeType ChildNumberingScheme
		{
			get { return _childNumberingScheme; }
			set { _childNumberingScheme = value; }
		}

		public string Text
		{
			get { return Utility.XMLHelper.GetXMLText(_text); }
			set { _text = Utility.XMLHelper.SetXMLText(value); }
		}

		public List<ITATClause> Children
		{
			get { return _children; }
			set { _children = value; }
		}

		public ITATClause Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public string DependsOnTermName
		{
			get { return _dependsOnTermName; }
		}

		public Guid DependsOnTermID
		{
			get { return _dependsOnTermID; }
			set { 
					_dependsOnTermID = value;
					_dependsOnTermName = null;
				}
		}

		public string DependsOnOperator
		{
			get { return _dependsOnOperator; }
			set { _dependsOnOperator = value; }
		}

		public string DependsOnValue
		{
			get { return _dependsOnValue; }
			set { _dependsOnValue = value; }
		}


		#endregion


		#region Constructors

		public ITATClause()
		{
			_children = new List<ITATClause>();
		}

		private bool CreateNode(XmlNode clauseNode, ITATClause clause)
		{
			clause._name = Utility.XMLHelper.GetAttributeString(clauseNode, XMLNames._A_Name);
			clause._indentFirstParagraph = Utility.XMLHelper.GetAttributeBool(clauseNode, XMLNames._A_IndentFirstParagraph);
			clause._indentSubsequentParagraphs = Utility.XMLHelper.GetAttributeBool(clauseNode, XMLNames._A_IndentSubsequentParagraphs);
			clause._hangingIndent = Utility.XMLHelper.GetAttributeBool(clauseNode, XMLNames._A_HangingIndent);
			clause._breakParagraphs = Utility.XMLHelper.GetAttributeBool(clauseNode, XMLNames._A_BreakParagraphs);
			clause._pageBreakBefore = Utility.XMLHelper.GetAttributeBool(clauseNode, XMLNames._A_PageBreakBefore);
			clause._suppressSpacingBefore = Utility.XMLHelper.GetAttributeBool(clauseNode, XMLNames._A_SuppressSpacingBefore);

			clause._dependsOnTermName = Utility.XMLHelper.GetAttributeString(clauseNode, XMLNames._A_DependsOnTermName);
			clause._dependsOnTermID = Term.CreateID(clauseNode, XMLNames._A_DependsOnTermID);

			clause._dependsOnOperator = Utility.XMLHelper.GetAttributeString(clauseNode, XMLNames._A_DependsOnOperator);
			clause._dependsOnValue = Utility.XMLHelper.GetAttributeString(clauseNode, XMLNames._A_DependsOnValue);

			clause._childNumberingScheme = ChildNumberingSchemeHelper.GetSchemeType(Utility.XMLHelper.GetAttributeString(clauseNode, XMLNames._A_ChildNumberingScheme));
			clause._text = Utility.XMLHelper.GetText(clauseNode);

			return ParseDOM(clauseNode, clause);
		}

		private bool ParseDOM(XmlNode parentNode, ITATClause parentClause)
		{
			foreach (XmlNode childNode in parentNode.ChildNodes)
			{
				if (!Utility.XMLHelper.IsTextNode(childNode))
				{
					ITATClause childClause = new ITATClause();
					childClause.CreateNode(childNode, childClause);
					childClause._parent = parentClause;
					parentClause._children.Add(childClause);
				}
			}
			return true;
		}

		//Generate the ITATClause collection (of ITATClause) based on an existing Template
		public bool Create(XmlNode parentClauseNode)
		{
			return CreateNode(parentClauseNode, this);
		}

		#endregion



		#region Build XML

		public void BuildNode(XmlDocument xmlDoc, XmlNode node, ITATClause clause, bool bValidate)
		{
			if (bValidate)
			{
				if (string.IsNullOrEmpty(clause._text))
					throw new XmlException("Required ITATClause text not supplied");
			}
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Name, clause._name);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_IndentFirstParagraph, clause._indentFirstParagraph);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_IndentSubsequentParagraphs, clause._indentSubsequentParagraphs);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_HangingIndent, clause._hangingIndent);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_BreakParagraphs, clause._breakParagraphs);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_PageBreakBefore, clause._pageBreakBefore);
			Utility.XMLHelper.AddAttributeBool(xmlDoc, node, XMLNames._A_SuppressSpacingBefore, clause._suppressSpacingBefore);
			Term.StoreID(xmlDoc, node, XMLNames._A_DependsOnTermName, clause._dependsOnTermName, XMLNames._A_DependsOnTermID, clause._dependsOnTermID);

			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_DependsOnOperator, clause._dependsOnOperator);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_DependsOnValue, clause._dependsOnValue);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_ChildNumberingScheme, ChildNumberingSchemeHelper.GetSchemeText(clause._childNumberingScheme));
			Utility.XMLHelper.AddText(xmlDoc, node, clause._text);
		}

		public void Build(XmlDocument xmlDoc, XmlNode parentNode, ITATClause parentClause, bool bValidate)
		{
			foreach (ITATClause childClause in parentClause._children)
			{
				if (bValidate)
				{
					Utility.XMLHelper.ValidateString("ITATClause text", childClause._text);
				}

				XmlNode elementChildClause = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Clause, XMLNames._M_NameSpaceURI);
				BuildNode(xmlDoc, elementChildClause, childClause, bValidate);
				Build(xmlDoc, elementChildClause, childClause, bValidate);
				parentNode.AppendChild(elementChildClause);
			}
		}

		#endregion


		#region public methods

		/// <summary>
		/// Returns the child clause with the matching name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ITATClause ChildClause(string name)
		{
			Predicate<ITATClause> p = delegate(ITATClause clause) { return (clause.Name == name); };
			return _children.Find(p);
		}

		internal List<string> TermReferences(Template template, string termName)
		{
			string text = System.Web.HttpUtility.HtmlDecode(_text);
			List<string> rtn = Term.TermReferences(template, termName, text, Name, "clause");

			if (Term.MatchReference(template, _dependsOnTermID, _dependsOnTermName, Guid.Empty, termName))
				rtn.Add(string.Format("Clause \"{0}\" is dependent on this term", termName));

			//recursively call this method for each child clause
			foreach (ITATClause clause in _children)
				rtn.AddRange(clause.TermReferences(template, termName));

			return rtn;
		}


		internal void SubstituteImages(Template template)
		{
			//NOTE:	Due to limitations in the TallComponents PDF-generation tool, 
			//      there is a limit of one image per clause.
			//      So we only process the first match in the MatchCollection.
			if (!string.IsNullOrEmpty(_text) && _text.Contains("ShowImage.ashx"))
			{
				string decodedText = HttpUtility.HtmlDecode(_text);
				Match match = Regex.Match(decodedText, XMLNames._M_ImageTemplate);
				if (match.Success)
				{
					string matchImageId = match.Groups["id"].Value;
					Guid imageId = new Guid(matchImageId);
                    byte[] imageBlob = Data.ITATSystem.GetSystemImage(imageId);
                    Data.ITATSystem.UpdateSystemImageInUse(imageId, true);

					string matchImageHeight = match.Groups["height"].Value;
					string matchImageWidth = match.Groups["width"].Value;
					string matchImageHSpace = match.Groups["hspace"].Value;
					string matchImageVSpace = match.Groups["vspace"].Value;
					string element = XMLNames._A_ITATImageElement;

					if (!string.IsNullOrEmpty(matchImageWidth))
					{
						element = string.Format("{0} width=\"{1}\"", element, matchImageWidth);
					}
					if (!string.IsNullOrEmpty(matchImageHeight))
					{
						element = string.Format("{0} height=\"{1}\"", element, matchImageHeight);
					}
					if (!string.IsNullOrEmpty(matchImageHSpace))
					{
						element = string.Format("{0} hspace=\"{1}\"", element, matchImageHSpace);
					}
					if (!string.IsNullOrEmpty(matchImageVSpace))
					{
						element = string.Format("{0} vspace=\"{1}\"", element, matchImageVSpace);
					}
					decodedText = decodedText.Replace(match.Value, string.Format("<{0}>{1}</{2}>", element, Convert.ToBase64String(imageBlob), XMLNames._A_ITATImageElement));
					_text = HttpUtility.HtmlEncode(decodedText);
				}
			}
			for (int i = 0; i < _children.Count; i++)
			{
				_children[i].SubstituteImages(template);
			}
		}


		//For each term in the clause, replace the placeholder with the term's VALUE
		internal void SubstituteTerms(Template template)
		{
			SubstituteComplexLists(template);
			Term.SubstituteBasicTerms(template, ref _text);
			Term.SubstituteSpecialTerms(ref _text, template);
			for (int i = 0; i < _children.Count; i++)
			{
				_children[i].SubstituteTerms(template);
			}
		}

		public static void SubstituteTermNames(Template template, ITATClause child)
		{
			child.Text = Term.SubstituteTermNames(template, child.Text);
			for (int i = 0; i < child._children.Count; i++)
			{
				child._children[i].Text = Term.SubstituteTermNames(template, child._children[i].Text);
			}
		}

		public static void ReplaceEmbeddedTermNames(Template template, ITATClause child, List<string> sErrors)
		{
			if (!string.IsNullOrEmpty(child.DependsOnTermName))
			{
				try
				{
					child.DependsOnTermID = template.FindTerm(child.DependsOnTermName).ID;
				}
				catch (Exception)
				{
					sErrors.Add(string.Format("Unable to find Clause Dependent term '{0}'", child.DependsOnTermName));
				}
			}

			for (int i = 0; i < child._children.Count; i++)
			{
				if (!string.IsNullOrEmpty(child._children[i].DependsOnTermName))
				{
					try
					{
						child._children[i].DependsOnTermID = template.FindTerm(child._children[i].DependsOnTermName).ID;
					}
					catch (Exception)
					{
						sErrors.Add(string.Format("Unable to find Clause Dependent term '{0}'", child._children[i].DependsOnTermName));
					}
				}
			}
		}

		//returns true if the clause contains text other than terms or HTML tags
		private bool ContainsPrintableCharacters()
		{
			string clauseText = Utility.XMLHelper.GetInnerText(this.Text).Trim();
			return (!string.IsNullOrEmpty(clauseText));
		}


		private bool IsInTable(string text, string termName)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(string.Concat("<root>", text, "</root>"));
			XmlNodeList nodes = doc.SelectNodes(string.Format("//*/img[@src=\"TextImage.ashx?text={0}\"]", termName));
			foreach (XmlNode node in nodes)
			{
				XmlNode workingNode = node;
				while (workingNode != doc.DocumentElement)
				{
					if (workingNode.Name.ToLower() == "td")
						return true;
					workingNode = workingNode.ParentNode;
				}
			}
			return false;
		}


		private void SubstituteComplexLists(Template template)
		{
			int sampleListItemsOnPreview = 2;

			//First, substitute a child clause for each item in each DISTINCT complex list in this clause			
			string text = Term.SubstituteTermIDs(template, HttpUtility.HtmlDecode(_text));
			//NOTE: (RR 2-11-2010) -- added filter to only select complex lists where Runtime.Enabled==true.
			List<ComplexList> complexLists = FindComplexLists(template, text).FindAll(cl => cl.Runtime.Enabled == true);
			foreach (ComplexList complexList in complexLists)
			{
				string pattern = string.Format(XMLNames._M_TermImageFindPattern, complexList.Name);
				bool inTable = IsInTable(text, complexList.Name);
				if (!string.IsNullOrEmpty(complexList.Header))
				{
					string headerText = (template.IsManagedItem ? complexList.Header : complexList.StandardHeader);
					Term.SubstituteBasicTerms(template, ref headerText);
					Term.SubstituteSpecialTerms(ref headerText, template);
					if (ContainsPrintableCharacters())
					{
						text = string.Concat(text, HttpUtility.HtmlDecode(headerText));
					}
					else
					{
						text = HttpUtility.HtmlDecode(headerText);
					}
				}

				//when doing the preview (i.e. template is NOT a managed item), add sample listitems to the complex list
				if (template.IsManagedItem)
				{
					int index = 0;
					for (int i = 0; i < complexList.Items.Count; i++)
					{
						if (complexList.Items[i].Selected ?? false)
						{
							if (inTable)
							{
								string itemDisplayValue = complexList.ItemDisplayValue(complexList.Items[i]);
								text = text.Replace(pattern, string.Format("{0}{1}", itemDisplayValue, pattern));
							}
							else
							{
								ITATClause childClause = new ITATClause();
								childClause.HangingIndent = this.HangingIndent;
								childClause.IndentFirstParagraph = this.IndentFirstParagraph;
								childClause.IndentSubsequentParagraphs = this.IndentSubsequentParagraphs;
								childClause.BreakParagraphs = this.BreakParagraphs;
								childClause.PageBreakBefore = false;
								childClause.Text = complexList.ItemDisplayValue(complexList.Items[i]);
								if (this.ChildNumberingScheme == ChildNumberingSchemeType.None)
									childClause.SuppressSpacingBefore = true;
								_children.Insert(index, childClause);
								index++;
							}
						}
					}
				}
				else
				{
					int index = 0;
					for (int i = 0; i < sampleListItemsOnPreview; i++)
					{
						ComplexListItem newItem = new ComplexListItem();
						for (int j = 0; j < complexList.Fields.Count; j++)
						{
							ComplexListField field = complexList.Fields[j];
                            ComplexListItemValue complexListItemValue = new ComplexListItemValue(field.ID, string.Format(@"<span style=""font-weight:bold; text-decoration:underline;"">{3}{0} #{1} | {2}{4}</span>", complexList.Name, i + 1, field.Name, (char)0xab, (char)0xbb), complexList, field.FilterTerm, true);
							newItem.ItemValues.Add(complexListItemValue);
						}
						if (inTable)
						{
							text = text.Replace(pattern, string.Format("{0}{1}", complexList.ItemDisplayValue(newItem), pattern));
						}
						else
						{
							ITATClause childClause = new ITATClause();
							childClause.HangingIndent = this.HangingIndent;
							childClause.IndentFirstParagraph = this.IndentFirstParagraph;
							childClause.IndentSubsequentParagraphs = this.IndentSubsequentParagraphs;
							childClause.BreakParagraphs = this.BreakParagraphs;
							childClause.PageBreakBefore = false;
							childClause.Text = complexList.ItemDisplayValue(newItem);
							_children.Insert(index, childClause);
							index++;
						}
					}
				}

				//Delete the Complex List Term placeholder from _text
				string sOutput = text.Replace(pattern, string.Empty);
				_text = HttpUtility.HtmlEncode(sOutput);
			}
		}


		internal List<ComplexList> FindComplexLists(Template template, string text)
		{
			List<string> complexListNames = new List<string>();
			List<ComplexList> rtn = new List<ComplexList>();
			string[] delimiter = new string[] { " | " };
			//match anything of the form <img...src="TextImage.ashx?text="...".../> in _text
			if (!string.IsNullOrEmpty(text))
			{
				MatchCollection matches = Regex.Matches(text, XMLNames._M_TermImageTemplate);
				foreach (Match match in matches)
				{
					//first search for a matching Term
					string[] termNameParts = match.Groups[1].Value.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
					string termIdentifier = termNameParts[0];
					Term term = null;
					if (termIdentifier.StartsWith(XMLNames._M_TermID))
					{
						Guid g = new Guid(termIdentifier.Substring(XMLNames._M_TermID.Length));
						term = template.FindTerm(g);
					}
					else
						term = template.FindTerm(termIdentifier);
					//if a term is found, and it is a complex list, add the term name to the list being returned
					if (term != null)
						if (term.TermType == TermType.ComplexList)
							if (!complexListNames.Contains(term.Name))
							{
								complexListNames.Add(term.Name);
								rtn.Add((ComplexList)term);
							}
				}
			}
			return rtn;
		}


		private string DecodeText(string text)
		{
			//Handle special cases such as the "&" character in the text.  
			//Add other Replaces as needed.
			string newText = text.Replace("&amp;", "&amp;amp;");
			return System.Web.HttpUtility.HtmlDecode(newText);
		}


		internal void WriteClause(XmlWriter writer, int level, ChildNumberingSchemeType numberingSchemeType, int number, Template template)
		{
			string clauseXML = string.Format(@"<clause level=""{0}"" number=""{1}      "">{2}</clause>", level, ChildNumberingSchemeHelper.ParagraphNumber(numberingSchemeType, number), DecodeText(_text));

			bool indentFirstParagraph = _indentFirstParagraph ?? false;
			bool indentSubsequentParagraphs = _indentSubsequentParagraphs ?? false;
			bool hangingIndent = _hangingIndent ?? false;
			bool breakParagraphs = _breakParagraphs ?? false;
			bool pageBreakBefore = _pageBreakBefore ?? false;
			bool suppressSpacingBefore = _suppressSpacingBefore ?? false;

			XmlDocument doc = new XmlDocument();
			doc.PreserveWhitespace = true;
			doc.LoadXml(clauseXML);
			PdfHelper.WriteClause(writer, doc.DocumentElement, level, numberingSchemeType, number, indentFirstParagraph, indentSubsequentParagraphs, hangingIndent, breakParagraphs, pageBreakBefore, suppressSpacingBefore);

			int paragraphNumber = 0;
			for (int i = 0; i < _children.Count; i++)
			{
				ITATClause childClause = _children[i];
				if (childClause.ShouldShow(template))
				{
					paragraphNumber++;
					_children[i].WriteClause(writer, level + 1, _childNumberingScheme, paragraphNumber, template);
				}
			}
		}
		 

		#endregion


		//If a clause is a conditional clause (i.e. it DependsOn a term, then check to see if the term has the required value for the clause to appear.
		internal bool ShouldShow(Template template)
		{
			//If we are previewing in the template editor (i.e., template is NOT a ManagedItem), then always show clauses (even conditional ones)
			if (!(template is ManagedItem))
				return true;

			Term term = null;
			if (Term.ValidID(_dependsOnTermID))
				term = template.FindTerm(_dependsOnTermID);
			else
				if (!string.IsNullOrEmpty(_dependsOnTermName))
					term = template.FindBasicTerm(_dependsOnTermName);

			if (term == null)
				return true;
			else
				if (_dependsOnOperator == "=")
					return (term.DisplayValue(XMLNames._TPS_None) == _dependsOnValue);
				else
					return (term.DisplayValue(XMLNames._TPS_None) != _dependsOnValue);
		}

	}
}
