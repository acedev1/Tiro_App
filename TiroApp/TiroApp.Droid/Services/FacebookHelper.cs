using System;
using Xamarin.Auth;

[assembly: Xamarin.Forms.Dependency(typeof(TiroApp.Droid.Services.FacebookHelper))]
namespace TiroApp.Droid.Services
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
                redirectUrl: new Uri("http://tiro.flexible-solutions.com.ua/"));

            auth.Completed += (sender, eventArgs) =>
            {
                // We presented the UI, so it's up to us to dimiss it on iOS.
                //DismissViewController(true, null);

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
            var context = Xamarin.Forms.Forms.Context;
            context.StartActivity(auth.GetUI(context));
            //PresentViewController (auth.GetUI (), true, null);
        }
    }
}