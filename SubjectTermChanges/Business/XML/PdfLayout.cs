using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.Specialized;

namespace Kindred.Knect.ITAT.Business
{

		public class PdfLayoutSettings
		{

			#region private members
			NameValueCollection _firstParagraphAttributes;
			NameValueCollection _otherParagraphAttributes;
			ChildNumberingSchemeType _numberingSchemeType;
			int _numberingIndex;
			bool _inTextParagraph;
			bool _isFirstParagraph;
			bool _pageBreakBefore;
			bool _completedFirstTextNode;
			KeyValuePair<string, string> _paragraphAlignment;
			bool _suppressSpacingBefore;
			bool _suppressSpaceBetweenFragments;
			int _openParagraphCounter;
			int _tableCellPadding;
			int _tableCellSpacing;
			string _tableCellAlignment;
			int _tableWidth;
			#endregion

			#region properties
			public NameValueCollection FirstParagraphAttributes
			{
				get { return _firstParagraphAttributes; }
				set { _firstParagraphAttributes = value; }
			}

			public NameValueCollection OtherParagraphAttributes
			{
				get { return _otherParagraphAttributes; }
				set { _otherParagraphAttributes = value; }
			}

			public ChildNumberingSchemeType NumberingSchemeType
			{
				get { return _numberingSchemeType; }
				set { _numberingSchemeType = value; }
			}

			public int NumberingIndex
			{
				get { return _numberingIndex; }
				set { _numberingIndex = value; }
			}
			 
			public bool InTextParagraph
			{
				get { return _inTextParagraph; }
				set { _inTextParagraph = value; }
			}

			public bool IsFirstParagraph
			{
				get { return _isFirstParagraph; }
				set { _isFirstParagraph = value; }
			}

			public bool PageBreakBefore
			{
				get { return _pageBreakBefore; }
				set { _pageBreakBefore = value; }
			}

			public bool CompletedFirstTextNode
			{
				get { return _completedFirstTextNode; }
				set { _completedFirstTextNode = value; }
			}

			public KeyValuePair<string, string> ParagraphAlignment
			{
				get { return _paragraphAlignment; }
				set { _paragraphAlignment = value; }
			}

			public bool SuppressSpacingBefore
			{
				get { return _suppressSpacingBefore; }
				set { _suppressSpacingBefore = value; }
			}

			public bool SuppressSpaceBetweenFragments
			{
				get { return _suppressSpaceBetweenFragments; }
				set { _suppressSpaceBetweenFragments = value; }
			}

			public int OpenParagraphCounter
			{
				get { return _openParagraphCounter; }
				set { _openParagraphCounter = value; }
			}

			public int TableCellPadding
			{
				get { return _tableCellPadding; }
				set { _tableCellPadding = value; }
			}

			public int TableCellSpacing
			{
				get { return _tableCellSpacing; }
				set { _tableCellSpacing = value; }
			}

			public string TableCellAlignment
			{
				get { return _tableCellAlignment; }
				set { _tableCellAlignment = value; }
			}

			public int TableWidth
			{
				get { return _tableWidth; }
				set { _tableWidth = value; }
			}


			#endregion


			#region constructor

			public PdfLayoutSettings()
			{
				_openParagraphCounter = 0;
				_tableCellPadding = 0;
				_tableCellSpacing = 0;
				_tableCellAlignment = string.Empty;
				_tableWidth = 0;
			}

			#endregion

			public void CopyTo(ref PdfLayoutSettings settings)
			{
				settings.CompletedFirstTextNode = this.CompletedFirstTextNode;
				settings.FirstParagraphAttributes = this.FirstParagraphAttributes;
				settings.InTextParagraph = this.InTextParagraph;
				settings.IsFirstParagraph = this.IsFirstParagraph;
				settings.NumberingSchemeType = this.NumberingSchemeType;
				settings.OpenParagraphCounter = this.OpenParagraphCounter;
				settings.OtherParagraphAttributes = this.OtherParagraphAttributes;
				settings.PageBreakBefore = this.PageBreakBefore;
				settings.ParagraphAlignment = this.ParagraphAlignment;
				settings.SuppressSpaceBetweenFragments = this.SuppressSpaceBetweenFragments;
				settings.SuppressSpacingBefore = this.SuppressSpacingBefore;
				settings.TableCellAlignment = this.TableCellAlignment;
				settings.TableCellPadding = this.TableCellPadding;
				settings.TableCellSpacing = this.TableCellSpacing;
				settings.TableWidth = this.TableWidth;
			}

		}


		public enum FontTransform
		{
			None,
			Subscript,
			Superscript
		}


		public class PdfStyle : System.Web.UI.WebControls.Style
		{
            private bool _preserveWhiteSpace;
			private FontTransform _fontTransform;

			public FontTransform FontTransform
			{
				get { return _fontTransform; }
				set { _fontTransform = value; }
			}

            public bool PreserveWhiteSpace
            {
                get { return _preserveWhiteSpace; }
                set { _preserveWhiteSpace = value; }
            }

		}



}
