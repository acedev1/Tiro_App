using System.Xml.Linq;
using Xamarin.Forms;

namespace TiroApp
{
    public static class Props
    {
        public static readonly Color ButtonColor = Color.FromHex("CF7090");
        public static readonly Color ButtonInfoPageColor = Color.FromHex("#352E50");
        public static readonly Color BlackoutColor = Color.FromRgba(53, 46, 79, 170);
        public static readonly Color GrayColor = Color.FromRgb(239, 241, 243);
        public const int ButtonBorderRadius = 0;
        public static readonly XNamespace XmlNamespace = "http://schemas.datacontract.org/2004/07/TiroWeb.Models";
        public const int RatingMax = 5;
        public static string GOOGLE_KEY = "AIzaSyAxy5GmU9gEJswEDAXauw6DHPr8eu_J6qY";
        public static System.Xml.Linq.XNamespace TIRO_NS = System.Xml.Linq.XNamespace.Get("http://schemas.datacontract.org/2004/07/TiroWeb.Models");
    }
}
