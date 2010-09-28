using System;
using System.ServiceModel;


namespace Communication
{

    /// <summary>
    /// This is the DarPooling Service interface which is used by clients. 
    /// Also note that the callback interface is specified, so it is used
    /// for two-way communication (non-mobile clients).
    /// </summary>
    [ServiceContract(
        Namespace="http://www.darpooling.org",
        CallbackContract = typeof(IDarPoolingCallback), 
        SessionMode=SessionMode.Required)]
    public interface IDarPooling
    {
        [OperationContract(IsOneWay=true)]
        void HandleDarPoolingRequest(Command command);
    }


    /// <summary>
    /// This is the Callback interface, that must be implemented by the
    /// client in order to receive the Result of the sent Commands.
    /// </summary>
    public interface IDarPoolingCallback
    {
        [OperationContract(IsOneWay = true)]
        void GetResult(Result result);
    }


    /// <summary>
    /// This is the interface of the DarPooling service that will be used by
    /// the mobile clients.
    /// </summary>
    [ServiceContract]
    public interface IDarPoolingMobile { }
}
