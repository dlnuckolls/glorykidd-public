using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;

namespace Kindred.Knect.ITAT.Utility
{

	public static class XMLHelper
	{


		public static XmlWriterSettings GetWriterSettings(Encoding encoding)
		{
			XmlWriterSettings rtn = new XmlWriterSettings();
			rtn.Encoding = encoding;
			return rtn;
		}


		/// <summary>
		/// Determines if the current xml node is of type "text"
		/// </summary>
		/// <param name="node"></param>
		/// <returns>true if a text node</returns>
		public static bool IsTextNode(XmlNode node)
		{
			return node.NodeType == XmlNodeType.Text;
		}


		/// <summary>
		/// Determines if the the node list is null or empty (i.e., has no nodes)
		/// </summary>
		/// <param name="nodeList"></param>
		/// <returns>true if nodeList is null, true if nodeList has no nodes, false otherwise</returns>
		public static bool ListNullOrEmpty(XmlNodeList nodeList)
		{
			if (nodeList == null)
				return true;
			if (nodeList.Count == 0)
				return true;
			return false;
		}

		/// <summary>
		/// Creates a "text" node and adds the node to the current xml node
		/// </summary>
		public static bool AddText(XmlDocument xmlDoc, XmlNode node, string sValue)
		{
			string nodeName = "Text";
			return AddText(xmlDoc, node, sValue, nodeName);
		}


		/// <summary>
		/// Creates a text node with a specific name
		/// </summary>
		/// <param name="xmlDoc">The XML doc.</param>
		/// <param name="node">The node.</param>
		/// <param name="sValue">The s value.</param>
		/// <param name="nodeName">Name of the node.</param>
		/// <returns></returns>
		/// Created by Larry Richardson LRR 4/2/2008
		public static bool AddText(XmlDocument xmlDoc, XmlNode node, string sValue, string nodeName)
		{
			if (sValue != null)
			{
				XmlNode textNode = xmlDoc.CreateNode(XmlNodeType.Text, nodeName, "");
				textNode.Value = sValue;
				node.AppendChild(textNode);
				return true;
			}
			return false;
		}

		/// <summary>
		/// If the current node already contains a "text" node, modify the text node value.
		/// Otherwise, create a new "text" node and adds the node to the current xml node
		/// </summary>

