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
    }

    /// <summary>
    /// The command class allows arbitrary commands to be executed.
    /// </summary>
    [DataContract]
    public class Command
    {

    }

    public class JoinCommand : Command { }
    public class UnjoinCommand : Command { }
    public class LoginCommand : Command { }
    public class LogoutCommand : Command { }
    public class InsertTripCommand : Command { }
    public class SearchTripCOmmand : Command { }
}