using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication
{

    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void Notify(string value);
    }

    [ServiceContract(CallbackContract = typeof(ICallback), SessionMode=SessionMode.Required)]
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
