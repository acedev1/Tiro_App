using ImageCircle.Forms.Plugin.Abstractions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages
{
    public class OrderSummary : ContentPage
    {
        private RelativeLayout root;
        private Order _order;
        private bool _isInfo;
        private bool _isNeedCardPay;
        private Button continueButton;
        //private OrderProcess orderProcess;
        private CustomerSignupView registerLayout;
        private StackLayout servicesInfo;
        private StackLayout info;
        private StackLayout addressHolder;
        private BoxView separatorDate;
        private Label alertInfo;
        private RelativeLayout header;
        private TabView tabView;
        private ContentView tabViewContent;
        private StackLayout main;

        public OrderSummary(Order order, bool isInfo = false)
        {
            _isInfo = isInfo;
            _order = order;
            _isNeedCardPay = _isInfo && _order.Status == AppointmentStatus.Approved && _order.PaymentType == PaymentType.Card;
            Utils.SetupPage(this);
            BuildLayout();
            //if (isInfo)
            //{
            //    orderProcess = new OrderProcess(_order, root, this);
            //}
        }

        private void BuildLayout()
        {
            header = UIUtils.MakeHeader(this, "Booking Summary");

            var separator = UIUtils.MakeSeparator(true);
            separator.VerticalOptions = LayoutOptions.Start;

            var muaHeader = MakeMuaHeader(_order);

            var muaHeaderSeparator = UIUtils.MakeSeparator(true);

            servicesInfo = new StackLayout {
                Spacing = 0,
                Margin = new Thickness(0, 10, 0, 10)
            };
            foreach (var item in _order.Basket)
            {
                servicesInfo.Children.Add(MakeOrderItemRaw(item));
            }
            var serviceSeparator = UIUtils.MakeSeparator();
            servicesInfo.Children.Add(serviceSeparator);
            var subtotal = new CustomLabel {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Text = "Subtotal",
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR
            };
            var totalPrice = new CustomLabel {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Text = $"{UIUtils.NIARA_SIGN}{_order.TotalPrice}",
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_SEMIBOLD
            };
            var total =  new StackLayout {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 10, 20, 10),
                Children = { subtotal, totalPrice }
            };
            servicesInfo.Children.Add(total);

            separatorDate = UIUtils.MakeSeparator(true);

            var dateTime = new CustomLabel {
                Text = _order.DateTime.ToString("dd.MM.yyyy hh:mm tt", Utils.EnCulture),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(0, 10, 0, 10),
                TextColor = Props.ButtonColor,
                FontSize = 22,
                FontFamily = UIUtils.FONT_BEBAS_REGULAR
            };

            var separatorAddress = UIUtils.MakeSeparator(true);

            var address = new CustomLabel {
                Text = _order.Mua.Address,
                TextColor = Color.Gray,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT,
                FontSize = 14,
                Margin = new Thickness(20, 10, 20, 10)
            };

            var map = _order.Mua.Map;
            var loc = string.Format("{0},{1}", _order.Mua.Lat, _order.Mua.Lon);
            map.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                switch (Device.OS)
                {
                    case TargetPlatform.iOS:
                        Device.OpenUri(
                          new Uri(string.Format("http://maps.apple.com/maps?q={0}&sll={1}", address.Text.Replace(' ', '+'), loc)));
                        break;
                    case TargetPlatform.Android:
                        Device.OpenUri(
                          new Uri(string.Format("geo:0,0?q={0}({1})", loc, address.Text)));
                        break;
                }
            }));

            addressHolder = new StackLayout {
                Spacing = 0,
                Children = { address, map }
            };

            info = new StackLayout {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = { muaHeader, muaHeaderSeparator, servicesInfo, separatorDate, dateTime, separatorAddress, addressHolder }
            };
            var scrollView = new ScrollView { Content = info };
            
            continueButton = UIUtils.MakeButton("BOOK NOW", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            if (_isInfo)
            {
                continueButton.Text = _isNeedCardPay ? "Pay" : "OK";
            }
            continueButton.VerticalOptions = LayoutOptions.EndAndExpand;
            continueButton.Clicked += OnContinueClick;

            tabView = new TabView(new List<string> { "LOG IN", "REGISTER" }, Color.FromHex("CF7090"), true, true);
            tabView.SelectedIndex = 1;
            tabView.Margin = new Thickness(0, 5, 0, 0);
            tabView.HeightRequest = 50;
            tabView.VerticalOptions = LayoutOptions.EndAndExpand;
            tabView.OnIndexChange += OnTabChange;

            main = new StackLayout {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = { header, separator, scrollView, continueButton }
            };

            root = new RelativeLayout();
            root.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));

            Content = root;
        }

        private void OnTabChange(object sender, int e)
        {
            if (tabView.SelectedIndex == 0)
            {
                BuildLogin();
            }
            else
            {
                BuildRegister();
            }
        }

        private void BuildRegister()
        {
            registerLayout = new CustomerSignupView(this, true);
            registerLayout.OnFinish += (o, a) => { SendOrder(); };
            tabViewContent.Content = registerLayout;
        }

        private void BuildLogin()
        {
            var login = new StackLayout();
            login.Orientation = StackOrientation.Vertical;
            login.Spacing = 0;
            login.BackgroundColor = Color.White;

            var emailEntry = UIUtils.MakeEntry("Email", UIUtils.FONT_SFUIDISPLAY_BOLD);
            login.Children.Add(emailEntry);
            login.Children.Add(UIUtils.MakeSeparator());

            var pswdEntry = UIUtils.MakeEntry("Password", UIUtils.FONT_SFUIDISPLAY_BOLD);
            pswdEntry.IsPassword = true;
            login.Children.Add(pswdEntry);
            login.Children.Add(UIUtils.MakeSeparator());

            var loginButton = UIUtils.MakeButton("LOG IN", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            loginButton.Margin = new Thickness(0, 40, 0, 0);
            login.Children.Add(loginButton);
            loginButton.Clicked += (o, a) =>
            {
                var email = emailEntry.Text;
                var password = pswdEntry.Text;
                DoLogin(email, password);
            };
            login.VerticalOptions = LayoutOptions.EndAndExpand;
            tabViewContent.Content = login;
        }

        private void DoLogin(string email, string password)
        {
            var spinner = UIUtils.ShowSpinner(this);
            DataGate.CustomerLoginJson(email, Ext.MD5.GetMd5String(password), null, (data) =>
            {
                if (data.Code == ResponseCode.OK)
                {
                    OnLoginOk(JObject.Parse(data.Result));
                }
                else
                {
                    UIUtils.ShowMessage("Login failed. Try later", this);
                }
                UIUtils.HideSpinner(this, spinner);
            });
        }

        private void OnLoginOk(JObject obj)
        {
            var id = obj["Id"] != null ? (string)obj["Id"] : null;
            if (!string.IsNullOrEmpty(id))
            {
                GlobalStorage.Settings.CustomerId = id;
                GlobalStorage.SaveAppSettings();
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (!(bool)obj["AllowFreeMakeover"])
                    {
                        _order.IsFree = false;
                    }
                    SendOrder();
                });
            }
            else
            {
                UIUtils.ShowMessage("Login failed. Wrong email or password", this);
            }
        }

        private StackLayout MakeMuaHeader(Order order)
        {
            var muaIcon = new CircleImage();
            muaIcon.Source = order.Mua.ArtistImage;
            muaIcon.Aspect = Aspect.AspectFill;
            muaIcon.HeightRequest = 60;
            muaIcon.WidthRequest = muaIcon.HeightRequest;
            muaIcon.HorizontalOptions = LayoutOptions.CenterAndExpand;
            muaIcon.VerticalOptions = LayoutOptions.CenterAndExpand;

            var muaName = new CustomLabel {
                Text = order.Mua.FullName,
                TextColor = Color.Black,
                FontSize = 20,
                FontFamily = UIUtils.FONT_BEBAS_REGULAR
            };
            var muaBusiness = new CustomLabel {
                Text = $"Artist at {order.Mua.BusinessName}",
                TextColor = Color.Black,
                FontSize = 14,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR
            };
            var textHolder = new StackLayout {
                Spacing = 0,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(10, 0, 0, 0),
                Children = { muaName, muaBusiness }
            };

            return new StackLayout {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
                Margin = new Thickness(0, 10, 0, 10),
                HorizontalOptions = LayoutOptions.Center,
                Children = { muaIcon, textHolder }
            };
        }

        private StackLayout MakeOrderItemRaw(OrderItem item)
        {
            var serviceName = new CustomLabel {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Text = $"{item.Count}X {item.Service.Name}",
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR
            };
            var servicePrice = new CustomLabel {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Text = $"{UIUtils.NIARA_SIGN}{item.Service.Price}",
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_SEMIBOLD
            };
            return new StackLayout {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 10, 20, 10),
                Children = { serviceName, servicePrice }
            };
        }

        private void OnContinueClick(object sender, EventArgs e)
        {
            if (_isNeedCardPay)
            {
                new PaymentHelper().Start(_order.AppointmentId, this, (result) =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (result == ResponseCode.OK)
                        {
                            _isNeedCardPay = false;
                            continueButton.Text = "OK";
                            UIUtils.ShowMessage("Payment success", this);
                        }
                        else
                        {
                            UIUtils.ShowMessage("Payment failed", this);
                        }
                    });
                });
                return;
            }
            if (_isInfo)
            {
                Device.BeginInvokeOnMainThread(() => {
                    Navigation.PopAsync();
                });
                return;
            }
            if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
            {
                continueButton.IsVisible = false;
                info.Children.Remove(servicesInfo);
                info.Children.Remove(addressHolder);
                info.Children.Remove(separatorDate);
                header = UIUtils.MakeHeader(this, "Your Booking is Incomplete", true);
                main.Children.RemoveAt(0);
                main.Children.Insert(0, header);
                alertInfo = new CustomLabel {
                    Text = "To complete your booking, you need to\n register an account with us or log in",
                    TextColor = Color.Black,
                    FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 10)
                };
                info.Children.Add(alertInfo);
                main.Children.Add(tabView);
                tabViewContent = new ContentView();
                tabViewContent.VerticalOptions = LayoutOptions.EndAndExpand;
                main.Children.Add(tabViewContent);
                BuildRegister();
            }
            else
            {
                SendOrder();
            }
        }
        private void SendOrder()
        {
            var spinner = UIUtils.ShowSpinner(this);
            DataGate.AddAppointment(string.Empty, _order, resp =>
            {
                UIUtils.HideSpinner(this, spinner);
                if (resp.Code == ResponseCode.OK && resp.Result == "true")
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        tabView.IsVisible = false;
                        if (tabViewContent != null)
                        {
                            tabViewContent.IsVisible = false;
                            alertInfo.IsVisible = false;
                        }
                        info.Children.Add(new Label
                        {
                            Text = "Your beauty professional has received\n your booking request and will get back\n "
                                    + " to you shortly.\n\n"
                                    + "You can check out the status of your\n request on the MyAppointments\n menu item",
                            TextColor = Color.Black,
                            FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
                            Margin = new Thickness(10)
                        });
                        continueButton.IsVisible = true;
                        continueButton.Clicked -= OnContinueClick;
                        continueButton.Clicked += (o, a) =>
                        {
                            Utils.ShowPageFirstInStack(this, new CustomerAppointments());
                        };
                        continueButton.Text = "CHECK MY APPOINTMENTS";
                        header = UIUtils.MakeHeader(this, "Booking Confirmed");
                        main.Children.RemoveAt(0);
                        main.Children.Insert(0, header);
                    });
                }
                else
                {
                    UIUtils.ShowMessage("Booking was not added. Try later.", this);
                }
            });
        }        
    }
}
