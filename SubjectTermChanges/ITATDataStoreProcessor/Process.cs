using System;
using System.Collections.Generic;
using System.Text;
using Kindred.Common.Batch;
using Kindred.Knect.ITAT.Business;
using Kindred.Common.Logging;
using System.Data;
using Kindred.Knect.ITAT;

namespace Kindred.Knect.ITAT
{
	class Process : BatchBase
	{
		static int Main(string[] args)
		{
            int nReturn = (new Process()).Run(args);
            //Console.ReadLine();
            return nReturn;
        }

		public override void InitializeStorageAreas()
		{
		}

		public override int ProcessData()
		{
            int errorCount = 0;
            string systemName = string.Empty;
            try
            {
                using (DataSet ds = Business.ITATSystem.GetSystemList())
                {
                    foreach (DataRow drSystem in ds.Tables[0].Rows)
                    {
                        int preSystemErrorCount = errorCount;
                        try
                        {
                            systemName = (string)drSystem[Data.DataNames._C_ITATSystemName];
                            errorCount += new Business.SystemStore(Log).ProcessSystem((Guid)drSystem[Data.DataNames._C_ITATSystemID], systemName);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("ProcessSystem halted for '{0}'.  Exception: '{1}'.  StackTrace: '{2}'.", systemName, ex.Message, ex.StackTrace));
                            errorCount++;
                        }
                        if (errorCount > 0)
                        {
                            if (preSystemErrorCount != errorCount)
                                Log.Error(string.Format("Processing system '{0}' resulted in {1:D} errors.", systemName, errorCount - preSystemErrorCount));
                            else
                                Log.Info(string.Format("Processing system '{0}' resulted in no errors.", systemName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Job stopped at ProcessData during processing of system '{0}' due to unhandled exception: '{1}'.  StackTrace: '{2}'.", systemName, ex.Message, ex.StackTrace));
                errorCount++;
            }
            if (errorCount > 0)
                Log.Error(string.Format("Job error count = {0:D}", errorCount));
            return errorCount;
		}

		public override void DisplayCounters()
		{
		}

    }
}
