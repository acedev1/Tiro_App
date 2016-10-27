using TiroApp.iOS.Renderers;
using TiroApp.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

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
                Control.BorderStyle = UITextBorderStyle.None;
                Control.Font = UIFont.FromName(UIUtils.FONT_BEBAS_REGULAR, 16);
            }
        }
    }
}
