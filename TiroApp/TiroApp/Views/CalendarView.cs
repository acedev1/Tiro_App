using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class CalendarView : View
    {
        public CalendarView()
        {
            this.BackgroundColor = Color.White;
        }

        public ICalendarViewHelper Helper { get; set; }
    }

    public interface ICalendarViewHelper
    {
        DateTime SelectedDate { get; set; }
        List<DateTime> DottedDates { get; set; }
        string DotColor { get; set; }
        event EventHandler OnSelectedDateChange;
    }
}
