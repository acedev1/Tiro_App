using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Pages;

namespace TiroApp.Model
{
    public class Order
    {
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

        private void Init(JObject jObj)
        {
            if (_isForMua)
            {
                Customer = new Customer(jObj);
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
}
