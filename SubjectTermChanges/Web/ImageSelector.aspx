<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImageSelector.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ImageSelector" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >

<head id="Head1" runat="server">
	<title>Image Selector</title>
	<base target='_self' />
	<link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<script type="text/javascript" src="Scripts/itat.js"></script>
</head>

<body id="body" runat="server">
 
		<form id="form1" method="post" runat="server" enctype="multipart/form-data">
			
			<kh:fileupload id="imageUpload" runat="server" buttontext="Add" onupload="imageUploadOnUpload" caption="Select Image" style="width:100%;" />
			
			<div>
				Double-click an image to select it:
				<asp:datalist id="imageList" runat="server" width="100%"
						repeatdirection="Horizontal" 
						repeatcolumns="4"
						repeatlayout="Table"
						onitemdatabound="imageListItemDataBound"
						onitemcommand="imageListItemCommand"
						>
					<itemtemplate>
						<div style="text-align:center; vertical-align:top; padding:3px;">
							<table cellspacing="5" cellpadding="0" border="0" style="background-color:#e5e0e0;">
								<tr>
									<td><asp:image id="itemImage" runat="server" /></td>
								</tr>
								<tr>
									<td><asp:label id="itemName" runat="server" text="Image Name" /></td>
								</tr>
								<tr>
									<td><asp:linkbutton id="itemDelete" runat="server" text="Delete" /></td>
								</tr>
							</table>
						</div>
					</itemtemplate>
				</asp:datalist>
			</div>
		
		</form>

</body>
</html>
