using System;
using System.Runtime.Serialization;
using System.Reflection;

namespace Communication
{

    /// <summary>
    /// Location is the class that models the geocoord of a city, in terms of
    /// latitude and longitude.
    /// It also exposes the method to compute the distance between two location.
    /// </summary>
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

        #region Properties
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
        #endregion

        /// <summary>
        /// Return distance in km to another Location.
        /// </summary>
        /// <param name="other">the other location</param>
        /// <returns>the distance</returns>
        public double distance(Location other)
        {
            /** Convert angle coordinates into radians. */
            double a1 = (this.latitude* Math.PI ) / 180.0;
            double b1 = (this.longitude* Math.PI ) / 180.0;
            double a2 = (other.latitude* Math.PI ) / 180.0;
            double b2 = (other.longitude* Math.PI ) / 180.0;

            if ((a1 == 0 && b1 == 0) || (a2 == 0 && b2 == 0))
                throw new System.Exception("One of the member has no coordinates");

            int earthRadius = 6371;

            double ret = Math.Acos(Math.Cos(a1) * Math.Cos(b1) * Math.Cos(a2) * Math.Cos(b2) + Math.Cos(a1) * Math.Sin(b1) * Math.Cos(a2) * Math.Sin(b2) + Math.Sin(a1) * Math.Sin(a2)) * earthRadius;

            // Error due to decimal precision
            if (ret == Double.NaN)
                ret = 0.0;

            return ret;
        }
    }


    public class LocationRange : Location
    {
        private int range;

        public LocationRange(double latitude, double longitude, int range) :
            base(latitude, longitude)
        {
            this.range = range;
        }

        public bool contains(Location other)
        {
            return this.distance(other) < range;
        }
    }


    /// <summary>
    /// Class User represents the generic user of the DarPooling system.
    /// It containts the fields that model the personal information and the properties
    /// to access them.
    /// Instances of this class will be stored in the service database.
    /// </summary>
    [DataContract]
    public class User
    {
        private string pw_hash;

        public enum Sex { f, m }
        
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public String UserName { get; set; }
        [DataMember] // Should be datamember?
        public String Password
        {
            set { pw_hash = Tools.HashString(value); } // FIXME
        }
        [DataMember]
        public String PasswordHash
        {
            get { return pw_hash; }
        }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public Sex UserSex { get; set; }
        [DataMember]
        public DateTime BirthDate { get; set; }
        [DataMember]
        public String Email { get; set; }
        [DataMember]
        public Boolean Smoker { get; set; }
        [DataMember]
        public DateTime SignupDate { get; set; }
        [DataMember]
        public String Whereabouts { get; set; }

        public User(){ }

        public User(String name) : this(name,null)  {}

        public User(String name, String username)
        {
            this.Name = name;
            this.UserName = username;
        }

    }


    /// <summary>
    /// Trip models the fundamental entity of the DarPooling service, that is
    /// trip that a generic user chooses to share with other users. Trip are created
    /// and stored in the service databases, and they could be searched and modified.
    /// </summary>
    [DataContract]
    public class Trip
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public String Owner { get; set; }

        [DataMember]
        public String DepartureName { get; set; }
        [DataMember]
        public DateTime DepartureDateTime { get; set; }

        [DataMember]
        public String ArrivalName { get; set; }
        [DataMember]
        public DateTime ArrivalDateTime { get; set; }
        
        [DataMember]
        public Boolean Smoke { get; set; }
        [DataMember]
        public Boolean Music { get; set; }
        
        [DataMember]
        public Double Cost { get; set; }
        [DataMember]
        public int FreeSits { get; set; }
        [DataMember]
        public String Notes { get; set; }
        
        [DataMember]
        public Boolean Modifiable { get; set; }

        public Trip() { }

        // Debug
        public void PrintFullInfo()
        {
            Console.WriteLine("** Full Trip Info **");
            Type t = typeof(Trip);

            PropertyInfo[] props = t.GetProperties();

            foreach ( PropertyInfo p in props)
            {
                MethodInfo get = p.GetGetMethod();
                Console.WriteLine("{0} : {1}",p.Name,p.GetValue(this,null));
            }

        }
    }


    /// <summary>
    /// Represent the set of parameters used to perform the search of a Trip.
    /// The structure is nearly identical to Trip class, but actually QueryBuilder
    /// represent a SET of Trips, and could be later extended to support different
    /// search criteria.
    /// </summary>
    [DataContract]
    public class QueryBuilder
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public String Owner { get; set; }

        [DataMember]
        public String DepartureName { get; set; }
        [DataMember]
        public DateTime DepartureDateTime { get; set; }

        [DataMember]
        public String ArrivalName { get; set; }
        [DataMember]
        public DateTime ArrivalDateTime { get; set; }

        [DataMember]
        public Boolean Smoke { get; set; }
        [DataMember]
        public Boolean Music { get; set; }

        [DataMember]
        public Double Cost { get; set; }
        [DataMember]
        public int FreeSits { get; set; }
        [DataMember]
        public String Notes { get; set; }

        [DataMember]
        public Boolean Modifiable { get; set; }

        public QueryBuilder() { }

    }

    
    public class TripSpecifier
    {
        private Location source;
        private Location destination;
    }



} //End Namespace
