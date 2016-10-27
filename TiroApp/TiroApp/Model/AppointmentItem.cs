using Newtonsoft.Json.Linq;
using System;
using System.Text;
using Xamarin.Forms;

namespace TiroApp.Model
{
    public class AppointmentItem
    {
        private JObject jobj;
        public AppointmentItem(JObject jobj)
        {
            this.jobj = jobj;
        }

        public ImageSource Image
        {
            get
            {
                if (jobj["CustomerImage"] != null)
                {
                    return ImageSource.FromUri(new Uri((string)jobj["CustomerImage"]));
                }
                if (jobj["MuaImage"] != null)
                {
                    return ImageSource.FromUri(new Uri((string)jobj["MuaImage"]));
                }
                return ImageSource.FromResource("TiroApp.Images.empty_profile.jpg");
            }
        }
        public string Name
        {
            get
            {
                if (jobj["CustomerFirstName"] != null)
                {
                    return (string)jobj["CustomerFirstName"] + " " + (string)jobj["CustomerLastName"];
                }
                if (jobj["MuaFirstName"] != null)
                {
                    return (string)jobj["MuaFirstName"] + " " + (string)jobj["MuaLastName"];
                }
                return null;
            }
        }
        public string TotalPrice
        {
            get
            {
                double price = 0;
                var sArr = (JArray)jobj["Services"];
                foreach (var s in sArr)
                {
                    price += (int)s["Count"] * (double)s["Price"];
                }
                return UIUtils.NIARA_SIGN + price.ToString("0.00");
            }
        }
        public string ServicesName
        {
            get
            {
                var sb = new StringBuilder();
                var sArr = (JArray)jobj["Services"];
                foreach (var s in sArr)
                {
                    sb.Append((string)s["Name"]);
                    sb.Append(" | ");
                }
                sb.Length -= 3;
                return sb.ToString();
            }
        }
        public string Address
        {
            get
            {
                if (jobj["MuaAddress"] != null)
                {
                    return (string)jobj["MuaAddress"];
                }
                return null;
            }
        }
        public string Date
        {
            get
            {
                return ((DateTime)jobj["Time"]).ToString("dd.MM.yyyy hh:mm tt");
            }
        }
        public DateTime DateDT
        {
            get
            {
                return ((DateTime)jobj["Time"]);
            }
        }
        public string Id
        {
            get
            {
                return (string)jobj["Id"];
            }
        }
        public AppointmentStatus Status
        {
            get
            {
                return (AppointmentStatus)(int)jobj["Status"];
            }
        }
        public bool IsStatusNew
        {
            get
            {
                return Status == AppointmentStatus.New;
            }
        }
        public bool IsStatusNotNew
        {
            get
            {
                return Status != AppointmentStatus.New;
            }
        }
        public ImageSource ConfirmedImageSource
        {
            get
            {
                if (Status == AppointmentStatus.Approved)
                {
                    return ImageSource.FromResource("TiroApp.Images.confirmed.png");
                }
                else
                {
                    return ImageSource.FromResource("TiroApp.Images.declined.png");
                }
            }
        }
    }

    public enum AppointmentStatus
    {
        New = 0,
        Approved = 1,
        Declined = 2,
        Paid = 5
    }
}
