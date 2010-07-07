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

    public class ServiceNodeCore
    {
        //private List<ServiceNode> neighbours;
        //private List<UserNode> localUsers;
        private ServiceNode serviceNode;
        private ServiceHost serviceHost;
        //private EndpointAddress address;
        private WSHttpBinding binding;
        private Type contract;
        private ChannelFactory<IDarPooling> channelFactory;
        private IDarPooling client;
        private ServiceMetadataBehavior behavior;

        public const string baseAddress = "http://localhost:1111/";
        public string nodeName { get; private set; }


        public ServiceNodeCore(string nodeName)
        {
            this.nodeName = nodeName;
        }

        public void StartService()
        {
            serviceNode = new ServiceNode();

            binding = new WSHttpBinding();
            contract = typeof(IDarPooling);

            serviceHost = new ServiceHost(typeof(DarPoolingService), new Uri( baseAddress + nodeName));
            serviceHost.AddServiceEndpoint(contract, binding, "");

            behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;

            serviceHost.Description.Behaviors.Add(behavior);
            serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            
            serviceHost.Open();
            Console.WriteLine(nodeName + "\t\tnode is now active.");        
            
        }


        public string CallNeighbour()
        {
            /* Verbose */
            EndpointAddress n_address = new EndpointAddress("http://localhost:1111/Milano");
            WSHttpBinding  n_binding = new WSHttpBinding();
            channelFactory = new ChannelFactory<IDarPooling>(n_binding);
            client = channelFactory.CreateChannel(n_address);
            return client.SayHello();
        }

        public void StopService()
        {
            Console.WriteLine("Closing the service host...");
            serviceHost.Close();
            //if (Channel != null)
            //    ((ICommunicationObject)Channel).Close();
            //if (_factory != null)
            //    _factory.Close();
            if (channelFactory != null)
                channelFactory.Close();
        }


        public bool hasNeighbour(ServiceNode node)
        {
            return serviceNode.hasNeighbour(node);
        }

        public void addNeighbour(ServiceNode node)
        {
            serviceNode.addNeighbour(node);
        }

        public void removeNeighbour(ServiceNode node)
        {
            serviceNode.removeNeighbour(node);
        }

        public bool hasUser(UserNode node)
        {
            return serviceNode.hasUser(node);
        }

        public void addUser(UserNode node)
        {
            serviceNode.addUser(node);
        }

        public void removeUser(UserNode node)
        {
            serviceNode.removeUser(node);
        }
    }

   
    public class DarPoolingService : IDarPooling
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

    
        /* public void Ping( string sender, string message)
        {
           Console.WriteLine("I received from {0} the following message: {1}", sender, message);
        }*/
        #endregion





    }
    
    public class Launcher
    {
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

        public static void StartBackboneNodes()
        {
            //string[] nodeNames = { "Catania" };
            string[] nodeNames = { "Chiasso", "Milano", "Roma", "Napoli", "Catania" };

            foreach (string name in nodeNames)
            {
                peers.Add(new ServiceNodeCore(name.ToUpper()));
            }

            foreach (ServiceNodeCore peer in peers)
            {
                peer.StartService();
            }

            //peer.Channel.Ping(peer.Id, tmp);
            //peer.Stop();
            //peerThread.Join();

        }

        public static void TestPeers(string message)
        {

            ServiceNodeCore p0 = (ServiceNodeCore) peers[0];
            ServiceNodeCore p1 = (ServiceNodeCore)peers[1];
            
            Console.WriteLine(p0.nodeName + " is calling Milano....");
            string mex = p0.CallNeighbour();
            Console.WriteLine("Got :" + mex);
            
            //Console.WriteLine("\nEnabling WCF P2P. Please, wait...");
            //p0.EnableP2P();
            //p1.EnableP2P();

            //p1.Channel.Ping(p1.nodeName,message);
        }

 
        


        public static void CloseBackboneNodes()
        {
            foreach(ServiceNodeCore peer in peers)
            {
                peer.StopService();
            }
        }

    } //End Class
} //End Namespace


/*
public class PeerNode : Node
{
    public string nodeName { get; private set; }

    public IPeer Channel;
    //        public IPeer Host;
    private ServiceHost host;

    private DuplexChannelFactory<IPeer> _factory;
    //        private readonly AutoResetEvent _stopFlag = new AutoResetEvent(false);

    EndpointAddress n_address;
    WSHttpBinding n_binding;
    ChannelFactory<IDarPooling> channelFactory;
    IDarPooling client;


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

    public string CallNeighbour()
    {
        // Verbose
        n_address = new EndpointAddress("http://localhost:1111/Milano");
        n_binding = new WSHttpBinding();
        channelFactory = new ChannelFactory<IDarPooling>(n_binding);
        client = channelFactory.CreateChannel(n_address);
        return client.SayHello();
    }

    public void StopService()
    {
        Console.WriteLine("Closing the service host...");
        host.Close();
        if (Channel != null)
            ((ICommunicationObject)Channel).Close();
        if (_factory != null)
            _factory.Close();
        if (channelFactory != null)
            channelFactory.Close();
    }


    public void DisplayHostInfo()
    {
        Console.WriteLine();
        Console.WriteLine("**** Host Info ****");
        Console.WriteLine("Uri: {0}", host.State);
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
     
}*/
