<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermEditExternal.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermEditExternal" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>
<body id="body" runat="server" class="NoScroll">
	<form id="form1" runat="server">
		<kh:standardheader id="header" runat="server"  pagetitle="Edit Term" />

		<asp:panel cssclass="TemplateEditBody AutoScroll" id="editBody" runat="server">

			<table border="0" cellpadding="2" cellspacing="0" width="100%" style="table-layout:fixed;">
				<colgroup>
					<col width="200px" />
					<col width="100%" />
				</colgroup>	
				<tr>
					<td class="DataEntryCaption">Term Name:</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtTermName" runat="server" width="300px"  /></td>
				</tr>

				<tr>
					<td  class="DataEntryCaption">Term Type:</td>
					<td  class="DataEntryCaption AlignLeft"><asp:label id="lblTermType" runat="server" /></td>
				</tr>

				<tr>
					<td colspan="2"><hr style="margin:0; padding:0; height:2px; color:#333" /></td>
				</tr>

				<tr id="trTermGroup" runat="server" visible="false">
					<td class="DataEntryCaption">Tab:</td>
					<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlTermGroup" runat="server" /></td>
				</tr>
				
				<tr id="trHeaderTerm" runat="server" visible="false">
					<td class="DataEntryCaption">Header Term:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkHeaderTerm" runat="server" textalign="Right" cssclass="DataEntryCaption"   text=""  /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Editable:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkEditable" runat="server" textalign="Right" cssclass="DataEntryCaption" text=""  /></td>
				</tr>

<!--
				<tr>
					<td class="DataEntryCaption">Validate on Save:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkValidateOnSave" runat="server" textalign="Right" cssclass="DataEntryCaption" text=""  /></td>
				</tr>
-->

				<tr id="trSyncWithSystem" runat="server">
					<td class="DataEntryCaption">&nbsp;</td>
					<td class="DataEntryCaption AlignLeft"><asp:button id="btnSyncWithSystem" runat="server" cssclass="KnectButton" style="width:130px;" text="Sync With System" onclick="btnSyncWithSystem_Click" /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Required:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkRequired" runat="server" textalign="Right" cssclass="DataEntryCaption" text=""  /></td>
				</tr>

			  <tr>
					<td class="DataEntryCaption" style="height: 32px">Keyword Searchable:</td>
					<td class="DataEntryCaption AlignLeft" style="height: 32px"><asp:checkbox id="chkbxKeywordSearchable" runat="server" textalign="Right" cssclass="DataEntryCaption" text=""  /></td>
			  </tr>
				
				<tr>
					<td>&nbsp;</td>
					<td class="DataEntryCaption AlignLeft" style="padding: 15px 0 0 0;">
						<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" onclick="btnOK_Click" />
						<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" onclick="btnCancel_Click" />
					</td>
				</tr>

			</table>

		</asp:panel>

	</form>
</body>
</html>
