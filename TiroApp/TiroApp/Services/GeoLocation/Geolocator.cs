using System;
using XLabs.Platform.Services.Geolocation;

namespace Gis4Mobile.Services.GeoLocation
{
    public class Geolocator
    {
        private static Geolocator instance = null;
        public static Geolocator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Geolocator();
                }
                return instance;
            }
        }

        public event EventHandler<PositionEventArgs> PositionChanged;

        public Position LastKnowPosition
        {
            get
            {
                return lastPosition;
            }
        }

        public void GetPosition()
        {
            locator.StartListening(0, 0);
        }

        private IGeolocator locator;
        private Position lastPosition;
        private Geolocator()
        {
            locator = Xamarin.Forms.DependencyService.Get<IGeolocator>();
            //locator.DesiredAccuracy = 1000;
            locator.PositionChanged += Locator_PositionChanged;
            locator.PositionError += Locator_PositionError;
        }

        private void Locator_PositionError(object sender, PositionErrorEventArgs e)
        {
            locator.StopListening();
        }

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            lastPosition = e.Position;
            locator.StopListening();
            if (PositionChanged != null)
            {
                PositionChanged(sender, e);
            }
        }

        public static double DistanceBetweenPlaces(double lon1, double lat1, double lon2, double lat2)
        {
            double R = 6371; // km

            double sLat1 = Math.Sin(Radians(lat1));
            double sLat2 = Math.Sin(Radians(lat2));
            double cLat1 = Math.Cos(Radians(lat1));
            double cLat2 = Math.Cos(Radians(lat2));
            double cLon = Math.Cos(Radians(lon1) - Radians(lon2));

            double cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;

            double d = Math.Acos(cosD);

            double dist = R * d;

            return dist;
        }

        private static double Radians(double degs)
        {
            return degs * (Math.PI / 180.0);
        }
    }
}
