<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateWorkflows.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateWorkflows" %>

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


		<div class="TemplateEditBody" id="divWorkflowContainer">
			<div class="GridTitle">
				<span class="AlignMiddle" style="float:left; text-align:left;">Workflows</span>
				<span style="float:right; text-align:right; padding: 6px 0 0 0;">
					<asp:button id="btnAddWorkflow" runat="server" cssclass="KnectButton" text="Add" oncommand="btnAdd_Command"  commandname="Add" commandargument="Workflow" />
				</span>	
				<span style="float:right; margin: 0 4px 0 0;">
						<asp:imagebutton id="btnWorkflowMoveUp" runat="server" width="11px" height="11px" alternatetext="Move Row Up" oncommand="btnSwitchRows_Command" /><br />
						<asp:imagebutton id="btnWorkflowMoveDown" runat="server" width="11px" height="11px" alternatetext="Move Row Down" oncommand="btnSwitchRows_Command"  />
				</span>
			</div>
			<asp:panel id="pnlWorkflowGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlWorkflowGridBody" runat="server" cssclass="GridBodyPanel" style="height:280px;">
				<kh:kindredgridview id="grdWorkflow" 
							runat="server" 
							cssclass="NetTable"
							cellpadding="2"	
							cellspacing="0"	
							onrowcreated="grd_RowCreated"	
							onrowcommand="grd_RowCommand" 
							autogeneratecolumns="False" 
							 datakeynames="Name" 
							rowhighlighting="True" 
							confirmondelete="False" 
							enabledoubleclickevent="false" 
							enableheaderclick="false" 
							enableclickevent="true" 
							headercontainer="pnlWorkflowGridHeader" 
							headerrowsize="1" 
							container="pnlWorkflowGridBody" 
							sortascending="True" 
							sortcolumn="-1"
				>
					<columns>
					    
						<asp:boundfield datafield="Name" readonly="True" headertext="Name" >
							<headerstyle width="100%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield readonly="True" headertext="Default" >
							<headerstyle width="100px"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>		
                        <asp:buttonfield  buttontype="Link" text="Copy" headertext="" commandname="CopyRow">
							<headerstyle width="100px" />	
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
						<asp:buttonfield  buttontype="Link" text="Edit" headertext="" commandname="EditRow">
							<headerstyle width="100px" />
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
						<asp:buttonfield  buttontype="Link" text="Delete" headertext="" commandname="DeleteRow">
							<headerstyle width="100px" />	
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
						<asp:boundfield datafield="Id" readonly="True" headertext="" visible="false">
							<headerstyle width="100%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
			</columns>
				</kh:kindredgridview>
			</asp:panel>
		</div>



		<div class="TemplateEditBody" id="divDependencyContainer" style="margin-top: 5px;">
			<div class="GridTitle">
				<span style="float:left; text-align:left;">Workflow Dependencies</span>
				<span style="float:right; text-align:right; padding: 6px 0 0 0;">
					<asp:button id="btnAddDependency" runat="server" cssclass="KnectButton" text="Add" oncommand="btnAdd_Command"  commandname="Add" commandargument="Dependency" />
				</span>	
				<span style="float:right;  margin: 0 4px 0 0;">
						<asp:imagebutton id="btnDependencyMoveUp" runat="server" width="11px" height="11px" alternatetext="Move Row Up" oncommand="btnSwitchDependencyRows_Command" /><br />
						<asp:imagebutton id="btnDependencyMoveDown" runat="server" width="11px" height="11px" alternatetext="Move Row Down" oncommand="btnSwitchDependencyRows_Command"  />
				</span>
			</div>
			<asp:panel id="pnlDependencyGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlDependencyGridBody" runat="server" cssclass="GridBodyPanel" style="height:61px;">
				<kh:kindredgridview id="grdDependency" 
							runat="server" 
							cssclass="NetTable"
							onrowcreated="grdDependency_RowCreated"	
							onrowcommand="grdDependency_RowCommand" 
							autogeneratecolumns="False" 
							datakeynames="ID" 
							rowhighlighting="True" 
							confirmondelete="False" 
							enabledoubleclickevent="true" 
							enableheaderclick="false" 
							enableclickevent="true" 
							headercontainer="pnlDependencyGridHeader" 
							headerrowsize="1" 
							container="pnlDependencyGridBody" 
							sortascending="True" 	
							sortcolumn="-1"
				>
					<columns>
						<asp:boundfield readonly="True" headertext="Workflow" >
							<headerstyle width="25%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="SourceTermText" readonly="True" headertext="Depends On">
							<headerstyle width="75%"></headerstyle>
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
					</columns>
				</kh:kindredgridview>
			</asp:panel>
		</div>

	



	
	</form>
</body>
</html>
