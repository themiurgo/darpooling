using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;

namespace Communication
{



    public class Location
    {
        private string name;
        private double latitude;
        private double longitude;

        public Location(string locName)
        {
            Name = locName;
        }

        public Location(string locName, double latitude, double longitude)
        {
            Name = locName;
            Latitude = latitude;
            Longitude = longitude;
        }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        // Properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        // Methods
        public double distance(Location other)
        {
            double a1 = this.latitude;
            double b1 = this.longitude;
            double a2 = other.latitude;
            double b2 = other.longitude;

            if ((a1 == 0 && b1 == 0) || (a2 == 0 && b2 == 0))
                throw new System.Exception("One of the member has no coordinates");

            int earthRadius = 6378;
            double ret = Math.Acos(Math.Cos(a1) * Math.Cos(b1) * Math.Cos(a2) * Math.Cos(b2) + Math.Cos(a1) * Math.Sin(b1) * Math.Cos(a2) * Math.Sin(b2) + Math.Sin(a1) * Math.Sin(a2)) * earthRadius;
            //Console.WriteLine("{0}", ret);
            return ret;
        }
    }

    [DataContract]
    public class User
    {
        enum Sex { f, m };
        int userId;
        String username;

        [DataMember]
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

} //End Namespace
