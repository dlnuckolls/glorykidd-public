<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateTermDependencies.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateTermDependencies" title="Term Dependencies"%>

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
				<span style="float:left; text-align:left;">Term Dependencies</span>
				<span style="float:right; text-align:right; padding: 6px 0 0 0;">
					<asp:button id="btnAddDependency" runat="server" cssclass="KnectButton" text="Add" oncommand="btnAdd_Command"  commandname="Add" />
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
                        <asp:boundfield datafield="DependentTerms"  HtmlEncode="false" readonly="True" headertext="Term List" >
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
