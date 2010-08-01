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
    /// This class implements the IDarpooling interface, i.e. the interface 
    /// of the Darpooling service.
    /// Its main goal is to wait for incoming requests from clients and then
    /// satisfy these requests by using a ServiceNodeCore instance.
    /// When the result of a particular request is ready, DarPoolingService
    /// will call the client and send it back the result.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DarPoolingService : IDarPooling
    {
        // An istance of ServiceNodeCore is the receiver for all
        // client commands.
        private ServiceNodeCore receiver;
        // The numeric identifier for a received command
        private int commandCounter;
        // This dictionary let DarPoolingService to identify the client
        // that has sent a particular command
        private Dictionary<int, IDarPoolingCallback> commandClient;


        /// <summary>
        /// The default constructor has been marked as private to
        /// be aware of attemps to instantiate this class. 
        /// </summary>
        private DarPoolingService() { }


        public DarPoolingService(ServiceNodeCore receiver)
        {
            this.receiver = receiver;
            
            commandCounter = -1;
            commandClient = new Dictionary<int, IDarPoolingCallback>();
        }


        #region IDarpooling implementation

        /// <summary>
        /// HandleUser is one of the method of the IDarpooling interface.
        /// It receives a Command which deals with User management. 
        /// After setting some command paramenters as specified below,
        /// it invokes the Execute() method on the command itself.
        /// </summary>
        /// <param name="command">The Command sent by a client</param>
        public void HandleUser(Command command)
        {
            Console.Write("{0} received a User request. Processing the request... ", receiver.NodeName.ToUpper());

            // Assign an ID to the command, for later use;
            commandCounter++;
            command.CommandID = commandCounter;
            
            // Set a ServiceNodeCore as the receiver of the command;
            command.Receiver = receiver;
            
            /// Set the callback method, i.e. the method that will be
            /// invoked when the receiver finishes to compute the result
            command.Callback = new AsyncCallback(ReturnResult);

            // Invoke the Execute() method of the command
            command.Execute();
            
            // Save information about the client that sent the command
            IDarPoolingCallback client = OperationContext.Current.GetCallbackChannel<IDarPoolingCallback>();
            commandClient.Add(command.CommandID, client);

            Console.WriteLine("Done!");
        }

        public void HandleTrip(Command tripCommand)
        {

        }

        #endregion


        public void ReturnResult(IAsyncResult itfAR)
        {
            // Used to store the Result of a particular command
            Result tempResult;
            
            // Retrieve the command which started the request
            Command originator = (Command)itfAR.AsyncState;
            
            // Obtain the Result of the command
            tempResult = originator.EndExecute(itfAR);
            
            Console.Write("Client request n° {0} has been completed. Sending the result to Client...", originator.CommandID);
            // Retrieve the Client who sent the command
            commandClient[originator.CommandID].GetResult(tempResult);
            Console.WriteLine("Done!");
        }




        public void GetData(User u)
        {
            Console.WriteLine("GetData() on DarPoolingService does nothing...");
            /*
            Result res;
            Console.WriteLine("Satisfying Client Request...");
            if (CheckUser(u.UserName))
            {
                res = new LoginResult("Ok, you can register");
            }
            else
            {
                res = new LoginResult("Sorry, this username is already present!");
            }
            Thread.Sleep(2000);
            Console.WriteLine("Ready to send data back to Client...");
            OperationContext.Current.GetCallbackChannel<IDarPoolingCallback>().GetResult(res);
             */
        }



        

        




    }
}