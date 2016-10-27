using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace TiroApp.Pages
{
    public class EditPaymentPage : ContentPage
    {
        private RelativeLayout root;
        private StackLayout main;

        public EditPaymentPage()
        {
            Utils.SetupPage(this);
            BuildLayout();
        }

        private void BuildLayout()
        {
            main = new StackLayout();
            main.Spacing = 0;
            main.BackgroundColor = Color.White;

            var header = UIUtils.MakeHeader(this, "Payment Method");
            main.Children.Add(header);
            var separator = UIUtils.MakeSeparator(true);
            main.Children.Add(separator);

            root = new RelativeLayout();
            root.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));

            Content = root;
        }
    }
}
