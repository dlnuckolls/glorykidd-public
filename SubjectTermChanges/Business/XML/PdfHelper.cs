using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Collections.Specialized;

namespace Kindred.Knect.ITAT.Business
{
	public static class PdfHelper
	{

		#region public static constants

		//For now, the following values are hard-coded.  May want to make these configurable the future.
		public static int TAB_SIZE = 36;
		public static int PARAGRAPH_SPACING = 18;

		public static int DOTS_PER_INCH = 72;
		public static int PAGE_WIDTH = 612;   //8.5 inches * 72 dpi
		public static int LEFT_MARGIN = 36;
		public static int RIGHT_MARGIN = 36;
		public static int TOP_MARGIN = 72;
		public static int BOTTOM_MARGIN = 72;

		public const string PARA_NAME = "paragraph";
		public const string PARA_SPACING_BEFORE = "spacingbefore";
		public const string PARA_HORIZONTAL_ALIGNMENT = "horizontalalignment";
		public const string FRAGMENT_NAME = "fragment";
		public const string FRAGMENT_FONT = "font";
		public const string FRAGMENT_FONT_SIZE = "fontsize";

		#endregion


		#region  public methods

		public static void WriteClause(XmlWriter writer, XmlNode node, int level, ChildNumberingSchemeType numberingSchemeType, int number, bool indentFirstParagraph, bool indentSubsequentParagraphs, bool hasHangingIndent, bool breakParagraphs, bool pageBreakBefore, bool suppressSpacingBefore)
		{
			bool isClauseNumbered = (numberingSchemeType != ChildNumberingSchemeType.None);

			Stack<PdfStyle> styleStack = new Stack<PdfStyle>();
			styleStack.Push(DefaultStyle());  //ensure that there is a default style on the stack

			PdfLayoutSettings pdfLayoutSettings = new PdfLayoutSettings();
			pdfLayoutSettings.NumberingIndex = number;
			pdfLayoutSettings.NumberingSchemeType = numberingSchemeType;
			pdfLayoutSettings.IsFirstParagraph = true;
			pdfLayoutSettings.CompletedFirstTextNode = false;
			pdfLayoutSettings.PageBreakBefore = pageBreakBefore;
			pdfLayoutSettings.SuppressSpacingBefore = suppressSpacingBefore;
			pdfLayoutSettings.FirstParagraphAttributes = ParagraphAttributes(true, isClauseNumbered, level, indentFirstParagraph, hasHangingIndent, breakParagraphs);
			pdfLayoutSettings.OtherParagraphAttributes = ParagraphAttributes(false, isClauseNumbered, level, indentSubsequentParagraphs, hasHangingIndent, breakParagraphs);
			pdfLayoutSettings.OpenParagraphCounter = 0;
			pdfLayoutSettings.SuppressSpaceBetweenFragments = true;

			ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
		}

		#endregion


		#region  private methods

		private static bool ContainsImage(XmlNode node)
		{
			return node.InnerXml.Contains(string.Format("<{0}", XMLNames._A_ITATImageElement));
		}


