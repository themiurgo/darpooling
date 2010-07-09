using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication
{
    // NOTE: If you change the interface name "IService1" here, you must also update the reference to "IService1" in Web.config.
    //[ServiceContract(CallbackContract=typeof(IDarPoolingCallback))]
    [ServiceContract]
    public interface IDarPooling
    {
        [OperationContract]
        void SendCommand(Command command);

        [OperationContract]
        Result GetResult();

        [OperationContract]
        SimpleUser[] GetSimpleUsers(SimpleUser[] inputUsers);

        [OperationContract]
        string SayHello();

    }

    interface IDarPoolingCallback
    {
        [OperationContract]
        void OnCallback();
    }


}
