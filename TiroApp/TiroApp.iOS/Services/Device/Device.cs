using System;
using XLabs.Platform.Device;

[assembly: Xamarin.Forms.Dependency (typeof (Gis4Mobile.iOS.Device))]
namespace Gis4Mobile.iOS
{
    public class Device : IDevice
    {
        private IDevice device;

        public Device()
        {
            device = AppleDevice.CurrentDevice;
        }

        #region IDevice implementation

        public System.Threading.Tasks.Task<bool> LaunchUriAsync(Uri uri)
        {
            return device.LaunchUriAsync(uri);
        }

        public string Id
        {
            get
            {
                return device.Id;
            }
        }

        public XLabs.Platform.Services.Media.IMediaPicker MediaPicker
        {
            get
            {
                return device.MediaPicker;
            }
        }

        public XLabs.Platform.Services.INetwork Network
        {
            get
            {
                return device.Network;
            }
        }

        public string Name
        {
            get
            {
                return device.Name;
            }
        }

        public string FirmwareVersion
        {
            get
            {
                return device.FirmwareVersion;
            }
        }

        public string HardwareVersion
        {
            get
            {
                return device.HardwareVersion;
            }
        }

        public string Manufacturer
        {
            get
            {
                return device.Manufacturer;
            }
        }

        public long TotalMemory
        {
            get
            {
                return device.TotalMemory;
            }
        }

        public string LanguageCode
        {
            get
            {
                return device.LanguageCode;
            }
        }

        public double TimeZoneOffset
        {
            get
            {
                return device.TimeZoneOffset;
            }
        }

        public string TimeZone
        {
            get
            {
                return device.TimeZone;
            }
        }

        public IDisplay Display
        {
            get
            {
                return device.Display;
            }
        }

        #endregion
    }
}

