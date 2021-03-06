<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermEditFacility.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermEditFacility" %>
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
					<col width="150px" />
					<col width="100%" />
				</colgroup>	

				<tr>
					<td class="DataEntryCaption">Term Name:</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtTermName" runat="server" width="300px"  /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Term Type:</td>
					<td class="DataEntryCaption AlignLeft">Facility</td>
				</tr>

				<tr>
					<td colspan="2"><hr style="margin:0; padding:0; height:2px; color:#333" /></td>
				</tr>

				<kh:vos ID="vos" runat="server" />
				
				<tr id="trTermGroup" runat="server" visible="false">
					<td class="DataEntryCaption">Tab:</td>
					<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlTermGroup" runat="server" /></td>
				</tr>
				
				<tr id="trHeaderTerm" runat="server" visible="false">
					<td class="DataEntryCaption">Header Term:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkHeaderTerm" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlignLeft"   text=""  /></td>
				</tr>

                <tr id="trTermMapping" runat="server" visible="true">
					<td class="DataEntryCaption">System Term Mapping:</td>
					<td class="DataEntryCaption AlignLeft">
                    <asp:dropdownlist id="ddlMappedTerm" runat="server" style="margin-left:4px;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:button id="btnMap" runat="server" cssclass="KnectButton" text="Map" onclick="btnMap_Click" />
                    </td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Required:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkRequired" runat="server" textalign="Right"  cssclass="CheckBoxAlignLeft" text=""  /></td>
				</tr>

				<tr>
						<td class="DataEntryCaption" style="height: 30px">Keyword Searchable:</td>
						<td class="DataEntryCaption AlignLeft" style="height: 30px"><asp:checkbox id="chkboxKeywordSearchable" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft"  /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Enforce Security:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkEnforceSecurity" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft"  /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Facility Status:</td>
					<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlFacilityStatus" runat="server" /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Profile Screen View:</td>
					<td class="DataEntryCaption AlignLeft"><asp:radiobuttonlist id="rblProfileView" runat="server" repeatdirection="Horizontal"  repeatlayout="Flow" repeatcolumns="2" cssclass="CheckBoxAlignLeft"  /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Facility Type:</td>
					<td class="DataEntryCaption AlignLeft"><asp:listbox id="lstFacilityType" runat="server" selectionmode="Multiple" rows="8" /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption" id="litRow" runat="server" ><asp:literal id="litHierarchy" visible="true" runat="server" text='Include Hierarchy:' /></td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkIncludeHierarchy" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft"  /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Sort By:</td>
					<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlSortBy" runat="server" /></td>
				</tr>

				<tr>
					<td>&nbsp;</td>
					<td colspan="2" class="DataEntryCaption AlignLeft" style="padding: 15px 0 0 0;">
						<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" onclick="btnOK_Click" />
						<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" onclick="btnCancel_Click" />
					</td>
				</tr>

			</table>

		</asp:panel>

	</form>
</body>
</html>
