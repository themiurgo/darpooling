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
        void HandleForwardedUserCommand(ForwardedRequest fwdRequest);

        [OperationContract(IsOneWay = true)]
        void ForwardedUserCommandResult(ForwardedRequest fwdRequest, Result finalResult);

    }


    [DataContract]
    public class ForwardRequiredResult : Result { }

    // Acts like a Decorator
    [DataContract]
    public class ForwardedRequest
    {
        [DataMember]
        Command forwardedCommand;
        [DataMember]
        string rootSender;
        [DataMember]
        string forwardingKey;

        /*
        ForwardedCommand(Command clientCommand)
        {
            this.component = clientCommand;
        }*/

        public Command ForwardedCommand
        {
            get { return forwardedCommand; }
            set { forwardedCommand = value; }
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
