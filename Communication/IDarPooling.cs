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
        void GetResult(Result result);
    }

    /// <summary>
    /// This is the Service Interface
    /// </summary>
    [ServiceContract(Namespace="http://www.darpooling.org",CallbackContract = typeof(IDarPoolingCallback), SessionMode=SessionMode.Required)]
    public interface IDarPooling
    {
        [OperationContract(IsOneWay=true)]
        void HandleUser(Command c);

        [OperationContract(IsOneWay=true)]
        void HandleTrip(Command tripCommand);

    }

}//End Namespace

/*
[OperationContract]
string SayHello();
*/