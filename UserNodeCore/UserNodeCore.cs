using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using Communication;
using System.Threading;

namespace UserNodeCore
{
    /// <summary>
    /// This class implements the Callback interface, i.e. the set
    /// of methods that the service will call back when the result
    /// is ready.
    /// </summary>
    public class ClientCallback : IDarPoolingCallback
    {
        public void GetResult(Result result)
        {
            Console.WriteLine("Service says: " + result.Comment);
        }

        public void GetUsers(User[] result)
        { 
            Console.WriteLine("These are the users that the Service has returned:");
            foreach (User user in result)
            {
                Console.WriteLine("Name: {0}", user.Name);
            }
        }
    }


    /// <summary>
    /// UserNodeCore class is composed of informations of UserNode plus the status
    /// of its connection. Also, it allows to execute actions that will have
    /// consequences on both the UserNode and the Darpooling network.
    /// </summary>
    public class UserNodeCore
    {
        private UserNode userNode;
        private IState state;

        /* Service settings */
        EndpointAddress serviceAddress;
        WSDualHttpBinding binding;
        IDarPoolingCallback callback;
        DuplexChannelFactory<IDarPooling> factory;
        IDarPooling serviceProxy;
 
        //private Uri serviceAddress;
        //private Uri callbackAddress;

        /// <summary>
        /// Setup a new UserNodeCore.
        /// </summary>
        /// <param name="clientNode">represents the UserNode and its settings.</param>
        public UserNodeCore(UserNode clientNode)
        {
            state = new UnjointState();
            userNode = clientNode;
            Initialize();
        }

        private void Initialize()
        {
            /* Address */
            serviceAddress = new EndpointAddress("http://localhost:1111/" + userNode.UserLocationName);
            /* Binding */
            binding = new WSDualHttpBinding();
            binding.ClientBaseAddress = new Uri("http://localhost:2222/" + userNode.NodeName); //Callback address
            /* (Callback) contract  */
            callback = new ClientCallback();
            /** Channels */
            factory = new DuplexChannelFactory<IDarPooling>(callback, binding, serviceAddress);
            serviceProxy = factory.CreateChannel();
        }

        public void ConnectToService(Command c)
        {
            //serviceProxy.GetData(userNode.User);
            //Command c2 = new Command();
            //Command c1 = new Command();
            serviceProxy.HandleUser(c);
        }

        public UserNode UserNode
        {
            get { return userNode; }
            set { }
        }

        /// <summary>
        /// Represent the state of the connection to the network, according to
        /// State pattern.
        /// </summary>
        public IState State
        {
            get { return state; }
            set { state = value; }
        }
        
        /// <summary>
        /// Join (connect) to the network, through a ServiceNode.
        /// </summary>
        /// <param name="serviceNodeAddress">address of the ServiceNode</param>
        public void Join(string serviceNodeAddress)
        {
            // PUT JOIN HERE.
        }

        /// <summary>
        /// Unjoin (disconnect) from the network.
        /// </summary>
        public void Unjoin()
        {
            state.Unjoin(this);
        }

        public void LoginUser(string username, string pw_hash)
        {
            state.Join(this, username, pw_hash);
        }

        public void InsertTrip(Trip trip)
        {
            state.InsertTrip(this, trip);
        }

        public void SearchTrip()
        {
            state.SearchTrip(this);
        }
    }
}