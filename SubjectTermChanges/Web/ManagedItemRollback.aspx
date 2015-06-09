<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagedItemRollback.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ManagedItemRollback" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>

<body id="body" runat="server" class="NoScroll" title="">
	<form id="form1" runat="server">
    <asp:HiddenField ID="hfRollbackOption" runat="server" />
		<kh:manageditemheader id="header" runat="server" hidebuttons="true" />
		
			<asp:panel id="pnlGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" style="border:solid #333; border-width:1px 1px 1px 1px;">
				<kh:kindredgridview id="grdResults" 
							runat="server" 
							cellpadding="2"
							cellspacing="0"	
							cssclass="NetTable"
							autogeneratecolumns="False" 
							rowhighlighting="true" 
							confirmondelete="false" 
						  datakeynames="ManagedItemAuditID"
							enabledoubleclickevent="false" 
							enableheaderclick="false" 
							enableclickevent="true" 
							headercontainer="pnlGridHeader" 
							headerrowsize="1" 
							container="pnlGridBody" 
							sortascending="false" 
							sortcolumn="-1"
							onrowcommand="grdResultsRowCommand"
							onrowdatabound="grdResultsRowDataBound"
				>
					<columns>
					
						<asp:boundfield  datafield="LogOnID" readonly="True" headertext="UserID">
							<headerstyle width="70px"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						
						<asp:boundfield  datafield="PersonName" readonly="True" headertext="Person">
							<headerstyle width="35%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						
						<asp:boundfield  datafield="AuditTypeName" readonly="True" headertext="Retro/Rollback">
							<headerstyle width="20%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						
						<asp:boundfield  datafield="Status" readonly="True" headertext="Status">
							<headerstyle width="20%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						
						<asp:templatefield headertext="State">
							<headerstyle width="25%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
							<itemtemplate>
								<asp:literal id="litState" runat="server" />
							</itemtemplate>
						</asp:templatefield>
						
						<asp:boundfield  datafield="DateOfChange" readonly="True" headertext="Date" dataformatstring="{0:MM/dd/yyyy hh:mm tt}" htmlencode="false">
							<headerstyle width="135px"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						
					</columns>
				</kh:kindredgridview>
			</asp:panel>
		
		<div runat="server" id="divButtons" class="ProfileCaption" style="text-align:center; border: solid #333; border-width:0 1px 1px 1px; padding: 3px 3px 3px 3px">
			<asp:button id="btnRollback" runat="server" enabled="false" cssclass="KnectButton" text="Rollback" />
		</div>


	</form>
</body>
</html>
