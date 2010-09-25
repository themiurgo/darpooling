using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication;
using System.ServiceModel;
using System.Security.Cryptography;

namespace UserNodeCore
{
    public interface IState
    {
        bool RegisterUser(UserNodeCore context, string username, string pw_hash);
        bool Join(UserNodeCore context, string username, string password, string serviceNodeAddress, string callbackAddress);
        bool Unjoin(UserNodeCore context);
        bool InsertTrip(UserNodeCore context, Communication.Trip trip);
        bool SearchTrip(UserNodeCore context);
    }

    /// <summary>
    /// The initial state of any UserNode.
    /// 
    /// From here you can just:
    /// - Register a new User
    /// - Join (login) the network
    /// </summary>
    public class UnjointState : IState
    {
        public bool RegisterUser(UserNodeCore context, string username,
            string pw_hash)
        {
            Command c = new Communication.RegisterUserCommand();
            return true;
        }

        public bool Join(UserNodeCore context, string username, string password,
            string serviceNodeAddress, string callbackAddress)
        {
            try
            {
                ClientCallback callback = new ClientCallback(context);

                // First of all, set up the connection
                EndpointAddress endPointAddress = new EndpointAddress(serviceNodeAddress);
                WSDualHttpBinding binding = new WSDualHttpBinding();
                binding.ClientBaseAddress = new Uri(callbackAddress);
                DuplexChannelFactory<IDarPooling> factory = new DuplexChannelFactory<IDarPooling>(
                        callback, binding, endPointAddress);
                //Console.WriteLine("[{0}] Ready to invoke service",DateTime.Now.TimeOfDay);
                context.ServiceProxy = factory.CreateChannel();
            }
            catch
            {
                return false;
            }

            // Now, hopefully you have a working ServiceProxy
            // Send JoinCommand and have luck
            string passwordHash = Communication.Tools.HashString(password);
            //Console.WriteLine(passwordHash);
            Command c = new JoinCommand(context.UserNode, username, passwordHash);

            //Console.WriteLine("Press a key to start the communication");
            //Console.ReadLine();
            context.ServiceProxy.HandleDarPoolingRequest(c);

            // Finally, if Join is NOT successfull, remove reference (TODO)
            // context.ServiceProxy = null;
            return true;
        }

        public bool Unjoin(UserNodeCore context)
        {
            // Not Possible
            return false;
        }

        public bool InsertTrip(UserNodeCore context, Communication.Trip trip)
        {
            // Not possible
            return false;
        }

        public bool SearchTrip(UserNodeCore context)
        {
            // Not possible
            return false;
        }
    }

    /// <summary>
    /// The connected state of a UserNode.
    /// 
    /// From here you can:
    /// - Unjoin (disconnect) from the network
    /// - Insert a new trip
    /// - Search trips
    /// </summary>
    public class JointState : IState
    {
        public bool Unjoin(UserNodeCore context)
        {
            Command command = new UnjoinCommand(context.UserNode.User.UserName);
            context.ServiceProxy.HandleDarPoolingRequest(command);
            context.State = new UnjointState();
            
            return true;
        }

        public bool RegisterUser(UserNodeCore context, string username, string pw_hash)
        {
            return false;
        }

        public bool Join(UserNodeCore context, string username, string password,
            string serviceNodeAddress, string callbackAddress)
        {
            return false;
        }

        public bool InsertTrip(UserNodeCore context, Communication.Trip trip)
        {
            Command command = new InsertTripCommand(trip);
            context.ServiceProxy.HandleDarPoolingRequest(command);
            // This command does not change state
            return true;
        }

        public bool SearchTrip(UserNodeCore context)
        {
            return true;
        }
    }
}