<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermEditMSO.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermEditMSO" %>
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
					<td class="DataEntryCaption AlignLeft" ><asp:textbox id="txtTermName" runat="server" width="300px"  /></td>
				</tr>

				<tr>
					<td  class="DataEntryCaption">Term Type:</td>
					<td  class="DataEntryCaption AlignLeft">MSO</td>
				</tr>

				<tr>
					<td colspan="2"><hr style="height:2px; color:#333" /></td>
				</tr>

				<kh:vos id="vos" runat="server" />

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
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkRequired" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft"  /></td>
			  </tr>
	        
			  <tr>
					<td class="DataEntryCaption">Keyword Searchable:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkboxKeywordSearchable" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft"  /></td>
			  </tr>

				<tr>
					<td class="DataEntryCaption">Name:</td>
					<td class="DataEntryCaption AlignLeft">
						<asp:CheckBox ID="chkMSONameDisplayed" runat="server" OnCheckedChanged="chkMSONameDisplayed_CheckedChanged" cssclass="CheckBoxAlignLeft"  AutoPostBack="True" />
             <asp:textbox id="txtMSOName" runat="server" width="200px" />
					</td>
				</tr>

				<tr>
					<td class="DataEntryCaption" >Address1:</td>
					<td class="DataEntryCaption AlignLeft" >
						<asp:CheckBox ID="chkAddress1Displayed" runat="server" OnCheckedChanged="chkAddress1Displayed_CheckedChanged" cssclass="CheckBoxAlignLeft"  AutoPostBack="True" />
            <asp:textbox id="txtAddress1" runat="server" width="200px" />
					</td>
				</tr>
				
				<tr>
					<td class="DataEntryCaption">Address2:</td>
					<td class="DataEntryCaption AlignLeft">
						<asp:checkbox id="chkAddress2Displayed" runat="server" oncheckedchanged="chkAddress2Displayed_CheckedChanged" cssclass="CheckBoxAlignLeft"  autopostback="True" />
						<asp:textbox id="txtAddress2" runat="server" width="200px" />
					</td>
				</tr>
				
				<tr>
					<td class="DataEntryCaption">City:</td>
					<td class="DataEntryCaption AlignLeft">
						<asp:checkbox id="chkCityDisplayed" runat="server" oncheckedchanged="chkCityDisplayed_CheckedChanged" cssclass="CheckBoxAlignLeft"  autopostback="True" />
						<asp:textbox id="txtCity" runat="server" width="200px" />
					</td>
				</tr>
				
				<tr>
					<td class="DataEntryCaption">State:</td>
					<td class="DataEntryCaption AlignLeft">
						<asp:checkbox id="chkStateDisplayed" runat="server" oncheckedchanged="chkStateDisplayed_CheckedChanged" cssclass="CheckBoxAlignLeft"  autopostback="True" />
						<asp:textbox id="txtState" runat="server" width="200px" />
					</td>
				</tr>
				
				<tr>
					<td class="DataEntryCaption">Zip:</td>
					<td class="DataEntryCaption AlignLeft">
						<asp:checkbox id="chkZipDisplayed" runat="server" oncheckedchanged="chkZipDisplayed_CheckedChanged" cssclass="CheckBoxAlignLeft"  autopostback="True" />
						<asp:textbox id="txtZip" runat="server" width="200px" height="17px" />
					</td>
				</tr>
				
				<tr>
					<td class="DataEntryCaption">Phone:</td>
					<td class="DataEntryCaption AlignLeft">
						<asp:checkbox id="chkPhoneDisplayed" runat="server" oncheckedchanged="chkPhoneDisplayed_CheckedChanged" cssclass="CheckBoxAlignLeft"  autopostback="True" />
						<asp:textbox id="txtPhone" runat="server" width="200px" />
					</td>
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
