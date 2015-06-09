<%@ control language="C#" autoeventwireup="true" codebehind="ValidateOnSave.ascx.cs" inherits="Kindred.Knect.ITAT.Web.ValidateOnSave" %>
<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
<link rel="stylesheet" type="text/css" href="itat.css" />
<tr>
	<td class="DataEntryCaption">Validate on Save:</td>
	<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkValidateOnSave" runat="server" textalign="right" cssclass="CheckBoxAlignLeft" text="" causesvalidation="True" /></td>
</tr>
<tr runat="server" id="trValidateOnStatus" style="display:block;">
	<td class="DataEntryCaption">&nbsp;</td>
	<td class="DataEntryCaption AlignLeft" style="margin:0; padding:0 0 10px 0;">
		<fieldset id="statuses" runat="server" style="width: 100%;"  class="DataEntryCaption AlignLeft">
			<legend>Validate on Status:</legend>
			<asp:checkboxlist id="chkListValidateStatuses" cssclass="DataEntryCaption AlignLeft" repeatlayout="Table" repeatcolumns="3" repeatdirection="Horizontal" runat="server" />
		</fieldset>
	</td>
</tr>
<asp:customvalidator id="valSelectStatus" runat="server" onservervalidate="SelectionIsValid" errormessage="You must select at least one status for validate on save!" enabled="True" visible="True" display="None"></asp:customvalidator>