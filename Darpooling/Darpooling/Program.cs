using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darpooling
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
            Console.WriteLine("New Location {0} {1}", latitude, longitude);
        }


        public double distance(Location other)
        {
            double a1 = this.latitude;
            double b1 = this.longitude;
            double a2 = other.latitude;
            double b2 = other.longitude;


            double a = Math.Acos(Math.Cos(a1) * Math.Cos(b1) * Math.Cos(a2) * Math.Cos(b2) + Math.Cos(a1) * Math.Sin(b1) * Math.Cos(a2) * Math.Sin(b2) + Math.Sin(a1) * Math.Sin(a2)) * r;
            Console.WriteLine("{0}", a);
            return a;
        }
    }

    public class User
    {
        enum Sex {f, m};
        int userId;
        String username;
        String name;
        Sex sex;
        DateTime birthDate;
        String email;
        Boolean smoker;
        DateTime signupDate;
        String whereabouts;

        public User(String name) 
        {
            this.name = name;
        }

        public String Name 
        {
            get 
            {
                return name;
            }
         }
    }

    public class Trip
    {
        Int32 id;
        DateTime departureDateTime;
        DateTime arrivalDateTime;
        String departureName;
        String arrivalName;   
        Boolean smoke;
        Boolean music;
        Double cost;
        User owner;
        int freeSits;
        String notes;
        Boolean modifiable;
    }


    class Program
    {
        static void Main(string[] args)
        {
            User antonio = new User("Antonio");
            User daniele = new User("Daniele");
            Console.WriteLine("Nome utente : {0}", antonio.Name);
            Console.WriteLine("Nome utente : {0}", daniele.Name);
            Console.ReadLine();

            Location p1 = new Location(Math.PI / 2, Math.PI / 2);
            Location p2 = new Location(Math.PI / 2, Math.PI);
            Console.WriteLine(p1.distance(p2));
            Console.ReadLine();
        }
    }


};