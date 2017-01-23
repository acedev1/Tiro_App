using Gis4Mobile.Services.GeoLocation;
using PushNotification.Plugin;
using System;
using TiroApp.Pages;
using Xamarin.Forms;

namespace TiroApp
{
    public class App : Application
    {
        static public int ScreenWidth;
        static public int ScreenHeight;

        public App()
        {
            Utils.CheckCrash();
            Page p;
            if (!string.IsNullOrEmpty(GlobalStorage.Settings.MuaId))
            {
                p = new Pages.Mua.MuaHomePage();
            }
            else //(!string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
            {
                p = new HomePage();
            }
            MainPage = new NavigationPage(p);
            MainPage.Navigation.PushAsync(new Splash2Page());            
            Geolocator.Instance.GetPosition();
        }

        protected override void OnStart()
        {
            CrossPushNotification.Current.Register();
            Utils.StartTimer(TimeSpan.FromSeconds(Device.OnPlatform(3, 6, 3)), () =>
            {
                MainPage.Navigation.PopAsync();
                return false;
            });
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
