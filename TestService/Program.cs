using System;
using System.ServiceModel;

namespace OpenNETCF.WCF.Sample
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Net;

    namespace OpenNETCF.WCF.Sample
    {
        class Server
        {
            static void Main(string[] args)
            {

                Console.WriteLine("Starting Sample WCF...");

                // Set address, binding, contract and behavior of the Service
                Uri http_uri = new Uri("http://localhost:1111/Calculator");
                BasicHttpBinding binding = new BasicHttpBinding();
                Type contract = typeof(ICalculator);
                ServiceMetadataBehavior mex_behavior = new ServiceMetadataBehavior();
                mex_behavior.HttpGetEnabled = true;

                // Hosting the services
                ServiceHost serviceHost = new ServiceHost(typeof(CalculatorService), http_uri);
                serviceHost.AddServiceEndpoint(contract, binding, "");
                serviceHost.Description.Behaviors.Add(mex_behavior);
                serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                // Run the service
                serviceHost.Open();

                Console.ReadLine();

                // Close the ServiceHostBase to shutdown the service.
                serviceHost.Close();
            }
        }
    }
}
