using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class Comment : IComparable<Comment>
	{
		#region private members

		private string _userName;
		private string _userID;
		private DateTime? _created;
		private string _text;

		#endregion

		#region Properties

		public string UserName
		{
			get { return Utility.XMLHelper.GetXMLText(_userName); }
			set { _userName = Utility.XMLHelper.SetXMLText(value); }
		}

		public string UserID
		{
			get { return Utility.XMLHelper.GetXMLText(_userID); }
			set { _userID = Utility.XMLHelper.SetXMLText(value); }
		}

		public DateTime? Created
		{
			get { return _created; }
			set { _created = value; }
		}

		public string Text
		{
			get { return Utility.XMLHelper.GetXMLText(_text); }
			set { _text = Utility.XMLHelper.SetXMLText(value); }
		}

		#endregion

		#region Constructors

		public Comment()
		{
		}

		public Comment(XmlNode node)
		{
			_userName = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_UserName);
			_userID = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_UserID);
			_created = Utility.DateHelper.GetXMLDate(Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Created));

			_text = Utility.XMLHelper.GetText(node);
		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			if (bValidate)
			{
			}

			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_UserName, _userName);
			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_UserID, _userID);

			if (_created.HasValue)
			{
				string sCreated = Utility.DateHelper.SetXMLDate(_created.Value);
				Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Created, sCreated);
			}

			Utility.XMLHelper.AddText(xmlDoc, node, _text);
		}

		#endregion


		#region IComparable<Comment> Members

		//A comment is "less than" another comment if it is more recent than the other comment.   
		//This is so that after the comments are sorted, the most recent comments appear at the top of the list.
		public int CompareTo(Comment other)
		{
			if (this.Created.HasValue)
				if (other.Created.HasValue)
					return other.Created.Value.CompareTo(this.Created.Value);
				else
					return 1;
			else
				if (other.Created.HasValue)
					return -1;
				else
					return this.UserID.CompareTo(other.UserID);
		}

		#endregion


	}
}
