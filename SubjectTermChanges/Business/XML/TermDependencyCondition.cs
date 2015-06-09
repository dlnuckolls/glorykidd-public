using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class TermDependencyCondition
	{

		
		#region private fields

		private string _sourceTerm;
		private Guid _sourceTermID;
		private string _oper;
		private string _value1;
		private string _value2;
		//This reference is needed when converting from TermID to TermName
		private Template _template;

		#endregion


		#region instance properties

		//Note - This is needed by the GUI for binding.
		public string SourceTerm
		{
			get	{ return _template.FindTermName(_sourceTermID, _sourceTerm); }
		}

		public Guid SourceTermID
		{
			get { return _sourceTermID; }
			set { 
					_sourceTermID = value;
					_sourceTerm = null;
				}
		}

		public string Oper
		{
			get { return _oper; }
			set { _oper = value; }
		}

		public string Value1
		{
			get { return _value1; }
			set { _value1 = value; }
		}

		public string Value2
		{
			get { return _value2; }
			set { _value2 = value; }
		}

		#endregion

		#region constructor

		public TermDependencyCondition(Template template, string sourceTerm, string oper, string value1, string value2)
		{
			_template = template;
			_sourceTermID = _template.FindTerm(sourceTerm).ID;
			_oper = oper;
			_value1 = value1;
			_value2 = value2;
		}

		public TermDependencyCondition(Template template, XmlNode conditionNode)
		{
			_template = template;
			string sourceTerm = Utility.XMLHelper.GetAttributeString(conditionNode, XMLNames._A_SourceTerm);
			if (!string.IsNullOrEmpty(sourceTerm))
			{
				_sourceTermID = _template.FindTerm(sourceTerm).ID;
			}
			else
			{
				_sourceTermID = Term.CreateID(conditionNode, XMLNames._A_SourceTermID);
			}
			_oper = Utility.XMLHelper.GetAttributeString(conditionNode, XMLNames._A_Oper);
			_value1 = Utility.XMLHelper.GetAttributeString(conditionNode, XMLNames._A_Value1);
			_value2 = Utility.XMLHelper.GetAttributeString(conditionNode, XMLNames._A_Value2);
		}

		#endregion


		#region instance methods

		public bool ConditionMet(Term managedItemSourceTerm)
		{
			string termValue = managedItemSourceTerm.Runtime.SetValue;
			if (string.IsNullOrEmpty(termValue) || (termValue == Term._SET_VALUE_DEFAULT))
				termValue = managedItemSourceTerm.DisplayValue("");	

			TextTerm tt = managedItemSourceTerm as TextTerm;
			if (tt != null)
				return ConditionMet(tt);
			else
			{
				switch (_oper)
				{
					case XMLNames._TermDependencyOperator_Equals:
						return termValue.CompareTo(_value1) == 0;
					case XMLNames._TermDependencyOperator_NotEqual:
						return termValue.CompareTo(_value1) != 0;
					case XMLNames._TermDependencyOperator_GreaterThan:
						return termValue.CompareTo(_value1) > 0;
					case XMLNames._TermDependencyOperator_LessThan:
						return termValue.CompareTo(_value1) < 0;
					case XMLNames._TermDependencyOperator_NoLessThan:
						return termValue.CompareTo(_value1) >= 0;
					case XMLNames._TermDependencyOperator_NoMoreThan:
						return termValue.CompareTo(_value1) <= 0;
					case XMLNames._TermDependencyOperator_Contains:
						return termValue.Contains(_value1);
					case XMLNames._TermDependencyOperator_StartsWith:
						return termValue.StartsWith(_value1);
					case XMLNames._TermDependencyOperator_EndsWith:
						return termValue.EndsWith(_value1);
					case XMLNames._TermDependencyOperator_Between:
						return ((termValue.CompareTo(_value1) >= 0) && (termValue.CompareTo(_value2) <= 0));
					default:
						return false;
				}
			}
		}


		private bool ConditionMet(TextTerm sourceTextTerm)
		{
			switch (sourceTextTerm.Format)
			{
				case TextTermFormat.Number:
				case TextTermFormat.Currency:
					{
						decimal termValue;
						decimal value1;
						decimal value2;
						if (!decimal.TryParse(sourceTextTerm.UnformattedValue, out termValue))
							return false;
						if (!decimal.TryParse(_value1, out value1))
							value1 = decimal.MinValue;
						if (!decimal.TryParse(_value2, out value2))
							value2 = decimal.MaxValue;
						switch (_oper)
						{
							case XMLNames._TermDependencyOperator_Equals:
								return termValue == value1;
							case XMLNames._TermDependencyOperator_NotEqual:
								return termValue != value1;
							case XMLNames._TermDependencyOperator_GreaterThan:
								return termValue > value1;
							case XMLNames._TermDependencyOperator_LessThan:
								return termValue < value1;
							case XMLNames._TermDependencyOperator_NoLessThan:
								return termValue >= value1;
							case XMLNames._TermDependencyOperator_NoMoreThan:
								return termValue <= value1;
							case XMLNames._TermDependencyOperator_Between:
								return ((termValue >= value1) && (termValue <= value2));
							default:
								return false;
						}
					}

				case TextTermFormat.Plain:
				case TextTermFormat.SSN:
				case TextTermFormat.Phone:
				case TextTermFormat.PhonePlusExtension:
				default:
					{
						string termValue = sourceTextTerm.UnformattedValue;
						if (string.IsNullOrEmpty(termValue))
							return false;
						switch (_oper)
						{
							case XMLNames._TermDependencyOperator_Equals:
								return termValue.CompareTo(_value1) == 0;
							case XMLNames._TermDependencyOperator_NotEqual:
								return termValue.CompareTo(_value1) != 0;
							case XMLNames._TermDependencyOperator_StartsWith:
								return termValue.StartsWith(_value1);
							case XMLNames._TermDependencyOperator_EndsWith:
								return termValue.EndsWith(_value1);
							case XMLNames._TermDependencyOperator_Between:
								return ((termValue.CompareTo(_value1) >= 0) && (termValue.CompareTo(_value2) <= 0));
							default:
								return false;
						}
					}
			}
		}


		#endregion


		#region static methods

		internal void Build(XmlDocument xmlDoc, XmlNode termDependencyNode, bool bValidate)
		{
			if (bValidate)
			{
				//TODO:  ???
			}
			Term.StoreID(xmlDoc, termDependencyNode, XMLNames._A_SourceTerm, _sourceTerm, XMLNames._A_SourceTermID, _sourceTermID);

			Utility.XMLHelper.AddAttributeString(xmlDoc, termDependencyNode, XMLNames._A_Oper, _oper);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termDependencyNode, XMLNames._A_Value1, _value1);
			Utility.XMLHelper.AddAttributeString(xmlDoc, termDependencyNode, XMLNames._A_Value2, _value2);
		}


		#endregion


	}
}
