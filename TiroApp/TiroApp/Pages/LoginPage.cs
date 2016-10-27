using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Pages;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp
{
    public class LoginPage : ContentPage
    {
        private RelativeLayout main;
        private ContentView bottomView;
        private TabView tabView;
        private CustomEntry emailEntry;
        private CustomEntry pswdEntry;
        private bool _backToPage;

        public LoginPage() : this(false) { }
        public LoginPage(bool backToPage)
        {
            _backToPage = backToPage;
            BuildLayout();
            Utils.SetupPage(this);
        }

        public void BuildLayout()
        {
            main = new RelativeLayout();
            this.Content = main;

            var imageTop = new Image();
            imageTop.Source = ImageSource.FromResource("TiroApp.Images.w1.png");
            imageTop.Aspect = Aspect.AspectFill;
            main.Children.Add(imageTop, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((p) => { return p.Width; }),
                Constraint.RelativeToParent((p) => { return p.Width * 1.2; }));

            bottomView = new ContentView();
            main.Children.Add(bottomView, Constraint.Constant(0),
                Constraint.RelativeToParent((p) =>
                {
                    return p.Height - bottomView.HeightRequest;
                }), Constraint.RelativeToParent((p) => { return p.Width; }));

            tabView = new TabView(new List<string> { "SING UP", "LOG IN" }, Color.FromHex("CF7090"));
            tabView.OnIndexChange += OnTabChange;
            main.Children.Add(tabView, Constraint.Constant(0),
                Constraint.RelativeToParent((p) =>
                {
                    return p.Height - bottomView.HeightRequest - tabView.HeightRequest;
                }),
                Constraint.RelativeToParent((p) => { return p.Width; }),
                Constraint.Constant(tabView.HeightRequest));

            BuildSignUp();

            main.ForceLayout();
        }

        private void OnTabChange(object sender, int e)
        {
            if (tabView.SelectedIndex == 0)
            {
                BuildSignUp();
            }
            else
            {
                BuildLogin();
            }
            main.ForceLayout();
        }

        private void BuildSignUp()
        {
            var bottom = new StackLayout();
            bottom.Orientation = StackOrientation.Vertical;
            bottom.Spacing = 0;

            var label1 = new CustomLabel();
            label1.Text = "Sign up or log in to book your appointment";
            label1.TextColor = Color.Black;
            label1.BackgroundColor = Color.White;
            label1.HorizontalTextAlignment = TextAlignment.Center;
            label1.VerticalTextAlignment = TextAlignment.Center;
            label1.HeightRequest = 70;
            label1.FontSize = 14;
            label1.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            bottom.Children.Add(label1);

            var label2 = new CustomLabel();
            label2.Text = "By signing up, you agree to the Terms of Service and \n\r Privacy Policy";
            label2.TextColor = Color.FromHex("8C8C8C");
            label2.BackgroundColor = Color.White;
            label2.HorizontalTextAlignment = TextAlignment.Center;
            label2.VerticalTextAlignment = TextAlignment.Center;
            label2.HeightRequest = 70;
            label2.FontSize = 12;
            label2.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            bottom.Children.Add(label2);
            
            var signupFBButton = UIUtils.MakeButton("SIGN UP WITH FACEBOOK", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            signupFBButton.BackgroundColor = Color.FromHex("5E73AB");
            bottom.Children.Add(signupFBButton);
            signupFBButton.Clicked += OnSignupFB;
            
            var signupMailButton = UIUtils.MakeButton("SIGN UP WITH MAIL", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            bottom.Children.Add(signupMailButton);
            signupMailButton.Clicked += OnSignupMail;

            bottomView.Content = bottom;
            bottomView.HeightRequest = Utils.GetControlSize(bottom).Height;            
        }

        private void BuildLogin()
        {
            var bottom = new StackLayout();
            bottom.Orientation = StackOrientation.Vertical;
            bottom.Spacing = 0;
            bottom.BackgroundColor = Color.White;

            emailEntry = UIUtils.MakeEntry("Email", UIUtils.FONT_SFUIDISPLAY_BOLD);
            bottom.Children.Add(emailEntry);
            bottom.Children.Add(UIUtils.MakeSeparator());

            pswdEntry = UIUtils.MakeEntry("Password", UIUtils.FONT_SFUIDISPLAY_BOLD);
            pswdEntry.IsPassword = true;
            bottom.Children.Add(pswdEntry);
            bottom.Children.Add(UIUtils.MakeSeparator());

            var loginButton = UIUtils.MakeButton("LOG IN", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            loginButton.Margin = new Thickness(0, 40, 0, 0);
            bottom.Children.Add(loginButton);
            loginButton.Clicked += (o, a) =>
            {
                var email = emailEntry.Text;
                var password = pswdEntry.Text;
                DoLogin(email, password);
            };

            bottomView.Content = bottom;
            bottomView.HeightRequest = Utils.GetControlSize(bottom).Height;
        }

        private void OnSignupMail(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new SignUpEmailPage());
        }

        private void OnSignupFB(object sender, EventArgs e)
        {
            var spinner = UIUtils.ShowSpinner(this);
            DependencyService.Get<IFacebookHelper>().Start((resp) =>
            {
                if (resp.Code == ResponseCode.OK)
                {
                    var obj = JObject.Parse(resp.Result);
                    var sendData = new Dictionary<string, object>()
                    {
                        { "Email", obj["email"].ToString() },
                        { "FbId", obj["id"].ToString() },
                        { "FirstName", obj["first_name"].ToString() },
                        { "LastName", obj["last_name"].ToString() },
                        { "Password" , Ext.MD5.GetMd5String(obj["id"].ToString()) },
                        { "Phone", "" }
                    };                    
                    DataGate.CustomerSignupJson(sendData, (singupResp) =>
                    {
                        if (singupResp.Code == ResponseCode.OK)
                        {
                            var jobj = JObject.Parse(singupResp.Result);
                            if (jobj["Id"] == null || string.IsNullOrEmpty((string)jobj["Id"]))
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DoLogin(obj["email"].ToString(), obj["id"].ToString());
                                });
                            }
                            else
                            {
                                OnLoginOk(jobj);
                            }
                        }
                        else
                        {
                           
                        }
                        UIUtils.HideSpinner(this, spinner);
                    });
                }
                else
                {
                    UIUtils.ShowMessage("Login failed. Try later", this);
                    UIUtils.HideSpinner(this, spinner);
                }
            });
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
                    if (_backToPage)
                    {
                        this.Navigation.PopAsync();
                        //Utils.ShowPageFirstInStack(this, new OrderSummary());
                    }
                    else
                    {
                        Utils.ShowPageFirstInStack(this, new HomePage());
                    }
                });
            }
            else
            {
                UIUtils.ShowMessage("Login failed. Wrong email or password", this);
            }
        }
    }
}
