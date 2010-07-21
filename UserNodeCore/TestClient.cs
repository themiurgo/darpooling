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

    /// <summary>
    /// This class implements the Callback interface, i.e. the set
    /// of methods that the service will call back when the result
    /// is ready.
    /// </summary>
    public class ClientCallback : IDarPoolingCallback
    {
        public void GetResult(Result result)
        {
            Console.WriteLine("Service says: " + result.Comment);
        }

        public void GetUsers(User[] result)
        { 
            Console.WriteLine("These are the users that the Service has returned:");
            foreach (User user in result)
            {
                Console.WriteLine("Name: {0}", user.Name);
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            //NoCallbackHTTPClient();
            //NoCallbackTCPClient();
            CallbackClient();
        }


        static void StartClientNodes()
        { 
            /** 
              * Service communication settings 
              */
            /* Address */
            EndpointAddress serviceAddress = new EndpointAddress("http://localhost:1111/Milano");
            /* Binding */
            WSDualHttpBinding binding = new WSDualHttpBinding();
            binding.ClientBaseAddress = new Uri("http://localhost:2222/Client1"); //Callback address
            /* (Callback) contract  */
            IDarPoolingCallback callback = new ClientCallback();
            /** Channels */
            DuplexChannelFactory<IDarPooling> factory = new DuplexChannelFactory<IDarPooling>(callback, binding, serviceAddress);
            IDarPooling proxy = factory.CreateChannel();

            /* Clients */
            /* Obtain the Location of the Node */
            UserNode milano1 = new UserNode("Milano");

            Console.WriteLine("*****  Test HTTP CALLBACK Client  *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();
            proxy.GetData("Gimme Trips");
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            Console.ReadLine();
            

        
        }



        static void CallbackClient()
        {
            IDarPoolingCallback callback = new ClientCallback();
            WSDualHttpBinding binding = new WSDualHttpBinding();
            binding.ClientBaseAddress = new Uri("http://localhost:2222/Client1");
            EndpointAddress address = new EndpointAddress("http://localhost:1111/Milano");
            
            DuplexChannelFactory<IDarPooling> factory = new DuplexChannelFactory<IDarPooling>(callback,binding,address);
            IDarPooling proxy = factory.CreateChannel();

            Console.WriteLine("*****  Test HTTP CALLBACK Client  *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();
            proxy.GetData("Gimme Trips");
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            Console.ReadLine();

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
            Console.WriteLine("*****  Test TCP Client  (NO Callback) *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();

            //Console.WriteLine(client2.SayHello());
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            Console.ReadLine();

        }


        static void NoCallbackHTTPClient()
        {
            /* Verbose */
            EndpointAddress address = new EndpointAddress("http://localhost:1111/Milano");
            WSDualHttpBinding binding = new WSDualHttpBinding();
            binding.ClientBaseAddress = new Uri("http://localhost:2222/Client1");
            ChannelFactory<IDarPooling> channelFactory = new ChannelFactory<IDarPooling>(binding);
            IDarPooling client2 = channelFactory.CreateChannel(address);

            //Thread.Sleep(7000);
            Console.WriteLine("*****  Test HTTP Client  (NO Callback) *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();

            //Console.WriteLine(client2.SayHello());
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