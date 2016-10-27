using ImageCircle.Forms.Plugin.Abstractions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages
{
    public class MuaPage : ContentPage
    {
        private RelativeLayout mainLayout;
        private TabView tabView;
        private const double ScaleBackImage = 0.7;
        private ContentView mainContent;
        private ContentView orderLayout;
        private ContentView orderInfo;
        private Grid serviceTimeGrid;
        private Dictionary<string, List<Service>> categories;
        private StackLayout servicesHolder;
        private Order order;
        private ActivityIndicator spinner;
        private string bussinesName;
        private MuaArtist mua;
        private Entry reviewEntry;
        private RatingLayout reviewRating;
        private Image muaBackground;
        private int muaBackgroundImageIndex = 0;

        public MuaPage(JObject jObj)
        {
            this.BackgroundColor = Color.White;
            Utils.SetupPage(this);
            mua = new MuaArtist(jObj, true);
            BuildLayout();
        }

        public bool CanSendReview { get; set; } = false;

        private void BuildLayout()
        {
            var imageArrowBack = new Image();
            imageArrowBack.Source = ImageSource.FromResource("TiroApp.Images.ArrowBack.png");
            imageArrowBack.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
            {
                this.Navigation.PopAsync();
            }));

            Uri imgUri = null;
            Uri.TryCreate(mua.Images[muaBackgroundImageIndex], UriKind.Absolute, out imgUri);
            muaBackground = new Image();
            muaBackground.BackgroundColor = Color.Black;
            muaBackground.Aspect = Aspect.AspectFill;
            if (imgUri != null)
            {
                muaBackground.Source = ImageSource.FromUri(imgUri);
            }
            var pan = new PanGestureRecognizer();
            pan.PanUpdated += OnBgImagePan;
            muaBackground.GestureRecognizers.Add(pan);

            var muaIcon = new CircleImage();
            //muaIcon.Source = ImageSource.FromResource("TiroApp.Images.muaIcon.png");
            muaIcon.BorderThickness = 0;
            muaIcon.Source = mua.ArtistImage;
            muaIcon.Aspect = Aspect.AspectFill;
            muaIcon.HeightRequest = 60;
            muaIcon.WidthRequest = muaIcon.HeightRequest;

            var name = new CustomLabel();
            name.Text = mua.FullName;
            name.TextColor = Color.White;
            name.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            name.FontSize = 20;

            var artistCompany = new CustomLabel();
            bussinesName = mua.BusinessName;
            artistCompany.Text = $"Artist at {bussinesName}";
            artistCompany.TextColor = Color.White;
            artistCompany.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;

            tabView = new TabView(new List<string> { "Services", "Review", "Info" }, Color.FromHex("CF7090"), true);
            tabView.HeightRequest = 50;
            tabView.OnIndexChange += OnTabChange;

            mainContent = new ContentView();            

            mainLayout = new RelativeLayout();
            mainLayout.Children.Add(muaBackground
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Width * ScaleBackImage));
            mainLayout.Children.Add(imageArrowBack
                , Constraint.Constant(10)
                , Constraint.Constant(30)
                , Constraint.Constant(20)
                , Constraint.Constant(20));
            mainLayout.Children.Add(muaIcon
                , Constraint.Constant(20)
                , Constraint.RelativeToParent(p => p.Width * ScaleBackImage - Utils.GetControlSize(muaIcon).Height - 20));
            mainLayout.Children.Add(name
                , Constraint.Constant(90)
                , Constraint.RelativeToParent(p => p.Width * ScaleBackImage - Utils.GetControlSize(muaIcon).Height - 10));
            mainLayout.Children.Add(artistCompany
                , Constraint.Constant(90)
                , Constraint.RelativeToParent(p => p.Width * ScaleBackImage - Utils.GetControlSize(muaIcon).Height / 2 - 15));
            mainLayout.Children.Add(tabView
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width * ScaleBackImage)
                , Constraint.RelativeToParent(p => p.Width));
            mainLayout.Children.Add(mainContent
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width * ScaleBackImage + Utils.GetControlSize(tabView).Height)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height 
                    - p.Width * ScaleBackImage - Utils.GetControlSize(tabView).Height));

            Content = mainLayout;
            OnTabChange(null, 0);
        }
        
        private void OnTabChange(object sender, int e)
        {
            if (tabView.SelectedIndex == 0)
            {
                BuildServices();
            }
            else if (tabView.SelectedIndex == 1)
            {
                BuildReview();
            }
            else
            {
                BuildInfo();
            }
            mainLayout.ForceLayout();
        }

        private void BuildServices()
        {
            /*
            listheaders - sfuidisplay-light
            list - sfuidisplay-regular
            counlabel - sfuidisplay-semibold
            time - bebasneueregular
            */

            if (order == null)
            {
                order = new Order(mua);
            }
            categories = new Dictionary<string, List<Service>>();
            foreach (var service in mua.Services)
            {
                var name = service.Category;
                if (categories.ContainsKey(name))
                {
                    categories[name].Add(service);
                }
                else
                {
                    categories.Add(name, new List<Service> { service });
                }
            }
            servicesHolder = new StackLayout();
            servicesHolder.Spacing = 0;

            orderLayout = new ContentView();
            orderLayout.IsVisible = false;
            orderLayout.VerticalOptions = LayoutOptions.EndAndExpand;
            BuildServicesList();
            BuildOrderLayout();

            var scrollView = new ScrollView { Content = servicesHolder };
            var stackLayout = new StackLayout();
            stackLayout.Children.Add(scrollView);
            stackLayout.Children.Add(orderLayout);
            mainContent.Content = stackLayout;
        }

        private void BuildReview()
        {
            reviewEntry = UIUtils.MakeEntry("Write a review", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            reviewEntry.BackgroundColor = Color.Transparent;
            reviewEntry.VerticalOptions = LayoutOptions.CenterAndExpand;
            reviewEntry.HorizontalOptions = LayoutOptions.FillAndExpand;
            reviewEntry.HeightRequest = 40;
            reviewEntry.FontSize = 15;
            var sendButton = new Button();
            sendButton.BackgroundColor = Color.Transparent;
            sendButton.Text = "SEND";
            sendButton.WidthRequest = 100;
            sendButton.VerticalOptions = LayoutOptions.CenterAndExpand;
            sendButton.TextColor = Props.ButtonColor;
            sendButton.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            sendButton.FontSize = 18;
            sendButton.Margin = new Thickness(0, 0, 20, 0);
            sendButton.HorizontalOptions = LayoutOptions.EndAndExpand;
            sendButton.Clicked += OnSendReviewClick;

            reviewRating = new RatingLayout()
            {
                Rating = 0,
                IsEditable = true,
                Margin = new Thickness(20, 5, 0, 0),
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.Fill
            };

            var sendLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Props.GrayColor,
                IsVisible = CanSendReview,
                Children = {
                    reviewRating,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children = {
                            reviewEntry,
                            sendButton
                        }
                    }
                }
            };

            if (mua.Reviews == null || mua.Reviews.Count == 0)
            {
                var stackLayout = new StackLayout();
                stackLayout.Children.Add(sendLayout);
                stackLayout.VerticalOptions = LayoutOptions.End;
                mainContent.Content = stackLayout;
            }
            else
            {
                double averageValue = mua.Reviews.Select(s => s.Rating).Average();

                var averageLabel = new CustomLabel();
                averageLabel.Text = "AVERAGE REVIEW:";
                averageLabel.TextColor = Props.ButtonInfoPageColor;
                averageLabel.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
                averageLabel.FontSize = 20;
                averageLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
                averageLabel.Margin = new Thickness(20, 0, 20, 0);

                var line = MakeSeparateLine();
                var starsLayout = new RatingLayout();
                starsLayout.HeightRequest = 20;
                starsLayout.Rating = averageValue;
                starsLayout.HorizontalOptions = LayoutOptions.EndAndExpand;
                starsLayout.VerticalOptions = LayoutOptions.CenterAndExpand;
                starsLayout.Margin = new Thickness(0, 0, 20, 0);
                var averageLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(0, 20, 0, 20),
                    Children = {
                    averageLabel,
                    starsLayout
                }
                };
                var header = new StackLayout
                {
                    Children = {
                    averageLayout, line
                }
                };

                var reviewList = new ListView();
                reviewList.ItemTemplate = GetDataTemplate();
                reviewList.ItemSelected += (s, args) => { reviewList.SelectedItem = null; };
                reviewList.ItemsSource = mua.Reviews;
                reviewList.SeparatorColor = Props.GrayColor;
                //reviewList.RowHeight = 70;
                reviewList.Header = header;

                var stackLayout = new StackLayout();
                reviewList.HasUnevenRows = true;
                stackLayout.Children.Add(reviewList);
                stackLayout.Children.Add(sendLayout);
                mainContent.Content = stackLayout;
            }
        }

        private void OnSendReviewClick(object sender, EventArgs arg)
        {
            var spinner = UIUtils.ShowSpinner(this);
            int rating = Convert.ToInt32(reviewRating.Rating);
            DataGate.SendReview(mua.Id, GlobalStorage.Settings.CustomerId, rating, reviewEntry.Text, (res) =>
            {
                if (res.Code == ResponseCode.OK && res.Result.ToString() == "true")
                {
                    DataGate.GetMua(mua.Id, (data) =>
                    {
                        if (data.Code == ResponseCode.OK)
                        {
                            mua = new MuaArtist(JObject.Parse(data.Result), true);
                        }
                        DataGate.CanWriteReview(mua.Id, GlobalStorage.Settings.CustomerId, data2 =>
                        {
                            CanSendReview = (data2.Code == ResponseCode.OK && data2.Result == "true");
                            UIUtils.HideSpinner(this, spinner);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                BuildReview();
                            });
                        });
                    });
                }
                else
                {
                    UIUtils.ShowMessage("Review send fail. Try later", this);
                    UIUtils.HideSpinner(this, spinner);
                }                
            });
        }

        private void BuildInfo()
        {
            var map = mua.Map;

            var artistCompanyInfo = new CustomLabel();
            artistCompanyInfo.Text = $"Artist at {bussinesName}";
            artistCompanyInfo.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            artistCompanyInfo.TextColor = Color.Black;
            artistCompanyInfo.HorizontalOptions = LayoutOptions.Center;
            artistCompanyInfo.Margin = new Thickness(0, 10, 0, 5);

            var categories = new CustomLabel();
            categories.Text = mua.Services.Select(e => e.Category).Distinct().Aggregate((i, j) => i + " | " + j);
            categories.TextColor = Props.ButtonInfoPageColor;
            categories.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            categories.HorizontalOptions = LayoutOptions.Center;

            var address = new CustomLabel();
            address.Text = mua.Address;
            address.TextColor = Color.Gray;
            address.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            address.HorizontalOptions = LayoutOptions.Center;
            address.Margin = new Thickness(0, 10, 0, 10);

            var loc = string.Format("{0},{1}", mua.Lat, mua.Lon);
            map.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                switch (Device.OS)
                {
                    case TargetPlatform.iOS:
                        Device.OpenUri(
                          new Uri(string.Format("http://maps.apple.com/maps?q={0}&sll={1}", address.Text.Replace(' ', '+'), loc)));
                        break;
                    case TargetPlatform.Android:
                        Device.OpenUri(
                          new Uri(string.Format("geo:0,0?q={0}({1})", loc, address.Text)));
                        break;
                }
            }));

            var description = new CustomLabel();
            //description.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed to eiusmod tempor incididunt ut labore "
            //    + "et dolore magna aliqua. Ut enim ad minim veniam quis nostrud.";
            description.Text = mua.AboutMe;
            description.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            description.TextColor = Color.Black;
            description.HorizontalOptions = LayoutOptions.Center;
            description.HorizontalTextAlignment = TextAlignment.Center;
            description.Margin = new Thickness(40, 0, 40, 0);

            var scrollView = new ScrollView {
                Content = new StackLayout {
                    Children = {
                        map,
                        artistCompanyInfo,
                        categories,
                        address,
                        description
                    }
                }
            };

            var stackLayout = new StackLayout();
            stackLayout.Children.Add(scrollView);
            //stackLayout.Children.Add(UIUtils.MakeButton("Request", UIUtils.FONT_SFUIDISPLAY_MEDIUM, true));

            mainContent.Content = stackLayout;
        }

        private DataTemplate GetDataTemplate()
        {
            return new DataTemplate(() =>
            {
                Label author = new CustomLabel();
                author.SetBinding(Label.TextProperty, "Author");
                author.TextColor = Props.ButtonInfoPageColor;
                author.FontSize = 20;
                author.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
                author.Margin = new Thickness(20, 0, 20, 0);

                var ratingLayout = new RatingLayout();
                ratingLayout.HeightRequest = 20;
                ratingLayout.SetBinding(RatingLayout.RatingProperty, "Rating");
                ratingLayout.HorizontalOptions = LayoutOptions.EndAndExpand;
                ratingLayout.Margin = new Thickness(0, 0, 20, 0);

                var top = new StackLayout {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(0, 20, 0, 0),
                    Children = {
                        author,
                        ratingLayout
                    }
                };

                var review = new CustomLabel();
                review.SetBinding(Label.TextProperty, "Description");
                review.TextColor = Color.Black;
                review.HorizontalOptions = LayoutOptions.Fill;
                review.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
                review.Margin = new Thickness(20, 0, 20, 0);

                var layout = new StackLayout();
                layout.Children.Add(top);
                layout.Children.Add(review);
                var viewCell = new ViewCell();
                viewCell.View = layout;
                return viewCell;
            });
        }

        private StackLayout MakeServiceRow(Service service, bool isChecked = false)
        {
            var name = new CustomLabel();
            name.Text = service.Name;
            name.Margin = new Thickness(20, 0, 0, 0);
            name.TextColor = Color.Black;
            name.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            name.FontSize = 16;

            var price = new CustomLabel();
            var length = service.Length;
            price.TextColor = Color.FromHex("CCCCCC");
            price.Text = UIUtils.NIARA_SIGN + service.Price + " and up for " + length + " minutes";
            price.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            price.Margin = new Thickness(20, 0, 0, 0);

            var info = new StackLayout {
                Children = { name, price }
            };

            var line = MakeSeparateLine();

            var infoLayout = new StackLayout();
            infoLayout.Orientation = StackOrientation.Horizontal;
            infoLayout.Children.Add(info);
            if (isChecked)
            {
                var check = new Image();
                check.Source = ImageSource.FromResource("TiroApp.Images.check.png");
                check.HeightRequest = 20;
                check.WidthRequest = check.HeightRequest;
                Device.OnPlatform(() => check.Margin = new Thickness(0, 0, 20, 0),
                    () => check.Margin = new Thickness(0, 0, 40, 0));
                check.HorizontalOptions = LayoutOptions.EndAndExpand;
                check.VerticalOptions = LayoutOptions.CenterAndExpand;
                check.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                    order.Basket.Remove(order.Basket.Single(i => i.Service.Id.Equals(service.Id)));
                    BuildServicesList();
                }));
                infoLayout.Children.Add(check);
            }
            else
            {
                var bookButton = new Button();
                bookButton.BackgroundColor = Color.Transparent;
                bookButton.Margin = new Thickness(0, 0, 20, 0);
                bookButton.Text = "BOOK";
                bookButton.TextColor = Props.ButtonColor;
                bookButton.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
                bookButton.FontSize = 18;
                bookButton.HorizontalOptions = LayoutOptions.EndAndExpand;
                bookButton.VerticalOptions = LayoutOptions.CenterAndExpand;
                bookButton.Clicked += (s, args) => {
                    order.Basket.Add(new OrderItem(service, 1));
                    BuildServicesList();
                };
                infoLayout.Children.Add(bookButton);
            }

            var layout = new StackLayout {
                Children = { infoLayout, line }
            };
            return layout;
        }

        private void BuildServicesList()
        {
            servicesHolder.Children.Clear();
            foreach (var category in categories)
            {
                servicesHolder.Children.Add(new StackLayout
                {
                    BackgroundColor = Props.GrayColor,
                    Children = {
                        new CustomLabel {
                            Text = category.Key, HorizontalOptions = LayoutOptions.CenterAndExpand,
                            TextColor = Color.Gray, FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT
                        }
                    }
                });
                foreach (var service in category.Value)
                {
                    var services = order.Basket.Select(i => i.Service);
                    if (services.Contains(service))
                    {
                        servicesHolder.Children.Add(MakeServiceRow(service, true));
                    }
                    else
                    {
                        servicesHolder.Children.Add(MakeServiceRow(service));
                    }
                }
            }
            if (order.Basket.Count > 0)
            {
                orderLayout.IsVisible = true;
                BuildOrderInfo();
            }
            else
            {
                orderLayout.IsVisible = false;
            }
        }

        private BoxView MakeSeparateLine()
        {
            return new BoxView()
            {
                Color = Props.GrayColor,
                HeightRequest = 1
            };
        }

        private void BuildOrderLayout()
        {
            orderInfo = new ContentView();
            //serviceTimeGrid = MakeOrderLayout();
            var confirmButton = UIUtils.MakeButton("CONTINUE", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            confirmButton.Clicked += ContinueOrder;
            var layout = new StackLayout {
                Spacing = 0,
                Children = {
                    orderInfo, confirmButton //serviceTimeGrid
                }
            };
            orderLayout.Content = layout;
        }

        private void ContinueOrder(object sender, EventArgs e)
        {
            spinner = UIUtils.ShowSpinner(this);
            var id = mua.Id;
            DataGate.MuaGetAvailability(id, DateTime.Today, DateTime.Today.AddMonths(1), true, result =>
            {
                if (result.Code == ResponseCode.OK)
                {
                    var avail = Availibility.Parse(result.Result);
                    if (avail.DatesFrom.Count == 0)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            UIUtils.HideSpinner(this, spinner);
                            UIUtils.ShowMessage("The artist does not have free time for the next month", this);
                        });
                        return;
                    }
                    order.IsFree = SearchHeader.IsFreeMakeoverL;
                    var ap = new AvailabilityPage(order);
                    ap.Availibility = avail;
                    ap.SelectedDate = avail.DatesFrom.First();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.Navigation.PushAsync(ap);
                    });
                }
                else
                {
                    UIUtils.ShowServerUnavailable(this);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    UIUtils.HideSpinner(this, spinner);
                });
            });
        }

        private Grid MakeOrderLayout()
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            var l1 = MakeDateLabel(DateTime.Today);
            var l2 = MakeDateLabel(DateTime.Today);
            var l3 = MakeDateLabel(DateTime.Today);
            var l4 = MakeDateLabel(DateTime.Today);
            var l5 = new CustomLabel { Text = "MORE", HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center, Margin = new Thickness(0, 5, 0, 0) };
            grid.Children.Add(l1, 0, 0);
            grid.Children.Add(l2, 1, 0);
            grid.Children.Add(l3, 2, 0);
            grid.Children.Add(l4, 3, 0);
            grid.Children.Add(l5, 4, 0);
            var l6 = MakeTimeLabel("06 PM");
            var l7 = MakeTimeLabel("06:15 PM");
            var l8 = MakeTimeLabel("06:30 PM");
            var l9 = MakeTimeLabel("06:45 PM");
            var l10 = MakeTimeLabel("07 PM");
            grid.Children.Add(l6, 0, 1);
            grid.Children.Add(l7, 1, 1);
            grid.Children.Add(l8, 2, 1);
            grid.Children.Add(l9, 3, 1);
            grid.Children.Add(l10, 4, 1);
            grid.BackgroundColor = Props.GrayColor;
            //grid.ColumnSpacing = 3;
            //grid.RowSpacing = 3;

            return grid;
        }

        private StackLayout MakeDateLabel(DateTime date)
        {
            return new StackLayout {
                Spacing = 0,
                Children = {
                    new CustomLabel {
                        Text = date.ToString("dd") + Environment.NewLine
                            + date.ToString("MMM", Utils.EnCulture).ToUpper(),
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Color.Black,
                        Margin = new Thickness(0, 5, 0, 0)
                    },
                    new CustomLabel {
                        Text = date.ToString("ddd", Utils.EnCulture).ToUpper(),
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 0)
                    }
                }
            };
        }

        private Label MakeTimeLabel(string text)
        {
            return new CustomLabel {
                Text = text,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Props.ButtonColor,
                Margin = new Thickness(0, 15, 0, 15)
            };
        }

        private void BuildOrderInfo()
        {
            var layout = new StackLayout {
                Spacing = 0
            };
            foreach (var item in order.Basket)
            {
                layout.Children.Add(BuildOrderRaw(item));
            }
            orderInfo.Content =  layout;
        }

        private StackLayout BuildOrderRaw(OrderItem item)
        {
            var name = new CustomLabel();
            name.Text = item.Service.Name;
            name.TextColor = Color.Black;
            name.FontSize = 16;
            name.FontFamily = UIUtils.FONT_SFUIDISPLAY_SEMIBOLD;
            name.HorizontalOptions = LayoutOptions.Start;
            name.Margin = new Thickness(20, 0, 0, 0);

            var price = new CustomLabel();
            price.Text = UIUtils.NIARA_SIGN + item.TotalPrice;
            price.TextColor = Color.Black;
            price.FontSize = 16;
            price.FontFamily = UIUtils.FONT_SFUIDISPLAY_SEMIBOLD;
            price.HorizontalOptions = LayoutOptions.CenterAndExpand;

            var countLabel = new CustomLabel();
            countLabel.Text = $" {item.Count}x ";
            countLabel.TextColor = Color.Black;
            countLabel.FontSize = 16;
            countLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_SEMIBOLD;

            var minus = new Image();
            minus.Source = ImageSource.FromResource("TiroApp.Images.minusService.png");
            minus.HeightRequest = 20;
            minus.WidthRequest = minus.HeightRequest;
            minus.HorizontalOptions = LayoutOptions.End;
            minus.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                item.Count = item.Count - 1 > 0 ? item.Count - 1 : 1;
                countLabel.Text = $" {item.Count}x ";
                price.Text = UIUtils.NIARA_SIGN + item.TotalPrice;
            }));

            var plus = new Image();
            plus.Source = ImageSource.FromResource("TiroApp.Images.plusService.png");
            plus.HeightRequest = 20;
            plus.WidthRequest = plus.HeightRequest;
            plus.HorizontalOptions = LayoutOptions.End;
            plus.Margin = new Thickness(0, 0, 20, 0);
            plus.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                countLabel.Text = $" {++item.Count}x ";
                price.Text = UIUtils.NIARA_SIGN + item.TotalPrice;
            }));

            return new StackLayout
            {
                Spacing = 0,
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.FromRgb(248, 248, 248),
                Padding = new Thickness(0, 10, 0, 10),
                Children = {
                    name, price, minus, countLabel, plus
                }
            };
        }

        private double imageBgPanDX = 0;
        private void OnBgImagePan(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    imageBgPanDX = 0;
                    break;
                case GestureStatus.Running:
                    imageBgPanDX = e.TotalX;
                    break;
                case GestureStatus.Completed:
                    var inc = imageBgPanDX >= 0 ? 1 : -1;
                    try
                    {
                        if (mua.Images.Count == 0)
                            return;
                        muaBackgroundImageIndex += inc;
                        if (muaBackgroundImageIndex < 0)
                        {
                            muaBackgroundImageIndex = mua.Images.Count - 1;
                        }
                        else if (muaBackgroundImageIndex > mua.Images.Count - 1)
                        {
                            muaBackgroundImageIndex = 0;
                        }
                        muaBackground.Source = ImageSource.FromUri(new Uri(mua.Images[muaBackgroundImageIndex]));
                    }
                    catch { }
                    break;
            }
        }
    }

    public class Review
    {
        public Review(JObject jObj)
        {
            Init(jObj);
        }

        public string Id { get; private set; }
        public string Description { get; private set; }
        public double Rating { get; private set; }
        public string Author { get; private set; }

        private void Init(JObject jObj)
        {
            Id = (string)jObj["Id"];
            Rating = (double)jObj["Rating"];
            Description = (string)jObj["Description"];
            Author = (string)jObj["ReviewFrom"];
        }
    }

    public class Service
    {
        public Service(JObject jObj, bool isFromGetMua = false)
        {
            Init(jObj, isFromGetMua);
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public double Price { get; private set; }
        public string Length { get; private set; }
        public string Category { get; private set; }

        private void Init(JObject jObj, bool isFromGetMua)
        {
            Id = isFromGetMua ? (string)jObj["Id"] : (string)jObj["ServiceId"];
            Name = (string)jObj["Name"];
            Length = (string)jObj["Length"];
            Price = (double)jObj["Price"];
            Category = (string)jObj["Category"];
        }
    }
}
