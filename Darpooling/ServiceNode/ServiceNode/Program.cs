using System;
using System.Collections.Generic;
using System.Linq;
//using System.Math;

namespace ServiceNode
{
    class Location
    {
        double latitude;
        double longitude;
        double r = 6378;

        public Location(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }


        public double distance(Location other)
        {
            double a1 = this.latitude;
            double b1 = this.longitude;
            double a2 = other.latitude;
            double b2 = other.longitude;
            return Math.Acos(Math.Cos(a1)*Math.Cos(b1)*Math.Cos(a2)*Math.Cos(b2) + Math.Cos(a1)*Math.Sin(b1)*Math.Cos(a2)*Math.Sin(b2) + Math.Sin(a1)*Math.Sin(a2)) * r;
        }
    }

    class Node
    {
        string address;
        string location;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Location p1 = new Location(Math.PI/2, Math.PI/2);
            Location p2 = new Location(Math.PI/2, Math.PI);
            Console.WriteLine(p1.distance(p2));
        }
    }
}
