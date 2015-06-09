<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.FileUpload" %>
<div id="container" runat="server" style="text-align:right; position:relative; margin: 5px 0 0 0; width:100%;" nowrap="nowrap">
		<input type="file" id="inputFile" name="inputFile" runat="server">
		<div id="divFake" runat="server" style="z-index: 1;position:absolute; top:0px; right:0px;">
			<asp:label runat="server" id="lblCaption" />&nbsp;<asp:textbox id="txtFake" runat="server" width="300px" readonly="true" /><asp:button id="btnFake" runat="server" usesubmitbehavior="false" text="Browse..." cssclass="KnectButton" forecolor="#000000" width="66px" onclientclick="return false;" />
			<asp:button id="btnUpload" runat="server" text="Add" cssclass="KnectButton"   />
		</div>
</div>
