using System;
using Android.Locations;

namespace GatewayXNative.Models
{
    public class Position
    {
        private Location location;

        public double? lat { get; set; }
        public double? lon { get; set; }
        public double? accuracyInMeters { get; set; }
        public double? altitude { get; set; }
        public double? altitudeAccuracyInMeters { get; set; }
        public double? speed { get; set; }
        public double? heading { get; set; }
        public DateTime? positionAt { get; set; }

        public Position()
        {
        }

        public Position(Location location)
        {
            this.location = location;

            this.lat = location.Latitude;
            this.lon = location.Longitude;
            this.accuracyInMeters = location.Accuracy;
            this.altitude = location.Altitude;
            this.speed = location.Speed;
            this.heading = location.Bearing;
            this.positionAt = GetDateTime(location.Time).DateTime;

            System.Diagnostics.Debug.WriteLine("GPS - " + location.ToString());
        }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        internal static DateTimeOffset GetDateTime(long time)
        {
            try
            {
                return new DateTimeOffset(Epoch.AddMilliseconds(time));
            }
            catch (Exception)
            {
                return new DateTimeOffset(Epoch);
            }
        }
    }

    public class PositionMinified
    {
        //lat
        public double? t { get; set; }
        //lon
        public double? n { get; set; }
        //accuracyInMeters
        public double? m { get; set; }
        //altitude
        public double? l { get; set; }
        //altitudeAccuracyInMeters
        public double? c { get; set; }
        //speed
        public double? s { get; set; }
        //heading
        public double? h { get; set; }
        //positionAt
        public DateTime? a { get; set; }

        public PositionMinified(Position pos)
        {
            t = pos.lat;
            n = pos.lon;
            m = pos.accuracyInMeters;
            l = pos.altitude;
            c = pos.altitudeAccuracyInMeters;
            s = pos.speed;
            h = pos.heading;
            a = pos.positionAt;

        }
    }
}
