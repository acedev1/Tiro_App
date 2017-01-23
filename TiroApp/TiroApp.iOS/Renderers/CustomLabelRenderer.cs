using Foundation;
using TiroApp.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(TiroApp.iOS.Renderers.CustomLabelRenderer))]
namespace TiroApp.iOS.Renderers
{
    public class CustomLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            try
            {
                var spacingVal = ((CustomLabel)Element).LetterSpacing;
                if (Control != null && Element.Text != null && spacingVal != 0)
                {
                    var uilabel = Control as UILabel;
                    NSMutableAttributedString str = new NSMutableAttributedString(Element.Text);
                    var spacing = NSObject.FromObject(10f * spacingVal);
                    var range = new NSRange(0, Element.Text.Length - 1);
                    str.AddAttribute(UIStringAttributeKey.KerningAdjustment, spacing, range);
                    uilabel.AttributedText = str;
                }
            }
            catch { }
        }
    }
}