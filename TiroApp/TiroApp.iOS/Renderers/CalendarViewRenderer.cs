using Xamarin.Forms;
using System.ComponentModel;
using TiroApp.Views;
using TiroApp.Droid.Views;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CalendarView), typeof(TiroApp.iOS.Renderers.CalendarViewRenderer))]
namespace TiroApp.iOS.Renderers
{
    public class CalendarViewRenderer : ViewRenderer<CalendarView, CalendarViewIOS>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CalendarView> e)
        {
            base.OnElementChanged(e);

            if (this.Element != null)
            {
                var nativeView = new CalendarViewIOS();
                this.Element.Helper = nativeView;
                SetNativeControl(nativeView);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }
    }
}