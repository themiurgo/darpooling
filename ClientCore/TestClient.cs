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
            IDarPooling client = ChannelFactory<IDarPooling>.CreateChannel(
                                 new WSHttpBinding(), new EndpointAddress(
                                 "http://localhost:1111/Roma/"
                                 ));
            Console.WriteLine(client.SayHello());
            Console.ReadLine();


        }
    }
}
