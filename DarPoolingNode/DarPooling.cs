using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Communication;
using System.Runtime.Serialization;

using System.Threading;
using System.Diagnostics;
using System.Collections;


namespace DarPoolingNode
{

    public class PeerNode : Node
    {
        public string nodeName { get; private set; }
 
        public IPeer Channel;
        public IPeer Host;
        private ServiceHost host;

        private DuplexChannelFactory<IPeer> _factory;
        private readonly AutoResetEvent _stopFlag = new AutoResetEvent(false);
 
        public PeerNode(string nodeName)
        {
            this.nodeName = nodeName;
        }

        
        public void StartService()
        {

            WSHttpBinding bind = new WSHttpBinding();
            Type contract = typeof(IDarPooling);

            host = new ServiceHost(typeof(DarPooling), new Uri("http://localhost:1111/" + nodeName));

            host.AddServiceEndpoint(contract, bind, "");

            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;

            host.Description.Behaviors.Add(behavior);
            

            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            

            host.Open();
            Console.WriteLine(nodeName + "\t\tnode is now active. Waiting...");
           
            //Console.Read();

            //host.Close();
        
            
            
           /* var peer_binding = new NetPeerTcpBinding();
            peer_binding.Security.Mode = SecurityMode.None;

            var peer_endpoint = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IPeer)),
                peer_binding,
                new EndpointAddress("net.p2p://SimpleP2P"));

            Host = new DarPooling();

            _factory = new DuplexChannelFactory<IPeer>(
                new InstanceContext(Host),
                peer_endpoint);

            var channel = _factory.CreateChannel();

            ((ICommunicationObject)channel).Open();

            // wait until after the channel is open to allow access.
            Channel = channel;*/
        }

        public void StopService()
        {
           // ((ICommunicationObject)Channel).Close();
           // if (_factory != null)
           //     _factory.Close();
            Console.WriteLine("Closing the service host...");
            host.Close();


        }

        public void Run()
        {
            Console.WriteLine("[ Starting Service ]");
            StartService();

            Console.WriteLine("[ Service Started ]");
            _stopFlag.WaitOne();

            Console.WriteLine("[ Stopping Service ]");
            StopService();

            Console.WriteLine("[ Service Stopped ]");
        }

/*        public void Stop()
        {
            _stopFlag.Set();
        }
  */     

    }

    public class DarPooling : IDarPooling, IPeer
    {
        #region Interfaces Implementation

        public SimpleUser[] GetSimpleUsers(SimpleUser[] inputUsers)
        {

            List<SimpleUser> result = new List<SimpleUser>();
            foreach (SimpleUser su in inputUsers)
            {
                if (su.userId < 10)
                {
                    result.Add(su);
                }

            }

            return result.ToArray();
        }

        public string SayHello() 
        {
            return "Hi, dummy";
        }

    
        public void Ping(string sender, string message)
        {
           Console.WriteLine("{0} says: {1}", sender, message);
        }
        #endregion



        private static ArrayList peers = new ArrayList();

        // Start The Service Nodes
        static void Main(string[] args)
        {
            Console.WriteLine("**** Starting the Backbone Nodes... ****\n");
            StartBackboneNodes();

            Console.WriteLine("\nAll Service nodes are now Active");

            Console.ReadLine();
            CloseBackboneNodes();
        }


 
        public static void StartBackboneNodes()
        {
            string[] nodeNames = { "Chiasso", "Milano", "Roma", "Napoli", "Catania" };
            
            
            
            foreach (string name in nodeNames)
            {
                peers.Add(new PeerNode(name.ToUpper()));
            
            }

            foreach (PeerNode peer in peers)
            {
                peer.StartService();
            }

                //Console.WriteLine(c);
            //var peer = new PeerNode(name);
            //var peerThread = new Thread(peer.StartService) {IsBackground = true};
            //peerThread.Start();
 
            //wait for the server to start up.
            //Thread.Sleep(1000);
            //peer.StartService();
           /* while (true)
            {
                Console.Write("Enter Something: ");
                string tmp = Console.ReadLine();
 
                if (tmp == "") break;
 
   //             peer.Channel.Ping(peer.Id, tmp);
            }
 /*
            peer.Stop();
            peerThread.Join();
            */
        }


        public static void CloseBackboneNodes()
        {
            foreach(PeerNode peer in peers)
            {
                peer.StopService();
            }
        
        }


        static void OLD_StartBackboneNodes()
        {

            try
            {

                WSHttpBinding bind = new WSHttpBinding();
                Type contract = typeof(IDarPooling);

                ServiceHost milano = new ServiceHost(typeof(DarPooling), new Uri("http://localhost:1111/Milano"));
                ServiceHost roma = new ServiceHost(typeof(DarPooling), new Uri("http://localhost:1111/Roma"));

                roma.AddServiceEndpoint(contract, bind, "");
                milano.AddServiceEndpoint(contract, bind, "");


                ServiceMetadataBehavior ro_behavior = new ServiceMetadataBehavior();
                ro_behavior.HttpGetEnabled = true;

                ServiceMetadataBehavior mi_behavior = new ServiceMetadataBehavior();
                mi_behavior.HttpGetEnabled = true;

                roma.Description.Behaviors.Add(ro_behavior);
                milano.Description.Behaviors.Add(mi_behavior);

                roma.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
                milano.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                roma.Open();
                Console.WriteLine("ROMA node is now active. Waiting...");
                milano.Open();
                Console.WriteLine("MILANO node is now active. Waiting...");
                Console.Read();

                roma.Close();
                //
                milano.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }


        }


        static void DisplayHostInfo(ServiceHost host)
        {
            Console.WriteLine();
            Console.WriteLine("**** Host Info ****");

            Console.WriteLine("Uri: {0}", host.State);

        }



    } //End Class
} //End Namespace

/*         using (ServiceHost sh = new ServiceHost(typeof(DarPooling)))
           {
               sh.Open();
               Console.WriteLine("Service Starts...");
               Console.ReadLine();
               sh.Close();
           }
           Console.ReadLine();
    */



//roma.Open();




//milano.AddServiceEndpoint(contract, mil_bind, milano_addr);

//milano.Open();

//  Console.WriteLine("Service Starts...");

//DisplayHostInfo(roma);
//DisplayHostInfo(milano);

//    Console.ReadLine();

//BindingElement bindingElement = new HttpTransportBindingElement();
//CustomBinding binding =
// new CustomBinding(bindingElement);
// bindingElement = new BasicHttpBinding();
//CustomBinding binding = new CustomBinding(bindingElement);
//Uri tcpBaseAddress = new Uri("net.tcp://localhost:9000/");
//ServiceHost host = new ServiceHost(typeof(MyService), tcpBaseAddress);
//ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
//metadataBehavior.HttpGetEnabled = true;
//roma.Description.Behaviors.Add(metadataBehavior);
//roma.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
/*metadataBehavior = roma.Description.Behaviors.Find<ServiceMetadataBehavior>();
if (metadataBehavior == null)
{
    metadataBehavior = new ServiceMetadataBehavior();
    roma.Description.Behaviors.Add(metadataBehavior);
}*/
//roma.AddServiceEndpoint(typeof(IMetadataExchange), bind, "MEX");
//host.Open();