<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateDocuments.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateDocuments" Title="Documents" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />    
</head>
<body id="htmlBody" class="NoScroll" runat="server">
    <form id="form1" runat="server">
        <kh:templateheader id="header" runat="server" onheaderevent="OnHeaderEvent" />

		<div class="TemplateEditBody" id="divDependencyContainer" style="margin-top: 5px;">
			<div class="GridTitle">
				<span style="float:left; text-align:left;">Documents</span>
				<span style="float:right; text-align:right; padding: 6px 0 0 0;">
					<asp:button id="btnAddDocument" runat="server" cssclass="KnectButton" text="Add" oncommand="btnAdd_Command"  commandname="Add" />
				</span>	
				<span style="float:right;  margin: 0 4px 0 0;">
						<asp:imagebutton id="btnDocumentMoveUp" runat="server" width="11px" height="11px" alternatetext="Move Row Up" oncommand="btnSwitchDocumentRows_Command" /><br />
						<asp:imagebutton id="btnDocumentMoveDown" runat="server" width="11px" height="11px" alternatetext="Move Row Down" oncommand="btnSwitchDocumentRows_Command"  />
				</span>
			</div>
			<asp:panel id="pnlDocumentGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlDocumentGridBody" runat="server" cssclass="GridBodyPanel" style="height:61px;">
				<kh:kindredgridview id="grdDocument" 
							runat="server" 
							cssclass="NetTable"
							onrowcreated="grdDocument_RowCreated"	
							onrowcommand="grdDocument_RowCommand" 
							autogeneratecolumns="False" 
							datakeynames="ITATDocumentID" 
							rowhighlighting="True" 
							confirmondelete="False" 
							enabledoubleclickevent="true" 
							enableheaderclick="false" 
							enableclickevent="true" 
							headercontainer="pnlDocumentGridHeader" 
							headerrowsize="1" 
							container="pnlDocumentGridBody" 
							sortascending="True" 	
							sortcolumn="-1"
				>
					<columns>
						<asp:boundfield datafield="DocumentName" readonly="True" headertext="Document Name" >
							<headerstyle width="50%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="WorkflowEnabled" readonly="True" headertext="Workflow Enabled">
							<headerstyle width="25%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="DefaultDocument" readonly="True" headertext="Default Document">
							<headerstyle width="25%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:buttonfield  buttontype="Link" text="Edit" headertext="" commandname="EditRow">
							<headerstyle width="100px" />
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
						<asp:buttonfield  buttontype="Link" text="Delete" headertext="" commandname="DeleteRow">
							<headerstyle width="100px" />
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
						<asp:buttonfield  buttontype="Link" text="Preview" headertext="" commandname="Preview">
							<headerstyle width="100px" />
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
					</columns>
				</kh:kindredgridview>
			</asp:panel>
		</div>  

    </form>
</body>
</html>
