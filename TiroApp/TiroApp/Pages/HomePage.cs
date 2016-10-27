using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages
{
    public class HomePage : BasePage
    {
        private StackLayout homeHeader;
        private SearchHeader searchHeader;
        private RelativeLayout searchContainer;

        public HomePage()
        {
            Utils.SetupPage(this);
            this.BackgroundColor = Color.White;
            BuildLayout();
            AddSideMenu();
        }

        private void BuildLayout()
        {
            homeHeader = BuildHomeHeader();
            mainLayout.Children.Add(homeHeader, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent(p => p.Width));

            var cLayout = new StackLayout();
            cLayout.Orientation = StackOrientation.Vertical;
            cLayout.Children.Add(BuildImageBlock("TiroApp.Images.homew1.jpg", "GET YOUR FIRSTS FREE MAKEOVER NOW!", "near you", (v) =>
            {
                Navigation.PushAsync(new SearchPage(true));
            }));
            cLayout.Children.Add(BuildImageBlock("TiroApp.Images.homew2.jpg", "DISCOVER IMAZING MAKEUP ARTIST", "near you", (v) => { }));
            cLayout.Children.Add(BuildImageBlock("TiroApp.Images.homew3.jpg", "BEST LOOKS", "featured artists", (v) => { }));

            var scrollView = new ScrollView();
            scrollView.Content = cLayout;
            mainLayout.Children.Add(scrollView,
                Constraint.Constant(0),
                Constraint.Constant(homeHeader.HeightRequest),
                Constraint.RelativeToParent(p => p.Width),
                Constraint.RelativeToParent(p => p.Height - homeHeader.HeightRequest));

            BuildSearchHeader();
        }

        private StackLayout BuildHomeHeader()
        {
            var headerLabel = new CustomLabel();
            headerLabel.Text = "Find + Book";
            headerLabel.TextColor = Color.Black;
            headerLabel.BackgroundColor = Color.White;
            headerLabel.VerticalTextAlignment = TextAlignment.Center;
            headerLabel.HorizontalTextAlignment = TextAlignment.Center;
            headerLabel.HeightRequest = 50;
            headerLabel.FontSize = 17;
            headerLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;

            var separator = UIUtils.MakeSeparator(true);

            var searchHeight = 40;
            var searchLabel = new CustomLabel();
            searchLabel.Text = "Services, salon or stylist";
            searchLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            searchLabel.FontSize = 14;
            searchLabel.TextColor = Color.FromHex("D8D8D8");
            searchLabel.HorizontalTextAlignment = TextAlignment.Center;
            searchLabel.VerticalTextAlignment = TextAlignment.Center;
            searchLabel.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
            {
                searchContainer.IsVisible = true;
            }));
            searchLabel.HeightRequest = searchHeight;
            var sBg = new Image();
            sBg.Source = ImageSource.FromResource("TiroApp.Images.searchBackground2.png");
            sBg.HeightRequest = searchHeight;
            var searchLabelLayout = new RelativeLayout();
            searchLabelLayout.HorizontalOptions = LayoutOptions.Center;
            searchLabelLayout.WidthRequest = 300;
            searchLabelLayout.HeightRequest = searchHeight;
            searchLabelLayout.Children.Add(sBg, Constraint.Constant(0), Constraint.Constant(0));
            searchLabelLayout.Children.Add(searchLabel, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent(p => p.Width));

            var homeHeader = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                HeightRequest = headerLabel.HeightRequest + separator.HeightRequest + searchLabelLayout.HeightRequest,
                Children = { headerLabel, separator, searchLabelLayout }
            };
            return homeHeader;
        }

        private void BuildSearchHeader()
        {
            searchHeader = new SearchHeader();
            searchHeader.BackgroundColor = Color.White;
            searchHeader.filterIcon.IsVisible = false;
            searchHeader.filtersLayout.IsVisible = true;
            searchHeader.OnSearchClick += SearchButton_Clicked;

            searchContainer = new RelativeLayout();
            searchContainer.BackgroundColor = Props.BlackoutColor;
            searchContainer.GestureRecognizers.Add(new TapGestureRecognizer((v) => { searchContainer.IsVisible = false; }));
            searchContainer.Children.Add(searchHeader,
                Constraint.Constant(0), Constraint.Constant(50));
            mainLayout.Children.Add(searchContainer, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
            searchContainer.IsVisible = false;
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            searchContainer.IsVisible = false;
            var listCheck = searchHeader.filters.Select(f => f.IsChecked).ToList();
            var searchPage = new SearchPage(listCheck);
            this.Navigation.PushAsync(searchPage);
        }

        private View BuildImageBlock(string image, string title, string title2, Action<View> tapAction)
        {
            var rl = new RelativeLayout();
            rl.HorizontalOptions = LayoutOptions.Fill;
            rl.HeightRequest = Device.OnPlatform(238, 270, 250);

            var img = new Image();
            img.Source = ImageSource.FromResource(image);
            img.Aspect = Aspect.AspectFill;
            rl.Children.Add(img, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));

            var locLabel = new CustomLabel();
            locLabel.Text = title2;
            locLabel.TextColor = Color.White;
            locLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            locLabel.FontSize = 16;

            var tLabel = new CustomLabel();
            tLabel.Text = title;
            tLabel.TextColor = Color.White;
            tLabel.FontFamily = UIUtils.FONT_BEBAS_BOOK;
            tLabel.FontSize = 28;
            tLabel.WidthRequest = 200;

            rl.Children.Add(locLabel, Constraint.Constant(15), Constraint.RelativeToParent(p =>
            {
                var h = locLabel.Height != -1 ? locLabel.Height : Utils.GetControlSize(locLabel).Height;
                return p.Height - h - 12;
            }));

            rl.Children.Add(tLabel, Constraint.Constant(15), Constraint.RelativeToParent(p =>
            {
                var hLoc = Utils.GetControlSize(locLabel).Height;
                var h = tLabel.Height != -1 ? tLabel.Height : Utils.GetControlSize(tLabel).Height;
                return p.Height - hLoc - h - 12;
            }));

            rl.GestureRecognizers.Add(new TapGestureRecognizer(tapAction));

            return rl;
        }
    }
}
