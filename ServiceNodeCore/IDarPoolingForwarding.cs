using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Communication;

namespace ServiceNodeCore
{
    /// <summary>
    /// This interface is used by the Service Nodes to pass any command that contains
    /// a request that cannot be satisfied by the current node.
    /// </summary>
    [ServiceContract]
    public interface IDarPoolingForwarding
    {
        [OperationContract(IsOneWay = true)]
        void HandleForwardedUserCommand(Command forwardedCommand);

        [OperationContract(IsOneWay = true)]
        void ForwardedUserCommandResult(Command forwardedCommand, Result finalResult);

    }

}
