using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Xml;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.IO;


namespace Kindred.Knect.ITAT.Web
{
	public partial class ManagedItemImport : BaseSystemPage
	{

		protected override string GetApplicationFunction()
		{
			return Business.XMLNames._AF_ImportData;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			ClientScript.RegisterClientScriptInclude(this.GetType(), "_kh_jsITAT", "../Scripts/itat.js");
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (IsPostBack)
			{
				Server.ScriptTimeout = 900;
				int nErrorsReported;
				bool bSuccess = ImportData(out nErrorsReported);
				if (bSuccess)
				{
					if (nErrorsReported > 0)
						Server.Transfer(string.Format("~/Message.aspx?system=ITAT Data Import&message=The Data Import is Complete.     ****  Please check the application log for {0} error(s)!  ****", nErrorsReported.ToString()));
					else
						Server.Transfer("~/Message.aspx?system=ITAT Data Import&message=The Data Import is Complete");
				}
			}
			else
			{
				InitializeForm();
			}
		}


		private void InitializeForm()
		{
			List<short> statuses = new List<short>();
			statuses.Add((short)Business.TemplateStatusType.SearchOnly);
			statuses.Add((short)Business.TemplateStatusType.Active);

            using (DataSet ds = Business.Template.GetTemplateListWithStatus(_itatSystem.ID, statuses, null))
			{
				ddlTemplate.DataSource = ds;
				ddlTemplate.DataTextField = "TemplateName";
				ddlTemplate.DataValueField = "TemplateID";
				ddlTemplate.DataBind();
			}
			ddlTemplate.Items.Insert(0, new ListItem("(Please select a template)", ""));
		}


