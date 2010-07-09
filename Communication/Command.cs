using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Communication
{
    public interface ICommand
    {
        void Execute();
    }

    /// <summary>
    /// The command class allows arbitrary commands to be executed.
    /// </summary>
    [DataContract]
    public abstract class Command : ICommand
    {
        private ServiceNode remoteServiceNode;

        public void Execute()
        {
        }
    }

    public class JoinCommand : Command
    {
        private UserNode node;

        public JoinCommand(UserNode node)
        {
            this.node = node;
        }

        public void Execute()
        {

        }
    }
    public class UnjoinCommand : Command { }
    public class RegisterUserCommand : Command { }
    public class LoginUserCommand : Command { }
    public class LogoutUserCommand : Command { }
    public class InsertTripCommand : Command { }
    public class SearchTripCOmmand : Command { }
}