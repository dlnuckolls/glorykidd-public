<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GridMoveRows.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.Controls.GridMoveRows" %>
<img id="btnListMoveUp" width="11px" height="11px" alt="Move Row Up"  onclick="MoveRow(currentRowId - 1);" /><br />						
<img id="btnListMoveDown" width="11px" height="11px" alt="Move Row Down"  onclick="MoveRow(currentRowId + 1);" />
<asp:HiddenField ID="hfListIndexes" runat="server" />
<asp:HiddenField ID="hfTargetControl" runat="server" />
<asp:HiddenField ID="hfCurrentRowId" runat="server" />

    <script language="javascript" type="text/javascript">


        var grdArray = null;
        var currentRowId = null;
        var gridRowPartialName = document.getElementById('<%=hfTargetControl.ClientID%>').value + "_R_";

        document.getElementById("btnListMoveUp").src = "Images/MoveUpDisabled.gif";
        document.getElementById("btnListMoveDown").src = "Images/MoveDownDisabled.gif";






        function SelectRow(obj) {

            var rows = GetRows();

            if (currentRowId != null) {
                rows[currentRowId].style.backgroundColor = '';
                rows[currentRowId].style.color = '';
                rows[currentRowId].style.height = '';
                rows[currentRowId].style.fontSize = '';
                rows[currentRowId].style.fontWeight = '';
                rows[currentRowId].style.fontFamily = '';
            }

            currentRowId = obj.rowIndex - 1;  // Using 0-based index
            rows[currentRowId].style.color = 'gold';
            rows[currentRowId].style.backgroundColor = 'navy';            
            rows[currentRowId].style.height = 'auto';
            rows[currentRowId].style.fontSize = 'xx-small';
            rows[currentRowId].style.fontWeight = '400';
            rows[currentRowId].style.fontFamily = 'Verdana';
            rows[currentRowId].style.linkColor = 'white';

            var arrLinks = rows[currentRowId].getElementsByTagName("a");

            for (var i = 0; i < arrLinks.length; i++) {
                arrLinks[i].style.color = 'white';
            }            
            

            

            SetUpDownImages(currentRowId);
           
        }





        // Move row up/down
        function MoveRow(rowId) {




            var rows = GetRows();

            if (rowId < 0 || rowId > rows.length - 1)
                return;            
            
          

            //Get the current row positions on the first attempt and fill the array

            if (grdArray == null) {
                grdArray = new Array(rows.length);

                //first round fill

                for (var i = 0; i < rows.length; i++) {
                    grdArray[i] = new Array(4);


                    //first element - grid rowid

                    grdArray[i][0] = rows[i].id;

                    //second element - row index initial
                    grdArray[i][1] = i;

                    //third element - row index changed
                    grdArray[i][2] = i;

                    //fourth element - ischanged flag
                    grdArray[i][3] = false;

                    //Fifth element - The Guid of the item
                    grdArray[i][4] = rows[i].id.substr(gridRowPartialName.length, (rows[i].id.length - (gridRowPartialName.length)));         
                    
                    
                }

            }


            SetUpDownImages(rowId);





            var newRow = rows[currentRowId].cloneNode('true');
            var delRow = rows[currentRowId];

            var p = delRow.parentNode;
            p.removeChild(delRow);

            var row2 = null;
            var newRowID = null;

            if (currentRowId < rowId) { // Going down

                newRowID = rowId + 1;

                if (newRowID < rows.length) {
                    row2 = rows[newRowID];
                }
            }
            else {

                newRowID = rowId;
                row2 = rows[newRowID]; // Going up
            }

            p.insertBefore(newRow, row2);




            if (currentRowId != null) {


                for (var j = 0; j < grdArray.length; j++) {
                    if (grdArray[j][0] == rows[currentRowId].id) {
                        grdArray[j][2] = rowId;
                        grdArray[j][3] = true;


                    }

                }

            }

            SetHiddenField();

            currentRowId = rowId;
        }


        function GetRows() {
            var strTagName = 'tr';
            var arrElements = document.getElementsByTagName(strTagName);
            var arrReturnElements = new Array();
            var oElement;

            
            for (var i = 0; i < arrElements.length; i++) {
                oElement = arrElements[i];
                if (oElement.id.indexOf(gridRowPartialName) == 0) {
                    arrReturnElements.push(oElement);
                }
            }

            return (arrReturnElements);
        }





        function SetHiddenField() {
            var ret = "";
            if (grdArray != null) {

                for (k = 0; k < grdArray.length; k++) {

                    if (grdArray[k][1]!= grdArray[k][2] && grdArray[k][3] == true) {
                        ret += grdArray[k][1] + ":" + grdArray[k][2] + ":" + grdArray[k][4] + ";"
                    }
                }

                document.getElementById('<%=hfListIndexes.ClientID%>').value = ret;
            }
        }



        function SetUpDownImages(rowId) {


            var rows = GetRows();




            if (rowId <= 0) {

                document.getElementById("btnListMoveUp").src = "Images/MoveUpDisabled.gif";
            }
            else {
                document.getElementById("btnListMoveUp").src = "Images/MoveUp.gif";
            }


            if (rowId > rows.length - 2) {
                document.getElementById("btnListMoveDown").src = "Images/MoveDownDisabled.gif";
            }
            else {
                document.getElementById("btnListMoveDown").src = "Images/MoveDown.gif";
            }






        }        
    
    </script>