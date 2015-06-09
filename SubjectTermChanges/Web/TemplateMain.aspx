<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateMain.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateMain"
    Title="Untitled Page" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
    <link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
    <link rel="stylesheet" type="text/css" href="itat.css" />

    <script type="text/javascript">
        function confirmPromote(retroMessage) {

            if (retroMessage)
                return confirm('Are you sure you want to promote this template?    This will make this version of the template "live".  Also, THIS WILL TRIGGER a Retro Action during the next batch job execution.');
            else
                return confirm('Are you sure you want to promote this template?    This will make this version of the template "live".');
        }

        function confirmDemote() {
            return confirm('Are you sure you want to demote this template?    You will lose all of the changes made to this template since the last time it was promoted.');
        }

        function confirmSecurityModelChange(chk)
        {
        	var msg;
        	if (chk.checked)
        		msg = "Warning:\n\nYou are about to change the Security settings of this template from Basic to Advanced.\n\nClick OK to continue.";
        	else
        		msg = "Warning:\n\nYou are about to change the Security settings of this template from Advanced back to Basic.  This action will clear ALL security settings for this template.  Admin MUST set Reviewer and Editor permissions within each state.\n\nClick OK to continue.";
        	return confirm(msg);
        }
    </script>

</head>
<body id="htmlBody" class="NoScroll" runat="server">
    <form id="form1" runat="server">
    <kh:TemplateHeader ID="header" runat="server" OnHeaderEvent="OnHeaderEvent" />
    <div class="TemplateEditBody" id="divMain">
        <asp:Panel CssClass="TemplateEditBody AutoScroll" ID="pnlForm" runat="server">
            <table id="tblMain" style="width: 100%" cellspacing="0" cellpadding="2">
                <tr>
                    <td style="width: 60%">
                        <table id="tblTopLeft" style="width: 100%" cellspacing="0" cellpadding="2">
                            <colgroup>
                                <col width="100px" />
                                <col width="80%" />
                            </colgroup>
                            <tr>
                                <td class="DataEntryCaption">
                                    Name:
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:TextBox ID="txtName" runat="server" Width="400px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="DataEntryCaption">
                                    Description:
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:TextBox ID="txtDescription" runat="server" Width="400px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="DataEntryCaption">
                                    Status:
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:DropDownList ID="ddlStatus" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:CheckBox ID="chkGenerateDocuments" runat="server" Text="Create System-Generated Documents"
                                        TextAlign="Right" CssClass="DataEntryCaption" OnCheckedChanged="chkGenerateDocumentsOnCheckedChanged"
                                        AutoPostBack="true" /><br />
                                    <kh:RoleSelector ID="rsPrintRolesSystemDocs" runat="server" Rows="6" RoleType="Security" Widths="80,300"
                                        Caption="Print Roles" Visible="false" />
                                    <asp:CheckBox ID="chkGenerateUserDocuments" runat="server" Text="Create Non-System-Generated Documents"
                                        TextAlign="Right" CssClass="DataEntryCaption" OnCheckedChanged="chkGenerateUserDocumentsOnCheckedChanged" AutoPostBack="true"/><br />
                                    <kh:RoleSelector ID="rsPrintRolesUserDocs" runat="server" Rows="6" RoleType="Security" Widths="80,300"
                                        Caption="Print Roles" Visible="false" />

                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:CheckBox ID="chkAllowAttachments" runat="server" Text="Allow Attachments" TextAlign="Right"
                                        CssClass="DataEntryCaption" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:CheckBox ID="chkAdvancedSecurity" runat="server" Text="Advanced Security" TextAlign="Right" onclick="return confirmSecurityModelChange(this);" 
                                        CssClass="DataEntryCaption" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:CheckBox ID="chkDetailedDescriptions" runat="server" Text="Detail Template Description"
                                        TextAlign="Right" CssClass="DataEntryCaption" AutoPostBack="true" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                    <asp:Panel ID="pnlRetro" runat="server">
                        <div id="divTopRight" style="margin: 0 0 2px 0; border: 1px solid #666;">
                            <table id="tblTopRight" style="width: 90%" cellspacing="0" cellpadding="2">
                                <tr>
                                    <td class="DataEntryCaption AlignLeft">
                                        <asp:CheckBox ID="chkRetro" runat="server" Text="Retro ON" TextAlign="Right" CssClass="DataEntryCaption"
                                            AutoPostBack="true" oncheckedchanged="chkRetro_CheckedChanged"/>
                                    </td>
                                    <td class="DataEntryCaption AlignLeft">
                                        <asp:DropDownList runat="server" ID="ddlRetroOptions" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table id="tblBottom" style="width: 100%" cellspacing="0" cellpadding="2">
                            <colgroup>
                                <col width="100px" />
                                <col width="80%" />
                            </colgroup>
                            <tr id="rowDetailedDescription1" runat="server" visible="false">
                                <td class="DataEntryCaption">
                                    When to use this template?
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:TextBox ID="txtDetailedDescription1" TextMode="MultiLine" Rows="3" runat="server"
                                        Width="100%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="rowDetailedDescription2" runat="server" visible="false">
                                <td class="DataEntryCaption">
                                    When not to use this template?
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:TextBox ID="txtDetailedDescription2" TextMode="MultiLine" Rows="3" runat="server"
                                        Width="100%"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="rowDetailedDescription3" runat="server" visible="false">
                                <td class="DataEntryCaption">
                                    Additional Template Information:
                                </td>
                                <td class="DataEntryCaption AlignLeft" colspan="1">
                                    <asp:TextBox ID="txtDetailedDescription3" TextMode="MultiLine" Rows="3" runat="server"
                                        Width="100%"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <div runat="server" id="divPromoteButtons" class="ProfileCaption" style="text-align: center;
        border: solid #333; border-width: 0 1px 1px 1px; padding: 3px 3px 3px 3px">
        <asp:Button ID="btnPromote" runat="server" Text="Promote" CssClass="KNectButton"
            OnCommand="btnPromote_Command" />
        <asp:Button ID="btnDemote" runat="server" Text="Demote" CssClass="KNectButton" OnClientClick="return confirmDemote()"
            OnCommand="btnDemote_Command" />
    </div>
    </form>
</body>
</html>
