using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Kindred.Knect.ITAT.Web.Controls;


namespace Kindred.Knect.ITAT.Web.Common
{
    public static class GridHelper
    {
        public delegate bool ItemComparison<T>(T item, string id);

        //This method will be used to sync client side state of the grid rows to the server.
        public static void SyncToGridClientState<T>(GridMoveRows ctlGridMoveRows, List<T> itemList, ItemComparison<T> itemcomparision) 
        {

            //The initial, changed info is on the client side in the format "0:1:865ecef7-79ca-40fa-8509-dcbfaf0e279d;"
            //where the elements are initialIndex, newIndex, Guid of the row being moved. 

            HiddenField hf = (HiddenField)ctlGridMoveRows.FindControl("hfListIndexes");
            string sortingString = hf.Value;

            if (String.IsNullOrEmpty(sortingString))
                return;

            string[] pairs = sortingString.Split(";".ToCharArray());

            foreach (string pair in pairs)
            {
                if (!String.IsNullOrEmpty(pair))
                {
                    string[] vals = pair.Split(":".ToCharArray());

                    if (vals.Length == 3)
                    {

                        foreach (T item in itemList)
                        {                  

                            if (itemcomparision(item, vals[2]))
                            {
                                itemList.Remove(item);
                                itemList.Insert(Convert.ToInt32(vals[1]), item);
                                break;
                            }
                        }

                        

                    }

                }
            }


        }
    }
}
