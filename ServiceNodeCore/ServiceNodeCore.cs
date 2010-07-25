using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

using System.ServiceModel;
using System.ServiceModel.Description;

using Communication;
//using System.Runtime.Serialization;
//using System.ServiceModel.Channels;
//using System.IO;
//using System.Threading;


namespace ServiceNodeCore
{

    public class ServiceNodeCore
    {
        private ServiceNode serviceNode;
        private ServiceHost serviceHost;

        /* Service Settings */
        private Type contract;
        private WSDualHttpBinding http_binding;
        private NetTcpBinding tcp_binding;
        private ServiceMetadataBehavior mex_behavior;

        private IDarPooling client;
        private ChannelFactory<IDarPooling> channelFactory;
        
        public const string baseHTTPAddress = "http://localhost:1111/";
        public const string baseTCPAddress = "net.tcp://localhost:1112/";


        public ServiceNodeCore(ServiceNode sn)
        {
            serviceNode =sn;
        }


        public void StartService()
        {
            /* Address */
            Uri http_uri = new Uri(baseHTTPAddress + NodeName);
            Uri tcp_uri = new Uri(baseTCPAddress + NodeName);
            /* Binding */
            http_binding = new WSDualHttpBinding();
            tcp_binding = new NetTcpBinding();
            /* Contract */
            contract = typeof(IDarPooling);
            /* Behavior */
            mex_behavior = new ServiceMetadataBehavior();
            mex_behavior.HttpGetEnabled = true;
            
            /* Hosting the service */
            serviceHost = new ServiceHost(typeof(DarPoolingService), http_uri);
            serviceHost.AddServiceEndpoint(contract, http_binding, "");
            serviceHost.AddServiceEndpoint(contract, tcp_binding, tcp_uri);
            serviceHost.Description.Behaviors.Add(mex_behavior);
            serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            
            /* Run the service */
            serviceHost.Open();
            Console.WriteLine(NodeName + "\t\tnode is now active.");        
        }


        public void StopService()
        {
            Console.WriteLine("Closing the service host...");
            serviceHost.Close();
            if (channelFactory != null)
                channelFactory.Close();
        }

        public static void SaveTrip(Trip t)
        {
            string filename = @"..\..\..\config\trip.xml";

            
            XDocument doc = XDocument.Load(filename);

            XElement newTrip = new XElement("Trip",
                new XElement("ID", t.ID),
                new XElement("Owner", t.Owner),
                new XElement("DepartureName", t.DepartureName),
                new XElement("DepartureDateTime", t.DepartureDateTime),
                new XElement("ArrivalName", t.ArrivalName),
                new XElement("ArrivalDateTime", t.ArrivalDateTime),
                new XElement("Smoke", t.Smoke),
                new XElement("Music", t.Music),
                new XElement("Cost", t.Cost),
                new XElement("FreeSits", t.FreeSits),
                new XElement("Notes", t.Notes),
                new XElement("Modifiable", t.Modifiable)
                );
            doc.Element("Trips").Add(newTrip);
            doc.Save(filename);
           
            Console.WriteLine("Trip Saved!");
        }

        public static List<Trip> GetTrip()
        {
            string filename = @"..\..\..\config\trip.xml";
            XDocument doc = XDocument.Load(filename);

            return (from t in doc.Descendants("Trip")
                    orderby t.Element("ID").Value
                    select new Trip()
                    {
                        ID = Convert.ToInt32(t.Element("ID").Value),
                        Owner = t.Element("Owner").Value,
                        DepartureName = t.Element("DepartureName").Value,
                        DepartureDateTime = Convert.ToDateTime(t.Element("DepartureDateTime").Value),
                        ArrivalName = t.Element("ArrivalName").Value,
                        ArrivalDateTime = Convert.ToDateTime(t.Element("ArrivalDateTime").Value),
                        Smoke = Convert.ToBoolean(t.Element("Smoke").Value),
                        Music = Convert.ToBoolean(t.Element("Music").Value),
                        Cost = Convert.ToDouble(t.Element("Cost").Value),
                        FreeSits = Convert.ToInt32(t.Element("FreeSits").Value),
                        Notes = t.Element("Notes").Value,
                        Modifiable = Convert.ToBoolean(t.Element("Modifiable").Value)
                    }).ToList();

        
        }
         

        // Properties
        public string NodeName
        {
            get { return serviceNode.NodeName; }
            private set { serviceNode.NodeName = value; }
        }

        public Location NodeLocation
        {
            get { return serviceNode.Location; }
            private set { serviceNode.Location = value; }
        }

        public int NumNeighbours 
        {
            get { return serviceNode.NumNeighbours; }
        }

        public ServiceNode ServiceNode
        {
            get { return serviceNode; }
        }

        #region ServiceNode methods

        public bool hasNeighbour(ServiceNodeCore node)
        {
            return serviceNode.hasNeighbour(node.ServiceNode);
        }

        public void addNeighbour(ServiceNodeCore node)
        {
            serviceNode.addNeighbour(node.ServiceNode);
        }

