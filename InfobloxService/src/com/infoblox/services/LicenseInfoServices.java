/*
 * DLN - 30 OCT 2014
 *  This is the base for the custom service to increase performance 
 *  when quering FNO for data.
 *      Methods:
 *          getPartyList (string, string)
 *              Takes in OrgId and State and returns JSON Array with Org List
 * 
 */
package com.infoblox.services;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.Statement;
import com.flexnet.operations.publicapi.DBConnectionManager;
import org.json.simple.JSONObject;
import org.json.simple.JSONArray;

/**
 *
 * @author dave
 */
public class LicenseInfoServices {
    /* URL for the registeruser Public Web service */

    private static final String FNO_PUBSVC_LICENSEINFOSERVICE_URL = "http://localhost:8888/flexnet/pubservices/licenseinfoservice";

    /**
     *
     * @param orgId
     * @param statusType
     * @return
     */
    public static JSONArray getPartyList(String orgId, String statusType) {
        JSONArray rtnArray = new JSONArray();
        try {
            class orgItem {

                String orgId;
                String orgName;
            }
            String DATA_SOURCE_NAME = "jdbc/CustomerDBDataSource";

            /* get DBConnection manager instance*/
            DBConnectionManager conMgr = DBConnectionManager.Factory.getInstance();

            /* get a jdbc connection from the pool for the given data source name */
            Connection connection = conMgr.getDBConnection(DATA_SOURCE_NAME);

            String sql = "DECLARE "
                    + "  ORGID VARCHAR2(200); "
                    + "  STATE VARCHAR2(200); "
                    + "  v_Return sys_refcursor; "
                    + "BEGIN "
                    + "  ORGID := '" + orgId + "'; "
                    + "  STATE := '" + statusType + "'; "
                    + "  v_Return := GETPARTYLIST( ORGID => ORGID, STATE => STATE ); "
                    + "  :v_Return := v_Return; "
                    + "END;";
            Statement stmt = connection.createStatement();
            ResultSet rs = stmt.executeQuery(sql);
            while (rs.next()) {
                JSONObject rtnVal = new JSONObject();
                rtnVal.put("end_customer_id", rs.getString("NAME"));
                rtnVal.put("end_customer_name", rs.getString("DISPLAYNAME"));
                rtnArray.add(rtnVal);
            }

            /* process the response message to check the status 200 for OK, 500 for ERROR */
        } catch (Exception e) {
            System.out.println("Error while requesting data from FlexNet Operations Server " + e.getMessage());
        }
        return rtnArray;

    }

}
