<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermEditComplexListItem.aspx.cs"
    Inherits="Kindred.Knect.ITAT.Web.TermEditComplexListItem" %>

<%@ Register TagPrefix="ITAT" Namespace="Kindred.Knect.ITAT.Web.Controls" Assembly="Knect.ITAT.Web" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
    <link rel="stylesheet" type="text/css" href="itat.css" />
</head>
<body id="body" runat="server" class="NoScroll">
    <iframe style="display: none; position: absolute; width: 148; height: 194;" id="CalFrame"
        marginheight="0" marginwidth="0" noresize frameborder="0" scrolling="NO" src="/Global/bin/Calendar/calendar.htm"
        onblur="javascript:this.style.display='none';"></iframe>
    <script language="JavaScript" src="/Global/bin/Calendar/cal_routines.js"></script>
    <form id="form1" runat="server">
    <kh:standardheader id="header" runat="server" />
    <asp:Panel CssClass="TemplateEditBody" ID="pnlNewItem" runat="server">
        <div class="DataEntryCaption AlignLeft">
            <label id="ComplexListName" runat="server" class="GridTitle" ></label>
            </div>
        <div class="DataEntryCaption AlignLeft TemplateEditBody" style="border-right:1px solid #333;border-bottom: 1px solid #333;margin:0 0 0 0;padding:0 0 0 0">
            <table width="100%" cellpadding="0" cellspacing="0" style="border:0;margin:0 0 0 0;left:0px" border="0">
                <tr>
                    <td    class="DataEntryEdit" align="left" valign="top" style="border: 0 0 0 0; padding: 0 0 0 0;
                        margin: 0 0 0 0" width="99%">
                        <itat:itatplaceholder id="apnlCriteria" runat="server" >
                           
                        </itat:itatplaceholder>
                    </td>
                </tr>
                <tr>
                    <td class="DataEntryCaption AlignLeft"  >
                        <span style="width: 80px;">
                            <asp:CheckBox ID="chkbxSelectable" runat="server" Text="Selectable" />
                    </td>
                    
                </tr>
                <tr>
                    <td class="DataEntryCaption AlignLeft"  >
                        <span style="width: 90px;" />
                        <asp:CheckBox ID="chkbxSelected" runat="server" Text="Selected" />
                    </td>
                    
                </tr>
                <tr>
                    <td class="DataEntryCaption AlignLeft"  >
                        <span style="width: 80px;">
                            <asp:CheckBox ID="chkbxEditable" runat="server" Text="Editable" />
                    </td>
                    
                </tr>
                <tr>
                    <td class="DataEntryCaption AlignLeft"  >
                        <span style="width: 80px;">
                            <asp:CheckBox ID="chkbxDeletable" runat="server" Text="Deletable" />
                    </td>
                    
                </tr>
            </table>
        </div>
        <div class="DataEntryCaption AlignCenter" style="padding: 10px 0 5px 0;">
            <asp:Button ID="btnOK" runat="server" CssClass="KnectButton" Text="OK" OnClick="btnOK_Click" />
            <asp:Button ID="btnCancel" runat="server" CssClass="KnectButton" Text="Cancel" OnClick="btnCancel_Click" />
        </div>
    </asp:Panel>
    </form>
</body>
</html>
