using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public class MuaAddServicePage : ContentPage
    {
        private CustomEntry nameEntry;
        private CustomEntry lengthEntry;
        private CustomEntry priceEntry;
        private List<RadioButton> categories;
        private RadioButton rbSelected;
        private ActivityIndicator spinner;
        private Service service;
        private bool isEditMode = false;

        public MuaAddServicePage()
        {
            Utils.SetupPage(this);
            BuildLayout();
        }

        public MuaAddServicePage(Service service)
        {
            Utils.SetupPage(this);
            this.service = service;
            this.isEditMode = true;
            BuildLayout();

            nameEntry.Text = service.Name;
            lengthEntry.Text = service.Length;
            priceEntry.Text = service.Price.ToString(Utils.EnCulture);
            rbSelected = categories.Find(c => c.Text == service.Category);
            rbSelected.IsChecked = true;
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

            BuildHeader(main, isEditMode ? "Edit Service" : "Add Service");

            nameEntry = UIUtils.MakeEntry("Name of Service", UIUtils.FONT_SFUIDISPLAY_BOLD);
            nameEntry.Margin = new Thickness(40, 20, 40, 0);
            main.Children.Add(nameEntry);
            main.Children.Add(UIUtils.MakeSeparator());

            lengthEntry = UIUtils.MakeEntry("Length of Time", UIUtils.FONT_SFUIDISPLAY_BOLD);            
            lengthEntry.Margin = new Thickness(40, 10, 40, 0);
            lengthEntry.Keyboard = Keyboard.Numeric;
            main.Children.Add(lengthEntry);
            main.Children.Add(UIUtils.MakeSeparator());

            priceEntry = UIUtils.MakeEntry("Price", UIUtils.FONT_SFUIDISPLAY_BOLD);
            priceEntry.Margin = new Thickness(40, 10, 40, 0);
            lengthEntry.Keyboard = Keyboard.Numeric;
            main.Children.Add(priceEntry);
            main.Children.Add(UIUtils.MakeSeparator());

            var label = new CustomLabel()
            {
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HeightRequest = 45,
                FontSize = 17,
                Margin = new Thickness(40, 10, 40, 0),
                TextColor = Color.FromHex("787878"),
                Text = "Category"
            };
            main.Children.Add(label);

            var filter_1 = MakeCheckBox("Makeup");            
            var filter_2 = MakeCheckBox("Bridal");
            var filter_3 = MakeCheckBox("Gele");
            var filter_4 = MakeCheckBox("Eyebrow");
            categories = new List<RadioButton> { filter_1, filter_2, filter_3, filter_4 };
            rbSelected = filter_1;
            if (!isEditMode)
            {
                filter_1.IsChecked = true;
            }
            main.Children.Add(new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 45,
                Children = { filter_1, filter_2 }
            });
            main.Children.Add(new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 45,
                Children = { filter_3, filter_4 }
            });

            var btn = UIUtils.MakeButton(isEditMode ? "EDIT" : "ADD", UIUtils.FONT_BEBAS_REGULAR);
            btn.VerticalOptions = LayoutOptions.EndAndExpand;
            btn.Clicked += OnAddServiceClick;
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
            }));
            var headerLabel = new CustomLabel();
            headerLabel.Text = text;
            headerLabel.TextColor = Color.Black;
            headerLabel.BackgroundColor = Color.White;
            headerLabel.HorizontalTextAlignment = TextAlignment.Center;
            headerLabel.VerticalTextAlignment = TextAlignment.Center;
            headerLabel.HeightRequest = 50;
            headerLabel.FontSize = 17;
            headerLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
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

        private RadioButton MakeCheckBox(string text)
        {
            var checkBox = new RadioButton(false);
            checkBox.Text = text;
            checkBox.TextColor = Color.Black;
            checkBox.FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT;
            checkBox.FontSize = 18;
            checkBox.VerticalOptions = LayoutOptions.Center;
            checkBox.HorizontalOptions = LayoutOptions.FillAndExpand;
            checkBox.WidthRequest = 250;
            checkBox.OnCheckedChange += (s, a) =>
            {
                rbSelected = s as RadioButton;
                foreach (var cb in categories)
                {
                    if (cb != s)
                    {
                        cb.IsChecked = false;
                    }
                }
            };
            return checkBox;
        }

        private void OnAddServiceClick(object sender, EventArgs e)
        {
            if (!UIUtils.ValidateEntriesWithEmpty(new List<Entry> { nameEntry, lengthEntry, priceEntry }, this))
            {
                return;
            }            
            var muaId = GlobalStorage.Settings.MuaId;
            int length = 0;
            double price = 0;
            int.TryParse(lengthEntry.Text, out length);
            double.TryParse(priceEntry.Text, out price);
            if (length == 0 || price == 0)
            {
                UIUtils.ShowMessage("Price or length is not valid", this);
                return;
            }
            spinner = UIUtils.ShowSpinner(this);
            var callback = new Action<ResponseDataJson>(resp =>
            {
                UIUtils.HideSpinner(this, spinner);
                if (resp.Code == ResponseCode.OK && resp.Result == "true")
                {
                    UIUtils.ShowMessage($"Service was {(isEditMode ? "changed" : "added")} successfully.", this);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.Navigation.PopAsync();
                    });
                }
                else
                {
                    UIUtils.ShowMessage($"Service was not {(isEditMode ? "changed" : "added")}. Try later.", this);
                }
            });
            if (isEditMode)
            {
                DataGate.ServiceUpdate(service.Id, nameEntry.Text, price, length, rbSelected.Text, callback);
            }
            else
            {
                DataGate.ServiceAdd(muaId, nameEntry.Text, price, length, rbSelected.Text, callback);
            }
        }
    }
}
