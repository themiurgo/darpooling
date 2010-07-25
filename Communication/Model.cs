using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization;

using System.Reflection;


namespace Communication
{
    /// <summary>
    /// This assembly contains Location, User, Trip
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

        /// <summary>
        /// Return distance in km to another Location.
        /// </summary>
        /// <param name="other">the other location</param>
        /// <returns></returns>
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

    [DataContract]
    public class User
    {
        public enum Sex { f, m }
        
        [DataMember]
        public int UserID { get; private set; }
        [DataMember]
        public String UserName { get; private set; }
        
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

        public User(String name)
        {
            this.Name = name;
        }

        public User(String name, String username)
        {
            this.Name = name;
            this.UserName = username;
        }

    }

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

            /*Console.WriteLine("These are the Trip info");
            Console.WriteLine("Id: ", ID);
            Console.WriteLine(": ",Owner);
            DepartureName);
            DepartureDateTime);
            ArrivalName);
            ArrivalDateTime);
            Smoke);
            Music);
            Cost);
            FreeSits);
            Notes);
            Modifiable = Convert.ToBoolean(t.Attribute("Modifiable").Value)
             * */
        }
    }
    
    public class TripSpecifier
    {
        private Location source;
        private Location destination;
    }

/*    public class Movie
    {
        [XmlElement("MovieName")]
        public string Title
        { get; set; }

        [XmlElement("MovieRating")]
        public float Rating
        { get; set; }

        [XmlElement("MovieReleaseDate")]
        public DateTime ReleaseDate
        { get; set; }
    }

    public class Customer
    {
        public int ID { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string DOB { get; set; }
        public string Location { get; set; }
    }
*/

} //End Namespace
