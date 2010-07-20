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

    class Program
    {
        static void Main(string[] args)
        {
            NoCallbackHTTPClient();
            NoCallbackTCPClient();
            //CallbackClient();
        }


        static void CallbackClient()
        { 
        
        
        }


        static void NoCallbackTCPClient()
        {
            /* Verbose */
            EndpointAddress address = new EndpointAddress("net.tcp://localhost:1112/Milano");
            NetTcpBinding binding = new NetTcpBinding();
            //WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IDarPooling> channelFactory = new ChannelFactory<IDarPooling>(binding);
            IDarPooling client2 = channelFactory.CreateChannel(address);

            //Thread.Sleep(7000);
            Console.WriteLine("*****  Test TCP Client  *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();

            Console.WriteLine(client2.SayHello());
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            Console.ReadLine();

        }


        static void NoCallbackHTTPClient()
        {
            /* Verbose */
            EndpointAddress address = new EndpointAddress("http://localhost:1111/Milano");
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IDarPooling> channelFactory = new ChannelFactory<IDarPooling>(binding);
            IDarPooling client2 = channelFactory.CreateChannel(address);

            //Thread.Sleep(7000);
            Console.WriteLine("*****  Test HTTP Client  *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();

            Console.WriteLine(client2.SayHello());
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            Console.ReadLine();
        
        }


    } // End Program
} // End Namespace


/* Compact
IDarPooling client = ChannelFactory<IDarPooling>.CreateChannel(
                     new WSHttpBinding(), new EndpointAddress(
                     "http://localhost:1111/Roma/"
                     ));
*/


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

/* WSDualHttpBinding binding = new WSDualHttpBinding();
      var callback = new CallbackClient();
      var client3 = new ContractClient(callback, binding, new EndpointAddress(""));
      var proxy = client3.ChannelFactory.CreateChannel();
      proxy.SayHello(); 
      */