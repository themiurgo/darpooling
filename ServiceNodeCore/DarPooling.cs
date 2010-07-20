using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Communication;
using System.Runtime.Serialization;

using System.Collections;
using System.Threading;


namespace DarPoolingNode
{

    public class ServiceNodeCore
    {
        private ServiceNode serviceNode;
        private ServiceHost serviceHost;

        private Type contract;
        private WSDualHttpBinding http_binding;
        private NetTcpBinding tcp_binding;
        private ServiceMetadataBehavior behavior;

        private IDarPooling client;
        private ChannelFactory<IDarPooling> channelFactory;

        //private SimpleUser[] users;
        
        public const string baseHTTPAddress = "http://localhost:1111/";
        public const string baseTCPAddress = "net.tcp://localhost:1112/";
        public string nodeName { get; private set; }


        public ServiceNodeCore(string nodeName)
        {
            this.nodeName = nodeName;
        }

        public void StartService()
        {
            
            serviceNode = new ServiceNode(new Location("Ragusa"), "Ragusa1");

            http_binding = new WSDualHttpBinding();
            tcp_binding = new NetTcpBinding();
            contract = typeof(IDarPooling);

            
            serviceHost = new ServiceHost(typeof(DarPoolingService), new Uri(baseHTTPAddress + nodeName));
            serviceHost.AddServiceEndpoint(contract, http_binding, "");
            serviceHost.AddServiceEndpoint(contract, tcp_binding, baseTCPAddress + nodeName);

            behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;

            serviceHost.Description.Behaviors.Add(behavior);
            serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            
            serviceHost.Open();
            Console.WriteLine(nodeName + "\t\tnode is now active.");        
            
        }


        public string CallNeighbour()
        {
            EndpointAddress n_address = new EndpointAddress("http://localhost:1111/Milano");
            WSDualHttpBinding  n_binding = new WSDualHttpBinding();
            channelFactory = new ChannelFactory<IDarPooling>(n_binding);
            client = channelFactory.CreateChannel(n_address);
            return client.SayHello();
        }

        public void StopService()
        {
            Console.WriteLine("Closing the service host...");
            serviceHost.Close();
            if (channelFactory != null)
                channelFactory.Close();
        }

        #region ServiceNode methods

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
        #endregion

    }

    /// <summary>
    /// This class implements the interface of the Darpooling service
    /// </summary>
    public class DarPoolingService : IDarPooling
    {
        public void SendCommand(Communication.Command command) {}

        public Communication.Result GetResult()
        {
            return new Result("");
        }
        
        public string SayHello() 
        {
            Console.WriteLine("I received a request");
            return "Hi, dummy";
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
    
    public class Launcher
    {
        private static ArrayList nodes = new ArrayList();
    
        static void Main(string[] args)
        {
            
            /*  StartBackboneNodes();
                Console.ReadLine(); // Press Enter to stop the services
                CloseBackboneNodes();
            */

            WriteXML();
        }

        public static void WriteXML()
        {

            string filename = @"..\..\..\config\chiasso.xml";
            XmlDocument xmlDoc = new XmlDocument();

            // Create a new XML file
            XmlTextWriter textWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            textWriter.Formatting = Formatting.Indented;
            textWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            textWriter.WriteComment("Configuration File for a DarPooling Service Node");
            textWriter.WriteStartElement("configuration");
            textWriter.Close();
            
            xmlDoc.Load(filename);
            XmlNode root = xmlDoc.DocumentElement;
            XmlElement neighbours = xmlDoc.CreateElement("neighbours");
            XmlElement neighbour = xmlDoc.CreateElement("neighbour");
            //XmlText textNode = xmlDoc.CreateTextNode("hello");
            //textNode.Value = "hello, world";

            root.AppendChild(neighbours);
            neighbours.AppendChild(neighbour);
            neighbour.SetAttribute("Name", "Milano");
            //neighbour.AppendChild(textNode);

            //textNode.Value = "replacing hello world";
            xmlDoc.Save(filename);

         
            // Read a document
            XmlTextReader textReader = new XmlTextReader(filename);
            // Read until end of file
            while (textReader.Read())
            {
                Console.WriteLine(textReader.Value);   
            }

            

        }

        public static void StartBackboneNodes()
        {
            //string[] nodeNames = { "Catania" };
            string[] nodeNames = { "Chiasso", "Milano", "Roma", "Napoli", "Catania" };

            Console.WriteLine("**** Starting the Backbone Nodes... ****\n");
    
            foreach (string name in nodeNames)
            {
                nodes.Add(new ServiceNodeCore(name.ToUpper()));
            }

            foreach (ServiceNodeCore node in nodes)
            {
                node.StartService();
            }

            Console.WriteLine("\nAll Service nodes are now Active");
            Console.WriteLine("\nWaiting for incoming requests...");
    
        }

        public static void TestNeighbour(string message)
        {

            ServiceNodeCore n0 = (ServiceNodeCore) nodes[0];
            ServiceNodeCore n1 = (ServiceNodeCore)nodes[1];
            
            Console.WriteLine(n0.nodeName + " is calling Milano....");
            string mex = n0.CallNeighbour();
            Console.WriteLine("Got :" + mex);
        }

        public static void CloseBackboneNodes()
        {
            foreach(ServiceNodeCore node in nodes)
            {
                node.StopService();
            }
        }

    } //End Launcher
} //End Namespace