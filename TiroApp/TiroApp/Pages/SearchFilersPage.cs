using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Platform.Services.Geolocation;

namespace TiroApp.Pages
{
    public class SearchFilersPage : ContentPage
    {
        private CustomEntry locationEntry;
        private CustomLabel timeLabel;
        private int availabilityMode = 1;
        private IGeolocator locator;

        private const double RADIUS = 5000;

        public event EventHandler OnSelect;

        public SearchFilersPage()
        {
            Utils.SetupPage(this);
            BuildLayout();

            locator = Xamarin.Forms.DependencyService.Get<IGeolocator>();
            locator.PositionChanged += (o, arg) =>
            {
                locator.StopListening();
                if (arg.Position != null)
                {
                    LocationFilter = new LocationFilter()
                    {
                        Lat = arg.Position.Latitude,
                        Lon = arg.Position.Longitude,
                        Radius = RADIUS
                    };
                    LocationAddress = "Current location";
                }
            };
            locator.PositionError += (o, arg) =>
            {
                locator.StopListening();
            };
            locator.StopListening();
            locator.StartListening(0, 0);
        }

        private void BuildLayout()
        {
            var main = new StackLayout();
            main.Spacing = 0;
            main.BackgroundColor = Color.White;
            var rl = new RelativeLayout();
            rl.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
            Content = rl;

            BuildHeader(main, "Where + When");
            BuildLocationLayout(main);
            BuildTimeLayout(main);

            var btn = UIUtils.MakeButton("CONFIRM", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            btn.VerticalOptions = LayoutOptions.EndAndExpand;
            btn.Clicked += (o, a) =>
            {
                this.Navigation.PopAsync();
                OnSelect?.Invoke(this, EventArgs.Empty);
            };
            main.Children.Add(btn);
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
                OnSelect?.Invoke(this, EventArgs.Empty);
            }));
            var headerLabel = new CustomLabel();
            headerLabel.Text = text;
            headerLabel.TextColor = Color.Black;
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

