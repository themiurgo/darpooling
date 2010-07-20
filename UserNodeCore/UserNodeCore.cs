using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Communication;
//using Client.ServiceRef;

namespace Client
{
    /// <summary>
    /// UserNodeCore class is composed of informations of UserNode plus the status
    /// of its connection. Also, it allows to execute actions that will have
    /// consequences on both the UserNode and the Darpooling network.
    /// </summary>
    public class UserNodeCore
    {
        private UserNode myNode;
        private IState state;

        /// <summary>
        /// Setup a new UserNodeCore.
        /// </summary>
        /// <param name="clientNode">represents the UserNode and its settings.</param>
        public UserNodeCore(UserNode clientNode)
        {
            state = new DisconnectedState();
            myNode = clientNode;
        }

        public UserNode UserNode
        {
            get { return myNode; }
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
            state.Join(this, serviceNodeAddress);
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
            state.LoginUser(this, username, pw_hash);
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