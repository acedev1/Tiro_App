using Services;
using Android.Content;
using Xamarin.Forms;
[assembly: Dependency(typeof(ShowGMaps))]
namespace Services
{
    public class ShowGMaps : Gis4Mobile.IShowGmaps
    {
        public void ShowGmaps(double lat, double lon)
        {
            string locLat = lat.ToString(new System.Globalization.CultureInfo("en-US"));
            string locLon = lon.ToString(new System.Globalization.CultureInfo("en-US"));
            var geoUri = Android.Net.Uri.Parse("google.navigation:q=" + locLat+ ","+locLon );
            var mapIntent = new Intent(Intent.ActionView, geoUri);
            mapIntent.SetClassName("com.google.android.apps.maps", "com.google.android.maps.MapsActivity");
            Forms.Context.StartActivity(mapIntent);
        }
    }
}