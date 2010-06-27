using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication;

namespace Client
{
    public interface IState
    {
        bool Join(ClientCore context, string address);
        bool RegisterUser(string username, string pw_hash);
        bool LoginUser(string username, string pw_hash);
        bool Unjoin(ClientCore context);
        bool InsertTrip(ClientCore context, Communication.Trip trip);
        bool SearchTrip(ClientCore context);
    }

    public class DisconnectedState : IState
    {
        public bool Join(ClientCore context, string address)
        {
            // Connect operations
            context.State = new ConnectedState();
            return true;
        }

        public bool RegisterUser(string username, string pw_hash)
        {
            return false;
        }

        public bool LoginUser(string username, string pw_hash)
        {
            return false;
        }

        public bool Unjoin(ClientCore context)
        {
            return false;
        }

        public bool InsertTrip(ClientCore context, Communication.Trip trip)
        {
            return false;
        }

        public bool SearchTrip(ClientCore context)
        {
            return false;
        }
    }

    public class ConnectedState : IState
    {
        public bool Join(ClientCore context, string address)
        {
            return false;
        }

        public bool Unjoin(ClientCore context)
        {
            context.State = new DisconnectedState();
            return true;
        }

        public bool RegisterUser(string username, string pw_hash)
        {
            return false;
        }

        public bool LoginUser(string username, string pw_hash)
        {
            return false;
        }

        public bool InsertTrip(ClientCore context, Communication.Trip Trip)
        {
            return true;
        }

        public bool SearchTrip(ClientCore context)
        {
            return true;
        }
    }

    public class LoggedState : IState
    {
        public bool Join(ClientCore context, string address)
        {
            return false;
        }

        public bool Unjoin(ClientCore context)
        {
            context.State = new DisconnectedState();
            return true;
        }

        public bool RegisterUser(string username, string pw_hash)
        {
            return false;
        }

        public bool LoginUser(string username, string pw_hash)
        {
            return false;
        }

        public bool InsertTrip(ClientCore context, Communication.Trip Trip)
        {
            return true;
        }

        public bool SearchTrip(ClientCore context)
        {
            return true;
        }
    }
}