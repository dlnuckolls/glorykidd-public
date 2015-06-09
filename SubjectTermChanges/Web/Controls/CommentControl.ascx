<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommentControl.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.CommentControl" %>

<asp:panel id="pnlContainer" runat="server">
	<asp:textbox id="txtNewComment" runat="server" rows="6" textmode="multiLine" width="100%"  onfocus="javascript:this.select();" />
	<asp:datalist id="lstComments" runat="server" onitemdatabound="lstComments_ItemDataBound"	>
		<itemtemplate>
			<div id="divCommentHeader" class="ProfileCommentHeader"><asp:literal id="litCommentHeader" runat="server" /></div>
			<div id="divCommentBody" class="ProfileCommentBody"><asp:literal id="litCommentBody" runat="server" /></div>
		</itemtemplate>
	</asp:datalist>
</asp:panel>

<%--  ontextchanged="txtNewComment_TextChanged"  --%>