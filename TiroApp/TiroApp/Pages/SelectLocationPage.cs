using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace TiroApp.Pages
{
    public class SelectLocationPage : ContentPage
    {
        private RelativeLayout root;
        private StackLayout main;
        private RelativeLayout header;
        private Button continueButton;
        private Button studioBtn;
        private Button myAddressBtn;
        private Image map;
        private Entry addressEntry;
        private Label changeAddressBtn;
        private GeoCoordinate location;
        private Order _order;
        private bool _isMyAddress;
        private StackLayout address;
        private Label muaAddress;

        public SelectLocationPage(Order order)
        {
            this._order = order;
            this.BackgroundColor = Color.White;
            Utils.SetupPage(this);
            BuildLayout();
            ChangeAddress();
        }

        private void BuildLayout()
        {
            header = UIUtils.MakeHeader(this, $"Services {UIUtils.NIARA_SIGN}{_order.TotalPrice}");

            var separator = UIUtils.MakeSeparator(true);
            separator.VerticalOptions = LayoutOptions.Start;

            var nameHolder = new StackLayout {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 10, 0, 10),
                BackgroundColor = Props.GrayColor,
                Children = {
                    new CustomLabel {
                        Text = "SELECT LOCATION",
                        FontSize = 17,
                        FontFamily = UIUtils.FONT_BEBAS_REGULAR,
                        TextColor = Color.Black,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.Start }}
            };

            var questionLabel = new CustomLabel {
                Text = "Where do you want to meet?",
                TextColor = Color.Black,
                FontSize = 15,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(0, 50, 0, 0)
            };

            var buttons = MakeSelectLocationButtons();

            address = MakeAddressBlock();

            muaAddress = new Label {
                Text = _order.Mua.Location.Address,
                TextColor = Color.Black,
                FontSize = 15,
                Margin = new Thickness(20, 0, 20, 0),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            ChangeAddressVisibility();

            var addressSeparator = UIUtils.MakeSeparator();

            map = _order.Mua.Location.Map;
            map.VerticalOptions = LayoutOptions.EndAndExpand;

            continueButton = UIUtils.MakeButton("CONTINUE", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            continueButton.VerticalOptions = LayoutOptions.EndAndExpand;
            continueButton.Clicked += ContinueButton_Clicked;

            main = new StackLayout {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = { header, separator, nameHolder, questionLabel, buttons, address, muaAddress, addressSeparator, map, continueButton }
            };

            root = new RelativeLayout();
            root.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));

            Content = root;
        }

        private void ContinueButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new OrderSummary(_order));
        }

        private StackLayout MakeSelectLocationButtons()
        {
            studioBtn = UIUtils.MakeButton("Studio", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            studioBtn.WidthRequest = 80;
            studioBtn.Clicked += (o, a) => {
                if (_isMyAddress)
                {
                    _isMyAddress = !_isMyAddress;
                    ChangeLocationMode();
                    ChangeAddress();
                    ChangeAddressVisibility();
                }
            };
            myAddressBtn = UIUtils.MakeButton("My Address", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            myAddressBtn.TextColor = Props.ButtonColor;
            myAddressBtn.BackgroundColor = Color.FromHex("F8F8F8");
            myAddressBtn.WidthRequest = 80;
            myAddressBtn.Clicked += (o, a) => {
                if (!_isMyAddress)
                {
                    _isMyAddress = !_isMyAddress;
                    ChangeLocationMode();
                    ChangeAddress();
                    ChangeAddressVisibility();
                }
            };
            return new StackLayout {
                Spacing = 0,
                Margin = new Thickness(20, 0, 20, 20),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = { studioBtn, myAddressBtn }
            };
        }

        private void ChangeAddressVisibility()
        {
            address.IsVisible = _isMyAddress;
            muaAddress.IsVisible = !_isMyAddress;
        }

        private StackLayout MakeAddressBlock()
        {
            var addressACEntry = new AutoCompleteView();
            addressACEntry.WidthRequest = App.ScreenWidth - 40 - 60;
            addressEntry = addressACEntry.EntryText;
            addressEntry.Placeholder = _order.Mua.Location.Address;
            addressEntry.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            addressEntry.HeightRequest = 38;
            addressEntry.FontSize = 15;
            addressEntry.BackgroundColor = Color.White;
            addressEntry.PlaceholderColor = Color.FromHex("787878");
            addressEntry.TextColor = Color.Black;
            addressEntry.IsEnabled = false;
            var sh = new PlaceSearchHelper(addressACEntry);
            sh.OnSelected += (o, p) => { location = p; };

            changeAddressBtn = new CustomLabel {
                Text = "CHANGE",
                TextColor = Props.ButtonColor,
                FontSize = 15,
                FontFamily = UIUtils.FONT_BEBAS_REGULAR,
                IsVisible = false,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center
            };
            changeAddressBtn.GestureRecognizers.Add(new TapGestureRecognizer(v => { ChangeAddress(); }));
            return new StackLayout {
                Spacing = 0,
                Margin = new Thickness(20, 0, 20, 0),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = { addressACEntry, changeAddressBtn }
            };
        }

        private void ChangeLocationMode()
        {
            myAddressBtn.TextColor = _isMyAddress ? Color.White : Props.ButtonColor;
            myAddressBtn.BackgroundColor = _isMyAddress ? Props.ButtonColor : Color.FromHex("F8F8F8");
            studioBtn.TextColor = _isMyAddress ? Props.ButtonColor : Color.White;
            studioBtn.BackgroundColor = _isMyAddress ? Color.FromHex("F8F8F8") : Props.ButtonColor;
            addressEntry.IsEnabled = _isMyAddress;
            changeAddressBtn.IsVisible = _isMyAddress;
        }

        private void ChangeAddress()
        {
            if (!_isMyAddress)
            {
                //_order.Location = _order.Mua.Location;
                var url = $"https://maps.googleapis.com/maps/api/staticmap?center={_order.Mua.Location.Lat},{_order.Mua.Location.Lon}&zoom=12&size={App.ScreenWidth}x300&key={Props.GOOGLE_KEY}";
                map.Source = ImageSource.FromUri(new Uri(url));
            }
            else if (!string.IsNullOrEmpty(addressEntry.Text) && location != null)
            {
                var orderLocation = new Location();
                orderLocation.Address = addressEntry.Text;
                orderLocation.Lat = location.Latitude;
                orderLocation.Lon = location.Longitude;
                _order.Location = orderLocation;
                var url = $"https://maps.googleapis.com/maps/api/staticmap?center={location.Latitude},{location.Longitude}&zoom=12&size={App.ScreenWidth}x300&key={Props.GOOGLE_KEY}";
                map.Source = ImageSource.FromUri(new Uri(url));
            }
        }
    }
}
