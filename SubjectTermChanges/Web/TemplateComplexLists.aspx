<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateComplexLists.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateComplexLists" title="Complex Lists"%>

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
		<div class="TemplateEditBody" id="divListContainer" style="margin-top: 5px;">
			<div class="GridTitle">
				<span style="float:left; text-align:left;">Complex Lists</span>
				<span style="float:right; text-align:right; padding: 6px 0 0 0;">
					<asp:button id="btnAddList" runat="server" cssclass="KnectButton" text="Add" oncommand="btnAdd_Command"  commandname="Add" commandargument="ComplexList" />
				</span>	
				<span style="float:right; margin: 0 4px 0 0;">
                    <kh:GridMoveRows runat="server" ID="ucGridMoveRows" TargetControl="grdList" />
				</span>
			</div>
			<asp:panel id="pnlListGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlListGridBody" runat="server" cssclass="GridBodyPanel" style="height:46px;">
				<kh:kindredgridview id="grdList" 
							runat="server" 
							cssclass="NetTable"
							onrowcreated="grdList_RowCreated"	
							onrowcommand="grd_RowCommand" 
							autogeneratecolumns="False" 
							datakeynames="ID" 
							rowhighlighting="True" 
							confirmondelete="False" 
							enabledoubleclickevent="false" 
							enableheaderclick="false" 
							enableclickevent="false" 
							headercontainer="pnlListGridHeader" 
							headerrowsize="1" 
							container="pnlListGridBody" 
							sortascending="True" 	
							sortcolumn="-1"
				>
					<columns>
						<asp:boundfield datafield="Name" readonly="True" headertext="Name" >
							<headerstyle width="100%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="TermType" readonly="True" headertext="Type">
							<headerstyle width="100px"></headerstyle>
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
