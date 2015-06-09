<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermDependencyEdit.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermDependencyEdit" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>
<body id="body" runat="server" class="NoScroll">
	<IFRAME STYLE="display:none;position:absolute;width:148;height:194;z-index:3000;" ID="CalFrame" MARGINHEIGHT="0" MARGINWIDTH="0" NORESIZE FRAMEBORDER="0" SCROLLING="NO" SRC="/Global/bin/Calendar/calendar.htm" onblur="javascript:this.style.display='none';"></IFRAME>
	<script type="text/javascript" src="/Global/bin/Calendar/cal_routines.js"></script>
	<form id="form1" runat="server">
		<asp:hiddenfield id="hfAddCondition" runat="server" />
		<kh:standardheader  id="header" runat="server"  pagetitle="Edit&nbsp;Term&nbsp;Dependency" />
		

		<asp:panel cssclass="TemplateEditBody AutoScroll" id="editBody" runat="server" style="padding:5px;">

			<table border="0" cellpadding="2" cellspacing="0" width="100%" style="table-layout:fixed;">
				<colgroup>
					<col width="80px" />
					<col width="100%" />
				</colgroup>	

				<tr style="padding: 10px 5px 10px 5px;" id="rowDDLTermDependencyType" runat="server" >
					<td class="DataEntryCaption" style="font-weight:bold;">Term Type:</td>
					<td class="DataEntryCaption AlignLeft">
                    <asp:dropdownlist id="ddlTermDependencyType" runat="server" autopostback="true" OnSelectedIndexChanged="ddlTermDependencyTypeOnSelectedIndexChanged" />
                    </td>
				</tr>

				<tr style="padding: 10px 5px 10px 5px;" id="rowDDLDependentTerm" runat="server" >
					<td class="DataEntryCaption" style="font-weight:bold;">Term:</td>
					<td class="DataEntryCaption AlignLeft">
                    <asp:dropdownlist id="ddlDependentTerm" runat="server" AutoPostBack="true"  OnSelectedIndexChanged="ddlDependentTermOnSelectedIndexChanged" />
                    </td>
				</tr>

				<tr style="padding: 10px 5px 10px 5px;" id="rowListBoxDependentTerms" runat="server" >
					<td class="DataEntryCaption" style="font-weight:bold;">Terms:</td>
					<td class="DataEntryCaption AlignLeft">
                    <asp:ListBox id="lstbxDependentTerms" runat="server" SelectionMode="Multiple" />
                    </td>
				</tr>

				<tr style="padding: 10px 5px 10px 5px;">
					<td class="DataEntryCaption AlignTop" style="font-weight:bold;">Conditions:</td>
					<td class="DataEntryCaption AlignLeft" style="white-space:nowrap">
						<span style="float:left; clear:both;">
							<asp:dropdownlist id="ddlQuantifier" runat="server" />
						</span>
						<span style="float:right; clear:both">
							<button id="btnAddCondition" class="KNectButton" onclick="ShowPopup(true)">Add</button>
							<asp:button id="btnDeleteCondition" runat="server" text="Delete" cssclass="KNectButton" enabled="false" oncommand="btnDeleteConditionOnCommand" />
						</span>
						<br />
						<asp:listbox id="lstConditions" runat="server" width="100%" rows="6" onchange="SetDeleteButtonEnabled();"	 />
					</td>
				</tr>

				<tr style="padding: 10px 5px 10px 5px;">
					<td class="DataEntryCaption AlignTop" style="font-weight:bold;">Action:</td>
					<td class="DataEntryCaption AlignLeft" style="margin:0; padding: 6px;">
							<div style="white-space:nowrap" id="divActionEnabled" runat="server" ><span style="width:60px;">Enabled:</span><asp:dropdownlist id="ddlActionEnabled" runat="server" width="120px" /></div>
							<div style="white-space:nowrap" id="divActionRequired" runat="server" ><span style="width:60px;">Required:</span><asp:dropdownlist id="ddlActionRequired" runat="server" width="120px" /></div>
							<div style="white-space:nowrap" id="divActionSetValue" runat="server" ><span style="width:60px;">Set Value:</span><asp:dropdownlist id="ddlActionSetValue" runat="server" width="300px" /></div>
							<div style="white-space:nowrap" id="divActionSetActiveWorkflow" runat="server" ><span style="width:120px;">Set Active Workflow:</span><asp:dropdownlist id="ddlActionSetActiveWorkflow" runat="server" width="300px" /></div>
					</td>
				</tr>
				<tr style="padding: 10px 5px 10px 5px;" id="Active" runat="server" >
					<td class="DataEntryCaption" style="font-weight:bold;">Is Active:</td>
					<td class="DataEntryCaption AlignLeft"><asp:CheckBox ID="cbActive" runat="server"/></td>
				</tr>		

				<tr style="padding: 10px 5px 5px 5px;">
					<td>&nbsp;</td>
					<td class="DataEntryCaption AlignLeft" style="white-space:nowrap">
						<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" onclick="btnOK_Click" />
						<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" onclick="btnCancel_Click" />
					</td>
				</tr>

			</table>

		</asp:panel>


		<div id="divCondition" runat="server" style="position:absolute; display:none; z-index: 2000; border: 2px outset #006; top: 30px; left:1%; height: 260px; width: 99%; padding: 4px;">

			<div class="PageTitle">Add Condition</div>

			<div style="margin:10px 0 0 10px;">
				<span class="FormCaption" style="width:80px;">Depends On:</span>
				<asp:dropdownlist id="ddlSourceTerm" runat="server"  autopostback="true" onselectedindexchanged="ddlSourceTermOnSelectedIndexChanged" />
			</div>
	
			<div style="margin:10px 0 0 10px;">
				<span class="FormCaption" style="width:80px;">Operator:</span>
				<asp:dropdownlist id="ddlOperator" runat="server" width="150px"  autopostback="true" onselectedindexchanged="ddlOperatorOnSelectedIndexChanged"  />
			</div>
	
			<div style="margin:10px 0 0 10px;">
				<span class="FormCaption" style="width:80px;">Value(s):</span>

				<asp:textbox id="txtValue1" runat="server" width="200px" visible="false" />
				<asp:dropdownlist id="ddlValue1" runat="server" visible="false" />
				<asp:textbox id="dateValue1" runat="server" width="114px"  cssclass="DateControlTextBox" maxlength="12" visible="false" onfocus="javascript:this.select();" /><asp:image id="dateImage1" runat="server"  cssclass="DateControlImage" imagealign="top" imageurl="/Global/bin/Calendar/calendar.gif" visible="false" onclick="window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('dateValue1'), null, -600, 3600);" />	

				<asp:label id="lblOperatorMiddleText" runat="server" cssclass="FormCaption"  />

				<asp:textbox id="txtValue2" runat="server" width="200px" visible="false" />
				<asp:dropdownlist id="ddlValue2" runat="server" visible="false" />
				<asp:textbox id="dateValue2" runat="server" width="114px"  cssclass="DateControlTextBox" maxlength="12" visible="false" onfocus="javascript:this.select();" /><asp:image id="dateImage2" runat="server"  cssclass="DateControlImage" imagealign="top" imageurl="/Global/bin/Calendar/calendar.gif" visible="false"  onclick="window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('dateValue2'), null, -600, 3600);" />						
			</div>
			
			<div style="margin:15px 0 0 0; text-align:center;">
				<asp:button id="btnAddConditionOK" runat="server"  cssclass="KnectButton" text="OK" onclientclick="document.getElementById('hfAddCondition').value='0';" onclick="btnAddConditionOKOnClick" />&nbsp;
				<button id="btnAddConditionCancel" class="KnectButton" onclick="ShowPopup(false);">Cancel</button>
			</div>		
		</div>
		
		<iframe src="" id="ifrBlocker" style="position:absolute; display:none;">
		</iframe>
	
	</form>
	
</body>
</html>
