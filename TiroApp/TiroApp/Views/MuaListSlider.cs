using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class MuaListSlider : StackLayout
    {
        private StackLayout slider;
        private Image expressBtn;
        private Image premiumBtn;
        private Image luxBtn;
        private List<Image> modeButtons;
        private List<CustomLabel> modeInfos;
        private List<CustomLabel> modePrices;
        private int selectedIndex;

        public event EventHandler<Tier> OnSlide;

        public MuaListSlider()
        {
            this.Spacing = 0;
            //this.BackgroundColor = Color.FromHex("#f2f2f2");
            this.BackgroundColor = Color.White;
            modeButtons = new List<Image>();
            modeInfos = new List<CustomLabel>();
            modePrices = new List<CustomLabel>();
            //BuildLayout();
        }

        public void BuildLayout()
        {
            var expressInfo = new CustomLabel {
                Text = "Tiro Xpress",
                FontSize = 10,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(25, 0, 0, 0)
            };
            modeInfos.Add(expressInfo);
            var premiumInfo = new CustomLabel {
                Text = "Tiro Premium",
                FontSize = 10,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
            };
            modeInfos.Add(premiumInfo);
            var luxInfo = new CustomLabel {
                Text = "Tiro Lux",
                FontSize = 10,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 20, 0)
            };
            modeInfos.Add(luxInfo);
            var info = new StackLayout {
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                Margin = new Thickness(0, 5, 0, 0),
                Children = { expressInfo, premiumInfo, luxInfo }
            };
            this.Children.Add(info);

            var expressPrice = new CustomLabel {
                Text = "5K - 10K",
                FontSize = 10,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(32, 0, 0, 0)
            };
            modePrices.Add(expressPrice);
            var premiumPrice = new CustomLabel {
                Text = "10K - 25K",
                FontSize = 10,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
            };
            modePrices.Add(premiumPrice);
            var luxPrice = new CustomLabel {
                Text = "25K +",
                FontSize = 10,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 20, 0)
            };
            modePrices.Add(luxPrice);
            var prices = new StackLayout {
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Margin = new Thickness(0, 5, 0, 0),
                Children = { expressPrice, premiumPrice, luxPrice }
            };

            expressBtn = new Image {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                //VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(25, 0, 0, 0),
                HeightRequest = 50,
                WidthRequest = 50
            };
            modeButtons.Add(expressBtn);
            premiumBtn = new Image {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                //VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(0, 0, 0, 0),
                HeightRequest = 50,
                WidthRequest = 50
            };
            modeButtons.Add(premiumBtn);
            luxBtn = new Image {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                //VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 10, 0),
                HeightRequest = 50,
                WidthRequest = 50
            };
            modeButtons.Add(luxBtn);
            for (int i = 0; i < modeButtons.Count; i++)
            {
                var btn = modeButtons[i];
                var index = i;
                btn.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                    selectedIndex = index;
                    ChangeButtons();
                }));
            }
            selectedIndex = 0;
            double panDX = 0;
            var pan = new PanGestureRecognizer();
            pan.PanUpdated += (o, e) =>
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        panDX = 0;
                        break;
                    case GestureStatus.Running:
                        panDX = e.TotalX;
                        break;
                    case GestureStatus.Completed:
                        Slide(panDX > 0) ;
                        break;
                }
            };
            slider = new StackLayout {
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(0, 8, 0, 0),
                Children = { expressBtn, premiumBtn, luxBtn }
            };
            var sliderHolder = new StackLayout {
                Spacing = 0,
                Children = { slider, prices }
            };
            slider.GestureRecognizers.Add(pan);
            var rl = new RelativeLayout();
            var sliderImage = new Image();
            sliderImage.Source = ImageSource.FromResource("TiroApp.Images.Slider.png");
            sliderImage.VerticalOptions = LayoutOptions.CenterAndExpand;
            sliderImage.Aspect = Aspect.Fill;
            rl.Children.Add(sliderImage, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height - 18));
            rl.Children.Add(sliderHolder, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height - 25));
            this.Children.Add(rl);

            ChangeButtons();
        }

        private void ChangeButtons()
        {
            /*
            colors Hex:
            xpress: D06286
            premium: EDD569
            lux: 352E4F

            SRUIDisplayRegular, 22, Hex: AFB6BC
            */
            for (int i = 0; i < modeButtons.Count; i++)
            {
                var btn = modeButtons[i];
                if (i == selectedIndex)
                {
                    btn.Source = ImageSource.FromResource($"TiroApp.Images.SliderBtnChecked_{i}.png");
                    modeInfos[i].TextColor = GetColorForCheckedMode(i);
                    modePrices[i].TextColor = GetColorForCheckedMode(i);
                }
                else
                {
                    btn.Source = ImageSource.FromResource("TiroApp.Images.SliderBtnUnchecked.png");
                    modeInfos[i].TextColor = Color.FromHex("AFB6BC");
                    modePrices[i].TextColor = Color.FromHex("AFB6BC");
                }
            }
            OnSlide?.Invoke(this, (Tier)(selectedIndex + 1));
        }

        private Color GetColorForCheckedMode(int i)
        {
            switch (i)
            {
                case 0:
                    return Color.FromHex("D06286");
                case 1:
                    return Color.FromHex("EDD569");
                case 2:
                    return Color.FromHex("352E4F");
            }
            return Color.FromHex("AFB6BC");
        }

        private void Slide(bool isRight)
        {
            if (isRight)
            {
                selectedIndex++;
                if (selectedIndex == modeButtons.Count)
                {
                    selectedIndex--;
                    return;
                }
            }
            else
            {
                selectedIndex--;
                if (selectedIndex < 0)
                {
                    selectedIndex++;
                    return;
                }
            }
            ChangeButtons();
        }
    }

    public enum Tier
    {
        Express = 1,
        Premium,
        Lux
    }
}
