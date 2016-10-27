using System;
using XLabs.Platform.Device;

[assembly: Xamarin.Forms.Dependency(typeof(Gis4Mobile.Droid.URILuancher))]
namespace Gis4Mobile.Droid
{
	public class URILuancher : IURILauncher
	{
		public URILuancher()
		{
		}

		#region IURILauncher implementation

		public void OpenUrl(string url)
		{
			new AndroidDevice().LaunchUriAsync(new Uri(url));
		}

		#endregion
	}
}