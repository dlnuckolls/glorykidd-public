using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public class Message
	{
		#region private members
		private string _subject;
		private string _text;
		private List<string> _recipients;
		private List<string> _notificationStatuses;
		private Guid? _filterFacilityTermID;

		#endregion

		#region Properties

		public string Subject
		{
			get { return Utility.XMLHelper.GetXMLText(_subject); }
			set { _subject = Utility.XMLHelper.SetXMLText(value); }
		}

		public string Text
		{
			get { return Utility.XMLHelper.GetXMLText(_text); }
			set { _text = Utility.XMLHelper.SetXMLText(value); }
		}

		public List<string> Recipients
		{
			get { return _recipients; }
			set { _recipients = value; }
		}

		public List<string> NotificationStatuses
		{
			get { return _notificationStatuses; }
			set { _notificationStatuses = value; }
		}

		public bool AllStatusesValid
		{
			get { return _notificationStatuses.Contains(XMLNames._M_AllStatuses); }
		}

		//This is used to store the FacilityTerm (Term)ID of the term that will be used to filter the recipients of this message.
		public Guid? FilterFacilityTermID
		{
			get { return _filterFacilityTermID; }
			set { _filterFacilityTermID = value; }
		}

		#endregion

		#region Constructors

		public Message()
		{
			_recipients = new List<string>();
			_notificationStatuses = new List<string>();
		}

		public Message(XmlNode node)
		{
			_subject = Utility.XMLHelper.GetAttributeString(node, XMLNames._A_Subject);
			_filterFacilityTermID = Term.CreateID(node, XMLNames._A_FilterFacilityTermID);
			if (_filterFacilityTermID == Guid.Empty)
				_filterFacilityTermID = null;

			XmlNode bodyNode = node.SelectSingleNode(XMLNames._E_Body);
			_text = Utility.XMLHelper.GetText(bodyNode);

			_recipients = new List<string>();
			XmlNodeList listRecipients = node.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Recipients, XMLNames._E_Recipient));
			if (listRecipients != null)
			{
				foreach (XmlNode nodeRecipient in listRecipients)
				{
					string sRecipient = Utility.XMLHelper.GetAttributeString(nodeRecipient, XMLNames._A_Role);
					_recipients.Add(sRecipient);
				}
			}

			_notificationStatuses = new List<string>();
			XmlNode nodeStatuses = node.SelectSingleNode(Utility.XMLHelper.GetXPath(false, XMLNames._E_Statuses));
			if (nodeStatuses != null)
			{
				if (Utility.XMLHelper.GetAttributeBool(nodeStatuses, XMLNames._A_AllStatuses) ?? false)
					_notificationStatuses.Add(XMLNames._M_AllStatuses);
				else
				{
					XmlNodeList listStatuses = node.SelectNodes(Utility.XMLHelper.GetXPath(false, XMLNames._E_Statuses, XMLNames._E_Status));
					foreach (XmlNode nodeStatus in listStatuses)
						_notificationStatuses.Add(Utility.XMLHelper.GetAttributeString(nodeStatus, XMLNames._A_Status));
				}
			}

		}

		#endregion

		#region Build XML

		public void Build(XmlDocument xmlDoc, XmlNode node, bool bValidate)
		{
			//if (bValidate)
			//{
			//   Utility.XMLHelper.ValidateString(XMLNames._A_Subject, _subject);
			//   Utility.XMLHelper.ValidateString(XMLNames._A_Text, _text);
			//}

			Utility.XMLHelper.AddAttributeString(xmlDoc, node, XMLNames._A_Subject, _subject);
			if (_filterFacilityTermID.HasValue)
				Term.StoreID(xmlDoc, node, null, null, XMLNames._A_FilterFacilityTermID, _filterFacilityTermID.Value);

			XmlNode elementBody = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Body, XMLNames._M_NameSpaceURI);
			node.AppendChild(elementBody);
			Utility.XMLHelper.AddText(xmlDoc, elementBody, _text);

			XmlNode elementRecipients = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Recipients, XMLNames._M_NameSpaceURI);
			node.AppendChild(elementRecipients);
			foreach (string recipient in _recipients)
			{
				XmlNode elementRecipient = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Recipient, XMLNames._M_NameSpaceURI);
				Utility.XMLHelper.AddAttributeString(xmlDoc, elementRecipient, XMLNames._A_Role, recipient);
				elementRecipients.AppendChild(elementRecipient);
			}

			bool allStatuses = AllStatusesValid; 
			XmlNode elementStatuses = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Statuses, XMLNames._M_NameSpaceURI);
			Utility.XMLHelper.AddAttributeString(xmlDoc, elementStatuses, XMLNames._A_AllStatuses, (allStatuses ? bool.TrueString : bool.FalseString));
			node.AppendChild(elementStatuses);
			if (!allStatuses)
			{
				foreach (string notificationStatus in _notificationStatuses)
				{
					XmlNode elementStatus = xmlDoc.CreateNode(XmlNodeType.Element, XMLNames._E_Status, XMLNames._M_NameSpaceURI);
					Utility.XMLHelper.AddAttributeString(xmlDoc, elementStatus, XMLNames._A_Status, notificationStatus);
					elementStatuses.AppendChild(elementStatus);
				}
			}
		}

		#endregion

		internal List<string> TermReferences(Template template, string termName, string sClient)
		{
			return Term.TermReferences(template, termName, System.Web.HttpUtility.HtmlDecode(Text), sClient, "notification");
		}

		private string EmailInfo(string from, Kindred.Common.Security.NameEmailPair[] toPairs, string subject, string text)
		{
			string to = "";
			if (toPairs != null && toPairs.Length > 0)
			{
				foreach (Kindred.Common.Security.NameEmailPair pair in toPairs)
				{
					to += string.Format("{0};", pair.Email);
				}
			}
			else
			{
				to = "NOT-DEFINED";
			}
			from = string.IsNullOrEmpty(from) ? "NOT-DEFINED" : from;
			subject = string.IsNullOrEmpty(subject) ? "NOT-DEFINED" : subject;
			text = string.IsNullOrEmpty(text) ? "NOT-DEFINED" : text;
			return string.Format("From:{0} To:{1} Subject:{2} Text:{3}", from, to, subject, text);
		}

		public string Send(ManagedItem managedItem, ITATSystem system, string sEnvironment, List<int> owningFacilityIDs)
		{
			//If the subject or body or recipients are empty, then do not send anything
			if ((string.IsNullOrEmpty(Subject)) || (string.IsNullOrEmpty(this.Text)) || (this.Recipients == null) || (this.Recipients.Count == 0))
				return string.Empty;

			string sError = string.Empty;

			string from = null;
			string subject = null;
			string text = null;
			Kindred.Common.Security.NameEmailPair[] emailRecipients = null;

			try
			{
				Kindred.Common.Email.Email email = new Kindred.Common.Email.Email();
				from = XMLNames._M_EmailFrom;
				subject = string.IsNullOrEmpty(Subject) ? "Subject Missing" : Subject;
				text = string.IsNullOrEmpty(Text) ? "Text Missing" : Text;

				Term.SubstituteBasicTerms(managedItem, ref text);
				Term.SubstituteSpecialTerms(ref text, managedItem);

				//Now complete the 'ManagedItemReference' email links - fill in the dynamically supplied info
				if (!string.IsNullOrEmpty(system.Name))
					text = text.Replace(XMLNames._M_SystemNameHolder, system.Name);
				if (!string.IsNullOrEmpty(sEnvironment))
					text = text.Replace(XMLNames._M_EnvironmentHolder, sEnvironment);
				text = text.Replace(XMLNames._M_ManagedItemIdHolder, managedItem.ManagedItemID.ToString());

				List<int> facilityIDs = null;
				if (owningFacilityIDs != null)
					facilityIDs = new List<int>(owningFacilityIDs);
				else
					facilityIDs = new List<int>();
				bool overrideFacilities = false;
				//Note - HasOwningFacility trumps use of filterFacilityTerm
				if (!(system.HasOwningFacility ?? false))
				{
					if (FilterFacilityTermID.HasValue)
					{
						Term filterFacilityTerm = managedItem.FindTerm(FilterFacilityTermID.Value);
						if (filterFacilityTerm != null)
						{
							FacilityTerm facilityTerm = null;
							if (filterFacilityTerm.TermType == TermType.Facility)
								facilityTerm = filterFacilityTerm as FacilityTerm;
							else
								facilityTerm = new FacilityTerm(false, managedItem, filterFacilityTerm);
							if (facilityTerm != null)
							{
								facilityIDs = facilityTerm.SelectedFacilityIDs;
								overrideFacilities = true;
							}
						}
					}
				}

				List<int> allRecipientFacilityIDs = new List<int>();
				
				if ((system.HasOwningFacility ?? false) || overrideFacilities)
				{
					foreach (string recipient in Recipients)
					{
						List<int> absoluteLevels = null;
						List<int> relativeLevels = null;
						system.GetFacilityLevels(recipient, ref absoluteLevels, ref relativeLevels);
						List<int> recipientFacilityIDs = FacilityCollection.FacilityAncestry(facilityIDs, absoluteLevels, relativeLevels);
						foreach (int facilityID in recipientFacilityIDs)
							if (!allRecipientFacilityIDs.Contains(facilityID))
								allRecipientFacilityIDs.Add(facilityID);
					}
					emailRecipients = SecurityHelper.GetEmailRecipients(system, Recipients, allRecipientFacilityIDs);
				}
				else
				{
					emailRecipients = SecurityHelper.GetEmailRecipients(system, Recipients);
				}

				EmailHelper.SendEmail(from, subject, text, emailRecipients, system, Recipients, allRecipientFacilityIDs);
			}
			catch (Exception e)
			{
				sError = string.Format("Email={0} Exception={1}", EmailInfo(from, emailRecipients, subject, text), e.ToString());
			}
			return sError;
		}

		public Message Copy()
		{
			Message message = new Message();
			message.Subject = Subject;
			message.FilterFacilityTermID = FilterFacilityTermID;
			message.Text = Text;
			message.Recipients.Clear();
			foreach (string recipient in Recipients)
			{
				message.Recipients.Add(recipient);
			}
			message.NotificationStatuses.Clear();
			foreach (string notificationStatus in NotificationStatuses)
			{
				message.NotificationStatuses.Add(notificationStatus);
			}
			return message;
		}

	}
}
