using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TiroApp.Pages;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages
{
    public class LaunchPage : ContentPage
    {
        private Button startBtn;
        private Label mainText;
        private Image background;
        private AbsoluteLayout mainLayout;
        private BoxView blackout;
        private StackLayout mainTextHolder;
        private StackLayout additionalTextHolder;

        public LaunchPage()
        {
            Utils.SetupPage(this);
            Init();
            BuildLayout();
        }

        private void Init()
        {
            mainText = new CustomLabel();
            startBtn = new Button();
            background = new Image();
            mainLayout = new AbsoluteLayout();
            blackout = new BoxView();
            mainTextHolder = new StackLayout();
            //additionalTextHolder = new StackLayout();
        }

        private void BuildLayout()
        {
            mainText.Text = "BEAUTY AT YOUR\r\nFINGERTIPS";
            mainText.TextColor = Color.White;
            mainText.FontSize = 28;
            mainText.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            mainText.HorizontalTextAlignment = TextAlignment.Center;
            mainText.HorizontalOptions = LayoutOptions.CenterAndExpand;
            
            startBtn = UIUtils.MakeButton("GET YOUR FREE MAKEOVER NOW", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            startBtn.VerticalOptions = LayoutOptions.End;
            startBtn.Clicked += async (s, a) =>
            {
                try
                {
                    var sPage = new SearchPage(true);
                    await Navigation.PushAsync(sPage);
                    Navigation.InsertPageBefore(new HomePage(), sPage);
                    Navigation.RemovePage(Navigation.NavigationStack[0]);
                }
                catch { }
            };

            blackout.Color = Props.BlackoutColor;

            background.Source = ImageSource.FromResource("TiroApp.Images.launch.jpg");
            background.Aspect = Aspect.AspectFill;

            mainTextHolder.Children.Add(mainText);
            BuildAdditionalTextHolder();

            mainLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            mainLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

            AbsoluteLayout.SetLayoutFlags(background, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(background, new Rectangle(0f, 0f, 1f, 1f));
            mainLayout.Children.Add(background);

            //AbsoluteLayout.SetLayoutFlags(blackout, AbsoluteLayoutFlags.All);
            //AbsoluteLayout.SetLayoutBounds(blackout, new Rectangle(0f, 0f, 1f, 1f));
            //mainLayout.Children.Add(blackout);

            AbsoluteLayout.SetLayoutFlags(mainTextHolder, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(mainTextHolder, new Rectangle(0.2f, 0.54f, App.ScreenWidth, AbsoluteLayout.AutoSize));
            mainLayout.Children.Add(mainTextHolder);

            AbsoluteLayout.SetLayoutFlags(additionalTextHolder, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(additionalTextHolder, new Rectangle(0f, 0.64f, App.ScreenWidth, AbsoluteLayout.AutoSize));
            mainLayout.Children.Add(additionalTextHolder);

            AbsoluteLayout.SetLayoutFlags(startBtn, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(startBtn, new Rectangle(0f, 1f, App.ScreenWidth, AbsoluteLayout.AutoSize));
            mainLayout.Children.Add(startBtn);

            var signupMua = new CustomLabel()
            {
                Text = "I'm a Makeup Artist",
                TextColor = Props.ButtonColor,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                FontSize = 16
            };
            signupMua.GestureRecognizers.Add(new TapGestureRecognizer(v =>
            {
                Utils.ShowPageFirstInStack(this, new Mua.MuaLoginPage());
            }));
            var signupMuaWidth = Utils.GetControlSize(signupMua).Width;
            if (signupMuaWidth == -1)
            {
                signupMuaWidth = 150;
            }
            mainLayout.Children.Add(signupMua, new Point(App.ScreenWidth - signupMuaWidth - 20, 20));

            Content = mainLayout;

        }

        private void BuildAdditionalTextHolder()
        {
            additionalTextHolder = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(30, 0, 30, 0),
                Children =
                {
                    new CustomLabel
                    {
                        Text = "Makeover"
                        ,TextColor = Color.White
                        ,FontSize = 12
                        ,HorizontalOptions = LayoutOptions.CenterAndExpand
                        ,VerticalOptions = LayoutOptions.Center
                        ,FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT
                    },
                    new CustomLabel
                    {
                        Text = "\u25CF"
                        ,TextColor = Color.White
                        ,FontSize = 20
                        ,HorizontalOptions = LayoutOptions.CenterAndExpand
                        ,VerticalOptions = LayoutOptions.Center
                    },
                    new CustomLabel
                    {
                        Text = "Bridal"
                        ,TextColor = Color.White
                        ,FontSize = 12
                        ,HorizontalOptions = LayoutOptions.CenterAndExpand
                        ,VerticalOptions = LayoutOptions.Center
                        ,FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT
                    },
                    new CustomLabel
                    {
                        Text = "\u25CF"
                        ,TextColor = Color.White
                        ,FontSize = 20
                        ,HorizontalOptions = LayoutOptions.CenterAndExpand
                        ,VerticalOptions = LayoutOptions.Center
                    },
                    new CustomLabel
                    {
                        Text = "Occasions"
                        ,TextColor = Color.White
                        ,FontSize = 12
                        ,HorizontalOptions = LayoutOptions.CenterAndExpand
                        ,VerticalOptions = LayoutOptions.Center
                        ,FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT
                    },
                }
            };
        }

    }
}