		private bool ImportData(out int nErrorsReported)
		{
			nErrorsReported = 0;
			if (ddlTemplate.SelectedIndex == 0)
			{
				RegisterAlert("A valid template must be selected.");
				return false;
			}

			if (filUpload.PostedFile == null || filUpload.PostedFile.ContentLength == 0)
			{
				RegisterAlert("A valid import file must be selected.");
				return false;
			}

			Guid templateId = new Guid(ddlTemplate.SelectedValue);

			XmlReader reader = new XmlTextReader(filUpload.PostedFile.InputStream);
			Business.ManagedItem currentManagedItem = null;

			while (reader.Read())
			{
				//TODO - Should a 'Validate' call be made against the MI as a whole before saving?
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:

						switch (reader.Name)
						{
							case "ManagedItem":
								{
									currentManagedItem = CreateManagedItem(templateId, reader);
									break;
								}
							case "Term":
								{
									string termName = reader.GetAttribute("Name");
									Business.Term term = currentManagedItem.FindBasicTerm(termName);
									if (term == null)
										throw new Exception(string.Format("Unable to find term named {0} in template {1} for Contract Number {2}", termName, currentManagedItem.Name, currentManagedItem.ItemNumber));
									string[] selectedValues = null;
									switch (term.TermType)
									{
										case Kindred.Knect.ITAT.Business.TermType.Text:
											{
												//TODO Note - how is a check made on the text format?
												((Business.TextTerm)term).Value = reader.GetAttribute("Value");
												break;
											}
										case Kindred.Knect.ITAT.Business.TermType.Date:
											{
												DateTime dt;
												string dateValue = reader.GetAttribute("Value");
												if (DateTime.TryParse(dateValue, out dt))
												{
													((Business.DateTerm)term).Value = dt;
												}
												else
												{
													throw new Exception(string.Format("Invalid value for Date Term named {0} in template {1} for Contract Number {2}: {3}", termName, currentManagedItem.Name, currentManagedItem.ItemNumber, dateValue));
												}
												break;
											}
										case Kindred.Knect.ITAT.Business.TermType.MSO:
											{
												Business.MSOTerm msoTerm = term as Business.MSOTerm;
												if (msoTerm == null)
													throw new Exception(string.Format("Error finding MSO Term named {0} on Contract Number {1}", termName, currentManagedItem.ItemNumber));
												msoTerm.MSOValue = reader.GetAttribute("MSOName");
												msoTerm.Address1Value = reader.GetAttribute("MSOAddress1");
												msoTerm.Address2Value = reader.GetAttribute("MSOAddress2");
												msoTerm.CityValue = reader.GetAttribute("MSOCity");
												msoTerm.StateValue = reader.GetAttribute("MSOState");
												msoTerm.ZipValue = reader.GetAttribute("MSOZip");
												msoTerm.PhoneValue = reader.GetAttribute("MSOPhone");
												break;
											}
										case Kindred.Knect.ITAT.Business.TermType.Renewal:
											{
												//May need to modify how Renewal term is handled for MCC.
												Business.RenewalTerm renewalTerm = term as Business.RenewalTerm;
												if (renewalTerm == null)
													throw new Exception(string.Format("Error finding Renewal Term named {0} on Contract Number {1}", termName, currentManagedItem.ItemNumber));

												DateTime effDate;
												int initialDurationCount;

												string effDateValue = reader.GetAttribute("EffectiveDate");
												string initialDurationCountValue = reader.GetAttribute("InitialDurationCount");

												if (!DateTime.TryParse(effDateValue, out effDate))
													throw new Exception(string.Format("Invalid Effective Date on Renewal Term of ContractNumber {0} : {1}", currentManagedItem.ItemNumber, effDateValue));
												if (!int.TryParse(initialDurationCountValue, out initialDurationCount))
													throw new Exception(string.Format("Invalid Initial Duration Count value on Renewal Term of ContractNumber {0} : {1}", currentManagedItem.ItemNumber, initialDurationCountValue));

												renewalTerm.RenewalTermType = ((reader.GetAttribute("RenewalType") == "None") ? Business.RenewalTermType.None : Business.RenewalTermType.Manual);
												renewalTerm.EffectiveDate = effDate;
												renewalTerm.InitialDurationUnitCount = initialDurationCount;
												renewalTerm.InitialDurationUnitSelected = reader.GetAttribute("InitialDurationUnit");
												if (!renewalTerm.IsTypeNone)
												{
													string renewalCountValue = reader.GetAttribute("RenewalCount");
													string renewalDurationCountValue = reader.GetAttribute("RenewalDurationCount");

													int renewalCount;
													int renewalDurationCount;

													if (!int.TryParse(renewalCountValue, out renewalCount))
														throw new Exception(string.Format("Invalid Renewal Count value on Renewal Term of ContractNumber {0} : {1}", currentManagedItem.ItemNumber, renewalCountValue));
													if (!int.TryParse(renewalDurationCountValue, out renewalDurationCount))
														throw new Exception(string.Format("Invalid Renewal Duration Count value on Renewal Term of ContractNumber {0} : {1}", currentManagedItem.ItemNumber, renewalDurationCountValue));

													renewalTerm.RenewalCount = renewalCount;
													renewalTerm.RenewalDurationUnitCount = renewalDurationCount;
													renewalTerm.RenewalDurationUnitSelected = reader.GetAttribute("RenewalDurationUnit");
												}
												break;
											}
										case Kindred.Knect.ITAT.Business.TermType.Facility:
											{
												Business.FacilityTerm facilityTerm = term as Business.FacilityTerm;
												if (facilityTerm == null)
													throw new Exception(string.Format("Error finding Facility Term named {0} on Contract Number {1}", termName, currentManagedItem.ItemNumber));
												int facilityID;
												if (!(facilityTerm.MultiSelect ?? false))
												{
													string facilityIDValue = reader.GetAttribute("Value");
													if (int.TryParse(facilityIDValue, out facilityID))
													{
														facilityTerm.SelectedFacilityIDs.Clear();
														facilityTerm.SelectedFacilityIDs.Add(facilityID);
													}
													else
														throw new Exception(string.Format("Invalid FacilityID value on {0} Term of ContractNumber {1} : {2}", termName, currentManagedItem.ItemNumber, facilityIDValue));
												}
												else
												{
													facilityTerm.SelectedFacilityIDs.Clear();
													selectedValues = reader.GetAttribute("Value").Trim().Split('|');
													foreach (string selectedValue in selectedValues)
													{
														if (int.TryParse(selectedValue, out facilityID))
															facilityTerm.SelectedFacilityIDs.Add(facilityID);
														else
															throw new Exception(string.Format("Invalid FacilityID value on {0} Term of ContractNumber {1} : {2}", termName, currentManagedItem.ItemNumber, selectedValue));
													}
												}
												break;
											}
										case Kindred.Knect.ITAT.Business.TermType.PickList:
											Business.PickListTerm pickListTerm = term as Business.PickListTerm;
											selectedValues = reader.GetAttribute("Value").Trim().Split('|');
											foreach (Business.PickListItem pickListItem in pickListTerm.PickListItems)
											{
												pickListItem.Selected = false;
												for (int i = 0, j = selectedValues.Length; i < j; i++)
												{
													if (pickListItem.Value == selectedValues[i])
													{
														pickListItem.Selected = true;
														break;
													}
												}
											}
											break;
										case Kindred.Knect.ITAT.Business.TermType.External:
											{
												Business.ExternalTerm extTerm = term as Business.ExternalTerm;
												XmlReader elementReader = reader.ReadSubtree();
												XmlDocument doc = new XmlDocument();
												doc.LoadXml(reader.ReadOuterXml());
												XmlDeclaration decl = doc.CreateXmlDeclaration("1.0", null, null);
												doc.InsertBefore(decl, doc.DocumentElement);
												foreach (XmlNode selectedItemNode in doc.DocumentElement.ChildNodes)
												{
													Business.ExternalInterfaceListItem selectedItem = new Kindred.Knect.ITAT.Business.ExternalInterfaceListItem(extTerm.InterfaceConfig);
													selectedItem.Key = selectedItemNode.Attributes["Key"].Value;
													foreach (XmlNode fieldValueNode in selectedItemNode.ChildNodes)
													{
														selectedItem.FieldValues.Add(fieldValueNode.Attributes["Name"].Value, fieldValueNode.Attributes["Value"].Value);
													}
													extTerm.SelectedItems.Add(selectedItem);
												}
												extTerm.ValuesLoaded = true;
												break;
											}
										case Kindred.Knect.ITAT.Business.TermType.Link:
										case Kindred.Knect.ITAT.Business.TermType.ComplexList:
										case Kindred.Knect.ITAT.Business.TermType.None:
										default:
											break;

									}
									break;
								}
							case "Attachments":
								{
									break;
								}
							case "Attachment":
								{
									string fullPath = reader.GetAttribute("Filename");
									string attachmentType = reader.GetAttribute("Type");

									//separate the file path into its parts
									string filename = fullPath.Substring(fullPath.LastIndexOf('\\') + 1);
									string fileExt = System.IO.Path.GetExtension(fullPath).Substring(1);
									string fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(filename);

									//store the file into the data store
									byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
									Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(_itatSystem.DocumentStorageType);
									documentStorageObject.RootPath = _itatSystem.DocumentStorageRootPath;

									try
									{
										string documentStoreId = documentStorageObject.SaveDocument(fileNameWithoutExt, fileExt, bytes);
										//create attachment and add it to the managed item
										Business.Attachment newAttachment = new Kindred.Knect.ITAT.Business.Attachment(Guid.NewGuid(), fullPath, "", documentStoreId, new Business.DocumentType(attachmentType), false);
										currentManagedItem.Attachments.Add(newAttachment);
									}
									catch
									{
										//If the exception occurs when saving to Documentum, the Exception is logged in the SaveDocument() method
										nErrorsReported++;
									}
									break;
								}
							case "Comment":
								{
									//Allow for invalid entries here?
									string dateValue = reader.GetAttribute("Date");
									DateTime date;
									if (!DateTime.TryParse(dateValue, out date))
										throw new Exception(string.Format("Invalid Date value on Comment for ContractNumber {0} : {1}", currentManagedItem.ItemNumber, dateValue));
									Business.Comment comment = new Business.Comment();
									comment.Created = date;
									comment.UserID = reader.GetAttribute("UserID");
									comment.UserName = reader.GetAttribute("Name");
									comment.Text = reader.GetAttribute("Text");
									currentManagedItem.Comments.Add(comment);
									break;
								}
							default:
								break;
						}
						break;
					case XmlNodeType.EndElement:
						if (reader.Name == "ManagedItem")
						{
							if (currentManagedItem != null)
							{
								try
								{
									bool validate = false;
									currentManagedItem.FirstSave(true, validate);  //initial insert
									//Make sure save to database ok here
                                    currentManagedItem.Update(validate, Business.Retro.AuditType.Saved);  //save to update comments and calculate scheduled events
								}
								catch (Exception ex)
								{
									throw new Exception(string.Format("Error saving contract number {0}.  Error: {1}", currentManagedItem.ItemNumber, ex.Message));
								}
							}
						}
						break;
					case XmlNodeType.Attribute:
					case XmlNodeType.CDATA:
					case XmlNodeType.Comment:
					case XmlNodeType.Document:
					case XmlNodeType.DocumentFragment:
					case XmlNodeType.DocumentType:
					case XmlNodeType.EndEntity:
					case XmlNodeType.Entity:
					case XmlNodeType.EntityReference:
					case XmlNodeType.None:
					case XmlNodeType.Notation:
					case XmlNodeType.ProcessingInstruction:
					case XmlNodeType.SignificantWhitespace:
					case XmlNodeType.Text:
					case XmlNodeType.Whitespace:
					case XmlNodeType.XmlDeclaration:
					default:
						break;
				}
			}
			return true;
		}


