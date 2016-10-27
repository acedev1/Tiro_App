using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Model
{
    public class PaymentHelper
    {
        public void Start(string appointmentId, Page p, Action<ResponseCode> callback)
        {
            var webPage = new WebViewPage(DataGate.SERVER_URL + "/Payment/Index?id=" + appointmentId, (r) =>
            {
                DataGate.GetAppointmentById(appointmentId, (result) =>
                {
                    var code = ResponseCode.Fail;
                    if (result.Code == ResponseCode.OK)
                    {
                        var obj = JObject.Parse(result.Result);
                        if ((int)obj["Status"] == (int)AppointmentStatus.Paid)
                        {
                            code = ResponseCode.OK;
                        }
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        p.Navigation.PopAsync();
                    });
                    callback?.Invoke(code);
                });
            });
            p.Navigation.PushAsync(webPage);
        }

        public class WebViewPage : ContentPage
        {
            private WebView webview;
            private Action<ResponseCode> callback;

            public WebViewPage(string url, Action<ResponseCode> callback)
            {
                Utils.SetupPage(this);
                this.callback = callback;
                var layout = new StackLayout();
                layout.BackgroundColor = Color.White;
                BuildHeader(layout, "Payment");

                webview = new WebView();
                webview.VerticalOptions = LayoutOptions.FillAndExpand;
                webview.Navigated += Webview_Navigated;
                webview.Source = url;                
                layout.Children.Add(webview);
                this.Content = layout;
            }

            private void Webview_Navigated(object sender, WebNavigatedEventArgs e)
            {
                if (e.Url.ToLower().Contains("payment/confirm"))
                {                    
                    callback.Invoke(ResponseCode.OK);
                }
            }

            private void BuildHeader(StackLayout main, string text)
            {
                var imageArrowBack = new Image();
                imageArrowBack.Source = ImageSource.FromResource("TiroApp.Images.ArrowBackBlack.png");
                imageArrowBack.HeightRequest = 20;
                imageArrowBack.Margin = new Thickness(10, 0, 0, 0);
                imageArrowBack.VerticalOptions = LayoutOptions.Center;
                imageArrowBack.HorizontalOptions = LayoutOptions.Start;
                imageArrowBack.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
                {
                    this.Navigation.PopAsync();
                }));
                var headerLabel = new CustomLabel();
                headerLabel.Text = text;
                headerLabel.TextColor = Color.Black;
                headerLabel.BackgroundColor = Color.White;
                headerLabel.HorizontalTextAlignment = TextAlignment.Center;
                headerLabel.VerticalTextAlignment = TextAlignment.Center;
                headerLabel.HeightRequest = 50;
                headerLabel.FontSize = 17;
                headerLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
                headerLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
                var header = new RelativeLayout();
                header.VerticalOptions = LayoutOptions.Start;
                header.HeightRequest = headerLabel.HeightRequest;
                header.Children.Add(imageArrowBack, Constraint.Constant(0), Constraint.Constant(20));
                header.Children.Add(headerLabel, Constraint.RelativeToParent(p =>
                {
                    var lWidth = headerLabel.Width;
                    if (lWidth == -1)
                    {
                        lWidth = Utils.GetControlSize(headerLabel).Width;
                    }
                    return (p.Width - lWidth) / 2;
                }));

                var separator = UIUtils.MakeSeparator(true);
                separator.VerticalOptions = LayoutOptions.Start;
                main.Children.Add(header);
                main.Children.Add(separator);
            }
        }
    }
}
