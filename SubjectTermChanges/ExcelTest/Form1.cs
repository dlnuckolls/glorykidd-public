using System;
using System.Data;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.OleDb;

namespace ExcelTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            QueryFile();
        }

        public static DataSet QueryFile()
        {
            //OLEDB;Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\MyExcel.xls;Extended Properties="Excel 8.0;HDR=Yes;IMEX=1";
            //            string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Development\ITAT2\Web\Book1.xls";
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Development\\ITAT2\\Web\\Book1.xls;Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            //string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Book1.xls;Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");
            using (DbConnection connection = factory.CreateConnection())
            {
                try
                {
                    connection.ConnectionString = connectionString;
                    using (DbCommand command = connection.CreateCommand())
                    {
                        // Cities$ comes from the name of the worksheet
                        //                    command.CommandText = "SELECT ID,City,State FROM [Cities$]";
                        //                    command.CommandText = "SELECT * FROM [Cities$]";
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT 'Test'";
                        connection.Open();
                        using (DbDataReader dr = command.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                //Debug.WriteLine(dr["ID"].ToString());
                            }
                        }
                    }
                }
                catch (OleDbException ex)
                {
                    string sError = DisplayOleDbErrorCollection(ex);
                }
            }
            return null;
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

    }
}
