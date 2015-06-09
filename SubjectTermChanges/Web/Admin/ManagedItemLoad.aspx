<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagedItemLoad.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ManagedItemLoad" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title>ITAT Bulk Data Load</title>
		<link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
		<link rel="stylesheet" type="text/css" href="../itat.css" />
		<script type="text/javascript">
			function ShowMessage(sender)
			{
				document.all.divMessage.style.display='block';
				return ShowWait(sender);
			}
		</script>
	</head>
	<body id="htmlBody" runat="server">
		<form id="ExceptionPage" method="post" runat="server">
			<div class="PageTitle">ITAT Bulk Data Load</div>

			<div class="TemplateEditBody" id="divAddFile" style="margin: 4px 0  4px 0; padding:10px 5px 10px 24px;">
				<div class="DataEntryCaption AlignLeft" style="margin:10px 0 10px 0;">If you would like to perform a Bulk Data Load, first select a System and a Template, and then click the Load button to begin the Data Load Process.&nbsp;&nbsp;Do not close the browser or click the Back button during this process.</div>
				<div class="DataEntryCaption AlignLeft" style="margin:10px 0 10px 0;">(Please note that this may take several minutes to complete.&nbsp;&nbsp;Please be patient.)</div>
				<div class="DataEntryCaption AlignLeft" style="margin:10px 0 10px 0;">You may also create a blank excel file to be used for a later Data Load.  First select a System and a Template, and then click the 'Export Template Excel' button.</div>

				<div nowrap style="margin:20px 0 0 0;">
					<span class="DataEntryCaption" style="width:150px; padding:0 5px 0 0;">System: </span>
					<span class="DataEntryEdit"><asp:dropdownlist id="ddlSystem" runat="server" width="400px"  onselectedindexchanged="ddlSystem_SelectedIndexChanged" AutoPostBack="true" /></span>
				</div>

				<div nowrap style="margin:4px 0 0 0;">
					<span class="DataEntryCaption" style="width:150px; padding:0 5px 0 0;">Source Template: </span>
					<span class="DataEntryEdit"><asp:dropdownlist id="ddlSourceTemplate" runat="server" width="400px" /></span>
				</div>

				<div id="divDestinationTemplate" visible="false" runat="server" style="margin:4px 0 0 0;">
					<span class="DataEntryCaption" style="width:150px; padding:0 5px 0 0;">Destination Template:<br />(For Retro)</span>
					<span class="DataEntryEdit"><asp:dropdownlist id="ddlDestinationTemplate" runat="server" width="400px" /></span>
				</div>
			
				<div nowrap style="margin:4px 0 5px 0;">
					<span class="DataEntryCaption" style="width:150px; padding:0 5px 0 0;">Data Load File: </span>
					<asp:fileupload runat="server" id="filUpload" cssclass="TextBox" width="400px" />
				</div>
			</div>

			<div class="ProfileCaption" id="divConfirmationMessage"  visible="false" style="text-align:center; margin:1px 0 0 0;" runat="server" >
			<asp:label id="lblConfirmationMessage" runat="server"  Text="Confirmation Message:" style="vertical-align:text-top;" />
			<asp:textbox id="txtConfirmationMessage" runat="server" textmode="MultiLine" Wrap="false" Rows="12" Width="500px" />
			</div>				

            <div class="ProfileCaption" style="text-align: center; border: solid #333; border-width: 0 1px 1px 1px; padding: 3px 3px 3px 3px">
                <asp:button runat="server" id="btnExportTemplateExcel" cssclass="KnectButtonWide" text="Export Template Excel" OnClick="btnExportTemplateExcel_Click" />       
				<asp:button runat="server" id="btnLoad" cssclass="KnectButtonWide" text="Load" OnClick="btnLoad_Click" />
				<asp:button runat="server" id="btnLoadTest" cssclass="KnectButtonWide" text="*Load Test*" OnClick="btnLoadTest_Click" />
				<asp:button runat="server" id="btnHelp" Visible="false" cssclass="KnectButtonWide" text="Help" />
				<asp:button runat="server" id="btnExportFeedbackExcel" cssclass="KnectButtonWide"  text="Export Feedback Excel" OnClick="btnExportFeedbackExcel_Click" Visible="false" />
            </div>

		</form>
	</body>
</html>