		private static void ProcessClauseContent(XmlWriter writer, XmlNodeList nodeList, PdfLayoutSettings pdfLayoutSettings, Stack<PdfStyle> styleStack)
		{
			foreach (XmlNode node in nodeList)
			{
				switch (node.NodeType)
				{
					#region NodeType == Element
					case XmlNodeType.Element:
						{
							switch (node.Name)
							{
								#region case font
								case "font":
									{
										styleStack.Push(GetStyleFromFontElement(node, styleStack.Peek()));
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										styleStack.Pop();
										break;
									}
								#endregion

								#region case b/strong
								case "strong":
								case "b":
									{
										PdfStyle newStyle = new PdfStyle();
										newStyle.CopyFrom(styleStack.Peek());
										newStyle.Font.Bold = true;
										styleStack.Push(newStyle);
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										styleStack.Pop();
										break;
									}
								#endregion

                                #region case pws    
                                case Term._ELEMENT_PRESERVE_WHITE_SPACE:
                                    {
                                        PdfStyle newStyle = new PdfStyle();
                                        newStyle.CopyFrom(styleStack.Peek());
                                        newStyle.PreserveWhiteSpace = true;
                                        styleStack.Push(newStyle);
                                        ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
                                        styleStack.Pop();
                                        break;
                                    }
                                #endregion

								#region case sup
								case "sup":
									{
										PdfStyle newStyle = new PdfStyle();
										newStyle.CopyFrom(styleStack.Peek());
										newStyle.FontTransform = FontTransform.Superscript;
										styleStack.Push(newStyle);
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										styleStack.Pop();
										break;
									}
								#endregion

								#region case sub
								case "sub":
									{
										PdfStyle newStyle = new PdfStyle();
										newStyle.CopyFrom(styleStack.Peek());
										newStyle.FontTransform = FontTransform.Subscript;
										styleStack.Push(newStyle);
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										styleStack.Pop();
										break;
									}
								#endregion

								#region case i/em
								case "i":
								case "em":
									{
										PdfStyle newStyle = new PdfStyle();
										newStyle.CopyFrom(styleStack.Peek());
										newStyle.Font.Italic = true;
										styleStack.Push(newStyle);
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										styleStack.Pop();
										break;
									}
								#endregion

								#region case u, a
								//treat <a></a>  just like <u></u> when rendering a PDF
								case "a":
								case "u":
									{
										PdfStyle newStyle = new PdfStyle();
										newStyle.CopyFrom(styleStack.Peek());
										newStyle.Font.Underline = true;
										styleStack.Push(newStyle);
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										styleStack.Pop();
										break;
									}
								#endregion

								#region case strike
								case "strike":
									{
										PdfStyle newStyle = new PdfStyle();
										newStyle.CopyFrom(styleStack.Peek());
										newStyle.Font.Strikeout = true;
										styleStack.Push(newStyle);
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										styleStack.Pop();
										break;
									}
								#endregion

								#region case span
								case "span":
									{
										XmlAttribute attrSpanStyle = node.Attributes["style"];
										if (attrSpanStyle != null)
											styleStack.Push(GetStyleFromStyleAttribute(attrSpanStyle, styleStack.Peek()));
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										if (attrSpanStyle != null)
											styleStack.Pop();
										break;
									}
								#endregion

								#region case div
								case "div":
									{
										XmlAttribute attrDivStyle = node.Attributes["style"];
										if (attrDivStyle != null)
											styleStack.Push(GetStyleFromStyleAttribute(attrDivStyle, styleStack.Peek()));
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										if (attrDivStyle != null)
											styleStack.Pop();
										WriteLineBreak(writer, true);
										break;
									}
								#endregion

								#region case br
								case "br":
									{
										WriteLineBreak(writer, pdfLayoutSettings.OpenParagraphCounter <= 0);
										break;
									}
								#endregion

								#region case  p
								case "p":
									{
										if (ContainsImage(node))
										{
											ProcessImage(writer, pdfLayoutSettings, node);
										}
										else
										{
											if (pdfLayoutSettings.OpenParagraphCounter > 0)
											{
												writer.WriteEndElement();  //close current paragraph before starting a new one
												pdfLayoutSettings.OpenParagraphCounter--;
											}
											if (node.ChildNodes.Count > 0)
											{
												writer.WriteStartElement("paragraph");
												pdfLayoutSettings.InTextParagraph = true;
												pdfLayoutSettings.OpenParagraphCounter++;
												pdfLayoutSettings.ParagraphAlignment = GetParagraphAlignment(node);
												writer.WriteAttributeString("spacingbefore", (pdfLayoutSettings.SuppressSpacingBefore ? "0" : PARAGRAPH_SPACING.ToString()));
												if (!string.IsNullOrEmpty(pdfLayoutSettings.ParagraphAlignment.Key))
													writer.WriteAttributeString(pdfLayoutSettings.ParagraphAlignment.Key, pdfLayoutSettings.ParagraphAlignment.Value);
												if (pdfLayoutSettings.IsFirstParagraph && !pdfLayoutSettings.CompletedFirstTextNode)
												{
													WriteParagraphAttributes(writer, pdfLayoutSettings.FirstParagraphAttributes);
													if (pdfLayoutSettings.PageBreakBefore)
													{
														writer.WriteAttributeString("startonnewpage", "true");
														pdfLayoutSettings.PageBreakBefore = false;
													}
													if (pdfLayoutSettings.NumberingSchemeType != ChildNumberingSchemeType.None)
													{
														PdfStyle numberingStyle = DetermineNumberingStyle(node, DefaultStyle());
														WriteNumberFragment(writer, pdfLayoutSettings.ParagraphAlignment, pdfLayoutSettings.NumberingSchemeType, pdfLayoutSettings.NumberingIndex, numberingStyle);
													}
												}
												else
												{
													WriteParagraphAttributes(writer, pdfLayoutSettings.OtherParagraphAttributes);
												}
												ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
												pdfLayoutSettings.IsFirstParagraph = false;
											}   // if (node.ChildNodes.Count > 0)
											if (pdfLayoutSettings.OpenParagraphCounter > 0)
											{
												writer.WriteEndElement();  //paragraph
												pdfLayoutSettings.OpenParagraphCounter--;
											}
										}
										break;
									}
								#endregion

								#region case  hr
								case "hr":
									{
										if (pdfLayoutSettings.OpenParagraphCounter > 0)
										{
											writer.WriteEndElement();  //close current paragraph before starting a new one 
											pdfLayoutSettings.OpenParagraphCounter--;
										}

										XmlAttribute attrHrWidth = node.Attributes["size"];
										int lineWidth = 1;
										if (attrHrWidth != null)
											if (!int.TryParse(attrHrWidth.Value, out lineWidth))
												throw new ArgumentException(string.Format("Invalid width parameter on hr tag: '{0}'.", attrHrWidth.Value));
										writer.WriteStartElement("paragraph");
										pdfLayoutSettings.InTextParagraph = false;
										writer.WriteAttributeString("type", "drawing");
										writer.WriteAttributeString("spacingbefore", "3");
										writer.WriteAttributeString("spacingafter", "3");
										writer.WriteAttributeString("width", "540");
										writer.WriteAttributeString("height", lineWidth.ToString());
										writer.WriteStartElement("shape");
										writer.WriteAttributeString("type", "lineshape");
										writer.WriteAttributeString("x", "0");
										writer.WriteAttributeString("y", "0");
										writer.WriteAttributeString("x1", "540");
										writer.WriteAttributeString("y1", "0");
										writer.WriteStartElement("pen");
										writer.WriteAttributeString("color", "black");
										writer.WriteAttributeString("width", lineWidth.ToString());
										writer.WriteEndElement();   // pen
										writer.WriteEndElement();   // shape
										writer.WriteEndElement();   // paragraph
										break;
									}
								#endregion

								#region case  table
								case "table":
									{
										if (pdfLayoutSettings.OpenParagraphCounter > 0)
										{
											writer.WriteEndElement();  //close current paragraph before starting a new one 
											pdfLayoutSettings.OpenParagraphCounter--;
										}
										writer.WriteStartElement("paragraph");
										writer.WriteAttributeString("type", "table");
										writer.WriteAttributeString("spacingbefore", (pdfLayoutSettings.CompletedFirstTextNode ? "0" : PARAGRAPH_SPACING.ToString()));
										pdfLayoutSettings.InTextParagraph = false;
										WriteTableAttributes(writer, node.Attributes, pdfLayoutSettings);
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										pdfLayoutSettings.TableCellSpacing = 0;
										pdfLayoutSettings.TableCellPadding = 0;
										writer.WriteEndElement();   // paragraph
										pdfLayoutSettings.IsFirstParagraph = false;
										break;
									}
								#endregion

								#region case  tr

								case "tr":
									{
										writer.WriteStartElement("row");
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										writer.WriteEndElement();   //  row
										break;
									}
								#endregion

								#region case  td/th
								case "td":
								case "th":
									{
										//save off current pdfLayoutSettings to re-use after the table cell
										PdfLayoutSettings savedPdfLayoutSettings = new PdfLayoutSettings();
										pdfLayoutSettings.CopyTo(ref savedPdfLayoutSettings);
										if (!string.IsNullOrEmpty(pdfLayoutSettings.TableCellAlignment))
										{
											pdfLayoutSettings.FirstParagraphAttributes["horizontalalignment"] = pdfLayoutSettings.TableCellAlignment;
											pdfLayoutSettings.OtherParagraphAttributes["horizontalalignment"] = pdfLayoutSettings.TableCellAlignment;
										}
										//Set selected paragrpah layout attributes
										pdfLayoutSettings.SuppressSpacingBefore = true;
										pdfLayoutSettings.FirstParagraphAttributes = ParagraphAttributes(false, false, 0, false, false, false);
										pdfLayoutSettings.OtherParagraphAttributes = ParagraphAttributes(false, false, 0, false, false, false);

										pdfLayoutSettings.FirstParagraphAttributes = pdfLayoutSettings.OtherParagraphAttributes;

										writer.WriteStartElement("cell");
										WriteCellAttributes(writer, node.Attributes, pdfLayoutSettings);
										if (node.ChildNodes.Count > 0)
										{
											writer.WriteStartElement("paragraph");
											pdfLayoutSettings.InTextParagraph = true;
											pdfLayoutSettings.OpenParagraphCounter++;
											pdfLayoutSettings.ParagraphAlignment = GetParagraphAlignment(node);
											writer.WriteAttributeString("spacingbefore", (pdfLayoutSettings.SuppressSpacingBefore ? "0" : PARAGRAPH_SPACING.ToString()));
											if (!string.IsNullOrEmpty(pdfLayoutSettings.ParagraphAlignment.Key))
												writer.WriteAttributeString(pdfLayoutSettings.ParagraphAlignment.Key, pdfLayoutSettings.ParagraphAlignment.Value);
											if (pdfLayoutSettings.IsFirstParagraph && !pdfLayoutSettings.CompletedFirstTextNode)
											{
												WriteParagraphAttributes(writer, pdfLayoutSettings.FirstParagraphAttributes);
												if (pdfLayoutSettings.PageBreakBefore)
												{
													writer.WriteAttributeString("startonnewpage", "true");
													pdfLayoutSettings.PageBreakBefore = false;
												}
											}
											else
											{
												WriteParagraphAttributes(writer, pdfLayoutSettings.OtherParagraphAttributes);
											}
											ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
											pdfLayoutSettings.IsFirstParagraph = false;
										}   // if (node.ChildNodes.Count > 0)
										if (pdfLayoutSettings.OpenParagraphCounter > 0)
										{
											writer.WriteEndElement();  //paragraph
											pdfLayoutSettings.OpenParagraphCounter--;
										}
										writer.WriteEndElement();   //  cell

										//reset paragrpah layout attributes back to the saved values
										savedPdfLayoutSettings.CopyTo(ref pdfLayoutSettings);
										break;
									}
								#endregion

								#region unused table elements

								case "tbody":
								case "thead":
								case "tfoot":
									{
										ProcessClauseContent(writer, node.ChildNodes, pdfLayoutSettings, styleStack);
										break;
									}
								#endregion

								#region case "img"
								case "img":
									{
										PdfStyle currentStyle = styleStack.Peek();
										writer.WriteStartElement("fragment");
										writer.WriteAttributeString("font", TallPDFFontName(currentStyle.Font));
										writer.WriteAttributeString("fontsize", currentStyle.Font.Size.ToString());
										if (pdfLayoutSettings.SuppressSpaceBetweenFragments)
											if (!char.IsWhiteSpace(node.OuterXml[0]))
												writer.WriteAttributeString("suppressspacebefore", "true");
										if (currentStyle.Font.Underline)
											writer.WriteAttributeString("underline", "true");
										if (currentStyle.Font.Strikeout)
											writer.WriteAttributeString("strikeout", "true");
										if (currentStyle.FontTransform == FontTransform.Subscript)
											writer.WriteAttributeString("subscript", "true");
										else
											if (currentStyle.FontTransform == FontTransform.Superscript)
												writer.WriteAttributeString("superscript", "true");
										writer.WriteString(Utility.XMLHelper.GetText(node));
										writer.WriteEndElement();  //fragment
										break;
									}
								#endregion

								#region "itatImage"
								case XMLNames._A_ITATImageElement:
									{
										//do nothing.  This is processed within the <p> tag, using the ProcessImage() method
										break;
									}
								#endregion

								#region default (throw exception)
								default:
									throw new Exception("XHTML Element not processed:  " + node.OuterXml);
								#endregion
							}
							break;
						}
					#endregion

					#region NodeType == Text
					case XmlNodeType.Text:
						{
							bool createdNewParagraph = false;
							if (pdfLayoutSettings.OpenParagraphCounter <= 0)
							{
								createdNewParagraph = true;
								writer.WriteStartElement("paragraph");  //start new paragraph
								pdfLayoutSettings.InTextParagraph = true;
								pdfLayoutSettings.OpenParagraphCounter++;
								writer.WriteAttributeString("spacingbefore", "0");
								if (!string.IsNullOrEmpty(pdfLayoutSettings.ParagraphAlignment.Key))
									writer.WriteAttributeString(pdfLayoutSettings.ParagraphAlignment.Key, pdfLayoutSettings.ParagraphAlignment.Value);
								if (pdfLayoutSettings.IsFirstParagraph)
								{
									WriteParagraphAttributes(writer, pdfLayoutSettings.FirstParagraphAttributes);
									if (pdfLayoutSettings.PageBreakBefore)
									{
										writer.WriteAttributeString("startonnewpage", "true");
										pdfLayoutSettings.PageBreakBefore = false;
									}
								}
								else
									WriteParagraphAttributes(writer, pdfLayoutSettings.OtherParagraphAttributes);
							}
							PdfStyle currentStyle = styleStack.Peek();
							writer.WriteStartElement("fragment");
							writer.WriteAttributeString("font", TallPDFFontName(currentStyle.Font));
                            if (currentStyle.PreserveWhiteSpace)
							    writer.WriteAttributeString("preservewhitespace", "true");  //RLR,DG v1.5 - added to preserve formatting in text terms (e.g., line breaks)
							writer.WriteAttributeString("fontsize", currentStyle.Font.Size.ToString());
							if (pdfLayoutSettings.SuppressSpaceBetweenFragments)
								if (!char.IsWhiteSpace(node.Value[0]))
									writer.WriteAttributeString("suppressspacebefore", "true");
							if (currentStyle.Font.Underline)
								writer.WriteAttributeString("underline", "true");
							if (currentStyle.Font.Strikeout)
								writer.WriteAttributeString("strikeout", "true");

							if (currentStyle.FontTransform == FontTransform.Subscript)
								writer.WriteAttributeString("subscript", "true");
							else
								if (currentStyle.FontTransform == FontTransform.Superscript)
									writer.WriteAttributeString("superscript", "true");

							writer.WriteString(node.Value);
							writer.WriteEndElement();  //fragment
							//if THIS node does NOT end with a space, then suppress the extra space before the NEXT fragment
							pdfLayoutSettings.SuppressSpaceBetweenFragments = (!node.Value.EndsWith(" "));
							pdfLayoutSettings.CompletedFirstTextNode = true;
							if (createdNewParagraph)
							{
								writer.WriteEndElement();  //paragraph
								pdfLayoutSettings.OpenParagraphCounter--;
							}
							break;
						}
					#endregion

					#region NodeType == Whitespace
					case XmlNodeType.Whitespace:
						{
							bool createdParagraph = false;
							if (pdfLayoutSettings.OpenParagraphCounter <= 0)
							{
								writer.WriteStartElement("paragraph");
								createdParagraph = true;
								pdfLayoutSettings.OpenParagraphCounter++;
								writer.WriteAttributeString("type", "textparagraph");
								writer.WriteAttributeString("spacingbefore", (pdfLayoutSettings.SuppressSpacingBefore ? "0" : PARAGRAPH_SPACING.ToString()));
							}
							PdfStyle currentStyle = styleStack.Peek();
							writer.WriteStartElement("fragment");
							writer.WriteAttributeString("font", TallPDFFontName(currentStyle.Font));
							writer.WriteAttributeString("fontsize", currentStyle.Font.Size.ToString());
							if (pdfLayoutSettings.SuppressSpaceBetweenFragments)
								writer.WriteAttributeString("suppressspacebefore", "true");
							if (currentStyle.Font.Underline)
								writer.WriteAttributeString("underline", "true");
							if (currentStyle.Font.Strikeout)
								writer.WriteAttributeString("strikeout", "true");
							writer.WriteString(node.Value);
							writer.WriteEndElement();  //fragment
							if (createdParagraph)
							{
								writer.WriteEndElement();  // paragraph
								pdfLayoutSettings.OpenParagraphCounter--;
							}
							//DO NOT suppress the space before the NEXT fragment
							pdfLayoutSettings.SuppressSpaceBetweenFragments = false;
							break;
						}
					#endregion

					#region unused NodeTypes (throw exception)
					case XmlNodeType.SignificantWhitespace:
					case XmlNodeType.Attribute:
					case XmlNodeType.CDATA:
					case XmlNodeType.Comment:
					case XmlNodeType.Document:
					case XmlNodeType.DocumentFragment:
					case XmlNodeType.DocumentType:
					case XmlNodeType.EndElement:
					case XmlNodeType.EndEntity:
					case XmlNodeType.Entity:
					case XmlNodeType.EntityReference:
					case XmlNodeType.None:
					case XmlNodeType.Notation:
					case XmlNodeType.ProcessingInstruction:
					case XmlNodeType.XmlDeclaration:
					default:
						throw new Exception(string.Format("XHTML Node not processed:  NodeType={0}, OuterXml=  ", node.NodeType.ToString(), node.OuterXml));
					#endregion
				}
			}
		}