		private Kindred.Knect.ITAT.Business.ManagedItem CreateManagedItem(Guid templateId, XmlReader reader)
		{
			string contractNumber = reader.GetAttribute("Number");
			int? facId = null;
			if (_itatSystem.HasOwningFacility ?? false)
			{
				int ifacId;
				string strFacId = reader.GetAttribute("OwningFacility");
				if (!int.TryParse(strFacId, out ifacId))
					throw new Exception(string.Format("Invalid Facility ID ({0}) for Contract Number {1}", strFacId, contractNumber));
				facId = ifacId;
			}

			Business.ManagedItem mi = null;
			try
			{
				mi = Business.ManagedItem.Create(true, templateId, facId);
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error creating managed item for contract number {0}, templateid={1}, facId={2}.  Error: {3}", contractNumber, templateId, facId, ex.Message));
			}

			//We may not want to overwrite the dynamically created number???
			mi.ItemNumber = contractNumber;
			string strState = reader.GetAttribute("State");
			Business.State state = mi.Workflow.FindState(strState);
			if (state == null)
				throw new Exception(string.Format("Invalid State ({0}) for Contract Number {1}", strState, contractNumber));
			mi.State = state;
			return mi;
		}



		internal override HtmlGenericControl HTMLBody()
		{
			return htmlBody;
		}

		internal override Control ResizablePanel()
		{
			return null;
		}
	}
}
