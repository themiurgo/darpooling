using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Communication;
using System.Threading;
using System.ServiceModel;

namespace ServiceNodeCore
{
    /// <summary>
    /// This class implements the interface of the Darpooling service
    /// </summary>
    public class DarPoolingService : IDarPooling
    {
        public void SendCommand(Communication.Command command)
        {

        }

        public Communication.Result GetResult()
        {
            return new Result("");
        }

        public void GetData(string value)
        {
            Console.WriteLine("I've received the following Request: {0}", value);
            //User[] users = new User[] { new User("Daniele"), new User("Antonio") };
            Console.WriteLine("Satisfying Client Request...");
            Result res = new Result("There are 5 Trips available");
            Thread.Sleep(4000);
            Console.WriteLine("Ready to send data back to Client...");
            OperationContext.Current.GetCallbackChannel<IDarPoolingCallback>().GetResult(res);
        }

    }
}
