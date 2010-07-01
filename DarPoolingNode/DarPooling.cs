using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Communication;
using System.Runtime.Serialization;

using System.Collections;


namespace DarPoolingNode
{

    public class PeerNode : Node
    {
        public string nodeName { get; private set; }
 
        public IPeer Channel;
//        public IPeer Host;
        private ServiceHost host;

//        private DuplexChannelFactory<IPeer> _factory;
//        private readonly AutoResetEvent _stopFlag = new AutoResetEvent(false);
 
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
            Console.WriteLine("Closing the service host...");
            host.Close();
        }
        
        
        public void DisplayHostInfo()
        {
            Console.WriteLine();
            Console.WriteLine("**** Host Info ****");
            Console.WriteLine("Uri: {0}", host.State);
        }
/*        public void Run()
        {
            Console.WriteLine("[ Starting Service ]");
            StartService();

            Console.WriteLine("[ Service Started ]");
            _stopFlag.WaitOne();

            Console.WriteLine("[ Stopping Service ]");
            StopService();

            Console.WriteLine("[ Service Stopped ]");
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

        
        static void Main(string[] args)
        {
            Console.WriteLine("**** Starting the Backbone Nodes... ****\n");
            StartBackboneNodes();

            Console.WriteLine("\nAll Service nodes are now Active \nPress enter to quit...");

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
 
            //peer.Channel.Ping(peer.Id, tmp);
            //peer.Stop();
            //peerThread.Join();
           
        }


        public static void CloseBackboneNodes()
        {
            foreach(PeerNode peer in peers)
            {
                peer.StopService();
            }
        }

    } //End Class
} //End Namespace