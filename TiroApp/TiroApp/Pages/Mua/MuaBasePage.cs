using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public abstract class MuaBasePage : BasePage
    {
        private ActivityIndicator spinner;

        protected override IList<View> GetTopItems()
        {
            List<View> list = new List<View>();
            list.Add(GetMenuItem("Appointments", "appointment", (v) => { ShowPage(typeof(MuaHomePage)); }));
            list.Add(GetMenuItem("My Availability", "muah_availability", OnAvailablilitySelect));
            list.Add(GetMenuItem("My services", "muah_services", (v) => { Navigation.PushAsync(new MuaServicesPage()); }));
            list.Add(GetMenuItem("My portfolio", "muah_services", v => { OnMyAccountSelect(true); }));
            return list;
        }

        protected override IList<View> GetBottomItems()
        {
            List<View> list = new List<View>();
            list.Add(GetMenuItem("My account", "avatar", v => { OnMyAccountSelect(); }));
            list.Add(GetMenuItem("Logout", "muah_logout", (v) => {
                GlobalStorage.Settings.MuaId = string.Empty;
                GlobalStorage.SaveAppSettings();
                Notification.CrossPushNotificationListener.UnregisterPushNotification();
                Utils.ShowPageFirstInStack(this, new MuaLoginPage());
            }));
            return list;
        }

        private void OnMyAccountSelect(bool isOnlyPortfolio = false)
        {
            spinner = UIUtils.ShowSpinner(this);
            DataGate.GetMuaInfo(GlobalStorage.Settings.MuaId, resp => {
                if (resp.Code == ResponseCode.OK)
                {
                    var jObj = JObject.Parse(resp.Result);
                    //Utils.ShowPageFirstInStack(this, new AccountPage(jObj, true));
                    if (isOnlyPortfolio)
                    {
                        Device.BeginInvokeOnMainThread(() => { 
                        Navigation.PushAsync(new MuaProfilePage(jObj));
                        });
                    }
                    else
                    {
                        Utils.ShowPageFirstInStack(this, new MuaAccountPage(jObj));
                    }
                }
                else
                {
                    UIUtils.ShowServerUnavailable(this);
                }
            });
            Device.BeginInvokeOnMainThread(() =>
            {
                UIUtils.HideSpinner(this, spinner);
            });
        }

        private void OnAvailablilitySelect(View v)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                spinner = UIUtils.ShowSpinner(this);
            });
            DataGate.MuaGetAvailability(GlobalStorage.Settings.MuaId, DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(5), false, result =>
            {
                if (result.Code == ResponseCode.OK)
                {
                    var avail = Availibility.Parse(result.Result);
                    var ap = new AvailabilityPage()
                    {
                        Mode = ViewMode.MultiRange,
                        SelectedDate = DateTime.Now,
                        Availibility = avail,
                        ButtonText = "Save"
                    };
                    ap.OnFinishSelection += (o, s) =>
                    {
                        SaveAvailability(ap.Availibility);
                    };
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PushAsync(ap);
                    });
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

        private void SaveAvailability(Availibility a)
        {
            spinner = UIUtils.ShowSpinner(this);
            DataGate.MuaSetAvailability(GlobalStorage.Settings.MuaId, a, r =>
            {
                if (r.Code != ResponseCode.OK)
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
