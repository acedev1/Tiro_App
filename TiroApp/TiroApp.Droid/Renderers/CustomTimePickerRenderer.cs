using Android.Graphics;
using TiroApp.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TimePicker), typeof(CustomTimePickerRenderer))]
namespace TiroApp.iOS.Renderers
{
    public class CustomTimePickerRenderer : TimePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Typeface = Typeface.CreateFromAsset(Forms.Context.Assets, UIUtils.FONT_BEBAS_REGULAR + ".otf");
                Control.TextSize = 16;
            }
        }
    }
}
