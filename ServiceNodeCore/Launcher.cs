using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.Threading;

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
            
            //TestService();
            Console.ReadLine();
            //StopService();
        }

        public static void InitializeService()
        {
            InitializeNodes(false);
            PopulateUserDB();
            //PopulateTripDB();
        }

        /// <summary>
        /// Retrieve the coordinates from location names, build the ServiceNode
        /// and ServiceNodeCore objects, set the topology of the net of service nodes.
        /// </summary>
        public static void InitializeNodes(bool fullTopology)
        {
            Console.WriteLine("**** DarPooling Service Nodes ****\n");

            string[] locNames = new string[] { "Venezia", "Milano", "Torino", "Genova", "Pisa", "Roma",
                                                "Napoli", "Bari", "CT Aci Trezza", "CT Nicolosi", "Bologna"
                                               };

            // Obtain the Location coordinates from the location name
            Console.WriteLine("Retrieving Coordinates from GMap Server....   ");
            foreach (string locName in locNames)
            {
                Location location = GMapsAPI.addressToLocation(locName);
                nameLoc.Add(locName, location);
            }

            foreach (string locName in locNames)
            {
                //Console.WriteLine("Dtance between {0} and {1} is  {2} km", locNames[4], locName, nameLoc[locNames[4]].distance(nameLoc[locName]));
            }

            Console.WriteLine("Done!");

            // ServiceNode
            ServiceNode veneziaSN = new ServiceNode("Venezia", locNames[0], nameLoc[locNames[0]]);
            ServiceNode milanoSN = new ServiceNode("Milano", locNames[1], nameLoc[locNames[1]]);
            ServiceNode torinoSN = new ServiceNode("Torino", locNames[2], nameLoc[locNames[2]]);
            ServiceNode genovaSN = new ServiceNode("Genova", locNames[3], nameLoc[locNames[3]]);
            ServiceNode pisaSN = new ServiceNode("Pisa", locNames[4], nameLoc[locNames[4]]);
            ServiceNode romaSN = new ServiceNode("Roma", locNames[5], nameLoc[locNames[5]]);
            ServiceNode napoliSN = new ServiceNode("Napoli", locNames[6], nameLoc[locNames[6]]);
            ServiceNode bariSN = new ServiceNode("Bari", locNames[7], nameLoc[locNames[7]]);
            ServiceNode cataniaSN = new ServiceNode("Catania", locNames[8], nameLoc[locNames[8]]);
            ServiceNode catania2SN = new ServiceNode("Catania2", locNames[9], nameLoc[locNames[9]]);
            ServiceNode bolognaSN = new ServiceNode("Bologna", locNames[10], nameLoc[locNames[10]]);
            // ServiceNodeCore
            ServiceNodeCore venezia = new ServiceNodeCore(veneziaSN);
            ServiceNodeCore milano = new ServiceNodeCore(milanoSN);
            ServiceNodeCore torino = new ServiceNodeCore(torinoSN);
            ServiceNodeCore genova = new ServiceNodeCore(genovaSN);
            ServiceNodeCore pisa = new ServiceNodeCore(pisaSN);
            ServiceNodeCore roma = new ServiceNodeCore(romaSN);
            ServiceNodeCore  napoli = new ServiceNodeCore(napoliSN);
            ServiceNodeCore bari = new ServiceNodeCore(bariSN);
            ServiceNodeCore catania = new ServiceNodeCore(cataniaSN);
            ServiceNodeCore catania2 = new ServiceNodeCore(catania2SN);
            ServiceNodeCore bologna = new ServiceNodeCore(bolognaSN);



            // Set Topology
            if (fullTopology)
            {
                venezia.addNeighbour(milano);
                venezia.addNeighbour(bologna);

                milano.addNeighbour(torino);
                milano.addNeighbour(genova);

                torino.addNeighbour(genova);

                genova.addNeighbour(pisa);

                bologna.addNeighbour(pisa);

                pisa.addNeighbour(roma);
                roma.addNeighbour(napoli);
                napoli.addNeighbour(bari);
                napoli.addNeighbour(catania);
                napoli.addNeighbour(catania2);
                catania.addNeighbour(catania2);
            }
            else
            {
                milano.addNeighbour(roma);
                roma.addNeighbour(catania);
            }

            ServiceNodeCore[] nodes;

            if (fullTopology)
            {
                nodes =
                    new ServiceNodeCore[] { venezia, milano, torino, genova, bologna, pisa, roma, napoli, bari, catania, catania2
                                      };
            }
            else
            {
                nodes =
                    new ServiceNodeCore[] { milano, roma, catania //,venezia, torino, genova, pisa, roma, napoli, bari, catania2, bologna
                                      };
            }
            sncList.AddRange(nodes);

        }


        /// <summary>
        /// Create some sample users and save them into
        /// the user database of a ServiceNodeCore.
        /// </summary>
        public static void PopulateUserDB()
        {
            Console.WriteLine("Initializing Users DB... \n");

            #region Create some sample user
            User daniele = new User
            {
                UserName = "daniele",
                Password = "dani",
                Name = "Daniele",
                UserSex = User.Sex.m,
                BirthDate = new DateTime(1986, 04, 08),
                Email = "danielemar86@gmail.com",
                Smoker = false,
                SignupDate = DateTime.Now.AddDays(-30),
                Whereabouts = ""
            };

            User antonio = new User
            {
                UserName = "antonio",
                Password = "anto",
                Name = "Antonio",
                UserSex = User.Sex.m,
                BirthDate = new DateTime(1987, 06, 12),
                Email = "anto87@gmail.com",
                Smoker = false,
                SignupDate = DateTime.Now.AddDays(-30),
                Whereabouts = ""
            };
            #endregion

            User[] users =new User[] { daniele, antonio };
            userList.AddRange(users);

            ServiceNodeCore firstNode = sncList.First();
            ServiceNodeCore lastNode = sncList.Last();
            Thread[] threads = new Thread[4];

            Thread registerThread;

            foreach (ServiceNodeCore snc in sncList)
            {
                snc.RegisterUser(daniele);
                snc.RegisterUser(antonio);

            }

            /*
            threads[0] = new Thread(() => firstNode.RegisterUser(daniele));
            threads[0].Name = "Register Daniele";

            threads[1] = new Thread(() => lastNode.RegisterUser(antonio));
            threads[1].Name = "Register Antonio";

            //threads[2] = new Thread(() => firstNode.RegisterUser(dummy));
            //threads[2].Name = "Register Dummy";
            //threads[2].Start();

            /*
            threads[2] = new Thread(() => firstNode.Join("Shaoran","shaoran"));
            threads[2].Name = "Join Daniele";

            threads[3] = new Thread(() => firstNode.Unjoin("Shaoran"));
            threads[3].Name = "UnJoin Daniele ";
            
            // Testing the concurrency
            threads[0].Start();
            threads[1].Start();
            */
            //Thread.Sleep(200);
            //threads[2].Start();
            //Thread.Sleep(1000);
            //Thread.Sleep(500);
            //threads[3].Start();

            //Console.WriteLine("Done!");

        }


        /// <summary>
        /// Create some sample trips and save them into
        /// the trip database of a ServiceNodeCore. 
        /// </summary>
        public static void PopulateTripDB()
        {
            Console.Write("Initializing Trips DB... ");

            #region Sample trips

            Trip trip1 = new Trip
            {
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

            #endregion

            Trip[] trips = new Trip[] { trip1, trip2, trip3 };
            tripList.AddRange(trips);

            ServiceNodeCore randomNode = sncList.ElementAt(0);
            foreach (Trip t in tripList)
            {
                randomNode.InsertTrip(t);
            }

            Console.WriteLine("Done!");
        }


        public static void StartService()
        {
            Console.WriteLine("\nStarting the Service Nodes...");
            foreach (ServiceNodeCore node in sncList)
            {
                node.StartService();
            }
            Console.WriteLine("ALL Service Nodes are now ONLINE");
            //PrintDebug();
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


        public static void TestService()
        {
            /*
            ServiceNodeCore randomNode = sncList.ElementAt(0);
            Trip queryTrip = new Trip { DepartureName = "Catania", ArrivalDateTime = new DateTime(2010,08,4,19,0,0)};
            //queryTrip.PrintFullInfo();
            List<Trip> list = randomNode.GetTrip(queryTrip);
            Console.WriteLine("Retrieved {0} trip(s).", list.Count());
            Console.ReadLine();
            foreach (Trip t in list)
            {
                t.PrintFullInfo();
            }*/

            //Console.WriteLine("\nWaiting for incoming request...");
        }


        public static void CreateTrips()
        {

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
