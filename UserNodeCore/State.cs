using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Communication;
using System.ServiceModel;
using System.Security.Cryptography;

namespace UserNodeCore
{
    /// <summary>
    /// Interface of a UserNode state.
    /// </summary>
    public interface IState
    {
        bool RegisterUser(UserNodeCore context, User user, string serviceNodeAddress, string callbackAddress);
        bool Join(UserNodeCore context, string username, string password, string serviceNodeAddress, string callbackAddress);
        bool Unjoin(UserNodeCore context);
        bool InsertTrip(UserNodeCore context, Communication.Trip trip);
        bool SearchTrip(UserNodeCore context, QueryBuilder qb);
    }

    /// <summary>
    /// Default implementation of a UserNode state.
    /// Derived class should override supported methods.
    /// </summary>
    public abstract class State : IState
    {
        public virtual bool RegisterUser(UserNodeCore context, User user, string serviceNodeAddress, string callbackAddress) { return false; }
        public virtual bool Join(UserNodeCore context, string username, string password, string serviceNodeAddress, string callbackAddress) { return false; }
        public virtual bool Unjoin(UserNodeCore context) { return false; }
        public virtual bool InsertTrip(UserNodeCore context, Communication.Trip trip) { return false; }
        public virtual bool SearchTrip(UserNodeCore context, QueryBuilder qb) { return false; }
    }

    /// <summary>
    /// The initial state of any UserNode.
    /// 
    /// From here you can just:
    /// - Register a new User
    /// - Join (login) the network
    /// </summary>
    public class UnjointState : State
    {
        public override bool RegisterUser(UserNodeCore context, User user,
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
                context.ServiceProxy = factory.CreateChannel();
            }
            catch
            {
                context.ServiceProxy = null;
                return false;
            }

            Command c = new Communication.RegisterUserCommand(user);

            try
            {
                context.ServiceProxy.HandleDarPoolingRequest(c);
            }
            catch (TimeoutException e)
            {
                context.ServiceProxy = null;
                throw e;
            }
            return true;
        }

        public override bool Join(UserNodeCore context, string username, string password,
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
                context.ServiceProxy = factory.CreateChannel();
            }
            catch
            {
                context.ServiceProxy = null;
                return false;
            }

            // Now, hopefully you have a working ServiceProxy.
            string passwordHash = Communication.Tools.HashString(password);
            Command c = new JoinCommand(context.UserNode, username, passwordHash);

            try
            {
                context.ServiceProxy.HandleDarPoolingRequest(c);
            }
            catch (TimeoutException e)
            {
                context.ServiceProxy = null;
                throw e;
            }

            // Finally, if Join is NOT successfull, remove reference (UserNodeCore.onResultReceive)
            return true;
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
    public class JointState : State
    {
        public override bool Unjoin(UserNodeCore context)
        {
            Command command = new UnjoinCommand(context.UserNode.User.UserName);
            context.ServiceProxy.HandleDarPoolingRequest(command);
            context.State = new UnjointState();
            context.ServiceProxy = null;
            
            return true;
        }

        public override bool RegisterUser(UserNodeCore context, User user,
            string serviceNodeAddress, string callbackAddress)
        {
            return false;
        }

        public override bool InsertTrip(UserNodeCore context, Communication.Trip trip)
        {
            Command command = new InsertTripCommand(trip);
            context.ServiceProxy.HandleDarPoolingRequest(command);
            // This command does not change state
            return true;
        }

        public override bool SearchTrip(UserNodeCore context, QueryBuilder qb)
        {
            Command command = new SearchTripCommand(qb);
            context.ServiceProxy.HandleDarPoolingRequest(command);
            return true;
        }
    }
}