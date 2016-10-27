using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public class MuaServicesPage : ContentPage
    {
        private List<Service> serviceList = new List<Service>();
        private ListView list;

        public MuaServicesPage()
        {
            Utils.SetupPage(this);
            BuildLayout();
        }

        private void RefreshList()
        {
            var spinner = UIUtils.ShowSpinner(this);
            DataGate.GetMua(GlobalStorage.Settings.MuaId, data =>
            {
                if (data.Code == ResponseCode.OK)
                {
                    serviceList.Clear();
                    var jObj = JObject.Parse(data.Result);
                    if (jObj["Services"] != null)
                    {
                        foreach (JObject service in (JArray)jObj["Services"])
                        {
                            serviceList.Add(new Service(service, true));
                        }
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        list.ItemsSource = null;
                        list.ItemsSource = serviceList;
                    });
                }
                UIUtils.HideSpinner(this, spinner);
            });
        }

        private void BuildLayout()
        {
            var main = new StackLayout();
            main.BackgroundColor = Color.White;
            main.Spacing = 0;
            var rl = new RelativeLayout();
            rl.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
            this.Content = rl;

            var header = UIUtils.MakeHeader(this, "My Services");

            var addButton = UIUtils.MakeButton("ADD SERVICE", UIUtils.FONT_BEBAS_REGULAR);
            addButton.VerticalOptions = LayoutOptions.EndAndExpand;
            addButton.Clicked += AddButton_Clicked;

            list = new ListView();
            list.RowHeight = Device.OnPlatform(140, 140, 100);
            list.SeparatorColor = Color.Transparent;
            list.SeparatorVisibility = SeparatorVisibility.None;
            list.ItemTemplate = new DataTemplate(() => new ServiceViewCell(OnEditClick, OnDeleteClick));
            list.VerticalOptions = LayoutOptions.FillAndExpand;

            main.Children.Add(header);
            main.Children.Add(UIUtils.MakeSeparator(true));
            main.Children.Add(list);
            main.Children.Add(addButton);

            list.HeightRequest = App.ScreenHeight - Utils.GetControlSize(header).Height - Utils.GetControlSize(addButton).Height - 1;
            main.ForceLayout();
        }

        private void OnDeleteClick(object sender, EventArgs e)
        {
            ConfirmDialog.Show(this, "Do you want to delete service?", new List<string> { "Yes", "No" }, index =>
            {
                if (index == 0)
                {
                    var serviceId = ((Button)sender).GetValue(UIUtils.TagProperty).ToString();
                    var spinner = UIUtils.ShowSpinner(this);
                    DataGate.ServiceDelete(serviceId, data =>
                    {
                        UIUtils.HideSpinner(this, spinner);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            RefreshList();
                        });
                    });
                }
            });
        }

        private void OnEditClick(object sender, EventArgs e)
        {
            var serviceId = ((Button)sender).GetValue(UIUtils.TagProperty).ToString();
            var service = serviceList.Find(s => s.Id == serviceId);
            Navigation.PushAsync(new MuaAddServicePage(service));
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new MuaAddServicePage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshList();
        }
    }

    class ServiceViewCell : ViewCell
    {
        public ServiceViewCell(EventHandler OnEditClick, EventHandler OnDeleteClick)
        {
            var nameLabel = new CustomLabel();
            nameLabel.TextColor = Color.Black;
            nameLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            nameLabel.FontSize = 16;
            nameLabel.HorizontalOptions = LayoutOptions.Start;
            nameLabel.SetBinding(Label.TextProperty, "Name", stringFormat: "Name: {0}");
            var priceLabel = new CustomLabel();
            priceLabel.TextColor = Color.Black;
            priceLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            priceLabel.FontSize = 16;
            priceLabel.HorizontalOptions = LayoutOptions.EndAndExpand;
            priceLabel.SetBinding(Label.TextProperty, "Price", stringFormat: UIUtils.NIARA_SIGN + "{0}");
            var row1 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 5, 20, 5),
                Children = { nameLabel, priceLabel }
            };

            var categoryLabel = new CustomLabel();
            categoryLabel.TextColor = Color.Black;
            categoryLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            categoryLabel.FontSize = 16;
            categoryLabel.HorizontalOptions = LayoutOptions.Start;
            categoryLabel.SetBinding(Label.TextProperty, "Category");
            var lengthLabel = new CustomLabel();
            lengthLabel.TextColor = Color.Black;
            lengthLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            lengthLabel.FontSize = 16;
            lengthLabel.HorizontalOptions = LayoutOptions.EndAndExpand;
            lengthLabel.SetBinding(Label.TextProperty, "Length", stringFormat: "{0} minutes");
            var row2 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 5, 20, 5),
                Children = { categoryLabel, lengthLabel }
            };

            var btn1 = UIUtils.MakeButton("EDIT", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            btn1.TextColor = Props.ButtonColor;
            btn1.BackgroundColor = Color.FromHex("F8F8F8");
            btn1.SetBinding(UIUtils.TagProperty, "Id");
            btn1.Clicked += (o, a) => { OnEditClick?.Invoke(o, a); };
            var btn2 = UIUtils.MakeButton("DELETE", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            btn2.SetBinding(UIUtils.TagProperty, "Id");
            btn2.Clicked += (o, a) => { OnDeleteClick?.Invoke(o, a); };
            var row3 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0,
                Children = { btn1, btn2 }
            };

            var content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                Children = { row1, row2, row3, UIUtils.MakeSeparator(true) }
            };
            this.View = content;
        }
    }
}