        public void removeNeighbour(ServiceNodeCore node)
        {
            serviceNode.removeNeighbour(node.ServiceNode);
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

    
    
    
    /* Start the SNs */
    public class Launcher
    {
        private static ArrayList nodes = new ArrayList();
        private static Dictionary<string,Location> nameLoc = new Dictionary<string,Location>(); 
        //private static ArrayList nodeNames = new ArrayList();
    
        static void Main(string[] args)
        {
            Console.WriteLine("**** Starting the Backbone Nodes... ****\n");
            //StartBackboneNodes();
            
            //List<Customer> list = ServiceNodeCore.ReadFromXML();
            //foreach (Customer c in list)
            //    Console.WriteLine("Customer name is: {0}", c.Forename);
            InitTripXML();
            Console.ReadLine();
            //CloseBackboneNodes();
        }



        public static void StartBackboneNodes()
        {
            string[] locNames = new string[] {"Chiasso", "Milano", "Roma", "Napoli", "Catania" };
            string[] coords;
            double latitude;
            double longitude;
            Location location;
            
            /* Obtain the Location of the Node */
            Console.Write("Getting coordinates from GMap server....   ");
            foreach (string locName in locNames)
            {
                coords = GMapsAPI.addrToLatLng(locName);
                latitude = double.Parse(coords[0]);
                longitude = double.Parse(coords[1]);
                location = new Location(locName, latitude, longitude);
                nameLoc.Add(locName,location);
            }
            Console.WriteLine("Done!");

            /* Service nodes */
            ServiceNode chiassoSN = new ServiceNode("Chiasso", nameLoc["Chiasso"]);
            ServiceNode milanoSN = new ServiceNode("Milano", nameLoc["Milano"]);
            ServiceNode romaSN = new ServiceNode("Roma", nameLoc["Roma"]);
            ServiceNode napoliSN = new ServiceNode("Napoli", nameLoc["Napoli"]);
            ServiceNode cataniaSN = new ServiceNode("Catania", nameLoc["Catania"]);
            ServiceNode catania2SN = new ServiceNode("Catania2", nameLoc["Catania"]);
            /* Service node core */
            ServiceNodeCore chiasso = new ServiceNodeCore(chiassoSN);
            ServiceNodeCore milano  = new ServiceNodeCore(milanoSN);
            ServiceNodeCore roma    = new ServiceNodeCore(romaSN);
            ServiceNodeCore napoli  = new ServiceNodeCore(napoliSN);                                                             
            ServiceNodeCore catania = new ServiceNodeCore(cataniaSN);
            ServiceNodeCore catania2= new ServiceNodeCore(catania2SN);

            /* Set of Backbone Nodes */
            ServiceNodeCore[] nodes = 
                new ServiceNodeCore[] { chiasso, milano, roma, napoli, catania, catania2
                                      };

            Console.WriteLine("\nStarting the Service...");
            foreach (ServiceNodeCore node in nodes)
                { node.StartService(); }
            Console.WriteLine("\nAll Service nodes are now Online");

            /* Set Topology */
            chiasso.addNeighbour(milano);
            milano.addNeighbour(roma);
            roma.addNeighbour(napoli);
            napoli.addNeighbour(catania);
            napoli.addNeighbour(catania2);
            catania.addNeighbour(catania2);

            Console.WriteLine("\nRetrieving information....");
            foreach (ServiceNodeCore n in nodes)
            {
                Console.WriteLine("I'm {0}. My Coords are : {1} and {2}. I have {3} neighbours", n.NodeName, n.NodeLocation.Latitude, n.NodeLocation.Longitude, n.NumNeighbours);
            }
            Console.WriteLine("\nWaiting for incoming requests...");
    
        }
  
        
        public static void CloseBackboneNodes()
        {
            foreach(ServiceNodeCore node in nodes)
            {
                node.StopService();
            }
        }

        /* This methods initializes the DB with sample Trips */
        public static void InitTripXML()
        {
            User massimo = new User("Massimo", "MAXXI");
            User michela = new User("Michela", "Mia");
            User daniele = new User("Daniele", "Shaoran");
            User antonio = new User("Antonio", "4nt0");
            User federica = new User("Federica", "Fede");

            Trip trip1 = new Trip
            {
                ID = 0,
                Owner = massimo.UserName,
                DepartureName = "Catania",
                DepartureDateTime = new DateTime(2010, 7, 30, 8, 0, 0),
                ArrivalName = "Messina",
                ArrivalDateTime = new DateTime(2010, 7, 30, 10, 30, 0),
                Smoke = false,
                Music = false,
                Cost = 10,
                FreeSits = 4,
                Notes = "none",
                Modifiable = false
            };

            // Create a new XML file
            string filename = @"..\..\..\config\trip.xml";
            XmlTextWriter textWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            textWriter.Formatting = Formatting.Indented;
            textWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            textWriter.WriteComment("Trips DB for a DarPooling Service Node");
            textWriter.WriteStartElement("Trips");
            textWriter.Close();
            
            ServiceNodeCore.SaveTrip(trip1);
            ServiceNodeCore.SaveTrip(trip1);
            //Trip queryTrip = new Trip {DepartureName="Catania"};
            List<Trip> list = ServiceNodeCore.GetTrip();
            Console.WriteLine("Retrieved {0} trip(s).", list.Count);
            foreach (Trip t in list)
            {
                t.PrintFullInfo();
            }

        }

    } //End Launcher
} //End Namespace
