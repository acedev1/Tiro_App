using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Model
{
    public class PlaceSearchHelper
    {
        private XLabs.Forms.Controls.AutoCompleteView searchEntry;
        private Dictionary<string, string> placeList = new Dictionary<string, string>();

        public event EventHandler<GeoCoordinate> OnSelected;

        public PlaceSearchHelper(XLabs.Forms.Controls.AutoCompleteView entry)
        {
            this.searchEntry = entry;
            this.searchEntry.ShowSearchButton = false;
            this.searchEntry.TextChanged += SearchEntry_TextChanged;
            this.searchEntry.SelectedItemChanged += SearchEntry_SelectedItemChanged;
        }

        public GeoCoordinate Location { get; private set; }

        private void SearchEntry_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            var placeId = placeList[(string)e.SelectedItem];
            string url = "https://maps.googleapis.com/maps/api/place/details/xml?placeid={0}&key=AIzaSyCEIQNsuPwizWAWUuC68d1RJNONXXFAQlI";
            url = string.Format(url, placeId);
            System.Net.HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(url);
            request.BeginGetResponse((iar) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.searchEntry.ShowHideListbox(false);
                    this.searchEntry.ForceLayout();
                });
                System.Net.WebResponse response = ((System.Net.HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                var xmlDoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());
                var xmlRoot = (System.Xml.Linq.XElement)xmlDoc.FirstNode;
                if (xmlRoot.Element("status").Value.Equals("OK"))
                {
                    var loc = xmlRoot.Element("result").Element("geometry").Element("location");
                    Location = new GeoCoordinate(double.Parse(loc.Element("lat").Value, Utils.EnCulture), double.Parse(loc.Element("lng").Value, Utils.EnCulture));
                    OnSelected?.Invoke(this, Location);
                }
            }, request);
            this.searchEntry.ForceLayout();
        }

        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (searchEntry.Text.Length < 2)
            {
                return;
            }

            string url = "https://maps.googleapis.com/maps/api/place/autocomplete/xml?input={0}&types=address&components=country:ng&key=AIzaSyCEIQNsuPwizWAWUuC68d1RJNONXXFAQlI";
            url = string.Format(url, searchEntry.Text);
            System.Net.HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(url);
            request.BeginGetResponse((iar) =>
            {
                System.Net.WebResponse response = ((System.Net.HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                var xmlDoc = System.Xml.Linq.XDocument.Load(response.GetResponseStream());
                var xmlRoot = (System.Xml.Linq.XElement)xmlDoc.FirstNode;
                if (xmlRoot.Element("status").Value.Equals("OK"))
                {
                    lock (placeList)
                    {
                        placeList.Clear();
                        foreach (var item in xmlRoot.Elements("prediction"))
                        {
                            var text = item.Element("description").Value;
                            var placeId = item.Element("place_id").Value;
                            if (!placeList.ContainsKey(text))
                            {
                                placeList.Add(text, placeId);
                            }

                        }
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ShowSearchResult();
                        });
                    }
                }
            }, request);
        }

        private void ShowSearchResult()
        {
            this.searchEntry.Suggestions = placeList.Keys;
            this.searchEntry.RaiseTextValueChanged();
            this.searchEntry.ShowHideListbox(true);
        }
    }
}