		private static void ProcessImage(XmlWriter writer, PdfLayoutSettings pdfLayoutSettings, XmlNode node)
		{
			XmlNode imageNode = node.SelectSingleNode(string.Format("//{0}", XMLNames._A_ITATImageElement));
			if (imageNode != null)
			{
				writer.WriteStartElement("paragraph");
				writer.WriteAttributeString("type", "image");
				writer.WriteAttributeString("keepaspectratio", "false");
				KeyValuePair<string, string> alignmentKeyValue = GetImageParagraphAlignment(node);
				writer.WriteAttributeString(alignmentKeyValue.Key, alignmentKeyValue.Value);
				if (imageNode.Attributes["width"] != null)
					writer.WriteAttributeString("width", imageNode.Attributes["width"].Value);
				if (imageNode.Attributes["height"] != null)
					writer.WriteAttributeString("height", imageNode.Attributes["height"].Value);
				if (imageNode.Attributes["hspace"] != null || imageNode.Attributes["vspace"] != null)
				{
					writer.WriteStartElement("padding");
					if (imageNode.Attributes["hspace"] != null)
					{
						writer.WriteAttributeString("left", imageNode.Attributes["hspace"].Value);
						writer.WriteAttributeString("right", imageNode.Attributes["hspace"].Value);
					}
					else
					{
						writer.WriteAttributeString("left", "0");
						writer.WriteAttributeString("right", "0");
					}
					if (imageNode.Attributes["vspace"] != null)
					{
						writer.WriteAttributeString("top", imageNode.Attributes["vspace"].Value);
						writer.WriteAttributeString("bottom", imageNode.Attributes["vspace"].Value);
					}
					else
					{
						writer.WriteAttributeString("top", "0");
						writer.WriteAttributeString("bottom", "0");
					}
					writer.WriteEndElement();  //padding
				}
				writer.WriteString(imageNode.InnerXml);
				writer.WriteEndElement(); //paragraph
			}
		}


