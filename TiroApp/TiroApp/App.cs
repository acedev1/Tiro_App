using PushNotification.Plugin;
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
            else if (!string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId))
            {
                p = new HomePage();
            }
            else
            {
                p = new LaunchPage();
            }
            MainPage = new NavigationPage(p);
        }

        protected override void OnStart()
        {
            CrossPushNotification.Current.Register();
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
