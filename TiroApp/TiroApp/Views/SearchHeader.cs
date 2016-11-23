using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Pages;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class SearchHeader : RelativeLayout
    {
        public StackLayout filtersLayout;
        public List<RadioButton> filters;
        public StackLayout filtersBoxHolder;
        public StackLayout filtersBoxHolderAdditional;
        private string searchBackgroundSource = "TiroApp.Images.searchBackground.png";
        private string searchBackgroundBigSource = "TiroApp.Images.searchBackgroundBig.png";
        public Image searchBackgroundBig;
        public Image sortIcon;
        public SearchFilersPage filersPage;
        private CustomLabel placeLabel;
        private static bool isFreeMakeover = false;
        private RelativeLayout sortContainer;

        public event EventHandler OnSearchClick;
        public event EventHandler OnSortChanged;

        public SearchHeader()
        {
            isFreeMakeover = false;
            BuildLayout();
        }

        private void BuildLayout()
        {
            var rl = new RelativeLayout();
            rl.WidthRequest = Math.Min(App.ScreenWidth, Device.OnPlatform(400, 480, 400));
            this.Children.Add(rl, Constraint.RelativeToParent(p => (p.Width - rl.WidthRequest) / 2), Constraint.Constant(0));

            sortIcon = new Image();
            sortIcon.Source = ImageSource.FromResource("TiroApp.Images.search_sort_icon.png");
            sortIcon.HeightRequest = 22;
            sortIcon.WidthRequest = sortIcon.HeightRequest;
            sortIcon.Margin = new Thickness(5, 10, 5, 0);
            sortIcon.GestureRecognizers.Add(new TapGestureRecognizer(OnSortClick));

            searchBackgroundBig = new Image
            {
                Source = ImageSource.FromResource(searchBackgroundBigSource),
                Aspect = Aspect.Fill,
                Margin = Device.OnPlatform(new Thickness(5, 0, 5, 0), new Thickness(20, 0, 20, 0), new Thickness(5, 0, 5, 0))
            };

            var searchBackground = new Image();
            searchBackground.Source = ImageSource.FromResource(searchBackgroundSource);
            searchBackground.HeightRequest = 40;
            searchBackground.Margin = new Thickness(5, 0, 5, 0);
            searchBackground.GestureRecognizers.Add(new TapGestureRecognizer(OnSearchCaregoryClick));

            var placeBackground = new Image();
            placeBackground.Source = ImageSource.FromResource(searchBackgroundSource);
            placeBackground.HeightRequest = 40;
            placeBackground.Margin = new Thickness(5, 0, 5, 0);

            placeLabel = new CustomLabel()
            {
                Text = "Anywhere at Anytime",
                TextColor = Color.Black,
                FontSize = 13,
                HorizontalTextAlignment = TextAlignment.Center,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR
            };
            var placeTap = new TapGestureRecognizer((v) =>
            {
                if (filersPage == null)
                {
                    filersPage = new SearchFilersPage();
                    filersPage.OnSelect += FilersPage_OnSelect;
                }                
                this.Navigation.PushAsync(filersPage);
            });
            placeBackground.GestureRecognizers.Add(placeTap);
            placeLabel.GestureRecognizers.Add(placeTap);

            var plus = new Image();
            plus.Source = ImageSource.FromResource("TiroApp.Images.plus.png");
            plus.HeightRequest = 15;
            plus.WidthRequest = plus.HeightRequest;
            plus.GestureRecognizers.Add(new TapGestureRecognizer(OnSearchCaregoryClick));

            var calendar = new Image();
            calendar.Source = ImageSource.FromResource("TiroApp.Images.calendar.png");
            calendar.HeightRequest = 20;
            calendar.WidthRequest = 22;

            var filter_1 = MakeCheckBox("Makeup");
            var filter_2 = MakeCheckBox("Bridal");
            var filter_3 = MakeCheckBox("Gele");
            var filter_4 = MakeCheckBox("Eyebrow");
            filters = new List<RadioButton> { filter_1, filter_2, filter_3, filter_4 };

            var topFilters = new RelativeLayout();
            topFilters.HeightRequest = 45;
            topFilters.WidthRequest = rl.WidthRequest;
            topFilters.Children.Add(filter_1
                , Constraint.Constant(30)
                , Constraint.RelativeToParent((p) => { return p.Height / 2 - Utils.GetControlSize(filter_1).Height / 2; }));
            topFilters.Children.Add(filter_2
                , Constraint.RelativeToParent((p) => { return p.Width / 2; })
                , Constraint.RelativeToParent((p) => { return p.Height / 2 - Utils.GetControlSize(filter_2).Height / 2; }));
            var bottomFilters = new RelativeLayout();
            bottomFilters.HeightRequest = 45;
            bottomFilters.WidthRequest = rl.WidthRequest;
            bottomFilters.Children.Add(filter_3
                , Constraint.Constant(30)
                , Constraint.RelativeToParent((p) => { return p.Height / 2 - Utils.GetControlSize(filter_3).Height / 2; }));
            bottomFilters.Children.Add(filter_4
                , Constraint.RelativeToParent((p) => { return p.Width / 2; })
                , Constraint.RelativeToParent((p) => { return p.Height / 2 - Utils.GetControlSize(filter_4).Height / 2; }));

            var searchButton = UIUtils.MakeButton("SEARCH", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            searchButton.Clicked += SearchButton_Clicked;
            filtersLayout = new StackLayout
            {
                IsVisible = false,
                BackgroundColor = Color.White,
                Children = {
                    topFilters,
                    UIUtils.MakeSeparator(true),
                    bottomFilters,
                    searchButton,
                    //new BoxView { Color = Props.BlackoutColor }
                }
            };

            

            rl.Children.Add(sortIcon
                , Constraint.RelativeToParent(p => { return p.Width - 40; })
                , Constraint.Constant(15));

            rl.Children.Add(searchBackground
               , Constraint.Constant(40)
               , Constraint.Constant(15)
               , Constraint.RelativeToParent(p => {
                   return p.Width - 80;
               }));

            searchBackgroundBig.IsVisible = false;
            rl.Children.Add(searchBackgroundBig
                , Constraint.Constant(40)
                , Constraint.Constant(15)
                , Constraint.RelativeToParent(p => {
                    return p.Width - 80;
                })
                , Constraint.Constant(60));

            rl.Children.Add(plus
                , Constraint.RelativeToParent(p => {
                    return p.Width - Device.OnPlatform(80, 100, 80);
                })
                , Constraint.Constant(25));

            rl.Children.Add(placeBackground
                , Constraint.Constant(40)
                , Constraint.Constant(75)
                , Constraint.RelativeToParent(p => {
                    return p.Width - 80;
                }));
            rl.Children.Add(placeLabel,
                 Constraint.Constant(45),
                 Constraint.Constant(Device.OnPlatform(85, 82, 85)),
                 Constraint.RelativeToParent(p => p.Width - 85));

            rl.Children.Add(calendar
                , Constraint.Constant(80)
                , Constraint.Constant(85));

            this.Children.Add(filtersLayout
                , Constraint.Constant(0)
                , Constraint.Constant(75)
                , Constraint.RelativeToParent(p => {
                    return p.Width;
                }));

            filtersBoxHolder = new StackLayout { Orientation = StackOrientation.Horizontal };
            filtersBoxHolderAdditional = new StackLayout { Orientation = StackOrientation.Horizontal };
            rl.Children.Add(filtersBoxHolder
                , Constraint.Constant(Device.OnPlatform(50, 80, 50))
                , Constraint.Constant(24));
            rl.Children.Add(filtersBoxHolderAdditional
                , Constraint.Constant(Device.OnPlatform(50, 80, 50))
                , Constraint.Constant(48));            
        }
        
        private void FilersPage_OnSelect(object sender, EventArgs e)
        {
            string t = "";
            if (filersPage.LocationAddress != null)
            {
                t = filersPage.LocationAddress;
            }
            else
            {
                t = "Current location";
            }
            switch (filersPage.AvailabilityMode)
            {
                case 1:
                    t += " at Anytime";
                    break;
                case 2:
                    t += " Today";
                    break;
                case 3:
                    t += $" From {filersPage.AvailibilityFilter.DatesFrom[0].ToString("HH:mm")} To {filersPage.AvailibilityFilter.DatesTo[0].ToString("HH:mm")}";
                    break;
            }
            placeLabel.Text = t;
            OnSearchClick?.Invoke(sender, e);
        }

        private void SearchButton_Clicked(object sender, EventArgs e)
        {
            OnSearchClick?.Invoke(sender, e);
        }

        private RadioButton MakeCheckBox(string text)
        {
            var checkBox = new RadioButton(false);
            checkBox.Text = text;
            checkBox.TextColor = Color.Black;
            checkBox.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            checkBox.FontSize = 18;
            checkBox.VerticalOptions = LayoutOptions.Center;
            checkBox.OnCheckedChange += (s, args) => {
                isFreeMakeover = false;
                RefreshSearchBox();
            };
            return checkBox;
        }

        public void RefreshSearchBox(string text = null)
        {
            filtersBoxHolder.Children.Clear();
            filtersBoxHolderAdditional.Children.Clear();
            if (isFreeMakeover)
            {
                var selectedFilterBox = MakeSelectedFilterBox(text);
                searchBackgroundBig.IsVisible = false;
                filtersBoxHolder.Children.Add(selectedFilterBox);
                this.ForceLayout();
                return;
            }
            if (!string.IsNullOrEmpty(text))
            {
                foreach (var filter in filters)
                {
                    if (filter.IsChecked && filter.Text.Equals(text))
                    {
                        filter.IsChecked = !filter.IsChecked;
                    }
                }
            }
            List<Frame> filterBoxes = new List<Frame>();
            foreach (var filter in filters)
            {
                if (filter.IsChecked)
                {
                    var selectedFilterBox = MakeSelectedFilterBox(filter.Text);
                    filterBoxes.Add(selectedFilterBox);
                }
            }
            if (filterBoxes.Count <= 2)
            {
                searchBackgroundBig.IsVisible = false;
                foreach (var selectedFilter in filterBoxes)
                {
                    filtersBoxHolder.Children.Add(selectedFilter);
                }
            }
            else
            {
                searchBackgroundBig.IsVisible = true;
                for (int i = 0; i < filterBoxes.Count; i++)
                {
                    if (i < filterBoxes.Count / 2)
                    {
                        filtersBoxHolder.Children.Add(filterBoxes[i]);
                    }
                    else
                    {
                        filtersBoxHolderAdditional.Children.Add(filterBoxes[i]);
                    }
                }
                //mainLayout.Children.Add(filtersBoxHolderAdditional
                //, Constraint.Constant(50)
                //, Constraint.Constant(50));
            }
            //mainLayout.Children.Add(filtersBoxHolder
            //    , Constraint.Constant(50)
            //    , Constraint.Constant(24));
            this.ForceLayout();
        }

        private Frame MakeSelectedFilterBox(string text)
        {
            var close = new Image();
            close.Source = ImageSource.FromResource("TiroApp.Images.close.png");
            close.HeightRequest = 14;
            close.Margin = new Thickness(2, 2, 2, 2);
            close.WidthRequest = close.HeightRequest;
            //close.GestureRecognizers.Add(new TapGestureRecognizer((v) => {
            //    RefreshSearchBox(text);
            //}));
            var layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                Children = {
                    close,
                    new CustomLabel { Text = text, TextColor = Color.Black, FontSize = 14 }
                }
            };
            layout.GestureRecognizers.Add(new TapGestureRecognizer((v) => {
                RefreshSearchBox(text);
            }));
            var frame = new Frame
            {
                OutlineColor = Color.Black,
                Margin = new Thickness(5, 0, 5, 0),
                Padding = new Thickness(2, 1, 4, 1),
                HasShadow = false,
                Content = layout
            };
            return frame;
        }

        private void OnSearchCaregoryClick(View v)
        {
            if (isFreeMakeover)
            {
                ConfirmDialog.Show(this.ParentPage, "Services other than Free Makeover will not be free",
                    new List<string> { "Go Back", "Continue" }, (bIndex) =>
                    {
                        if (bIndex == 1)
                        {
                            isFreeMakeover = false;
                            filtersLayout.IsVisible = true;
                        }
                    });
                return;
            }
            filtersLayout.IsVisible = !filtersLayout.IsVisible;
        }

        public bool IsFreeMakeover
        {
            get
            {
                return isFreeMakeover;
            }
            set
            {
                isFreeMakeover = value;
                if (isFreeMakeover)
                {
                    filtersLayout.IsVisible = false;
                    RefreshSearchBox("Free Makeover");
                }
            }
        }

        public ContentPage ParentPage { get; set; }

        public static bool IsFreeMakeoverL
        {
            get
            {
                return isFreeMakeover;
            }
        }

        private void OnSortClick(View arg1, object arg2)
        {
            var main = ParentPage.Content as RelativeLayout;
            if (main == null)
                return;
            if (sortContainer == null)
            {
                sortContainer = new RelativeLayout();
                sortContainer.BackgroundColor = Props.BlackoutColor;
                main.Children.Add(sortContainer, Constraint.Constant(0), Constraint.Constant(0),
                    Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));

                var sl = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(40, 20),
                    BackgroundColor = Color.White,
                    WidthRequest = 200
                };
                sortContainer.Children.Add(sl,
                    Constraint.RelativeToParent(p => { return (p.Width - sl.Width) / 2; }),
                    Constraint.RelativeToParent(p => { return (p.Height - sl.Height) / 2; }));

                var title = new CustomLabel()
                {
                    Text = "Sort By",
                    TextColor = Color.Black,
                    FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Start
                };
                sl.Children.Add(title);

                var list = new List<RadioButton>();
                sl.Children.Add(MakeSortCheckBox("Ratings", SearchSortType.Rating, list));
                sl.Children.Add(MakeSortCheckBox("Nearest", SearchSortType.Nearest, list));
                sl.Children.Add(MakeSortCheckBox("Highest Price", SearchSortType.HighestPrice, list));
                sl.Children.Add(MakeSortCheckBox("Lowest Price", SearchSortType.LowestPrice, list));
                list[0].IsChecked = true;

                var cancel = new CustomLabel()
                {
                    Text = "Cancel",
                    TextColor = Props.ButtonColor,
                    FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };
                cancel.GestureRecognizers.Add(new TapGestureRecognizer(v => { sortContainer.IsVisible = false; }));
                sl.Children.Add(cancel);

            }
            sortContainer.IsVisible = true;
        }

        private RadioButton MakeSortCheckBox(string text, SearchSortType type, List<RadioButton> list)
        {
            var checkBox = new RadioButton(false);
            checkBox.Text = text;
            checkBox.TextColor = Color.Black;
            checkBox.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            checkBox.FontSize = 16;
            checkBox.HeightRequest = 50;
            checkBox.VerticalOptions = LayoutOptions.Center;
            checkBox.SetValue(UIUtils.TagProperty, type);
            list.Add(checkBox);
            checkBox.OnCheckedChange += (s, args) =>
            {
                foreach (var cb in list)
                {
                    if (s == cb)
                    {
                        SearchSort = (SearchSortType)cb.GetValue(UIUtils.TagProperty);
                        sortContainer.IsVisible = false;
                        OnSortChanged?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        cb.IsChecked = false;
                    }
                }
            };
            return checkBox;
        }

        public SearchSortType SearchSort { get; set; } = SearchSortType.Rating;
    }

    public enum SearchSortType
    {
        Rating,
        Nearest,
        HighestPrice,
        LowestPrice
    }
}
