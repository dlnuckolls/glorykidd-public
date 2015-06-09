<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermEditText.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermEditText" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
 
</head>
<body id="body" runat="server" class="NoScroll">
<form id="form1" runat="server">
		<kh:standardheader id="header" runat="server"  pagetitle="Edit Term" />
        &nbsp;

		<asp:panel cssclass="TemplateEditBody AutoScroll" id="editBody" runat="server">

			<table border="0" cellpadding="2" cellspacing="0" style="table-layout:fixed;">
				<colgroup>
					<col width="200px" />
					<col width="100%" />
				</colgroup>	

				<tr>
					<td class="DataEntryCaption">Term Name:</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtTermName" runat="server" width="300px"  /></td>
				</tr>

				<tr>
					<td  class="DataEntryCaption">Term Type:</td>
					<td  class="DataEntryCaption AlignLeft">Text</td>
				</tr>

				<tr>
					<td colspan="2"><hr style="margin:0; padding:0; height:2px; color:#333" /></td>
				</tr>
                
				<kh:VOS runat="server" ID="vos"  />
        
				<tr id="trTermGroup" runat="server" visible="false">
					<td class="DataEntryCaption">Tab:</td>
					<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlTermGroup" runat="server" /></td>
				</tr>
				
				<tr id="trHeaderTerm" runat="server" visible="false">
					<td class="DataEntryCaption">Header Term:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkHeaderTerm" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlignLeft"   text=""  /></td>
				</tr>

				<tr id="trBigText" runat="server" visible="false">
					<td class="DataEntryCaption">Big Text:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkBigText" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlignLeft"   text=""  /></td>
				</tr>

				<tr id="trShowSummary" runat="server" visible="false">
					<td class="DataEntryCaption">Show Summary:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkShowSummary" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlignLeft"   text=""  /></td>
				</tr>

                <tr id="trTermMapping" runat="server" visible="true">
					<td class="DataEntryCaption">System Term Mapping:</td>
					<td class="DataEntryCaption AlignLeft">
                    <asp:dropdownlist id="ddlMappedTerm" runat="server" style="margin-left:4px;" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:button id="btnMap" runat="server" cssclass="KnectButton" text="Map" onclick="btnMap_Click" />
                    </td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Required:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkRequired" runat="server" textalign="Right" cssclass="CheckBoxAlignLeft" text=""  /></td>
				</tr>

			  <tr id="trKeywordSearchable" runat="server" visible="true">
					<td class="DataEntryCaption">Keyword Searchable:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkKeywordSearchable" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft" /></td>
			  </tr>

				<tr>
					<td class="DataEntryCaption">Default:</td>
					<td class="DataEntryCaption AlignLeft">
						<asp:checkbox id="chkDefault" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft" autopostback="true" oncheckedchanged="chkDefault_OnCheckedChanged" />&nbsp;&nbsp;
						<asp:textbox id="txtDefaultValue" runat="server" text="Default Value" width="300px" />
					</td>
				</tr>

				<tr id="trEditable" runat="server" visible="false">
					<td class="DataEntryCaption">Editable:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkEditable" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft" /></td>
				</tr>

				<tr id="trPreserveSummaryLineBreaks" runat="server" visible="false">
					<td class="DataEntryCaption">Preserve Summary Line Breaks:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkPreserveSummaryWhiteSpace" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft" /></td>
				</tr>

				<tr id="trPreserveDocumentLineBreaks" runat="server" visible="false">
					<td class="DataEntryCaption">Preserve Document Line Breaks:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkPreserveDocumentWhiteSpace" runat="server" text="" textalign="Right" cssclass="CheckBoxAlignLeft" /></td>
				</tr>
				
				<tr>
					<td class="DataEntryCaption AlignTop" style="padding-top:5px;">Format:</td>
					<td class="DataEntryCaption AlignLeft AlignTop">
						<asp:dropdownlist id="ddlFormat" runat="server" autopostback="true" onselectedindexchanged="ddlFormat_SelectedIndexChanged" style="margin-left:4px;"  />
						<div id="divFormatSpecifiers" runat="server" style="display:block; width: 190px; height: 70px; margin-top:5px;">
							<span style="width:90px; text-align:right; padding-right:5px;"><asp:label id="lblLowerLimit" runat="server" text="Min Length:" /></span><asp:textbox id="txtLowerLimit" runat="server" width="90px" /><br />
							<span style="width:90px; text-align:right; padding-right:5px;"><asp:label id="lblUpperLimit" runat="server" text="Max Length:" /></span><asp:textbox id="txtUpperLimit" runat="server" width="90px" /><br />
							<span style="width:90px; text-align:right; padding-right:5px;"><asp:label id="lblShowCents" runat="server" text="Show Cents:" /></span><asp:checkbox id="chkShowCents" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlignLeft" />
						</div>	
					</td>
				</tr>

				<tr id="trTextNumberFormat" runat="server">
					<td class="DataEntryCaption">Number (##) Format:</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkTextNumberFormat" runat="server" text="" textalign="Right" cssclass="DataEntryCaption" /></td>
				</tr>

				<tr>
					<td>&nbsp;</td>
					<td class="DataEntryCaption AlignLeft" style="padding: 15px 0 0 0; white-space:nowrap;">
						<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" onclick="btnOK_Click" />
						<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" onclick="btnCancel_Click" />&nbsp;
					</td>
				</tr>

			</table>
 
                 
		</asp:panel>

	</form>
     
</body>
</html>
