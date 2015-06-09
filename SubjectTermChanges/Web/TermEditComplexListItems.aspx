<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true" CodeBehind="TermEditComplexListItems.aspx.cs"
    Inherits="Kindred.Knect.ITAT.Web.TermEditComplexListItems" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
    <link rel="stylesheet" type="text/css" href="itat.css" />
    <base target="_self" />
</head>
<body id="htmlBody" runat="server" class="NoScroll ">
    <form id="form1" runat="server">
    <kh:templateheader id="header" runat="server" HideButtons="true"  />
    <div class="TemplateEditBody" id="divListContainer" >
            <div class="GridTitle">
            <span style="float: left; text-align: left;">
                <label id="complexlistfields" runat="server">
                </label>
            </span><span style="float: right; text-align: right; padding: 6px 0 0 0;">
                <asp:Button ID="btnAddList" runat="server" CssClass="KnectButton" Text="Add" OnCommand="btnAdd_Click"
                    CommandName="Add" CommandArgument="ComplexList" />
            </span><span style="float: right; margin: 0 4px 0 0;">
                <kh:gridmoverows runat="server" id="ucGridMoveRows" targetcontrol="grdList" />
            </span>
        </div>
        <div class="TermEditBody" style="border:0;height:46px">
        <asp:Panel ID="pnlListGridHeader" runat="server" Visible="false" />
        <asp:Panel ID="pnlListGridBody" runat="server" CssClass="GridBodyPanel" style="height:auto" >
            <kh:kindredgridview id="grdList" runat="server" cssclass="NetTable" onrowcreated="grdList_RowCreated"
                onrowcommand="grd_RowCommand" autogeneratecolumns="False" datakeynames="Index"
                rowhighlighting="True" confirmondelete="False" enabledoubleclickevent="false"
                enableheaderclick="false" enableclickevent="false" headercontainer="pnlListGridHeader"
                headerrowsize="1" container="pnlListGridBody" sortascending="True" sortcolumn="-1">
					<columns>
                    </columns>
				</kh:kindredgridview>
        
        </asp:Panel>
    </div>
      <asp:Panel ID="pnlButtons" runat="server" CssClass="DataEntryCaption AlignCenter"
            style="text-align: center;
        border: solid #333; border-width: 1px 1px 1px 1px; padding: 3px 3px 3px 3px">
            <asp:Button ID="btnOK" runat="server" CssClass="KnectButton" Text="OK"  OnClick="btnOK_Click"/>
            <asp:Button ID="btnCancel" runat="server" CssClass="KnectButton" Text="Cancel" OnClick="btnCancel_Click" />
        </asp:Panel>
        
    </div>

    </form>
</body>
</html>