		private static NameValueCollection ParagraphAttributes(bool isFirstParagraph, bool isClauseNumbered, int level, bool indentParagraph, bool hasHangingIndent, bool breakParagraphs)
		{
			int leftIndent = 0;
			int firstLineIndent = 0;
			int numberLeftIndent = 0;
			int hangingIndent = 0;

			string paragraphType = "textparagraph";
			if (isFirstParagraph)
			{
				if (isClauseNumbered)
				{
					paragraphType = "numbereditem";
					numberLeftIndent = level * PdfHelper.TAB_SIZE;
					leftIndent = (level + 1) * PdfHelper.TAB_SIZE;
				}
				else
				{
					if (indentParagraph)
						leftIndent = (level + 1) * PdfHelper.TAB_SIZE;
					else
						leftIndent = level * PdfHelper.TAB_SIZE;
				}
			}
			else
			{
				if (indentParagraph)
					leftIndent = (level + 1) * PdfHelper.TAB_SIZE;
				else
					leftIndent = level * PdfHelper.TAB_SIZE;
			}

			if (hasHangingIndent)
				hangingIndent = 0;
			else
				hangingIndent = -1 * (leftIndent + firstLineIndent);

			NameValueCollection rtn = new NameValueCollection();
			rtn.Add("type", paragraphType);

			if (leftIndent != 0)
				rtn.Add("leftindentation", leftIndent.ToString());

			if (hangingIndent != 0)
				rtn.Add("hangindentation", hangingIndent.ToString());

			if (paragraphType == "numbereditem")
				if (numberLeftIndent != 0)
					rtn.Add("numberleftindentation", numberLeftIndent.ToString());

			if (!breakParagraphs)
				rtn.Add("donotbreak", "true");

			return rtn;
		}


