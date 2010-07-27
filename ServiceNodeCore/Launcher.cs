using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Communication;

namespace ServiceNodeCore
{
    /* Start the SNs */
    public class Launcher
    {
        private static List<ServiceNodeCore> sncList = new List<ServiceNodeCore>();
        private static List<User> userList = new List<User>();
        private static List<Trip> tripList = new List<Trip>();
        private static Dictionary<string, Location> nameLoc = new Dictionary<string, Location>();

        static void Main(string[] args)
        {
            InitializeService();
            StartService();
            Console.ReadLine();
            TestService();
            Console.ReadLine();
            StopService();
        }

        public static void InitializeService()
        {
            InitializeNodes();
            InitializeUserDB();
            InitializeTripDB();
        }


        public static void InitializeNodes()
        {
            Console.WriteLine("**** DarPooling Service Nodes ****\n");

            string[] locNames = new string[] { "Aosta", "Milano", "Roma", "Napoli", "Catania" };
            string[] coords;
            double latitude;
            double longitude;
            Location location;

            /* Obtain the Location of the Node */
            Console.Write("Retrieving Coordinates from GMap Server....   ");
            foreach (string locName in locNames)
            {
                coords = GMapsAPI.addrToLatLng(locName);
                latitude = double.Parse(coords[0]);
                longitude = double.Parse(coords[1]);
                location = new Location(locName, latitude, longitude);
                nameLoc.Add(locName, location);
            }
            Console.WriteLine("Done!");

            /* Service Node(s) */
            ServiceNode aostaSN = new ServiceNode("Aosta", nameLoc["Aosta"]);
            ServiceNode milanoSN = new ServiceNode("Milano", nameLoc["Milano"]);
            ServiceNode romaSN = new ServiceNode("Roma", nameLoc["Roma"]);
            ServiceNode napoliSN = new ServiceNode("Napoli", nameLoc["Napoli"]);
            ServiceNode cataniaSN = new ServiceNode("Catania", nameLoc["Catania"]);
            ServiceNode catania2SN = new ServiceNode("Catania2", nameLoc["Catania"]);
            /* Service Node Core(s) */
            ServiceNodeCore aosta = new ServiceNodeCore(aostaSN);
            ServiceNodeCore milano = new ServiceNodeCore(milanoSN);
            ServiceNodeCore roma = new ServiceNodeCore(romaSN);
            ServiceNodeCore napoli = new ServiceNodeCore(napoliSN);
            ServiceNodeCore catania = new ServiceNodeCore(cataniaSN);
            ServiceNodeCore catania2 = new ServiceNodeCore(catania2SN);

            /* Set Topology */
            aosta.addNeighbour(milano);
            milano.addNeighbour(roma);
            roma.addNeighbour(napoli);
            napoli.addNeighbour(catania);
            napoli.addNeighbour(catania2);
            catania.addNeighbour(catania2);

            /* Set of Backbone Nodes */
            ServiceNodeCore[] nodes =
                new ServiceNodeCore[] { catania //, catania2, napoli, roma, milano, aosta 
                                      };

            sncList.AddRange(nodes);

        }


        public static void InitializeUserDB()
        {
            Console.Write("Initializing Users DB... ");
            string usersDBPath = @"..\..\..\config\users.xml";

            XmlTextWriter textWriter = new XmlTextWriter(usersDBPath, System.Text.Encoding.UTF8);
            textWriter.Formatting = Formatting.Indented;
            textWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            textWriter.WriteComment("Users DB for the DarPooling Service");
            textWriter.WriteStartElement("Users");
            textWriter.Close();

            CreateUsers();

            foreach (User u in userList)
            {
                DarPoolingService.SaveUser(u);
            }
            Console.WriteLine("Done!");

        }

