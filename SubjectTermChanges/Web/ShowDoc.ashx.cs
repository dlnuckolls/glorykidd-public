using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using Kindred.Common.WebServices;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{

	[WebService(Namespace = "http://kindredhealthcare.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ShowPdf : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			switch (context.Request.QueryString["type"])
			{
				case "preview":
					{
						string systemId = context.Request.QueryString[Common.Names._QS_ITAT_SYSTEM_ID];
						string managedItemID = context.Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
						string templateID = string.Empty;
                        bool showDefaultDocument = Convert.ToBoolean(context.Request.QueryString[Common.Names._QS_SHOW_DEFAULT_DOCUMENT]);
                        Guid ITATDocumentID = new Guid();
                        if (!showDefaultDocument)
                        {
                            ITATDocumentID = new Guid(context.Request.QueryString[Common.Names._QS_ITAT_DOCUMENT_ID]);
                        }


						if (string.IsNullOrEmpty(managedItemID))
						{
							templateID = context.Request.QueryString[Common.Names._QS_TEMPLATE_ID];
							if (string.IsNullOrEmpty(templateID))
								throw new Exception(string.Format("'{0}' Query String not supplied.     Query string:  '{1}'.", Common.Names._QS_TEMPLATE_ID, context.Request.QueryString.ToString()));
						}
                        Render(context, "preview.pdf", GetPreviewPdf(systemId, managedItemID, templateID, ITATDocumentID, showDefaultDocument));
						break;
					}

				case "summary":
					{
						string managedItemId = context.Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
						if (string.IsNullOrEmpty(managedItemId))
							throw new Exception(string.Format("'{0}' Query String not supplied.     Query string:  '{1}'.", Common.Names._QS_MANAGED_ITEM_ID, context.Request.QueryString.ToString()));
						else
							if (!Utility.TextHelper.IsGuid(managedItemId))
								throw new Exception(string.Format("id is not a valid Guid: {0}", managedItemId));

						string systemManagedItemName = context.Request.QueryString[Common.Names._QS_MANAGED_ITEM_NAME];
						if (string.IsNullOrEmpty(systemManagedItemName))
							systemManagedItemName = "Item";
						Render(context, "summary.pdf", GetSummaryPdf(managedItemId, systemManagedItemName));
						break;
					}

				case "manageditemview":
					{
						string managedItemId = context.Request.QueryString[Common.Names._QS_MANAGED_ITEM_ID];
                        string ITATDocumentID = context.Request.QueryString[Common.Names._QS_ITAT_DOCUMENT_ID];


                        if (string.IsNullOrEmpty(ITATDocumentID.Trim()))
                        {
                            throw new ApplicationException(string.Format("'{0}' Query String not supplied.     Query string:  '{1}'.", Common.Names._QS_ITAT_DOCUMENT_ID, context.Request.QueryString.ToString()));
                        }


						if (string.IsNullOrEmpty(managedItemId))
							throw new Exception(string.Format("'{0}' Query String not supplied.     Query string:  '{1}'.", Common.Names._QS_MANAGED_ITEM_ID, context.Request.QueryString.ToString()));
						else
							if (!Utility.TextHelper.IsGuid(managedItemId))
								throw new Exception(string.Format("id is not a valid Guid: {0}", managedItemId));

						string systemManagedItemName = context.Request.QueryString[Common.Names._QS_MANAGED_ITEM_NAME];
						if (string.IsNullOrEmpty(systemManagedItemName))
							systemManagedItemName = "Item";

						Render(context, "manageditem.pdf", GetManagedItemView(managedItemId, systemManagedItemName, new Guid(ITATDocumentID)));
						break;
					}

				case "attachment":
				case "extension":
					{
						string systemId = context.Request.QueryString[Common.Names._QS_ITAT_SYSTEM_ID];
						string documentId = context.Request.QueryString[Common.Names._QS_DOCUMENT_ID];
						string filename = context.Request.QueryString[Common.Names._QS_FILENAME];
						string contentType;
						byte[] doc = GetDocument(systemId, documentId, filename, out contentType);
						Render(context, filename, doc, contentType);
						break;
					}

				default:
					break;
			}
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}


		private byte[] GetDocument(string systemId, string documentId, string filename, out string contentType)
		{
			Business.ITATSystem itatSystem = Business.ITATSystem.Get(new Guid(systemId));
			Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(itatSystem.DocumentStorageType);
			return documentStorageObject.GetDocument(itatSystem.ID, documentId, out contentType);
		}


		private byte[] GetPreviewPdf(string systemId, string managedItemId, string templateId, Guid ITATDocumentID, bool showDefaultDocument)
		{
			Business.ITATSystem itatSystem = Business.ITATSystem.Get(new Guid(systemId));
			Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(itatSystem.DocumentStorageType);
			Business.Template template = null;
			if (string.IsNullOrEmpty(managedItemId))
                //Changed for Multiple Documents
				template = new Kindred.Knect.ITAT.Business.Template(new Guid(templateId), Kindred.Knect.ITAT.Business.DefType.Draft);
  
			else
				template = Kindred.Knect.ITAT.Business.ManagedItem.Get(new Guid(managedItemId), true);
			template.SubstituteTerms();

            string xml = default(string);

            if (showDefaultDocument)
            {
                ITATDocument defaultDocument = template.GetDefaultITATDocument();
                xml = defaultDocument.CreateXml(true, itatSystem.ManagedItemName, string.Empty, template, defaultDocument.ITATDocumentID);
            }
            else
            {
                xml = template.GetITATDocument(ITATDocumentID).CreateXml(true, itatSystem.ManagedItemName, string.Empty, template, ITATDocumentID);
            }

#if DEBUG
            string templateXml = template.TemplateDef;
			string folderName = @"C:\temp\ITAT\";
			if (!System.IO.Directory.Exists(folderName))
				System.IO.Directory.CreateDirectory(folderName);
			string filename = string.Format("{0}PreviewPDF-{1:yyyyMMdd-hhmmss}.xml", folderName, DateTime.Now);
			if (System.IO.File.Exists(filename))
				System.IO.File.Delete(filename);
			System.IO.File.WriteAllText(filename, xml);
			string templateFilename = string.Format("{0}Template-{1:yyyyMMdd-hhmmss}.xml", folderName, DateTime.Now);
			if (System.IO.File.Exists(templateFilename))
				System.IO.File.Delete(templateFilename);
			System.IO.File.WriteAllText(templateFilename, templateXml);
#endif

			//get the template's Extensions (if any) from the DocumentStore (e.g. Documentum) and call the PDF Service to generate the PDF document
			byte[] bytes;
			int extensionCount = template.Extensions.Count;
            if (extensionCount > 0 && (showDefaultDocument || (template.GetITATDocument(ITATDocumentID).DefaultDocument ?? false)))
            {
				byte[][] extensions = new byte[template.Extensions.Count][];
				for (int i = 0; i < extensionCount; i++)
				{
					string extensionContentType = string.Empty;
					extensions[i] = documentStorageObject.GetDocument(itatSystem.ID, template.Extensions[i].ObjectID, out extensionContentType);
				}
				bytes = Kindred.Common.WebServices.WebServiceFactory<PDFService.Service>.CreateService(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine).GeneratePDF2(xml, true, extensions, true, true);
			}
			else
			{
				bytes = Kindred.Common.WebServices.WebServiceFactory<PDFService.Service>.CreateService(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine).GeneratePDF4(xml, true, true, true);
			}
			return bytes;
		}


		private byte[] GetSummaryPdf(string managedItemId, string systemManagedItemName)
		{
			Business.ManagedItem managedItem = Business.ManagedItem.Get(new Guid(managedItemId), true);
			managedItem.ApplyTermDependencies(true,null);
            Guid itatSystemId = managedItem.SystemID;
            Business.ITATSystem itatSystem = Business.ITATSystem.Get(itatSystemId);
            Business.SecurityHelper securityHelper = new Kindred.Knect.ITAT.Business.SecurityHelper(itatSystem);
			Business.ManagedItemSummary miSummary = new Business.ManagedItemSummary(managedItem);
            string xml = miSummary.CreateXml(systemManagedItemName, securityHelper.UserRoles);	
			return Kindred.Common.WebServices.WebServiceFactory<PDFService.Service>.CreateService(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine).GeneratePDF4(xml, managedItem.State.IsDraft ?? false, true, true);
		}


		private byte[] GetManagedItemView(string managedItemId, string systemManagedItemName, Guid ITATDocumentID)
		{
			Business.ManagedItem managedItem = Business.ManagedItem.Get(new Guid(managedItemId), true);
			managedItem.ApplyTermDependencies(true,null);
			Guid itatSystemId = managedItem.SystemID;
			Business.ITATSystem itatSystem = Business.ITATSystem.Get(itatSystemId);
			Utility.DocumentStorage documentStorageObject = Utility.DocumentStorage.GetDocumentStorageObject(itatSystem.DocumentStorageType);
			documentStorageObject.RootPath = itatSystem.DocumentStorageRootPath;

			//Determine if document should be dynamically generated or pulled from a document store
            //Note - This does not take any action for the case of Draft -> NonDraft -> Draft.
			byte[] doc;


            //In case of the old structure, just get the default document
            if (!managedItem.isNewDocumentStructure())
                ITATDocumentID = managedItem.GetDefaultITATDocument().ITATDocumentID;


            if (((managedItem.GetITATDocument(ITATDocumentID).WorkflowEnabled ?? false) && (managedItem.State.IsDraft ?? false)) || (!managedItem.GetITATDocument(ITATDocumentID).WorkflowEnabled ?? false))
            {
                doc = GenerateDocument(managedItem, systemManagedItemName, itatSystemId, documentStorageObject, ITATDocumentID);
            }
            else
            {

                //Changed for Multiple Documents
                if (string.IsNullOrEmpty(managedItem.GetITATDocument(ITATDocumentID).GeneratedDocumentID))
                {
                    doc = GenerateDocument(managedItem, systemManagedItemName, itatSystemId, documentStorageObject, ITATDocumentID);
                    //Changed for Multiple Documents
                    managedItem.UpdateGeneratedDocument(documentStorageObject.SaveDocument("GeneratedDocument.pdf", "pdf", doc), ITATDocumentID);
                    return doc;
                }
                else
                {
                    //Changed for Multiple Documents
                    string contentType;
                    doc = documentStorageObject.GetDocument(itatSystemId, managedItem.GetITATDocument(ITATDocumentID).GeneratedDocumentID, out contentType);
                }
            }
            return doc;
        }


        private byte[] GenerateDocument(Business.ManagedItem managedItem, string systemManagedItemName, Guid itatSystemId, Utility.DocumentStorage documentStorageObject, Guid ITATDocumentID)
		{
			managedItem.SubstituteTerms();
            string xml = managedItem.GetITATDocument(ITATDocumentID).CreateXml(false, systemManagedItemName, managedItem.ItemNumber, managedItem, ITATDocumentID);
#if DEBUG
			string templateXml = managedItem.TemplateDef;
			string folderName = @"C:\temp\ITAT\";
			if (!System.IO.Directory.Exists(folderName))
				System.IO.Directory.CreateDirectory(folderName);
			string xmlFilename = string.Format("{0}GeneratedPDF-{1:yyyyMMdd-hhmmss}.xml", folderName, DateTime.Now);
			if (System.IO.File.Exists(xmlFilename))
				System.IO.File.Delete(xmlFilename);
			System.IO.File.WriteAllText(xmlFilename, xml);
			string templateFilename = string.Format("{0}Template-{1:yyyyMMdd-hhmmss}.xml", folderName, DateTime.Now);
			if (System.IO.File.Exists(templateFilename))
				System.IO.File.Delete(templateFilename);
			System.IO.File.WriteAllText(templateFilename, templateXml);
#endif
			//Changed for Multiple Documents
			//get the template's Extensions (if any) from the DocumentStore (e.g. Documentum) and call the PDF Service to generate the PDF document
			byte[] rtn;
			int extensionCount = managedItem.Extensions.Count;
            if (extensionCount > 0 && (managedItem.GetITATDocument(ITATDocumentID).DefaultDocument ?? false))
			{
				byte[][] extensions = new byte[managedItem.Extensions.Count][];
				for (int i = 0; i < extensionCount; i++)
				{
					string extensionContentType = string.Empty;
					extensions[i] = documentStorageObject.GetDocument(itatSystemId, managedItem.Extensions[i].ObjectID, out extensionContentType);
				}
                rtn = Kindred.Common.WebServices.WebServiceFactory<PDFService.Service>.CreateService(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine).GeneratePDF2(xml, ((managedItem.GetITATDocument(ITATDocumentID).WorkflowEnabled ?? false) && (managedItem.State.IsDraft ?? false)), extensions, true, true);
			}
			else
			{
                rtn = Kindred.Common.WebServices.WebServiceFactory<PDFService.Service>.CreateService(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine).GeneratePDF4(xml, ((managedItem.GetITATDocument(ITATDocumentID).WorkflowEnabled ?? false) && (managedItem.State.IsDraft ?? false)), true, true);
			}
			return rtn;
		}



		protected void Render(HttpContext context, string name, byte[] bytes, string contentType)
		{
			context.Response.Clear();
			context.Response.ContentType = contentType;
			context.Response.AddHeader("Content-Header", bytes.Length.ToString());
			context.Response.AddHeader("Content-Disposition", string.Format("inline; filename={0}", name));
			context.Response.OutputStream.Write(bytes, 0, bytes.Length);
		}


		protected void Render(HttpContext context, string name, byte[] bytes)
		{
			Render(context, name, bytes, "application/pdf");
		}


	}
}
