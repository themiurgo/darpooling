using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Communication;
using Client.ServiceRef;

namespace Client
{
    class ConsoleClient
    {
        static void Main(string[] args)
        {
            SimpleUser[] users = new SimpleUser[] 
            {
                new SimpleUser { userId=1, Name="Antonio", userName="anto"},
                new SimpleUser { userId=2, Name="Daniele", userName="dani"},
                new SimpleUser { userId=100, Name="Follia", userName="folle"},
            
            };

            PrintUsers(users);

            using (DarPoolingClient proxy = new DarPoolingClient())
            {
                Console.WriteLine("Processing...");
                SimpleUser[]  result = proxy.GetSimpleUsers(users);
                Console.WriteLine("Result :");
                PrintUsers(result);

            }
            Console.ReadLine();

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
