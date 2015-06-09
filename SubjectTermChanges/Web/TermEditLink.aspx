<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermEditLink.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermEditLink" %>

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
					<td class="DataEntryCaption" style="width: 200px">Term Name:</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtTermName" runat="server" width="300px"  /></td>
				</tr>

				<tr>
					<td  class="DataEntryCaption" style="width: 200px">Term Type:</td>
					<td  class="DataEntryCaption AlignLeft">Link</td>
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
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkHeaderTerm" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlignLeft"   text=""  /></td>
				</tr>

        <tr>
            <td class="DataEntryCaption" style="width: 200px;">Keyword Searchable:</td> 
            <td class="DataEntryCaption AlignLeft" style="height: 24px"><asp:checkbox id="chkboxKeywordSearchable" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft"  /></td>
        </tr>

				<tr>
					<td class="DataEntryCaption" style="width: 200px">Caption:</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtCaption" runat="server" width="90%" /></td>
				</tr>
				<tr>
					<td class="DataEntryCaption"style="width: 200px">Link Source:</td>
					<td class="DataEntryCaption AlignLeft"><asp:radiobuttonlist id="rblLinkSource" runat="server" repeatdirection="Horizontal"  repeatlayout="Flow" repeatcolumns="3" cssclass="CheckBoxAlignLeft" OnSelectedIndexChanged="rblLinkSource_SelectedIndexChanged"  AutoPostBack="true" /></td>
				</tr>
	

				<tr>
					<td class="DataEntryCaption" style="width: 200px" id="tdlabel"><br />&nbsp;</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtURL" runat="server" width="90%" Visible="false" Text = ""/><asp:listbox id="lstComplexList" runat="server" selectionmode="Single" rows="1" Visible="false" /></td>
				</tr>
				<tr>
					<td style="width: 150px">&nbsp;</td>
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