		private static void WriteParagraphAttributes(XmlWriter writer, NameValueCollection attributeCollection)
		{
			for (int i = 0, j = attributeCollection.Count; i < j; i++)
				writer.WriteAttributeString(attributeCollection.GetKey(i), attributeCollection[i]);
		}


		private static void WriteCellAttributes(XmlWriter writer, XmlAttributeCollection xmlAttributeCollection, PdfLayoutSettings pdfLayoutSettings)
		{
			//first write attributes of the <cell> node
			foreach (XmlAttribute attr in xmlAttributeCollection)
			{
				string value = attr.Value.Trim();
				switch (attr.Name.ToUpper())
				{
					case "STYLE":
						string[,] styleParts = ParseStyleAttribute(value);
						int numberOfStyleParts = styleParts.GetLength(0);
						for (int i = 0; i < numberOfStyleParts; i++)
						{
							switch (styleParts[i, 0].ToUpper())
							{
								case "WIDTH":
									Unit widthUnit = Unit.Parse(styleParts[i, 1]);
									//units other than percentages or pixels are ignored
									switch (widthUnit.Type)
									{
										case UnitType.Percentage:
											{
												writer.WriteAttributeString("fixed", "true");
												int cellWidth = (int)(pdfLayoutSettings.TableWidth * widthUnit.Value / 100.0);
												writer.WriteAttributeString("preferredwidth", cellWidth.ToString());
												break;
											}
										case UnitType.Pixel:
											{
												writer.WriteAttributeString("fixed", "true");
												writer.WriteAttributeString("preferredwidth", widthUnit.Value.ToString());
												break;
											}
										default:
											break;
									}
									break;
							}

						}
						break;

					case "VALIGN":
						switch (value.ToUpper())
						{
							case "TOP":
								writer.WriteAttributeString("verticalalignment", "top");
								break;
							case "MIDDLE":
								writer.WriteAttributeString("verticalalignment", "middle");
								break;
							case "BOTTOM":
								writer.WriteAttributeString("verticalalignment", "bottom");
								break;
						}
						break;

					case "ALIGN":
						switch (value.ToUpper())
						{
							case "LEFT":
								pdfLayoutSettings.TableCellAlignment = "left";
								break;
							case "CENTER":
								pdfLayoutSettings.TableCellAlignment = "center";
								break;
							case "RIGHT":
								pdfLayoutSettings.TableCellAlignment = "right";
								break;
						}
						break;

				}
			}
			//then write padding and margin elements, if appropriate
			if (pdfLayoutSettings.TableCellPadding > 0)
			{
				writer.WriteStartElement("padding");
				writer.WriteAttributeString("top", pdfLayoutSettings.TableCellPadding.ToString());
				writer.WriteAttributeString("right", pdfLayoutSettings.TableCellPadding.ToString());
				writer.WriteAttributeString("bottom", pdfLayoutSettings.TableCellPadding.ToString());
				writer.WriteAttributeString("left", pdfLayoutSettings.TableCellPadding.ToString());
				writer.WriteEndElement();   // padding
			}
			if (pdfLayoutSettings.TableCellSpacing > 0)
			{
				writer.WriteStartElement("margin");
				writer.WriteAttributeString("top", pdfLayoutSettings.TableCellSpacing.ToString());
				writer.WriteAttributeString("right", pdfLayoutSettings.TableCellSpacing.ToString());
				writer.WriteAttributeString("bottom", pdfLayoutSettings.TableCellSpacing.ToString());
				writer.WriteAttributeString("left", pdfLayoutSettings.TableCellSpacing.ToString());
				writer.WriteEndElement();   // margin
			}


		}


