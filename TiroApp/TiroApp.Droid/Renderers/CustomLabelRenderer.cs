using Android.Graphics;
using Android.Widget;
using Droid.CustomRenderers;
using TiroApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace Droid.CustomRenderers
{
    public class CustomLabelRenderer : LabelRenderer
    {
        private object _xamFormsSender;

        public CustomLabelRenderer()
        {
            _xamFormsSender = null;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // Update native Textview
            if (_xamFormsSender != sender || e.PropertyName == CustomLabel.FontFamilyProperty.PropertyName)
            {
                var font = ((CustomLabel)sender).FontFamily;

                // valid font 
                if (!string.IsNullOrEmpty(font))
                {
                    // check font file name
                    if (!font.Contains(".otf"))
                    {
                        font += ".otf";
                    }
                    var typeface = Typeface.CreateFromAsset(Forms.Context.Assets, font);

                    // update font
                    var label = Control as TextView;
                    if (label != null)
                        label.Typeface = typeface;
                }

                _xamFormsSender = sender;
            }
        }
    }
}