<%@ page language="C#" autoeventwireup="true" codebehind="TemplateWorkflowMain.aspx.cs" inherits="Kindred.Knect.ITAT.Web.TemplateWorkflowMain" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>

<body id="htmlBody" class="NoScroll" runat="server">
	<form id="form1" runat="server">

		<kh:standardheader id="header" runat="server"  pagetitle="Edit Workflow" />

		<div class="TemplateEditBody" id="divContainer">
			<div class="GridTitle" style="margin-right:-2px;">
				<table border="0" cellpadding="0" cellspacing="0" style="table-layout:fixed; height:24px; width:100%;">
					<tr>
 					    <td class="DataEntryCaption AlignLeft" style="width:300px;">
 							 Name:<asp:textbox id="txtWorkflowName" runat="server" width="190px"  />&nbsp;
							<asp:checkbox id="chkboxDefaultWorkflow" runat="server" text="" textalign="Left" cssclass="CheckBoxAlignLeft"  />Default
						</td>
						<td class="1stTblColor GridTitle AlignMiddle" style="width:auto"></td>
							<td runat="server" id="tdTreeViewHeader" class="1stTblColor AlignRight"  style="width:100%; padding: 4px 2px 1px 5px; white-space:nowrap;">
							<asp:Button id="btnRetroNotification" runat="server" Width="120px" CssClass="KnectButton" Text="Retro Notification" OnCommand="btnRetroNotification_Command" /><asp:button id="btnLimitTime" runat="server" width="80px" cssclass="KnectButton" text="Limit Time" oncommand="btnLimitTime_Command" /><asp:button id="btnAddTerm" runat="server" width="90px" cssclass="KnectButton" text="Add" oncommand="btnAdd_Command" />
						</td>
						<td class="1stTblColor" style="width:16px; padding: 0 5px 0 0;">
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
								datakeynames="Name" 
								rowhighlighting="true" 
								confirmondelete="False" 
								enabledoubleclickevent="false" 
								enableheaderclick="false" 
								enableclickevent="true" 
								headercontainer="pnlGridHeader" 
								headerrowsize="2" 
								container="pnlGridBody" 
								sortascending="True" 
								sortcolumn="-1"
								>
					<columns>
						<asp:boundfield datafield="Name" readonly="True" headertext="State Name">
							<headerstyle width="60%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="Status" readonly="True" headertext="Status">
							<headerstyle width="40%"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="RequiresValidation" readonly="True" headertext="Requires Validation">
							<headerstyle width="80px"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="IsDraft" readonly="True" headertext="Water mark">
							<headerstyle width="45px"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="IsBase" readonly="True" headertext="Is Base">
							<headerstyle width="40px"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="IsExit" readonly="True" headertext="Is Exit">
							<headerstyle width="40px"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>
						<asp:buttonfield buttontype="Link" text="Edit" headertext="" commandname="EditRow">
							<headerstyle width="40px" />
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
						<asp:buttonfield buttontype="Link" text="Delete" headertext="" commandname="DeleteRow">
							<headerstyle width="50px" />
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:buttonfield>
					</columns>
				</kh:kindredgridview>

			</asp:panel>

		</div>
		
		<div class="DataEntryCaption AlignCenter" style="padding: 8px 0 8px 0;">
			<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" oncommand="btnOK_Command" />
			<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" oncommand="btnCancel_Command" />
		</div>	

	</form>
</body>

</html>
