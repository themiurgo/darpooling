using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using Communication;
using System.Threading;

namespace UserNodeCore
{
    /// <summary>
    /// This class implements the Callback interface, i.e. the set
    /// of methods that the service will call back when the result
    /// is ready.
    /// </summary>
    public class ClientCallback : IDarPoolingCallback
    {
        private UserNodeCore parent;

        public ClientCallback() { }

        public ClientCallback(UserNodeCore parent)
        {
            this.parent = parent;
        }

        public UserNodeCore Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public void GetResult(Result result)
        {
            Console.WriteLine("Service says: " + result.Comment);
            
            // FIXME: This line MUST BE decommented when using GUI
            parent.resultCallback(result);
            
        }
    }

    /// <summary>
    /// UserNodeCore class is composed of informations of UserNode plus the status
    /// of its connection. Also, it allows to execute actions that will have
    /// consequences on both the UserNode and the Darpooling network.
    /// </summary>
    public class UserNodeCore
    {
        private UserNode userNode;
        private IState state;
        private List<SearchTripResult> results;
        private IDarPooling serviceProxy;

        public delegate void ResultReceiveHandler(Result r);
        public ResultReceiveHandler resultCallback;
        private IDarPoolingCallback clientCallback;

        public IDarPoolingCallback ClientCallback
        {
            get { return clientCallback; }
        }

        private void onResultReceive(Result result)
        {
            if (result.GetType() == typeof(LoginOkResult))
                state = new JointState();
            /*
             * UnjointState case not present here: the client sets its
             * new Unjointstate right after it has sent the UnjoinCommand.
             */

        }

        /// <summary>
        /// Setup a new UserNodeCore.
        /// </summary>
        /// <param name="clientNode">represents the UserNode and its settings.</param>
        public UserNodeCore(UserNode clientNode)
        {
            results = new List<SearchTripResult>();
            state = new UnjointState();
            userNode = clientNode;
            clientCallback = new ClientCallback();
            ((ClientCallback) clientCallback).Parent = this;
            resultCallback += new ResultReceiveHandler(onResultReceive);
        }

        public bool Connected
        {
            get {
                if (serviceProxy != null)
                    return true;
                else
                    return false;
            }
        }

        public UserNode UserNode
        {
            get { return userNode; }
            set { }
        }

        public IDarPooling ServiceProxy
        {
            get { return serviceProxy; }
            set { serviceProxy = value; }
        }

        /// <summary>
        /// Represent the state of the connection to the network, according to
        /// State pattern.
        /// </summary>
        public IState State
        {
            get { return state; }
            set { state = value; }
        }

        public void RegisteredUser (User user, string registrarAddress)
        {
            state.RegisterUser(this, user);
        }
        
        /// <summary>
        /// Join (connect) to the network, through a ServiceNode.
        /// </summary>
        /// <param name="serviceNodeAddress">address of the ServiceNode</param>
        public void Join(string username, string password,
            string serviceNodeAddress, string callbackAddress)
        {
            userNode.User = new User();
            userNode.User.UserName = username;
            state.Join(this, username, password, serviceNodeAddress,
                callbackAddress);
        }

        /// <summary>
        /// Unjoin (disconnect) from the network.
        /// </summary>
        public void Unjoin()
        {
            state.Unjoin(this);
        }

        public void InsertTrip(Trip trip)
        {
            state.InsertTrip(this, trip);
        }

        public void SearchTrip()
        {
            state.SearchTrip(this);
        }

        // Console-Client, used for debug purposes
        public static void Main()
        {
            UserNodeCore user = new UserNodeCore(new UserNode("prova"));
            Console.WriteLine("***** DarPooling Client Console Testing  *****\n\n");

            User dummy = new User
            {
                UserName = "Dummy",
                Password = "shaoran",
                Name = "Daniele",
                UserSex = User.Sex.m,
                BirthDate = new DateTime(1986, 04, 08),
                Email = "danielemar86@gmail.com",
                Smoker = false,
                SignupDate = DateTime.Now.AddDays(-30),
                Whereabouts = ""
            };

            Trip trip1 = new Trip
            {
                Owner = "Shaoran@http://localhost:1111/Milano",
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


            // Case 4: LoginForward
            Console.ReadLine();
            Console.WriteLine("Press a key... (Forward expected)");
            Console.ReadLine();
            Console.WriteLine("Key pressed!");
            user.Join("Shaoran@http://localhost:1111/Milano", "shaoran", "http://localhost:1111/Catania",
    "http://localhost:2222/prova");

            
            Console.ReadLine();
            Console.WriteLine("Press a key... (Unjoin)");
            Console.ReadLine();
            UnjoinCommand unjoin = new UnjoinCommand("Shaoran@http://localhost:1111/Milano");
            TestCommands(unjoin);
            

            Console.ReadLine();
            Console.WriteLine("Press a key... (Register)");
            Console.ReadLine();
            RegisterUserCommand register = new RegisterUserCommand(dummy);
            TestCommands(register);
            //TestCommands(register);

            Console.ReadLine();
            Console.WriteLine("Press a key... (Insert Trip)");
            Console.ReadLine();
            InsertTripCommand insert = new InsertTripCommand(trip1);
            TestCommands(insert);
           
            Console.ReadLine();
        }

        public static void TestCommands(Command c)
        {
            string serviceNodeAddress = "http://localhost:1111/Catania";
            string callbackAddress = "http://localhost:2222/prova";
            
            ClientCallback callback = new ClientCallback();
         
            // First of all, set up the connection
            EndpointAddress endPointAddress = new EndpointAddress(serviceNodeAddress);
            WSDualHttpBinding binding = new WSDualHttpBinding();
            binding.ClientBaseAddress = new Uri(callbackAddress);
            DuplexChannelFactory<IDarPooling> factory = new DuplexChannelFactory<IDarPooling>(
                    callback, binding, endPointAddress);

            IDarPooling serviceProxy = factory.CreateChannel();

            serviceProxy.HandleDarPoolingRequest(c);
        }


        private string LogTimestamp
        {
            get
            {
                string time = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                return ("[" + time + "] ");
            }
        }
    }


}


//Console.WriteLine(" {0}", Tools.HashString(DateTime.Now.ToString() + "1") );
//Console.WriteLine(" {0}", Tools.HashString(DateTime.Now.ToString() + "2" ));
// In order: username, password (blank), Service Addr, Callback Addr.
/*  
  // Case 1: LoginError
  Console.WriteLine("Press a key... (Error expected)");
  Console.ReadLine();
  user.Join("Shaoran@http://localhost:1111/", "shaoran", "http://localhost:1111/Catania",
      "http://localhost:2222/prova");
            
  // Case 2: LoginInvalid
  Console.ReadLine();
  Console.WriteLine("Press a key... (Invalid expected)");
  Console.ReadLine();
  user.Join("Anto@http://localhost:1111/Catania", "XxXxXXxxX", "http://localhost:1111/Catania",
"http://localhost:2222/prova");
            
  // Case 3: LoginOk
  Console.ReadLine();
  Console.WriteLine("Press a key... (Login OK expected)");
  Console.ReadLine();
  user.Join("Anto@http://localhost:1111/Catania", "anto", "http://localhost:1111/Catania",
"http://localhost:2222/prova");
  */

// Case 4: LoginForward
/*
Console.ReadLine();
Console.WriteLine("Press a key... (Forward expected)");
Console.ReadLine();
user.Join("Shaoran@http://localhost:1111/Milano", "shaoran", "http://localhost:1111/Catania",
"http://localhost:2222/prova");
*/