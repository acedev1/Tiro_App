using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Model
{
    public class InstagramHelper
    {
        const string CLIENT_ID = "e3ec17aebc6a4cd69ba012277ca2b88e";// "742adcaf27074f9996b3848373f1e09f";//
        public const string REDIRECT_URI = "http://tiro.flexible-solutions.com.ua";

        private string username;
        private Page pageCurrent;

        private string userid;
        private string access_token;

        public event EventHandler<string> OnImagesLoad;

        public InstagramHelper(string username, Page p)
        {
            this.username = username;
            this.pageCurrent = p;
        }

        public void Start()
        {
            string url = $"https://www.instagram.com/{username}/?__a=1";
            System.Net.HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(url);
            request.BeginGetResponse((iar) =>
            {
                try
                {
                    System.Net.WebResponse response = ((System.Net.HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                    var responseStream = response.GetResponseStream();
                    var data = (JObject)JObject.ReadFrom(new JsonTextReader(new StreamReader(responseStream)));
                    responseStream.Dispose();
                    userid = (string)data["user"]["id"];
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        StartBrowser();
                    });
                }
                catch
                {
                    InvokeFail();
                }
            }, request);
        }

        private void StartBrowser()
        {   
            var auth = $"https://api.instagram.com/oauth/authorize/?client_id={CLIENT_ID}&redirect_uri={REDIRECT_URI}&response_type=token&scope=basic+public_content";
            pageCurrent.Navigation.PushAsync(new WebViewPage(auth, r =>
            {
                if (!string.IsNullOrEmpty(r))
                {
                    access_token = r;
                    LoadImages();
                }
                else
                {
                    InvokeFail();
                }
            }));
        }

        private void LoadImages()
        {
            string url = $"https://api.instagram.com/v1/users/{userid}/media/recent/?access_token={access_token}&COUNT=10";
            System.Net.HttpWebRequest request = System.Net.HttpWebRequest.CreateHttp(url);
            request.BeginGetResponse((iar) =>
            {
                try
                {
                    System.Net.WebResponse response = ((System.Net.HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                    var responseStream = response.GetResponseStream();
                    var data = (JObject)JObject.ReadFrom(new JsonTextReader(new StreamReader(responseStream)));
                    responseStream.Dispose();
                    var sb = new StringBuilder();
                    foreach (var item in (JArray)data["data"])
                    {
                        try
                        {
                            var countOfTag = item["tags"].Where(t => ((string)t) == "tiro" || ((string)t) == "tirobeauty").Count();
                            if (countOfTag != 0)
                            {
                                var img = (string)item["images"]["standard_resolution"]["url"];
                                sb.Append(img);
                                sb.Append(",");
                            }
                        }
                        catch { }
                    }
                    sb.Length--;
                    OnImagesLoad?.Invoke(this, sb.ToString());
                }
                catch
                {
                    InvokeFail();
                }
            }, request);
        }

        private void InvokeFail()
        {
            OnImagesLoad?.Invoke(this, null);
        }
    }

    public class WebViewPage : ContentPage
    {
        private WebView webview;
        private Action<string> callback;

        public WebViewPage(string url, Action<string> callback)
        {
            Utils.SetupPage(this);
            this.callback = callback;
            webview = new WebView();
            webview.Navigating += Webview_Navigating;
            webview.Source = url;
            var layout = new AbsoluteLayout();
            layout.Children.Add(webview, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
            this.Content = layout;
        }

        private void Webview_Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.StartsWith(InstagramHelper.REDIRECT_URI))
            {
                e.Cancel = true;
                int codeIndex = e.Url.IndexOf("access_token=");
                if (codeIndex > 0)
                {
                    var code = e.Url.Substring(codeIndex + "access_token=".Length);
                    callback(code);
                }
                else
                {
                    callback(null);
                }
                this.Navigation.PopAsync();
            }
        }
    }
}
