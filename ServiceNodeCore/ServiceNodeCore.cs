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

    /// <summary>
    /// Class ServiceNodeCore has two main purposes:
    /// 1) Hosting the DarPooling Service;
    /// 2) Implementing the DarPooling Operations.
    /// The execution of the Commands sent by Clients will
    /// result in the invocation of a specific DarPooling
    /// Operation; thus, the ServiceNodeCore is the class that
    /// actually satisfies the Client request.
    /// It holds the local Users and Trips databases.
    /// </summary>
    public class ServiceNodeCore : IDarPoolingOperations
    {
        #region ServiceNodeCore Fields

        // The ServiceNode instance contains information about
        // location (coordinates) and topology (neighbours).
        private ServiceNode serviceNode;
        
        // Provide a host for the DarPooling Service.
        private ServiceHost serviceHost;
        
        // Reference of the implementor of the IDarPooling interface.
        private DarPoolingService serviceImpl;

        // The root directory for all local databases
        // FIXME: The directory should be automatically created, if it is not found.
        // TODO: Every service node should have its own subdirectory
        private const string databaseRootPath = @"..\..\..\config\";

        // Path, document and lock for the USER database.
        private string userDatabasePath;
        private XDocument userDatabase;
        private ReaderWriterLockSlim userLock = new ReaderWriterLockSlim();

        // Path, document and lock for the TRIP database.
        private string tripDatabasePath;
        private XDocument tripDatabase;
        private ReaderWriterLockSlim tripLock = new ReaderWriterLockSlim();
                        
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
            this.serviceNode = serviceNode;
            
            // Create a new instance of the service implementor.
            serviceImpl = new DarPoolingService(this);

            InitializeXmlDatabases();
        }


        /// <summary>
        /// Create the files for User and Trip databases.
        /// Set format
        /// </summary>
        private void InitializeXmlDatabases()
        {
            XmlTextWriter xmlWriter;
            
            // Initialize the User DB
            userDatabasePath = databaseRootPath + NodeName.ToUpper() + "_users.xml";
            xmlWriter = new XmlTextWriter(userDatabasePath, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteComment("Users DB for " + this.NodeName  + " DarPooling Service Node");
            // This is the root tag for all user
            xmlWriter.WriteStartElement("Users");
            xmlWriter.Close();

            // Initialize the Trip DB
            tripDatabasePath = databaseRootPath + NodeName.ToUpper() + "_trips.xml";
            xmlWriter = new XmlTextWriter(tripDatabasePath, System.Text.Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteComment("Trips DB for " + this.NodeName + " DarPooling Service Node");
            xmlWriter.WriteStartElement("Trips");
            xmlWriter.Close();
        
        }


        /// <summary>
        /// Set the parameters of the service and then host it.
        /// </summary>
        public void StartService()
        {
            Console.Write("Starting " + NodeName + " node... ");

            // Set address, binding, contract and behavior of the Service
            Uri http_uri = new Uri(baseHTTPAddress + NodeName);
            Uri tcp_uri = new Uri(baseTCPAddress + NodeName);
            WSDualHttpBinding http_binding = new WSDualHttpBinding();
            NetTcpBinding tcp_binding = new NetTcpBinding();
            Type contract = typeof(IDarPooling);
            ServiceMetadataBehavior mex_behavior = new ServiceMetadataBehavior();
            mex_behavior.HttpGetEnabled = true;

            // Hosting the service
            serviceHost = new ServiceHost(serviceImpl, http_uri);
            serviceHost.AddServiceEndpoint(contract, http_binding, "");
            serviceHost.AddServiceEndpoint(contract, tcp_binding, tcp_uri);
            serviceHost.Description.Behaviors.Add(mex_behavior);
            serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            // Run the service
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
            //if (channelFactory != null)
            //    channelFactory.Close();
            Console.WriteLine("Stopped");
        }


        public void SaveUser(User newUser)
        {

            userLock.EnterWriteLock();
            try
            {
                Console.WriteLine("Accessing critical section....");
                Thread.Sleep(5000);
                userDatabase = XDocument.Load(userDatabasePath);

                int nextAvailableID = Convert.ToInt32(
                                     (from user in userDatabase.Descendants("User")
                                      orderby Convert.ToInt32(user.Element("UserID").Value) descending
                                      select user.Element("UserID").Value).FirstOrDefault()) + 1;

                newUser.UserID = nextAvailableID;

                XElement newXmlUser = new XElement("User",
                    new XElement("UserID", newUser.UserID),
                    new XElement("UserName", newUser.UserName),
                    new XElement("Password", newUser.Password),
                    new XElement("Name", newUser.Name),
                    new XElement("Sex", newUser.UserSex),
                    new XElement("BirthDate", newUser.BirthDate),
                    new XElement("Email", newUser.Email),
                    new XElement("Smoker", newUser.Smoker),
                    new XElement("SignupDate", newUser.SignupDate),
                    new XElement("Whereabouts", newUser.Whereabouts)
                    );

                userDatabase.Element("Users").Add(newXmlUser);
                userDatabase.Save(userDatabasePath);
            }
            finally
            {
                userLock.ExitWriteLock();
                Console.WriteLine("Exiting critical section....");
            }

        }

        public void SaveTrip(Trip newTrip)
        {
            tripDatabase = XDocument.Load(tripDatabasePath);

            int nextAvailableID = Convert.ToInt32(
                     (from trip in tripDatabase.Descendants("Trip")
                      orderby Convert.ToInt32(trip.Element("ID").Value) descending
                      select trip.Element("ID").Value).FirstOrDefault()) + 1;


            newTrip.ID = nextAvailableID;

            XElement newXmlTrip = new XElement("Trip",
                new XElement("ID", newTrip.ID),
                new XElement("Owner", newTrip.Owner),
                new XElement("DepartureName", newTrip.DepartureName),
                new XElement("DepartureDateTime", newTrip.DepartureDateTime),
                new XElement("ArrivalName", newTrip.ArrivalName),
                new XElement("ArrivalDateTime", newTrip.ArrivalDateTime),
                new XElement("Smoke", newTrip.Smoke),
                new XElement("Music", newTrip.Music),
                new XElement("Cost", newTrip.Cost),
                new XElement("FreeSits", newTrip.FreeSits),
                new XElement("Notes", newTrip.Notes),
                new XElement("Modifiable", newTrip.Modifiable)
                );
            tripDatabase.Element("Trips").Add(newXmlTrip);
            tripDatabase.Save(tripDatabasePath);
        }

        private bool CheckUser(string username)
        {
            //userLock.ExitReadLock
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


