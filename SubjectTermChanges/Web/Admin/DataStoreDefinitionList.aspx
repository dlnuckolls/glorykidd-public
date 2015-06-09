<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataStoreDefinitionList.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.Admin.DataStoreDefinitionList" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
      <title>Data Store Definition</title>
    <link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
    <link rel="stylesheet" type="text/css" href="../itat.css" />
    <script type="text/javascript" src="/Global/bin/Calendar/cal_routines.js"></script>
    <script src="../Scripts/jquery-1.3.2-vsdoc2.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-1.3.2.min.js" type="text/javascript"></script>
</head>
<body id="htmlBody" runat="server">
    <form id="form1" runat="server">
    <kh:standardheader id="itatHeader" pagetitle="DataStore Definitions" runat="server" />
    
    <div>
    <table width="100%" style="table-layout: fixed;">
            <tr>
                <td class="DataEntryCaption" width="120px">
                    System:
                </td>
                <td class="DataEntryEdit" width="100%">
                    <asp:DropDownList ID="ddlSystem" runat="server" Width="100%" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlSystemOnSelectedIndexChanged" />
                </td>
            </tr>
            </table>
            </div>
            <div style="text-align: right; margin: 2px 16px 2px 0;">
			&nbsp;&nbsp;
			<asp:button id="btnAdd" cssclass="KnectButton" runat="server" text="Add" onclick="btnAdd_Click" />
		</div>

            		<asp:panel id="pnlGridHeader" runat="server" visible="false" />
		<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" height="100%">
				<kh:kindredgridview id="grdResults" 
							runat="server" 
							cellpadding="0"
							cellspacing="0"	
							cssclass="NetTable"
							autogeneratecolumns="False" 
							datakeynames ="DataStoreDefinitionID" 
							rowhighlighting="false" 
							confirmondelete="false" 
							enabledoubleclickevent="false" 
							enableheaderclick="true" 
							enableclickevent="false" 
                            headercontainer="pnlGridHeader" 
							headerrowsize="1" 
							container="pnlGridBody" 
							sortascending="True" 
							sortcolumn="-1" onrowcommand="grdResults_RowCommand" 
                            OnSorting="gridView_Sorting"
                    onrowcreated="grdResults_RowCreated" onrowdatabound="grdResults_RowDataBound"

				>
					<columns>
						<asp:boundfield  datafield="Name" readonly="True" headertext="Name">
							<headerstyle width="45%" ></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield  datafield="Active" readonly="True" headertext="Status">
							<headerstyle width="10%"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>
						<asp:boundfield  datafield="LastRunDate" readonly="True" headertext="Last Run Date" dataformatstring="{0:MM/dd/yyyy hh:mm tt}" htmlencode="false">
							<headerstyle width="15%"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>
                        <asp:boundfield  datafield="LastModifiedDate" readonly="True" headertext="Last Updated Date" dataformatstring="{0:MM/dd/yyyy hh:mm tt}" htmlencode="false">
							<headerstyle width="15%"></headerstyle>
							<itemstyle horizontalalign="center"></itemstyle>
						</asp:boundfield>
                        <asp:buttonfield  buttontype="Link" text="Edit" headertext="" commandname="Edit">
						<headerstyle width="10%" />
						<itemstyle horizontalalign="center"></itemstyle>
					</asp:buttonfield>
                        
					</columns>
				</kh:kindredgridview>
			</asp:panel>
           
    
    </form>
</body>
</html>
