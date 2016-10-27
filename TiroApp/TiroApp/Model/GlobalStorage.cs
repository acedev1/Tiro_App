using Gis4Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace TiroApp
{
    public class GlobalStorage
    {
        private const string FileAppSettings = "appsettings";

        private static AppSettings settings = null;
        public static AppSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = LoadSettings();
                }
                return settings;
            }
            set
            {
                settings = value;
                SaveAppSettings();
            }
        }

        private static AppSettings LoadSettings()
        {
            string xmlStr = DependencyService.Get<IFileSaveLoad>().LoadText(FileAppSettings);
            if (!string.IsNullOrEmpty(xmlStr))
            {
                return AppSettings.Parse(xmlStr);
            }
            return AppSettings.GetDefault();
        }

        public static void SaveAppSettings()
        {
            DependencyService.Get<IFileSaveLoad>().SaveText(FileAppSettings, settings.GetXmlString());
        }

        

        public static byte[] LoadResource(string name)
        {
            var assembly = typeof(Pages.HomePage).GetTypeInfo().Assembly;
            System.IO.Stream stream = assembly.GetManifestResourceStream(name);
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            stream.CopyTo(memStream);
            byte[] arr = memStream.ToArray();
            stream.Dispose();
            memStream.Dispose();
            return arr;
        }

        private static Dictionary<string, string> keyValueMap = null;

        private static void LoadKeyValueMap()
        {
            keyValueMap = new Dictionary<string, string>();
            var stream = DependencyService.Get<IFileSaveLoad>().OpenFile("keyValueMap");
            if (stream.Length != 0)
            {
                var doc = XDocument.Load(stream);
                foreach (var el in doc.Element("items").Elements())
                {
                    if (keyValueMap.ContainsKey(el.Attribute("key").Value) == false)
                        keyValueMap.Add(el.Attribute("key").Value, el.Attribute("value").Value);
                }
            }
            stream.Dispose();
        }

        private static void SaveKeyValueMap()
        {
            XDocument doc = new XDocument(new XElement("items"));
            foreach (var kv in keyValueMap)
            {
                var el = new XElement("item");
                el.Add(new XAttribute("key", kv.Key));
                el.Add(new XAttribute("value", kv.Value));
                doc.Root.Add(el);
            }
            var stream = DependencyService.Get<IFileSaveLoad>().OpenFile("keyValueMap");
            stream.SetLength(0);
            doc.Save(stream);
            stream.Dispose();
        }

        public static string GetValueByKey(string key, string defaultValue)
        {
            if (keyValueMap == null)
            {
                LoadKeyValueMap();
            }
            if (keyValueMap.ContainsKey(key))
            {
                return keyValueMap[key];
            }
            return defaultValue;
        }

        public static void SetKeyValue(string key, string value)
        {
            if (keyValueMap == null)
            {
                LoadKeyValueMap();
            }
            keyValueMap[key] = value;
            SaveKeyValueMap();
        }
    }

    public class AppSettings
    {
        public string CustomerId { get; set; }
        public string MuaId { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerCardNumber { get; set; }

        public static AppSettings GetDefault()
        {
            return new AppSettings()
            {
            };
        }

        public static AppSettings Parse(string xml)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            var sw = new System.IO.StreamWriter(stream);
            sw.Write(xml);
            sw.Flush();
            stream.Position = 0;
            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            AppSettings s = (AppSettings)serializer.Deserialize(stream);
            stream.Dispose();
            return s;
        }

        public string GetXmlString()
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(AppSettings));
            serializer.Serialize(stream, this);
            stream.Position = 0;
            var sr = new System.IO.StreamReader(stream);
            string myStr = sr.ReadToEnd();
            stream.Dispose();
            sr.Dispose();
            return myStr;
        }
    }
}
