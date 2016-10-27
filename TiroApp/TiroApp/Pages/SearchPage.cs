using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TiroApp.Views;
using Xamarin.Forms;
using System;

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
            muaList.RowHeight = Device.OnPlatform(238, 270, 250);
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
                    return p.Height - 120;
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
                    var sellers = sellersEl.Select(s => new MuaDataItem((JObject)s));
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        muaList.ItemsSource = null;
                        muaList.ItemsSource = sellers;
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
        
        private DataTemplate GetDataTemplate()
        {
            return new DataTemplate(() =>
            {
                Label address = new CustomLabel();
                address.SetBinding(Label.TextProperty, "Address");
                address.TextColor = Color.White;
                address.LineBreakMode = LineBreakMode.TailTruncation;
                address.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;

                Label mainText = new CustomLabel();
                mainText.SetBinding(Label.TextProperty, "BusinessName");
                mainText.FontSize = 21;
                mainText.TextColor = Color.White;
                mainText.FontFamily = UIUtils.FONT_BEBAS_BOOK;

                Label price = new CustomLabel();
                price.SetBinding(Label.TextProperty, "Price");
                price.TextColor = Color.White;
                price.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;

                Label nameMua = new CustomLabel();
                nameMua.SetBinding(Label.TextProperty, "FullName");
                nameMua.TextColor = Color.White;
                nameMua.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
                                
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
                location.Source = ImageSource.FromResource("TiroApp.Images.location.png");
                location.HeightRequest = 20;
                location.WidthRequest = rightImg.HeightRequest;

                var ratingLayout = new RatingLayout();
                ratingLayout.HeightRequest = 20;
                ratingLayout.SetBinding(RatingLayout.RatingProperty, "Rating");

                var layout = new RelativeLayout();
                layout.BackgroundColor = Color.Black;//TEMP
                layout.Children.Add(imageBackground, Constraint.Constant(0), Constraint.Constant(0)
                    , Constraint.RelativeToParent(p => p.Width)
                    , Constraint.RelativeToParent(p => p.Height));
                layout.Children.Add(price
                    , Constraint.Constant(10)
                    , Constraint.Constant(10));
                layout.Children.Add(mainText
                    , Constraint.Constant(10)
                    , Constraint.RelativeToParent(p => p.Height - p.Height / 3 - Utils.GetControlSize(mainText).Height + 20));
                layout.Children.Add(nameMua
                    , Constraint.Constant(10)
                    , Constraint.RelativeToParent(p => p.Height - p.Height / 3 + 20));
                layout.Children.Add(leftImg
                    , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(leftImg).Width - 10)
                    , Constraint.RelativeToParent(p => p.Height / 2 - Utils.GetControlSize(leftImg).Height / 2));
                layout.Children.Add(rightImg
                   , Constraint.Constant(10)
                   , Constraint.RelativeToParent(p => p.Height / 2 - Utils.GetControlSize(rightImg).Height / 2));
                layout.Children.Add(location
                    , Constraint.Constant(0)
                    , Constraint.RelativeToParent(p => p.Height - Utils.GetControlSize(location).Height - 15));
                layout.Children.Add(address
                    , Constraint.Constant(40)
                    , Constraint.RelativeToParent(p => p.Height - Utils.GetControlSize(address).Height - 15)
                    , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(ratingLayout).Width - 60)
                    , Constraint.RelativeToParent(p => Utils.GetControlSize(address).Height));
                layout.Children.Add(ratingLayout
                    , Constraint.RelativeToParent(p => p.Width - Utils.GetControlSize(ratingLayout).Width - 15)
                    , Constraint.RelativeToParent(p => p.Height - Utils.GetControlSize(ratingLayout).Height - 15));
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
        public string Price { get { return $"{UIUtils.NIARA_SIGN}{PriceMin} - {UIUtils.NIARA_SIGN}{PriceMax}"; } }
        public double Rating { get { return (double)_jElement["Rating"]; } }
        public ImageSource PictureSourse
        {
            get
            {
                var pics = Pictures;
                if (pics.Length != 0)
                {
                    return ImageSource.FromUri(new System.Uri(pics[0]));
                }
                return ImageSource.FromUri(new System.Uri("http://tiro.flexible-solutions.com.ua/Untitled-1.png"));
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
