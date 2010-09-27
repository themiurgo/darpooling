using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using Communication;

namespace ServiceNodeCore
{
    /// <summary>
    /// Utility class used to retrieve coordinates (latitude,longitude) starting
    /// from a geographical name.
    /// </summary>
    class GMapsAPI
    {
        /// <summary>
        /// Translate an address to coordinates (latitude and longitude)
        /// </summary>
        /// <param name="address">The address to be translated</param>
        /// <returns></returns>
        public static string[] addressToLatLng(string address)
        {
            string key, gMapUrl, status, accuracy, latitude, longitude;

            address.Replace(" ", "+");
            key = "ABQIAAAApM5Cuio981ky_h5rXnr3uhT2yXp_ZAY8_ufC3CFXhHIE1NvwkxRczD8EFDGHM7KVLNJB1qP52_6uEg";
            gMapUrl = String.Format("http://maps.google.com/maps/geo?q={0}&output=csv&sensor=false&key={1}", address, key);
            WebRequest gMapRequest = WebRequest.Create(gMapUrl);

            using (WebResponse response = gMapRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string[] content = reader.ReadToEnd().Split(',');
                    status = content[0];
                    accuracy = content[1];
                    latitude = content[2];
                    longitude = content[3];
                }
            }
            string[] coordinates = { latitude, longitude };
            return coordinates;

            // V3 Requires Google Maps Api Premium
            // url = String.Format("http://maps.google.com/maps/api/geocode/xml?address={0}&sensor=false", address;

        }


        /// <summary>
        /// Translate an address to a Location object, containing the latitude and
        /// the longitude of the address.
        /// </summary>
        /// <param name="address">The address to be translated</param>
        /// <returns>A Location instance</returns>
        public static Location addressToLocation(string address)
        {
            string[] coords;
            double latitude;
            double longitude;
            Location location;

            coords = GMapsAPI.addressToLatLng(address);
            latitude = double.Parse(coords[0]);
            longitude = double.Parse(coords[1]);
            location = new Location(address, latitude, longitude);

            return location;
        }



        /*
        public static Location updateLatLng(Location location)
        {
            string[] latlng = addrToLatLng(location.Name);
            location.Latitude = double.Parse(latlng[0]);
            location.Longitude = double.Parse(latlng[1]);
            return location;
        }*/

    }
}