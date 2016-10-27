using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class ConfirmDialog
    {
        public static void Show(ContentPage page, string text, IEnumerable<string> buttons, Action<int> callback)
        {
            RelativeLayout rl = null;
            try
            {
                var cd = new ConfirmDialog(text, buttons);
                rl = new RelativeLayout();
                rl.BackgroundColor = Props.BlackoutColor;
                rl.Children.Add(cd.view,
                       Constraint.RelativeToParent(p => ((p.Width - Utils.GetControlSize(cd.view).Width) / 2)),
                       Constraint.RelativeToParent(p => ((p.Height - Utils.GetControlSize(cd.view).Height) / 2)));
                if (page.Content is RelativeLayout)
                {
                    ((RelativeLayout)page.Content).Children.Add(rl, Constraint.Constant(0), Constraint.Constant(0),
                        Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
                }
                else if (page.Content is AbsoluteLayout)
                {
                    ((AbsoluteLayout)page.Content).Children.Add(rl,
                        new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.SizeProportional);
                }
                else
                {
                    //??
                }
                cd.OnSelect += (s, a) =>
                {
                    ((Layout<View>)page.Content).Children.Remove(rl);
                    callback?.Invoke(a);
                };
            }
            catch
            {
                if (rl != null)
                {
                    ((Layout<View>)page.Content).Children.Remove(rl);
                }
            }
        }

        private View view;
        public event EventHandler<int> OnSelect;

        private ConfirmDialog(string text, IEnumerable<string> buttons)
        {
            view = BuildLayout(text, buttons);
        }

        private View BuildLayout(string text, IEnumerable<string> buttons)
        {
            var row = new StackLayout();
            row.Orientation = StackOrientation.Horizontal;
            row.Spacing = 0;
            var bArr = buttons.ToList();
            for (var i = 0; i < bArr.Count; i++)
            {
                var b = UIUtils.MakeButton(bArr[i], UIUtils.FONT_SFUIDISPLAY_REGULAR);
                if (i != bArr.Count - 1)
                {
                    b.TextColor = Props.ButtonColor;
                    b.BackgroundColor = Color.FromHex("F8F8F8");
                }
                b.SetValue(UIUtils.TagProperty, i);
                b.Clicked += Button_Clicked;
                row.Children.Add(b);
            }
            var label = new CustomLabel()
            {
                Text = text,
                TextColor = Color.Black,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                FontSize = 16,
                Margin = new Thickness(10, 20, 10, 20),
                HorizontalTextAlignment = TextAlignment.Center
            };
            var sl = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                BackgroundColor = Color.White,
                WidthRequest = 300,
                Children = { label, row }
            };

            return sl;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            int index = (int)((Button)sender).GetValue(UIUtils.TagProperty);
            OnSelect?.Invoke(this, index);
        }
    }
}
