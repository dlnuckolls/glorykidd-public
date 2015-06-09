<%@ page language="C#" autoeventwireup="true" codebehind="TemplateList.aspx.cs" inherits="Kindred.Knect.ITAT.Web.TemplateList" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>
<body id="htmlBody" runat="server"  class="NoScroll">
	<form id="form1" runat="server">
		<kh:standardheader id="itatHeader" runat="server"  />
		<div style="text-align: right; margin: 2px 16px 2px 0;">
			Show:
			<asp:dropdownlist id="ddlShowFilter" runat="server" autopostback="true" />
			&nbsp;&nbsp;
			<asp:button id="btnAdd" cssclass="KnectButton" runat="server" text="Add" onclick="btnAdd_Click" />
		</div>
		<asp:panel id="pnlGridHeader" runat="server" visible="false" />
		<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel">
			<kh:kindredgridview id="grdTemplateList" 
						runat="server" 
						cellpadding="2"
						cellspacing="0"	
						cssclass="NetTable"
						onrowdatabound="grdTemplateList_RowDataBound"
						onrowcommand="grdTemplateList_RowCommand" 
						autogeneratecolumns="False" 
						datakeynames ="TemplateID" 
						rowhighlighting="false" 
						confirmondelete="False" 
						enabledoubleclickevent="false" 
						enableheaderclick="false" 
						enableclickevent="false" 
						headercontainer="pnlGridHeader" 
						headerrowsize="1" 
						container="pnlGridBody" 
						sortascending="True" 
						sortcolumn="-1"
			>
				<columns>
					<asp:boundfield  datafield="TemplateName" readonly="True" headertext="Name">
						<headerstyle width="50%"></headerstyle>
						<itemstyle horizontalalign="left"></itemstyle>
					</asp:boundfield>
					<asp:boundfield datafield="Status" readonly="True" headertext="Status">
						<headerstyle width="25%"></headerstyle>
						<itemstyle horizontalalign="center"></itemstyle>
					</asp:boundfield>
					<asp:buttonfield  buttontype="Link" text="Edit" headertext="" commandname="Edit">
						<headerstyle width="15%" />
						<itemstyle horizontalalign="center"></itemstyle>
					</asp:buttonfield>
					<asp:buttonfield  buttontype="Link"  text="Copy" headertext="" commandname="Copy">
						<headerstyle width="15%" />
						<itemstyle horizontalalign="center"></itemstyle>
					</asp:buttonfield>
				</columns>
			</kh:kindredgridview>
		</asp:panel>
	</form>
</body>
</html>
