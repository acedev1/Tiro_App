using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TiroApp.Droid
{
    [Activity(Label = "TiroApp", Icon = "@drawable/tiro_logo", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            AndroidEnvironment.UnhandledExceptionRaiser += Android_UnhandledException;
            base.OnCreate(bundle);
            
            App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);
            App.ScreenHeight = (int)((Resources.DisplayMetrics.HeightPixels) / Resources.DisplayMetrics.Density);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            ImageCircle.Forms.Plugin.Droid.ImageCircleRenderer.Init();
            LoadApplication(new App());

            var img = new ImageView(this);
            img.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
            img.SetImageResource(Resource.Drawable.splashscreen);
            img.SetScaleType(ImageView.ScaleType.CenterCrop);
            var cv = (FrameLayout)this.FindViewById(Android.Resource.Id.Content);
            cv.AddView(img);
            cv.PostDelayed(() =>
            {
                cv.RemoveView(img);
            }, 3000);
        }

        protected override void OnStart()
        {
            base.OnStart();
            var cv = this.FindViewById(Android.Resource.Id.Content);
        }

        private void Android_UnhandledException(object sender, RaiseThrowableEventArgs e)
        {
            Utils.SaveGlobalException(e.Exception);
        }
    }
}

