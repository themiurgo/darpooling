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
    /// information and obtain data that belong to other nodes.
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

        // The numeric identifier for a received Command.
        private int commandCounter;

        // This dictionary let DarPoolingService to identify the client
        // that has sent a particular command
        private Dictionary<int, IDarPoolingCallback> commandClient;
        private ReaderWriterLockSlim commandClientLock;
        
        // Keep track of the currently joined users.
        private List<string> joinedUsersList;
        private ReaderWriterLockSlim joinedUsersListLock;

        // Contain the pair <id of request to be forwarded, address of the destination node>
        private Dictionary<int, string> forwardDestination;
        private ReaderWriterLockSlim forwardDestinationLock;


        /// <summary>
        /// FIXME: The default constructor has been marked as private only to
        /// be aware of attemps to instantiate this class (WCF mess).
        /// </summary>
        private DarPoolingService() { }


        public DarPoolingService(ServiceNodeCore receiver)
        {
            this.receiver = receiver;
            
            commandCounter = -1;
            commandClient = new Dictionary<int, IDarPoolingCallback>();
            commandClientLock = new ReaderWriterLockSlim();
            
            joinedUsersList = new List<string>();
            joinedUsersListLock = new ReaderWriterLockSlim();

            forwardDestination = new Dictionary<int, string>();
            forwardDestinationLock = new ReaderWriterLockSlim();
        }




        /// <summary>
        /// HandleUser is one of the method of the IDarpooling interface.
        /// It receives a Command which deals with User management. 
        /// After setting some command paramenters as specified below,
        /// it invokes the Execute() method on the command itself.
        /// </summary>
        /// <param name="command">The Command sent by a client</param>
        public void HandleUser(Command command)
        {
            Console.Write("{0} node has received a HandleUser request. Processing... ", receiver.NodeName.ToUpper());

            // Assign an ID to the command, for later use; use the atomic sum.
            Interlocked.Add(ref commandCounter, 1);
            command.CommandID = commandCounter;

            // Save information about the client that has sent the command
            IDarPoolingCallback client = OperationContext.Current.GetCallbackChannel<IDarPoolingCallback>();
            commandClient.Add(command.CommandID, client);

            // Set a ServiceNodeCore as the receiver of the command;
            command.Receiver = receiver;
            
            /// Set the callback method, i.e. the method that will be
            /// invoked when the receiver finishes to compute the result
            command.Callback = new AsyncCallback(ReturnUserResult);

            // Invoke the Execute() method of the command
            command.Execute();

            // DarPoolingService can now return to listen incoming request, while the secondary thread
            // started by Execute() performs the necessary operation
            Console.WriteLine("Done!");
        }


        /// <summary>
        /// TODO: to be implemented
        /// </summary>
        /// <param name="tripCommand"></param>
        public void HandleTrip(Command tripCommand)
        {

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
        public void ReturnUserResult(IAsyncResult iAsyncResult)
        {
            // Used to store the Result of a particular command
            Result executionResult;

            // Retrieve the command which started the request
            Command originalCommand = (Command)iAsyncResult.AsyncState;

            // Obtain the Result of the command
            executionResult = originalCommand.EndExecute(iAsyncResult);

            
            // The command must be forwarded
            if (IsForwardRequired(executionResult.ResultID))
            {

                Console.Write("{0} node could not satisfy the client request. Preparing to forward...", receiver.NodeName);

                string destinationAddress = GetForwardDestinationAddress(executionResult.ResultID);
                originalCommand.RootSender = receiver.BaseForwardAddress + receiver.NodeName;

                // Get ready to call the remote node service via the dedicated forward endpoint address.
                BasicHttpBinding fwdBinding = new BasicHttpBinding();
                EndpointAddress fwdEndpoint = new EndpointAddress(destinationAddress);
                ChannelFactory<IDarPoolingForwarding> forwardChannelFactory = new ChannelFactory<IDarPoolingForwarding>(fwdBinding, fwdEndpoint);
                IDarPoolingForwarding destinationService = forwardChannelFactory.CreateChannel();

                // Forward the Command to the remote note, using the IDarPoolingForwarding interface.
                destinationService.HandleForwardedUserCommand(originalCommand);
                
                // Close the channels. The communication is fire-and-forget (one-way)
                ((IClientChannel)destinationService).Close();
                forwardChannelFactory.Close();

                Console.WriteLine("Forwarded!");

            }
            // The Execution was successfull, i.e. the requested data were in this node.
            else
            {

                Console.Write("Client request n° {0} has been completed. Sending the result to Client...", originalCommand.CommandID);

                // Retrieve the Client who sent the command and send it the Result
                commandClient[originalCommand.CommandID].GetResult(executionResult);

                
                if (executionResult is LoginOkResult)
                {
                    JoinCommand jc = (JoinCommand)originalCommand;
                    AddJoinedUser(jc.UserName);
                }

                // Delete the client from the DarPoolingService cache and close the channel
                IDarPoolingCallback client = GetSenderClient(originalCommand.CommandID);
                commandClient.Remove(originalCommand.CommandID);
                ((IClientChannel)client).Close();

                Console.WriteLine("Done!");
            
            }
            
        }


        /// <summary>
        /// IDarPoolingForwarding method. The user-related command uses only one hop,
        /// i.e. they always reach the correct and final destination node with only
        /// one connession. For these reasons, we only have to execute the command,
        /// and the return the result to the rootSender service node.
        /// </summary>
        /// <param name="forwardedCommand"></param>
        public void HandleForwardedUserCommand(Command forwardedCommand) 
        {
            // Set a ServiceNodeCore as the receiver of the command;
            forwardedCommand.Receiver = receiver;

            /// Set the callback method, i.e. the method that will be
            /// invoked when the receiver finishes to compute the result
            forwardedCommand.Callback = new AsyncCallback(ReturnForwardResult);

            // Invoke the Execute() method of the command
            forwardedCommand.Execute();
        }


        public void ReturnForwardResult(IAsyncResult iAsyncResult)
        {
            // Used to store the Result of a particular command
            Result forwardResult;

            // Retrieve the command which started the request
            Command originalCommand = (Command)iAsyncResult.AsyncState;

            // Obtain the Result of the command
            forwardResult = originalCommand.EndExecute(iAsyncResult);

            Console.Write("Forwarded request n° {0} has been completed. Sending the result back to ServiceNode...", originalCommand.CommandID);

            // Get ready to contact the RootSender service node.
            BasicHttpBinding myBinding = new BasicHttpBinding();
            EndpointAddress myEndpoint = new EndpointAddress(originalCommand.RootSender);
            ChannelFactory<IDarPoolingForwarding> myChannelFactory = new ChannelFactory<IDarPoolingForwarding>(myBinding, myEndpoint);
            IDarPoolingForwarding client = myChannelFactory.CreateChannel();

            // Give the result back to the rootsender node.
            client.ForwardedUserCommandResult(originalCommand, forwardResult);
            
            // Close channels
            ((IClientChannel)client).Close();
            myChannelFactory.Close();
            
            }


        /// <summary>
        /// IDarPoolingForwarding method. The service node obtain the result of the forwarded command.
        /// </summary>
        /// <param name="forwardedCommand"></param>
        /// <param name="finalResult"></param>
        public void ForwardedUserCommandResult(Command forwardedCommand, Result finalResult) {
            Console.WriteLine("Received answer for command {0} , that is: {1}", forwardedCommand.CommandID, finalResult.Comment);

            // FIXME: temporary code. Need to find a solution to this problem
            if (finalResult is LoginOkResult)
            {
                JoinCommand jc = (JoinCommand)forwardedCommand;
                AddJoinedUser(jc.UserName);
            }

            // Retrieve the client that originates the request
            IDarPoolingCallback client = GetSenderClient(forwardedCommand.CommandID);
            commandClient.Remove(forwardedCommand.CommandID);

            // Sent the result back to the client.
            client.GetResult(finalResult);
            ((IClientChannel)client).Close();

        }


        #region Collections

        // Add an user in the list of joined user
        public void AddJoinedUser(string username)
        {
            //Obtain the write lock   
            joinedUsersListLock.EnterWriteLock();
            try
            {
                Console.WriteLine("{0} added to joinedUsers of {1}", username, receiver.NodeName);
                joinedUsersList.Add(username);
            }
            finally
            {
                joinedUsersListLock.ExitWriteLock();
                //Console.WriteLine("{0} thread releases the write lock in AddJoinedUser()", Thread.CurrentThread.Name);
            }
        }


        // Remove an user from the list of joined user
        public void RemoveJoinedUser(string username)
        {
            //Obtain the write lock of the list
            joinedUsersListLock.EnterWriteLock();
            try
            {
                //Console.WriteLine("{0} thread obtains the write lock in RemoveJoinedUser()", Thread.CurrentThread.Name);
                if (joinedUsersList.Remove(username))
                {
                    //Console.WriteLine("User removed from the joined list");
                }

            }
            finally
            {
                joinedUsersListLock.ExitWriteLock();
                //Console.WriteLine("{0} thread releases the write lock in RemoveJoinedUser()", Thread.CurrentThread.Name);
            }
        }


        // Check if a user with the given username is a 
        // joined client
        public bool IsJoinedUser(string username)
        { 
            joinedUsersListLock.EnterReadLock();
            try
            {
                return joinedUsersList.Contains(username);
            }
            finally
            {
                joinedUsersListLock.ExitReadLock();
            }

        }


        public void AddForwardingRequest(int id, string destinationAddress)
        {
            forwardDestinationLock.EnterWriteLock();
            try
            {
                forwardDestination.Add(id, destinationAddress);
            }
            finally
            {
                forwardDestinationLock.ExitWriteLock();
            }
        
        }

        private bool IsForwardRequired(int resultID)
        {
            forwardDestinationLock.EnterReadLock();
            try
            {
                return forwardDestination.ContainsKey(resultID);
            }
            finally
            {
                forwardDestinationLock.ExitReadLock();    
            }
        
        }

        private string GetForwardDestinationAddress(int resultID)
        {
            forwardDestinationLock.EnterWriteLock();
            try
            {
                string destinationAddress = forwardDestination[resultID];
                forwardDestination.Remove(resultID);
                return destinationAddress;
            }
            finally
            {
                forwardDestinationLock.ExitWriteLock();
            }
        
        }

        private void AddCommandClient(int commandID, IDarPoolingCallback client)
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

        private void RemoveCommandClient(int commandID)
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

        private IDarPoolingCallback GetSenderClient(int commandID)
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

        #endregion

    }
}