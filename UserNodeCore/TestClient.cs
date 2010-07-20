using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using Communication;
using System.Threading;

namespace ClientCore
{

   /* public class ContractClient : DuplexClientBase<IDarPooling>
    { 
        public ContractClient(object callbackInstance, Binding binding, EndpointAddress remoteAddress)
        : base(callbackInstance, binding, remoteAddress) { }
    }

    public class CallbackClient : IDarPoolingCallback
    {
        public void OnCallback()
        {
            Console.WriteLine("Hi from client!");
        }
    }
  */


    class Program
    {
        static void Main(string[] args)
        {
            /* Compact
            IDarPooling client = ChannelFactory<IDarPooling>.CreateChannel(
                                 new WSHttpBinding(), new EndpointAddress(
                                 "http://localhost:1111/Roma/"
                                 ));
            */
            /* Verbose */
            EndpointAddress address = new EndpointAddress("http://localhost:1111/Milano");
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IDarPooling> channelFactory = new ChannelFactory<IDarPooling>(binding);
            IDarPooling client2 = channelFactory.CreateChannel(address);
           
           /* WSDualHttpBinding binding = new WSDualHttpBinding();
            var callback = new CallbackClient();
            var client3 = new ContractClient(callback, binding, new EndpointAddress(""));
            var proxy = client3.ChannelFactory.CreateChannel();
            proxy.SayHello(); 
            */
            //Thread.Sleep(7000);
            Console.WriteLine("*****  Test Client  *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();
            
            
            //client.SayHello();
            Console.WriteLine();
            Console.WriteLine(client2.SayHello());
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            Console.ReadLine();


        }
    }
}