using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication
{
    /// <summary>
    /// This is the Callback interface.
    /// The Client MUST implement it.
    /// </summary>
    public interface IDarPoolingCallback
    {
        [OperationContract(IsOneWay = true)]
        void Notify(string value);

        [OperationContract(IsOneWay = true)]
        void GetUsers(SimpleUser[] result);

    }

    [ServiceContract(CallbackContract = typeof(IDarPoolingCallback), SessionMode=SessionMode.Required)]
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

        [OperationContract(IsOneWay = true)]
        void GetData(string value);

    }

}
