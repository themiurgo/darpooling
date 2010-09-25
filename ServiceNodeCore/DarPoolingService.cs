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
    /// The first interface, IDarPooling, declares the set of methods that darpooling clients
    /// use to send their requests.
    /// The second interface, IDarPoolingForwarding, is used only by the services to exchange 
    /// information and retrieve results.
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

        private bool debug = true;
        

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
        /// Execute request of clients.
        /// </summary>
        /// <param name="command">The Command object, sent by a client</param>
        public void HandleDarPoolingRequest(Command command)
        {
            if (debug) 
                Console.WriteLine("\n{0} {1} node received {2}",LogTimestamp, receiver.NodeName.ToUpper(),command.GetType());

            /** Assign a GUID to the command. */
            command.CommandID = generateGUID();
            /** Save information about the client that has sent the command. */
            IDarPoolingCallback client = OperationContext.Current.GetCallbackChannel<IDarPoolingCallback>();
            commandClient.Add(command.CommandID, client);
            /** Set a ServiceNodeCore as the receiver of the command and execute the command. */
            command.Timestamp = DateTime.Now;
            command.Receiver = receiver;   
            command.Callback = new AsyncCallback(ProcessResult);
            command.Execute();
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
            if (debug)
                Console.WriteLine("{0} {1} node received FWD_{2}",LogTimestamp, receiver.NodeName.ToUpper(), fwdCommand.GetType().Name);

            AddFwdCommandService(fwdCommand.CommandID, senderAddress);
            fwdCommand.Receiver = receiver;
            fwdCommand.Callback = new AsyncCallback(ProcessResultOfForwardedCommand);
            fwdCommand.Execute();
        }


        /// <summary>
        /// IDarPoolingForwarding method. The service node obtain the result of the forwarded command.
        /// </summary>
        /// <param name="forwardedCommand"></param>
        /// <param name="finalResult"></param>
        public void BackPropagateResult(Result result, Command originalCommand)
        {

            if (IsFwdCommand(originalCommand.CommandID))
            {
                string senderAddress = ExtractService(originalCommand.CommandID);
                // Get ready to contact the sender Service node.
                BasicHttpBinding myBinding = new BasicHttpBinding();
                EndpointAddress myEndpoint = new EndpointAddress(senderAddress);
                ChannelFactory<IDarPoolingForwarding> myChannelFactory = new ChannelFactory<IDarPoolingForwarding>(myBinding, myEndpoint);
                IDarPoolingForwarding service = myChannelFactory.CreateChannel();

                // Give the result back to the the sender Service node.
                service.BackPropagateResult(result, originalCommand);
                // Close channel.
                ((IClientChannel)service).Close();
                myChannelFactory.Close();
            }
            else
            {
                if (debug)
                {
                    TimeSpan totalTime = DateTime.Now.Subtract(originalCommand.Timestamp);
                    Console.WriteLine("{0} Total time for {1}: {2}", LogTimestamp, originalCommand.GetType().Name, totalTime.TotalMilliseconds);
                }
                IDarPoolingCallback client = ExtractClient(originalCommand.CommandID);
                ReturnFinalResult(result, client);
            }
        }




        /// <summary>
        /// Retrieve the result of the execution of a Command. Two alternative possibilities:
        /// 1) Give the result back to client.
        /// 2) Forward the command.
        /// </summary>
        /// <param name="iAsyncResult">Represent the object which is available after the 
        /// asynchronous invocation of Execute(). It gives access to all state information.</param>
        public void ProcessResult(IAsyncResult iAsyncResult)
        {
            /** Retrieve the pair Command-Result. */
            Command command = (Command) iAsyncResult.AsyncState;
            Result result = command.EndExecute(iAsyncResult);

            /** Check if the Command has to be forwarded. */
            ForwardRequiredResult checkForward = result as ForwardRequiredResult;
            
            if (checkForward != null)   //Forward the command
            {
                string service = checkForward.Destination;
                ForwardCommand(command, service);
            }
            else    //Give the result to client
            {
                if (debug)
                {
                    TimeSpan totalTime = DateTime.Now.Subtract(command.Timestamp);
                    Console.WriteLine("{0} Total time for {1}: {2}", LogTimestamp, command.GetType().Name, totalTime.TotalMilliseconds);
                }
                IDarPoolingCallback client = ExtractClient(command.CommandID);
                ReturnFinalResult(result, client);
            }
        }


        public void ProcessResultOfForwardedCommand(IAsyncResult iAsyncResult)
        {
            Command fwdCommand = (Command)iAsyncResult.AsyncState;
            Result result = fwdCommand.EndExecute(iAsyncResult);

            /** Check if the Command has to be forwarded. */
            ForwardRequiredResult checkForward = result as ForwardRequiredResult;

            if (checkForward != null)   //Forward the command
            {
                string service = checkForward.Destination;
                ForwardCommand(fwdCommand, service);
            }
            else
            {

                BackPropagateResult(result, fwdCommand);
            }

        }




        /// <summary>
        /// Forward aCommand to remote service.
        /// </summary>
        /// <param name="command">The Command to be forwarded</param>
        /// <param name="destination">String that represent the address of the target service.</param>
        private void ForwardCommand(Command command, string destination)
        {
            if (debug)
                Console.WriteLine("{0} Forwarding a {1} to {2}",LogTimestamp, command.GetType().Name, destination.Split('/').Last().ToUpper());
            
            /** Invoke the remote node service via the dedicated forward endpoint address. */
            BasicHttpBinding fwdBinding = new BasicHttpBinding();
            EndpointAddress fwdEndpoint = new EndpointAddress(destination);
            ChannelFactory<IDarPoolingForwarding> fwdChannelFactory = new ChannelFactory<IDarPoolingForwarding>(fwdBinding, fwdEndpoint);
            IDarPoolingForwarding destinationService = fwdChannelFactory.CreateChannel();

            string senderAddress = receiver.BaseForwardAddress + receiver.NodeName;
            destinationService.HandleForwardedDarPoolingRequest(command, senderAddress);

            /** Close the channel: the communication is fire-and-forget (one-way) */
            ((IClientChannel)destinationService).Close();
            fwdChannelFactory.Close();
        }


        private void ReturnFinalResult(Result finalResult, IDarPoolingCallback destination)
        {
            /** Apply changes on the service. */
            RegisterResult(finalResult);

            if (debug)
                Console.WriteLine("{0} {1} return a {2}",LogTimestamp,receiver.NodeName.ToUpper(), finalResult.GetType().Name);

            destination.GetResult(finalResult);

            bool closeConnection = IsFinalInteraction(finalResult);
            if (closeConnection)
            {
                Console.WriteLine("{0} End of communication with client.",LogTimestamp);
                ((IClientChannel)destination).Close();
            }

        }


        private bool IsFinalInteraction(Result result)
        {
            LoginErrorResult error = result as LoginErrorResult;
            if (error != null)
                return true;

            UnjoinConfirmedResult unjoin = result as UnjoinConfirmedResult;
            if (unjoin != null)
                return true;

            return false;        
        }
        



        // Generate GUIDs using SHA1
        public string generateGUID()
        {
            // Use atomic sum
            Interlocked.Add(ref commandCounter, 1);
            string baseString = receiver.BaseForwardAddress + receiver.NodeName +
                                DateTime.Now.ToString() + commandCounter;// +objType.ToString();
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


        private string UTC
        {
            get
            {
                DateTime utcTime = DateTime.UtcNow.Date;
                return utcTime.ToString();
            }
        }

        private string LogTimestamp
        {
            get
            {
                string compact = "HH:mm:ss.fff";
                //string full = "yyyy-MM-ddTHH:mm:ss.fff";
                string time = DateTime.Now.ToString(compact);
                return ("[" + time + "]"); 
            }
        }






        #region JoinedUser List Management


        public void AddJoinedUser(string username)
        {
            joinedUserNamesLock.EnterWriteLock();
            try
            {
                if(debug)
                    Console.WriteLine("{0} {1} joined {2}",LogTimestamp, username, receiver.NodeName.ToUpper());
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
                joinedUserNames.Remove(username);
                if (debug)
                    Console.WriteLine("{0} {1} unjoined {2}",LogTimestamp, username, receiver.NodeName.ToUpper());   
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


        private IDarPoolingCallback ExtractClient(string commandID)
        {
            commandClientLock.EnterWriteLock();
            try
            {
                IDarPoolingCallback client = commandClient[commandID];
                commandClient.Remove(commandID);
                return client;
            }
            finally
            {
                commandClientLock.ExitWriteLock();
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

        private string ExtractService(string commandID)
        {
            fwdCommandServiceLock.EnterWriteLock();
            try
            {
                string service = fwdCommandService[commandID];
                fwdCommandService.Remove(commandID);
                return service;
            }
            finally
            {
                fwdCommandServiceLock.ExitWriteLock();
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