using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using TiroApp.Droid.Views;
using System.ComponentModel;
using TiroApp.Views;

[assembly: ExportRenderer(typeof(CalendarView), typeof(TiroApp.Droid.Renderers.CalendarViewRenderer))]
namespace TiroApp.Droid.Renderers
{
    public class CalendarViewRenderer : ViewRenderer<CalendarView, CalendarViewDroid>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CalendarView> e)
        {
            base.OnElementChanged(e);

            var nativeView = new CalendarViewDroid(Xamarin.Forms.Forms.Context);
            this.Element.Helper = nativeView;
            SetNativeControl(nativeView);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}