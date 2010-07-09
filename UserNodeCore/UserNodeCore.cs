using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Communication;
using Client.ServiceRef;

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

        public UserNodeCore(UserNode clientNode)
        {
            state = new DisconnectedState();
            myNode = clientNode;
        }

        public IState State
        {
            get { return state; }
            set { state = value; }
        }
        
        public void Join(string address)
        {
            state.Join(this, address);
        }

        public void LoginUser(string username, string pw_hash)
        {
            state.LoginUser(this, username, pw_hash);
        }

        public void Unjoin()
        {
            state.Unjoin(this);
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