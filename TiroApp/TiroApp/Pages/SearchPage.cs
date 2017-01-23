using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TiroApp.Views;
using Xamarin.Forms;
using System;
using Gis4Mobile.Services.GeoLocation;

namespace TiroApp.Pages
{
    public class SearchPage : ContentPage
    {
        //private StackLayout filtersLayout;
        //private List<RadioButton> filters;
        //private StackLayout filtersBoxHolder;
        //private StackLayout filtersBoxHolderAdditional;        
        //private Image searchBackgroundBig;
        private ListView muaList;
        private AbsoluteLayout spinnerHolder;
        private ActivityIndicator spinner;
        private SearchHeader searchHeader;
        private RelativeLayout mainLayout;
        private IEnumerable<MuaDataItem> muaData;
        private IEnumerable<MuaDataItem> filteredMuaData;
        private MuaListSlider slider;

        public SearchPage()
        {
            Utils.SetupPage(this);

            mainLayout = new RelativeLayout();
            this.Content = mainLayout;

            BackgroundColor = Color.White;
            BuildLayout();
            //AddSideMenu();
            searchHeader.filtersLayout.IsVisible = true;
        }

        public SearchPage(bool isFreeMakeover)
            : this()
        {
            searchHeader.IsFreeMakeover = isFreeMakeover;
            Utils.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                SearchButton_Clicked(this, EventArgs.Empty);
                return false;
            });
        }

        public SearchPage(List<bool> filter)
            : this()
        {
            searchHeader.filtersLayout.IsVisible = false;
            for (var i = 0; i < searchHeader.filters.Count; i++)
            {
                searchHeader.filters[i].IsChecked = filter[i];
            }
            searchHeader.RefreshSearchBox();
            Utils.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                SearchButton_Clicked(this, EventArgs.Empty);
                return false;
            });
        }

        private void BuildLayout()
        {
            muaList = new ListView();
            muaList.RowHeight = Device.OnPlatform(238 + 70, 270 + 70, 250 + 70);
            muaList.SeparatorColor = Color.Transparent;
            muaList.SeparatorVisibility = SeparatorVisibility.None;
            muaList.ItemTemplate = GetDataTemplate();
            //muaList.ItemSelected += OnItemSelected;
            muaList.ItemTapped += OnItemSelected;
            mainLayout.Children.Add(muaList
                , Constraint.Constant(0)
                , Constraint.Constant(120)
                , Constraint.RelativeToParent(p => {
                    return p.Width;
                })
                , Constraint.RelativeToParent(p => {
                    return p.Height - 120 - 100;
                }));

            slider = new MuaListSlider();
            slider.OnSlide += Slider_OnSlide;
            mainLayout.Children.Add(slider
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => {
                    return p.Height - 100;
                })
                , Constraint.RelativeToParent(p => {
                    return p.Width;
                })
                , Constraint.RelativeToParent(p => {
                    return 100;
                }));

            BuildSearchHeader();

            var imageArrowBack = new Image();
            imageArrowBack.Source = ImageSource.FromResource("TiroApp.Images.ArrowBackBlack.png");
            imageArrowBack.HeightRequest = 20;
            imageArrowBack.Margin = new Thickness(10, 0, 0, 0);
            imageArrowBack.VerticalOptions = LayoutOptions.Center;
            imageArrowBack.HorizontalOptions = LayoutOptions.Start;
            imageArrowBack.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
            {
                this.Navigation.PopAsync();
            }));
            mainLayout.Children.Add(imageArrowBack, Constraint.Constant(0), Constraint.Constant(20));

            spinnerHolder = new AbsoluteLayout();
            spinnerHolder.BackgroundColor = Props.BlackoutColor;
            spinner = new ActivityIndicator();
            //spinner.Color = Color.Red;
            //spinner.IsRunning = true;
            AbsoluteLayout.SetLayoutFlags(spinner, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(spinner, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            spinnerHolder.Children.Add(spinner);
            spinnerHolder.IsVisible = false;
            mainLayout.Children.Add(spinnerHolder
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));
        }

        private void BuildSearchHeader()
        {
            searchHeader = new SearchHeader();
            searchHeader.ParentPage = this;
            searchHeader.OnSearchClick += SearchButton_Clicked;
            searchHeader.OnSortChanged += SearchHeader_OnSortChanged;
            mainLayout.Children.Add(searchHeader, 
                Constraint.Constant(0), Constraint.Constant(0));
        }
        
        private void OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            SpinnerShowChange(true);
            var selItem = e.Item as MuaDataItem;
            DataGate.GetMua(selItem.Id, (data) => {                
                if (data.Code == ResponseCode.OK)
                {
                    var jobj = JObject.Parse(data.Result);
                    bool canWriteReview = !string.IsNullOrEmpty(GlobalStorage.Settings.CustomerId);
                    if (!canWriteReview)
                    {
                        ShowMuaPage(jobj, canWriteReview);
                    }
                    else
                    {
                        DataGate.CanWriteReview(selItem.Id, GlobalStorage.Settings.CustomerId, data2 =>
                        {
                            canWriteReview = (data2.Code == ResponseCode.OK && data2.Result == "true");
                            ShowMuaPage(jobj, canWriteReview);
                        });
                    }
                }
                else
                {
                    SpinnerShowChange(false);
                    Device.BeginInvokeOnMainThread(() => {
                        this.DisplayAlert("Tiro", "Nothing was found", "OK");
                    });
                }
            });
        }

        private void ShowMuaPage(JObject data, bool canWriteReview)
        {
            SpinnerShowChange(false);
            Device.BeginInvokeOnMainThread(() =>
            {
                var muaPage = new MuaPage(data);
                muaPage.CanSendReview = canWriteReview;
                this.Navigation.PushAsync(muaPage);
            });
        }

        private void SearchButton_Clicked(object sender, System.EventArgs e)
        {
            SpinnerShowChange(true);
            searchHeader.filtersLayout.IsVisible = false;
            var sendData = searchHeader.filters.Where(f => f.IsChecked).Select(f => f.Text).ToList();
            Availibility aFilter = null;
            LocationFilter lFilter = null;
            if (searchHeader.filersPage != null)
            {
                aFilter = searchHeader.filersPage.AvailibilityFilter;
                lFilter = searchHeader.filersPage.LocationFilter;
            }
            var callback = new Action<ResponseDataJson>((data) =>
            {
                SpinnerShowChange(false);
                if (data.Code == ResponseCode.OK)
                {
                    var sellersEl = JArray.Parse(data.Result);
                    muaData = sellersEl.Select(s => new MuaDataItem((JObject)s));
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        muaList.ItemsSource = null;
                        slider.BuildLayout();
                        SearchHeader_OnSortChanged(null, null);
                    });
                }
                else
                {
                    UIUtils.ShowMessage("Nothing was found", this);
                }
            });
            if (searchHeader.IsFreeMakeover)
            {
                DataGate.MuaSearchFree(sendData, lFilter, aFilter, callback);
            }
            else
            {
                DataGate.DoMUASearchJson(sendData, lFilter, aFilter, callback);
            }
        }

        private void SearchHeader_OnSortChanged(object sender, EventArgs e)
        {
            var itemSource = filteredMuaData;
            switch (searchHeader.SearchSort)
            {
                case SearchSortType.Rating:
                    itemSource = filteredMuaData.OrderByDescending(m => m.Rating);
                    break;
                case SearchSortType.Nearest:
                    var currPosition = Geolocator.Instance.LastKnowPosition;
                    if (currPosition != null)
                    {
                        itemSource = filteredMuaData.OrderBy(m => Geolocator.DistanceBetweenPlaces(
                            currPosition.Longitude, currPosition.Latitude, m.LocationLon, m.LocationLat));
                    }
                    break;
                case SearchSortType.LowestPrice:
                    itemSource = filteredMuaData.OrderBy(m => m.PriceMin);
                    break;
                case SearchSortType.HighestPrice:
                    itemSource = filteredMuaData.OrderByDescending(m => m.PriceMax);
                    break;
            }
            muaList.ItemsSource = itemSource;
        }

        private void Slider_OnSlide(object sender, Tier e)
        {
            filteredMuaData = muaData.Where(m => m.Tier == e);
            muaList.ItemsSource = filteredMuaData;
        }

        private DataTemplate GetDataTemplate()
        {
            return new DataTemplate(() =>
            {
                CustomLabel address = new CustomLabel();
                address.SetBinding(Label.TextProperty, "Address");
                address.LetterSpacing = 0.15f;
                address.FontSize = 14;
                address.TextColor = Color.FromHex("#4A4A4A");
                address.LineBreakMode = LineBreakMode.TailTruncation;
                address.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
                address.Margin = new Thickness(10, 0, 5, 0);

                CustomLabel mainText = new CustomLabel();
                mainText.SetBinding(Label.TextProperty, "BusinessName");
                mainText.LetterSpacing = 0.25f;
                mainText.FontSize = 21;
                mainText.TextColor = Color.FromHex("#4A4A4A");
                mainText.FontFamily = UIUtils.FONT_BEBAS_BOOK;
                mainText.Margin = new Thickness(5, 0, 0, 0);

                CustomLabel price = new CustomLabel();
                price.SetBinding(Label.TextProperty, "Price");
                price.LetterSpacing = 0.1f;
                price.FontSize = 14;
                price.TextColor = Color.FromHex("#4A4A4A");
                price.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;

                CustomLabel nameMua = new CustomLabel();
                nameMua.SetBinding(Label.TextProperty, "FullName");
                nameMua.LetterSpacing = 0.15f;
                nameMua.FontSize = 16;
                nameMua.TextColor = Color.FromHex("#4A4A4A");
                nameMua.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
                nameMua.Margin = new Thickness(5, 0, 0, 0);

                var imageBackground = new Image();
                imageBackground.SetBinding(UIUtils.TagProperty, "Pictures");
                imageBackground.SetValue(UIUtils.Tag2Property, 0);
                imageBackground.Aspect = Aspect.AspectFill;
                imageBackground.SetBinding(Image.SourceProperty, "PictureSourse");

                var leftImg = new Image();
                leftImg.Source = ImageSource.FromResource("TiroApp.Images.goToMua.png");
                leftImg.HeightRequest = 40;
                leftImg.WidthRequest = leftImg.HeightRequest;
                leftImg.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
                {
                    ShowNextBgImage(imageBackground, -1);
                }));

                var rightImg = new Image();
                rightImg.Source = ImageSource.FromResource("TiroApp.Images.goToMua.png");
                rightImg.Rotation = 180;
                rightImg.HeightRequest = 40;
                rightImg.WidthRequest = rightImg.HeightRequest;
                rightImg.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
                {
                    ShowNextBgImage(imageBackground, 1);
                }));

                var location = new Image();                
                location.Source = ImageSource.FromResource("TiroApp.Images.location_black.png");
                location.HeightRequest = 20;
                //location.WidthRequest = rightImg.HeightRequest;

                var ratingLayout = new RatingLayout();
                ratingLayout.HeightRequest = 20;
                ratingLayout.SetBinding(RatingLayout.RatingProperty, "Rating");

                var infoImage = new Image();
                infoImage.Source = ImageSource.FromResource("TiroApp.Images.muaListRectangle.png");
                infoImage.Aspect = Aspect.Fill;
                //infoImage.HeightRequest = 70;
                //infoImage.WidthRequest = App.ScreenWidth;

                //var infoFrame = new Frame();
                //infoFrame.Content = infoImage;
                //infoFrame.OutlineColor = Color.FromHex("#979797");

                var layout = new RelativeLayout();
                layout.Children.Add(imageBackground, Constraint.Constant(0), Constraint.Constant(0)
                    , Constraint.RelativeToParent(p => p.Width)
                    , Constraint.RelativeToParent(p => p.Height - 70));
                layout.Children.Add(leftImg
                    , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(leftImg).Width - 10)
                    , Constraint.RelativeToParent(p => (p.Height - 70) / 2 - Utils.GetControlSize(leftImg).Height / 2));
                layout.Children.Add(rightImg
                    , Constraint.Constant(10)
                    , Constraint.RelativeToParent(p => (p.Height - 70) / 2 - Utils.GetControlSize(rightImg).Height / 2));
                layout.Children.Add(infoImage
                    , Constraint.Constant(0)
                    , Constraint.RelativeToParent(p => p.Height - 70)
                    , Constraint.RelativeToParent(p => p.Width)
                    , Constraint.Constant(75));
                layout.Children.Add(mainText
                    , Constraint.Constant(3)
                    , Constraint.RelativeToParent(p => p.Height - 65));
                layout.Children.Add(nameMua
                    , Constraint.Constant(3)
                    , Constraint.RelativeToParent(p => p.Height - Utils.GetControlSize(nameMua).Height - Utils.GetControlSize(location).Height - 4));
                layout.Children.Add(location
                    , Constraint.Constant(8)
                    , Constraint.RelativeToParent(p => p.Height - Utils.GetControlSize(location).Height - 2));
                layout.Children.Add(address
                    , Constraint.RelativeToParent(p => Utils.GetControlSize(location).Width + 4)
                    , Constraint.RelativeToParent(p => p.Height - Utils.GetControlSize(address).Height - 2)
                    , Constraint.RelativeToParent(p => Utils.GetControlSize(address).Width)
                    , Constraint.RelativeToParent(p => Utils.GetControlSize(address).Height));
                layout.Children.Add(ratingLayout
                    , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(ratingLayout).Width - 5)
                    , Constraint.RelativeToParent(p => p.Height - 65));
                layout.Children.Add(price
                    , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(price).Width - 5)
                    , Constraint.RelativeToParent(p => p.Height - 40));
                layout.Margin = new Thickness(0, 0, 0, 25);
                var viewCell = new ViewCell();
                viewCell.View = layout;
                return viewCell;
            });
        }

        private void ShowNextBgImage(Image img, int inc)
        {
            try
            {
                var index = (int)img.GetValue(UIUtils.Tag2Property);
                var imagesArr = (string[])img.GetValue(UIUtils.TagProperty);
                if (imagesArr.Length == 0)
                    return;
                index += inc;
                if (index < 0)
                {
                    index = imagesArr.Length - 1;
                }
                else if (index > imagesArr.Length - 1)
                {
                    index = 0;
                }
                img.SetValue(UIUtils.Tag2Property, index);
                img.Source = ImageSource.FromUri(new Uri(imagesArr[index]));
            }
            catch { }
        }

        private void SpinnerShowChange(bool isShow)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                spinnerHolder.IsVisible = isShow;
                spinner.IsRunning = isShow;
            });
        }
    }

    public class MuaDataItem
    {
        private JObject _jElement;

        public MuaDataItem(JObject jElementml)
        {
            this._jElement = jElementml;
        }

        public string Id { get { return (string)_jElement["Id"]; } }
        public string Email { get { return (string)_jElement["Email"]; } }
        public string FirstName { get { return (string)_jElement["FirstName"]; } }
        public string LastName { get { return (string)_jElement["LastName"]; } }
        public string FullName {  get { return "Make Up by " + FirstName + " " + LastName; } }
        public string BusinessName { get { return (string)_jElement["BusinessName"]; } }
        public string Phone { get { return (string)_jElement["Phone"]; } }
        public string Address { get { return (string)_jElement["Address"]; } }
        public double LocationLat { get { return (double)_jElement["LocationLat"]; } }
        public double LocationLon { get { return (double)_jElement["LocationLon"]; } }        
        public string AboutMe { get { return (string)_jElement["AboutMe"]; } }
        public double PriceMin { get { return (double)_jElement["PriceMin"]; } }
        public double PriceMax { get { return (double)_jElement["PriceMax"]; } }
        //public string Price { get { return $"{UIUtils.NIARA_SIGN}{PriceMin} - {UIUtils.NIARA_SIGN}{PriceMax}"; } }
        public string Price { get { return $"From {UIUtils.NIARA_SIGN}{PriceMin}"; } }
        public double Rating { get { return (double)_jElement["Rating"]; } }
        public Tier Tier { get { return (Tier)(int)_jElement["Tier"]; } }
        public string DefaultPicture { get { return (string)_jElement["DefaultPicture"]; } }
        public ImageSource PictureSourse
        {
            get
            {
                if (DefaultPicture != null)
                {
                    return ImageSource.FromUri(new Uri(DefaultPicture));
                }
                var pics = Pictures;
                if (pics.Length != 0)
                {
                    return ImageSource.FromUri(new Uri(pics[0]));
                }
                return ImageSource.FromUri(new Uri("http://tiro.flexible-solutions.com.ua/Untitled-1.png"));
            }
        }

        public string[] Pictures
        {
            get
            {
                try
                {
                    var list = new List<string>();
                    foreach (string imgEl in _jElement["Pictures"])
                    {
                        var imgStr = imgEl.Trim();
                        if (!string.IsNullOrEmpty(imgStr))
                        {
                            list.Add(imgStr);
                        }
                    }
                    return list.ToArray();
                }
                catch
                {
                }
                return new string[] { };
            }
        }
    }

}