		public static bool SetText(XmlDocument xmlDoc, XmlNode node, string sValue)
		{
			if (sValue != null)
			{
				foreach (XmlNode childNode in node.ChildNodes)
					if (childNode.NodeType == XmlNodeType.Text)
					{
						childNode.Value = sValue;
						return true;
					}

				XmlNode textNode = xmlDoc.CreateNode(XmlNodeType.Text, "Text", "");
				textNode.Value = sValue;
				node.AppendChild(textNode);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Adds an attribute of type "string" to the current node
		/// </summary>
		public static void AddAttributeString(XmlDocument xmlDoc, XmlNode node, string sName, string sValue)
		{
			if (sValue != null)
			{
				XmlAttribute attrib = xmlDoc.CreateAttribute(sName);
				attrib.Value = sValue;
				node.Attributes.Append(attrib);
			}
		}

		/// <summary>
		/// Adds an attribute of type "bool" to the current node
		/// </summary>
		public static void AddAttributeBool(XmlDocument xmlDoc, XmlNode node, string sName, bool? bValue)
		{
			if (bValue.HasValue)
			{
				XmlAttribute attrib = xmlDoc.CreateAttribute(sName);
				attrib.Value = bValue.ToString();
				node.Attributes.Append(attrib);
			}
		}

		/// <summary>
		/// Adds an attribute of type "long" to the current node
		/// </summary>
		public static void AddAttributeLong(XmlDocument xmlDoc, XmlNode node, string sName, long? nValue)
		{
			if (nValue.HasValue)
			{
				XmlAttribute attrib = xmlDoc.CreateAttribute(sName);
				attrib.Value = nValue.ToString();
				node.Attributes.Append(attrib);
			}
		}

		/// <summary>
		/// Adds an attribute of type "int" to the current node
		/// </summary>
		public static void AddAttributeInt(XmlDocument xmlDoc, XmlNode node, string sName, int? nValue)
		{
			if (nValue.HasValue)
			{
				XmlAttribute attrib = xmlDoc.CreateAttribute(sName);
				attrib.Value = nValue.ToString();
				node.Attributes.Append(attrib);
			}
		}

		/// <summary>
		/// Gets the value of the text node, if one exists.  Otherwise, returns null
		/// </summary>
		public static string GetText(XmlNode node)
		{
			if (node != null)
				foreach (XmlNode childNode in node.ChildNodes)
					if (childNode.NodeType == XmlNodeType.Text)
						return (string)childNode.Value;
			return null;
		}

		public static DateTime? GetValueDate(XmlNode node)
		{
			string sValue = GetText(node);
			if (sValue != null)
			{
				return Utility.DateHelper.GetXMLDate(sValue);
			}
			return null;
		}

		public static int? GetValueInt(XmlNode node)
		{
			string sValue = GetText(node);
			if (sValue != null)
			{
				try
				{
					return int.Parse(sValue);
				}
				catch { }
			}
			return null;
		}

		/// <summary>
		/// Gets the value of the indicated attribute of type "string"
		/// </summary>
		public static string GetAttributeString(XmlNode node, string sName)
		{
			if (node.Attributes[sName] != null)
				return node.Attributes[sName].Value;
			return null;
		}

		/// <summary>
		/// Gets the value of the indicated attribute of type "long"
		/// </summary>
		public static long? GetAttributeLong(XmlNode node, string sName)
		{
			if (node.Attributes[sName] != null)
				return long.Parse(node.Attributes[sName].Value);
			return null;
		}

		/// <summary>
		/// Gets the value of the indicated attribute of type "bool"
		/// </summary>
		public static bool? GetAttributeBool(XmlNode node, string sName)
		{
			if (node.Attributes[sName] != null)
				return bool.Parse(node.Attributes[sName].Value);
			return null;
		}

		/// <summary>
		/// Gets the value of the indicated attribute of type "int"
		/// </summary>
		public static int? GetAttributeInt(XmlNode node, string sName)
		{
			if (node.Attributes[sName] != null)
				return int.Parse(node.Attributes[sName].Value);
			return null;
		}

		public static int GetAttributeIntNonNull(XmlNode node, string sName)
		{
			if (node.Attributes[sName] != null)
				return int.Parse(node.Attributes[sName].Value);
			return -1;
		}

		/// <summary>
		/// Sets the value of the indicated attribute of type "string", if it exists
		/// </summary>
		public static bool SetAttributeString(XmlNode node, string sName, string sValue)
		{
			if (node.Attributes[sName] == null)
				return false;
			node.Attributes[sName].Value = sValue;
			return true;
		}

		/// <summary>
		/// Sets the value of the indicated attribute of type "bool", if it exists
		/// </summary>
		public static bool SetAttributeBool(XmlNode node, string sName, bool? bValue)
		{
			if (!bValue.HasValue)
				return false;
			if (node.Attributes[sName] == null)
				return false;
			node.Attributes[sName].Value = bValue.ToString();
			return true;
		}

		public static bool SetValueDate(XmlDocument xmlDoc, XmlNode node, DateTime? date)
		{
			if (!date.HasValue)
				return false;
			string sValue = Utility.DateHelper.SetXMLDate(date.Value);
			if (sValue != null)
			{
				return SetText(xmlDoc, node, sValue);
			}
			return false;
		}

		public static bool SetValueInt(XmlDocument xmlDoc, XmlNode node, int? value)
		{
			if (!value.HasValue)
				return false;
			return SetText(xmlDoc, node, value.ToString());
		}

		public static void ValidateString(string sName, string sValue)
		{
			if (string.IsNullOrEmpty(sValue))
				throw new XmlException(string.Format("Required Text '{0}' not supplied", sName));
		}

		public static void ValidateBool(string sName, bool? bValue)
		{
			if (!bValue.HasValue)
				throw new XmlException(string.Format("Required Bool '{0}' not supplied", sName));
		}

		public static void ValidateLong(string sName, long? nValue)
		{
			if (!nValue.HasValue)
				throw new XmlException(string.Format("Required Long '{0}' not supplied", sName));
		}

		public static void ValidateInt(string sName, int? nValue)
		{
			if (!nValue.HasValue)
				throw new XmlException(string.Format("Required Int '{0}' not supplied", sName));
		}

		public static string GetXMLText(string sText)
		{
			//Converts a string that has been HTML-encoded for HTTP transmission into a decoded string. 
			//The result is a 'live' version of the text, not suitable for embedding into xml.
			return HttpUtility.HtmlDecode(sText);
		}

		public static string SetXMLText(string sText)
		{
			return HttpUtility.HtmlEncode(sText);
		}

		public static string GetXPath(bool bAbsolute, params string[] sElements)
		{
			string sPath = string.Join("/", sElements);
			if (bAbsolute)
				sPath = string.Concat("/", sPath);
			return sPath;
		}

		/// <summary>
		/// Strips out all of the xml tags, leaving only the text nodes, which are then concatenated.
		/// </summary>
		public static string GetInnerText(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(string.Concat("<root>", xml, "</root>"));
			return doc.DocumentElement.InnerText;
		}


		public static string FormatXML(string xml)
		{
			XmlDocument xDoc = new XmlDocument();
			xDoc.LoadXml(xml);
			return xDoc.Format();
		}

		public static string Format(this XmlDocument doc)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.NewLineOnAttributes = false;
			settings.NewLineHandling = NewLineHandling.Replace;
			settings.Indent = true; 
			settings.IndentChars = "  "; 
			settings.NewLineChars = "\r\n"; 

			using (StringWriterWithEncoding sw = new StringWriterWithEncoding(Encoding.UTF8))
 			{
				using (XmlWriter writer = XmlWriter.Create(sw, settings))
				{
					sw.Write(doc.OuterXml);
					sw.Flush();
					return sw.ToString();
				}
			}
		}

        public static string SafeReadElementString(XmlReader reader)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    return reader.ReadElementString();

                case XmlNodeType.Text:
                    return reader.Value;

                default:
                    return null;
            }
        }

	}
}
