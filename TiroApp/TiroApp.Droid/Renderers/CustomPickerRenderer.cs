using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using TiroApp.Droid.Renderers;
using Xamarin.Forms.Platform.Android;
using System.Text.RegularExpressions;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(Picker), typeof(CustomPickerRenderer))]
namespace TiroApp.Droid.Renderers
{
    public class CustomPickerRenderer : PickerRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Control.Text = Regex.Match(Control.Text, @"\+?\d+\s?\d+").Value;
        }
    }
}