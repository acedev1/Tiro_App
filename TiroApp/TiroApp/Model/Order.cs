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
    public class Order
    {
        public const int DISCOUNT_TYPE_PERCENT = 1;
        public const int DISCOUNT_TYPE_FIXED = 2;

        private bool _isForMua;
        public Order(JObject jObj, bool isForMua = false)
        {
            _isForMua = isForMua;
            Init(jObj);
        }
        public Order(MuaArtist mua)
        {
            Mua = mua;
        }
        public bool IsUpdate { get; set; } = false;
        public string MuaAddress { get; private set; }
        public Customer Customer { get; private set; }
        public MuaArtist Mua { get; private set; }
        public List<OrderItem> Basket { get; set; } = new List<OrderItem>();
        public double TotalPrice {
            get {
                return Basket.Sum(i => i.TotalPrice);
            }
        }
        public DateTime DateTime { get; set; }
        public string Note { get; set; } = string.Empty;
        public PaymentType PaymentType { get; set; }
        public AppointmentStatus Status { get; set; }
        public string AppointmentId { get; set; }
        public bool IsFree { get; set; }
        public string DiscountId { get; set; }
        public Location Location { get; set; }
        public Discount Discount { get; set; }

        private void Init(JObject jObj)
        {
            if (_isForMua)
            {
                Customer = new Customer(jObj);
                if (jObj["CustomerId"] != null)
                {
                    Customer.Id = (string)jObj["CustomerId"];
                }
                MuaAddress = (string)jObj["MuaAddress"];
            }
            else
            {
                Mua = new MuaArtist(jObj);
            }
            DateTime = (DateTime)jObj["Time"];
            Note = (string)jObj["Message"];
            PaymentType = (PaymentType)(int)jObj["PaymentType"];
            Status = (AppointmentStatus)(int)jObj["Status"];
            AppointmentId = (string)jObj["Id"];
            foreach (JObject service in (JArray)jObj["Services"])
            {
                Basket.Add(new OrderItem(new Service(service), (int)service["Count"]));
            }
            var discount = jObj["Discount"];
            if (discount != null)
            {
                Discount = new Discount(discount as JObject);
            }
            var location = jObj["Location"];
            if (location != null)
            {
                Location = new Location((location as JObject), true, true);
            }
            else
            {
                Location = new Location {
                    Address = (string)jObj["MuaAddress"],
                    Lat = (double)jObj["MuaLocationLat"],
                    Lon = (double)jObj["MuaLocationLon"]
                };
                Location.MapInit();
            }
        }
    }

    public class OrderItem
    {
        public OrderItem(Service service, int count)
        {
            Service = service;
            Count = count;
        }
        public Service Service { get; set; }
        public int Count { get; set; }
        public double TotalPrice
        {
            get
            {
                return Service.Price * Count;
            }
        }
    }

    public class Discount
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public DateTime ExpireDate { get; set; }
        public ImageSource Image { get; set; }
        public double Value { get; set; }

        public Discount(JObject jObj)
        {
            Init(jObj);
        }

        private void Init(JObject jObj)
        {
            var test = jObj.ToString();
            Id = (string)jObj["Id"];
            Name = (string)jObj["Name"];
            Type = (int)jObj["Type"];
            ExpireDate = (DateTime)jObj["ExpireDate"];
            var imageUri = jObj["Image"];
            if (imageUri != null)
            {
                UriImageSource imgs = (UriImageSource)ImageSource.FromUri(new Uri((string)imageUri));
                imgs.CachingEnabled = false;
                Image = imgs;
            }
            Value = (double)jObj["Value"];
        }
    }
}
