using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Motion_Planner
{
    class GPS
    {
        public double long_coord; //x 
        public double lat_coord; //y
        public double heading;
        public GPS(double long_coord, double lat_coord, double heading)
        {
            this.long_coord = long_coord;
            this.lat_coord = lat_coord;
            this.heading = heading;
        }

        public List<double> getlocal(GPS origin)
        {
            //  AddToLogBox("lat1 : " + lat1 + ", long1: " + lng1 + ", lat2: " + lat2 + ", long2: " + lng2);

            double lng1 = this.long_coord;
            double lng2 = origin.long_coord;
            double lat1 = this.lat_coord;
            double lat2 = origin.lat_coord;

            double earthRadius = 3958.75;
            double dLat = (Math.PI / 180) * (lat2 - lat1);
            double dLng = (Math.PI / 180) * (lng2 - lng1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos((Math.PI / 180) * (lat1)) * Math.Cos((Math.PI / 180) * (lat2)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double dist = earthRadius * c;
            //AddToLogBox("dist in miles: " + dist);

            double meterConversion = 1609;
            dist = dist * meterConversion;

            //    AddToLogBox("dist in meteres: " + dist);
            List<double> local = new List<double>();
            return local;
        } 
    }
}
