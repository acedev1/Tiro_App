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


        private bool _useExternalGPS = false;
        public bool UseExternalGPS
        { 
            get
            { 
                return _useExternalGPS;
            } 
            set
            { 
                try
                {
                    if (_useExternalGPS != value)
                    {
                        _useExternalGPS = value;
                        locator.UseExternalGPS = _useExternalGPS;
                        lastPosition = null;
                        locator.StopListening();
                        locator.StartListening(1000, 0);
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        private IGeolocator locator;
        private Position lastPosition;
        private Geolocator()
        {
            locator = Xamarin.Forms.DependencyService.Get<IGeolocator>();
            locator.DesiredAccuracy = 10;
            locator.PositionChanged += Locator_PositionChanged;            
        }

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            lastPosition = e.Position;
            if (PositionChanged != null)
            {               
                PositionChanged(sender, e);
            }
        }
    }
}
