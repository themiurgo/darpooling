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

    class Program
    {
        private static List<UserNodeCore> uncList = new List<UserNodeCore>();
        private static List<User> userList = new List<User>();
        //private static List<Trip> tripList = new List<Trip>();
        //private static Dictionary<string, Location> nameLoc = new Dictionary<string, Location>();

        static void Main(string[] args)
        {
            InitializeClient();
            StartCommunication();
            Console.ReadLine();
            EndCommunication();
            
        }


        static void InitializeClient()
        {
            CreateUsers();
            
            /* Clients */
            UserNode catania1UN = new UserNode(userList.ElementAt(1), "Catania");
            UserNode milano1UN = new UserNode("4nT0", "Milano");

            UserNodeCore catania1 = new UserNodeCore(catania1UN);
            UserNodeCore milano1 = new UserNodeCore(milano1UN);

            UserNodeCore[] userNodes = 
                new UserNodeCore[] { catania1, milano1
                                   };

            uncList.AddRange(userNodes);
        
        }

        public static void CreateUsers()
        {
            User daniele = new User
            {
                UserName = "Shaoran",
                Password = "shaoran",
                Name = "Daniele",
                UserSex = User.Sex.m,
                BirthDate = new DateTime(1986, 04, 08),
                Email = "danielemar86@gmail.com",
                Smoker = false,
                SignupDate = DateTime.Today.AddDays(-30)
            };

            User ubuntu = new User
            {   
                UserName = "ubuntu",
                Password = "ubuntu",
                Name = "Ubuntu",
                UserSex = User.Sex.m,
                BirthDate = new DateTime(1987, 06, 12),
                Email = "chief@ubuntu.org",
                Smoker = false,
                SignupDate = DateTime.Today.AddDays(-30),
                Whereabouts = "Linux for human being"
            };

            User[] users =
                new User[] { daniele, ubuntu
                           };

            userList.AddRange(users);
        }


        /* Callback */
        static void StartCommunication()
        { 
            Console.WriteLine("*****  Test HTTP CALLBACK Client  *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();
           
            User testUser = userList.ElementAt(0);
            UserNodeCore testClient = uncList.ElementAt(0);

            LoginUserCommand login = new LoginUserCommand(testUser.UserName, testUser.Password);

            testClient.ConnectToService(login);
            //milano1.ConnectToService();
            
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            
            //proxy.GetData("Gimme Trips");
        }

        static void EndCommunication()
        { 
        
        }


        static void NoCallbackTCPClient()
        {
            /* Verbose */
            EndpointAddress address = new EndpointAddress("net.tcp://localhost:1112/Milano");
            NetTcpBinding binding = new NetTcpBinding();
            //WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IDarPooling> channelFactory = new ChannelFactory<IDarPooling>(binding);
            IDarPooling client2 = channelFactory.CreateChannel(address);

            //Thread.Sleep(7000);
            Console.WriteLine("*****  Test TCP Client  (NO Callback) *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();

            //Console.WriteLine(client2.SayHello());
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            Console.ReadLine();

        }


        static void NoCallbackHTTPClient()
        {
            /* Verbose */
            EndpointAddress address = new EndpointAddress("http://localhost:1111/Milano");
            WSDualHttpBinding binding = new WSDualHttpBinding();
            binding.ClientBaseAddress = new Uri("http://localhost:2222/Client1");
            ChannelFactory<IDarPooling> channelFactory = new ChannelFactory<IDarPooling>(binding);
            IDarPooling client2 = channelFactory.CreateChannel(address);

            //Thread.Sleep(7000);
            Console.WriteLine("*****  Test HTTP Client  (NO Callback) *****");
            Console.WriteLine("\n\nPress a key to start the communication");
            Console.ReadLine();

            //Console.WriteLine(client2.SayHello());
            Console.WriteLine("\n\n\nClient is now ready to perform some other task");
            Console.ReadLine();
        
        }


    } // End Program
} // End Namespace


/* Compact
IDarPooling client = ChannelFactory<IDarPooling>.CreateChannel(
                     new WSHttpBinding(), new EndpointAddress(
                     "http://localhost:1111/Roma/"
                     ));
*/
/* Address */
//EndpointAddress serviceAddress = new EndpointAddress("http://localhost:1111/Catania");
/* Binding */
//WSDualHttpBinding binding = new WSDualHttpBinding();
//binding.ClientBaseAddress = new Uri("http://localhost:2222/Client1"); //Callback address
/* (Callback) contract  */
//IDarPoolingCallback callback = new ClientCallback();
/** Channels */
//DuplexChannelFactory<IDarPooling> factory = new DuplexChannelFactory<IDarPooling>(callback, binding, serviceAddress);
//IDarPooling proxy = factory.CreateChannel();




/* public class ContractClient : DuplexClientBase<IDarPooling>
 { 
     public ContractClient(object callbackInstance, Binding binding, EndpointAddress remoteAddress)
     : base(callbackInstance, binding, remoteAddress) { }
 }

 public class CallbackClient : IDarPoolingCallback
 {
     public void OnCallback()
     {
         Console.WriteLine("Hi from client!");
     }
 }
*/

/* WSDualHttpBinding binding = new WSDualHttpBinding();
      var callback = new CallbackClient();
      var client3 = new ContractClient(callback, binding, new EndpointAddress(""));
      var proxy = client3.ChannelFactory.CreateChannel();
      proxy.SayHello(); 
      */


/*
static void CallbackClient()
{
    IDarPoolingCallback callback = new ClientCallback();
    WSDualHttpBinding binding = new WSDualHttpBinding();
    binding.ClientBaseAddress = new Uri("http://localhost:2222/Client1");
    EndpointAddress address = new EndpointAddress("http://localhost:1111/Milano");
            
    DuplexChannelFactory<IDarPooling> factory = new DuplexChannelFactory<IDarPooling>(callback,binding,address);
    IDarPooling proxy = factory.CreateChannel();

    Console.WriteLine("*****  Test HTTP CALLBACK Client  *****");
    Console.WriteLine("\n\nPress a key to start the communication");
    Console.ReadLine();
    proxy.GetData("Gimme Trips");
    Console.WriteLine("\n\n\nClient is now ready to perform some other task");
    Console.ReadLine();

}
*/
