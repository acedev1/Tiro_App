using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Droid.Views
{
    public class CalendarViewDroid: Android.Views.View, ICalendarViewHelper
    {        
        private DateTime _selectedDate;        
        private string _dotColor;
        private List<DateTime> _dotDates;
        private List<int> _dotDays;

        public event EventHandler OnSelectedDateChange;

        public CalendarViewDroid(Context context)
            : base(context)
        {
            _dotColor = "#4083FF";
            _selectedDate = DateTime.Today;
            _dotDays = new List<int>();
        }
        
        public string DotColor
        {
            get
            {
                return _dotColor;
            }

            set
            {
                _dotColor = value;
                Invalidate();
            }
        }

        public List<DateTime> DottedDates
        {
            get
            {
                return _dotDates;
            }

            set
            {
                this._dotDates = value;
                this.Invalidate();
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }

            set
            {
                _selectedDate = value;
                this.Invalidate();
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var colCount = 7;
            var rowCout = 6 + 1;
            var boxWidth = this.Width / colCount;
            var boxHeight = this.Height / rowCout;
            var dotRadius = 3 * Resources.DisplayMetrics.Density;

            var paintNumber = new Paint();
            paintNumber.Color = Android.Graphics.Color.Black;
            paintNumber.TextSize = boxHeight * 0.3f;
            var paintHeader = new Paint();
            paintHeader.Color = Android.Graphics.Color.Gray;
            paintHeader.TextSize = paintNumber.TextSize;
            var paintDot = new Paint();
            paintDot.Color = Android.Graphics.Color.ParseColor(_dotColor);
            paintDot.SetStyle(Paint.Style.Fill);
            var paintBox = new Paint();
            paintBox.Color = Android.Graphics.Color.Gray;
            paintBox.SetStyle(Paint.Style.Stroke);

            var textYOffset = (boxHeight - paintNumber.TextSize) / 2 + paintNumber.TextSize;

            var strHeader = new string[] { "S", "M", "T", "W", "T", "F", "S" };

            _dotDays = new List<int>();
            if (_dotDates != null)
            {
                foreach (var d in _dotDates)
                {
                    if (d.Month == _selectedDate.Month)
                    {
                        _dotDays.Add(d.Day);
                    }
                }
            }

            //for (var r = 0; r < rowCout; r++)
            //{
            //    for (var c = 0; c < colCount; c++)
            //    {
            //        float left = c * boxWidth;
            //        float top = r * boxHeight;
            //        canvas.DrawRect(left, top, left + boxWidth, top + boxHeight, paintBox);
            //    }
            //}

            for (var c = 0; c < colCount; c++)
            {
                float left = c * boxWidth + boxWidth * 0.35f;
                canvas.DrawText(strHeader[c], left, textYOffset, paintHeader);
            }

            var daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);
            var topOffset = textYOffset + boxHeight;
            for (var day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(SelectedDate.Year, SelectedDate.Month, day);
                int dayOfWeek = (int)date.DayOfWeek;
                var left = dayOfWeek * boxWidth + (day < 10 ? boxWidth * 0.4f : boxWidth * 0.35f);
                if (day == SelectedDate.Day)
                {
                    var p = new Paint();
                    p.Color = Android.Graphics.Color.ParseColor("#E3E3E3");
                    p.SetStyle(Paint.Style.Fill);
                    canvas.DrawCircle(dayOfWeek * boxWidth + boxWidth / 2, topOffset - textYOffset + boxHeight / 2, boxWidth * 0.5f, p);
                }
                if (_dotDays.Contains(day))
                {
                    canvas.DrawCircle(dayOfWeek * boxWidth + boxWidth / 2, topOffset + 1 * Resources.DisplayMetrics.Density, dotRadius, paintDot);
                }
                canvas.DrawText(day.ToString(), left, topOffset - 10, paintNumber);

                if (dayOfWeek == 6) //saturday
                {
                    topOffset += boxHeight;
                }
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                var x = e.GetX();
                var y = e.GetY();
                var colCount = 7;
                var rowCout = 6 + 1;
                var boxWidth = this.Width / colCount;
                var boxHeight = this.Height / rowCout;

                int colIndex = (int)(x / boxWidth);
                int rowIndex = (int)(y / boxHeight);
                if (rowIndex != 0)
                {
                    var daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);
                    int startD = (int)new DateTime(SelectedDate.Year, SelectedDate.Month, 1).DayOfWeek;
                    int selectedDay = (rowIndex - 1) * colCount + colIndex + 1 - startD;

                    if (selectedDay > 0 && selectedDay <= daysInMonth)
                    {
                        SelectedDate = new DateTime(SelectedDate.Year, SelectedDate.Month, selectedDay);
                        if (OnSelectedDateChange != null)
                        {
                            OnSelectedDateChange(this, EventArgs.Empty);
                        }
                    }
                }
            }
            return base.OnTouchEvent(e);
        }
    }
}