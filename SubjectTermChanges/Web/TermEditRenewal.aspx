<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermEditRenewal.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermEditRenewal" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>
<body id="body" runat="server" class="NoScroll">
	<form id="form1" runat="server" defaultfocus="chkValidateOnSave" defaultbutton="btnOK">
		<kh:standardheader id="header" runat="server"  pagetitle="Edit Term" />

		<asp:panel cssclass="TemplateEditBody" id="pnlTop" runat="server" style="padding: 2px 0 2px 9px;">
				<table border="0" cellpadding="2" cellspacing="0" style="table-layout:fixed;" >
					<colgroup>
						<col width="175px" />
						<col width="100%" />
					</colgroup>	
					<tr>
						<td class="DataEntryCaption">Term Name:</td>
						<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtTermName" runat="server" width="300px"  /></td>
					</tr>

					<tr>
						<td  class="DataEntryCaption">Term Type:</td>
						<td  class="DataEntryCaption AlignLeft">Renewal</td>
					</tr>
				</table>
		</asp:panel>
		
		<asp:panel cssclass="TemplateEditBody GridBodyPanel" id="editBody" runat="server">
			<div style="margin:0 0 0 9px;">
				<table border="0" cellpadding="2" cellspacing="0" style="table-layout:fixed;" >
					<colgroup>
						<col width="175" />
						<col width="100%" />
					</colgroup>
					
					<kh:VOS runat="server" ID="vos"  /> 
					
					<tr id="trTermGroup" runat="server" visible="false">
						<td class="DataEntryCaption">Tab:</td>
						<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlTermGroup" runat="server" /></td>
					</tr>
					
					<tr id="trHeaderTerm" runat="server" visible="false">
						<td class="DataEntryCaption">Header Term:</td>
						<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkHeaderTerm" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlignLeft"   text=""  /></td>
					</tr>

					<tr>
						<td class="DataEntryCaption">Keyword Searchable:</td>
						<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkbxKeywordSearchable" runat="server" cssclass="CheckBoxAlignLeft" /></td>
					</tr>
					
					<tr>
						<td class="DataEntryCaption">Allow Backdating:</td>
						<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkbxAllowBackdating" runat="server" cssclass="CheckBoxAlignLeft" /></td>
					</tr>
					
					<tr id="RowEditable" runat="server">
						<td class="DataEntryCaption">Editable:</td>
						<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkbxEditable" runat="server" cssclass="CheckBoxAlignLeft" /></td>
					</tr>

                    <tr id="trTermMapping" runat="server" visible="true">
					    <td class="DataEntryCaption">System Term Mapping:</td>
					    <td class="DataEntryCaption AlignLeft">
                        <asp:dropdownlist id="ddlMappedTerm" runat="server" style="margin-left:4px;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:button id="btnMap" runat="server" cssclass="KnectButton" text="Map" onclick="btnMap_Click" />
                        </td>
				    </tr>

					<tr>
						<td class="DataEntryCaption">Effective Date Format:</td>
						<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlEffectiveDateFormat" runat="server" /></td>
					</tr>
					
					<tr>
						<td class="DataEntryCaption">Expiration Date Format:</td>
						<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlExpirationDateFormat" runat="server" /></td>
					</tr>
					
				</table>				
		</div>
		
				<fieldset id="fldsetInitial">
					<legend style="font-weight:bold;">Initial Duration Information</legend>
					<table style="table-layout: fixed;">
					<colgroup>
						<col width="175" />
						<col width="100%" />
					</colgroup>	
						<tr>
							<td class="DataEntryCaption">Duration Unit:</td>
							<td class="DataEntryCaption AlignLeft"><asp:listbox id="listbxInitialDurationUnit" runat="server" selectionmode="Multiple" width="160px"></asp:listbox></td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Default Duration Unit:</td>
							<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlDefaultInitialDurationUnit" runat="server" width="160px"></asp:dropdownlist></td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Default Duration:</td>
							<td class="DataEntryCaption AlignLeft"><asp:textbox id="textbxDefaultInitialDuration" runat="server" width="50px"></asp:textbox></td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Popup If Not Default Duration:</td>
							<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkbxInitialPopUpIfNot" runat="server" cssclass="CheckBoxAlignLeft" /></td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Popup Text:</td>
							<td class="DataEntryCaption AlignLeft"><asp:TextBox id="textbxInitialDurationPopup" runat="server" textmode="MultiLine" rows="4" width="100%" /></td>
						</tr>
					</table>
				</fieldset>
				
				<div style="margin:4px 0 0 9px;">
					<table border="0" cellpadding="2" cellspacing="0" style="table-layout:fixed;" >
						<colgroup>
							<col width="175" />
							<col width="100%" />
						</colgroup>	
						<tr>
						  <td class="DataEntryCaption" style="font-weight:bold;">Renewal Type:</td>
						  <td class="DataEntryCaption AlignLeft"><asp:DropDownList id="ddlRenewalType" runat="server" width="160px" /></td>
						</tr>
					</table>
				</div>
				
				<fieldset id="fldsetRenewal">
					<legend style="font-weight:bold;">Renewal Duration Information</legend>
					<table id="tblRenewal" style="table-layout: fixed;">
						<colgroup>
							<col width="175" />
							<col width="100%" />
						</colgroup>	
					
						<tr>
							<td class="DataEntryCaption">Editable:</td>
							<td class="DataEntryCaption AlignLeft" ><asp:checkbox id="chkbxEditableRenewalDurationUnit" runat="server" cssclass="CheckBoxAlignLeft" /></td>
						</tr>
					
						<tr>
							<td class="DataEntryCaption">Renewers:</td>
							<td class="DataEntryCaption AlignLeft"><asp:ListBox id="listbxRenewers" runat="server" SelectionMode="Multiple" width="300px" /></td>
						</tr>

						<tr id="trPopupTextWhenRenewSelected">
							<td class="DataEntryCaption">Popup Text When "Renew" Selected:</td>
							<td class="DataEntryCaption AlignLeft"><asp:TextBox id="textbxRenewalPopup" runat="server" textmode="MultiLine" rows="4" width="100%" /></td>
						</tr>

						<tr>
							<td class="DataEntryCaption">Duration Unit:</td>
							<td class="DataEntryCaption AlignLeft" ><asp:listbox id="listbxRenewalDurationUnit" runat="server" selectionmode="Multiple" width="160px" /></td>
						</tr>
					
						<tr>
							<td class="DataEntryCaption" >Default Duration Unit:</td>
							<td class="DataEntryCaption AlignLeft" ><asp:dropdownlist id="ddlDefaultRenewalDurationUnit" runat="server" width="160px" />
						</tr>
						
						<tr>
							<td class="DataEntryCaption" >Default Duration:</td>
							<td class="DataEntryCaption AlignLeft" ><asp:textbox id="textbxDefaultRenewalDuration" runat="server" width="50px" /></td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Popup If Not Default Duration</td>
							<td class="DataEntryCaption AlignLeft" ><asp:checkbox id="chkbxRenewalPopUpIfNot" runat="server" cssclass="CheckBoxAlignLeft" /></td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Popup Text:</td>
							<td class="DataEntryCaption AlignLeft"><asp:TextBox id="textbxRenewalDurationPopup" runat="server" textmode="MultiLine" rows="4" width="100%" /></td>
						</tr>
					</table>
				</fieldset>

				<fieldset  id="fldsetNotification">
					<legend style="font-weight:bold;">Notification Information</legend>
					<table style="table-layout: fixed;">
						<colgroup>
							<col width="175" />
							<col width="100%" />
						</colgroup>	
						<tr>
							<td class="DataEntryCaption">Send Notification:</td>
							<td class="DataEntryCaption AlignLeft">
								<asp:checkbox id="chkbxSendNotification" runat="server" cssclass="CheckBoxAlignLeft" />
								&nbsp;If&nbsp;Status&nbsp;Is&nbsp;<asp:dropdownlist id="ddlSendNotificationStatus" runat="server" />
							</td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Offset Term:</td>
							<td class="DataEntryCaption AlignLeft">
								<asp:dropdownlist id="ddlOffsetTerm" runat="server" width="300px" />&nbsp; &nbsp;
								Default&nbsp;Value:&nbsp;<asp:textbox id="txtOffsetDefault" runat="server" width="50px" maxlength="4" />
							</td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Offset Days:</td>
							<td class="DataEntryCaption AlignLeft"><asp:TextBox id="textbxOffsetDays" runat="server" width="300px"/></td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Subject:</td>
							<td class="DataEntryCaption AlignLeft"><asp:TextBox id="textbxSubject" runat="server" width="300px"></asp:TextBox></td>
						</tr>
						<tr>
							<td class="DataEntryCaption">Recipients:</td>
							<td id="RecipientsRow" class="DataEntryCaption AlignLeft"><asp:ListBox id="listbxRecipients" runat="server" SelectionMode="Multiple" width="300px" /></td>
						</tr>
						<tr id="row_ddlFilterFacility" runat="server" visible="false">
							<td class="DataEntryCaption">Filter By Facility</td>	
							<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlFilterFacility" width="300px" runat="server" /></td>
						</tr>
					</table>	
					
					<asp:panel id="divEditor" runat="server" style="display: block; height: 240px; margin: 3px; border: 1px solid #666;">
						<rade:radeditor id="edt" 
									runat="server" 
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
									enabledocking="false"
									enableclientserialize="false"  
									font-size="" 
									converttagstolower="true"
									converttoxhtml="true"
									convertfonttospan="false"  	 				  
									enableserversiderendering="true" 
									stripformattingonpaste="MSWordRemoveAll"   
									toolbarmode="Default"
									showsubmitcancelbuttons="false"
									showpreviewmode="false"
									showhtmlmode="false" 
									renderastextarea="false"		   
						/>
					</asp:panel>

				</fieldset>
				
		</asp:panel>

		<div class="TemplateEditBody AlignCenter" style="padding: 4px 0 4px 0;">
			<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" onclick="btnOK_Click" />
			<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" onclick="btnCancel_Click" />
		</div>

	</form>
</body>
</html>
