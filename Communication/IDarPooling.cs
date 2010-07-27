using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Communication
{
    /// <summary>
    /// This is the Callback interface. The Client MUST implement it.
    /// </summary>
    public interface IDarPoolingCallback
    {
        [OperationContract(IsOneWay = true)]
        void GetUsers(User[] result);

        [OperationContract(IsOneWay = true)]
        void GetResult(Result result);
    }

    /// <summary>
    /// This is the Service Interface
    /// </summary>
    [ServiceContract(Namespace="http://www.darpooling.org",CallbackContract = typeof(IDarPoolingCallback), SessionMode=SessionMode.Required)]
    public interface IDarPooling
    {
        [OperationContract]
        void SendCommand(Command command);

        [OperationContract]
        Result GetResult();

        /// <summary>
        /// This method is used for testing purposes.
        /// </summary>
        /// <param name="value"></param>
        [OperationContract(IsOneWay = true)]
        void GetData(User u);
    }

}//End Namespace

/*
[OperationContract]
string SayHello();
*/