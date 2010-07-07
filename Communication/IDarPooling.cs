using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in Web.config.
    [ServiceContract]
    public interface IDarPooling
    {
        [OperationContract]
        SimpleUser[] GetSimpleUsers(SimpleUser[] inputUsers);

        [OperationContract]
        string SayHello();

        
    }

    //Contract for our network. It says we can 'ping'
    [ServiceContract(CallbackContract = typeof(IPeer))]
    public interface IPeer
    {
        [OperationContract(IsOneWay = true)]
        void Ping(string sender, string message);
    }

}