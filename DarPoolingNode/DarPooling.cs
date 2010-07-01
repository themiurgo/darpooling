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

        private DuplexChannelFactory<IPeer> _factory;
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
            Console.WriteLine(nodeName + "\t\tnode is now active.");        
            
        }

        public void EnableP2P()
        {

            NetPeerTcpBinding peerBinding = new NetPeerTcpBinding();
            peerBinding.Security.Mode = SecurityMode.None;

            ServiceEndpoint peerEndpoint = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IPeer)),
                peerBinding,
                new EndpointAddress("net.p2p://DarpoolingP2P")
                );

            InstanceContext site = new InstanceContext(new DarPooling());
            _factory = new DuplexChannelFactory<IPeer>(site, peerEndpoint);

            var channel = _factory.CreateChannel();

            ((ICommunicationObject)channel).Open();

            // Wait until after the channel is open to allow access.
            Channel = channel;   
        
        
        }

        public void StopService()
        {
            Console.WriteLine("Closing the service host...");
            host.Close();
            if (Channel != null)
                ((ICommunicationObject)Channel).Close();
            if (_factory != null)
                _factory.Close();
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

    
        public void Ping( string sender, string message)
        {
           Console.WriteLine("I received from {0} the following message: {1}", sender, message);
        }
        #endregion



        private static ArrayList peers = new ArrayList();

        
        static void Main(string[] args)
        {
            Console.WriteLine("**** Starting the Backbone Nodes... ****\n");
            StartBackboneNodes();

            Console.WriteLine("\nAll Service nodes are now Active");
            Console.WriteLine("\n\n" + "Insert a message: ");

            string tmp = Console.ReadLine();
            TestPeers(tmp);
            Console.ReadLine();


            CloseBackboneNodes();
        }


        public static void TestPeers(string message)
        {

            PeerNode p0 = (PeerNode) peers[0];
            PeerNode p1 = (PeerNode)peers[1];

            Console.WriteLine("\nEnabling WCF P2P. Please, wait...");
            p0.EnableP2P();
            p1.EnableP2P();

            p1.Channel.Ping(p1.nodeName,message);
        }

 
        public static void StartBackboneNodes()
        {
            //string[] nodeNames = { "Catania" };
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