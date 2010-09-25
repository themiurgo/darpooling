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
        void HandleForwardedDarPoolingRequest(Command fwdCommand, string senderAddress);

        [OperationContract(IsOneWay = true)]
        void BackPropagateResult(Result finalResult, Command originalCommand);

    }


    //[DataContract]
    public class ForwardRequiredResult : Result 
    {
        string requestID;
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

    /*
    // Acts like a Decorator
    [DataContract]
    public class ForwardedCommand : Command
    {
        [DataMember]
        Command component;
        [DataMember]
        string rootSender;
        [DataMember]
        string forwardingKey;

        public override Result Execute()
        {
            return component.Execute();
        }

        public override Result EndExecute(IAsyncResult asyncValue)
        {
            return component.EndExecute(asyncValue);
        }


        public int CommandID
        {
            get { return component.CommandID; }
            set { component.CommandID = value; }
        }

        public IDarPoolingOperations Receiver
        {
            get { return component.Receiver; }
            set { component.Receiver = value; }
        }

        public AsyncCallback Callback
        {
            set { component.Callback = value; }
        }

        public Command Component
        {
            get { return component; }
            set { component = value; }
        }

        public string RootSender
        {
            get { return rootSender; }
            set { rootSender = value; }
        }

        public string ForwardingKey
        {
            get { return forwardingKey; }
            set { forwardingKey = value; }
        }

    }
    */

/*
    // When the Result of a previously forwarded command is received, the necessity of
    // applying some changes in the state of the receiving service could arise. This
    // interface is used in such situations.
    public interface IRegistrable
    {
        Result GetResult();
        void RegisterChanges();    
    }

    [DataContract]
    public class ForwardedLoginResult : Result,IRegistrable
    {
        [DataMember]
        private string username;
        [DataMember]
        private Result finalResult;

        public delegate void RegisterUser(string username);
        RegisterUser registerJoinedUser;

        public void RegisterChanges()
        {
            if (username != null && registerJoinedUser != null)
            {
                IAsyncResult asyncResult = registerJoinedUser.BeginInvoke(this.username, null, null);
                // Wait until the delegate complete its execution.
                while (!asyncResult.IsCompleted) { }
            }

        }

        public string JoinedUser
        {
            get { return username; }
            set { username = value; }
        }

        public RegisterUser ResultDelegate
        {
            set { registerJoinedUser = value; }
        }
   
    }

    */
}
