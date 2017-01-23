using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Pages;
using Xamarin.Forms;

namespace TiroApp.Model
{
    public abstract class Client
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return FirstName + " " + LastName; } }
        public string PhoneNumber { get; set; }
        public ImageSource CustomerImage { get; protected set; }
        public string DefaultPicture { get; set; }
    }

    public class MuaArtist : Client
    {
        private bool _isFromGetMua;
        public MuaArtist(JObject jObj, bool isFromGetMua = false)
        {
            _isFromGetMua = isFromGetMua;
            Init(jObj);
        }

        public string BusinessName { get; set; }
        public ImageSource ArtistImage { get; private set; }
        public Location Location { get; set; }
        public string AboutMe { get; set; }
        public List<Service> Services { get; private set; } = new List<Service>();
        public List<Review> Reviews { get; private set; } = new List<Review>();
        public List<string> Images { get; set; }
        public bool IsTravelAvailable { get; set; }
        public double TravelCharge { get; set; }

        private void Init(JObject jObj)
        {
            Id = _isFromGetMua ? (string)jObj["Id"] : (string)jObj["MuaId"];
            FirstName = _isFromGetMua ? (string)jObj["FirstName"] : (string)jObj["MuaFirstName"];
            LastName = _isFromGetMua ? (string)jObj["LastName"] : (string)jObj["MuaLastName"];
            BusinessName = _isFromGetMua ? (string)jObj["BusinessName"] : (string)jObj["MuaBusinessName"];
            var imageUri = _isFromGetMua ? jObj["ProfilePicture"] : jObj["MuaImage"];
            if (imageUri != null)
            {
                UriImageSource imgs = (UriImageSource)ImageSource.FromUri(new Uri((string)imageUri));
                imgs.CachingEnabled = false;
                ArtistImage = imgs;
            }
            else
            {
                ArtistImage = ImageSource.FromResource("TiroApp.Images.empty_profile.jpg");
            }
            if (jObj["Pictures"] != null)
            {
                Images = ((JArray)jObj["Pictures"]).Select(jo => (string)jo).ToList();
            }
            if (_isFromGetMua)
            {
                AboutMe = (string)jObj["AboutMe"];
                if (jObj["Services"] != null)
                {
                    foreach (JObject service in (JArray)jObj["Services"])
                    {
                        Services.Add(new Service(service, true));
                    }
                }
                if (jObj["Reviews"] != null)
                {
                    foreach (JObject review in (JArray)jObj["Reviews"])
                    {
                        Reviews.Add(new Review(review));
                    }
                }
                IsTravelAvailable = (bool)jObj["IsTravelAvailable"];
                TravelCharge = (double)jObj["TravelCharge"];
            }
            else
            {
                TravelCharge = (double)jObj["MuaTravelCharge"];
            }
            Location = new Location(jObj, _isFromGetMua);
            DefaultPicture = (string)jObj["DefaultPicture"];
        }
    }

    public class Customer : Client
    {
        private bool _isInfo;
        public Customer(JObject jObj, bool isInfo = false)
        {
            _isInfo = isInfo;
            Init(jObj);
        }

        private void Init(JObject jObj)
        {
            Id = (string)jObj["Id"];
            FirstName = _isInfo ? (string)jObj["FirstName"] : (string)jObj["CustomerFirstName"];
            LastName = _isInfo ? (string)jObj["LastName"] : (string)jObj["CustomerLastName"];
            var imageUri = _isInfo ? jObj["ProfileImage"] : jObj["CustomerImage"];
            if (imageUri != null)
            {
                UriImageSource imgs = (UriImageSource)ImageSource.FromUri(new Uri((string)imageUri));
                imgs.CachingEnabled = false;
                CustomerImage = imgs;
            }
            else
            {
                CustomerImage = ImageSource.FromResource("TiroApp.Images.empty_profile.jpg");
            }
            PhoneNumber = (string)jObj["Phone"];
        }
    }

    public class Location
    {
        public string Address { get; set; }
        public Image Map { get; private set; }
        public double Lat { get; set; }
        public double Lon { get; set; }

        private bool _isFromGetMua;
        private bool _isAppointmentInfo;
        public Location() { }
        public Location(JObject jObj, bool isFromGetMua, bool isAppointmentInfo = false)
        {
            this._isFromGetMua = isFromGetMua;
            this._isAppointmentInfo = isAppointmentInfo;
            Init(jObj);
        }

        private void Init(JObject jObj)
        {
            Address = _isFromGetMua ? (string)jObj["Address"] : (string)jObj["MuaAddress"];
            Lat = _isFromGetMua ? (_isAppointmentInfo ? (double)jObj["Lat"] : (double)jObj["LocationLat"]) 
                : (double)jObj["MuaLocationLat"];
            Lon = _isFromGetMua ? (_isAppointmentInfo ? (double)jObj["Lon"] : (double)jObj["LocationLon"]) 
                : (double)jObj["MuaLocationLon"];
            MapInit();
        }
        public void MapInit()
        {
            var url = $"https://maps.googleapis.com/maps/api/staticmap?center={Lat},{Lon}&zoom=12&size={App.ScreenWidth}x300&key={Props.GOOGLE_KEY}";
            Map = new Image();
            Map.Source = ImageSource.FromUri(new Uri(url));
            Map.Aspect = Aspect.AspectFill;
        }
    }
}
