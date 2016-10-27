using System;
using XLabs.Platform.Device;

[assembly: Xamarin.Forms.Dependency(typeof(Gis4Mobile.IOS.URILuancher))]
namespace Gis4Mobile.IOS
{
	public class URILuancher : IURILauncher
	{
		public URILuancher()
		{
		}

		#region IURILauncher implementation

		public void OpenUrl(string url)
		{
            AppleDevice.CurrentDevice.LaunchUriAsync(new Uri(url));
		}

		#endregion
	}
}