<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagedItemImport.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ManagedItemImport" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title>ITAT Data Import</title>
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
			<div class="PageTitle">ITAT Data Import</div>

			<div class="TemplateEditBody" id="divAddFile" style="margin: 4px 0  4px 0; padding:10px 5px 10px 24px;">
				<div class="DataEntryCaption AlignLeft" style="margin:10px 0 10px 0;">Select an input file, and click the Start button to begin the Data Import Process.&nbsp;&nbsp;Do not close the browser or click the Back button during this process.</div>
				<div class="DataEntryCaption AlignLeft" style="margin:10px 0 10px 0;">Please note that this may take several minutes to complete.&nbsp;&nbsp;Please be patient.</div>

				<div nowrap style="margin:20px 0 0 0;">
					<span class="DataEntryCaption" style="width:107px; padding:0 5px 0 0;">Template: </span>
					<span class="DataEntryEdit"><asp:dropdownlist id="ddlTemplate" runat="server" width="400px" /></span>
				</div>
			
				<div nowrap style="margin:4px 0 5px 0;">
					<span class="DataEntryCaption" style="width:107px; padding:0 5px 0 0;">Import Data File: </span>
					<asp:fileupload runat="server" id="filUpload" cssclass="TextBox" width="400px" />
				</div>
			</div>

			<div style="text-align:center; margin:10px 0 0 0;">
				<div id="divButton" style="display:block;"><asp:button runat="server" id="btnStart" cssclass="KnectButton" text="Start" onclientclick="return ShowMessage(this);" /></div>
				<div id="divMessage" style="display:none;" class="Error">Processing...</div>
			</div>				
					
		</form>
	</body>
</html>
