using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages
{
    public class SignUpEmailPage : ContentPage
    {
        private RelativeLayout main;
        private CustomerSignupView cuLayout;
        private StackLayout mainTextHolder;
        private BoxView blackout;

        public SignUpEmailPage()
        {
            BuildLayout();
            Utils.SetupPage(this);
        }

        public void BuildLayout()
        {
            main = new RelativeLayout();
            blackout = new BoxView();
            blackout.Color = Props.BlackoutColor;
            this.Content = main;

            var mainText = new CustomLabel();
            mainText.Text = "SIGN UP TO BOOK YOUR\r\nAPPOINTMENT";
            mainText.TextColor = Color.White;
            mainText.FontSize = 28;
            mainText.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            mainText.HorizontalTextAlignment = TextAlignment.Center;
            mainText.HorizontalOptions = LayoutOptions.CenterAndExpand;

            mainTextHolder = new StackLayout();
            mainTextHolder.Children.Add(mainText);

            var imageTop = new Image();
            imageTop.Source = ImageSource.FromResource("TiroApp.Images.w1.png");
            imageTop.Aspect = Aspect.AspectFill;
            main.Children.Add(imageTop, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((p) => { return p.Width; }),
                Constraint.RelativeToParent((p) => { return p.Width * 1.2; }));

            main.Children.Add(blackout, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((p) => { return p.Width; }),
                Constraint.RelativeToParent((p) => { return p.Width * 1.2; }));

            var imageArrowBack = new Image();
            imageArrowBack.Source = ImageSource.FromResource("TiroApp.Images.ArrowBack.png");
            imageArrowBack.GestureRecognizers.Add(new TapGestureRecognizer((v) => 
            {
                this.Navigation.PopAsync();
            }));
            main.Children.Add(imageArrowBack
                , Constraint.Constant(10)
                , Constraint.Constant(30)
                , Constraint.Constant(20)
                , Constraint.Constant(20));

            main.Children.Add(mainTextHolder, Constraint.Constant(0),
                Constraint.RelativeToParent((p) => 
                {
                    return p.Height - Utils.GetControlSize(cuLayout).Height - Utils.GetControlSize(mainTextHolder).Height;
                })
                , Constraint.RelativeToParent((p) => { return p.Width; }));

            cuLayout = new CustomerSignupView(this);
            main.Children.Add(cuLayout, Constraint.Constant(0),
                Constraint.RelativeToParent((p) =>
                {
                    return p.Height - Math.Max(Utils.GetControlSize(cuLayout).Height, cuLayout.HeightRequest);
                }), Constraint.RelativeToParent((p) => { return p.Width; }));

            cuLayout.OnFinish += OnFinishSignup;
            //BuildBottomView();

            main.ForceLayout();
        }

        private void OnFinishSignup(object sender, ResponseDataJson e)
        {
            if (e.Code == ResponseCode.OK)
            {
                Utils.ShowPageFirstInStack(this, new HomePage());
            }
        }        
    }
}
