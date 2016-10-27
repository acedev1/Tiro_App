using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;
using TiroApp.Pages;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class AccountLayout : StackLayout
    {
        private Client _client;
        private Page _page;

        public AccountLayout(Client client, Page page)
        {
            _client = client;
            _page = page;
            BuildLayout();
        }

        private void BuildLayout()
        {
            this.Spacing = 0;

            var headerLabel = new CustomLabel();
            headerLabel.Text = "My Account";
            headerLabel.TextColor = Color.Black;
            headerLabel.BackgroundColor = Color.White;
            headerLabel.HorizontalTextAlignment = TextAlignment.Center;
            headerLabel.VerticalTextAlignment = TextAlignment.Center;
            headerLabel.HeightRequest = 50;
            headerLabel.FontSize = 16;
            headerLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            headerLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
            this.Children.Add(headerLabel);

            var separator = UIUtils.MakeSeparator(true);
            this.Children.Add(separator);

            AddInfoLayout();

            AddButtonBlock("Contact Information", (s, a) => {
                _page.Navigation.PushAsync(new EditInfoPage(_client));
            });
            if (_client is Customer)
            {
                AddButtonBlock("Payment Method", (s, a) => {
                    _page.Navigation.PushAsync(new EditPaymentPage());
                });
            }
            AddButtonBlock("Getting Started", (s, a) => { });
            AddButtonBlock("Help Center", (s, a) => { });
            AddButtonBlock("Privacy Policy", (s, a) => { });
            AddButtonBlock("Terms of Service", (s, a) => { });

            var logOut = new Button();
            logOut.BackgroundColor = Color.Transparent;
            logOut.Text = "Log Out";
            logOut.TextColor = Props.ButtonColor;
            logOut.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            logOut.Margin = new Thickness(20, 0, 0, 20);
            logOut.VerticalOptions = LayoutOptions.EndAndExpand;
            logOut.HorizontalOptions = LayoutOptions.StartAndExpand;
            logOut.Clicked += (s, a) => {
                GlobalStorage.Settings.CustomerId = string.Empty;
                GlobalStorage.Settings.MuaId = string.Empty;
                GlobalStorage.SaveAppSettings();
                Notification.CrossPushNotificationListener.UnregisterPushNotification();
                Utils.ShowPageFirstInStack(_page, new LoginPage());
            };
            this.Children.Add(logOut);
        }

        private void AddButtonBlock(string text, EventHandler handler)
        {
            var btn = new Button();
            btn.BackgroundColor = Color.Transparent;
            btn.Text = text;
            btn.TextColor = Color.Black;
            btn.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            btn.Margin = new Thickness(20, 4, 20, 4);
            btn.HorizontalOptions = LayoutOptions.StartAndExpand;
            btn.Clicked += handler;

            var separator = UIUtils.MakeSeparator(true);

            this.Children.Add(btn);
            this.Children.Add(separator);
        }

        private void AddInfoLayout()
        {
            var image = new CircleImage();
            image.Source = _client is Customer ? _client.CustomerImage : ((MuaArtist)_client).ArtistImage;
            image.Margin = new Thickness(20, 20, 20, 20);
            image.HeightRequest = 60;
            image.WidthRequest = image.HeightRequest;
            image.Aspect = Aspect.AspectFill;

            var name = new CustomLabel
            {
                Text = _client.FullName,
                FontSize = 22,
                FontFamily = UIUtils.FONT_BEBAS_REGULAR,
                TextColor = Color.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { image, name }
            };
            var separator = UIUtils.MakeSeparator(true);
            this.Children.Add(layout);
            this.Children.Add(separator);
        }
    }
}