		private static string[,] ParseStyleAttribute(string styleAttribute)
		{
			string[] styles = styleAttribute.Split(';');
			string[,] rtn = new string[styles.Length, 2];
			for (int i = 0; i < styles.Length; i++)
			{
				string[] styleParts = styles[i].Split(':');
				for (int j = 0; j < 2; j++)
					rtn[i, j] = styleParts[j];
			}
			return rtn;
		}


		private static void WriteTableAttributes(XmlWriter writer, XmlAttributeCollection xmlAttributeCollection, PdfLayoutSettings pdfLayoutSettings)
		{
			foreach (XmlAttribute attr in xmlAttributeCollection)
			{
				string value = attr.Value.Trim();
				switch (attr.Name.ToUpper())
				{
					case "STYLE":
						string[,] styleParts = ParseStyleAttribute(value);
						int numberOfStyleParts = styleParts.GetLength(0);
						for (int i = 0; i < numberOfStyleParts; i++)
						{
							switch (styleParts[i, 0].ToUpper())
							{
								case "WIDTH":
									Unit widthUnit = Unit.Parse(styleParts[i, 1]);
									//units other than percentages are ignored
									if (widthUnit.Type == UnitType.Percentage)
									{
										writer.WriteAttributeString("forcewidth", "true");
										pdfLayoutSettings.TableWidth = (int)((PAGE_WIDTH - LEFT_MARGIN - RIGHT_MARGIN) * widthUnit.Value / 100.0);
										writer.WriteAttributeString("preferredwidth", pdfLayoutSettings.TableWidth.ToString());
									}
									break;
							}

						}
						break;

					case "CELLSPACING":
						int spacing;
						if (int.TryParse(value, out spacing))
							pdfLayoutSettings.TableCellSpacing = spacing;
						break;

					case "CELLPADDING":
						int padding;
						if (int.TryParse(value, out padding))
							pdfLayoutSettings.TableCellPadding = padding;
						break;
				}
			}
		}


		private static PdfStyle DetermineNumberingStyle(XmlNode node, PdfStyle currentStyle)
		{
			//search through "node" until the first node of XmlNodeType.Text is reached.  Return the style of that node.
			foreach (XmlNode childNode in node.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.Text)
				{
					return currentStyle;
				}
				if (childNode.NodeType == XmlNodeType.Element)
				{
					switch (childNode.Name)
					{
						case "font":
							Style fontNodeStyle = GetStyleFromFontElement(childNode, currentStyle);
							currentStyle.Font.Name = fontNodeStyle.Font.Name;
							currentStyle.Font.Size = fontNodeStyle.Font.Size;
							break;

						case "span":
						case "div":
							XmlAttribute attrStyle = childNode.Attributes["style"];
							if (attrStyle != null)
							{
								Style divNodeStyle = GetStyleFromStyleAttribute(attrStyle, currentStyle);
								currentStyle.Font.Name = divNodeStyle.Font.Name;
								currentStyle.Font.Size = divNodeStyle.Font.Size;
							}
							break;
					}
				}
				DetermineNumberingStyle(childNode, currentStyle);
			}
			//if it gets to here, we didn't find a text node, so return the default style
			return DefaultStyle();
		}


		private static void WriteNumberFragment(XmlWriter writer, KeyValuePair<string, string> paragraphAlignment, ChildNumberingSchemeType numberingSchemeType, int number, PdfStyle numberingStyle)
		{
			if (paragraphAlignment.Key == "horizontalalignment")
				if (paragraphAlignment.Value != "left")
					throw new Exception(string.Format("A numbered paragraph cannot be {0}-aligned.", paragraphAlignment.Value));

			writer.WriteStartElement("numberfragment");
			writer.WriteAttributeString("type", "fragment");
			numberingStyle.Font.Bold = true;
			writer.WriteAttributeString("font", TallPDFFontName(numberingStyle.Font));
			writer.WriteAttributeString("fontsize", numberingStyle.Font.Size.ToString());
			writer.WriteString(ChildNumberingSchemeHelper.ParagraphNumber(numberingSchemeType, number));
			writer.WriteEndElement();  //numberfragment
		}


		private static KeyValuePair<string, string> GetParagraphAlignment(XmlNode node)
		{
			XmlAttribute attrAlign = node.Attributes["align"];
			string align = "left";
			if (attrAlign != null)
				align = attrAlign.Value;
			string key = string.Empty;
			string value = string.Empty;
			if (align == "justify")
			{
				key = "justified";
				value = "true";
			}
			else
			{
				key = "horizontalalignment";
				value = align;
			}
			return new KeyValuePair<string, string>(key, value);
		}

		private static KeyValuePair<string, string> GetImageParagraphAlignment(XmlNode node)
		{
			string key = "horizontalalignment";
			string value = "left";
			XmlAttribute attrAlign = node.Attributes["align"];
			if (attrAlign != null)
				if (attrAlign.Value != "justify")
					value = attrAlign.Value;
			return new KeyValuePair<string, string>(key, value);
		}


