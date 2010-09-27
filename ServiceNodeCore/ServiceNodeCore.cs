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
        private ReaderWriterLockSlim userDatabaseLock = new ReaderWriterLockSlim();

        // Path, document and lock for the TRIP database.
        private string tripDatabasePath;
        private XDocument tripDatabase;
        private ReaderWriterLockSlim tripDatabaseLock = new ReaderWriterLockSlim();

        //Delegates that deal with DarPoolingService structures
        private delegate void AddJoinedUser(string username);
        private delegate void RemoveJoinedUser(string username);
        private AddJoinedUser addJoinedUser;
        private RemoveJoinedUser removeJoinedUser;
                        
        // The root http and tcp addresses, which are the same for every
        // service instance.
        private const string baseHTTPAddress = "http://localhost:1111/";
        private const string baseTCPAddress = "net.tcp://localhost:1155/";

        // The root address where Services wait for a request forwarded by 
        // another service node.
        private const string baseForwardAddress = "http://localhost:1177/";
        // Keep track of total number of forwarded requests. Also used as an ID
        // for these forwarded request.
        //private int forwardCounter;

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

            // Set the delegates
            addJoinedUser = new AddJoinedUser(serviceImpl.AddJoinedUser);
            removeJoinedUser = new RemoveJoinedUser(serviceImpl.RemoveJoinedUser);

            // The forwardCounter should always be greater than zero.
            //forwardCounter = 1;

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
            Console.Write("Starting " + NodeName + " node in " + NodeGeoName + " ... ");

            // Set address, binding, contract and behavior of the Service
            Uri http_uri = new Uri(baseHTTPAddress + NodeName);
            Uri tcp_uri = new Uri(baseTCPAddress + NodeName);
            Uri fwd_uri = new Uri(baseForwardAddress + NodeName);
            WSDualHttpBinding http_binding = new WSDualHttpBinding();
            NetTcpBinding tcp_binding = new NetTcpBinding();
            BasicHttpBinding fwd_binding = new BasicHttpBinding();
            Type darPoolingContract = typeof(IDarPooling);
            Type darPoolingForwardingContract = typeof(IDarPoolingForwarding);
            ServiceMetadataBehavior mex_behavior = new ServiceMetadataBehavior();
            mex_behavior.HttpGetEnabled = true;

            // Hosting the services
            serviceHost = new ServiceHost(serviceImpl, http_uri);
            serviceHost.AddServiceEndpoint(darPoolingContract, http_binding, "");
            serviceHost.AddServiceEndpoint(darPoolingContract, tcp_binding, tcp_uri);
            serviceHost.AddServiceEndpoint(darPoolingForwardingContract, fwd_binding, fwd_uri);
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


        #region IDarPoolingOperations implementation


        public Result RegisterUser(User newUser)
        {
            // Enter the critical section in upgradeable mode.
            userDatabaseLock.EnterUpgradeableReadLock();
            try
            {
                userDatabase = XDocument.Load(userDatabasePath);

                // Determine if the provided username has been already taken
                var sameUserName = (from u in userDatabase.Descendants("User")
                                    where u.Attribute("principal").Value.Equals(newUser.UserName)
                                    select u);

                // The username is already present. The user must choose another one.
                if (sameUserName.Count() != 0)
                {
                    Result error = new RegisterErrorResult();
                    error.Comment = "Sorry, this UserName is already present.";
                    return error;
                }
                else //Register the user
                {
                    // Build the final username.
                    string darPoolingUsername = newUser.UserName + "@" + baseHTTPAddress + NodeName;
                    // Extract the next ID from the database
                    int nextAvailableID = Convert.ToInt32(
                                     (from user in userDatabase.Descendants("User")
                                      orderby Convert.ToInt32(user.Element("UserID").Value) descending
                                      select user.Element("UserID").Value).FirstOrDefault()) + 1;

                    newUser.UserID = nextAvailableID;

                    // Create the XML entity that represent the User in the database.
                    XElement newXmlUser = new XElement("User",
                        new XElement("UserID", newUser.UserID),
                        new XElement("UserName", darPoolingUsername),
                        new XElement("Password", newUser.PasswordHash), // FIXME: should store hash, not password
                        new XElement("Name", newUser.Name),
                        new XElement("Sex", newUser.UserSex),
                        new XElement("BirthDate", newUser.BirthDate),
                        new XElement("Email", newUser.Email),
                        new XElement("Smoker", newUser.Smoker),
                        new XElement("SignupDate", newUser.SignupDate),
                        new XElement("Whereabouts", newUser.Whereabouts)
                    );

                    // Maintain the info about the provided username
                    newXmlUser.SetAttributeValue("principal", newUser.UserName);

                    //Register the user: upgrade to Write mode
                    userDatabaseLock.EnterWriteLock();
                    //Console.WriteLine("{0} thread obtains the write lock", Thread.CurrentThread.Name);
                    try
                    {
                        userDatabase.Element("Users").Add(newXmlUser);
                        userDatabase.Save(userDatabasePath);
                    }
                    finally
                    {
                        //Console.WriteLine("{0} thread releases the write lock", Thread.CurrentThread.Name);
                        userDatabaseLock.ExitWriteLock();
                    }

                    RegisterOkResult success = new RegisterOkResult();
                    success.Comment = "User successfully registered! Your username is : " + darPoolingUsername;
                    success.FinalUsername = darPoolingUsername;
                    return success;

                } // End else

            } // End try upgradable
            finally
            {
                //Console.WriteLine("{0} thread releases the upgradeable lock", Thread.CurrentThread.Name);
                userDatabaseLock.ExitUpgradeableReadLock();
            }
        }// End RegisterUser


        /// <summary>
        /// Login the DarPooling Service network using username and password.
        /// Check if the current node is responsible for holding data about the user.
        /// If so, the given credential are checked, otherwise the request is 
        /// scheduled for forwarding.
        /// </summary>
        /// <param name="username">The username provided by the client</param>
        /// <param name="pw_hash">The password provided by the client</param>
        /// <returns>
        /// a Result instance that represent the result of the Join operation; Specifically:
        ///  - a ConnectionErrorResult if the username has an invalid format
        ///  - a LoginErrorResult instance if the credentials don't match in the database
        ///  - a LoginOkResult if the credentials are valid.
        /// </returns>
        public Result Join(string username, string pw_hash)
        {
            Result joinResult;

            // Determine the name of the node where the user registered. Based to
            // the format, it MUST be the final token. Es.  user@baseaddress/NODENAME
            string registrationNode = username.Split('/').Last();

            // The username has an invalid format, i.e. it is impossible to retrieve the
            // name of the node where the user registered 
            if (registrationNode.Length == 0 )
            {
                joinResult = new LoginErrorResult();
                joinResult.Comment = "Invalid username and/or password";
                return joinResult;
            }

            // The user has registered in a different node. To confirm the join, we MUST
            // forward the Join request to the appropriate node.
            if (!registrationNode.Equals(this.NodeName))
            {
                ForwardRequiredResult forwardRequest = new ForwardRequiredResult();
                forwardRequest.RequestID = serviceImpl.generateGUID();
                forwardRequest.Destination = baseForwardAddress + registrationNode;
                forwardRequest.Comment = "You were not registered in this node";
                joinResult = forwardRequest;

                return joinResult;            
            }

            // Obtain the Read lock to determine if the user is actually registered.
            userDatabaseLock.EnterReadLock();
            try
            {
                //Console.WriteLine("{0} thread obtain the read lock in Join()", Thread.CurrentThread.Name);
                
                userDatabase = XDocument.Load(userDatabasePath);

                // Determine if the username and password have a match in the database
                var registeredUserQuery = (from user in userDatabase.Descendants("User")
                                           where user.Element("UserName").Value.Equals(username) &&
                                                 user.Element("Password").Value.Equals(pw_hash)
                                           select user);

                // The provided username and password don't match in the database.
                if (registeredUserQuery.Count() == 0)
                {
                    joinResult = new LoginErrorResult();
                    joinResult.Comment = "Invalid username and/or password";
                    return joinResult;
                }
                else
                {   /** The Login is successful. */
                    LoginOkResult success = new LoginOkResult();
                    success.AuthorizedUsername = username;
                    success.Comment = "Account successfully verified. You can now access DarPooling";
                    joinResult = success;
                    return joinResult;
                }
            }
            finally
            {
                //Console.WriteLine("{0} thread releases the read lock in Join()", Thread.CurrentThread.Name);
                userDatabaseLock.ExitReadLock();
            }
        }


        public Result Unjoin(string username)
        {
            Result unjoinResult;

            //Remove the user from the list of joined users, held by DarPoolingService.
            IAsyncResult asyncResult = removeJoinedUser.BeginInvoke(username, null, null);
            // Wait until the thread complete its execution.
            while (!asyncResult.IsCompleted) { }

            unjoinResult = new UnjoinConfirmedResult();
            unjoinResult.Comment = "You are logged OFF from Darpooling";
            return unjoinResult;
        }
        

        
        public Result InsertTrip(Trip newTrip)
        {
            Result insertionResult;

            /** Check if the current node is the nearest node to the 
             * departure location.
             */
            string targetNode = NearestNodeToDeparture(newTrip.DepartureName);

            if (!targetNode.Equals(NodeName))
            {
                Console.WriteLine("Decision: sending newTripCommand to : {0}", targetNode);
                ForwardRequiredResult forwardRequest = new ForwardRequiredResult();
                forwardRequest.RequestID = serviceImpl.generateGUID();
                forwardRequest.Destination = baseForwardAddress + targetNode;
               
                return forwardRequest;
                
            }
            else
            {
                //Save the trip
                tripDatabaseLock.EnterWriteLock();
                try
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
                        new XElement("DepartureName", newTrip.DepartureName.ToLower()),
                        new XElement("DepartureDateTime", newTrip.DepartureDateTime),
                        new XElement("ArrivalName", newTrip.ArrivalName.ToLower()),
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

                    Console.WriteLine("{0} Trip saved in {1}", serviceImpl.LogTimestamp, NodeName);
                    insertionResult = new InsertOkResult();
                    insertionResult.Comment = "The trip has been successfully inserted";
                    return insertionResult;
                }
                finally
                {
                    tripDatabaseLock.ExitWriteLock();
                
                }
            }// end else
          
        }//End savetrip


        private string NearestNodeToDeparture(string departure)
        {
            Location departureLoc = GMapsAPI.addressToLocation(departure);

            string targetNode = this.NodeName;
            Location tempLocation = this.NodeLocation;
            double tempDistance;
            double minDistance = tempLocation.distance(departureLoc);

            //Console.WriteLine("Distance between {0} and {1}({2}) is  {3} km", departure, targetNode, NodeGeoName, minDistance);

            foreach (ServiceNode neighbour in Neighbours)
            {
                tempLocation = neighbour.Location;
                tempDistance = tempLocation.distance(departureLoc);

                //Console.WriteLine("Distance between {0} and {1}({2}) is  {3} km", departure, neighbour.NodeName,neighbour.NodeGeoName, tempDistance);

                // Error due to decimal precision
                if (tempDistance == Double.NaN)
                    tempDistance = 0.0;

                if (tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    targetNode = neighbour.NodeName;
                }
            } 

            return targetNode;
        }



        public Result SearchTrip(QueryBuilder queryTrip)
        {
            /** Check if the current node is the nearest node to the 
             * departure location.
             */
            string targetNode = NearestNodeToDeparture(queryTrip.DepartureName);

            if (!targetNode.Equals(NodeName))
            {
                Console.WriteLine("Decision: sending SearchTripCommand to : {0}", targetNode);
                ForwardRequiredResult forwardRequest = new ForwardRequiredResult();
                forwardRequest.RequestID = serviceImpl.generateGUID();
                forwardRequest.Destination = baseForwardAddress + targetNode;

                return forwardRequest;

            }
            else
            {
                    List<Trip> matchingTrip = GetTrip(queryTrip);

                    SearchTripResult searchResult = new SearchTripResult(matchingTrip);
                    Console.WriteLine("{0} {1} Trip(s) were found in {2}", serviceImpl.LogTimestamp, matchingTrip.Count, NodeName);

                    return searchResult;   
            }

        }

        #endregion


        public List<Trip> GetTrip(QueryBuilder filterTrip)
        {
            tripDatabaseLock.EnterReadLock();
            try
            {
                tripDatabase = XDocument.Load(tripDatabasePath);

                var baseQuery = (from t in tripDatabase.Descendants("Trip")
                                 where t.Element("DepartureName").Value.Equals(filterTrip.DepartureName.ToLower()) &&
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
            finally
            {
                tripDatabaseLock.ExitReadLock();
            }

        }


        private IEnumerable<Trip> FilterQuery(QueryBuilder filterTrip, IEnumerable<Trip> filteringQuery)
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
                                 where i.ArrivalName == filterTrip.ArrivalName.ToLower()
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

        public string BaseForwardAddress
        {
            get { return baseForwardAddress; }

        }

        #endregion


        #region ServiceNode methods

        public string NodeName
        {
            get { return serviceNode.NodeName; }
            private set { serviceNode.NodeName = value; }
        }

        public string NodeGeoName
        {
            get { return serviceNode.NodeGeoName; }
            private set { serviceNode.NodeGeoName = value; }
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

        public ServiceNode[] Neighbours
        {
            get { return serviceNode.Neighbours.ToArray<ServiceNode>(); }
        }

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



//Console.WriteLine("Login successful");

//FIXME: The following code generates inconsistency when
// used by a forwarded command
// Add the user to the list of joined users.
//IAsyncResult asyncResult = addJoinedUser.BeginInvoke(username, null, null);
// Wait until the thread complete its execution.
//while (!asyncResult.IsCompleted) { }