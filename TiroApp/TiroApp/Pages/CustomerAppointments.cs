using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TiroApp.Model;
using TiroApp.Pages.Mua;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages
{
    public class CustomerAppointments : BasePage
    {
        private StackLayout contentLayout;
        private ListView listView;
        private int currentTabIndex = 0;
        private JArray appData;
        private ActivityIndicator spinner;

        public CustomerAppointments()
        {
            Utils.SetupPage(this);
            this.BackgroundColor = Color.White;
            BuildLayout();
            AddSideMenu();
            spinner = UIUtils.ShowSpinner(this);
            RefreshData();
        }

        public void RefreshData()
        {
            DataGate.GetAppointmentsByCustomer(GlobalStorage.Settings.CustomerId, OnDataLoad);
        }

        private void BuildLayout()
        {
            contentLayout = new StackLayout();
            contentLayout.Spacing = 0;
            this.mainLayout.Children.Add(contentLayout, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));

            var headerLabel = new CustomLabel();
            headerLabel.Text = "Appointments";
            headerLabel.TextColor = Color.Black;
            headerLabel.BackgroundColor = Color.White;
            headerLabel.HorizontalTextAlignment = TextAlignment.Center;
            headerLabel.VerticalTextAlignment = TextAlignment.Center;
            headerLabel.HeightRequest = 50;
            headerLabel.FontSize = 16;
            headerLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            headerLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            contentLayout.Children.Add(headerLabel);

            var separator = UIUtils.MakeSeparator(true);
            separator.VerticalOptions = LayoutOptions.Start;
            contentLayout.Children.Add(separator);

            var tabView = new TabView(new List<string> {"Upcoming", "Past" }, Props.ButtonColor, true)
            {
                SelectedIndex = 0,
                BackgroundColor = Color.FromHex("F8F8F8"),
                HeightRequest = 50
            };
            tabView.OnIndexChange += OnTabChange;
            contentLayout.Children.Add(tabView);

            listView = new ListView();
            listView.ItemTemplate = new DataTemplate(() => new AppointmentViewCell());
            listView.SeparatorVisibility = SeparatorVisibility.None;
            listView.ItemTapped += ItemSelected;
            contentLayout.Children.Add(listView);
        }

        private void OnDataLoad(ResponseDataJson r)
        {
            if (r.Code == ResponseCode.OK)
            {
                appData = JArray.Parse(r.Result);
                Device.BeginInvokeOnMainThread(() =>
                {
                    //UpdateConfirmCount();
                    OnTabChange(listView, currentTabIndex);
                });
            }
            else
            {
                UIUtils.ShowMessage("Server does not response", this);
            }
            UIUtils.HideSpinner(this, spinner);
        }

        private void OnTabChange(object sender, int index)
        {
            currentTabIndex = index;
            var test = appData.ToString();
            if (currentTabIndex == 0)
            {
                var dataFiltered = appData.Where(o => ((DateTime)o["Time"] >= DateTime.Now));
                var dataConverted = dataFiltered.Select(o => new AppointmentItem((JObject)o));
                listView.RowHeight = Device.OnPlatform(115, 120, 115);
                listView.ItemsSource = null;
                listView.ItemsSource = dataConverted;
            }
            else if (currentTabIndex == 1)
            {
                var dataFiltered = appData.Where(o => ((DateTime)o["Time"] < DateTime.Now));
                var dataConverted = dataFiltered.Select(o => new AppointmentItem((JObject)o));
                listView.RowHeight = Device.OnPlatform(115, 120, 115);
                listView.ItemsSource = null;
                listView.ItemsSource = dataConverted;
            }
        }

        private void ItemSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
            {
                return;
            }
            var jObj = (JObject)appData.SingleOrDefault(o => (string)o["Id"] == (e.Item as AppointmentItem).Id);
            if (jObj == null)
            {
                return;
            }
            var order = new Order(jObj);
            Device.BeginInvokeOnMainThread(() => {
                Navigation.PushAsync(new OrderSummary(order, true));
            });

            listView.SelectedItem = null;
        }
    }
}
