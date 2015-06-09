<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditDataStoreDefinition.aspx.cs"
    Inherits="Kindred.Knect.ITAT.Web.Admin.EditDataStoreDefinition" %>
    <%@ Register TagPrefix="ITAT" NameSpace="Kindred.Knect.ITAT.Web.Controls" Assembly="Knect.ITAT.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Data Store Definition</title>
    <link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
    <link rel="stylesheet" type="text/css" href="../itat.css" />
    <script type="text/javascript" src="/Global/bin/Calendar/cal_routines.js"></script>
    <script src="../Scripts/jquery-1.3.2-vsdoc2.js" type="text/javascript"></script>
    <script src="../Scripts/jquery-1.3.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/MaskedInput.js" type="text/javascript"></script>
</head>
<script language="javascript" type="text/javascript">

    var pressed = false;
    var startindex = -1;
    var stopindex = -1;
    var checkother = false;
    var checkotherdone = true;
    $(document).keydown(function (e) {
        if (e.keyCode == 17)
            pressed = true;
    });

    $(document).keyup(function (event) {
        pressed = false;
    });
    $(document).ready( //DOCUMENT READY START
                function () {

                    function getindexes(id) {

                        if (startindex < 0) {
                            //get the start index
                            var idofCheckbox = id.toString();
                            idofCheckbox = idofCheckbox.substring(idofCheckbox.indexOf("_") + 1);
                            idofCheckbox = idofCheckbox.substring(0, idofCheckbox.indexOf("_"));
                            idofCheckbox = idofCheckbox.substring(4);
                            startindex = parseInt(idofCheckbox);
                            pressed = false;
                        }
                        else if (stopindex < 0) {
                            var idofCheckbox = id.toString();
                            idofCheckbox = idofCheckbox.substring(idofCheckbox.indexOf("_") + 1);
                            idofCheckbox = idofCheckbox.substring(0, idofCheckbox.indexOf("_"));
                            idofCheckbox = idofCheckbox.substring(4);
                            stopindex = parseInt(idofCheckbox);
                            checkother = true;
                            checkotherdone = false;
                            pressed = false;
                        }
                    }

                    function checkfillers(checked) {
                        checkotherdone = true;
                        var total;
                        var index;
                        if (stopindex > startindex) {
                            index = startindex;
                            total = stopindex - startindex;
                        }
                        else {
                            index = stopindex;
                            total = startindex - stopindex;
                        }
                        for (i = 0; i < total; i++) {
                            // alert('inside');
                            var id = index + i;
                            $("#" + "lvSelectedTerms_ctrl" + id + "_chkTerm").attr("checked", checked);
                            if (checked)
                                $("#" + "lvSelectedTerms_ctrl" + id + "_chkTerm").parent().parent().parent().addClass("highlight");
                            else
                                $("#" + "lvSelectedTerms_ctrl" + id + "_chkTerm").parent().parent().parent().removeClass("highlight");

                        }
                        startindex = -1;
                        stopindex = -1;

                    }

                    var scrollTo = "";
                    function validateCheck() {
                        if ($(this).attr("checked") == true) {
                            if (pressed == true) {
                                getindexes($(this).attr("id"));


                            }
                            if ($(this).parent().hasClass('checkedbox')) {
                                $(this).parent().parent().parent().addClass("highlight");

                                if (checkother && !checkotherdone) {

                                    checkfillers(true);
                                }
                            }
                        }
                        else {
                            if (pressed == true) {
                                getindexes($(this).attr("id"));
                            }
                            if (checkother && !checkotherdone) {

                                checkfillers(false);
                            }

                            $(this).parent().parent().parent().removeClass("highlight");
                        }
                    }




                    $(":checkbox").click(validateCheck);

                    $("input[type=checkbox][checked]").each(
                                    function () {
                                        if ($(this).parent().hasClass('checkedbox'))
                                            $(this).parent().parent().parent().addClass("highlight");

                                        if (scrollTo == "") {
                                            scrollTo = $(this);
                                        }
                                    });

                    var container = $(".listViewTable");
                    container.scrollTop(scrollTo.offset().top - container.offset().top - 40);

                    $("#checkAll").click(
                                            function () {
                                                var checked_status = this.checked;
                                                $("input[type=checkbox]").each(function () {
                                                    if ($(this).parent().hasClass('checkedbox')) {
                                                        this.checked = checked_status;
                                                        if ($(this).attr("checked") == true) {
                                                            if ($(this).parent().hasClass('checkedbox'))
                                                                $(this).parent().parent().parent().addClass("highlight");
                                                        }
                                                        else {
                                                            $(this).parent().parent().parent().removeClass("highlight");
                                                        }
                                                    }

                                                });
                                            });


                });                                         //DOCUMENT READ ENDS



    $(document).ready(function ($) {
        function EnableDateFormatList(i) {
            var eleid = $(this).attr('id').toString();
            if ($("#" + eleid + " option:selected").val() == "DateTime")
                $("#" + eleid.replace('ddlTermType', 'ddlDateFormat')).attr('disabled', '');
            else
                $("#" + eleid.replace('ddlTermType', 'ddlDateFormat')).attr('disabled', 'disabled');
        }
        $(".termtypelist").each(EnableDateFormatList);
        function EnableDateFormatList() {
            var eleid = $(this).attr('id').toString();
            if ($("#" + eleid + " option:selected").val() == "DateTime")
                $("#" + eleid.replace('ddlTermType', 'ddlDateFormat')).attr('disabled', '');
            else
                $("#" + eleid.replace('ddlTermType', 'ddlDateFormat')).attr('disabled', 'disabled');
        }
        $(".termtypelist").change(EnableDateFormatList);
    });
    
