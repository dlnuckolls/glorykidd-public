<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdateSystem.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.UpdateSystem" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="../itat.css" />
</head>

<body id="htmlBody" runat="server">
    <form id="form1" runat="server">
			<kh:standardheader id="itatHeader" pagetitle="System List" runat="server"  />
			
			<div class="TemplateEditBody" id="divContainer">
				<asp:panel id="pnlGridHeader" runat="server" visible="false" />
				<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" style="height: 280px;">

					<kh:kindredgridview id="grd" runat="server" 
										cssclass="NetTable" 
										cellpadding="2" 
										cellspacing="0" 
										onrowcreated="grd_RowCreated" 
										onrowcommand="grd_RowCommand" 
										autogeneratecolumns="False" 
										datakeynames="ITATSystemID" 
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
								<asp:boundfield datafield="ITATSystemID" readonly="True" headertext="System ID">
									<headerstyle width="250px"></headerstyle>
									<itemstyle horizontalalign="left"></itemstyle>
								</asp:boundfield>
								<asp:boundfield datafield="ITATSystemName" readonly="True" headertext="System Name">
									<headerstyle width="100%"></headerstyle>
									<itemstyle horizontalalign="left"></itemstyle>
								</asp:boundfield>
								<asp:buttonfield buttontype="Link" text="Download" headertext="" commandname="Download">
									<headerstyle width="80px" />
									<itemstyle horizontalalign="center"></itemstyle>
								</asp:buttonfield>
							</columns>
					</kh:kindredgridview>

				</asp:panel>
			</div>

			<div class="TemplateEditBody" id="divAddFile" style="margin: 3px 0 0 0;">
					<table width="100%" style="table-layout:fixed;">
						<colgroup>
							<col width="100px" />
							<col width="100%" />
						</colgroup>
						<tr>
							<td class="DataEntryCaption">System Name</td>
							<td class="DataEntryCaption">
								<asp:textbox id="txtSystemName" runat="server" width="100%" />
								<asp:hiddenfield id="hidSystemID" runat="server" value="" />
							</td>
						</tr>
						
						<tr>
							<td class="DataEntryCaption">Filename</td>
							<td class="DataEntryCaption"><asp:fileupload id="filUpload" runat="server" width="100%" /></td>
						</tr>

						<tr>
							<td colspan="2" align="center">
								<asp:button id="btnUpload" runat="server" text="Add" cssclass="KnectButton" oncommand="btnUpload_OnCommand" />
								<asp:button id="btnReset" runat="server" text="Reset" cssclass="KnectButton" oncommand="btnReset_OnCommand" />
							</td>
						</tr>
						
						
					</table>
			</div>


    </form>
</body>
</html>
