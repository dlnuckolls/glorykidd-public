<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentControl.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.AttachmentControl" %>
<%@ register tagprefix="kh" tagname="fileupload2" src="~/Controls/FileUpload.ascx" %>
<%--
		NOTE:  The line above actually RE-REGISTERS the FileUpload.ascx contol although it is registered in web.config.  
		This is due to a quirk in ASP.NET that will not allow us to use a control registered in the web.config within another control 
		that is in the same directory.   		But we CAN register and use such a control locally (using a different tagname).
--%>

<asp:panel id="pnlContainer" runat="server">
	<asp:datalist id="lstAttachments" runat="server" onitemdatabound="lstAttachments_ItemDataBound"	>
		<headertemplate>
			<table cellpadding="0" cellspacing="0" border="0" class="NoBorder" style="table-layout:fixed;" width="100%">
		</headertemplate>
		<itemtemplate>
			<tr id="rowLink" runat="server" >
				<td class="ProfileEditReadOnly" style="border-width:0; width:100%;">
					<asp:hyperlink runat="server" id="linkOpenDocument" />
				</td>
				<td class="ProfileEditReadOnly" align="right" style="border-width:0;" >
					<asp:dropdownlist id="ddlDocumentType" runat="server" onselectedindexchanged="ddlDocumentType_SelectedIndexChanged" />&nbsp;&nbsp;
					<asp:linkbutton id="lnkRemoveAttachment" runat="server" text="Remove" />
				</td>
			</tr>	
		</itemtemplate>
		<footertemplate>
		</table>
		</footertemplate>
	</asp:datalist>
	<hr runat="server" id="separator" size="1" color="#999999" width="95%" align="center" style="margin:0; padding:0;" />	
	<kh:fileupload2 id="filNewAttachment" runat="server" buttontext="Add" caption="New Attachment:" onupload="filNewAttachment_OnUpload" />
	
</asp:panel>