		private static PdfStyle GetStyleFromStyleAttribute(XmlAttribute attrStyle, PdfStyle currentStyle)
		{
			Dictionary<string, string> dictStyleParts = new Dictionary<string, string>();
			foreach (string style in attrStyle.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
			{
				string[] styleParts = style.Split(':');
				dictStyleParts.Add(styleParts[0].Trim(), styleParts[1].Trim());
			}

			PdfStyle newStyle = new PdfStyle();

			if (dictStyleParts.ContainsKey("font-family"))
				newStyle.Font.Name = dictStyleParts["font-family"];
			else
				if (dictStyleParts.ContainsKey("FONT-FAMILY"))
					newStyle.Font.Name = dictStyleParts["FONT-FAMILY"];
				else
					newStyle.Font.Name = currentStyle.Font.Name;

			if (dictStyleParts.ContainsKey("font-size"))
				newStyle.Font.Size = ConvertFontSize(dictStyleParts["font-size"]);
			else
				if (dictStyleParts.ContainsKey("FONT-SIZE"))
					newStyle.Font.Size = ConvertFontSize(dictStyleParts["FONT-SIZE"]);
				else
					newStyle.Font.Size = currentStyle.Font.Size;

			if (dictStyleParts.ContainsKey("font-weight"))
				newStyle.Font.Bold = ConvertFontBold(dictStyleParts["font-weight"]);
			else
				if (dictStyleParts.ContainsKey("FONT-WEIGHT"))
					newStyle.Font.Bold = ConvertFontBold(dictStyleParts["FONT-WEIGHT"]);
				else
					newStyle.Font.Bold = currentStyle.Font.Bold;

			if (dictStyleParts.ContainsKey("font-style"))
				newStyle.Font.Italic = ConvertFontItalic(dictStyleParts["font-style"]);
			else
				if (dictStyleParts.ContainsKey("FONT-STYLE"))
					newStyle.Font.Italic = ConvertFontItalic(dictStyleParts["FONT-STYLE"]);
				else
					newStyle.Font.Italic = currentStyle.Font.Italic;

			if (dictStyleParts.ContainsKey("text-decoration"))
			{
				newStyle.Font.Underline = ConvertFontUnderline(dictStyleParts["text-decoration"]);
				newStyle.Font.Strikeout = ConvertFontStrikethrough(dictStyleParts["text-decoration"]);
			}
			else
				if (dictStyleParts.ContainsKey("TEXT-DECORATION"))
				{
					newStyle.Font.Underline = ConvertFontUnderline(dictStyleParts["TEXT-DECORATION"]);
					newStyle.Font.Strikeout = ConvertFontStrikethrough(dictStyleParts["TEXT-DECORATION"]);
				}
				else
				{
					newStyle.Font.Underline = currentStyle.Font.Underline;
					newStyle.Font.Strikeout = currentStyle.Font.Strikeout;
				}

			return newStyle;
		}


		private static bool ConvertFontStrikethrough(string fontDecoration)
		{
			return (fontDecoration.ToLower() == "line-through");
		}


		private static bool ConvertFontUnderline(string fontDecoration)
		{
			return (fontDecoration.ToLower() == "underline");
		}


		private static bool ConvertFontItalic(string fontStyle)
		{
			if (fontStyle.ToLower() == "italic")
				return true;
			if (fontStyle.ToLower() == "oblique")
				return true;
			return false;
		}


		private static bool ConvertFontBold(string fontWeight)
		{
			int numericWeight;
			if (int.TryParse(fontWeight, out numericWeight))
				return (numericWeight >= 700);
			if (fontWeight.ToLower() == "bold")
				return true;
			return false;
		}


		private static FontUnit ConvertFontSize(string fontSize)
		{
			int size;
			if (int.TryParse(fontSize, out size))
				switch (size)
				{
					case 1:
						return new FontUnit(8);
					case 2:
						return new FontUnit(10);
					case 3:
						return new FontUnit(12);
					case 4:
						return new FontUnit(14);
					case 5:
						return new FontUnit(16);
					case 6:
						return new FontUnit(18);
					case 7:
						return new FontUnit(24);
					default:
						throw new Exception("Unable to convert " + fontSize + " to a FontUnit.");
				}
			else
			{
				try { return new FontUnit(fontSize); }
				catch { throw new Exception("Unable to convert " + fontSize + " to a FontUnit."); }
			}
		}


		private static PdfStyle DefaultStyle()
		{
			PdfStyle rtn = new PdfStyle();
			rtn.Font.Name = "timesroman";
			rtn.Font.Size = new FontUnit(12);
			return rtn;
		}


		private static PdfStyle GetStyleFromFontElement(XmlNode node, PdfStyle currentStyle)
		{
			//parse <font> tag and return a corresponding Font obejct
			XmlAttribute attrFace = node.Attributes["face"];
			XmlAttribute attrSize = node.Attributes["size"];

			PdfStyle rtn = new PdfStyle();

			if (attrFace == null)
				rtn.Font.Name = currentStyle.Font.Name;
			else
				rtn.Font.Name = attrFace.Value;

			if (attrSize == null)
				rtn.Font.Size = currentStyle.Font.Size;
			else
				rtn.Font.Size = ConvertFontSize(attrSize.Value);

			return rtn;
		}


		private static string TallPDFFontName(FontInfo f)
		{
			switch (f.Name.ToLower())
			{
				case "times new roman":
				case "times":
				case "timesroman":
					if (f.Bold)
						if (f.Italic)
							return "timesbolditalic";
						else
							return "timesbold";
					else
						if (f.Italic)
							return "timesitalic";
						else
							return "timesroman";

				case "arial":
				case "helv":
				case "helvetica":
				case "verdana":
				case "tahoma":
					if (f.Bold)
						if (f.Italic)
							return "helveticaboldoblique";
						else
							return "helveticabold";
					else
						if (f.Italic)
							return "helveticaoblique";
						else
							return "helvetica";

				case "courier":
				case "courier new":
					if (f.Bold)
						if (f.Italic)
							return "courierboldoblique";
						else
							return "courierbold";
					else
						if (f.Italic)
							return "courieroblique";
						else
							return "courier";

				default:
					throw new Exception("Font not supported");

			}
		}


		public static void WriteLineBreak(XmlWriter writer, bool writeParagraphElement)
		{
			if (writeParagraphElement)
			{
				writer.WriteStartElement("paragraph");
				writer.WriteAttributeString("type", "textparagraph");
				writer.WriteAttributeString("spacingbefore", "0");
			}
			writer.WriteStartElement("fragment");
			writer.WriteAttributeString("type", "LineBreakFragment");
			writer.WriteEndElement(); //fragment
			if (writeParagraphElement)
			{
				writer.WriteEndElement(); //paragraph
			}
		}

		#endregion



		#region Generic PDF XML generators


		public static void AddPDFXMLHeader(XmlWriter writer)
		{
			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "table");
			writer.WriteAttributeString("spacingbefore", "1");
			writer.WriteAttributeString("spacingafter", "1");
			writer.WriteStartElement("rows");
		}

