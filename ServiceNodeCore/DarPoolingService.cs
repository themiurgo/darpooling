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

        // Keep track of the currently joined users.
        private List<string> joinedUsersList;
        private ReaderWriterLockSlim joinedUsersListLock;
        
        // This dictionary let DarPoolingService to identify the client
        // that has sent a particular command
        private Dictionary<string, IDarPoolingCallback> commandClient;
        private ReaderWriterLockSlim commandClientLock;

        // Keep track of the received forwarded commands.
        private Dictionary<string, string> fwdCommandSender;
        private ReaderWriterLockSlim fwdCommandSenderLock;
        


        // Contain the pair <id of request to be forwarded, address of the destination node>
        //private Dictionary<int, string> forwardDestination;
        //private ReaderWriterLockSlim forwardDestinationLock;


        /// <summary>
        /// FIXME: The default constructor has been marked as private only to
        /// be aware of attemps to instantiate this class (WCF mess).
        /// </summary>
        private DarPoolingService() { }


        public DarPoolingService(ServiceNodeCore receiver)
        {
            this.receiver = receiver;
            
            commandCounter = -1;
            commandClient = new Dictionary<string, IDarPoolingCallback>();
            commandClientLock = new ReaderWriterLockSlim();
            
            joinedUsersList = new List<string>();
            joinedUsersListLock = new ReaderWriterLockSlim();

            fwdCommandSender = new Dictionary<string, string>();
            fwdCommandSenderLock = new ReaderWriterLockSlim();


            //forwardDestination = new Dictionary<int, string>();
            //forwardDestinationLock = new ReaderWriterLockSlim();
        }


        public string generateGUID(Type objType)
        { 
            Interlocked.Add(ref commandCounter, 1);
            string baseString = receiver.BaseForwardAddress + receiver.NodeName +
                                DateTime.Now.ToString() + commandCounter + objType.ToString();
            return Tools.HashString(baseString);
        }

        /// <summary>
        /// Method of the IDarpooling interface.
        /// It receives a Command which deals with User management. 
        /// After setting some command paramenters as specified below,
        /// it invokes the Execute() method on the command itself.
        /// </summary>
        /// <param name="command">The Command object, sent by a client</param>
        public void HandleUser(Command command)
        {
            Console.WriteLine("\n{0} node has received a HandleUser request. Processing... ", receiver.NodeName.ToUpper());

            // Assign an ID to the command, for later use; use the atomic sum.

            command.CommandID = generateGUID(command.GetType());

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

            //Console.WriteLine("Listening for incoming requests...\n");
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

            ForwardRequiredResult forward = executionResult as ForwardRequiredResult;
            
            // The command must be forwarded
            if ( forward != null)
            {
                Console.Write("{0} node could not satisfy the client request. Preparing to forward...", receiver.NodeName);

                // Get ready to call the remote node service via the dedicated forward endpoint address.
                BasicHttpBinding fwdBinding = new BasicHttpBinding();
                EndpointAddress fwdEndpoint = new EndpointAddress(forward.Destination);
                ChannelFactory<IDarPoolingForwarding> forwardChannelFactory = new ChannelFactory<IDarPoolingForwarding>(fwdBinding, fwdEndpoint);
                IDarPoolingForwarding destinationService = forwardChannelFactory.CreateChannel();

                string senderAddress = receiver.BaseForwardAddress + receiver.NodeName;
                // Forward the Command to the remote note, using the IDarPoolingForwarding interface.
                destinationService.HandleForwardedUserCommand(originalCommand, senderAddress);

                // Close the channels. The communication is fire-and-forget (one-way)
                ((IClientChannel)destinationService).Close();
                forwardChannelFactory.Close();

                Console.WriteLine("Forwarded!");
                
                //string finalDestinationName = GetDestinationServiceName(executionResult.ResultID);

                /*ForwardedCommand fwdCommand = new ForwardedCommand();
                fwdCommand.Component = originalCommand;
                fwdCommand.ForwardingKey = finalDestinationName;
                fwdCommand.RootSender = receiver.BaseForwardAddress + receiver.NodeName;
                */
                // FIXME: Temporary code.
                //string destinationAddress = receiver.BaseForwardAddress + finalDestinationName;
                //Forward(originalCommand, forward.Destination);


            }
            // The Execution was successfull, i.e. the requested data were in this node.
            else
            {

                // Apply changes on the service
                RegisterResult(executionResult);

                Console.Write("Client request n° {0} has been completed. Sending the result to Client...", originalCommand.CommandID);

                // Retrieve the Client who sent the command and send it the Result
                commandClient[originalCommand.CommandID].GetResult(executionResult);

                // Delete the client from the DarPoolingService cache and close the channel
                IDarPoolingCallback client = GetSenderClient(originalCommand.CommandID);
                commandClient.Remove(originalCommand.CommandID);
                ((IClientChannel)client).Close();

                Console.WriteLine("Done!");
            
            }
            
        }

        /*
        public void Forward(Command fwdCommand, string destinationAddress)
        {


        
        }*/


        /// <summary>
        /// IDarPoolingForwarding method. The user-related command uses only one hop,
        /// i.e. they always reach the correct and final destination node with only
        /// one connession. For these reasons, we only have to execute the command,
        /// and the return the result to the rootSender service node.
        /// </summary>
        /// <param name="forwardedCommand"></param>
        public void HandleForwardedUserCommand(Command fwdCommand, string senderAddress)

        {
            Console.WriteLine("Forwarded request received!!  Info:");
            Console.WriteLine("RootSender: {0}", senderAddress);

            AddFwdCommandSender(fwdCommand.CommandID, senderAddress);

 
            // Set a ServiceNodeCore as the receiver of the command;
            fwdCommand.Receiver = receiver;

            /// Set the callback method, i.e. the method that will be
            /// invoked when the receiver finishes to compute the result
            fwdCommand.Callback = new AsyncCallback(ReturnForwardResult);

            // Invoke the Execute() method of the command
            fwdCommand.Execute();
 
        }


        public void ReturnForwardResult(IAsyncResult iAsyncResult)
        {
            // Used to store the Result of a particular command
            Result forwardResult;

            // Retrieve the command which started the request
            Command originalCommand = (Command)iAsyncResult.AsyncState;

            // Obtain the Result of the command
            forwardResult = originalCommand.EndExecute(iAsyncResult);


            string sender = GetSenderService(originalCommand.CommandID);


            Console.WriteLine("Class is {0}", originalCommand.GetType());

            Console.Write("Forwarded request n° {0} has been completed. Sending the result back to ServiceNode :  {1}", originalCommand.CommandID, sender);
            
            // Get ready to contact the RootSender service node.
            BasicHttpBinding myBinding = new BasicHttpBinding();
            EndpointAddress myEndpoint = new EndpointAddress(sender);
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
        public void ForwardedUserCommandResult(Command fwdCommand, Result finalResult)
        {
            Console.WriteLine("Received answer for command {0} , that is: {1}", fwdCommand.CommandID, finalResult.Comment);

            if (IsFwdCommand(fwdCommand.CommandID))
            {
                string sender = GetSenderService(fwdCommand.CommandID);

                // Get ready to contact the RootSender service node.
                BasicHttpBinding myBinding = new BasicHttpBinding();
                EndpointAddress myEndpoint = new EndpointAddress(sender);
                ChannelFactory<IDarPoolingForwarding> myChannelFactory = new ChannelFactory<IDarPoolingForwarding>(myBinding, myEndpoint);
                IDarPoolingForwarding client = myChannelFactory.CreateChannel();

                // Give the result back to the rootsender node.
                client.ForwardedUserCommandResult(fwdCommand, finalResult);

                // Close channels
                ((IClientChannel)client).Close();
                myChannelFactory.Close();


            }
            else
            {


                // Apply changes on the service
                RegisterResult(finalResult);

                Console.Write("Client request n° {0} has been completed. Sending the result to Client...", fwdCommand.CommandID);

                // Retrieve the Client who sent the command and send it the Result
                //commandClient[fwdRequest.CommandID].GetResult(executionResult);

                // Delete the client from the DarPoolingService cache and close the channel
                IDarPoolingCallback client = GetSenderClient(fwdCommand.CommandID);
                commandClient.Remove(fwdCommand.CommandID);
                client.GetResult(finalResult);
                ((IClientChannel)client).Close();

                Console.WriteLine("Done!");

            }
            
            /*

            // Retrieve the client that originates the request
            IDarPoolingCallback client = GetSenderClient(forwardedCommand.CommandID);
            commandClient.Remove(forwardedCommand.CommandID);

            // Sent the result back to the client.
            client.GetResult(finalResult);
            ((IClientChannel)client).Close();
            */
        }


        private void RegisterResult(Result commandResult)
        {
            LoginOkResult login = commandResult as LoginOkResult;
            if (login != null)
            {
                AddJoinedUser(login.AuthorizedUsername);
            }
        
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
                    Console.WriteLine("User {0}  removed from the joined list", username);
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

        /*
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
        */
        private bool IsFwdCommand(string commandID)
        {
            fwdCommandSenderLock.EnterReadLock();
            try
            {
                return fwdCommandSender.ContainsKey(commandID);
            }
            finally
            {
                fwdCommandSenderLock.ExitReadLock();    
            }
        
        }
        /*
        private string GetDestinationServiceName(int resultID)
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
        
        }*/

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

        private void RemoveCommandClient(string commandID)
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

        private IDarPoolingCallback GetSenderClient(string commandID)
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



        private void AddFwdCommandSender(string commandID, string sender)
        {
            fwdCommandSenderLock.EnterWriteLock();
            try
            {
                fwdCommandSender.Add(commandID, sender);
            }
            finally
            {
                fwdCommandSenderLock.ExitWriteLock();
            }

        }

        private void RemoveFwdCommandSender(string commandID)
        {
            fwdCommandSenderLock.EnterWriteLock();
            try
            {
                fwdCommandSender.Remove(commandID);
            }
            finally
            {
                fwdCommandSenderLock.ExitWriteLock();
            }

        }

        private string GetSenderService(string commandID)
        {
            fwdCommandSenderLock.EnterReadLock();
            try
            {
                return fwdCommandSender[commandID];
            }
            finally
            {
                fwdCommandSenderLock.ExitReadLock();
            }

        }

        #endregion

    }
}