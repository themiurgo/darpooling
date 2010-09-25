using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using Communication;

namespace ServiceNodeCore
{
    class GMapsAPI
    {
        public static Location updateLatLng(Location location)
        {
            string[] latlng = addrToLatLng(location.Name);
            location.Latitude = double.Parse(latlng[0]);
            location.Longitude = double.Parse(latlng[1]);
            return location;
        }

        public static Location geoNameToLocation(string address)
        {
            string[] coords;
            double latitude;
            double longitude;
            Location location;

            coords = GMapsAPI.addrToLatLng(address);
            latitude = double.Parse(coords[0]);
            longitude = double.Parse(coords[1]);
            location = new Location(address, latitude, longitude);

            return location;
        }

        public static string[] addrToLatLng(string address)
        {
            address.Replace(" ", "+");
            string key = "ABQIAAAApM5Cuio981ky_h5rXnr3uhT2yXp_ZAY8_ufC3CFXhHIE1NvwkxRczD8EFDGHM7KVLNJB1qP52_6uEg";
            string url = String.Format("http://maps.google.com/maps/geo?q={0}&output=csv&sensor=false&key={1}", address, key);
            WebRequest request = WebRequest.Create(url);

            string status, accuracy, lat, lng;
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string[] content = reader.ReadToEnd().Split(',');
                    status = content[0];
                    accuracy = content[1];
                    lat = content[2];
                    lng = content[3];
                }
            }
            string[] ret = {lat, lng};
            return ret;

            // V3 Requires Google Maps Api Premium
            // url = String.Format("http://maps.google.com/maps/api/geocode/xml?address={0}&sensor=false", address;

        }

    }
}