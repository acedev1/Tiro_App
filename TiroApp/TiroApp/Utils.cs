using Gis4Mobile;
using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace TiroApp
{
    public class Utils
    {
        public static CultureInfo EnCulture = new CultureInfo("en-US");

        public static string Serialize(object obj)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            serializer.Serialize(stream, obj);
            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            string coordStr = reader.ReadToEnd();
            reader.Dispose();
            return coordStr;
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.Encoding.UTF8.GetString(encodedDataAsBytes, 0, encodedDataAsBytes.Length);
            return returnValue;
        }

        public static void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            System.Threading.Tasks.Task.Delay(interval).ContinueWith((t) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (callback())
                    {
                        StartTimer(interval, callback);
                    }
                });
            });
        }

        public static void SetupPage(Page page)
        {
            NavigationPage.SetHasNavigationBar(page, false);
            Device.OnPlatform(iOS: () =>
            {
                page.Padding = new Thickness(0, 20, 0, 0);
            });
        }

        public static async void ShowPageFirstInStack(Page current, Page newPage)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await current.Navigation.PushAsync(newPage);
                for (int i = newPage.Navigation.NavigationStack.Count - 2; i >= 0; i--)
                {
                    newPage.Navigation.RemovePage(newPage.Navigation.NavigationStack[i]);
                }
            });
        }

        public static Size GetControlSize(Xamarin.Forms.VisualElement control)
        {
            var size = control.GetSizeRequest(double.PositiveInfinity, double.PositiveInfinity);
            //var size2 = control.Measure(double.PositiveInfinity, double.PositiveInfinity);
            return size.Request;
        }

        public static void SaveGlobalException(Exception e)
        {
#if DEBUG
#else
            try
            {
                string msg = DateTime.Now + " Platform:" + Device.OS
                    + " Version:" + typeof(App).GetTypeInfo().Assembly.GetName().Version.ToString()
                    + " \n" + e.ToString();
                var stream = DependencyService.Get<IFileSaveLoad>().OpenFile("crash");
                var sw = new System.IO.StreamWriter(stream);
                sw.Write(msg);
                sw.Flush();
                stream.Dispose();
            }
            catch { }
#endif
        }

        public static void CheckCrash()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                var stream = DependencyService.Get<IFileSaveLoad>().OpenFile("crash");
                if (stream != null && stream.Length != 0)
                {
                    var sr = new System.IO.StreamReader(stream);
                    var msg = sr.ReadToEnd();
                    stream.Dispose();
                    DependencyService.Get<IFileSaveLoad>().DeleteFile("crash");
                    DataGate.SendCrash(msg, (r) => { });
                }
                else
                {
                    if (stream != null)
                    {
                        stream.Dispose();
                    }
                }
            });
        }        
    }
}
