using Services;
using System;
using System.IO;
using Xamarin.Forms;
using Gis4Mobile;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Media;

[assembly: Dependency (typeof (FileSaveLoad))]
namespace Services
{
	public class FileSaveLoad : Gis4Mobile.IFileSaveLoad
	{
		private static string [] tags = new string[]
		{
			ExifInterface.TagAperture,
			ExifInterface.TagDatetime,
			//ExifInterface.TagDatetimeDigitized,
			ExifInterface.TagExposureTime,
			ExifInterface.TagFlash,
			ExifInterface.TagFocalLength,
			ExifInterface.TagGpsAltitude,
			ExifInterface.TagGpsAltitudeRef,
			ExifInterface.TagGpsDatestamp,
			ExifInterface.TagGpsLatitude,
			ExifInterface.TagGpsLatitudeRef,
			ExifInterface.TagGpsLongitude,
			ExifInterface.TagGpsLongitudeRef,
			ExifInterface.TagGpsProcessingMethod,
			ExifInterface.TagGpsTimestamp,
			ExifInterface.TagImageLength,
			ExifInterface.TagImageWidth,
			ExifInterface.TagIso,
			ExifInterface.TagMake,
			ExifInterface.TagModel,
			//ExifInterface.TagOrientation,
			//ExifInterface.TagSubsecTime,
			//ExifInterface.TagSubsecTimeDig,
			//ExifInterface.TagSubsecTimeOrig,
			ExifInterface.TagWhiteBalance
		};

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

        public System.IO.Stream OpenFile(string path)
        {
            try
            {
                path = path.Replace("\\","/");
                var extPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                var filePath = System.IO.Path.Combine(extPath, "g4m", path);
                var dirPath = System.IO.Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                System.IO.Stream stream;
                if (!File.Exists(filePath))
                {
                    stream = File.Create(filePath);
                }
                else
                {
                    stream = File.Open(filePath, FileMode.OpenOrCreate);
                }
                return stream;
            }
            catch (Exception e)
            {
                Android.Util.Log.Warn(GetType().ToString(), e.ToString());
            }
            return null;
        }

        public void DeleteFile(string path)
        {
			try
			{
				path = path.Replace("\\","/");
				var extPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				var filePath = System.IO.Path.Combine(extPath, "g4m", path);

				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}
			catch (Exception e)
			{
				Android.Util.Log.Warn(GetType().ToString(), e.ToString());
			}
        }

        public void DeleteDirectory(string path, EventHandler callbcak)
        {
			try
			{
				path = path.Replace("\\","/");
				var extPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				var dirPath = System.IO.Path.Combine(extPath, "g4m", path);

				if (Directory.Exists(dirPath))
				{
					Directory.Delete(dirPath, true);
				}
			}
            catch (Exception e)
            {
                Android.Util.Log.Warn(GetType().ToString(), e.ToString());
            }
            if (callbcak != null)
            {
                callbcak.Invoke(this, EventArgs.Empty);
            }
        }
        
        public static Bitmap LoadAndResizeBitmap(string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                int halfHeight = outHeight;
                int halfWidth = outWidth;
                do
                {
                    halfHeight = (int)(halfHeight / 2);
                    halfWidth = (int)(halfWidth / 2);
                    inSampleSize *= 2;
                } while (halfWidth > width || halfHeight > height);
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = null;
            try {
                resizedBitmap = BitmapFactory.DecodeFile(fileName, options);
            }
            catch
            {
            }

            return resizedBitmap;
        }

        private static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = options.OutHeight;
            float width = options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight && width > reqWidth)
            {
                int halfHeight = (int)(height / 2);
                int halfWidth = (int)(width / 2);

                // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return (int)inSampleSize;
        }

        internal static Bitmap GetPictureWithRotation(Bitmap inBitmap, ExifInterface exif)
        {
            var orientation = (Orientation)exif.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Undefined);

            Bitmap resultBitmap = inBitmap;
            Matrix mtx = new Matrix();

            switch (orientation)
            {
                case Orientation.Undefined: // Nexus 7 landscape...
                    break;
                case Orientation.Normal: // landscape
                    break;
                case Orientation.FlipHorizontal:
                    break;
                case Orientation.Rotate180:
                    mtx.PreRotate(180);
                    resultBitmap = Bitmap.CreateBitmap(resultBitmap, 0, 0, resultBitmap.Width, resultBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                case Orientation.FlipVertical:
                    break;
                case Orientation.Transpose:
                    break;
                case Orientation.Rotate90: // portrait
                    mtx.PreRotate(90);
                    resultBitmap = Bitmap.CreateBitmap(resultBitmap, 0, 0, resultBitmap.Width, resultBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                case Orientation.Transverse:
                    break;
                case Orientation.Rotate270: // might need to flip horizontally too...
                    mtx.PreRotate(270);
                    resultBitmap = Bitmap.CreateBitmap(resultBitmap, 0, 0, resultBitmap.Width, resultBitmap.Height, mtx, false);
                    mtx.Dispose();
                    mtx = null;
                    break;
                default:
                    break;
            }

            return resultBitmap;

        }

        public static Task<string> ResizeAndCompressImage(string filePath, int  maxDimention, int quality)
        {
			ExifInterface exifInterface = new ExifInterface(filePath);
			string [] values = new string[tags.Length];
			for (int i = 0; i < tags.Length; i++) {
				values [i] = exifInterface.GetAttribute (tags [i]);
			}
            var task = Task.Run<string>(() =>
                {
                    var extPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                    var filePathOut = System.IO.Path.Combine(extPath, "g4m", "thumb", Guid.NewGuid().ToString());
                    var dirPath = System.IO.Path.GetDirectoryName(filePathOut);
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    //File.Copy(filePath, filePathOut);

                    var fManager = new FileSaveLoad();
                    Bitmap bmp = LoadAndResizeBitmap(filePath, maxDimention, maxDimention);
                    bmp = GetPictureWithRotation(bmp, exifInterface);
                    var tStream = fManager.OpenFile(filePathOut);
                    bmp.Compress(Bitmap.CompressFormat.Jpeg, quality, tStream);
                    tStream.Flush();
                    tStream.Close();

					ExifInterface innerExif = new ExifInterface(filePathOut);
					for (int i = 0; i < tags.Length; i++) 
                    {
                        if (!string.IsNullOrEmpty(values[i])) 
                        {
						    innerExif.SetAttribute(tags[i], values[i]);
                        }
					}
                    innerExif.SaveAttributes();

                    return filePathOut;
                });
            return task;
        }
    }
}
