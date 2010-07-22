using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication;
using System.ServiceModel;

namespace Client
{
    public interface IState
    {
        bool RegisterUser(UserNodeCore context, string username, string pw_hash);
        bool Join(UserNodeCore context, string username, string pw_hash);
        bool Unjoin(UserNodeCore context);
        bool InsertTrip(UserNodeCore context, Communication.Trip trip);
        bool SearchTrip(UserNodeCore context);
    }

    public class UnjointState : IState
    {
        public bool RegisterUser(UserNodeCore context, string username, string pw_hash)
        {
            return false;
        }

        public bool Join(UserNodeCore context, string username, string pw_hash)
        {
            return false;
        }

        public bool Unjoin(UserNodeCore context)
        {
            return false;
        }

        public bool InsertTrip(UserNodeCore context, Communication.Trip trip)
        {
            return false;
        }

        public bool SearchTrip(UserNodeCore context)
        {
            return false;
        }
    }

    public class JointState : IState
    {
        public bool Unjoin(UserNodeCore context)
        {
            context.State = new UnjointState();
            return true;
        }

        public bool RegisterUser(UserNodeCore context, string username, string pw_hash)
        {
            return false;
        }

        public bool Join(UserNodeCore context, string username, string pw_hash)
        {
            Console.WriteLine("LOGIN");
            return false;
        }

        public bool InsertTrip(UserNodeCore context, Communication.Trip Trip)
        {
            return true;
        }

        public bool SearchTrip(UserNodeCore context)
        {
            return true;
        }
    }

    public class LoggedState : IState
    {
        public bool Unjoin(UserNodeCore context)
        {
            context.State = new UnjointState();
            return true;
        }

        public bool RegisterUser(UserNodeCore context, string username, string pw_hash)
        {
            return false;
        }

        public bool Join(UserNodeCore context, string username, string pw_hash)
        {
            return false;
        }

        public bool InsertTrip(UserNodeCore context, Communication.Trip Trip)
        {
            return true;
        }

        public bool SearchTrip(UserNodeCore context)
        {
            return true;
        }
    }
}