using CoreGraphics;
using CoreText;
using Foundation;
using System;
using System.Collections.Generic;
using TiroApp.Views;
using UIKit;

namespace TiroApp.Droid.Views
{
    public class CalendarViewIOS: UIView, ICalendarViewHelper
    {        
        private DateTime _selectedDate;        
        private string _dotColor;
        private List<DateTime> _dotDates;
        private List<int> _dotDays;

        public event EventHandler OnSelectedDateChange;

        public CalendarViewIOS ()
        {
            _dotColor = "#4083FF";
            _selectedDate = DateTime.Today;
            _dotDays = new List<int>();
        }

        public CalendarViewIOS(CGRect bounds)
			: base(bounds)
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
                SetNeedsDisplay();
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
                this.SetNeedsDisplay();
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
                this.SetNeedsDisplay();
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            var colCount = 7;
            var rowCout = 6 + 1;
            var dotRadius = 3;
            var textSize = 15;
            var boxWidth = this.Bounds.Width / colCount;
            var boxHeight = this.Bounds.Height / rowCout;
            var textYOffset = (boxHeight - textSize) / 2;// + textSize;
            var dotColor = ParseColor(_dotColor);

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

            using (var g = UIGraphics.GetCurrentContext())
            {
                //g.SetStrokeColor(UIColor.Gray.CGColor);
                //for (var r = 0; r < rowCout; r++)
                //{
                //    for (var c = 0; c < colCount; c++)
                //    {
                //        var left = c * boxWidth;
                //        var top = r * boxHeight;
                //        g.BeginPath();
                //        g.AddRect(new CGRect(left, top, left + boxWidth, top + boxHeight));
                //        g.DrawPath(CGPathDrawingMode.Stroke);
                //    }
                //}
                
                g.SetStrokeColor(UIColor.Gray.CGColor);
                for (var c = 0; c < colCount; c++)
                {
                    var left = c * boxWidth + boxWidth * 0.4f;
                    DrawText(strHeader[c], left, textYOffset, textSize, UIColor.Gray.CGColor);
                }

                var daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);
                var topOffset = textYOffset + boxHeight;
                for (var day = 1; day <= daysInMonth; day++)
                {
                    var date = new DateTime(SelectedDate.Year, SelectedDate.Month, day);
                    int dayOfWeek = (int)date.DayOfWeek;
                    var left = dayOfWeek * boxWidth + (day < 10 ? boxWidth * 0.4f : boxWidth * 0.3f);
                    if (day == SelectedDate.Day)
                    {
                        var color = ParseColor("#E3E3E3");
                        g.BeginPath();
                        g.SetFillColor(color);
                        g.AddEllipseInRect(new CGRect(dayOfWeek * boxWidth, topOffset - textYOffset, boxWidth, boxHeight));
                        g.DrawPath(CGPathDrawingMode.Fill);
                    }
                    if (_dotDays.Contains(day))
                    {
                        var centerX = dayOfWeek * boxWidth + boxWidth / 2;
                        var centerY = topOffset + textSize / 2 + 10;
                        g.BeginPath();
                        g.SetFillColor(dotColor);
                        g.AddEllipseInRect(new CGRect(centerX - dotRadius, centerY - dotRadius, dotRadius * 2, dotRadius * 2));
                        g.DrawPath(CGPathDrawingMode.Fill);
                    }
                    DrawText(day.ToString(), left, topOffset - 5, textSize, UIColor.Black.CGColor);

                    if (dayOfWeek == 6) //saturday
                    {
                        topOffset += boxHeight;
                    }
                }
            }
        }

        private void DrawText(string t, nfloat x, nfloat y, nfloat fontSize, CGColor color)
        {
            var attributedString = new NSAttributedString(t, new CTStringAttributes
            {
                ForegroundColorFromContext = true,
                ForegroundColor = color,
                StrokeColor = color,
                Font = new CTFont("SFUIDisplay-Regular", fontSize)
            });
            attributedString.DrawString(new CGPoint(x, y));
        }

        private CGColor ParseColor(string color)
        {
            var colorString = color.Replace("#", "");
            var red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
            var green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
            var blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
            return UIColor.FromRGB(red, green, blue).CGColor;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            var touch = touches.AnyObject as UITouch;
            if (touch != null)
            {
                var p = touch.LocationInView(this);
                var colCount = 7;
                var rowCout = 6 + 1;
                var boxWidth = this.Bounds.Width / colCount;
                var boxHeight = this.Bounds.Height / rowCout;

                int colIndex = (int)(p.X / boxWidth);
                int rowIndex = (int)(p.Y / boxHeight);
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
        }
    }
}