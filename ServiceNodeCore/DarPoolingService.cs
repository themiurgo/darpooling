using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

using Communication;
using System.Threading;
using System.ServiceModel;

namespace ServiceNodeCore
{
    /// <summary>
    /// DarPoolingService implements two service interfaces: IDarPooling and IDarPoolingForwarding.
    /// The first interface, IDarPooling, declares the set of method that darpooling clients
    /// use to send their requests.
    /// The second interface, IDarPoolingForwarding, is used only by the services to exchange 
    /// information.
    /// 
    /// The main goal of DarPoolingService is to wait for incoming requests from clients and then
    /// satisfy these requests by using a ServiceNodeCore instance.
    /// When the result of a particular request is ready, DarPoolingService
    /// will call the client and send it back the result. If the node doesn't have the requested information,
    /// it will forward the request to another node.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DarPoolingService : IDarPooling, IDarPoolingForwarding
    {
        // An istance of ServiceNodeCore is the receiver for all client commands.
        private ServiceNodeCore receiver;

        // A local identifier for Commands.
        private int commandCounter;

        // Keep track of the currently joined users.
        private List<string> joinedUserNames;
        private ReaderWriterLockSlim joinedUserNamesLock;
        
        // Let the Service to retrieve the client that has sent a given command.
        private Dictionary<string, IDarPoolingCallback> commandClient;
        private ReaderWriterLockSlim commandClientLock;

        // Keep track of the received forwarded commands.
        private Dictionary<string, string> fwdCommandService;
        private ReaderWriterLockSlim fwdCommandServiceLock;
        

        /// <summary>
        /// FIXME: The default constructor has been marked as private only to
        /// be aware of attemps to instantiate this class (WCF mess).
        /// </summary>
        private DarPoolingService() { }


        /// <summary>
        /// The constructor only set the receiver and initialize
        /// the data structures.
        /// </summary>
        /// <param name="receiver">A ServiceNodeCore instance that represents 
        /// the 'receiver' of the commands.</param>
        public DarPoolingService(ServiceNodeCore receiver)
        {
            this.receiver = receiver;

            joinedUserNames = new List<string>();
            joinedUserNamesLock = new ReaderWriterLockSlim();
            
            commandCounter = -1;
            commandClient = new Dictionary<string, IDarPoolingCallback>();
            commandClientLock = new ReaderWriterLockSlim();
            
            fwdCommandService = new Dictionary<string, string>();
            fwdCommandServiceLock = new ReaderWriterLockSlim();

        }


        /// <summary>
        /// Method of the IDarpooling interface.
        /// It receives a Command which deals with User management. 
        /// After setting some command paramenters as specified below,
        /// it invokes the Execute() method on the command itself.
        /// </summary>
        /// <param name="command">The Command object, sent by a client</param>
        public void HandleDarPoolingRequest(Command command)
        {
            Console.WriteLine("\n{0} node received {1}", receiver.NodeName.ToUpper(),command.GetType());

            // Assign a GUID to the command
            command.CommandID = generateGUID(command.GetType());

            // Save information about the client that has sent the command
            IDarPoolingCallback client = OperationContext.Current.GetCallbackChannel<IDarPoolingCallback>();
            commandClient.Add(command.CommandID, client);

            // Set a ServiceNodeCore as the receiver of the command;
            command.Receiver = receiver;
            
            command.Callback = new AsyncCallback(ProcessUserResult);

            command.Execute();
        }


        /// <summary>
        /// TODO: to be implemented
        /// </summary>
        /// <param name="tripCommand"></param>
        public void HandleTrip(Command tripCommand)
        {
            Console.WriteLine("\n{0} node received {1}", receiver.NodeName.ToUpper(), tripCommand.GetType());

            tripCommand.CommandID = generateGUID(tripCommand.GetType());
            IDarPoolingCallback client = OperationContext.Current.GetCallbackChannel<IDarPoolingCallback>();
            commandClient.Add(tripCommand.CommandID, client);

            tripCommand.Receiver = receiver;
            tripCommand.Callback = new AsyncCallback(ProcessUserResult);
            tripCommand.Execute();
        }


        /// <summary>
        /// This is the callback method of HandleUser, i.e. the method that is automatically
        /// invoked when a user command complete its Execute(). This behavior is obtained by
        /// exploiting the asynchronous delegate approach. See Communication.Command for further details.
        /// This method determines if the user request has been satisfied (in which case the final result
        /// is returned to client) or not (in which case the command must be forwarded to another node).
        /// </summary>
        /// <param name="iAsyncResult">Represent the object which is available after the 
        /// asynchronous invocation of Execute(). It gives access to all state information.</param>
        public void ProcessUserResult(IAsyncResult iAsyncResult)
        {
            Result executionResult;

            // Retrieve the Command whose execution has ended.
            Command originalCommand = (Command)iAsyncResult.AsyncState;

            // Obtain the Result of the Command.
            executionResult = originalCommand.EndExecute(iAsyncResult);

            bool forwarded = TryForwardCommand(originalCommand,executionResult);

            if ( !forwarded  )
            {
                ReturnFinalResult(originalCommand, executionResult);            
            }
        }


        /// <summary>
        /// IDarPoolingForwarding method. The user-related command uses only one hop,
        /// i.e. they always reach the correct and final destination node with only
        /// one connession. For these reasons, we only have to execute the command,
        /// and the return the result to the rootSender service node.
        /// </summary>
        /// <param name="forwardedCommand"></param>
        public void HandleForwardedDarPoolingRequest(Command fwdCommand, string senderAddress)
        {
            Console.WriteLine("\n{0} node received FWD:{1}", receiver.NodeName.ToUpper(), fwdCommand.GetType());

            AddFwdCommandService(fwdCommand.CommandID, senderAddress);
 
            fwdCommand.Receiver = receiver;
            fwdCommand.Callback = new AsyncCallback(ProcessResultOfForwardedCommand);
            fwdCommand.Execute(); 
        }


        public void ProcessResultOfForwardedCommand(IAsyncResult iAsyncResult)
        {
            Result executionResult;
            Command fwdCommand = (Command)iAsyncResult.AsyncState;
            executionResult = fwdCommand.EndExecute(iAsyncResult);

            bool forwarded = TryForwardCommand(fwdCommand, executionResult);

            if (!forwarded)
            {
                BackPropagateResult(fwdCommand, executionResult);
            }
          
        }


        /// <summary>
        /// IDarPoolingForwarding method. The service node obtain the result of the forwarded command.
        /// </summary>
        /// <param name="forwardedCommand"></param>
        /// <param name="finalResult"></param>
        public void BackPropagateForwardedDarPoolingRequest(Command fwdCommand, Result finalResult)
        {
           
            if (IsFwdCommand(fwdCommand.CommandID))
            {
                BackPropagateResult(fwdCommand, finalResult);
            }
            else
            {
                ReturnFinalResult(fwdCommand, finalResult);
            }
            
        }



        private bool TryForwardCommand(Command command, Result lastExecutionResult) 
        {
            // Determine if the Command must be forwarded to another Service.
            ForwardRequiredResult forward = lastExecutionResult as ForwardRequiredResult;

            // The command must be forwarded
            if (forward != null)
            {
                Console.Write("Forwarding a {0} to {1}", command.GetType(), forward.Destination);

                // Get ready to call the remote node service via the dedicated forward endpoint address.
                BasicHttpBinding fwdBinding = new BasicHttpBinding();
                EndpointAddress fwdEndpoint = new EndpointAddress(forward.Destination);
                ChannelFactory<IDarPoolingForwarding> fwdChannelFactory = new ChannelFactory<IDarPoolingForwarding>(fwdBinding, fwdEndpoint);
                IDarPoolingForwarding destinationService = fwdChannelFactory.CreateChannel();

                string senderAddress = receiver.BaseForwardAddress + receiver.NodeName;

                // Forward the Command to the remote note, using the IDarPoolingForwarding interface.
                destinationService.HandleForwardedDarPoolingRequest(command, senderAddress);

                // Close the channels. The communication is fire-and-forget (one-way)
                ((IClientChannel)destinationService).Close();
                fwdChannelFactory.Close();

                return true;

            }
            return false;
        
        }


        private void BackPropagateResult(Command fwdCommand, Result finalResult)
        {
            string senderAddress = GetService(fwdCommand.CommandID);
            RemoveFwdCommand(fwdCommand.CommandID);
            Console.WriteLine("{0} gives back a {1} to {2}", receiver.NodeName, finalResult.GetType(), senderAddress);

            // Get ready to contact the sender Service node.
            BasicHttpBinding myBinding = new BasicHttpBinding();
            EndpointAddress myEndpoint = new EndpointAddress(senderAddress);
            ChannelFactory<IDarPoolingForwarding> myChannelFactory = new ChannelFactory<IDarPoolingForwarding>(myBinding, myEndpoint);
            IDarPoolingForwarding service = myChannelFactory.CreateChannel();

            // Give the result back to the the sender Service node.
            service.BackPropagateForwardedDarPoolingRequest(fwdCommand, finalResult);

            // Close channel
            ((IClientChannel)service).Close();
            myChannelFactory.Close();
        
        }


        private void ReturnFinalResult(Command originalCommand, Result finalResult)
        {
            // Apply changes on the service
            RegisterResult(finalResult);

            Console.Write("Sending the FINAL Result to Client.", originalCommand.CommandID);

            IDarPoolingCallback client = GetClient(originalCommand.CommandID);
            RemoveCommand(originalCommand.CommandID);
            client.GetResult(finalResult);
            ((IClientChannel)client).Close();
        
        }



        // Generate GUIDs using SHA1
        public string generateGUID(Type objType)
        {
            // Use atomic sum
            Interlocked.Add(ref commandCounter, 1);
            string baseString = receiver.BaseForwardAddress + receiver.NodeName +
                                DateTime.Now.ToString() + commandCounter + objType.ToString();
            return Tools.HashString(baseString);
        }


        private void RegisterResult(Result commandResult)
        {
            LoginOkResult login = commandResult as LoginOkResult;
            if (login != null)
            {
                AddJoinedUser(login.AuthorizedUsername);
            }
        
        }


        #region JoinedUser List Management


        public void AddJoinedUser(string username)
        {
            joinedUserNamesLock.EnterWriteLock();
            try
            {
                Console.WriteLine("{0} joined {1} node", username, receiver.NodeName);
                joinedUserNames.Add(username);
            }
            finally
            {
                joinedUserNamesLock.ExitWriteLock();
            }
        }


        public bool IsJoinedUser(string username)
        {
            joinedUserNamesLock.EnterReadLock();
            try
            {
                return joinedUserNames.Contains(username);
            }
            finally
            {
                joinedUserNamesLock.ExitReadLock();
            }
        }


        public void RemoveJoinedUser(string username)
        {
            joinedUserNamesLock.EnterWriteLock();
            try
            {
                if (joinedUserNames.Remove(username))
                {
                    Console.WriteLine("{0} unjoined {1}", username, receiver.NodeName);
                }
            }
            finally
            {
                joinedUserNamesLock.ExitWriteLock();
            }
        }

        #endregion


        #region Command-Client Management
        
        private void AddCommandClient(string commandID, IDarPoolingCallback client)
        {
            commandClientLock.EnterWriteLock();
            try
            {
                commandClient.Add(commandID, client);
            }
            finally
            {
                commandClientLock.ExitWriteLock();
            }
        }

        private IDarPoolingCallback GetClient(string commandID)
        {
            commandClientLock.EnterReadLock();
            try
            {
                return commandClient[commandID];
            }
            finally
            {
                commandClientLock.ExitReadLock();
            }
        }

        private void RemoveCommand(string commandID)
        {
            commandClientLock.EnterWriteLock();
            try
            {
                commandClient.Remove(commandID);
            }
            finally
            {
                commandClientLock.ExitWriteLock();
            }
        }

        #endregion


        #region FwdCommand-Service Management

        private void AddFwdCommandService(string fwdCommandID, string sender)
        {
            fwdCommandServiceLock.EnterWriteLock();
            try
            {
                fwdCommandService.Add(fwdCommandID, sender);
            }
            finally
            {
                fwdCommandServiceLock.ExitWriteLock();
            }
        }


        private bool IsFwdCommand(string commandID)
        {
            fwdCommandServiceLock.EnterReadLock();
            try
            {
                return fwdCommandService.ContainsKey(commandID);
            }
            finally
            {
                fwdCommandServiceLock.ExitReadLock();    
            }
        
        }

        private void RemoveFwdCommand(string fwdCommandID)
        {
            fwdCommandServiceLock.EnterWriteLock();
            try
            {
                fwdCommandService.Remove(fwdCommandID);
            }
            finally
            {
                fwdCommandServiceLock.ExitWriteLock();
            }
        }

        private string GetService(string fwdCommandID)
        {
            fwdCommandServiceLock.EnterReadLock();
            try
            {
                return fwdCommandService[fwdCommandID];
            }
            finally
            {
                fwdCommandServiceLock.ExitReadLock();
            }

        }

        #endregion

    }
}


//string finalDestinationName = GetDestinationServiceName(executionResult.ResultID);

/*ForwardedCommand fwdCommand = new ForwardedCommand();
fwdCommand.Component = originalCommand;
fwdCommand.ForwardingKey = finalDestinationName;
fwdCommand.RootSender = receiver.BaseForwardAddress + receiver.NodeName;
*/
// FIXME: Temporary code.
//string destinationAddress = receiver.BaseForwardAddress + finalDestinationName;
//Forward(originalCommand, forward.Destination);