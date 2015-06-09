using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Kindred.Knect.ITAT.Business
{

	[Serializable]
	public enum TermDependencyActionValue
	{
		Default = 0,
		True,
		False
	}

	[Serializable]
	public class TermDependencyAction
	{
		#region private fields
		private TermDependencyActionValue _enabled;
		private TermDependencyActionValue _required;
		private string _setValue;
		#endregion

		#region properties
		public TermDependencyActionValue Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		public TermDependencyActionValue Required
		{
			get { return _required; }
			set { _required = value; }
		}

		public string SetValue
		{
			get { return Utility.XMLHelper.GetXMLText(_setValue); }
			set { _setValue = Utility.XMLHelper.SetXMLText(value); }
		}

		#endregion

		#region constructor

		public TermDependencyAction(TermDependencyActionValue enabled, TermDependencyActionValue required)
		{
			_enabled = enabled;
			_required = required;
		}

		public TermDependencyAction(XmlNode actionNode)
		{
			_enabled = (TermDependencyActionValue)Enum.Parse(typeof(TermDependencyActionValue), Utility.XMLHelper.GetAttributeString(actionNode, XMLNames._A_ActionEnabled));
			_required = (TermDependencyActionValue)Enum.Parse(typeof(TermDependencyActionValue), Utility.XMLHelper.GetAttributeString(actionNode, XMLNames._A_ActionRequired));
			_setValue = Utility.XMLHelper.GetAttributeString(actionNode, XMLNames._A_SetValue);
		}

		#endregion


		internal void Build(XmlDocument xmlDoc, XmlNode termActionNode, bool bValidate)
		{
			if (bValidate)
			{
				//TODO:  ???
			}
			Utility.XMLHelper.AddAttributeString(xmlDoc, termActionNode, XMLNames._A_ActionEnabled, _enabled.ToString());
			Utility.XMLHelper.AddAttributeString(xmlDoc, termActionNode, XMLNames._A_ActionRequired, _required.ToString());
			Utility.XMLHelper.AddAttributeString(xmlDoc, termActionNode, XMLNames._A_SetValue, _setValue);
		}


	}
}
