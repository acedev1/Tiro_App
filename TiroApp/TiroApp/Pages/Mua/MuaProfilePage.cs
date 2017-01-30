using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public class MuaProfilePage : ContentPage
    {
        private const int ColumnNumber = 4;

        private RelativeLayout root;
        private AbsoluteLayout confirmationWindow;
        private Grid grid;
        private ActivityIndicator spinner;
        private string selectedImage;
        private MuaArtist _mua;

        public MuaProfilePage(JObject jObj)
        {
            BackgroundColor = Color.White;
            Utils.SetupPage(this);
            _mua = new MuaArtist(jObj, true);
            _mua.Images = _mua.Images.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            BuildLayout();
        }

        private void BuildLayout()
        {
            root = new RelativeLayout();

            var header = UIUtils.MakeHeader(this, "My Portfolio");

            var separator = UIUtils.MakeSeparator(true);

            var infoLabel = new CustomLabel {
                Text = "#TIRO to add a photo",
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                Margin = new Thickness(0, 10, 0, 10)
            };

            grid = new Grid {
                ColumnSpacing = 5,
                RowSpacing = 5,
                Margin = new Thickness(5, 0, 5, 0)
            };
            var reloadButton = UIUtils.MakeButton("Refresh from instagram", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            reloadButton.Margin = new Thickness(0, 5, 0, 0);
            reloadButton.VerticalOptions = LayoutOptions.EndAndExpand;
            reloadButton.Clicked += OnReloadClicked;
            selectedImage = _mua.DefaultPicture;
            //if (_mua.Images.Count == 0)
            //{
            //    //TODO
            //    var empty = new StackLayout {
            //        Spacing = 0,
            //        Children = { header, separator, infoLabel, reloadButton }
            //    };
            //    root.Children.Add(empty, Constraint.Constant(0), Constraint.Constant(0)
            //    , Constraint.RelativeToParent(p => p.Width)
            //    , Constraint.RelativeToParent(p => p.Height));
            //    Content = root;
            //    return;
            //}            
            for (int i = 0; i < ColumnNumber; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            FillGrid();
            var scrollView = new ScrollView {
                Content = grid
            };
            
            var main = new StackLayout {
                Spacing = 0,
                Children = { header, separator, infoLabel, scrollView, reloadButton }
            };

            confirmationWindow = MakeConfirmWindow();

            root.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));
            root.Children.Add(confirmationWindow, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));

            Content = root;
        }

        private void FillGrid()
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            for (int i = 0; i < Math.Ceiling((double)_mua.Images.Count / ColumnNumber); i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = 80 });
            }
            for (int i = 0, col = 0, row = 0; i < _mua.Images.Count; i++, col++)
            {
                if (col == ColumnNumber)
                {
                    col = 0;
                    row++;
                }
                var imageStr = _mua.Images[i];
                var image = new Image();
                image.Aspect = Aspect.AspectFill;
                try
                {
                    image.Source = ImageSource.FromUri(new Uri(imageStr));
                }
                catch { }
                //image.Source = ImageSource.FromResource("TiroApp.Images.empty_profile.jpg");
                image.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                    selectedImage = imageStr;
                    confirmationWindow.IsVisible = true;
                }));
                if (!string.IsNullOrEmpty(selectedImage) && imageStr == selectedImage)
                {
                    var frame = new Frame
                    {
                        Padding = 3,
                        OutlineColor = Color.Red,
                        BackgroundColor = Color.Red,
                        Content = image
                    };
                    grid.Children.Add(frame, col, row);
                }
                else
                {
                    grid.Children.Add(image, col, row);
                }
            }
        }

        private AbsoluteLayout MakeConfirmWindow()
        {
            var confirmWindowHolder = new AbsoluteLayout();
            confirmWindowHolder.IsVisible = false;

            var confirmLabel = new CustomLabel {
                Text = "Do you want to make this the\r\nmain image that customers see\r\non your profile listing?",
                FontSize = 12,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                Margin = new Thickness(40, 30, 40, 30)
            };

            var noBtn = UIUtils.MakeButton("NO", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            noBtn.TextColor = Props.ButtonColor;
            noBtn.BackgroundColor = Color.FromHex("F8F8F8");
            noBtn.HeightRequest = 55;
            noBtn.WidthRequest = 80;
            noBtn.Clicked += (o, a) => { confirmWindowHolder.IsVisible = false; };
            var yesBtn = UIUtils.MakeButton("YES", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            yesBtn.HeightRequest = 55;
            yesBtn.WidthRequest = 80;
            yesBtn.Clicked += YesBtn_Clicked;

            var confirmWindow =  new StackLayout {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = {
                    confirmLabel,
                    new StackLayout {
                        Spacing = 0,
                        Orientation = StackOrientation.Horizontal,
                        Children = { noBtn, yesBtn }
                    }
                }
            };

            confirmWindowHolder.BackgroundColor = Props.BlackoutColor;
            AbsoluteLayout.SetLayoutFlags(confirmWindow, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(confirmWindow, new Rectangle(0.5, 0.3, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            confirmWindowHolder.Children.Add(confirmWindow);
            return confirmWindowHolder;
        }

        private void YesBtn_Clicked(object sender, EventArgs e)
        {
            confirmationWindow.IsVisible = false;
            spinner = UIUtils.ShowSpinner(this);
            DataGate.SetDefaultPicture(GlobalStorage.Settings.MuaId, selectedImage, resp => {
                if (resp.Code == ResponseCode.OK && resp.Result == "true")
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        FillGrid();
                    });
                }
                else
                {
                    selectedImage = string.Empty;
                    UIUtils.ShowServerUnavailable(this);
                }
            });
            Device.BeginInvokeOnMainThread(() =>
            {
                UIUtils.HideSpinner(this, spinner);
            });
        }
        
        private void OnReloadClicked(object sender, EventArgs e)
        {
            var spinner = UIUtils.ShowSpinner(this);
            var instagramHelper = new InstagramHelper(_mua.Instagram, this);
            instagramHelper.OnImagesLoad += (s, instagramStr) =>
            {
                if (!string.IsNullOrEmpty(instagramStr))
                {
                    DataGate.MuaUpdatePictures(_mua.Id, instagramStr.Split(','), r =>
                    {
                        if (r.Code == ResponseCode.OK && r.Result == "true")
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                _mua.Images = instagramStr.Split(',').ToList();
                                FillGrid();
                            });
                        }
                        else
                        {
                            UIUtils.ShowServerUnavailable(this);
                        }
                        UIUtils.HideSpinner(this, spinner);
                    });
                }
                else
                {
                    UIUtils.HideSpinner(this, spinner);
                }
            };
            instagramHelper.Start();
        }
    }
}
