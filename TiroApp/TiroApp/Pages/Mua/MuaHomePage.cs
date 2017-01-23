using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public class MuaHomePage : MuaBasePage
    {
        private StackLayout contentLayout;
        private ListView listView;
        private int currentTabIndex = 0;
        private JArray appData;
        private ActivityIndicator spinner;
        private Label confirmCountLabel;

        public MuaHomePage()
        {
            Utils.SetupPage(this);
            this.BackgroundColor = Color.White;
            BuildLayout();
            AddSideMenu();
            spinner = UIUtils.ShowSpinner(this);
            DataGate.GetAppointmentsByMua(GlobalStorage.Settings.MuaId, OnDataLoad);
        }

        public void RefreshData()
        {
            DataGate.GetAppointmentsByMua(GlobalStorage.Settings.MuaId, OnDataLoad);
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

            var tabView = new TabView(new List<string> { "To Confirm", "Upcoming", "Past" }, Props.ButtonColor, true)
            {
                SelectedIndex = 0,
                BackgroundColor = Color.FromHex("F8F8F8"),
                HeightRequest = 50
            };
            tabView.OnIndexChange += OnTabChange;
            contentLayout.Children.Add(tabView);

            listView = new ListView(ListViewCachingStrategy.RecycleElement);
            listView.ItemTemplate = new DataTemplate(() => new AppointmentViewCell(OnDeclineClick, OnConfirmClick));
            listView.SeparatorVisibility = SeparatorVisibility.None;
            listView.ItemTapped += ItemSelected;
            contentLayout.Children.Add(listView);

            AddConfirmLabel();
        }

        private void AddConfirmLabel()
        {
            var img = new Image();
            img.Source = ImageSource.FromResource("TiroApp.Images.count_bg.png");
            confirmCountLabel = new CustomLabel()
            {
                TextColor = Color.White,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                FontSize = Device.OnPlatform(14, 14, 14),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 2),
                Text = "0"
            };
            var rl = new RelativeLayout();
            rl.WidthRequest = 20;
            rl.HeightRequest = 20;
            rl.Children.Add(img, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
            rl.Children.Add(confirmCountLabel, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));

            var labelWidth = 80;
            var left = (App.ScreenWidth / 3 - labelWidth) / 2 + labelWidth + 10;
            var top = 50;
            mainLayout.Children.Add(rl, Constraint.Constant(left), Constraint.Constant(top),
                Constraint.Constant(rl.WidthRequest), Constraint.Constant(rl.HeightRequest));
        }

        private void OnDataLoad(ResponseDataJson r)
        {
            if (r.Code == ResponseCode.OK)
            {
                appData = JArray.Parse(r.Result);
                Device.BeginInvokeOnMainThread(() =>
                {
                    UpdateConfirmCount();
                    OnTabChange(listView, currentTabIndex);
                });
            }
            else
            {
                UIUtils.ShowMessage("Server does not response", this);
            }
            UIUtils.HideSpinner(this, spinner);
        }

        private void UpdateConfirmCount()
        {
            var count = appData.Where(o => ((int)o["Status"] == (int)AppointmentStatus.New)).Count();
            confirmCountLabel.Text = count.ToString();
        }

        private void OnTabChange(object sender, int index)
        {
            currentTabIndex = index;
            try
            {
                if (currentTabIndex == 0)
                {
                    var dataFiltered = appData.Where(o => ((int)o["Status"] == (int)AppointmentStatus.New));
                    var dataConverted = dataFiltered.Select(o => new AppointmentItem((JObject)o)).OrderByDescending(a => a.DateDT);
                    listView.RowHeight = Device.OnPlatform(170, 180, 170);
                    listView.ItemsSource = null;
                    listView.ItemsSource = dataConverted;
                }
                else if (currentTabIndex == 1)
                {
                    var dataFiltered = appData.Where(o => ((int)o["Status"] != (int)AppointmentStatus.New && (DateTime)o["Time"] > DateTime.Now));
                    var dataConverted = dataFiltered.Select(o => new AppointmentItem((JObject)o)).OrderByDescending(a => a.DateDT);
                    listView.RowHeight = Device.OnPlatform(115, 120, 115);
                    listView.ItemsSource = null;
                    listView.ItemsSource = dataConverted;
                }
                else if (currentTabIndex == 2)
                {
                    var dataFiltered = appData.Where(o => ((int)o["Status"] != (int)AppointmentStatus.New && (DateTime)o["Time"] < DateTime.Now));
                    var dataConverted = dataFiltered.Select(o => new AppointmentItem((JObject)o)).OrderByDescending(a => a.DateDT);
                    listView.RowHeight = Device.OnPlatform(115, 120, 115);
                    listView.ItemsSource = null;
                    listView.ItemsSource = dataConverted;
                }
            }
            catch { }
        }

        private void OnDeclineClick(object sender, EventArgs arg)
        {
            var btn = sender as Button;
            var id = btn.GetValue(UIUtils.TagProperty).ToString();
            DataGate.SetAppointmentStatus(id, (int)AppointmentStatus.Declined, OnStatusChanged);
            spinner = UIUtils.ShowSpinner(this);
        }

        private void OnConfirmClick(object sender, EventArgs arg)
        {
            var btn = sender as Button;
            var id = btn.GetValue(UIUtils.TagProperty).ToString();
            DataGate.SetAppointmentStatus(id, (int)AppointmentStatus.Approved, OnStatusChanged);
            spinner = UIUtils.ShowSpinner(this);
        }

        private void OnStatusChanged(ResponseDataJson r)
        {
            UIUtils.HideSpinner(this, spinner);
            if (r.Code == ResponseCode.OK && r.Result == "true")
            {
                RefreshData();
            }
            else
            {
                UIUtils.ShowMessage("Server does not response", this);
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
            var test = jObj.ToString();
            var order = new Order(jObj, true);
            Navigation.PushAsync(new AppointmentDetail(order));

            listView.SelectedItem = null;
        }

    }
}
