using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages
{
    public abstract class BasePage : ContentPage
    {
        protected RelativeLayout menuLayout;
        private Image showBtn;
        protected RelativeLayout mainLayout;

        public BasePage()
        {
            mainLayout = new RelativeLayout();
        }

        protected virtual IList<View> GetTopItems()
        {
            List<View> list = new List<View>();
            list.Add(GetMenuItem("Home", "home", (v) => { ShowPage(typeof(HomePage)); }));
            list.Add(GetMenuItem("Search", "search", (v) => { ShowPage(typeof(SearchPage)); }));
            if (!string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
                list.Add(GetMenuItem("My Appointments", "appointment", (v)=> { ShowPage(typeof(CustomerAppointments));}));
            list.Add(GetMenuItem("My Account", "avatar", OnMyAccountSelect));
            return list;
        }

        protected virtual IList<View> GetBottomItems()
        {
            List<View> list = new List<View>();
            list.Add(GetMenuItem("List Your Business", "wallet", (v) => { }));
            if (!string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
            {
                list.Add(GetMenuItem("Logout", "muah_logout", v =>
                {
                    GlobalStorage.Settings.CustomerId = string.Empty;
                    GlobalStorage.SaveAppSettings();
                    Notification.CrossPushNotificationListener.UnregisterPushNotification();
                    Utils.ShowPageFirstInStack(this, new LoginPage());
                }));
            }
            return list;
        }

        protected void AddSideMenu()
        {
            var topMenu = new StackLayout();
            topMenu.Padding = new Thickness(0, 40, 0, 0);
            var list = GetTopItems();
            foreach (var item in list)
            {
                topMenu.Children.Add(item);
            }

            var biMenu = new StackLayout();
            biMenu.Padding = new Thickness(0, 40, 0, 0);
            list = GetBottomItems();
            foreach (var item in list)
            {
                biMenu.Children.Add(item);
            }

            var separator = new BoxView()
            {
                //Color = Color.FromHex("#333333"),
                Color = Color.FromHex("#484848"),
                HeightRequest = 1,
                //Opacity = 0.5
            };

            var bottomMenu = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(0, 30, 0, 30),
                Children =
                {
                    GetIconBottomMenu("facebook"),
                    GetIconBottomMenu("instagram"),
                    GetIconBottomMenu("pinterest"),
                    GetIconBottomMenu("twitter"),
                }
            };

            var sideMenu = new RelativeLayout();
            sideMenu.WidthRequest = 290;
            sideMenu.BackgroundColor = Color.FromHex("#352e4f");
            sideMenu.Children.Add(topMenu, Constraint.Constant(0), Constraint.Constant(0));
            sideMenu.Children.Add(bottomMenu
                , Constraint.RelativeToParent((p) =>
                {
                    var w = bottomMenu.Width > 0 ? bottomMenu.Width : Utils.GetControlSize(bottomMenu).Width;
                    return (p.Width - w) / 2;
                })
                , Constraint.RelativeToParent((p) =>
                {
                    var h = bottomMenu.Height > 0 ? bottomMenu.Height : Utils.GetControlSize(bottomMenu).Height;
                    return p.Height - h;
                }));
            sideMenu.Children.Add(separator, Constraint.Constant(0)
                , Constraint.RelativeToParent((p) => 
                {
                    return p.Height - bottomMenu.Height;
                })
                , Constraint.RelativeToParent((p) => 
                {
                    return p.Width;
                }));
            sideMenu.Children.Add(biMenu
                , Constraint.Constant(0)
                , Constraint.RelativeToParent((p) =>
                {
                    return p.Height - Utils.GetControlSize(bottomMenu).Height - Utils.GetControlSize(separator).Height - Utils.GetControlSize(biMenu).Height - 10;
                }));

            var showBtnL = new RelativeLayout();
            showBtn = new Image();
            showBtn.Source = ImageSource.FromResource("TiroApp.Images.menuBtn.png");
            showBtnL.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
            {
                menuLayout.IsVisible = !menuLayout.IsVisible;
                showBtn.Source = ImageSource.FromResource(menuLayout.IsVisible ? "TiroApp.Images.menuBtnWhite.png" : "TiroApp.Images.menuBtn.png");
            }));            
            showBtnL.Children.Add(showBtn
                , Constraint.Constant(20)
                , Constraint.Constant(25)
                , Constraint.Constant(17)
                , Constraint.Constant(12));

            menuLayout = new RelativeLayout();
            menuLayout.IsVisible = false;
            menuLayout.BackgroundColor = Props.BlackoutColor;
            menuLayout.Children.Add(sideMenu
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.Constant(290)
                , Constraint.RelativeToParent((p) => { return p.Height; }));
            menuLayout.GestureRecognizers.Add(new TapGestureRecognizer(v =>
            {
                menuLayout.IsVisible = false;
                showBtn.Source = ImageSource.FromResource("TiroApp.Images.menuBtn.png");
            }));

            mainLayout.Children.Add(menuLayout
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.RelativeToParent((p) => { return p.Width; })
                , Constraint.RelativeToParent((p) => { return p.Height; }));
            mainLayout.Children.Add(showBtnL
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.Constant(60)
                , Constraint.Constant(60));
            Content = mainLayout;
            mainLayout.ForceLayout();
        }

        public async void ShowPage(Type pageType)
        {
            showBtn.Source = ImageSource.FromResource("TiroApp.Images.menuBtn.png");
            if (pageType == Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].GetType())
            {
                return;
            }
            ContentPage page = (ContentPage)Activator.CreateInstance(pageType);
            if (pageType == typeof(HomePage) || pageType == typeof(CustomerAppointments))
            {
                await Navigation.PushAsync(page);
                Navigation.RemovePage(Navigation.NavigationStack[0]);
            }
            else
            {
                await Navigation.PushAsync(page);
            }
        }

        protected StackLayout GetMenuItem(string name, string imageName, Action<View> action)
        {
            var itemName = new CustomLabel();
            itemName.Text = name;
            itemName.TextColor = Color.White;
            itemName.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            itemName.FontSize = 14;
            itemName.Margin = new Thickness(10, 0, 0, 0);

            var itemImage = new Image();
            itemImage.Source = ImageSource.FromResource($"TiroApp.Images.{imageName}.png");
            itemImage.HeightRequest = itemName.FontSize;
            itemImage.WidthRequest = itemName.HeightRequest;

            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(70, 20, 10, 20),
                Children =
                {
                    itemImage,
                    itemName
                }
            };

            itemName.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
            {
                itemName.FontFamily = UIUtils.FONT_SFUIDISPLAY_HEAVY;
                Utils.StartTimer(TimeSpan.FromMilliseconds(600), () =>
                {
                    itemName.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
                    menuLayout.IsVisible = false;
                    action.Invoke(v);
                    return false;
                });
            }));

            return layout;
        }

        private Image GetIconBottomMenu(string name)
        {
            var icon = new Image();
            icon.Source = ImageSource.FromResource($"TiroApp.Images.{name}.png");
            icon.HeightRequest = 40;
            icon.WidthRequest = icon.HeightRequest;
            icon.Margin = new Thickness(5, 0, 5, 0);
            icon.HorizontalOptions = LayoutOptions.Center;
            return icon;
        }

        private void OnMyAccountSelect(View v)
        {
            if (string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
            {
                ShowPage(typeof(LoginPage));
                return;
            }
            var spinner = UIUtils.ShowSpinner(this);
            DataGate.GetCustomerInfo(GlobalStorage.Settings.CustomerId, resp => {
                if (resp.Code == ResponseCode.OK)
                {
                    var jObj = JObject.Parse(resp.Result);
                    Utils.ShowPageFirstInStack(this, new AccountPage(jObj));
                }
                else
                {
                    UIUtils.ShowServerUnavailable(this);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    UIUtils.HideSpinner(this, spinner);
                });
            });
        }

    }
}
