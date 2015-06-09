using System;
using System.Data;
using System.Data.SqlClient;
using Kindred.Common.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Collections.Generic;

namespace Kindred.Knect.ITAT.Data
{
	public static class Excel
	{
        //private static string OLE_DB_4 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
        private static string OLE_DB_12 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";

        public static DataTable QueryFile(string fileName, string tableName, out string error)
        {
            error = null;
            DataTable dtExport = null;
            string connectionString = string.Format(OLE_DB_12, fileName);
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
            using (DbConnection connection = factory.CreateConnection())
            {
                try
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    using (DbCommand command = connection.CreateCommand())
                    {
                        //command.CommandText = "SELECT * FROM [Import$]";
                        command.CommandText = string.Format("SELECT * FROM [{0}$]",tableName);
                        command.CommandType = CommandType.Text;
                        using (DbDataReader dr = command.ExecuteReader())
                        {
                            dtExport = ExportTable(dr.GetSchemaTable());

                            DataRow rowHeader = dtExport.NewRow();
                            foreach (DataColumn col in dtExport.Columns)
                                rowHeader[col.ColumnName] = col.ColumnName;
                            dtExport.Rows.Add(rowHeader);

                            while (dr.Read())
                            {
                                DataRow rowExport = dtExport.NewRow();
                                foreach (DataColumn colExport in dtExport.Columns)
                                {
                                    rowExport[colExport.ColumnName] = dr[colExport.ColumnName];
                                }
                                dtExport.Rows.Add(rowExport);
                            }
                        }
                    }
                }
                catch (OleDbException ex)
                {
                    error = ex.Message;
                    //error = DisplayOleDbErrorCollection(ex);
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            return dtExport;
        }

        public static string DisplayOleDbErrorCollection(OleDbException exception)
        {
            string sError = "";
            for (int i = 0; i < exception.Errors.Count; i++)
            {
                sError += "Index #" + i + "\n" +
                       "Message: " + exception.Errors[i].Message + "\n" +
                       "Native: " + exception.Errors[i].NativeError.ToString() + "\n" +
                       "Source: " + exception.Errors[i].Source + "\n" +
                       "SQL: " + exception.Errors[i].SQLState + "\n";
            }
            return sError;
        }

        private static DataTable ExportTable(DataTable readerTable)
        {
            DataTable export = new DataTable();

            foreach (DataRow drReaderTable in readerTable.Rows)
                export.Columns.Add(drReaderTable["ColumnName"].ToString());

            return export;
        }

        private static string ExcelColumn(int columnIndex)
        {
            if (columnIndex < 0)
                return "UNDEFINED";
            char Right = (char)('A' + columnIndex % 26);
            char Left = ' ';
            if (columnIndex > 26)
                Left = (char)('A' + (columnIndex / 26) - 1);
            return string.Format("{0}{1}", Left, Right).Trim();
        }

        public static DataTable QueryFile(string[] rows, int rowMain, int rowSecondary, char delimiter, out string error)
        {
            error = null;
            DataTable dtExport = new DataTable();

            if (rows.Length >= rowSecondary + 1)
            {
                string[] columnNames = rows[rowMain].Split(delimiter);
                if (columnNames.Length > 0)
                {
                    int columnIndex = 0;
                    List<string> uniqueNames = new List<string>();
                    foreach (string columnName in columnNames)
                    {
                        if (string.IsNullOrEmpty(columnName))
                        {
                            error = string.Format("Column name at Excel column position '{0}' ({1:D}) is blank", ExcelColumn(columnIndex), columnIndex);
                            break;
                        }
                        if (uniqueNames.Contains(columnName))
                        {
                            throw new Exception(string.Format("Excel column name {0} is duplicated", columnName));
                        }
                        dtExport.Columns.Add(columnName);
                        uniqueNames.Add(columnName);
                        columnIndex++;
                    }
                    if (string.IsNullOrEmpty(error))
                    {
                        //Add a row that represents the main columns, in order to keep the row numbering consistent.
                        dtExport.Rows.Add(dtExport.NewRow());
                        for (int rowIndex = rowSecondary; rowIndex < rows.Length; rowIndex++)
                        {
                            DataRow drImport = dtExport.NewRow();
                            //If this is not a 'blank' row, then process it
                            if (rows[rowIndex].Trim(delimiter).Length > 0)
                            {
                                string[] rowImport = rows[rowIndex].Split(delimiter);
                                if (rowImport.Length == columnNames.Length)
                                {
                                    for (int colIndex = 0; colIndex < columnNames.Length; colIndex++)
                                    {
                                        drImport[colIndex] = rowImport[colIndex];
                                    }
                                }
                                else
                                {
                                    string rowIdentifier = rowIndex == rowSecondary ? string.Format("'secondary headings row ({0:D})'", rowSecondary + 1) : string.Format("row {0:D}",rowIndex + 1);
                                    
                                    string lastColumn = string.Empty;
                                    try
                                    {
                                        lastColumn = columnNames[rowImport.Length - 1];
                                    }
                                    catch { }
                                    if (string.IsNullOrEmpty(lastColumn))
                                        error = string.Format("Row data mismatch, {0} - expected {1:D} columns, instead found {2:D} columns.", rowIdentifier, columnNames.Length, rowImport.Length);
                                    else
                                        error = string.Format("Row data mismatch, {0} - expected {1:D} columns, instead found {2:D} columns.  The last column found corresponds to '{3}'.", rowIdentifier, columnNames.Length, rowImport.Length, lastColumn);
                                    break;
                                }
                                dtExport.Rows.Add(drImport);
                            }
                            else
                            {
                                if (rowIndex == rowSecondary)
                                {
                                    //Add a row that represents the secondary columns, in order to keep the row numbering consistent.
                                    dtExport.Rows.Add(drImport);
                                }
                            }
                        }
                    }
                }
                else
                    error = "No data columns found";
            }
            else
                error = "Fewer than 2 excel rows found";

            return dtExport;
        }
    
    
    }
}
