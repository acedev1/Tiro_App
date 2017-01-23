using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public class MuaLoginPage : ContentPage
    {
        private RelativeLayout main;
        private ContentView bottomView;
        private TabView tabView;
        private CustomEntry emailEntry;
        private CustomEntry pswdEntry;
        private CustomEntry businessEntry;

        private ActivityIndicator spinner;

        public MuaLoginPage()
        {
            BuildLayout();
            Utils.SetupPage(this);
        }

        public void BuildLayout()
        {
            main = new RelativeLayout();
            this.Content = main;

            var imageTop = new Image();
            //imageTop.Source = ImageSource.FromResource("TiroApp.Images.w1.png");
            imageTop.Source = ImageSource.FromResource("TiroApp.Images.login_bg.jpg");
            imageTop.Aspect = Aspect.AspectFill;
            main.Children.Add(imageTop, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((p) => { return p.Width; }),
                Constraint.RelativeToParent((p) => { return p.Width * 1.2; }));

            var bgView = new ContentView();
            bgView.BackgroundColor = Props.BlackoutColor;
            main.Children.Add(bgView, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));

            var label1 = new CustomLabel()
            {
                TextColor = Color.White,
                FontSize = 28,
                FontFamily = UIUtils.FONT_BEBAS_REGULAR,
                Text = "WELCOME TO TIRO!"
            };
            var label21 = new CustomLabel()
            {
                TextColor = Color.White,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "You’re on your way to growing your business."
            };
            var label22 = new CustomLabel()
            {
                TextColor = Color.White,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Discover over 1 million clients who use"
            };
            var label23 = new CustomLabel()
            {
                TextColor = Color.White,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Tiro every day."
            };
            var label3 = new CustomLabel()
            {
                TextColor = Props.ButtonColor,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                Text = "Questions About Tiro?"
            };

            var signupAsClient = new CustomLabel()
            {
                Text = "Sign Up as a Client",
                TextColor = Props.ButtonColor,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                FontSize = 16
            };
            signupAsClient.GestureRecognizers.Add(new TapGestureRecognizer(v =>
            {
                Utils.ShowPageFirstInStack(this, new LoginPage());
            }));

            main.Children.Add(label1,
                Constraint.RelativeToParent(p => { return (p.Width - label1.Width) / 2; }),
                Constraint.Constant(112));
            main.Children.Add(label21,
               Constraint.RelativeToParent(p => { return (p.Width - label21.Width) / 2; }),
               Constraint.Constant(170));
            main.Children.Add(label22,
               Constraint.RelativeToParent(p => { return (p.Width - label22.Width) / 2; }),
               Constraint.Constant(195));
            main.Children.Add(label23,
               Constraint.RelativeToParent(p => { return (p.Width - label23.Width) / 2; }),
               Constraint.Constant(220));
            main.Children.Add(label3,
               Constraint.RelativeToParent(p => { return (p.Width - label3.Width) / 2; }),
               Constraint.Constant(262));
            main.Children.Add(signupAsClient,
                Constraint.RelativeToParent(p => { return p.Width - signupAsClient.Width - 20; }),
                Constraint.Constant(20));

            bottomView = new ContentView();
            main.Children.Add(bottomView, Constraint.Constant(0),
                Constraint.RelativeToParent((p) =>
                {
                    return p.Height - bottomView.HeightRequest;
                }), Constraint.RelativeToParent((p) => { return p.Width; }));

            tabView = new TabView(new List<string> { "SIGN UP", "LOG IN" }, Color.FromHex("CF7090"));
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
            bottom.BackgroundColor = Color.White;

            businessEntry = UIUtils.MakeEntry("Business Name or Alias", UIUtils.FONT_SFUIDISPLAY_BOLD);
            bottom.Children.Add(businessEntry);
            bottom.Children.Add(UIUtils.MakeSeparator());

            var label2 = new CustomLabel();
            label2.Text = "By signing up, you agree to the Terms of Service and \n\r Privacy Policy";
            label2.TextColor = Color.FromHex("8C8C8C");
            label2.BackgroundColor = Color.White;
            label2.HorizontalTextAlignment = TextAlignment.Center;
            label2.VerticalTextAlignment = TextAlignment.Center;
            label2.HeightRequest = 75;
            label2.FontSize = 12;
            label2.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            bottom.Children.Add(label2);
            
            var signupMailButton = UIUtils.MakeButton("JOIN", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
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
            loginButton.Clicked += OnLogin;

            bottomView.Content = bottom;
            bottomView.HeightRequest = Utils.GetControlSize(bottom).Height;
        }

        private void OnSignupMail(object sender, EventArgs e)
        {
            bool valid = UIUtils.ValidateEntriesWithEmpty(new Entry[] { businessEntry }, this);
            if (valid)
            {
                this.Navigation.PushAsync(new MuaSignupPage(businessEntry.Text));
            }
        }

        private void OnLogin(object sender, EventArgs e)
        {
            bool valid = UIUtils.ValidateEntriesWithEmpty(new Entry[] { emailEntry, pswdEntry }, this);
            if (valid)
            {
                var email = emailEntry.Text;
                var password = pswdEntry.Text;
                spinner = UIUtils.ShowSpinner(this);
                DataGate.MuaLoginJson(email, Ext.MD5.GetMd5String(password), (data) =>
                {
                    if (data.Code == ResponseCode.OK)
                    {
                        var jobj = JObject.Parse(data.Result);
                        var muaId = jobj["Id"] != null ? (string)jobj["Id"] : null;                        
                        if (!string.IsNullOrEmpty(muaId))
                        {
                            if ((bool)jobj["IsConfirmed"])
                            {
                                GlobalStorage.Settings.MuaId = muaId;
                                GlobalStorage.SaveAppSettings();
                                Notification.CrossPushNotificationListener.RegisterPushNotification();
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Utils.ShowPageFirstInStack(this, new MuaHomePage());
                                });
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Utils.ShowPageFirstInStack(this, new UnderReviewPage());
                                });
                            }
                        }
                        else
                        {
                            UIUtils.ShowMessage("Login failed. Wrong email or password", this);
                        }
                    }
                    else
                    {
                        UIUtils.ShowMessage("Login failed. Try later", this);
                    }
                    UIUtils.HideSpinner(this, spinner);
                });
            }
        }
    }
}
