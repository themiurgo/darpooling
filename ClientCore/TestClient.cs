    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using Communication;

namespace ClientCore
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Compact */
            IDarPooling client = ChannelFactory<IDarPooling>.CreateChannel(
                                 new WSHttpBinding(), new EndpointAddress(
                                 "http://localhost:1111/Roma/"
                                 ));

            /* Verbose */
            EndpointAddress address = new EndpointAddress("http://localhost:1111/Milano");
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<IDarPooling> channelFactory = new ChannelFactory<IDarPooling>(binding);
            IDarPooling client2 = channelFactory.CreateChannel(address);


            Console.WriteLine(client.SayHello());
            Console.WriteLine(client2.SayHello());
            Console.ReadLine();


        }
    }
}