using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	[Serializable]
	public static class Workflows
	{
		//Generate the Terms collection (of Term) based on an existing Template
		public static List<Workflow> Create(string templateDef, Template template)
		{
			if (templateDef == null)
				return null;

			XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

			return Create(xmlTemplateDoc, template);
		}

		//Generate the Terms collection (of Term) based on an existing Template
		public static List<Workflow> Create(XmlDocument xmlTemplateDoc, Template template)
		{
			XmlNodeList nodeWorkflows = null;
			XmlNode nodeWorkflow = null;
			List<Workflow> rtn = new List<Workflow>();

			nodeWorkflows = xmlTemplateDoc.SelectNodes(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Workflows, XMLNames._E_Workflow));
			if (nodeWorkflows.Count == 0)
			{
				nodeWorkflow = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Workflow));
				if (nodeWorkflow == null)
				{
					//No Workflow defined
					throw new Exception(string.Format("No workflow defined for older structure template '{0}', ID = '{1}'", template.Name, template.ID.ToString()));
				}
				else
				{
					//This is the 'old' structure
                    Workflow workflow = new Workflow(nodeWorkflow, template is ManagedItem, template);
					template.ActiveWorkflowID = workflow.ID;
					Event revertToDraft = template.OldWorkflowRevertEvent;
					if (revertToDraft != null)
					{
						workflow.Events.Add(revertToDraft);
						template.OldWorkflowRevertEvent = null;
					}
					rtn.Add(workflow);
				}
			}
			else
			{
				//This is the 'new' structure - assume that RevertToDraft already set up properly.
				XmlNode singleNodeWorkflows = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Workflows));
				foreach (XmlNode nodeEachWorkflow in nodeWorkflows)
				{
                    rtn.Add(new Workflow(nodeEachWorkflow, template is ManagedItem, template));
				}

				string sActiveWorkflowID = string.Empty; 
				//if (sActiveWorkflowID == null)
				//   throw new Exception("The Active Workflow ID is not defined");
				sActiveWorkflowID = Utility.XMLHelper.GetAttributeString(singleNodeWorkflows, XMLNames._A_ActiveWorkflowID);
				if (Utility.TextHelper.IsGuid(sActiveWorkflowID))
					template.ActiveWorkflowID = new Guid(sActiveWorkflowID);
				else
					if (rtn.Count > 0)
						template.ActiveWorkflowID = rtn[0].ID;
					else
						throw new Exception("Unable to set the Active Workflow because no workflows are defined for this template.");
			}

			return rtn;
		}


		public static Guid Copy(Template template, Guid copyFromID, string newName)
		{
			Workflow workflowFrom = template.FindWorkflow(copyFromID);
			if (workflowFrom == null)
				return Guid.Empty;

            Workflow workflowCopy = workflowFrom.Copy(newName, template is ManagedItem, template);
			workflowCopy.Name = newName;
			template.Workflows.Add(workflowCopy);
			return workflowCopy.ID;
		}

		//Generate the Terms collection (of Term) based on an xml document
		public static bool Save(XmlDocument xmlTemplateDoc, Template template, bool bValidate)
		{
            //Convert the xml into an xmldocument
            //XmlDocument xmlTemplateDoc = new XmlDocument();
            //xmlTemplateDoc.PreserveWhitespace = false;
            //xmlTemplateDoc.LoadXml(template.TemplateDef);

            //Convert the objects stored in memory to an xmldocument
            XmlDocument xmlDoc = new XmlDocument();
			XmlElement nodeWorkflows = xmlDoc.CreateElement(XMLNames._E_Workflows);
			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeWorkflows, XMLNames._A_ActiveWorkflowID, template.ActiveWorkflowID.ToString());

			if (template.Workflows.Count > 0)
				foreach (Workflow workflow in template.Workflows)
				{
					XmlElement elementWorkflow = xmlDoc.CreateElement(XMLNames._E_Workflow);
					workflow.Build(xmlDoc, elementWorkflow, bValidate);
					nodeWorkflows.AppendChild(elementWorkflow);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeWorkflows = xmlTemplateDoc.ImportNode(nodeWorkflows, true);
			//Find the child node
			XmlNode workflowsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Workflows));
			XmlNode workflowChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Workflow));
			if (workflowsChildNode == null && workflowChildNode == null)
			{
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeWorkflows);
			}
			else
			{
				if (workflowsChildNode != null)
				{
					xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeWorkflows, workflowsChildNode);
				}
				else
				{
					//The 'old' structure
					xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeWorkflows, workflowChildNode);
				}
			}

			return true;
		}

		public static bool Save(Template template, ref string sXml, bool bValidate)
		{
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(sXml);

            //Convert the objects stored in memory to an xmldocument
            XmlDocument xmlDoc = new XmlDocument();
			XmlElement nodeWorkflows = xmlDoc.CreateElement(XMLNames._E_Workflows);
			Utility.XMLHelper.AddAttributeString(xmlDoc, nodeWorkflows, XMLNames._A_ActiveWorkflowID, template.ActiveWorkflowID.ToString());

			if (template.Workflows.Count > 0)
				foreach (Workflow workflow in template.Workflows)
				{
					XmlElement elementWorkflow = xmlDoc.CreateElement(XMLNames._E_Workflow);
					workflow.Build(xmlDoc, elementWorkflow, bValidate);
					nodeWorkflows.AppendChild(elementWorkflow);
				}

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedNodeWorkflows = xmlTemplateDoc.ImportNode(nodeWorkflows, true);
			//Find the "Document" child node
			XmlNode workflowsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Workflows));
			XmlNode workflowChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Workflow));
			if (workflowsChildNode == null && workflowChildNode == null)
			{
				xmlTemplateDoc.DocumentElement.AppendChild(importedNodeWorkflows);
			}
			else
			{
				if (workflowsChildNode != null)
				{
					xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeWorkflows, workflowsChildNode);
				}
				else
				{
					//The 'old' structure
					xmlTemplateDoc.DocumentElement.ReplaceChild(importedNodeWorkflows, workflowChildNode);
				}
			}

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

			return true;
		}

		public static List<string> TermReferences(Template template, string termName, Guid termID)
		{
			List<string> rtn = new List<string>();
			foreach (Workflow workflow in template.Workflows)
			{
				foreach (State state in workflow.States)
				{
					foreach (Action action in state.Actions)
					{
						foreach (Message message in action.Messages)
						{
							rtn.AddRange(message.TermReferences(template, termName, string.Format("State \"{0}\" Action \"{1}\"", state.Name, action.ButtonText)));
						}
					}
				}
			}

			return rtn;
		}

		public static void SubstituteTermNames(Template template)
		{
			foreach (Workflow workflow in template.Workflows)
			{
				foreach (State state in workflow.States)
				{
					foreach (Action action in state.Actions)
					{
						foreach (Message message in action.Messages)
						{
							message.Text = Business.Term.SubstituteTermNames(template, message.Text);
						}
					}
				}
			}
		}

		public static Workflow ActiveWorkflow(Template template)
		{
			if (template != null)
			{
				return template.FindWorkflow(template.ActiveWorkflowID);
			}
			return null;
		}

        //The 'roles' variable can be generated from the call 'Role.FromNames(List<string> names)';
        public static void SetStateTermGroupRoles(Template template, StateTermGroup.StateTermGroupRoleType stateTermGroupRoleType, List<Role> roles) 
        {
            foreach (Workflow workflow in template.Workflows)
            {
                foreach (State state in workflow.States)
                {
                    state.SetGroupRoles(stateTermGroupRoleType, roles);
                }
            }
        }

	}
}
