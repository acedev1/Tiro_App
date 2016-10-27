using System;
using Xamarin.Forms;
using Gis4Mobile.iOS;
using CoreLocation;
using UIKit;
using Foundation;
using MapKit;

[assembly: Dependency(typeof(ShowGMaps))]
namespace Gis4Mobile.iOS
{
	public class ShowGMaps : Gis4Mobile.IShowGmaps
	{
		public void ShowGmaps(double lat, double lon)
		{
				CLLocationCoordinate2D coordinate_end = new CLLocationCoordinate2D(lat, lon);
				MKPlacemark placeMark_end = new MKPlacemark (coordinate_end, new MKPlacemarkAddress ());
				MKMapItem mapItem_end = new MKMapItem (placeMark_end);

				MKMapItem mapItem_start = MKMapItem.MapItemForCurrentLocation();

				MKLaunchOptions options = new MKLaunchOptions();
                options.DirectionsMode = MKDirectionsMode.Driving;

                MKMapItem.OpenMaps(new MKMapItem[]{mapItem_start, mapItem_end }, options);
		}
	}
}

