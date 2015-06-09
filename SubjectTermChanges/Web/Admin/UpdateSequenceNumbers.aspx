<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateSequenceNumbers.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.UpdateSequenceNumbers" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title>Update Sequence Numbers</title>
		<link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
		<link rel="stylesheet" type="text/css" href="../itat.css" />
	</head>
	<body id="htmlBody" runat="server">
		<form id="ExceptionPage" method="post" runat="server">
			<div class="PageTitle">Update Sequence Numbers</div>

			<div class="TemplateEditBody" id="divAddFile" style="margin: 4px 0  4px 0; padding:10px 5px 10px 24px;">
				<div class="DataEntryCaption AlignLeft" style="margin:10px 0 10px 0;">Select an input XML document, and click the Submit button to update this system's sequence numbers.</div>

				<div nowrap style="margin:4px 0 5px 0;">
					<span class="DataEntryCaption" style="width:100px; padding:0 5px 0 0;">&nbsp;</span>
					<asp:checkbox runat="server" id="chkDeleteFirst" cssclass="DataEntryCaption CheckBoxAlignLeft" text="Delete this system's entries first" />
				</div>

				<div nowrap style="margin:4px 0 5px 0;">
					<span class="DataEntryCaption" style="width:100px; padding:0 5px 0 0;">XML Input: </span>
					<asp:fileupload runat="server" id="filUpload" cssclass="TextBox" width="400px" />
				</div>

			</div>

			<div style="text-align:center; margin:10px 0 0 0;">
				<div id="divButton" style="display:block;"><asp:button runat="server" id="btnStart" cssclass="KnectButton" text="Submit" onclientclick="return ShowWait(this);" /></div>
			</div>				
					
		</form>
	</body>
</html>
