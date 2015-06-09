<%@ page language="C#" autoeventwireup="true" codebehind="ManagedItemComplexList.aspx.cs" inherits="Kindred.Knect.ITAT.Web.ManagedItemComplexList" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>
<body id="htmlBody" runat="server"  class="NoScroll">
	<form id="form1" runat="server">
		 <kh:manageditemheader id="header" runat="server" onheaderevent="OnHeaderEvent" />

		<div class="TemplateEditBody" id="divTermContainer">

			<div id="divGridTitle" class="GridTitle">
                <span style="float:right; text-align:right; padding: 6px 0 0 0;">
			        <asp:button id="btnAdd" cssclass="KnectButton" visible="false" runat="server" text="Add" onclick="btnAdd_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
            		<asp:button id="btnExportToExcel" cssclass="KnectButton" runat="server" visible="true" text="Export To Excel" onclientclick="return SaveGrid();" onclick="btnExportToExcel_Click" />
			    </span>	
			    <span style="float:right; margin: 0 8px 0 0;">
					<asp:imagebutton id="btnItemMoveUp" runat="server" visible="false" width="11px" height="11px" alternatetext="Move Row Up" oncommand="btnSwitchRows_Command" /><br />
					<asp:imagebutton id="btnItemMoveDown" runat="server" visible="false" width="11px" height="11px" alternatetext="Move Row Down" oncommand="btnSwitchRows_Command"  />
			    </span>
		    </div>
	
		    <asp:panel id="pnlGridHeader" runat="server" visible="false" />
		    <asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel">
			    <kh:kindredgridview id="grdList" 
						    runat="server" 
						    cellpadding="2"
						    cellspacing="0"	
						    cssclass="NetTable"
						    onrowdatabound="grdList_RowDataBound"
						    onrowediting = "grdList_RowEditing"
						    onrowdeleting = "grdList_RowDeleting"
						    onrowcommand="grdList_RowCommand" 
						    autogeneratecolumns="False" 
						    datakeynames ="Index" 
						    rowhighlighting="true" 
						    confirmondelete="False" 
						    enabledoubleclickevent="true" 
						    enableheaderclick="true" 
						    enableclickevent="false" 
						    headercontainer="pnlGridHeader" 
						    headerrowsize="1" 
						    container="pnlGridBody" 
						    sortascending="True" 
						    sortcolumn="-1"
			    >
				    <columns>
				    </columns>
			    </kh:kindredgridview>
		    </asp:panel>
		
		</div>
		
</form>
</body>
</html>
