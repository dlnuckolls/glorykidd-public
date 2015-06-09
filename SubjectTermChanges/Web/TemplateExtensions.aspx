<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateExtensions.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateExtensions" %>
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

		<div class="TemplateEditBody" id="divContainer">
			<div class="GridTitle" style="margin-right:-2px;">
				<table border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td class="1stTblColor GridTitle AlignTop">Extensions</td>
						<td runat="server" id="tdTreeViewHeader" class="1stTblColor AlignRight"  style="width:100%; padding: 0 5px 1px 5px;">
									<asp:imagebutton id="imgMoveUp" runat="server" imageurl="~/Images/MoveUp.gif"  width="11px" height="11px" alternatetext="Move Clause Up" onclick="imgMoveUp_OnClick"  /><br />
									<asp:imagebutton id="imgMoveDown" runat="server" imageurl="~/Images/MoveDown.gif"  width="11px" height="11px" alternatetext="Move Clause Down" onclick="imgMoveDown_OnClick"  />
						</td>
					</tr>
				</table>	
			</div>

			<asp:panel id="pnlGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" style="height: 280px;">

				<kh:kindredgridview id="grd" runat="server" 
								cssclass="NetTable" 
								cellpadding="2" 
								cellspacing="0" 
								onrowcreated="grd_RowCreated" 
								onrowcommand="grd_RowCommand" 
								autogeneratecolumns="False" 
								datakeynames="ObjectId" 
								rowhighlighting="true" 
								confirmondelete="False" 
								enabledoubleclickevent="false" 
								enableheaderclick="false" 
								enableclickevent="true" 
								headercontainer="pnlGridHeader" 
								headerrowsize="1" 
								container="pnlGridBody" 
								sortascending="True" 
								sortcolumn="-1"
								>
					<columns>
						<asp:boundfield datafield="Filename" readonly="True" headertext="Filename">
							<headerstyle width="100%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:buttonfield buttontype="Link" text="View" headertext="">
							<headerstyle width="70px" />
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
						<asp:buttonfield buttontype="Link" text="Delete" headertext="" commandname="DeleteRow">
							<headerstyle width="70px" />
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
					</columns>
				</kh:kindredgridview>

			</asp:panel>
		</div>

		<div class="TemplateEditBody" id="divAddFile" style="margin: 3px 0 0 0;">
			<kh:fileupload id="filAddExtension" runat="server" caption="New Extension:" buttontext="Add"  onupload="filAddExtension_Upload" cssclass="DataEntryCaption"  />
		</div>
			
    </form>
</body>
</html>
