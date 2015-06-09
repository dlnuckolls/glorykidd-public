<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExternalTermControl.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.ExternalTermControl" %>

<asp:panel id="pnlContainer" runat="server" width="100%">

	<asp:table id="tblMultiValued" cellpadding="0" cellspacing="0" runat="server" style="table-layout:fixed;">
		<asp:tablerow>

			<asp:tablecell width="100%"  runat="server">
			<%-- 
				<asp:panel id="pnlSingleValueDropdown" runat="server">
					<asp:dropdownlist id="ddlValue" runat="server" width="100%" />
				</asp:panel>
			--%>
				<asp:panel id="pnlReadOnlyText" runat="server">
					<asp:label id="lblText" runat="server" text="Sample Text" />
				</asp:panel>
				
				<asp:panel id="pnlGridHeader" runat="server" />
				<asp:panel id="pnlGrid" runat="server">
					<kh:multiselectgrid id="grd" runat="server" />
				</asp:panel>
			</asp:tablecell>

			<asp:tablecell width="106px" runat="server">
				<asp:panel id="pnlButtons" runat="server">
					<asp:button id="btnAdd" runat="server"  text="Add" cssclass="KnectButton" style="margin:3px;" usesubmitbehavior="false"  /><br />
					<asp:panel id="pnlRemoveButton" runat="server"><asp:button id="btnRemove" runat="server" text="Remove" cssclass="KnectButton" style="margin:3px;" enabled="false" oncommand="btnRemoveOnCommand"  /><br /></asp:panel>
					<asp:button id="btnClear" runat="server" text="Clear" cssclass="KnectButton" style="margin:3px;" oncommand="btnClearOnCommand"  />
				</asp:panel>
			</asp:tablecell>

		</asp:tablerow>

	</asp:table>			
			
	
			

</asp:panel>
