using Newtonsoft.Json.Linq;
using PushNotification.Plugin;
using PushNotification.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;
using TiroApp.Pages;
using TiroApp.Pages.Mua;
using Xamarin.Forms;

namespace TiroApp.Notification
{
    public class CrossPushNotificationListener : IPushNotificationListener
    {
        public static string DeviceToken = null;

        private bool showMessage = false;
        //Here you will receive all push notification messages
        //Messages arrives as a dictionary, the device type is also sent in order to check specific keys correctly depending on the platform.
        void IPushNotificationListener.OnMessage(JObject parameters, DeviceType deviceType)
        {
            showMessage = false;
            try
            {
                int atype = (int)parameters["atype"];
                if (atype == (int)AppointmentStatus.New || atype == (int)AppointmentStatus.Paid)
                {
                    if (GlobalStorage.Settings.MuaId == ((string)parameters["amua"]).ToLower())
                    {
                        parameters[PushNotificationKey.Data] = new JObject();
                        parameters[PushNotificationKey.Data][PushNotificationKey.Message] = parameters["aps"]["alert"];
                        showMessage = true;
                        var topPage = GetTopStackPage();
                        if (topPage is MuaHomePage)
                        {
                            ((MuaHomePage)topPage).RefreshData();
                        }
                    }
                }
                else if (atype == (int)AppointmentStatus.Approved || atype == (int)AppointmentStatus.Declined)
                {
                    if (GlobalStorage.Settings.CustomerId == ((string)parameters["acustomer"]).ToLower())
                    {
                        parameters[PushNotificationKey.Data] = new JObject();
                        parameters[PushNotificationKey.Data][PushNotificationKey.Message] = parameters["aps"]["alert"];
                        showMessage = true;
                        var topPage = GetTopStackPage();
                        if (topPage is CustomerAppointments)
                        {
                            ((CustomerAppointments)topPage).RefreshData();
                        }
                    }
                }

                //if (deviceType == DeviceType.iOS)
                //{
                //    UIUtils.ShowMessage((string)parameters["aps"]["alert"], App.Current.MainPage);
                //}
            }
            catch { }
        }

        //Gets the registration token after push registration
        void IPushNotificationListener.OnRegistered(string Token, DeviceType deviceType)
        {
            Debug.WriteLine(string.Format("Push Notification - Device Registered - Token : {0}", Token));
            DeviceToken = Token;
            CrossPushNotificationListener.RegisterPushNotification();
        }

        //Fires when device is unregistered
        void IPushNotificationListener.OnUnregistered(DeviceType deviceType)
        {
            Debug.WriteLine("Push Notification - Device Unnregistered");
        }

        //Fires when error
        void IPushNotificationListener.OnError(string message, DeviceType deviceType)
        {
            Debug.WriteLine(string.Format("Push notification error - {0}", message));
        }

        //Enable/Disable Showing the notification
        bool IPushNotificationListener.ShouldShowNotification()
        {
            return showMessage;
        }

        private Page GetTopStackPage()
        {
            var mainPage = ((NavigationPage)App.Current.MainPage);
            var topPage = mainPage.Navigation.NavigationStack[mainPage.Navigation.NavigationStack.Count - 1];
            return topPage;
        }

        public static void RegisterPushNotification()
        {
            DataGate.PNRegister(GlobalStorage.Settings.MuaId, GlobalStorage.Settings.CustomerId, DeviceToken);
        }

        public static void UnregisterPushNotification()
        {
            DataGate.PNUnregister(GlobalStorage.Settings.MuaId, GlobalStorage.Settings.CustomerId);
        }
    }
}
