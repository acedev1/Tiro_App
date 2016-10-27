using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TiroApp.Model;
using TiroApp.Pages;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp
{
    public class AvailabilityPage : ContentPage
    {
        private Views.CalendarView _calendarView;
        private StackLayout _daysView;
        private StackLayout _calendarLayout;
        private ContentView _contentView;
        private StackLayout timePickerView = null;
        private CustomLabel monthLabel;

        private DateTime _selectedDate;
        private bool _needCalenderInit = true;

        private Order order;
        private bool isOrder = false;
        private RelativeLayout root;
        private TimeSpan? _selectedTime = null;
        private BoxView blackout;
        private CustomEntry noteEntry;

        public AvailabilityPage()
        {
            _selectedDate = DateTime.Now;
            Mode = ViewMode.MultiRange;
            ButtonText = "Save";
            this.Availibility = new Availibility()
            {
                Mode = AvailibilityMode.Dates,
                DatesFrom = new List<DateTime>(),
                DatesTo = new List<DateTime>()
            };

            Utils.SetupPage(this);
            Utils.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                BuildLayout();
                return false;
            });
        }

        public AvailabilityPage(Order order)
        {
            isOrder = true;
            this.order = order;
            Mode = ViewMode.SelectFromRow;
            ButtonText = "CONTINUE";
            this.Availibility = new Availibility()
            {
                Mode = AvailibilityMode.Dates,
                DatesFrom = new List<DateTime>(),
                DatesTo = new List<DateTime>()
            };

            Utils.SetupPage(this);
            Utils.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                BuildLayout();
                return false;
            });
        }

        public event EventHandler OnFinishSelection;
        public event EventHandler OnMonthChange;
        public ViewMode Mode { get; set; }
        public Availibility Availibility { get; set; }
        public string ButtonText { get; set; }

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
            }
        }

        private void BuildLayout()
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
            headerLabel.Text = isOrder ? $"Services {UIUtils.NIARA_SIGN}{order.TotalPrice}" : "My Availability";
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

            monthLabel = new CustomLabel();
            monthLabel.Text = _selectedDate.ToString("MMMM", Utils.EnCulture);
            monthLabel.TextColor = Color.Black;
            monthLabel.HorizontalTextAlignment = TextAlignment.Center;
            monthLabel.VerticalTextAlignment = TextAlignment.Center;
            monthLabel.HeightRequest = Device.OnPlatform(40, 50, 40);
            monthLabel.FontSize = 20;
            monthLabel.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            monthLabel.HorizontalOptions = LayoutOptions.Center;
            var monthBack = new Image();
            monthBack.Source = ImageSource.FromResource("TiroApp.Images.ArrowBackBlack.png");
            monthBack.HeightRequest = 20;
            monthBack.Margin = new Thickness(0, 0, 10, 0);
            monthBack.VerticalOptions = LayoutOptions.Center;
            monthBack.HorizontalOptions = LayoutOptions.Start;
            monthBack.GestureRecognizers.Add(new TapGestureRecognizer((v) => { ChangeMonth(-1); }));
            var monthForward = new Image();
            monthForward.Source = ImageSource.FromResource("TiroApp.Images.ArrowBackBlack.png");
            monthForward.Rotation = 180;
            monthForward.AnchorX = 0.5;
            monthForward.AnchorY = 0.5;
            monthForward.HeightRequest = 20;
            monthForward.Margin = new Thickness(10, 0, 0, 0);
            monthForward.VerticalOptions = LayoutOptions.Center;
            monthForward.HorizontalOptions = LayoutOptions.End;
            monthForward.GestureRecognizers.Add(new TapGestureRecognizer((v) => { ChangeMonth(1); }));
            var monthHeader = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                //BackgroundColor = Color.FromHex("F8F8F8"),
                Children = { monthBack, monthLabel, monthForward }
            };

            _calendarView = new Views.CalendarView();
            _calendarView.HeightRequest = Device.OnPlatform(300, 350, 300);
            _calendarView.WidthRequest = Device.OnPlatform(320, 370, 300);
            _calendarView.HorizontalOptions = LayoutOptions.Center;
            _calendarView.Margin = Device.OnPlatform(new Thickness(20, 0, 20, 0), new Thickness(20, 10, 20, 10), new Thickness(10));
            Utils.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                if (_calendarView.Helper == null)
                {
                    return false;
                }
                _calendarView.Helper.SelectedDate = _selectedDate;
                if (Mode == ViewMode.MultiRange || Mode == ViewMode.SelectFromRow)
                {
                    _calendarView.Helper.DottedDates = Availibility.DatesFrom;
                }
                _calendarView.Helper.OnSelectedDateChange += OnSelectedDateChange;
                _needCalenderInit = false;
                return false;
            });

            var btn = UIUtils.MakeButton(ButtonText, UIUtils.FONT_SFUIDISPLAY_REGULAR);
            btn.VerticalOptions = LayoutOptions.EndAndExpand;
            if (isOrder)
            {
                btn.Clicked += ContinueOrder;
            }
            else
            {
                btn.Clicked += OnFinish;
            }

            _calendarLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                Children = { monthHeader, _calendarView }
            };

            _daysView = new StackLayout();
            _daysView.Margin = new Thickness(10);
            for (var d = 0; d < 7; d++)
            {
                var rb = new RadioButton();
                rb.Text = ((DayOfWeek)d).ToString();
                rb.HeightRequest = Device.OnPlatform(30, 40, 30);
                rb.TextColor = Color.Black;
                rb.FontSize = 17;
                rb.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
                if (Availibility.DaysOfWeek != null)
                {
                    if (Availibility.DaysOfWeek.Contains(d))
                    {
                        rb.IsChecked = true;
                    }
                }
                _daysView.Children.Add(rb);
            }

            _contentView = new ContentView();
            _contentView.VerticalOptions = LayoutOptions.Start;
            if (this.Mode == ViewMode.MultiRange)
            {
                _contentView.Content = (this.Availibility.Mode == AvailibilityMode.Dates) ? _calendarLayout : _daysView;
            }
            else
            {
                _contentView.Content = _calendarLayout;
            }

            var main = new StackLayout();
            main.Spacing = 0;
            main.BackgroundColor = Color.White;
            main.Children.Add(header);
            main.Children.Add(separator);
            main.Children.Add(_contentView);

            timePickerView = new StackLayout();
            timePickerView.VerticalOptions = LayoutOptions.End;
            if (Mode == ViewMode.MultiRange)
            {   
                BuildTimePickerView();
                main.Children.Add(timePickerView);
                var selectLayout = BuildSelectorAvailabilityModeLayout();
                main.Children.Add(selectLayout);
            }
            else if (Mode == ViewMode.SingleRange)
            {
                BuildTimePickerView();
                main.Children.Add(timePickerView);
            }
            else if (Mode == ViewMode.SelectFromRow)
            {
                BuildOrderTimePickerView();
                main.Children.Add(timePickerView);
                noteEntry = new CustomEntry();
                noteEntry.FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR;
                noteEntry.Placeholder = "Add note to your makeup artist (optional)";
                noteEntry.PlaceholderColor = Props.GrayColor;
                noteEntry.TextColor = Color.Black;
                noteEntry.VerticalOptions = LayoutOptions.FillAndExpand;
                main.Children.Add(noteEntry);
            }

            main.Children.Add(btn);

            root = new RelativeLayout();
            root.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));

            blackout = new BoxView();
            blackout.Color = Props.BlackoutColor;
            blackout.IsVisible = false;
            root.Children.Add(blackout, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));

            Content = root;
        }

        private void ContinueOrder(object sender, EventArgs e)
        {
            if (!_selectedTime.HasValue)
            {
                UIUtils.ShowMessage("Please, select time", this);
                return;
            }
            order.DateTime = _selectedDate.Date + _selectedTime.Value;
            order.Note = noteEntry.Text;
            BuildPaymentLayout();
        }

        private void BuildOrderTimePickerView()
        {
            if (timePickerView == null)
            {
                return;
            }
            timePickerView.Children.Clear();
            var elWidth = Device.OnPlatform(50, 50, 80);
            var l = timePickerView;
            l.Orientation = StackOrientation.Horizontal;
            l.HorizontalOptions = LayoutOptions.Fill;
            l.BackgroundColor = Color.FromHex("EFF1F3");
            l.HeightRequest = 60;

            var datesFrom = Availibility.DatesFrom;
            //if (Availibility.Mode == AvailibilityMode.Dates)
            //{
                datesFrom = Availibility.DatesFrom.Where(d => d.Date == _selectedDate.Date).ToList();
            //}
            for (var i = 0; i < datesFrom.Count; i++)
            {
                var fromTime = datesFrom[i].TimeOfDay;
                if (_selectedTime != null && _selectedTime.Equals(fromTime))
                {
                    AddTimeLabel(elWidth, fromTime, l, true);
                }
                else
                {
                    AddTimeLabel(elWidth, fromTime, l);
                }
            }
        }

        private void BuildTimePickerView()
        {
            if (timePickerView == null)
            {
                return;
            }
            timePickerView.Children.Clear();
            var elWidth = Device.OnPlatform(50, 70, 80);
            var l = timePickerView;
            l.Orientation = StackOrientation.Horizontal;
            l.HorizontalOptions = LayoutOptions.Fill;
            l.BackgroundColor = Color.FromHex("EFF1F3");
            l.HeightRequest = Device.OnPlatform(80, 100, 100);

            var cLayout = new StackLayout();
            cLayout.Orientation = StackOrientation.Vertical;
            cLayout.WidthRequest = elWidth;
            cLayout.BackgroundColor = Color.FromHex("DADEE3");
            var label = new CustomLabel();
            label.Text = "From";
            label.FontSize = 16;            
            label.TextColor = Color.Black;
            label.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            label.HeightRequest = l.HeightRequest / 2;
            label.HorizontalOptions = LayoutOptions.Center;
            label.VerticalTextAlignment = TextAlignment.Center;
            cLayout.Children.Add(label);
            label = new CustomLabel();
            label.Text = "To";
            label.FontSize = 16;
            label.TextColor = Color.Black;
            label.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            label.HeightRequest = l.HeightRequest / 2;
            label.HorizontalOptions = LayoutOptions.Center;
            label.VerticalTextAlignment = TextAlignment.Center;
            cLayout.Children.Add(label);

            l.Children.Add(cLayout);

            var datesFrom = Availibility.DatesFrom;
            var datesTo = Availibility.DatesTo;
            if (Availibility.Mode == AvailibilityMode.Dates)
            {
                datesFrom = Availibility.DatesFrom.Where(d => d.Date == _selectedDate.Date).ToList();
                datesTo = Availibility.DatesTo.Where(d => d.Date == _selectedDate.Date).ToList();
            }

            for (var i = 0; i < datesFrom.Count; i++)
            {
                var fromTime = datesFrom[i].TimeOfDay;
                var toTime = datesTo[i].TimeOfDay;
                AddTimePickerPair(elWidth, fromTime, toTime, l);
            }

            if (datesFrom.Count == 0)
            {
                AddTimePickerPair(elWidth, new TimeSpan(), new TimeSpan(), l);
            }

            if (Mode == ViewMode.MultiRange)
            {
                var addImage = new Image();
                addImage.Source = ImageSource.FromResource("TiroApp.Images.PlusBlack.png");
                addImage.HeightRequest = 20;
                addImage.VerticalOptions = LayoutOptions.Center;
                addImage.GestureRecognizers.Add(new TapGestureRecognizer((v) =>
                {
                    AddTimePickerPair(elWidth, new TimeSpan(), new TimeSpan(), l, true);
                }));
                l.Children.Add(addImage);
            }
        }

        private void AddTimePickerPair(int elWidth, TimeSpan from, TimeSpan to, StackLayout root, bool beforeImage = false)
        {
            var tpnLayout = new StackLayout();
            tpnLayout.Orientation = StackOrientation.Vertical;
            tpnLayout.WidthRequest = elWidth;
            var tpn = new TimePicker();
            tpn.Time = from;
            tpn.TextColor = Props.ButtonColor;
            tpn.VerticalOptions = LayoutOptions.FillAndExpand;
            tpnLayout.Children.Add(tpn);
            tpn = new TimePicker();
            tpn.Time = to;
            tpn.TextColor = Props.ButtonColor;
            tpn.VerticalOptions = LayoutOptions.FillAndExpand;
            tpnLayout.Children.Add(tpn);
            if (beforeImage)
            {
                root.Children.Insert(root.Children.Count - 1, tpnLayout);
            }
            else
            {
                root.Children.Add(tpnLayout);
            }
        }

        private void AddTimeLabel(int elWidth, TimeSpan from, StackLayout root, bool isSelected = false)
        {
            var time = new CustomLabel();
            time.Text = DateTime.MinValue.Add(from).ToString("hh:mm tt", Utils.EnCulture);
            time.TextColor = Props.ButtonColor;
            if (isSelected)
            {
                time.BackgroundColor = Color.White;
            }
            time.FontSize = 12;
            time.VerticalOptions = LayoutOptions.CenterAndExpand;
            time.WidthRequest = elWidth;
            time.Margin = new Thickness(8, 0, 8, 0);
            time.HorizontalTextAlignment = TextAlignment.Center;
            time.GestureRecognizers.Add(new TapGestureRecognizer(v => {
                _selectedTime = DateTime.ParseExact(time.Text, "hh:mm tt", Utils.EnCulture).TimeOfDay;
                BuildOrderTimePickerView();
            }));
            root.Children.Add(time);
        }

        private StackLayout BuildSelectorAvailabilityModeLayout()
        {
            var selectLayout = new StackLayout();
            selectLayout.Orientation = StackOrientation.Horizontal;
            selectLayout.HorizontalOptions = LayoutOptions.Center;
            selectLayout.Margin = new Thickness(0, 10, 0, 0);
            selectLayout.HeightRequest = Device.OnPlatform(30, 40, 30);
            var rb1 = new RadioButton();
            rb1.Text = "One time";
            rb1.TextColor = Color.Black;
            rb1.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            rb1.FontSize = 18;
            rb1.IsChecked = Availibility.Mode == AvailibilityMode.Dates;
            var rb2 = new RadioButton();
            rb2.Text = "Recuring";
            rb2.TextColor = Color.Black;
            rb2.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            rb2.FontSize = 18;
            rb2.IsChecked = Availibility.Mode == AvailibilityMode.DaysOfWeek;
            selectLayout.Children.Add(rb1);
            selectLayout.Children.Add(rb2);
            rb1.OnCheckedChange += (s, a) => { rb2.IsChecked = false; OnSelectorModeChange(AvailibilityMode.Dates); };
            rb2.OnCheckedChange += (s, a) => { rb1.IsChecked = false; OnSelectorModeChange(AvailibilityMode.DaysOfWeek); };
            return selectLayout;
        }

        public void BuildPaymentLayout()
        {
            blackout.IsVisible = true;
            var layout = new StackLayout { Spacing = 0 };
            layout.WidthRequest = Utils.GetControlSize(root).Width - 80;

            var creditCardPay = UIUtils.MakeButton("PAY WITH CREDIT CARD", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            creditCardPay.BackgroundColor = Color.FromHex("#4E3752");
            layout.Children.Add(creditCardPay);
            creditCardPay.Clicked += (s, args) => {
                order.PaymentType = PaymentType.Card;
                root.Children.Remove(layout);
                blackout.IsVisible = false;
                this.Navigation.PushAsync(new OrderSummary(order));
            };

            var cashPay = UIUtils.MakeButton("PAY WITH CASH", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            layout.Children.Add(cashPay);
            cashPay.Clicked += (s, args) => {
                order.PaymentType = PaymentType.Cash;
                root.Children.Remove(layout);
                blackout.IsVisible = false;
                this.Navigation.PushAsync(new OrderSummary(order));
            };

            var cancel = UIUtils.MakeButton("CANCEL", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            cancel.BackgroundColor = Color.White;
            cancel.TextColor = Props.ButtonColor;
            layout.Children.Add(cancel);
            cancel.Clicked += (s, args) => {
                root.Children.Remove(layout);
                blackout.IsVisible = false;
            };

            root.Children.Add(layout
                , Constraint.RelativeToParent(p => p.Width / 2 - Utils.GetControlSize(layout).Width / 2)
                , Constraint.RelativeToParent(p => p.Height / 2 - Utils.GetControlSize(layout).Height / 2));
        }

        private void OnSelectedDateChange(object sender, EventArgs e)
        {
            if (isOrder && !Availibility.DatesFrom.Select(d => d.Date).Contains(_calendarView.Helper.SelectedDate.Date))
            {
                _calendarView.Helper.SelectedDate = _selectedDate;
                return;
            }
            RefreshAvilability();
            _selectedDate = _calendarView.Helper.SelectedDate;
            if (isOrder)
            {
                BuildOrderTimePickerView();
            }
            else
            {
                BuildTimePickerView();
            }
        }

        private void RefreshAvilability()
        {
            if (this.Mode == ViewMode.SingleRange)
            {
                StackLayout layout = (StackLayout)timePickerView.Children[1];
                var fromTime = ((TimePicker)layout.Children[0]).Time;
                var toTime = ((TimePicker)layout.Children[1]).Time;
                Availibility.DatesFrom.Clear();
                Availibility.DatesTo.Clear();
                Availibility.DatesFrom.Add(_selectedDate.Date.Add(fromTime));
                Availibility.DatesTo.Add(_selectedDate.Date.Add(toTime));
            }
            else if (Mode == ViewMode.MultiRange)
            {
                if (Availibility.Mode == AvailibilityMode.Dates)
                {
                    Availibility.DatesFrom.RemoveAll((dt) => { return dt.Date == _selectedDate.Date; });
                    Availibility.DatesTo.RemoveAll((dt) => { return dt.Date == _selectedDate.Date; });
                }
                else
                {
                    Availibility.DatesFrom.Clear();
                    Availibility.DatesTo.Clear();
                    Availibility.DaysOfWeek = new List<int>();
                    for (int d = 0; d < _daysView.Children.Count; d++)
                    {
                        var rb = _daysView.Children[d] as RadioButton;
                        if (rb.IsChecked)
                        {
                            Availibility.DaysOfWeek.Add(d);
                        }
                    }
                }
                for (int i = 1; i < timePickerView.Children.Count - 1; i++)
                {
                    StackLayout layout = timePickerView.Children[i] as StackLayout;
                    if (layout != null)
                    {
                        var fromTime = ((TimePicker)layout.Children[0]).Time;
                        var toTime = ((TimePicker)layout.Children[1]).Time;
                        if (fromTime != new TimeSpan())
                        {
                            Availibility.DatesFrom.Add(_selectedDate.Date.Add(fromTime));
                            Availibility.DatesTo.Add(_selectedDate.Date.Add(toTime));
                        }
                    }
                }
            }
        }

        private void OnSelectorModeChange(AvailibilityMode mode)
        {
            this.Availibility.Mode = mode;
            if (Availibility.Mode == AvailibilityMode.DaysOfWeek)
            {
                Availibility.DatesFrom = new List<DateTime>();
                Availibility.DatesTo = new List<DateTime>();
            }
            BuildTimePickerView();
            _contentView.Content = (this.Availibility.Mode == AvailibilityMode.Dates) ? _calendarLayout : _daysView;
            if (_needCalenderInit)
            {
                Utils.StartTimer(TimeSpan.FromMilliseconds(100), () =>
                {
                    if (_calendarView.Helper == null)
                    {
                        return false;
                    }
                    _calendarView.Helper.SelectedDate = _selectedDate;
                    if (Mode == ViewMode.MultiRange)
                    {
                        _calendarView.Helper.DottedDates = Availibility.DatesFrom;
                    }
                    _calendarView.Helper.OnSelectedDateChange += OnSelectedDateChange;
                    _needCalenderInit = false;
                    return false;
                });
            }
        }

        private void ChangeMonth(int inc)
        {   
            _calendarView.Helper.SelectedDate = _calendarView.Helper.SelectedDate.AddMonths(inc);
            OnSelectedDateChange(null, null);
            monthLabel.Text = _calendarView.Helper.SelectedDate.ToString("MMMM", Utils.EnCulture);
            if (OnMonthChange != null)
            {
                OnMonthChange(this, EventArgs.Empty);
            }
        }

        private void OnFinish(object sender, EventArgs e)
        {
            this.RefreshAvilability();
            this.Navigation.PopAsync();
            if (OnFinishSelection != null)
            {
                OnFinishSelection(this, EventArgs.Empty);
            }
        }

    }

    public enum ViewMode
    {
        MultiRange = 0,
        SingleRange = 1,
        SelectFromRow = 2
    }

    public class Availibility
    {
        public Availibility()
        {
            Mode = AvailibilityMode.Dates;
            DatesFrom = new List<DateTime>();
            DatesTo = new List<DateTime>();
        }
        public AvailibilityMode Mode { get; set; }
        public List<DateTime> DatesFrom { get; set; }
        public List<DateTime> DatesTo { get; set; }
        public List<int> DaysOfWeek { get; set; }

        public static Availibility Parse (string str)
        {   
            var avail = new Availibility();
            try
            {
                var aObj = JObject.Parse(str);
                avail.Mode = (AvailibilityMode)(int)aObj["Mode"];
                avail.DatesFrom = new List<DateTime>();
                avail.DatesTo = new List<DateTime>();
                foreach (DateTime d in (Newtonsoft.Json.Linq.JArray)aObj["DatesFrom"])
                {
                    avail.DatesFrom.Add(d);
                }
                foreach (DateTime d in (Newtonsoft.Json.Linq.JArray)aObj["DatesTo"])
                {
                    avail.DatesTo.Add(d);
                }
                if (avail.Mode == AvailibilityMode.DaysOfWeek)
                {
                    avail.DaysOfWeek = new List<int>();
                    var days = ((string)aObj["DaysOfWeek"]).Split(',');
                    foreach (var d in days)
                    {
                        avail.DaysOfWeek.Add(int.Parse(d));
                    }
                }
            }
            catch { }
            return avail;
        }
    }

    public enum AvailibilityMode
    {
        Dates = 0,
        DaysOfWeek = 1
    }
}
