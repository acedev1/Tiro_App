using Services;
using System;
using System.IO;
using Xamarin.Forms;
using Gis4Mobile;
using System.Threading.Tasks;

[assembly: Dependency (typeof (FileSaveLoad))]
namespace Services
{
	public class FileSaveLoad : Gis4Mobile.IFileSaveLoad
	{
		public void SaveText(string filename, string text)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            File.WriteAllText(filePath, text);
        }

        public string LoadText(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(documentsPath, filename);
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            else
            {
                return string.Empty;
            }
        }

		public Stream OpenFile (string path)
		{
			try
			{
                path = path.Replace("\\","/");
				var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				var filePath = System.IO.Path.Combine(documentsPath, path);
                var dirPath = System.IO.Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
				var stream = File.Open(filePath, FileMode.OpenOrCreate);
				return stream;
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);
			}
			return null;
		}

        public void DeleteFile(string path)
        {
            try
            {
                path = path.Replace("\\","/");
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = System.IO.Path.Combine(documentsPath, path);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        public void DeleteDirectory(string path, EventHandler callback)
        {
            try
            {
                path = path.Replace("\\","/");
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var dirPath = System.IO.Path.Combine(documentsPath, path);
                if (Directory.Exists(dirPath))
                {
                    Directory.Delete(dirPath, true);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            if (callback != null)
            {
                callback.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