</script>
<body id="htmlBody" runat="server">
    <iframe style="display: none; position: absolute; width: 148; height: 300; z-index: 3000;"
        id="CalFrame" marginheight="0" marginwidth="0" frameborder="0" scrolling="NO"
        src="/Global/bin/Calendar/calendar.htm" onblur="javascript:this.style.display='none';">
    </iframe>
    <form id="form1" runat="server">
    <asp:ValidationSummary ID="msgValidationSummar" runat="server" ShowMessageBox="true"  DisplayMode="List" ShowSummary="false" ValidationGroup="RequiredValidation" />
    <kh:standardheader id="itatHeader" pagetitle="Edit DataStore Definition" runat="server" />
    <div>
        <table id="tblDefinition" runat="server" width="100%" style="table-layout: fixed;" cellspacing="0">
            <tr>
                <td class="ProfileCaption" width="114px">
                    System
                </td>
                <td class="DataEntryEdit" width="100%">
                     <asp:Label ID="lblSystemName" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ProfileCaption" width="114px">
                  Definition Name
                </td>
                <td class="DataEntryEdit" width="100%">
                    <asp:Label ID="lblDefinitionName" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ProfileCaption" width="114px">
                    Description
                </td>
                <td class="DataEntryEdit" width="100%">
                    <asp:TextBox ID="txtDescription" runat="server" Width="100%" EnableViewState="true"></asp:TextBox>
                </td>
            </tr>
            
              <tr>
                <td class="ProfileCaption" width="114px">
                  <asp:CustomValidator ID="reqTemplate" runat="server" Text="*"  Display="Dynamic" OnServerValidate="CheckAtLeastOneTemplateIsSelected" ValidationGroup="RequiredValidation" ErrorMessage="Select atleast 1 Template."></asp:CustomValidator>  Template:
                </td>
                <td class="DataEntryEdit" width="100%">
                    <div style="width: 35%; float: left">
                        <asp:ListBox ID="lstTemplates" runat="server" SelectionMode="Multiple" Rows="6" Width="100%" EnableViewState="true"/>
                    </div>
                    <div style="width: 9%; float: left; vertical-align: middle; text-align: center;">
                        <br />
                        <asp:Button ID="btnAddTemplate" runat="server" Style="text-align: center"
                            Text="&gt;&gt;" OnClick="btnAddTemplate_Click" Width="30px"></asp:Button>
                        <br /><br />
                        <asp:Button ID="btnRemoveTemplate" runat="server" Style="text-align: center"
                            Text="&lt;&lt;" OnClick="btnRemoveTemplate_Click" Width="30px"></asp:Button>
                    </div>
                    <div style="width: 55%">
                        <asp:ListBox ID="lstSelectedTemplates" runat="server" SelectionMode="Multiple" Rows="6"
                            Width="100%" />
                    </div>
                </td>
            </tr>   

            <tr>
                <td class="ProfileCaption" width="114px">
                    Terms
                </td>
                <td class="DataEntryEdit" width="99%">
                    <div style="width: 100%; float: left">
                        <asp:ListBox ID="lstTerms" runat="server" SelectionMode="Multiple" Rows="36" Width="100%"
                             Height="170px" />
                    </div>
                        <asp:Button ID="btnAddTerm" runat="server" Style="text-align: center"
                            Text="Add &gt;&gt;" OnClick="btnAddTerm_Click" Width="100%" />
                       
                   
                 
                </td>
            </tr>
             <tr>
                <td class="ProfileCaption" width="114px">
                    <asp:CustomValidator ID="reqSelectedTerms" runat="server" Text="*"  Display="Dynamic"  OnServerValidate="CheckAtLeastOneTermIsSelected" ValidationGroup="RequiredValidation" ErrorMessage="Select atleast 1 Term."></asp:CustomValidator>  Selected Terms
                </td>
                <td class="DataEntryEdit" width="99%">
                
                  <div style="border-style: none; border-color: inherit; border-width: 0; width: 95%;
                        height: 180px; float: left;">
                        <table style="border-style: none; border-color: inherit; border-width: 0; width: 99%;">
                            <tr style="height: 10px;">
                                <td style="width: 60%; text-align: center" class="ProfileCaption">
                                <div style="text-align:left;float:left;border:0;margin:0;height:10px;width:10px"><input id="checkAll" class="SelectAll" type="checkbox"  style="border:0;margin:0;top:0;height:15px;width:15px"/></div>
                                    Term
                                </td>
                                <td style="width: 5%; text-align: center" class="ProfileCaption">
                                    Type
                                </td>
                                <td style="width: 15%; text-align: center" class="ProfileCaption">
                                    Alias
                                </td>
                                
                                <td style="width: 5%; text-align: center" class="ProfileCaption">
                                    Size 
                                </td>
                            </tr>
                        </table>
                        <div class="listViewTable" style="width: 99%">
                            <asp:ListView ID="lvSelectedTerms" runat="server" SelectionMode="Multiple" 
                                EnableViewState="true" onitemdatabound="lvSelectedTerms_ItemDataBound"  >
                                <LayoutTemplate>
                                    <table cellpadding="0" cellspacing="0" width="100%">
                                        <tr id="itemPlaceholder" runat="server" style="width: 100%">
                                        </tr>
                                    </table>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <tr style="width: 98%">
                                        <td style="width: 59%">
                                              <asp:CheckBox runat="server" class="checkedbox" myattr="checkedbox" ID="chkTerm"
                                                Text='<%#Eval("Name") %>' checked='<%#Eval("IsChecked")%>' Width="100%" Style="display: block"></asp:CheckBox>    </td>
                                        <td style="width: 5%" align="center">
                                            <asp:Label ID="lblType" runat="server" Visible="true"  Text='<%#Eval("TermType")%>'> </asp:Label>
                                            
                                        </td>
                                        <td style="width: 14%; text-align: left">
                                            <asp:TextBox runat="server" ID="txtAlias" class="aliasbox" Text='<%#  Eval("Alias") %>'
                                                Width="140px" MaxLength="90"></asp:TextBox>
                                        </td>
                                        <td style="width: 6%; text-align: left">
                                            <asp:TextBox runat="server" Class="size" ID="txtSize" Visible='<%#  (bool)Eval("RequiresTextLength") %>' Text='<%#Eval("Length") %>' Width="40px"></asp:TextBox>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <SelectedItemTemplate>
                                    <tr id="Tr1" runat="server" style="background-color: #90EE90">
                                        <td style="width: 50%">
                                            <asp:CheckBox runat="server" ID="lblId" class="checkedbox" Text='<%#Eval("Name") %>'
                                                CssClass="hideCheck"></asp:CheckBox>
                                        </td>
                                        <td style="width: 50%">
                                            <asp:TextBox runat="server" ID="lblName"  Text='<%#Eval("Name") %>' Width="95%"></asp:TextBox>
                                        </td>
                                    </tr>
                                </SelectedItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                     <div style="width: 3%; float: left; vertical-align: middle; text-align: center; margin: 0;
                        height: 200px;">
                        <br />
                        <br />
                        <asp:Button ID="btnFirst" runat="server" Style="width: 40px; text-align: center" Text="First"
                            OnClick="btnFirst_Click" />
                        <br />
                        <asp:Button ID="btnUp" runat="server" Style="width: 40px; text-align: center" Text="Up"
                            OnClick="btnUp_Click" />
                        <br />
                        <asp:Button ID="btnDown" runat="server" Style="width: 40px; text-align: center" Text="Down"
                            OnClick="btnDown_Click" />
                        <br/>
                        <asp:Button ID="btnLast" runat="server" Style="width: 40px; text-align: center" Text="Last"
                            OnClick="btnLast_Click" />
                            <br/>
                            <br/>

                    </div>
                     <asp:Button ID="btnRemoveTerm" runat="server" Style="text-align: center"
                            Text="&lt;&lt; Remove" OnClick="btnRemoveTerm_Click" Width="100%"></asp:Button>
                </td>
            </tr>
            
            <tr >
            <td colspan ="2" align="left" valign="top" style="border:0 0 0 0;padding:0 0 0 0">
                   <ITAT:ITATPlaceHolder ID="apnlCriteria" runat="server" Width="100%" style="padding:0 0 0 0;left:0;border:0 0 0 0;"></ITAT:ITATPlaceHolder> 
                   </td>
                   </tr>

            
            <tr>
                <td class="ProfileCaption" width="114px">
                    <asp:CustomValidator ID="reqStatus" runat="server" Text=""  Display="Dynamic" OnServerValidate="CheckAtLeastOneStatus" ValidationGroup="RequiredValidation1" ErrorMessage="Select atleast 1 Status."></asp:CustomValidator> Status
                </td>
                <td class="DataEntryEdit" width="100%">
                    <asp:CheckBoxList ID="chklstStatus" runat="server" RepeatDirection="Horizontal" RepeatColumns="6"
                        OnDataBound="chklstStatus_DataBound">
                    </asp:CheckBoxList>
                </td>
            </tr>
          
            <tr>
                <td class="ProfileCaption" width="114px">
                    Load Type
                </td>
                <td class="DataEntryEdit" width="100%">
                    <asp:RadioButton ID="radTypeFull" Checked="true" runat="server" GroupName="loadType"
                        Text="Full Load" />
                    <asp:RadioButton ID="radTypeDelta" Checked="false" runat="server" GroupName="loadType"
                        Text="Delta Load" />                     
                    <asp:TextBox ID="txtDeltaDays" runat="server" Width="114px" MaxLength="12" Visible="true"/> days.
                </td>
            </tr>
             <tr>
                <td class="ProfileCaption" width="114px">
                   Default Date Format
                </td>
                <td class="DataEntryEdit" width="100%">
                <asp:DropDownList ID="ddlDefaultDateFormat" runat="server"  Width="90px">
                                            </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="ProfileCaption" width="114px">
                    Path
                </td>
                <td class="DataEntryEdit" width="100%">
                   <asp:TextBox ID="txtPath" runat="server" Width="100%"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td class="ProfileCaption" width="114px">
                    Active
                </td>
                <td class="DataEntryEdit" width="100%">
                    <asp:RadioButton ID="radActiveYes"  Text="Yes" runat="server" GroupName="radActive" />
                    <asp:RadioButton ID="radActiveNo" Text="No" runat="server" GroupName="radActive" />
                </td>
            </tr>
            <tr>
                <td class="ProfileCaption" width="114px">
                   Error Log Email Recipient
                </td>
                <td class="ProfileEdit" width="100%">
                <asp:TextBox ID="txtEmail" runat="server" width="100%"></asp:TextBox>   
                (Enter multiple email addresses "," delimited e.g. Robert.Trimble@kindredhealthcare.com,Ela.Edwards@kindredhealthcare.com)
                </td>
            </tr>
            <tr>
                <td style="height: 34px" class="AlignLeft">
                    &nbsp;
                </td>
                <td class="AlignRight" style="padding: 0 0 0 0; height: 34px;">
                    <asp:Button ID="btnPromote" runat="server" Text="Save" CssClass="KnectButton" 
                        onclick="btnPromote_Click" />
                    <asp:Button ID="btnDemote" runat="server" Text="Cancel" CssClass="KnectButton" onclick="btnCancel_Click" />
                </td>
            </tr>
             
        </table>
         <asp:Panel ID="term1Panel" runat="server">
            </asp:Panel>
    </div>
    </form>
</body>
</html>