        public static void CreateUsers()
        {
            User daniele = new User
            {
                //UserID = 0,
                UserName = "Shaoran",
                Name = "Daniele",
                UserSex = User.Sex.m,
                BirthDate = new DateTime(1986, 04, 08),
                Email = "danielemar86@gmail.com",
                Smoker = false,
                SignupDate = DateTime.Today.AddDays(-30),
                Whereabouts = ""

            };

            User antonio = new User
            {
                //UserID = 1,
                UserName = "AnT0",
                Name = "Antonio",
                UserSex = User.Sex.m,
                BirthDate = new DateTime(1987, 06, 12),
                Email = "anto87@gmail.com",
                Smoker = false,
                SignupDate = DateTime.Today.AddDays(-30),
                Whereabouts = ""
            };

            User[] users =
                new User[] { daniele, antonio
                           };

            userList.AddRange(users);
        }


        public static void InitializeTripDB()
        {
            Console.Write("Initializing Trips DB... ");

            foreach (ServiceNodeCore snc in sncList)
            {
                string filename = snc.TripsDBPath;
                XmlTextWriter textWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
                textWriter.Formatting = Formatting.Indented;
                textWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                textWriter.WriteComment("Trips DB for " + snc.NodeName + " DarPooling Service Node");
                textWriter.WriteStartElement("Trips");
                textWriter.Close();
            }

            CreateTrips();

            ServiceNodeCore randomNode = sncList.ElementAt(0);
            foreach (Trip t in tripList)
            {
                randomNode.SaveTrip(t);
            }
            Console.WriteLine("Done!");
        }

        public static void CreateTrips()
        {
            Trip trip1 = new Trip
            {
                //ID = 0,
                Owner = userList.ElementAt(0).UserName,
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

            Trip trip2 = new Trip
            {
                //ID = 1,
                Owner = userList.ElementAt(0).UserName,
                DepartureName = "Catania",
                DepartureDateTime = new DateTime(2010, 8, 4, 8, 0, 0),
                ArrivalName = "Messina",
                ArrivalDateTime = new DateTime(2010, 8, 4, 10, 30, 0),
                Smoke = false,
                Music = false,
                Cost = 100,
                FreeSits = 2,
                Notes = "none",
                Modifiable = false
            };

            Trip trip3 = new Trip
            {
                //ID = 2,
                Owner = userList.ElementAt(0).UserName,
                DepartureName = "Catania",
                DepartureDateTime = new DateTime(2010, 8, 4, 18, 0, 0),
                ArrivalName = "Ragusa",
                ArrivalDateTime = new DateTime(2010, 8, 4, 20, 30, 0),
                Smoke = false,
                Music = false,
                Cost = 80,
                FreeSits = 3,
                Notes = "none",
                Modifiable = false
            };

            Trip[] trips =
                new Trip[] { trip1, trip2, trip3
                           };

            tripList.AddRange(trips);

        }


        public static void TestService()
        {
            ServiceNodeCore randomNode = sncList.ElementAt(0);
            Trip queryTrip = new Trip { DepartureName = "Catania", ArrivalDateTime = new DateTime(2010,08,4,19,0,0)};
            //queryTrip.PrintFullInfo();
            List<Trip> list = randomNode.GetTrip(queryTrip);
            Console.WriteLine("Retrieved {0} trip(s).", list.Count());
            Console.ReadLine();
            foreach (Trip t in list)
            {
                t.PrintFullInfo();
            }
        }


        public static void StartService()
        {
            Console.WriteLine("\nStarting the Service Nodes...");
            foreach (ServiceNodeCore node in sncList)
            {
                node.StartService();
            }
            Console.WriteLine("ALL Service Nodes are now ONLINE");
            PrintDebug();
        }


        public static void StopService()
        {
            Console.WriteLine("Stopping the Service Nodes...");
            foreach (ServiceNodeCore node in sncList)
            {
                node.StopService();
            }
            Console.WriteLine("ALL Service Nodes are now OFFLINE Quitting...");
        }



        public static void PrintDebug()
        {
            Console.WriteLine("\n**** DEBUG **** \nNODE INFO");
            foreach (ServiceNodeCore n in sncList)
            {
                Console.WriteLine("I'm {0}. My Coords are : {1} and {2}. I have {3} neighbours", n.NodeName, n.NodeLocation.Latitude, n.NodeLocation.Longitude, n.NumNeighbours);
            }

        }

    } //End Launcher


}
