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
        public string Address { get; set; }
        public ImageSource ArtistImage { get; private set; }
        public Image Map { get; private set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string AboutMe { get; set; }
        public List<Service> Services { get; private set; } = new List<Service>();
        public List<Review> Reviews { get; private set; } = new List<Review>();
        public List<string> Images { get; set; }

        private void Init(JObject jObj)
        {
            var test = jObj.ToString();
            Id = _isFromGetMua ? (string)jObj["Id"] : (string)jObj["MuaId"];
            FirstName = _isFromGetMua ? (string)jObj["FirstName"] : (string)jObj["MuaFirstName"];
            LastName = _isFromGetMua ? (string)jObj["LastName"] : (string)jObj["MuaLastName"];
            Address = _isFromGetMua ? (string)jObj["Address"] : (string)jObj["MuaAddress"];
            BusinessName = _isFromGetMua ? (string)jObj["BusinessName"] : null;            
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
            Lat = _isFromGetMua ? (double)jObj["LocationLat"] : (double)jObj["MuaLocationLat"];
            Lon = _isFromGetMua ? (double)jObj["LocationLon"] : (double)jObj["MuaLocationLon"];
            var url = $"https://maps.googleapis.com/maps/api/staticmap?center={Lat},{Lon}&zoom=12&size={App.ScreenWidth}x300&key={Props.GOOGLE_KEY}";
            Map = new Image();
            Map.Source = ImageSource.FromUri(new Uri(url));
            Map.Aspect = Aspect.AspectFill;
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
            }
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
}
