<%@ Page Language="C#" validaterequest="false" AutoEventWireup="true" CodeBehind="ViewTemplate.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ViewTemplate" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>View Template</title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="../itat.css" />
</head>

<body id="htmlBody" runat="server">
    <form id="form1" runat="server">
			<kh:standardheader id="itatHeader" pagetitle="View Template" runat="server"  />
			
			<div class="TemplateEditBody" id="divContainer">

				<table width="100%" style="table-layout:fixed;">

					<tr>
						<td class="DataEntryCaption" width="120px">System:</td>
						<td class="DataEntryCaption" width="100%"><asp:dropdownlist id="ddlSystem" runat="server"  width="100%"  autopostback="true" onselectedindexchanged="ddlSystemOnSelectedIndexChanged" /></td>
					</tr>

					<tr>
						<td class="DataEntryCaption">Type:</td>
						<td class="DataEntryCaption"><asp:dropdownlist id="ddlType" runat="server"  width="100%" autopostback="true" onselectedindexchanged="ddlTypeOnSelectedIndexChanged"  /></td>
					</tr>

					<tr id="trIncludeInactive" runat="server" visible="false">
						<td class="DataEntryCaption">Include Inactive:</td>
						<td class="DataEntryCaption AlignLeft">
							<asp:checkbox id="chkIncludeInactive" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlignLeft" text="" autopostback="true" oncheckedchanged="chkIncludeInactiveOnCheckedChanged"  />
						</td>
					</tr>
					
					<tr>
						<td class="DataEntryCaption"><asp:label id="lblItem" runat="server" /></td>
						<td class="DataEntryCaption">
							<asp:dropdownlist id="ddlTemplate" runat="server" autopostback="true" OnSelectedIndexChanged="ddlTemplateOnSelectedIndexChanged" width="100%" />
							<asp:textbox id="txtManagedItemNumber" runat="server" width="100%" />
						</td>
					</tr>
					
					<tr>
						<td  class="DataEntryCaption AlignCenter" colspan="2" align="center" style="padding:10px 0 10px 0;">
							<asp:button id="btnOpen" runat="server" text="Open" cssclass="KnectButton" oncommand="btnOpenOnCommand" />
							<asp:button id="btnReset" runat="server" text="Reset" cssclass="KnectButton" oncommand="btnResetOnCommand" />
                            <div id="divUpload" runat="server">
                                <br />
                                <asp:fileupload id="filUpload" runat="server" width="100%" />
                                <br />
							    <asp:button id="btnUpload" runat="server" text="Upload" cssclass="KnectButton" oncommand="btnUploadOnCommand" />
                            </div>
						</td>
					</tr>
						
				</table>
				
			</div>


    </form>
</body>
</html>
