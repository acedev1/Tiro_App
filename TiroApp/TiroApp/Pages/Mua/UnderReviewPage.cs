using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public class UnderReviewPage : ContentPage
    {
        public UnderReviewPage()
        {
            Utils.SetupPage(this);
            BuildLayout();
        }

        private void BuildLayout()
        {
            var main = new RelativeLayout();
            this.Content = main;

            var imageTop = new Image();
            imageTop.Source = ImageSource.FromResource("TiroApp.Images.launch.jpg");
            imageTop.Aspect = Aspect.AspectFill;
            main.Children.Add(imageTop, Constraint.Constant(0), Constraint.Constant(0),
               Constraint.RelativeToParent((p) => { return p.Width; }),
               Constraint.RelativeToParent((p) => { return p.Width * 1.2; }));

            var bgView = new ContentView();
            bgView.BackgroundColor = Props.BlackoutColor;
            main.Children.Add(bgView, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));

            var imageArrowBack = new Image();
            imageArrowBack.Source = ImageSource.FromResource("TiroApp.Images.ArrowBack.png");
            imageArrowBack.GestureRecognizers.Add(new TapGestureRecognizer(OnBack));
            main.Children.Add(imageArrowBack
                , Constraint.Constant(10)
                , Constraint.Constant(30)
                , Constraint.Constant(20)
                , Constraint.Constant(20));

            var button = UIUtils.MakeButton("MAKE A NEW APPLICATION", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            button.Clicked += OnBottomButtonClick;
            //main.Children.Add(button, Constraint.Constant(0), Constraint.RelativeToParent(p => p.Height - button.HeightRequest));

            var l21 = new CustomLabel()
            {
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(20),
                Text = "Thanks for applying!"
            };
            var l22 = new CustomLabel()
            {
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(20),
                Text = "Our Tiro Execs are reviewing your application and will get back to you shortly."
            };
            var l23 = new CustomLabel()
            {
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(20),
                Text = "We aim to follow-up between 24 - 48 hours after your application is submitted"
            };

            var bLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.White,
                //HeightRequest = 400,
                Children = { l21, l22, l23, button }
            };
            main.Children.Add(bLayout, Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Height - bLayout.Height),
                Constraint.RelativeToParent(p => p.Width));

            var label1 = new CustomLabel()
            {
                TextColor = Color.White,
                FontSize = 28,
                FontFamily = UIUtils.FONT_BEBAS_REGULAR,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20),
                Text = "SIT TIGHT, YOUR APPLICATION IS UNDER REVIEW"
            };
            main.Children.Add(label1, Constraint.Constant(0),
               Constraint.RelativeToView(bLayout, (p, v) => p.Height - v.Height - label1.Height - 40),
               Constraint.RelativeToParent(p => p.Width));
        }

        private void OnBottomButtonClick(object sender, EventArgs e)
        {
            Utils.ShowPageFirstInStack(this, new MuaLoginPage());
        }

        private void OnBack(View arg1, object arg2)
        {
            Utils.ShowPageFirstInStack(this, new MuaLoginPage());
        }
    }
}
