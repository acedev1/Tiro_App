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
    public class Splash2Page : ContentPage
    {
        private Label mainText;
        private Image background;
        private AbsoluteLayout mainLayout;
        private BoxView blackout;
        private StackLayout mainTextHolder;
        private StackLayout additionalTextHolder;

        public Splash2Page()
        {
            Utils.SetupPage(this);
            Init();
            BuildLayout();
        }

        private void Init()
        {
            mainText = new CustomLabel();
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

            mainTextHolder.Children.Add(mainText);
            BuildAdditionalTextHolder();

            mainLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            mainLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

            background.Source = ImageSource.FromResource("TiroApp.Images.launch.jpg");
            background.Aspect = Aspect.AspectFill;

            AbsoluteLayout.SetLayoutFlags(background, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(background, new Rectangle(0f, 0f, 1f, 1f));
            mainLayout.Children.Add(background);

            AbsoluteLayout.SetLayoutFlags(mainTextHolder, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(mainTextHolder, new Rectangle(0.2f, 0.7f, App.ScreenWidth, AbsoluteLayout.AutoSize));
            mainLayout.Children.Add(mainTextHolder);

            AbsoluteLayout.SetLayoutFlags(additionalTextHolder, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(additionalTextHolder, new Rectangle(0f, 0.9f, App.ScreenWidth, AbsoluteLayout.AutoSize));
            mainLayout.Children.Add(additionalTextHolder);                       

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
