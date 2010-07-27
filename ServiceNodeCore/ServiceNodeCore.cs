using System;
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
//using System.Collections;

namespace ServiceNodeCore
{

    public class ServiceNodeCore
    {
        #region Class Fields

        private ServiceNode serviceNode;
        private ServiceHost serviceHost;
        private DarPoolingService serviceImpl;

        /* Service Settings */
        private Type contract;
        private WSDualHttpBinding http_binding;
        private NetTcpBinding tcp_binding;
        private ServiceMetadataBehavior mex_behavior;

        private IDarPooling client;
        private ChannelFactory<IDarPooling> channelFactory;

        //private static int userCounter;
        private int tripCounter;
        private XDocument tripsDB;
        //private static XDocument usersDB;
        //private static string usersDBPath = @"..\..\..\config\users.xml";
        private string tripsDBPath;
        private const string tripsDBRootPath = @"..\..\..\config\";
        
        public const string baseHTTPAddress = "http://localhost:1111/";
        public const string baseTCPAddress = "net.tcp://localhost:1112/";

        #endregion

        public ServiceNodeCore(ServiceNode sn)
        {
            serviceNode = sn;
            tripsDBPath = tripsDBRootPath + "trips_" + sn.NodeName.ToUpper() + ".xml";
            serviceImpl = new DarPoolingService();
            //userCounter = -1;
            tripCounter = -1;
        }


        public void StartService()
        {
            Console.Write("Starting " + NodeName + " node... ");
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
            Console.WriteLine("OK!");        
        }

        public void StopService()
        {
            Console.Write("Stopping " + NodeName + " node... ");
            serviceHost.Close();
            if (channelFactory != null)
                channelFactory.Close();
            Console.WriteLine("Stopped"); 
        }


        public void SaveTrip(Trip t)
        {
            tripsDB = XDocument.Load(tripsDBPath);

            tripCounter++;
            t.ID = tripCounter;

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
            tripsDB.Element("Trips").Add(newTrip);
            tripsDB.Save(tripsDBPath);
           
           // Console.WriteLine("Trip Saved!");
        }

        public List<Trip> GetTrip(Trip filterTrip)
        {
            tripsDB = XDocument.Load(tripsDBPath);

            var baseQuery = (from t in tripsDB.Descendants("Trip")
                    where t.Element("DepartureName").Value.Equals(filterTrip.DepartureName) &&
                          Convert.ToInt32(t.Element("FreeSits").Value) > 0
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
                    });

            IEnumerable<Trip> filteredQuery = FilterQuery(filterTrip, baseQuery);
            return filteredQuery.ToList();

        }

        private IEnumerable<Trip> FilterQuery(Trip filterTrip, IEnumerable<Trip> filteringQuery)
        {
            /* Prefiltering */
            filteringQuery = from i in filteringQuery
                             where i.Smoke == filterTrip.Smoke && i.Music == filterTrip.Music
                             select i;
            filteringQuery = from i in filteringQuery
                             where DateTime.Compare(filterTrip.DepartureDateTime, i.DepartureDateTime) < 0 &&
                                   DateTime.Compare(filterTrip.ArrivalDateTime, i.ArrivalDateTime) < 0
                             select i;

            if (filterTrip.Owner != null)
            {
                filteringQuery = from i in filteringQuery
                                 where i.Owner == filterTrip.Owner
                                 select i;
            }

            if (filterTrip.ArrivalName != null)
            {
                filteringQuery = from i in filteringQuery
                                 where i.ArrivalName == filterTrip.ArrivalName
                                 select i;
            }

            if (filterTrip.Cost > 0)
            {
                filteringQuery = from i in filteringQuery
                                 where i.Cost < filterTrip.Cost
                                 select i;
            }


            IEnumerable<Trip> filteredQuery = filteringQuery;
            return filteredQuery;
        }

        #region Properties

        public string TripsDBPath
        {
            get { return tripsDBPath; }
        }

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
        #endregion


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

    

} //End Namespace
/* public static void SaveUser(User u)
  {
      usersDB = XDocument.Load(usersDBPath);

      userCounter++;
      u.UserID = userCounter;

      XElement newUser = new XElement("User",
          new XElement("UserID", u.UserID),
          new XElement("UserName", u.UserName),
          new XElement("Name", u.Name),
          new XElement("Sex", u.UserSex),
          new XElement("BirthDate", u.BirthDate),
          new XElement("Email", u.Email),
          new XElement("Smoker", u.Smoker),
          new XElement("SignupDate", u.SignupDate),
          new XElement("Whereabouts", u.Whereabouts)
          );
      usersDB.Element("Users").Add(newUser);
      usersDB.Save(usersDBPath);

      //Console.WriteLine("User Saved!");
  }*/