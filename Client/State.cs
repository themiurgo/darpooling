using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication;

namespace Client
{
    public interface State
    {
        bool Join(string address);
        bool Unjoin();
        bool NewTrip(Communication.Trip trip);
        bool SearchTrip();
    }

    public class DisconnectedState : State
    {
        public bool Join(string address)
        {
            // Connect operations
            return true;
        }

        public bool Unjoin()
        {
            return false;
        }

        public bool NewTrip(Communication.Trip trip)
        {
            return false;
        }

        public bool SearchTrip()
        {
            return false;
        }
    }

    public class ConnectedState : State
    {
        public bool Join(string address)
        {
            return false;
        }

        public bool Unjoin()
        {
            return true;
       }

        public bool NewTrip(Communication.Trip Trip)
        {
            return true;
        }

        public bool SearchTrip()
        {
            return true;
        }
    }
}