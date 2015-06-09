<%@ page language="C#" autoeventwireup="true" codebehind="TemplateTerms.aspx.cs" inherits="Kindred.Knect.ITAT.Web.TemplateTerms" title="Terms" %>

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

		<div class="TemplateEditBody" id="divTermGroupContainer" style="margin-top: 5px;" runat="server">
			<div class="GridTitle">
				<span style="float:left; text-align:left;">Tabs</span>
				<span style="float:right; text-align:right; padding: 6px 0 0 0;">
					<asp:button id="btnAddTermGroup" runat="server" cssclass="KnectButton" text="Add" oncommand="btnAdd_Command"  commandname="Add" commandargument="TermGroup" />
				</span>	
				<span style="float:right;  margin: 0 4px 0 0;">
						<asp:imagebutton id="btnTermGroupMoveUp" runat="server" width="11px" height="11px" alternatetext="Move Row Up" oncommand="btnSwitchTermGroupRows_Command" /><br />
						<asp:imagebutton id="btnTermGroupMoveDown" runat="server" width="11px" height="11px" alternatetext="Move Row Down" oncommand="btnSwitchTermGroupRows_Command"  />
				</span>
			</div>
			<asp:panel id="pnlTermGroupGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlTermGroupGridBody" runat="server" cssclass="GridBodyPanel" style="height:150px;">
				<kh:kindredgridview id="grdTermGroup" 
							runat="server" 
							cssclass="NetTable"
							onrowcreated="grdTermGroup_RowCreated"	
							onrowcommand="grdTermGroup_RowCommand"							 
							autogeneratecolumns="False" 
							datakeynames="ID" 
							rowhighlighting="True" 
							confirmondelete="False" 
							enabledoubleclickevent="true" 
							enableheaderclick="false" 
							enableclickevent="true" 
							headercontainer="pnlTermGroupGridHeader" 
							headerrowsize="1" 
							container="pnlTermGroupGridBody" 
							sortascending="True" 	
							sortcolumn="-1"
				>
					<columns>
						<asp:boundfield datafield="Name" readonly="True" headertext="Name" >
							<headerstyle width="25%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="Description" readonly="True" headertext="Description">
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

		<div class="TemplateEditBody" id="divTermContainer">
			<div class="GridTitle">
				<span class="AlignMiddle" style="float:left; text-align:left;">Terms</span>
				<span style="float:right; text-align:right; padding: 6px 0 0 0;">
					<asp:button id="btnAddTerm" runat="server" cssclass="KnectButton" text="Add" oncommand="btnAdd_Command"  commandname="Add" commandargument="Term"/>
				</span>	
				<span style="float:right; margin: 0 4px 0 0;">
						<kh:GridMoveRows runat="server" ID="ucGridMoveRows" TargetControl="grdTerm" />
				</span>
			</div>
			<asp:panel id="pnlTermGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlTermGridBody" runat="server" cssclass="GridBodyPanel" style="height:180px;">
				<kh:kindredgridview id="grdTerm" 
							runat="server" 
							cssclass="NetTable"
							cellpadding="2"	
							cellspacing="0"	
							onrowcreated="grd_RowCreated"	
							onrowcommand="grd_RowCommand" 
							onrowdatabound="grd_RowDataBound"
							autogeneratecolumns="False" 
							 datakeynames="ID" 
							rowhighlighting="True" 
							confirmondelete="False" 
							enabledoubleclickevent="false" 
							enableheaderclick="false" 
							enableclickevent="false" 
							headercontainer="pnlTermGridHeader" 
							headerrowsize="1" 
							container="pnlTermGridBody" 
							sortascending="True" 
							sortcolumn="-1"
				>
					<columns>
						<asp:boundfield datafield="Name" readonly="True" headertext="Name" >
							<headerstyle width="100%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>	
						<asp:TemplateField HeaderText="">
						<HeaderStyle Width="100px" />
						<ItemStyle HorizontalAlign="Center" />
						    <ItemTemplate><asp:Literal runat="server" ID="litHeaderTerm" /></ItemTemplate>
						</asp:TemplateField>												
						<asp:boundfield datafield="TermGroupName" readonly="True" headertext="Tab Name">
							<headerstyle width="150px"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>						
						<asp:boundfield datafield="TermType" readonly="True" headertext="Type">
							<headerstyle width="150px"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
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
