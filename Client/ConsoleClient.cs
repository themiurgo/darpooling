using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Communication;
using Client.ServiceRef;

namespace Client
{
    public class ClientCore
    {
        private UserNode myNode;
        private IState state;

        public ClientCore(UserNode clientNode)
        {
            state = new DisconnectedState();
            myNode = clientNode;
        }

        public IState State
        {
            get { return state; }
            set { state = value; }
        }
        
        public void Join(string address)
        {
            state.Join(this, address);
        }

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
    }

    public class ConsoleClient
    {
        static void Main(string[] args)
        {
            // Mock-up connessione
            Communication.UserNode myNode = new UserNode();
            ClientCore clientCore = new ClientCore(myNode);

            clientCore.Join("localhost");
            clientCore.SearchTrip();
            clientCore.InsertTrip(new Trip());
            clientCore.Unjoin();


            /*
           
           
            SimpleUser[] users = new SimpleUser[] 
            {
                new SimpleUser { userId=1, Name="Antonio", userName="anto"},
                new SimpleUser { userId=2, Name="Daniele", userName="dani"},
                new SimpleUser { userId=100, Name="Follia", userName="folle"},
            
            };

            PrintUsers(users);

            using (ServiceNodeProxy proxy = new ServiceNodeProxy())
            {
                Console.WriteLine("Processing...");
                SimpleUser[]  result = proxy.GetSimpleUsers(users);
                Console.WriteLine("Result :");
                PrintUsers(result);

            }
            Console.ReadLine();
             
            */
        }

        static void PrintUsers(SimpleUser[] array)
        {
            foreach (SimpleUser su in array)
            {
                Console.WriteLine(" {0}, {1}, {2}", su.userId, su.Name, su.userName);
            }        
        }
    }
}