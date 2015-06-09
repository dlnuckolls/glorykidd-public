using System;
using System.Collections.Generic;
using System.Text;
using Kindred.Common.Security;

namespace Kindred.Knect.ITAT.Business
{

	public static class EmailHelper
	{
        public static string EmailName(string systemName)
        {
            return string.Format("ITAT - {0}", systemName);
        }

		public static int SendEmail(string from, string subject, string body, Kindred.Common.Security.NameEmailPair[] recipients, ITATSystem system, List<string> roles, List<int> facilityIDs)
		{
			Kindred.Common.Email.Email email = new Kindred.Common.Email.Email();
			string emailBody = System.Web.HttpUtility.HtmlDecode(body);

			string[] overrideRecipients = system.OverrideEmail.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
			string[] ownerRecipients = system.OwnerEmail.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

			//Determine actual recipients and whether to add a "debug" or "recipients not found" message to the email body
			if (recipients.Length > 0)
			{
				if (overrideRecipients.Length > 0)
				{
					emailBody = DebugMessage(recipients) + emailBody;
					AddRecipients(email, overrideRecipients);
				}
				else
				{
					AddRecipients(email, recipients);  
				}
			}
			else
			{
				if (overrideRecipients.Length > 0)
				{
                    emailBody = DebugMessage(recipients) + emailBody + RecipientsNotFoundMessage(system, roles, facilityIDs);
					AddRecipients(email, overrideRecipients);
				}
				else
				{
                    emailBody = emailBody + RecipientsNotFoundMessage(system, roles, facilityIDs);
					AddRecipients(email, ownerRecipients);
				}
			}

			if (string.IsNullOrEmpty(email.To))
			{
                emailBody = emailBody + RecipientsNotFoundMessage(system, roles, facilityIDs);
				AddRecipients(email, ownerRecipients);
				if (string.IsNullOrEmpty(email.To))
				{
					throw new Exception("No e-mail recipients were found.   This should not happen, because the OwnerEmail attribute in the System Def XML should contain at least one e-mail address.  Report this issue to Support.");
				}
			}

			email.Subject = subject;
			email.From = from;
			email.Format = Kindred.Common.Email.EmailFormat.Html;
			email.Body = emailBody;
            email.ApplicationName = EmailName(system.Name);
			return email.Send();
		}


        private static string RecipientsNotFoundMessage(ITATSystem system, List<string> roles, List<int> facilityIDs)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<p /><p />");
			sb.Append("<p><b>The system was unable to determine the recipient for this message.  ");

            if (roles != null && roles.Count > 0)
            {
                sb.AppendFormat("The following role(s) were defined: {0}.  ", string.Join(",", roles.ToArray()).TrimEnd(','));
            }
            else
            {
                sb.Append("No roles were defined for this message.  ");
            }

            if (facilityIDs != null && facilityIDs.Count > 0)
            {
                System.Text.StringBuilder sbFacilityList = new System.Text.StringBuilder();
                sbFacilityList.Append("");
                foreach (int facilityID in facilityIDs)
                    sbFacilityList.AppendFormat("{0},", facilityID.ToString());
                sb.AppendFormat("The following facilities were defined:  {0}.    ", sbFacilityList.ToString().TrimEnd(','));
            }
            else
            {
                sb.Append("No facilities were defined for this message.    ");
            }
            sb.Append("This e-mail has been sent to you because you have designated as an owner of the ");
			sb.Append(system.Name);
			sb.Append(" system.</b></p>");
			sb.Append("<p /><p />");
			return sb.ToString();
		}


		private static string DebugMessage(Kindred.Common.Security.NameEmailPair[] recipients)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("<p><b>This is a test message.  If this were a real message it would be sent to the following recipients:</b></p>");
			for (int i = 0; i < recipients.Length; i++)
				sb.AppendFormat("<br /><b>{0} ({1})</b>", recipients[i].Email, recipients[i].Name);
			sb.Append("<p /><p />");
			return sb.ToString();
		}


		private static void AddRecipients(Kindred.Common.Email.Email email, string[] addresses)
		{
			for (int i = 0; i < addresses.Length; i++)
				email.AddTo(addresses[i]);
		}


		private static void AddRecipients(Kindred.Common.Email.Email email, Kindred.Common.Security.NameEmailPair[] recipients)
		{
			for (int i = 0; i < recipients.Length; i++)
				if (!string.IsNullOrEmpty(recipients[i].Email))
					email.AddTo(recipients[i].Email);
		}

        public static void SendNotificationToOwner(ITATSystem system, ManagedItem managedItem, string message, string errorMessage)
        {
            string[] ownerRecipients = system.OwnerEmail.Split(new char[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (ownerRecipients.Length > 0)
            {
                Kindred.Common.Email.Email email = new Kindred.Common.Email.Email();
                foreach (string recipient in ownerRecipients)
                    email.AddRecipient(Kindred.Common.Email.EmailRecipientType.To, recipient);
                email.Subject = string.Format("ITAT-{0} -- Unable to send notification for {1} {2}", system.Name, system.ManagedItemName, managedItem.ItemNumber);
                email.From = XMLNames._M_EmailFrom;
                email.Format = Kindred.Common.Email.EmailFormat.Html;
                email.Body = string.Format("<b>ITAT-{0} was unable to send a notification for {1} {2}.<br /><br />{3}</b><br /><br /><br />{4}", system.Name, system.ManagedItemName, managedItem.ItemNumber, errorMessage, message);
                email.ApplicationName = EmailName(system.Name);
                email.Send();
            }
        }

        public static void SendNotificationToRoles(ITATSystem system, string subject, string message, List<string> roles)
        {
            if (roles != null && roles.Count > 0)
            {
                Kindred.Common.Email.Email email = new Kindred.Common.Email.Email();
                Kindred.Common.Security.NameEmailPair[] recipients = SecurityHelper.GetEmailRecipients(system, roles);
                SendEmail(XMLNames._M_EmailFrom, subject, message, recipients, system, roles, null);
            }
        }
    }

}
