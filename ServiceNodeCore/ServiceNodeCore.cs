using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

using System.Threading;
using System.Security.Cryptography;

using System.ServiceModel;
using System.ServiceModel.Description;

using Communication;

namespace ServiceNodeCore
{

    public class ServiceNodeCore : IDarPoolingOperations
    {
        #region ServiceNodeCore Fields

        // The ServiceNode instance contains information about
        // location (coordinates) and topology (neighbours).
        private ServiceNode serviceNode;
        // Provide an endpoint for the DarPooling Service.
        private ServiceHost serviceHost;
        // Reference of the implementor of the DarPooling service.
        private DarPoolingService serviceImpl;

        /* Service Settings */
        private Type contract;
        private WSDualHttpBinding http_binding;
        private NetTcpBinding tcp_binding;
        private ServiceMetadataBehavior mex_behavior;

        private IDarPooling client;
        private ChannelFactory<IDarPooling> channelFactory;

        // The root directory for all local databases
        // FIXME: The directory should be automatically created, if it is not found.
        // TODO: Every service node should have its own subdirectory
        private const string databaseRootPath = @"..\..\..\config\";
        // Paths for the local DB of users and trips
        private string userDatabasePath;
        private string tripDatabasePath;
        // Represent the ID for users and trips respectively.
        // TODO: the ID could be automatically generated from the DB.
        private int userCounter;
        private int tripCounter;
        // Represent the XML document of the database.
        private XDocument userDatabase;
        private XDocument tripDatabase;
        
        // The root http and tcp addresses, which are the same for every
        // service instance.
        private const string baseHTTPAddress = "http://localhost:1111/";
        private const string baseTCPAddress = "net.tcp://localhost:1112/";

        #endregion

        /// <summary>
        /// Create a ServiceNodeCore and initialize its fields.
        /// </summary>
        /// <param name="serviceNode">represents the ServiceNode and its settings.</param>
        public ServiceNodeCore(ServiceNode serviceNode)
        {
            // Create a new instance of the service implementor.
            serviceImpl = new DarPoolingService(this);

            this.serviceNode = serviceNode;

            // Set up the files of the local databases. 
            string suffix = NodeName.ToUpper();
            tripDatabasePath = databaseRootPath + "trips_" + suffix + ".xml";
            userDatabasePath = databaseRootPath + "users_" + suffix + ".xml";
            userCounter = -1;
            tripCounter = -1;
        }


        /// <summary>
        /// Provide address, binding and behavior for the DarPooling service and then 
        /// start it.
        /// </summary>
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
            serviceHost = new ServiceHost(serviceImpl, http_uri);
            serviceHost.AddServiceEndpoint(contract, http_binding, "");
            serviceHost.AddServiceEndpoint(contract, tcp_binding, tcp_uri);
            serviceHost.Description.Behaviors.Add(mex_behavior);
            serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            /* Run the service */
            serviceHost.Open();
            Console.WriteLine("OK!");
        }

        /// <summary>
        /// End the service. The node will go offline.
        /// </summary>
        public void StopService()
        {
            Console.Write("Stopping " + NodeName + " node... ");
            serviceHost.Close();
            if (channelFactory != null)
                channelFactory.Close();
            Console.WriteLine("Stopped");
        }



        public void SaveUser(User u)
        {
            userDatabase = XDocument.Load(userDatabasePath);

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

            userDatabase.Element("Users").Add(newUser);
            userDatabase.Save(userDatabasePath);
        }

        private bool CheckUser(string username)
        {
            userDatabase = XDocument.Load(userDatabasePath);

            var baseQuery = (from u in userDatabase.Descendants("User")
                             where u.Element("UserName").Value.Equals(username)
                             select u);
            if (baseQuery.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        #region DarPoolingOperations Implementation

        public Result Join(string username, string password)
        {
            Console.WriteLine("I am ServiceNodeCore executing LoginUser()");
            Thread.Sleep(3000);

            if (true)
            {
                LoginOkResult success = new LoginOkResult();
                success.Comment = "Ok, you are now logged in";
                return success;
            }
            else
            {
                LoginErrorResult failure = new LoginErrorResult();
                failure.Comment = "Invalid Username or Password";
                return failure;
            }

        }


        public Result Unjoin(string username) 
        { 
            return new NullResult(); 
        }
        
        public Result RegisterUser(string username, string password) 
        {
            return new NullResult();
        }
        public Result InsertTrip(Trip trip) 
        {
            return new NullResult();
        }

        #endregion

        public void SaveTrip(Trip t)
        {
            tripDatabase = XDocument.Load(tripDatabasePath);

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
            tripDatabase.Element("Trips").Add(newTrip);
            tripDatabase.Save(tripDatabasePath);
           
           // Console.WriteLine("Trip Saved!");
        }

        public List<Trip> GetTrip(Trip filterTrip)
        {
            tripDatabase = XDocument.Load(tripDatabasePath);

            var baseQuery = (from t in tripDatabase.Descendants("Trip")
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
            get { return tripDatabasePath; }
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


