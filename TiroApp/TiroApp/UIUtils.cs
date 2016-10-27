using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp
{
    public class UIUtils
    {
        public static readonly string FONT_BEBAS_REGULAR;
        public static readonly string FONT_BEBAS_BOOK;
        public static readonly string FONT_SFUIDISPLAY_MEDIUM;
        public static readonly string FONT_SFUIDISPLAY_LIGHT;
        public static readonly string FONT_SFUIDISPLAY_REGULAR;
        public static readonly string FONT_SFUIDISPLAY_BOLD;
        public static readonly string FONT_SFUIDISPLAY_SEMIBOLD;
        public static readonly string FONT_SFUIDISPLAY_HEAVY;

        public static readonly BindableProperty TagProperty = BindableProperty.Create("Tag", typeof(object), typeof(View), null);
        public static readonly BindableProperty Tag2Property = BindableProperty.Create("Tag2", typeof(object), typeof(View), null);

        public const string NIARA_SIGN = "\u20A6";

        static UIUtils()
        {
            FONT_BEBAS_REGULAR = Device.OnPlatform("BebasNeueRegular", "BebasNeue_Regular", null);
            FONT_BEBAS_BOOK = Device.OnPlatform("BebasNeueBook", "BebasNeue_Book", null);
            FONT_SFUIDISPLAY_MEDIUM = Device.OnPlatform("SFUIDisplay-Medium", "SF-UI-Display-Medium", null);
            FONT_SFUIDISPLAY_LIGHT = Device.OnPlatform("SFUIDisplay-Light", "SF-UI-Display-Light", null);
            FONT_SFUIDISPLAY_REGULAR = Device.OnPlatform("SFUIDisplay-Regular", "SF-UI-Display-Regular", null);
            FONT_SFUIDISPLAY_BOLD = Device.OnPlatform("SFUIDisplay-Bold", "SF-UI-Display-Bold", null);
            FONT_SFUIDISPLAY_SEMIBOLD = Device.OnPlatform("SFUIDisplay-Semibold", "SF-UI-Display-Semibold", null);
            FONT_SFUIDISPLAY_HEAVY = Device.OnPlatform("SFUIDisplay-Heavy", "SF-UI-Display-Heavy", null);
        }

        public static Button MakeButton(string text, string font, bool isInfoPage = false)
        {
            var btn = new Button();
            btn.Text = text;
            btn.FontFamily = font;
            btn.HeightRequest = 65;
            btn.FontSize = 17;
            btn.HorizontalOptions = LayoutOptions.FillAndExpand;
            btn.BackgroundColor = isInfoPage ? Props.ButtonInfoPageColor : Props.ButtonColor;
            btn.BorderRadius = Props.ButtonBorderRadius;
            btn.TextColor = Color.White;
            return btn;
        }

        public static CustomEntry MakeEntry(string text, string font)
        {
            var entry = new CustomEntry();
            entry.FontFamily = font;
            entry.HeightRequest = 45;
            entry.FontSize = 17;
            entry.BackgroundColor = Color.White;
            entry.Margin = new Thickness(20, 10, 20, 0);
            entry.Placeholder = text;
            entry.PlaceholderColor = Color.FromHex("787878");
            entry.TextColor = Color.Black;
            //entry.VerticalOptions = LayoutOptions.Center;
            return entry;
        }

        public static BoxView MakeSeparator(bool fitWidth = false)
        {
            var b = new BoxView()
            {
                Color = Color.FromHex("DADADA"),
                HeightRequest = 1,
                Opacity = 0.5,
                Margin = new Thickness(20, 0, 20, 0)
            };
            if (fitWidth)
            {
                b.Margin = new Thickness(0, 0, 0, 0);
            }
            return b;
        }

        public static RelativeLayout MakeHeader(Page page, string text, bool isAlert = false)
        {
            var imageArrowBack = new Image();
            imageArrowBack.Source = ImageSource.FromResource("TiroApp.Images.ArrowBackBlack.png");
            imageArrowBack.HeightRequest = 20;
            imageArrowBack.Margin = new Thickness(10, 0, 0, 0);
            imageArrowBack.VerticalOptions = LayoutOptions.Center;
            imageArrowBack.HorizontalOptions = LayoutOptions.Start;
            imageArrowBack.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                page.Navigation.PopAsync();
            }));
            var headerLabel = new CustomLabel();
            headerLabel.Text = text;
            headerLabel.TextColor = isAlert ? Color.FromHex("FF001F") : Color.Black;
            headerLabel.BackgroundColor = Color.White;
            headerLabel.HorizontalTextAlignment = TextAlignment.Center;
            headerLabel.VerticalTextAlignment = TextAlignment.Center;
            headerLabel.HeightRequest = 50;
            headerLabel.FontSize = 17;
            headerLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
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
            return header;
        }

        public static bool ValidateEntriesWithEmpty(IEnumerable<Entry> entries, Page p)
        {
            foreach (var v in entries)
            {
                if (string.IsNullOrEmpty(v.Text))
                {
                    ShowMessage("Fill all required fields", p);
                    return false;
                }
            }
            return true;
        }

        public static void ShowMessage(string message, Page p)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                p.DisplayAlert("Tiro", message, "OK");
            });
        }

        public static async Task<int> ShowSelectList(string value1, string value2, Page p)
        {
            var actionPopupResult = await p.DisplayActionSheet("Tiro", "Cancel", null, value1, value2);            
            if (actionPopupResult == value1)
            {
                return 1;
            }
            if (actionPopupResult == value2)
            {
                return 2;
            }
            return -1;
        }

        public static void ShowServerUnavailable(Page p)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                p.DisplayAlert("Tiro", "Server is unabailable. Please, try later.", "OK");
            });
        }

        public static ActivityIndicator ShowSpinner(ContentPage page)
        {
            var spinner = new ActivityIndicator();
            var rl = new RelativeLayout();
            rl.BackgroundColor = Props.BlackoutColor;
            rl.Children.Add(spinner,
                   Constraint.RelativeToParent(p => ((p.Width - Utils.GetControlSize(spinner).Width) / 2)),
                   Constraint.RelativeToParent(p => ((p.Height - Utils.GetControlSize(spinner).Height) / 2)));
            if (page.Content is RelativeLayout)
            {
                ((RelativeLayout)page.Content).Children.Add(rl, Constraint.Constant(0), Constraint.Constant(0),
                    Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
            }
            else if (page.Content is AbsoluteLayout)
            {
                ((AbsoluteLayout)page.Content).Children.Add(rl,
                    new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.SizeProportional);
            }
            else
            {
                //??
            }
            spinner.IsRunning = true;
            return spinner;
        }

        public static void HideSpinner(ContentPage page, ActivityIndicator spinner)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ((Layout<View>)page.Content).Children.Remove((View)spinner.Parent);
            });
        }

    }
}
