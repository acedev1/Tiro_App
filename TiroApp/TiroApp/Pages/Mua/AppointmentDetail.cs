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
    public class AppointmentDetail : ContentPage
    {
        private RelativeLayout root;
        private Order _order;
        private const double ScaleCustomerImage = 0.8;
        private StackLayout infoLayout;

        public AppointmentDetail(Order order)
        {
            _order = order;
            Utils.SetupPage(this);
            BuildLayout();
            if (order.DateTime < DateTime.Now)
            {
                LoadReview();
            }
        }

        private void BuildLayout()
        {
            var header = UIUtils.MakeHeader(this, "Appointment Details");

            var customerImage = new Image();
            customerImage.Source = _order.Customer.CustomerImage;
            customerImage.Aspect = Aspect.AspectFill;

            var name = new CustomLabel();
            name.Text = _order.Customer.FullName;
            name.FontSize = 22;
            name.TextColor = Color.White;
            name.FontFamily = UIUtils.FONT_BEBAS_REGULAR;

            var services = new CustomLabel();
            services.Text = _order.Basket.Select(s => s.Service.Name).Aggregate((i, j) => i + " | " + j);
            services.TextColor = Color.Black;
            services.FontSize = 16;
            services.Margin = new Thickness(20, 10, 20, 0);
            services.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;

            var address = new CustomLabel();
            address.Text = _order.MuaAddress;
            address.TextColor = Color.Gray;
            address.FontSize = 14;
            address.HorizontalOptions = LayoutOptions.StartAndExpand;
            address.VerticalOptions = LayoutOptions.Center;
            address.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;

            var dateTime = new CustomLabel();
            dateTime.Text = _order.DateTime.ToString("dd.MM.yyyy hh:mm tt", Utils.EnCulture);
            dateTime.TextColor = Props.ButtonColor;
            dateTime.FontSize = 14;
            dateTime.HorizontalOptions = LayoutOptions.EndAndExpand;
            dateTime.VerticalOptions = LayoutOptions.Center;
            dateTime.FontFamily = UIUtils.FONT_BEBAS_REGULAR;

            var addressTimeHolder = new StackLayout {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(20, 10, 20, 20),
                Children = { address, dateTime }
            };

            var note = new CustomLabel();
            note.Text = _order.Note;
            //note.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed to eiusmod tempor incididunt ut labore "
            //    + "et dolore magna aliqua. Ut enim ad minim veniam quis nostrud.";
            note.TextColor = Color.Black;
            note.Margin = new Thickness(20, 20, 20, 0);
            note.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;

            infoLayout = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = { services, addressTimeHolder, note }
            };

            var rl = new RelativeLayout();
            rl.Children.Add(customerImage
                , Constraint.Constant(0)
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Width * ScaleCustomerImage));
            rl.Children.Add(name
                , Constraint.RelativeToParent(p => (p.Width - Utils.GetControlSize(name).Width) / 2)
                , Constraint.RelativeToParent(p => p.Width * (ScaleCustomerImage - 0.1)));
            rl.Children.Add(infoLayout
                , Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width * ScaleCustomerImage)
                , Constraint.RelativeToParent(p => p.Width));

            var scrollView = new ScrollView { Content = rl };

            var main = new StackLayout
            {
                Spacing = 0,
                BackgroundColor = Color.White,
                Children = { header, scrollView }
            };

            root = new RelativeLayout();
            root.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));

            Content = root;
        }

        private void LoadReview()
        {
            DataGate.GetReview(GlobalStorage.Settings.MuaId, _order.Customer.Id, r =>
            {
                if (r.Code == ResponseCode.OK)
                {
                    try
                    {
                        var reviewArr = JArray.Parse(r.Result);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (reviewArr.Count != 0)
                            {
                                BuildReviews(reviewArr);
                            }
                        });
                    }
                    catch { }
                }
            });
        }

        private void BuildReviews(JArray reviewArr)
        {
            infoLayout.Children.RemoveAt(infoLayout.Children.Count - 1);
            infoLayout.Children.Add(UIUtils.MakeSeparator(true));
            foreach (var review in reviewArr)
            {
                var reviewLabel = new CustomLabel()
                {
                    Text = "REVIEW:",
                    FontSize = 22,
                    TextColor = Color.Black,
                    VerticalOptions = LayoutOptions.Center,
                    FontFamily = UIUtils.FONT_BEBAS_REGULAR,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };
                var rating = new RatingLayout()
                {
                    Rating = (int)review["Rating"],
                    IsEditable = false,
                    HeightRequest = 30,
                    HorizontalOptions = LayoutOptions.End
                };
                var row1 = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Fill,
                    Margin = new Thickness(20, 20, 20, 10),
                    Children = { reviewLabel, rating }
                };
                infoLayout.Children.Add(row1);
                var reviewText = new CustomLabel()
                {
                    Text = (string)review["Description"],
                    TextColor = Color.Black,
                    FontSize = 14,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                    Margin = new Thickness(20, 0, 20, 20)
                };
                infoLayout.Children.Add(reviewText);
            }
        }
    }
}
