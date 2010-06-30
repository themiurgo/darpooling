using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
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


        public string SayHello() 
        {
            return "Hi, dummy";
        }


        // Start The Service Nodes
        static void Main(string[] args)
        {
            Console.WriteLine("**** Starting the Backbone Nodes... ****");
  
            try
            {

                WSHttpBinding bind = new WSHttpBinding();
                Type contract = typeof(IDarPooling);

                ServiceHost milano = new ServiceHost(typeof(DarPooling), new Uri("http://localhost:1111/Milano"));
                ServiceHost roma = new ServiceHost(typeof(DarPooling), new Uri("http://localhost:1111/Roma"));

                roma.AddServiceEndpoint(contract, bind, "");
                milano.AddServiceEndpoint(contract, bind, "");
                

                ServiceMetadataBehavior ro_behavior = new ServiceMetadataBehavior();
                ro_behavior.HttpGetEnabled = true;

                ServiceMetadataBehavior mi_behavior = new ServiceMetadataBehavior();
                mi_behavior.HttpGetEnabled = true;
                
                roma.Description.Behaviors.Add(ro_behavior);
                milano.Description.Behaviors.Add(mi_behavior);

                roma.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
                milano.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                roma.Open();
                Console.WriteLine("ROMA node is now active. Waiting...");
                milano.Open();
                Console.WriteLine("MILANO node is now active. Waiting...");
                Console.Read();

                roma.Close();
                //
                milano.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }



            //roma.Open();

            
         

            //milano.AddServiceEndpoint(contract, mil_bind, milano_addr);

            //milano.Open();

          //  Console.WriteLine("Service Starts...");

            //DisplayHostInfo(roma);
            //DisplayHostInfo(milano);
           
        //    Console.ReadLine();
        
        }

        static void DisplayHostInfo(ServiceHost host)
        {
            Console.WriteLine();
            Console.WriteLine("**** Host Info ****");

            Console.WriteLine("Uri: {0}", host.State);
        
        }
    }
}

/*         using (ServiceHost sh = new ServiceHost(typeof(DarPooling)))
           {
               sh.Open();
               Console.WriteLine("Service Starts...");
               Console.ReadLine();
               sh.Close();
           }
           Console.ReadLine();
    */


//BindingElement bindingElement = new HttpTransportBindingElement();
//CustomBinding binding =
// new CustomBinding(bindingElement);
// bindingElement = new BasicHttpBinding();
//CustomBinding binding = new CustomBinding(bindingElement);
//Uri tcpBaseAddress = new Uri("net.tcp://localhost:9000/");
//ServiceHost host = new ServiceHost(typeof(MyService), tcpBaseAddress);
//ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
//metadataBehavior.HttpGetEnabled = true;
//roma.Description.Behaviors.Add(metadataBehavior);
//roma.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
/*metadataBehavior = roma.Description.Behaviors.Find<ServiceMetadataBehavior>();
if (metadataBehavior == null)
{
    metadataBehavior = new ServiceMetadataBehavior();
    roma.Description.Behaviors.Add(metadataBehavior);
}*/
//roma.AddServiceEndpoint(typeof(IMetadataExchange), bind, "MEX");
//host.Open();