using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;

namespace Kindred.Knect.ITAT.Business
{
	public static class BasicTerms
	{
		//Generate the Terms collection (of Term) based on an existing Template
        //2090429 - This will be called with a param for the XPath, depending...
        //This is called from ManagedItem.Create and from BasicTerms prop-get
		public static List<Term> Create(string templateDef, ManagedItem managedItem, Template template)
		{
			if (templateDef == null)
				return null;
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(templateDef);

            return Create(xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Terms)), managedItem, template);
		}

		//Generate the Terms collection (of Term) based on an existing Template
        //Called by ManagedItem.GetBasic and by ManagedItem.Get
        public static List<Term> Create(XmlDocument xmlTemplateDoc, ManagedItem managedItem, Template template)
		{
            return Create(xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef, XMLNames._E_Terms)), managedItem, template);
		}

		//Generate the Terms collection (of Term) based on an xml document
        //Called by BasicTerms.Create, once from ITATSystem.LoadFromDatabase
 		public static List<Term> Create(XmlNode termsNode, ManagedItem managedItem, Template template)
		{
			List<Term> rtn = new List<Term>(termsNode.ChildNodes.Count);
            int order = 0;
			foreach (XmlNode termNode in termsNode)
			{
				Term t = null;
				switch (termNode.Name.ToString())
				{
					case XMLNames._E_Text:
                        t = new TextTerm(termNode, template, false);
						break;
					case XMLNames._E_Date:
                        t = new DateTerm(termNode, template, false);
						break;
					case XMLNames._E_MSO:
                        t = new MSOTerm(termNode, template, false);
						break;
					case XMLNames._E_Link:
                        t = new LinkTerm(termNode, template, false);
						break;
					case XMLNames._E_Facility:
                        t = new FacilityTerm(termNode, template, false);
						break;
					case XMLNames._E_Renewal:
                        t = new RenewalTerm(termNode, managedItem != null, template, false);
						break;
					case XMLNames._E_PickList:
                        t = new PickListTerm(termNode, template, false);
						break;
					case XMLNames._E_External:
                        t = new ExternalTerm(termNode, managedItem, template, false);
						break;
                    case XMLNames._E_PlaceHolderAttachments:
                        t = new PlaceHolderAttachments(termNode, template, false);
                        break;
                    case XMLNames._E_PlaceHolderComments:
                        t = new PlaceHolderComments(termNode, template, false);
                        break;
                    default:
						throw new XmlException(string.Format("Tried to create undefined term type {0}", termNode.Name.ToString()));
				}
                t.Order = order++;
                rtn.Add(t);
			}
            //If this is not a load of the ITATSystem terms, then ensure that the collection includes the Attachments and Comments terms.
            if (template != null)
            {
                List<Term> placeholderTerms = FindTermsOfType(rtn, (TermType.PlaceHolderAttachments | TermType.PlaceHolderComments));
                if (placeholderTerms == null || (placeholderTerms != null && placeholderTerms.Count == 0))
                {
                    //If this is the first time these are being added, then this should be under Basic Security.
                    Term t = new PlaceHolderAttachments(false, template, false);
                    t.TermGroupID = template.BasicSecurityTermGroupID;
                    t.Order = order++;
                    rtn.Add(t);

                    t = new PlaceHolderComments(false, template, false);
                    t.TermGroupID = template.BasicSecurityTermGroupID;
                    t.Order = order++;
                    rtn.Add(t);
                }
                else if (placeholderTerms.Count != 2)
                {
                    throw new Exception(string.Format("Encountered a PlaceHolder term count of {0:D} when 2 were expected", placeholderTerms.Count));
                }
            }
			return rtn;
		}

        public static List<Term> FindTermsOfType(List<Term> terms, TermType termType)
        {
            Predicate<Term> p = delegate(Term t) { return ((t.TermType & termType) > 0); };
            return terms.FindAll(p);
        }

        public static List<Term> FindTermsOfTypeExcluding(List<Term> terms, TermType termType)
        {
            Predicate<Term> p = delegate(Term t) { return ((t.TermType & termType) == 0); };
            return terms.FindAll(p);
        }

		public static List<string> ReplaceEmbeddedTermNames(Template template, List<Term> terms)
		{
			List<string> sErrors = new List<string>();
			if (terms != null)
				foreach (Term term in terms)
				{
					switch (term.TermType)
					{
						//Does not apply to these term types:
						case TermType.Text:
						case TermType.Date:
						case TermType.MSO:
						case TermType.PickList:
						case TermType.Facility:
						case TermType.External:  // ???
							break;

						case TermType.Link:
							//TODO - The assumption here is that the Link term will be up to date, since the
							//change requiring its inclusion will not go out until the TermID change goes out.
							//Otherwise, the Link term would first require the ComplexLists to be loaded.
							break;

						case TermType.Renewal:
							if (!Term.ValidID((term as RenewalTerm).RenewalEvent.OffsetTermID))
								if (!string.IsNullOrEmpty((term as RenewalTerm).RenewalEvent.OffsetTermName))
								{
									try
									{
										(term as RenewalTerm).RenewalEvent.OffsetTermID = template.FindTerm((term as RenewalTerm).RenewalEvent.OffsetTermName).ID;
									}
									catch (Exception)
									{
										sErrors.Add(string.Format("Unable to find Renewal term '{0}'", (term as RenewalTerm).RenewalEvent.OffsetTermName));
									}
								}

							break;
						default:
							break;
					}
				}
			return sErrors;
		}

		public static List<string> TermReferences(Template template, string termName, Guid termID)
		{
			List<string> sErrors = new List<string>();
			if (template.BasicTerms != null)
				foreach (Term term in template.BasicTerms)
				{
					switch (term.TermType)
					{
						//Does not apply to these term types:
						case TermType.Text:
						case TermType.Date:
						case TermType.MSO:
						case TermType.PickList:
						case TermType.Facility:
						case TermType.External:  // ???
							break;

						case TermType.Link:
							if ((term as LinkTerm).ComplexListID.Equals(termID))
								sErrors.Add(string.Format("Term \"{0}\" is in use by Term \"{1}\".", termName, term.Name));
							break;

						case TermType.Renewal:
							if (Term.MatchReference(template, (term as RenewalTerm).RenewalEvent.OffsetTermID, (term as RenewalTerm).RenewalEvent.OffsetTermName, termID, termName))
								sErrors.Add(string.Format("Term \"{0}\" is in use by Term \"{1}\".", termName, term.Name));
							break;
						default:
							break;
					}
				}
			return sErrors;
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
			XmlElement rootTerm = xmlDoc.CreateElement(XMLNames._E_Terms);

			SaveTerms(template.BasicTerms, xmlDoc, rootTerm, bValidate);

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedRootTerm = xmlTemplateDoc.ImportNode(rootTerm, true);
			//Find the "Terms" child node
			XmlNode termsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_Terms));
			if (termsChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedRootTerm, termsChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedRootTerm);

			return true;
		}

		//Generate the Terms collection (of Term) based on an xml document
		public static bool SaveTerms(IEnumerable<Term> terms, XmlDocument xmlDoc, XmlElement rootTerm, bool bValidate)
		{
			if (terms != null)
				foreach (Term term in terms)
				{
					XmlElement xmlTerm;
					switch (term.TermType)
					{
						case TermType.Text:
							xmlTerm = xmlDoc.CreateElement(XMLNames._E_Text);
							(term as TextTerm).Build(xmlDoc, xmlTerm, bValidate);
							rootTerm.AppendChild(xmlTerm);
							break;
						case TermType.Date:
							xmlTerm = xmlDoc.CreateElement(XMLNames._E_Date);
							(term as DateTerm).Build(xmlDoc, xmlTerm, bValidate);
							rootTerm.AppendChild(xmlTerm);
							break;
						case TermType.MSO:
							xmlTerm = xmlDoc.CreateElement(XMLNames._E_MSO);
							(term as MSOTerm).Build(xmlDoc, xmlTerm, bValidate);
							rootTerm.AppendChild(xmlTerm);
							break;
						case TermType.Link:
							xmlTerm = xmlDoc.CreateElement(XMLNames._E_Link);
							(term as LinkTerm).Build(xmlDoc, xmlTerm, bValidate);
							rootTerm.AppendChild(xmlTerm);
							break;
						case TermType.Facility:
							xmlTerm = xmlDoc.CreateElement(XMLNames._E_Facility);
							(term as FacilityTerm).Build(xmlDoc, xmlTerm, bValidate);
							rootTerm.AppendChild(xmlTerm);
							break;
						case TermType.Renewal:
							xmlTerm = xmlDoc.CreateElement(XMLNames._E_Renewal);
							(term as RenewalTerm).Build(xmlDoc, xmlTerm, bValidate);
							rootTerm.AppendChild(xmlTerm);
							break;
						case TermType.PickList:
							xmlTerm = xmlDoc.CreateElement(XMLNames._E_PickList);
							(term as PickListTerm).Build(xmlDoc, xmlTerm, bValidate);
							rootTerm.AppendChild(xmlTerm);
							break;
						case TermType.External:
							xmlTerm = xmlDoc.CreateElement(XMLNames._E_External);
							(term as ExternalTerm).Build(xmlDoc, xmlTerm, bValidate);
							rootTerm.AppendChild(xmlTerm);
							break;
                        case TermType.PlaceHolderAttachments:
                            xmlTerm = xmlDoc.CreateElement(XMLNames._E_PlaceHolderAttachments);
                            (term as PlaceHolderAttachments).Build(xmlDoc, xmlTerm, bValidate);
                            rootTerm.AppendChild(xmlTerm);
                            break;
                        case TermType.PlaceHolderComments:
                            xmlTerm = xmlDoc.CreateElement(XMLNames._E_PlaceHolderComments);
                            (term as PlaceHolderComments).Build(xmlDoc, xmlTerm, bValidate);
                            rootTerm.AppendChild(xmlTerm);
                            break;
                        default:
							throw new XmlException(string.Format("Tried to save an undefined term type '{0}' for term '{1}'", term.TermType.ToString(), term.Name.ToString()));
					}
				}
			return true;
		}

		//Generate the Terms collection (of Term) based on an xml document
		public static bool Save(Template template, ref string sXml, bool bValidate)
		{
            //Convert the xml into an xmldocument
            XmlDocument xmlTemplateDoc = new XmlDocument();
			xmlTemplateDoc.PreserveWhitespace = false;
			xmlTemplateDoc.LoadXml(sXml);

            //Convert the objects stored in memory to an xmldocument
            XmlDocument xmlDoc = new XmlDocument();
			XmlElement rootTerm = xmlDoc.CreateElement(XMLNames._E_Terms);

			SaveTerms(template.BasicTerms, xmlDoc, rootTerm, bValidate);

            //Replace the impacted portion of the complete xml with this version from memory
            XmlNode importedRootTerm = xmlTemplateDoc.ImportNode(rootTerm, true);
			//Find the "Terms" child node
			XmlNode termsChildNode = xmlTemplateDoc.SelectSingleNode(Utility.XMLHelper.GetXPath(true, XMLNames._E_TemplateDef,XMLNames._E_Terms));
			if (termsChildNode != null)
				xmlTemplateDoc.DocumentElement.ReplaceChild(importedRootTerm, termsChildNode);
			else
				xmlTemplateDoc.DocumentElement.AppendChild(importedRootTerm);

            //Store the entire xml back to the database
            sXml = xmlTemplateDoc.OuterXml;

			return true;
		}

	}
}
