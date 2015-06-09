<%@ Page Language="C#" ValidateRequest="false" AutoEventWireup="true" CodeBehind="TermEditComplexList.aspx.cs"
    Inherits="Kindred.Knect.ITAT.Web.TermEditComplexList" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
    <link rel="stylesheet" type="text/css" href="itat.css" />
    <base target="_self" />
</head>
<body id="body" runat="server" class="NoScroll">
    <form id="form1" runat="server">
    <kh:standardheader id="header" runat="server" pagetitle="Edit Complex List" />
    <asp:Panel CssClass="TemplateEditBody" ID="editBody" runat="server">
        <table border="0" cellpadding="2" cellspacing="0" style="table-layout: fixed;">
            <colgroup>
                <col width="150px" />
                <col width="170px" />
                <col width="100%" />
            </colgroup>
            <tr>
                <td class="DataEntryCaption">
                    Complex List Name:
                </td>
                <td colspan="2" class="DataEntryCaption AlignLeft">
                    <asp:TextBox ID="txtTermName" runat="server" Width="300px" />
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <hr style="margin: 0; padding: 0; height: 2px; color: #333" />
                </td>
            </tr>
            <tr>
                <td class="DataEntryCaption" style="width: 300px">
                    Show on Item Summary:
                </td>
                <td colspan="2" class="DataEntryCaption AlignLeft">
                    <asp:CheckBox ID="chkbxShowOnItemSummary" runat="server" CssClass="CheckBoxAlignLeft" />
                </td>
            </tr>
            <tr>
                <td class="DataEntryCaption" style="width: 300px">
                    Keyword Searchable:
                </td>
                <td colspan="2" class="DataEntryCaption AlignLeft">
                    <asp:CheckBox ID="chkbxKeywordSearchable" runat="server" CssClass="CheckBoxAlignLeft" />
                </td>
            </tr>
            <tr>
            <td colspan="3">
            <div>
                <div class="GridTitle">
                    <span style="float: left; text-align: left;">Fields</span> <span style="float: right;
                        text-align: right; padding: 6px 0 0 0;">
                        <asp:Button ID="btnAddList" runat="server" CssClass="KnectButton" Text="Add" OnCommand="btnAdd_Command"
                            CommandName="Add" CommandArgument="Field" />
                    </span>
				<span style="float:right;  margin: 0 4px 0 0;">
						    <asp:imagebutton id="btnTermGroupMoveUp" runat="server" width="11px" height="11px" alternatetext="Move Row Up" oncommand="btnSwitchTermGroupRows_Command" /><br />
						    <asp:imagebutton id="btnTermGroupMoveDown" runat="server" width="11px" height="11px" alternatetext="Move Row Down" oncommand="btnSwitchTermGroupRows_Command"  />
				    </span>
                </div>
                <asp:Panel ID="pnlListGridHeader" runat="server" Visible="false" CssClass="header" style="height:10px"/>
                <asp:Panel ID="pnlListGridBody" runat="server" CssClass="GridBodyPanel" Style="height: 100px;">
                    <kh:kindredgridview id="grdFieldList" 
                        runat="server" 
                        cssclass="NetTable" 
                        onrowcreated="grdFieldList_RowCreated"
                        onrowcommand="grdFieldList_RowCommand" 
                        autogeneratecolumns="False" 
                        datakeynames="ID" 
                        rowhighlighting="True"
                        confirmondelete="False" 
                        enabledoubleclickevent="true" 
                        enableheaderclick="false"
                        enableclickevent="true" 
                        headercontainer="pnlListGridHeader" 
                        headerrowsize="1"
                        container="pnlListGridBody" 
                        sortascending="True" 
                        sortcolumn="-1">
					<columns>
						<asp:boundfield datafield="Name" readonly="True" headertext="Name" >
							<headerstyle width="100%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield datafield="TermTypeName" readonly="True" headertext="Type" >
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
                </asp:Panel>
                </div>
                </td>
            </tr>
            <tr align="center">
                <td colspan="3">
                    <br />
                    <br />
                    <br />
                    <asp:Button ID="btnDefaultItems" runat="server" CssClass="KnectButton" Style="width:150px" Text="Default Items"
                        OnClick="btnDefaultItems_Click" />
                    <br />
                    <br />
                    <br />
                </td>
            </tr>
        </table>
        <div style="margin: 5px 0 0 5px; white-space: nowrap;">
            <asp:Button runat="server" CssClass="TabButtonSelected" ID="btnTabBody" Text="Body"
                OnCommand="btnTabOnCommand" CommandName="ContentBody" /><asp:Button runat="server"
                    CssClass="TabButton" ID="btnTabStandardHeader" Text="Std Header" OnCommand="btnTabOnCommand"
                    CommandName="ContentStandardHeader" /><asp:Button runat="server" CssClass="TabButton"
                        ID="btnTabAlternateHeader" Text="Alt Header" OnCommand="btnTabOnCommand" CommandName="ContentAlternateHeader" />
        </div>
        <asp:Panel ID="pnlEditorContainer" runat="server">
            <asp:Panel ID="pnlEditorBody" runat="server">
                <rade:radeditor id="edtBody" runat="server" toolsfile="~/Config/TermEditComplexListToolsFile.xml"
                    font-names="Times New Roman, Arial, Courier New" onclientload="radOnClientLoad"
                    enableviewstate="true" width="100%" focusonload="false" height="100%" enabled="true"
                    editable="true" visible="true" saveasxhtml="true" saveinfile="false" newlinebr="true"
                    enabletab="false" font-size="" converttagstolower="true" converttoxhtml="true"
                    convertfonttospan="false" enableserversiderendering="true" stripformattingonpaste="MSWordRemoveAll"
                    toolbarmode="Default" showpreviewmode="false" showsubmitcancelbuttons="false"
                    showhtmlmode="false" renderastextarea="false" suppressnotification="true" />
            </asp:Panel>
            <asp:Panel ID="pnlEditorStandardHeader" runat="server" Visible="false">
                <rade:radeditor id="edtStandardHeader" runat="server" toolsfile="~/Config/TermEditComplexListToolsFile.xml"
                    font-names="Times New Roman, Arial, Courier New" onclientload="radOnClientLoad"
                    enableviewstate="true" width="100%" focusonload="false" height="100%" enabled="true"
                    editable="true" visible="true" saveasxhtml="true" saveinfile="false" newlinebr="true"
                    enabletab="false" font-size="" converttagstolower="true" converttoxhtml="true"
                    convertfonttospan="false" enableserversiderendering="true" stripformattingonpaste="MSWordRemoveAll"
                    toolbarmode="Default" showpreviewmode="false" showsubmitcancelbuttons="false"
                    showhtmlmode="false" renderastextarea="false" />
            </asp:Panel>
            <asp:Panel ID="pnlEditorAlternateHeader" runat="server" Visible="false">
                <rade:radeditor id="edtAlternateHeader" runat="server" toolsfile="~/Config/TermEditComplexListToolsFile.xml"
                    font-names="Times New Roman, Arial, Courier New" onclientload="radOnClientLoad"
                    enableviewstate="true" width="100%" focusonload="false" height="100%" enabled="true"
                    editable="true" visible="true" saveasxhtml="true" saveinfile="false" newlinebr="true"
                    enabletab="false" font-size="" converttagstolower="true" converttoxhtml="true"
                    convertfonttospan="false" enableserversiderendering="true" stripformattingonpaste="MSWordRemoveAll"
                    toolbarmode="Default" showpreviewmode="false" showsubmitcancelbuttons="false"
                    showhtmlmode="false" renderastextarea="false" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="pnlButtons" runat="server" CssClass="DataEntryCaption AlignCenter"
            Style="padding: 8px 0 8px 0;">
            <asp:Button ID="btnOK" runat="server" CssClass="KnectButton" Text="OK" OnClick="btnOK_Click" />
            <asp:Button ID="btnCancel" runat="server" CssClass="KnectButton" Text="Cancel" OnClick="btnCancel_Click" />
        </asp:Panel>
    </asp:Panel>
    </form>
</body>
</html>
