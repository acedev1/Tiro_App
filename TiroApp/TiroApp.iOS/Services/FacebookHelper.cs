using System;
using System.Linq;
using TiroApp.iOS;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(TiroApp.Ios.Services.FacebookHelper))]
namespace TiroApp.Ios.Services
{
    public class FacebookHelper : IFacebookHelper
    {
        private string data;

        public void Start(Action<ResponseDataJson> callback)
        {
            var auth = new OAuth2Authenticator(
                clientId: "699621406860231",
                scope: "public_profile,email",
                authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
                redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));

            var firstController = UIApplication.SharedApplication.KeyWindow.RootViewController.ChildViewControllers.First().ChildViewControllers.Last().ChildViewControllers.First();
            //var navcontroller = firstController as UINavigationController;


            auth.Completed += (sender, eventArgs) =>
            {
                // We presented the UI, so it's up to us to dimiss it on iOS.
                firstController.DismissViewController(true, null);

                if (eventArgs.IsAuthenticated)
                {
                    var request = new OAuth2Request("GET", new Uri("https://graph.facebook.com/me?fields=email,name,first_name,last_name"), null, eventArgs.Account);
                    request.GetResponseAsync().ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            callback?.Invoke(new ResponseDataJson(ResponseCode.Fail, null));
                        }
                        else
                        {
                            data = t.Result.GetResponseText();
                            callback?.Invoke(new ResponseDataJson(ResponseCode.OK, data));                            
                        }
                    });
                }
                else
                {
                    callback?.Invoke(new ResponseDataJson(ResponseCode.Fail, null));
                }
            };
            //var context = Xamarin.Forms.Forms.Context;
            //context.StartActivity(auth.GetUI(context));
            firstController.PresentViewController (auth.GetUI (), true, null);
        }
    }
}