using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication;
using System.Threading;
using System.ServiceModel;

namespace ServiceNodeCore
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class DarPoolingServiceMobile : IDarPoolingMobile
    {
        private DarPoolingService mainServiceImpl;
        
        private Dictionary<string, Result> results;
        private ReaderWriterLockSlim resultsLock;

        private DarPoolingServiceMobile() { }


        public DarPoolingServiceMobile(DarPoolingService serviceImpl)
        {
            mainServiceImpl = serviceImpl;

            results = new Dictionary<string, Result>();
            resultsLock = new ReaderWriterLockSlim();
        }


        #region IDarPoolingMobile Members

        string IDarPoolingMobile.HandleDarPoolingMobileRequest(Command c)
        {
            Console.WriteLine("{0} Received Mobile Request", LogTimestamp);
            string requestID = mainServiceImpl.generateGUID();
            c.CommandID = requestID;
            
            // Handle the command using the main service. 
            mainServiceImpl.HandleMobileDarPoolingRequest(c);
            
            Console.WriteLine("Returning Request ID to client");
            //string requestIDTest = DateTime.Now.ToString();
            //return requestIDTest;
            return requestID;
        }


        Result IDarPoolingMobile.GetMobileResult(string requestID)
        {
            if (IsResultReady(requestID))
            {
                Result finalResult = ExtractResult(requestID);
                return finalResult;
            }
            else
            {
                WaitAndTryResult waitResult = new WaitAndTryResult();
                waitResult.Comment = "You have to wait";
                waitResult.MilliSeconds = 500;
                return waitResult;
            
            }
        }

        #endregion


        public string LogTimestamp
        {
            get
            {
                string compact = "HH:mm:ss.fff";
                string time = DateTime.Now.ToString(compact);
                return ("[" + time + "]");
            }
        }


        private void AddMobileResult(string requestID, Result result)
        {
            resultsLock.EnterWriteLock();
            try
            {
                results.Add(requestID, result);
            }
            finally
            {
                resultsLock.ExitWriteLock();
            }
        }

        private Result ExtractResult(string requestID)
        {
            resultsLock.EnterWriteLock();
            try
            {
                Result finalResult = results[requestID];
                results.Remove(requestID);
                return finalResult;
            }
            finally
            {
                resultsLock.ExitWriteLock();
            }
        }


        private bool IsResultReady(string requestID)
        {
            resultsLock.EnterReadLock();
            try
            {
                return results.ContainsKey(requestID);
            }
            finally
            {
                resultsLock.ExitReadLock();
            }

        }



    }
}