            var separator = UIUtils.MakeSeparator(true);
            separator.VerticalOptions = LayoutOptions.Start;
            main.Children.Add(header);
            main.Children.Add(separator);
        }

        private void BuildLocationLayout(StackLayout main)
        {
            var calendar = new Image();
            calendar.Source = ImageSource.FromResource("TiroApp.Images.calendar.png");
            calendar.HeightRequest = 20;
            calendar.WidthRequest = 22;
            var placeBackground = new Image();
            placeBackground.Source = ImageSource.FromResource("TiroApp.Images.searchBackground.png");
            placeBackground.Aspect = Aspect.Fill;
            placeBackground.HeightRequest = 44;

            var locationACEntry = new AutoCompleteView();
            locationEntry = locationACEntry.EntryText;
            locationEntry.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            locationEntry.WidthRequest = 250;
            locationEntry.HeightRequest = 38;
            locationEntry.FontSize = 16;
            locationEntry.BackgroundColor = Color.White;
            locationEntry.Placeholder = "Use current location";
            locationEntry.PlaceholderColor = Color.Black;
            locationEntry.TextColor = Color.Black;
            locationEntry.Focused += (o, a) => { locationEntry.Text = ""; };
            var sh = new PlaceSearchHelper(locationACEntry);
            sh.OnSelected += (o, p) =>
            {
                LocationFilter = new LocationFilter()
                {
                    Lat = p.Latitude,
                    Lon = p.Longitude,
                    Radius = RADIUS
                };
                LocationAddress = locationEntry.Text;
            };

            var rl = new RelativeLayout();
            rl.Margin = new Thickness(0, 10, 0, 10);
            rl.WidthRequest = Device.OnPlatform(300, 320, 300);
            rl.VerticalOptions = LayoutOptions.Start;
            rl.HorizontalOptions = LayoutOptions.Center;
            rl.Children.Add(placeBackground, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.Constant(placeBackground.HeightRequest));
            rl.Children.Add(calendar, Constraint.Constant(10), Constraint.Constant(10));
            rl.Children.Add(locationACEntry, Constraint.Constant(40), Constraint.Constant(4));
            //heightConstraint: Constraint.Constant(locationEntry.HeightRequest));
            
            var useCurrLocation = new CustomLabel()
            {
                HorizontalOptions = LayoutOptions.Center,
                Text = "Use Current Location",
                TextColor = Props.ButtonColor,
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 17,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT,
            };
            useCurrLocation.GestureRecognizers.Add(new TapGestureRecognizer(LocationClicked));

            main.Children.Add(useCurrLocation);
            main.Children.Add(rl);
            var separator = UIUtils.MakeSeparator(true);
            separator.VerticalOptions = LayoutOptions.Start;
            main.Children.Add(separator);
        }

        private void BuildTimeLayout(StackLayout main)
        {
            timeLabel = new CustomLabel()
            {
                HorizontalOptions = LayoutOptions.Start,
                Text = "Available Anytime",
                TextColor = Color.Black,
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 17,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT
            };
            var img = new Image()
            {
                HorizontalOptions = LayoutOptions.End,
                Source = ImageSource.FromResource("TiroApp.Images.ArrowDown.png"),
                VerticalOptions = LayoutOptions.Center,
                HeightRequest = 20,
                WidthRequest = 20
            };
            var l = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = 50,
                Margin = new Thickness(20, 10, 20, 10),
                Children = { timeLabel, img }
            };
            main.Children.Add(l);

            var separator = UIUtils.MakeSeparator(true);
            separator.VerticalOptions = LayoutOptions.Start;
            main.Children.Add(separator);

            var rb1 = BuildRadioButton("Available Anytime", true);
            var rb2 = BuildRadioButton("Available today", false);
            var rb3 = BuildRadioButton("Select Date", false);            
            main.Children.Add(rb1);
            main.Children.Add(rb2);
            main.Children.Add(rb3);
            l.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
            {
                rb1.IsVisible = !rb1.IsVisible;
                rb2.IsVisible = !rb2.IsVisible;
                rb3.IsVisible = !rb3.IsVisible;
            }));
            rb1.OnCheckedChange += (s, a) =>
            {
                rb2.IsChecked = rb3.IsChecked = false;
                OnAvailabilityChange(1);
            };
            rb2.OnCheckedChange += (s, a) =>
            {
                rb1.IsChecked = rb3.IsChecked = false;
                OnAvailabilityChange(2);
            };
            rb3.OnCheckedChange += (s, a) =>
            {
                rb1.IsChecked = rb2.IsChecked = false;
                OnAvailabilityChange(3);
            };
        }

        private RadioButton BuildRadioButton(string text, bool isChecked)
        {
            var rb1 = new RadioButton();
            rb1.Text = text;
            rb1.IsVisible = false;
            rb1.Margin = new Thickness(20, 5, 0, 0);
            rb1.VerticalOptions = LayoutOptions.Start;
            rb1.TextColor = Color.Black;
            rb1.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            rb1.FontSize = 17;
            rb1.HeightRequest = 30;
            rb1.IsChecked = isChecked;
            return rb1;
        }

        private void OnAvailabilityChange(int mode)
        {
            availabilityMode = mode;
            switch (availabilityMode)
            {
                case 1:
                    timeLabel.Text = "Available Anytime";
                    AvailibilityFilter = null;
                    break;
                case 2:
                    timeLabel.Text = "Available today";
                    AvailibilityFilter = new Availibility() {
                        Mode = AvailibilityMode.Dates,
                        DatesFrom = new List<DateTime>() { DateTime.Today.Add(new TimeSpan(0, 0, 0)) },
                        DatesTo = new List<DateTime>() { DateTime.Today.Add(new TimeSpan(24, 0, 0)) }
                    };
                    break;
                case 3:
                    var ap = new AvailabilityPage()
                    {
                        Mode = ViewMode.SingleRange,
                        SelectedDate = DateTime.Now,
                        ButtonText = "Select"
                    };
                    ap.OnFinishSelection += (o, s) =>
                    {
                        AvailibilityFilter = ap.Availibility;
                        timeLabel.Text = $"{ap.Availibility.DatesFrom[0].ToString("D", Utils.EnCulture)}\n\rFrom {ap.Availibility.DatesFrom[0].ToString("HH:mm")} To {ap.Availibility.DatesTo[0].ToString("HH:mm")}";
                    };
                    Navigation.PushAsync(ap);
                    break;
            }
        }

        private void LocationClicked(View v)
        {
            locationEntry.Text = "";
            LocationFilter = null;
            locator.StopListening();            
            locator.StartListening(0, 0);
        }

        public int AvailabilityMode
        {
            get { return availabilityMode; }
        }
        public LocationFilter LocationFilter { get; set; }
        public string LocationAddress { get; set; }
        public Availibility AvailibilityFilter { get; set; }
    }

    public class LocationFilter
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Radius { get; set; }
    }
}
