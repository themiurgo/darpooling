using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Communication;
using System.Runtime.Serialization;

namespace DarPoolingNode
{
    public class DarPooling : IDarPooling
    {
        public SimpleUser[] GetSimpleUsers(SimpleUser[] inputUsers)
        {

            List<SimpleUser> result = new List<SimpleUser>();
            foreach (SimpleUser su in inputUsers)
            {
                if (su.userId < 10)
                {
                    result.Add(su);
                }

            }

            return result.ToArray();
        }

        static void Main(string[] args)
        {
   /*         using (ServiceHost sh = new ServiceHost(typeof(DarPooling)))
            {
                sh.Open();
                Console.WriteLine("Service Starts...");
                Console.ReadLine();
                sh.Close();
            }
            Console.ReadLine();
     */
            ServiceHost roma = new ServiceHost(typeof(DarPooling));
            ServiceHost milano = new ServiceHost(typeof(DarPooling));

            Uri roma_addr = new Uri("http://localhost:3333/Roma");
            WSHttpBinding bind = new WSHttpBinding();
            Type contract = typeof(IDarPooling);

            roma.AddServiceEndpoint(contract, bind, roma_addr);

            roma.Open();


            Uri milano_addr = new Uri("http://localhost:3332/Milano");
            WSHttpBinding mil_bind = new WSHttpBinding();
         

            milano.AddServiceEndpoint(contract, mil_bind, milano_addr);

            milano.Open();

            Console.WriteLine("Service Starts...");

            DisplayHostInfo(roma);
            DisplayHostInfo(milano);
           
            Console.ReadLine();
        
        }

        static void DisplayHostInfo(ServiceHost host)
        {
            Console.WriteLine();
            Console.WriteLine("**** Host Info ****");

            Console.WriteLine("Uri: {0}", host.State);
        
        }
    }
}