		public static void AddPDFXMLFooter(XmlWriter writer)
		{
			writer.WriteEndElement();   // </rows>
			writer.WriteEndElement();   // </paragraph>

			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "horizontalline");
			writer.WriteAttributeString("linewidth", "0.02");
			writer.WriteEndElement();  // </paragraph>
		}


		public static bool EmitPDFXML(XmlWriter writer, string sCell1, string sCell2)
		{
			AddPDFXMLHeader(writer);
			AddPDFXMLRow(writer, sCell1, sCell2);
			AddPDFXMLFooter(writer);
			return true;
		}

		public static bool EmitPDFXML(XmlWriter writer, string sCell1, string sCell2, string sCell3)
		{
			AddPDFXMLHeader(writer);
			AddPDFXMLRow(writer, sCell1, sCell2, sCell3);
			AddPDFXMLFooter(writer);
			return true;
		}

		public static void AddPDFXMLRow(XmlWriter writer, string sCell1, string sCell2)
		{
			writer.WriteStartElement("row");
			writer.WriteAttributeString("minheight", "0");
			AddPDFXMLCell(writer, 180, HorizontalAlign.Left, sCell1, true, false);
            AddPDFXMLCell(writer, 10, HorizontalAlign.Left, string.Empty, false, false);
            AddPDFXMLCell(writer, 380, HorizontalAlign.Left, sCell2, false, false);
			writer.WriteEndElement();   // </row>
		}

        public static void AddPDFXMLRowText(XmlWriter writer, string sCell1, string sCell2, bool preserveWhiteSpace)
        {
            writer.WriteStartElement("row");
            writer.WriteAttributeString("minheight", "0");
            AddPDFXMLCell(writer, 180, HorizontalAlign.Left, sCell1, true, false);
            AddPDFXMLCell(writer, 10, HorizontalAlign.Left, string.Empty, false, false);
            AddPDFXMLCell(writer, 380, HorizontalAlign.Left, sCell2, false, preserveWhiteSpace);
            writer.WriteEndElement();   // </row>
        }

		public static void AddPDFXMLRow(XmlWriter writer, string sCell1, string sCell2, string sCell3)
		{
			writer.WriteStartElement("row");
			writer.WriteAttributeString("minheight", "0");
            AddPDFXMLCell(writer, 180, HorizontalAlign.Left, sCell1, true, false);
            AddPDFXMLCell(writer, 10, HorizontalAlign.Left, string.Empty, false, false);
            AddPDFXMLCell(writer, 100, HorizontalAlign.Left, sCell2, false, false);
            AddPDFXMLCell(writer, 10, HorizontalAlign.Left, string.Empty, false, false);
            AddPDFXMLCell(writer, 270, HorizontalAlign.Left, sCell3, false, false);
			writer.WriteEndElement();   // </row>
		}

		public static void AddListRowSeparator(XmlWriter writer)
		{
			writer.WriteStartElement("row");
			writer.WriteAttributeString("minheight", "0");
            AddPDFXMLCell(writer, 180, HorizontalAlign.Left, string.Empty, true, false);
            AddPDFXMLCell(writer, 10, HorizontalAlign.Left, string.Empty, false, false);
			AddCellSeparator(writer, 100);
			AddCellSeparator(writer, 10);
			AddCellSeparator(writer, 270);
			writer.WriteEndElement();   // </row>
		}

        public static void AddPDFXMLCell(XmlWriter writer, int width, System.Web.UI.WebControls.HorizontalAlign horizontalAlign, string content, bool isBold, bool preserveWhiteSpace)
		{
			writer.WriteStartElement("cell");
			writer.WriteAttributeString("preferredwidth", width.ToString());
			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "textparagraph");
			writer.WriteAttributeString("horizontalalignment", horizontalAlign.ToString().ToLower());
			writer.WriteStartElement("fragment");
            if (preserveWhiteSpace)
                writer.WriteAttributeString("preservewhitespace", "true");
            writer.WriteAttributeString("font", isBold ? "timesbold" : "timesroman");
			writer.WriteAttributeString("fontsize", "10");
			writer.WriteString(content);
			writer.WriteEndElement();			// </fragment>
			writer.WriteEndElement();			// </paragraph>
			writer.WriteEndElement();			// </cell>
		}

        public static void AddCellSeparator(XmlWriter writer, int width)
		{
			writer.WriteStartElement("cell");
			writer.WriteAttributeString("preferredwidth", width.ToString());
			writer.WriteStartElement("paragraph");
			writer.WriteAttributeString("type", "horizontalline");
			writer.WriteAttributeString("linewidth", "0.02");
			writer.WriteEndElement();			// </paragraph>
			writer.WriteEndElement();			// </cell>
		}

		public static void AddPDFXMLText(XmlWriter writer, string content, bool isBold)
		{
			writer.WriteStartElement("fragment");
			writer.WriteAttributeString("font", isBold ? "timesbold" : "timesroman");
			writer.WriteAttributeString("fontsize", "10");
			writer.WriteString(content);
			writer.WriteEndElement();			// </fragment>
		}



		#endregion



	}
}
