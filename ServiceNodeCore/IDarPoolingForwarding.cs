using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.ServiceModel;
using Communication;

namespace ServiceNodeCore
{
    /// <summary>
    /// This interface is used by the Service Nodes to pass any command that contains
    /// a request that cannot be satisfied by the current node.
    /// </summary>
    [ServiceContract]
    public interface IDarPoolingForwarding
    {
        [OperationContract(IsOneWay = true)]
        void HandleForwardedDarPoolingRequest(Command fwdCommand, string rootSenderAddress);

        [OperationContract(IsOneWay = true)]
        void HandleForwardedRangeSearch(Command command, string senderAddress, QueryBuilder query);

        [OperationContract(IsOneWay = true)]
        void ReturnFinalResult(Result finalResult, Command originalCommand);

    }

    
    /// <summary>
    /// ForwardRequiredResult represents the situation in which a Command
    /// must be forwarded to reach the node that is responsible for 
    /// handling the user request.
    /// </summary>
    public class ForwardRequiredResult : Result 
    {
        // GUID of the request.
        string requestID;
        // Complete address of the next destination node of
        // the Command
        string destinationAddress;

        public string RequestID
        {
            get { return requestID; }
            set { requestID = value; }
        }

        public string Destination
        {
            get { return destinationAddress; }
            set { destinationAddress = value; }
        }
        
    }


    public class MultipleForwardRequiredResult : Result
    {
        //private QueryBuilder originalQuery;

        //MultipleForwardRequiredResult(QueryBuilder qb)
        //{
        //    originalQuery = qb;
        //}
    
    }

}
