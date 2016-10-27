using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Pages;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Model
{
    public class OrderProcess
    {
        private RelativeLayout _layout;
        private BoxView blackout;
        private Page _page;
        private Order _order;

        public OrderProcess(Order order, RelativeLayout layout, Page page)
        {
            _order = order;
            _layout = layout;
            _page = page;
            //blackout = new BoxView();
            //blackout.Color = Props.BlackoutColor;
            //blackout.IsVisible = false;
            //_layout.Children.Add(blackout, Constraint.Constant(0), Constraint.Constant(0)
            //    , Constraint.RelativeToParent(p => p.Width)
            //    , Constraint.RelativeToParent(p => p.Height));
        }

        //public void BuildPaymentLayout()
        //{
        //    blackout.IsVisible = true;
        //    var layout = new StackLayout { Spacing = 0 };
        //    layout.WidthRequest = Utils.GetControlSize(_layout).Width - 80;

        //    var creditCardPay = UIUtils.MakeButton("PAY WITH CREDIT CARD", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
        //    creditCardPay.BackgroundColor = Color.FromHex("#4E3752");
        //    layout.Children.Add(creditCardPay);
        //    creditCardPay.Clicked += (s, args) => {
        //        _order.PaymentType = PaymentType.Card;
        //        _layout.Children.Remove(layout);
        //        if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerPhoneNumber))
        //        {
        //            BuildConfirmMobileLayout();
        //        }
        //        else if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerCardNumber))
        //        {
        //            BuildCreditCardInfoLayout();
        //        }
        //        else if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
        //        {
        //            BuildLoginLayout();
        //        }
        //        else
        //        {
        //            blackout.IsVisible = false;
        //            _page.Navigation.PushAsync(new OrderSummary(_order));
        //        }
        //    };

        //    var cashPay = UIUtils.MakeButton("PAY WITH CASH", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
        //    layout.Children.Add(cashPay);
        //    cashPay.Clicked += (s, args) => {
        //        _order.PaymentType = PaymentType.Cash;
        //        _layout.Children.Remove(layout);
        //        blackout.IsVisible = false;
        //        if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
        //        {
        //            BuildLoginLayout();
        //        }
        //        else
        //        {
        //            blackout.IsVisible = false;
        //            _page.Navigation.PushAsync(new OrderSummary(_order));
        //        }
        //    };

        //    var cancel = UIUtils.MakeButton("CANCEL", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
        //    cancel.BackgroundColor = Color.White;
        //    cancel.TextColor = Props.ButtonColor;
        //    layout.Children.Add(cancel);
        //    cancel.Clicked += (s, args) => {
        //        _layout.Children.Remove(layout);
        //        blackout.IsVisible = false;
        //    };

        //    _layout.Children.Add(layout
        //        , Constraint.RelativeToParent(p => p.Width / 2 - Utils.GetControlSize(layout).Width / 2)
        //        , Constraint.RelativeToParent(p => p.Height / 2 - Utils.GetControlSize(layout).Height / 2));
        //}

        private void BuildConfirmMobileLayout()
        {
            var headerLabel = new CustomLabel {
                Text = "Confirm Your Mobile Number",
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT,
                FontSize = 18,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 10, 0, 10),
                VerticalTextAlignment = TextAlignment.Center
            };

            var infoLabel = new CustomLabel {
                Text = "Where can we SMS your booking confirmation and receipt?",
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                FontSize = 18,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 10, 0, 10)
            };

            var entryNumber = UIUtils.MakeEntry("", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            entryNumber.Keyboard = Keyboard.Telephone;
            entryNumber.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            entryNumber.Margin = new Thickness(40, 10, 40, 0);

            var separator = UIUtils.MakeSeparator();
            separator.Margin = new Thickness(40, 2, 40, 20);

            var confirm = UIUtils.MakeButton("CONFIRM", UIUtils.FONT_SFUIDISPLAY_MEDIUM);

            var layout = new StackLayout {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = { headerLabel, infoLabel, entryNumber, separator, confirm }
            };
            _layout.Children.Add(layout
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width));

            var close = new Image();
            close.Source = ImageSource.FromResource("TiroApp.Images.closeBlack.png");
            close.HeightRequest = 18;
            close.WidthRequest = close.HeightRequest;
            close.GestureRecognizers.Add(new TapGestureRecognizer((v) => {
                _layout.Children.Remove(layout);
                _layout.Children.Remove(close);
                blackout.IsVisible = false;
            }));
            _layout.Children.Add(close
                , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(close).Width - 20)
                , Constraint.Constant(20));

            confirm.Clicked += (s, args) => {
                if (!UIUtils.ValidateEntriesWithEmpty(new Entry[] { entryNumber }, _page))
                {
                    return;
                }
                GlobalStorage.Settings.CustomerPhoneNumber = entryNumber.Text;
                GlobalStorage.SaveAppSettings();
                _layout.Children.Remove(layout);
                _layout.Children.Remove(close);
                if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerCardNumber))
                {
                    BuildCreditCardInfoLayout();
                }
                else if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
                {
                    BuildLoginLayout();
                }
                else
                {
                    blackout.IsVisible = false;
                    _page.Navigation.PushAsync(new OrderSummary(_order));
                }
            };
        }

        private void BuildCreditCardInfoLayout()
        {
            var headerLabel = new CustomLabel {
                Text = "Add a Payment Card",
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT,
                FontSize = 18,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 10, 0, 10),
                VerticalTextAlignment = TextAlignment.Center
            };

            var infoLabel = new CustomLabel {
                Text = "You will not be charged until your appointment ends.",
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 18,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 10, 0, 10)
            };

            var entryNumber = UIUtils.MakeEntry("Credit Card Number", UIUtils.FONT_SFUIDISPLAY_BOLD);
            entryNumber.Keyboard = Keyboard.Numeric;
            entryNumber.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            entryNumber.Margin = new Thickness(40, 10, 40, 0);

            var separator = UIUtils.MakeSeparator();
            separator.Margin = new Thickness(40, 2, 40, 20);

            var scanLabel = new CustomLabel {
                Text = "Scan your card",
                TextColor = Color.Red,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 15, 0, 15),
                VerticalTextAlignment = TextAlignment.Center
            };

            var verifyLabel = new CustomLabel {
                Text = "Verified & Secured. We accept:",
                TextColor = Props.GrayColor,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 20, 0, 10),
                VerticalTextAlignment = TextAlignment.Center
            };

            var visa = new Image();
            visa.Source = ImageSource.FromResource("TiroApp.Images.visa.png");
            visa.HeightRequest = 30;
            visa.WidthRequest = 45;
            visa.HorizontalOptions = LayoutOptions.CenterAndExpand;
            visa.Margin = new Thickness(0, 0, 0, 10);

            var confirm = UIUtils.MakeButton("BOOK APPOINTMENT", UIUtils.FONT_SFUIDISPLAY_MEDIUM);

            var layout = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = { headerLabel, infoLabel, entryNumber, separator, scanLabel, verifyLabel, visa, confirm }
            };
            _layout.Children.Add(layout
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width));

            var close = new Image();
            close.Source = ImageSource.FromResource("TiroApp.Images.closeBlack.png");
            close.HeightRequest = 20;
            close.WidthRequest = close.HeightRequest;
            close.GestureRecognizers.Add(new TapGestureRecognizer((v) => {
                _layout.Children.Remove(layout);
                _layout.Children.Remove(close);
                blackout.IsVisible = false;
            }));
            _layout.Children.Add(close
                , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(close).Width - 20)
                , Constraint.Constant(20));

            confirm.Clicked += (s, args) => {
                if (!UIUtils.ValidateEntriesWithEmpty(new Entry[] { entryNumber }, _page))
                {
                    return;
                }
                GlobalStorage.Settings.CustomerCardNumber = entryNumber.Text;
                GlobalStorage.SaveAppSettings();
                _layout.Children.Remove(layout);
                _layout.Children.Remove(close);
                if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
                {
                    BuildLoginLayout();
                }
                else
                {
                    blackout.IsVisible = false;
                    _page.Navigation.PushAsync(new OrderSummary(_order));
                }
            };
        }

        private void BuildLoginLayout()
        {
            var headerLabel = new CustomLabel {
                Text = "Your Booking is incomplete",
                TextColor = Color.Red,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT,
                FontSize = 18,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 10, 0, 10),
                VerticalTextAlignment = TextAlignment.Center
            };

            var infoLabel = new CustomLabel {
                Text = "To complete your booking, you need to register an account with us or login",
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                FontSize = 18,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 10, 0, 10)
            };

            var signupLabel = new CustomLabel {
                Text = "By signing up, you agree to the Terms of Service and Privacy Policy",
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                FontSize = 14,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HeightRequest = 45,
                Margin = new Thickness(0, 10, 0, 10)
            };


            var loginButton = UIUtils.MakeButton("LOG IN", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            loginButton.BackgroundColor = Color.FromHex("#4E3752");
            var signupFBButton = UIUtils.MakeButton("SIGN UP WITH FACEBOOK", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            signupFBButton.BackgroundColor = Color.FromHex("5E73AB");
            var signupMailButton = UIUtils.MakeButton("SIGN UP WITH MAIL", UIUtils.FONT_SFUIDISPLAY_MEDIUM);

            var layout = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = { headerLabel, infoLabel, signupLabel, loginButton, signupFBButton, signupMailButton }
            };
            _layout.Children.Add(layout
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width));

            var close = new Image();
            close.Source = ImageSource.FromResource("TiroApp.Images.closeBlack.png");
            close.HeightRequest = 20;
            close.WidthRequest = close.HeightRequest;
            close.GestureRecognizers.Add(new TapGestureRecognizer((v) => {
                _layout.Children.Remove(layout);
                _layout.Children.Remove(close);
                blackout.IsVisible = false;
            }));
            _layout.Children.Add(close
                , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(close).Width - 20)
                , Constraint.Constant(20));

            signupMailButton.Clicked += (s, args) => {
                _layout.Children.Remove(layout);
                _layout.Children.Remove(close);
                blackout.IsVisible = false;
                _page.Navigation.PushAsync(new SignUpEmailPage());
            };
            loginButton.Clicked += (s, args) => {
                _layout.Children.Remove(layout);
                _layout.Children.Remove(close);
                blackout.IsVisible = false;
                _page.Navigation.PushAsync(new LoginPage(true));
            };
            signupFBButton.Clicked += (s, args) => {
                _layout.Children.Remove(layout);
                _layout.Children.Remove(close);
                blackout.IsVisible = false;
                _page.Navigation.PushAsync(new OrderSummary(_order)); //TEMP
            };
        }

    }
}
