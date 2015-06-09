
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateNotifications.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateNotifications" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>
<body id="htmlBody" class="NoScroll" runat="server">
	<form id="form1" runat="server">
		<kh:templateheader id="header" runat="server" onheaderevent="OnHeaderEvent" />
		<div id="divTopHalf" style="margin: 0 0 2px 0; border: 1px solid #666;">
			<table border="0" cellpadding="0" cellspacing="0" width="100%" style="table-layout:fixed;">		
				<colgroup>
					<col width="100%" />
					<col width="330px" />
				</colgroup>	
				<tr>
					<td  style="border-right: 1px solid #666;" height="100%">
						<table border="0" cellpadding="0" cellspacing="0" width="100%" height="100%">
							<tr>
								<td class="1stTblColor GridTitle AlignTop">Notifications</td>
								<td runat="server" class="1stTblColor AlignRight" style="white-space:nowrap; padding: 4px 4px 0 4px;">
									<asp:button id="btnAddNotification" runat="server" text="Add" cssclass="KnectButton" width="60px" oncommand="btnAddNotification_OnCommand" />
									<asp:button id="btnDeleteNotification" runat="server" text="Delete" cssclass="KnectButton" width="60px"  oncommand="btnDeleteNotification_OnCommand" Enabled="False"  />
								</td>
							</tr>
							<tr>
								<td id="row_lstNotifications" colspan="2" class="DataEntryCaption" style="height:202px; width:100%;" runat="server">
										<asp:listbox id="lstNotifications" runat="server" height="100%" width="100%" autopostback="true" onselectedindexchanged="lstNotifications_SelectedIndexChanged" />
								</td>
							</tr>
						</table>
					</td>
					<td runat="server" id="tdProperties" style="vertical-align: middle;" class="1stTblColor">
						<table id="tblProperties" runat="server" border="0" cellpadding="1" cellspacing="0" width="100%"  style="table-layout:fixed;">
							<tr>
								<td class="DataEntryCaption" width="115px">Name</td>
								<td class="DataEntryCaption AlignLeft" width="100%"><asp:textbox id="txtName" runat="server" width="100%" /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption" width="115px">Recipients</td>
								<td class="DataEntryCaption AlignLeft" width="100%"><asp:listbox id="lstRecipients" runat="server" rows="5" selectionmode="multiple" width="100%" /></td>
							</tr>
							<tr id="row_ddlFilterFacility" runat="server" visible="false">
								<td class="DataEntryCaption" width="115px">Filter By Facility</td>
								<td class="DataEntryCaption AlignLeft" width="100%"><asp:dropdownlist id="ddlFilterFacility" runat="server" width="100%"  /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption" width="115px">Subject</td>
								<td class="DataEntryCaption AlignLeft" width="100%"><asp:textbox id="txtSubject" runat="server" width="100%"  /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption" width="115px">Send If Status Is</td>
								<td class="DataEntryCaption AlignLeft" width="100%"><asp:dropdownlist id="ddlSendNotificationStatus" runat="server" width="100%"  /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption" width="115px">Schedule Based On</td>
								<td class="DataEntryCaption AlignLeft" width="100%"><asp:dropdownlist id="ddlBaseDate" runat="server" width="100%"  /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption AlignTop">Offset Term:</td>
								<td class="DataEntryCaption AlignLeft">
									<asp:dropdownlist id="ddlOffsetTerm" runat="server" width="100%" /><br />
									Default&nbsp;Value:&nbsp;<asp:textbox id="txtOffsetDefault" runat="server" width="50px" maxlength="4" />
								</td>
							</tr>
							<tr id="trDateOffset" runat="server">
								<td class="DataEntryCaption" width="115px">Offset Days</td>
								<td class="DataEntryCaption AlignLeft" width="100%"><asp:textbox id="txtDateOffset" runat="server" width="100%" maxlength="30" /></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
		<asp:panel id="divEditor" runat="server" style="display: block; margin: 2px 0 0 0; width: 100%; border: 1px solid #666; font-family:Times New Roman; font-size:12pt">
			<rade:radeditor id="edt" 
					runat="server" 
					onclientload="radOnClientLoad"
					toolsfile="~/Config/TemplateClauseToolsFile.xml" 
					font-names="Times New Roman, Arial, Courier New"
					enableviewstate="true"
					width="100%" 
				   focusonload="false"
					height="100%" 
					enabled="true" 
					editable="true"
				   visible="true"
					saveasxhtml="true" 
					saveinfile="false" 
					newlinebr="true"
					 enabletab="false" 
					 font-size="" 
					converttagstolower="true"
					converttoxhtml="true"
					convertfonttospan="false"  	 				  
					 enableserversiderendering="true" 
					 stripformattingonpaste="MSWordRemoveAll"   
					 toolbarmode="Default"
					 showpreviewmode="false"
					 showsubmitcancelbuttons="false"
					 showhtmlmode="false" 
					 renderastextarea="false"		   
			/>
		</asp:panel>
	</form>
</body>
</html>
