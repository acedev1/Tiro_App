using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public class MuaServicesPage : ContentPage
    {
        private List<Service> serviceList = new List<Service>();
        private StackLayout servicesHolder;
        //private ListView list;

        public MuaServicesPage()
        {
            Utils.SetupPage(this);
            BuildLayout();
        }

        private void RefreshList()
        {
            var spinner = UIUtils.ShowSpinner(this);
            DataGate.GetMua(GlobalStorage.Settings.MuaId, data =>
            {
                if (data.Code == ResponseCode.OK)
                {
                    serviceList.Clear();
                    var jObj = JObject.Parse(data.Result);
                    if (jObj["Services"] != null)
                    {
                        foreach (JObject service in (JArray)jObj["Services"])
                        {
                            serviceList.Add(new Service(service, true));
                        }
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        BuildServicesList();
                    });
                }
                UIUtils.HideSpinner(this, spinner);
            });
        }

        private void BuildServicesList()
        {
            var categories = new Dictionary<string, List<Service>>();
            foreach (var service in serviceList)
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

            servicesHolder.Children.Clear();
            var oCategories = categories.OrderBy(kv => kv.Key);
            foreach (var category in oCategories)
            {
                servicesHolder.Children.Add(new StackLayout
                {
                    BackgroundColor = Props.GrayColor,
                    Padding = new Thickness(0, 5, 0, 5),
                    Spacing = 0,
                    Children = {
                        new CustomLabel {
                            Text = category.Key,
                            HorizontalOptions = LayoutOptions.CenterAndExpand,
                            TextColor = Color.Gray,
                            FontFamily = UIUtils.FONT_SFUIDISPLAY_LIGHT
                        }
                    }
                });
                var oServices = category.Value.OrderBy(s => s.Name);
                foreach (var service in oServices)
                {
                    servicesHolder.Children.Add(MakeServiceRow(service));
                }
            }
        }

        private StackLayout MakeServiceRow(Service service)
        {
            var name = new CustomLabel();
            name.Text = service.Name;
            name.Margin = new Thickness(20, 10, 0, 0);
            name.TextColor = Color.Black;
            name.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            name.FontSize = 16;

            var price = new CustomLabel();
            var length = service.Length;
            price.TextColor = Color.FromHex("CCCCCC");
            price.Text = UIUtils.NIARA_SIGN + service.Price + " and up for " + length + " minutes";
            price.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            price.Margin = new Thickness(20, 0, 0, 0);            

            var editButton = UIUtils.MakeButton("Edit", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            var deleteButton = UIUtils.MakeButton("Delete", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            editButton.BackgroundColor = Color.FromHex("C8C7CD");
            editButton.WidthRequest = 90;            
            deleteButton.WidthRequest = 100;
            editButton.SetValue(UIUtils.TagProperty, service.Id);
            deleteButton.SetValue(UIUtils.TagProperty, service.Id);
            editButton.Clicked += OnEditClick;
            deleteButton.Clicked += OnDeleteClick;

            var info = new StackLayout
            {
                BackgroundColor = Color.White,
                HeightRequest = editButton.HeightRequest,
                Children = { name, price }
            };

            var infoLayout = new RelativeLayout();
            infoLayout.Children.Add(deleteButton, Constraint.RelativeToParent(p => p.Width - deleteButton.WidthRequest));
            infoLayout.Children.Add(editButton, Constraint.RelativeToParent(p => p.Width - deleteButton.WidthRequest - editButton.WidthRequest));
            infoLayout.Children.Add(info, Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent(p => p.Width));

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
                        if (panDX < 0)
                        {
                            info.TranslateTo(-(editButton.WidthRequest + deleteButton.WidthRequest), 0);
                        }
                        else
                        {
                            info.TranslateTo(0, 0);
                        }
                        break;
                }
            };
            info.GestureRecognizers.Add(pan);

            var line = UIUtils.MakeSeparator(true);
            var layout = new StackLayout
            {
                Spacing = 0,
                Children = { infoLayout, line }
            };
            return layout;
        }

        private void BuildLayout()
        {
            var main = new StackLayout();
            main.BackgroundColor = Color.White;
            main.Spacing = 0;
            var rl = new RelativeLayout();
            rl.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
            this.Content = rl;

            var header = UIUtils.MakeHeader(this, "My Services");

            var addButton = UIUtils.MakeButton("ADD A SERVICE +", UIUtils.FONT_BEBAS_REGULAR);
            addButton.VerticalOptions = LayoutOptions.EndAndExpand;
            addButton.Clicked += AddButton_Clicked;
            
            servicesHolder = new StackLayout();
            servicesHolder.Spacing = 0;
            var scrollView = new ScrollView();
            scrollView.Content = servicesHolder;            

            main.Children.Add(header);
            main.Children.Add(UIUtils.MakeSeparator(true));
            main.Children.Add(addButton);
            main.Children.Add(scrollView);
            scrollView.HeightRequest = App.ScreenHeight - Utils.GetControlSize(header).Height - Utils.GetControlSize(addButton).Height - 1;
            main.ForceLayout();
        }

        private void OnDeleteClick(object sender, EventArgs e)
        {
            ConfirmDialog.Show(this, "Do you want to delete service?", new List<string> { "Yes", "No" }, index =>
            {
                if (index == 0)
                {
                    var serviceId = ((Button)sender).GetValue(UIUtils.TagProperty).ToString();
                    var spinner = UIUtils.ShowSpinner(this);
                    DataGate.ServiceDelete(serviceId, data =>
                    {
                        UIUtils.HideSpinner(this, spinner);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            RefreshList();
                        });
                    });
                }
            });
        }

        private void OnEditClick(object sender, EventArgs e)
        {
            var serviceId = ((Button)sender).GetValue(UIUtils.TagProperty).ToString();
            var service = serviceList.Find(s => s.Id == serviceId);
            Navigation.PushAsync(new MuaAddServicePage(service));
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new MuaAddServicePage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshList();
        }
    }

    class ServiceViewCell : ViewCell
    {
        public ServiceViewCell(EventHandler OnEditClick, EventHandler OnDeleteClick)
        {
            var nameLabel = new CustomLabel();
            nameLabel.TextColor = Color.Black;
            nameLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            nameLabel.FontSize = 16;
            nameLabel.HorizontalOptions = LayoutOptions.Start;
            nameLabel.SetBinding(Label.TextProperty, "Name", stringFormat: "Name: {0}");
            var priceLabel = new CustomLabel();
            priceLabel.TextColor = Color.Black;
            priceLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            priceLabel.FontSize = 16;
            priceLabel.HorizontalOptions = LayoutOptions.EndAndExpand;
            priceLabel.SetBinding(Label.TextProperty, "Price", stringFormat: UIUtils.NIARA_SIGN + "{0}");
            var row1 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 5, 20, 5),
                Children = { nameLabel, priceLabel }
            };

            var categoryLabel = new CustomLabel();
            categoryLabel.TextColor = Color.Black;
            categoryLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            categoryLabel.FontSize = 16;
            categoryLabel.HorizontalOptions = LayoutOptions.Start;
            categoryLabel.SetBinding(Label.TextProperty, "Category");
            var lengthLabel = new CustomLabel();
            lengthLabel.TextColor = Color.Black;
            lengthLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            lengthLabel.FontSize = 16;
            lengthLabel.HorizontalOptions = LayoutOptions.EndAndExpand;
            lengthLabel.SetBinding(Label.TextProperty, "Length", stringFormat: "{0} minutes");
            var row2 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 5, 20, 5),
                Children = { categoryLabel, lengthLabel }
            };

            var btn1 = UIUtils.MakeButton("EDIT", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            btn1.TextColor = Props.ButtonColor;
            btn1.BackgroundColor = Color.FromHex("F8F8F8");
            btn1.SetBinding(UIUtils.TagProperty, "Id");
            btn1.Clicked += (o, a) => { OnEditClick?.Invoke(o, a); };
            var btn2 = UIUtils.MakeButton("DELETE", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            btn2.SetBinding(UIUtils.TagProperty, "Id");
            btn2.Clicked += (o, a) => { OnDeleteClick?.Invoke(o, a); };
            var row3 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0,
                Children = { btn1, btn2 }
            };

            var content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                Children = { row1, row2, row3, UIUtils.MakeSeparator(true) }
            };
            this.View = content;
        }
    }
}
