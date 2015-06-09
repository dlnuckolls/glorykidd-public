<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateWorkflowActionEdit.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateWorkflowActionEdit" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>
	<script type="text/javascript">
		function UpdatePreview()
		{
			var dest = document.getElementById('btnButtonPreview');
			var src = document.getElementById('txtButtonText');
			if (dest)
				if (src)
					dest.innerText = src.value;
		}
	
	</script>

<body id="htmlBody" class="NoScroll" runat="server" onreadystatechange="if (readyState=='complete') UpdatePreview();">
	<form id="form1" runat="server">
		<kh:standardheader id="header" runat="server"  pagetitle="Edit State" />
			
	<div class="TemplateEditBody" id="divContainer">
	
		<table width="100%" cellpadding="0" cellspacing="0" border="0">
			<tr>
				
				<td width="50%" style="border-right:1px solid #666; border-bottom:1px solid #666;">
						<table cellpadding="2" cellspacing="0" border="0" style="margin: 0 5px 5px 0; table-layout:fixed">
							<colgroup>
								<col width="75px" />
								<col width="100%" />
							</colgroup>	
							<tr>
								<td class="DataEntryCaption">Button Text<br />&nbsp;</td>	
								<td class="DataEntryCaption AlignLeft">
									<asp:textbox id="txtButtonText" runat="server" textmode="SingleLine" 	width="100%" onkeyup="UpdatePreview();" /><br />
									Preview: <span id="btnButtonPreview" class="KnectButton ActionButton" style="border:2px outset">Preview</span>
								</td>
							</tr>
							<tr>
								<td class="DataEntryCaption">Target State</td>	
								<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlTargetState" width="100%" runat="server" /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption">
									Performers<br /><br />						
									<button id="btnClearPerformers" runat="server" class="KnectButton" style="width:60px" onclick="_kh_deselectListItems(document.all.lstPerformers);">Clear</button>
								</td>	
								<td class="DataEntryCaption AlignLeft"><asp:listbox id="lstPerformers" runat="server" width="100%" rows="6" selectionmode="multiple" /></td>
							</tr>
							<tr id="rowRequiresConfirmation" runat="server" visible="false">
								<td class="DataEntryCaption">Requires Confirmation</td>	
								<td class="DataEntryCaption AlignLeft"><asp:CheckBox id="chkRequiresConfirmation" width="100%" runat="server" /></td>
							</tr>
							<tr id="rowConfirmationText" runat="server" visible="false">
								<td class="DataEntryCaption">Confirmation Text</td>	
								<td class="DataEntryCaption AlignLeft"><asp:Textbox id="txtConfirmationText" width="100%" runat="server" /></td>
							</tr>
						</table>
				</td>	
				
				<td width="50%">
						<table cellpadding="2" cellspacing="0" border="0" width="100%" style="table-layout:fixed">
							<colgroup>
								<col width="95px" />
								<col width="100%" />
							</colgroup>	
							<tr>
								<td colspan="2" class="AlignCenter" style="text-decoration:underline; font-weight:bold;">Notification</td>
							</tr>
							<tr>
								<td class="DataEntryCaption">Subject</td>	
								<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtSubject" runat="server" width="100%" textmode="SingleLine" /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption">
									Recipients<br /><br />
									<button id="btnClearRecipients" runat="server" class="KnectButton" style="width:60px" onclick="_kh_deselectListItems(document.all.lstRecipients);">Clear</button>
								</td>	
								<td class="DataEntryCaption AlignLeft"><asp:listbox id="lstRecipients" runat="server" width="100%" rows="6" selectionmode="multiple" /></td>
							</tr>
							<tr id="row_ddlFilterFacility" runat="server" visible="false">
								<td class="DataEntryCaption">Filter By Facility</td>	
								<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlFilterFacility" width="100%" runat="server" /></td>
							</tr>
						</table>
				</td>	
				
			</tr>
		</table>
			
			<asp:panel id="divEditor" runat="server" style="display: block; margin: 2px 0 0 0; width: 100%; border: 1px solid #666; font-family:Times New Roman; font-size:12pt">
			<rade:radeditor id="edt" 
					runat="server" 
					toolsfile="~/Config/TemplateClauseToolsFile.xml" 
					font-names="Times New Roman, Arial, Courier New"
					onclientload="radOnClientLoad"
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
	
		<div class="DataEntryCaption AlignCenter" style="padding: 8px 0 8px 0;">
			<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" oncommand="btnOK_Command" />
			<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" oncommand="btnCancel_Command" />
		</div>	
		
	</div>
		
		
    </form>
</body>
</html>
