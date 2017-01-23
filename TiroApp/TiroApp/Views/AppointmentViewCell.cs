using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    class AppointmentViewCell : ViewCell
    {
        private EventHandler OnConfirmClick;
        private EventHandler OnDeclineClick;
        public EventHandler OnRescheduleClick;
        public EventHandler OnCancelClick;
        private bool _isMua;

        public AppointmentViewCell(bool isMua = false)
        {
            _isMua = isMua;
            BuildView();
        }

        public AppointmentViewCell(EventHandler onDeclineClick, EventHandler onConfirmClick) : this(true)
        {
            _isMua = true;
            this.OnDeclineClick = onDeclineClick;
            this.OnConfirmClick = onConfirmClick;
        }

        public AppointmentViewCell(bool isMua, EventHandler onRescheduleClick, EventHandler onCancelClick) : this()
        {
            _isMua = false;
            this.OnRescheduleClick = onRescheduleClick;
            this.OnCancelClick = onCancelClick;
        }

        public bool IsWithButtons { get; set; }
        private void BuildView()
        {
            var img = new CircleImage();
            img.WidthRequest = 55;
            img.HeightRequest = img.WidthRequest;
            img.Aspect = Aspect.AspectFill;
            img.SetBinding(Image.SourceProperty, "Image");
            img.VerticalOptions = LayoutOptions.Fill;
            img.HorizontalOptions = LayoutOptions.Start;

            var nameLabel = new CustomLabel();
            nameLabel.SetBinding(Label.TextProperty, "Name");
            nameLabel.VerticalOptions = LayoutOptions.Center;
            nameLabel.HorizontalOptions = LayoutOptions.Start;
            nameLabel.TextColor = Color.Black;
            nameLabel.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            nameLabel.FontSize = 18;

            var row1 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 65,
                Padding = new Thickness(10, 10, 20, 0),
                Children = { img, nameLabel }
            };

            if (_isMua)
            {
                var priceLabel = new CustomLabel();
                priceLabel.SetBinding(Label.TextProperty, "TotalPrice");
                priceLabel.VerticalOptions = LayoutOptions.Center;
                priceLabel.HorizontalOptions = LayoutOptions.EndAndExpand;
                priceLabel.TextColor = Color.Black;
                priceLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_SEMIBOLD;
                priceLabel.FontSize = 16;
                row1.Children.Add(priceLabel);
            }
            else
            {
                var confirmed = new Image();
                //confirmed.Source = ImageSource.FromResource("TiroApp.Images.confirmed.png");
                confirmed.SetBinding(Image.SourceProperty, "ConfirmedImageSource");
                confirmed.HorizontalOptions = LayoutOptions.EndAndExpand;
                confirmed.VerticalOptions = LayoutOptions.Center;
                confirmed.HeightRequest = 50;
                confirmed.WidthRequest = 70;
                confirmed.Margin = new Thickness(0, 0, 20, 10);
                confirmed.SetBinding(VisualElement.IsVisibleProperty, "IsStatusNotNew");
                row1.Children.Add(confirmed);
            }

            var row2 = new CustomLabel();
            row2.SetBinding(Label.TextProperty, "ServicesName");
            row2.VerticalOptions = LayoutOptions.Center;
            row2.HorizontalOptions = LayoutOptions.Start;
            row2.TextColor = Color.Black;
            row2.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            row2.FontSize = 16;
            row2.Margin = new Thickness(10, 0, 0, 0);

            var addrLabel = new CustomLabel();
            addrLabel.SetBinding(Label.TextProperty, "Address");
            addrLabel.VerticalOptions = LayoutOptions.Center;
            addrLabel.HorizontalOptions = LayoutOptions.Start;
            addrLabel.TextColor = Color.FromHex("CCCCCC");
            addrLabel.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
            addrLabel.FontSize = 14;

            var dateLabel = new CustomLabel();
            dateLabel.SetBinding(Label.TextProperty, "Date");
            dateLabel.VerticalOptions = LayoutOptions.Center;
            dateLabel.HorizontalOptions = LayoutOptions.EndAndExpand;
            dateLabel.TextColor = Props.ButtonColor;
            dateLabel.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            dateLabel.FontSize = 14;

            var row3 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 30,
                Padding = new Thickness(10, 0, 20, 0),
                Children = { addrLabel, dateLabel }
            };

            var l = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0
            };
            l.Children.Add(row1);
            l.Children.Add(row2);
            l.Children.Add(row3);
            if (_isMua)
            {
                var btn1 = UIUtils.MakeButton("DECLINE", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
                btn1.TextColor = Props.ButtonColor;
                btn1.BackgroundColor = Color.FromHex("F8F8F8");
                btn1.SetBinding(UIUtils.TagProperty, "Id");
                btn1.Clicked += (o, a) => { OnDeclineClick?.Invoke(o, a); };
                var btn2 = UIUtils.MakeButton("CONFIRM", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
                btn2.SetBinding(UIUtils.TagProperty, "Id");
                btn2.Clicked += (o, a) => { OnConfirmClick?.Invoke(o, a); };
                var row4 = new StackLayout() {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Spacing = 0,
                    Children = { btn1, btn2 }
                };
                row4.SetBinding(VisualElement.IsVisibleProperty, "IsStatusNew");
                l.Children.Add(row4);
            }
            else
            {
                var btn1 = UIUtils.MakeButton("RESCHEDULE", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
                btn1.TextColor = Props.ButtonColor;
                btn1.BackgroundColor = Color.FromHex("F8F8F8");
                btn1.SetBinding(UIUtils.TagProperty, "Id");
                btn1.Clicked += (o, a) => { OnRescheduleClick?.Invoke(o, a); };
                var btn2 = UIUtils.MakeButton("CANCEL", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
                btn2.SetBinding(UIUtils.TagProperty, "Id");
                btn2.Clicked += (o, a) => { OnCancelClick?.Invoke(o, a); };
                var row4 = new StackLayout() {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Spacing = 0,
                    Children = { btn1, btn2 }
                };
                row4.SetBinding(VisualElement.IsVisibleProperty, "IsStatusNewAndNotPast");
                l.Children.Add(row4);
            }

            var separator = UIUtils.MakeSeparator(true);
            l.Children.Add(separator);

            var al = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutFlags(l, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(l, new Rectangle(0f, 0f, 1f, 1f));
            al.Children.Add(l);
            if (!_isMua)
            {
                var overlayText = new CustomLabel();
                overlayText.Text = "Pending Confirmation from \nMakeup artist";
                overlayText.FontFamily = UIUtils.FONT_SFUIDISPLAY_BOLD;
                overlayText.FontSize = 16;
                overlayText.TextColor = Props.ButtonInfoPageColor;
                overlayText.HorizontalTextAlignment = TextAlignment.Center;
                overlayText.HorizontalOptions = LayoutOptions.CenterAndExpand;
                overlayText.Rotation = 350;
                overlayText.SetBinding(VisualElement.IsVisibleProperty, "IsStatusNew");

                var overlay = new BoxView {
                    Color = Color.FromRgba(255, 255, 255, 124)
                };
                overlay.SetBinding(VisualElement.IsVisibleProperty, "IsOverlayed");
                //overlay.HeightRequest = Device.OnPlatform(115, 120, 115);

                //AbsoluteLayout.SetLayoutFlags(overlay, AbsoluteLayoutFlags.All);
                ////AbsoluteLayout.SetLayoutFlags(overlay, AbsoluteLayoutFlags.SizeProportional);
                //AbsoluteLayout.SetLayoutBounds(overlay, new Rectangle(0f, 0f, 1f, 0.65f));
                var overlayHeight = Device.OnPlatform(111, 116, 111);
                AbsoluteLayout.SetLayoutFlags(overlay, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(overlay, new Rectangle(0, 0, 1000, overlayHeight));
                al.Children.Add(overlay);

                AbsoluteLayout.SetLayoutFlags(overlayText, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(overlayText, new Rectangle(0.4f, 0.4f, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                al.Children.Add(overlayText);

                var overlayTextPayment = new CustomLabel();
                overlayTextPayment.Text = "Make Payment to Secure \nBooking";
                overlayTextPayment.FontFamily = UIUtils.FONT_SFUIDISPLAY_BOLD;
                overlayTextPayment.FontSize = 16;
                overlayTextPayment.TextColor = Props.ButtonInfoPageColor;
                overlayTextPayment.HorizontalTextAlignment = TextAlignment.Center;
                overlayTextPayment.HorizontalOptions = LayoutOptions.CenterAndExpand;
                overlayTextPayment.Rotation = 350;
                overlayTextPayment.SetBinding(VisualElement.IsVisibleProperty, "IsMakePayment");

                AbsoluteLayout.SetLayoutFlags(overlayTextPayment, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(overlayTextPayment, new Rectangle(0.4f, 0.4f, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
                al.Children.Add(overlayTextPayment);
            }
            View = al;
        }
    }
}
