using System.Drawing;
using ImageIO;
using AssetsLibrary;

namespace XLabs.Platform.Services.Media
{
	using System;
	using CoreGraphics;
	using System.IO;
	using System.Threading.Tasks;

	using Foundation;
	using UIKit;

    using CoreFoundation;

	using XLabs.Platform.Extensions;

	/// <summary>
	/// Class MediaPickerDelegate.
	/// </summary>
	internal class MediaPickerDelegate : UIImagePickerControllerDelegate
	{
		/// <summary>
		/// The _orientation
		/// </summary>
		private UIDeviceOrientation? _orientation;

		/// <summary>
		/// The _observer
		/// </summary>
		private readonly NSObject _observer;

		/// <summary>
		/// The _options
		/// </summary>
		private readonly MediaStorageOptions _options;

		/// <summary>
		/// The _source
		/// </summary>
		private readonly UIImagePickerControllerSourceType _source;

		/// <summary>
		/// The _TCS
		/// </summary>
		private readonly TaskCompletionSource<MediaFile> _tcs = new TaskCompletionSource<MediaFile>();

		/// <summary>
		/// The _view controller
		/// </summary>
		private readonly UIViewController _viewController;

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaPickerDelegate"/> class.
		/// </summary>
		/// <param name="viewController">The view controller.</param>
		/// <param name="sourceType">Type of the source.</param>
		/// <param name="options">The options.</param>
		internal MediaPickerDelegate(
			UIViewController viewController,
			UIImagePickerControllerSourceType sourceType,
			MediaStorageOptions options)
		{
			_viewController = viewController;
			_source = sourceType;
			_options = options ?? new CameraMediaStorageOptions();

			if (viewController != null)
			{
				UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
				_observer = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, DidRotate);
			}
		}

		/// <summary>
		/// Gets or sets the popover.
		/// </summary>
		/// <value>The popover.</value>
		public UIPopoverController Popover { get; set; }

		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <value>The view.</value>
		public UIView View
		{
			get
			{
				return _viewController.View;
			}
		}

		/// <summary>
		/// Gets the task.
		/// </summary>
		/// <value>The task.</value>
		public Task<MediaFile> Task
		{
			get
			{
				return _tcs.Task;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is captured.
		/// </summary>
		/// <value><c>true</c> if this instance is captured; otherwise, <c>false</c>.</value>
		private bool IsCaptured
		{
			get
			{
				return _source == UIImagePickerControllerSourceType.Camera;
			}
		}

		/// <summary>
		/// Finisheds the picking media.
		/// </summary>
		/// <param name="picker">The picker.</param>
		/// <param name="info">The information.</param>
		/// <exception cref="NotSupportedException"></exception>
		public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
		{
			MediaFile mediaFile;
			switch ((NSString)info[UIImagePickerController.MediaType])
			{
				case MediaPicker.TypeImage:
                    GetPictureMediaFile(info, (mf) =>
                        {
                            Dismiss(picker, () => _tcs.TrySetResult(mf));
                        });
					break;

				case MediaPicker.TypeMovie:
					mediaFile = GetMovieMediaFile(info);
                    Dismiss(picker, () => _tcs.TrySetResult(mediaFile));
					break;

				default:
					throw new NotSupportedException();
			}			
		}

		/// <summary>
		/// Canceleds the specified picker.
		/// </summary>
		/// <param name="picker">The picker.</param>
		public override void Canceled(UIImagePickerController picker)
		{
			Dismiss(picker, () => _tcs.TrySetCanceled());
		}

		/// <summary>
		/// Displays the popover.
		/// </summary>
		/// <param name="hideFirst">if set to <c>true</c> [hide first].</param>
		public void DisplayPopover(bool hideFirst = false)
		{
			if (Popover == null)
			{
				return;
			}

			var swidth = UIScreen.MainScreen.Bounds.Width;
			var sheight = UIScreen.MainScreen.Bounds.Height;

			float width = 400;
			float height = 300;

			if (_orientation == null)
			{
				if (IsValidInterfaceOrientation(UIDevice.CurrentDevice.Orientation))
				{
					_orientation = UIDevice.CurrentDevice.Orientation;
				}
				else
				{
					_orientation = GetDeviceOrientation(_viewController.InterfaceOrientation);
				}
			}

			float x, y;
			if (_orientation == UIDeviceOrientation.LandscapeLeft || _orientation == UIDeviceOrientation.LandscapeRight)
			{
				y = (float)(swidth / 2) - (height / 2);
				x = (float)(sheight / 2) - (width / 2);
			}
			else
			{
				x = (float)(swidth / 2) - (width / 2);
				y = (float)(sheight / 2) - (height / 2);
			}

			if (hideFirst && Popover.PopoverVisible)
			{
				Popover.Dismiss(false);
			}

			Popover.PresentFromRect(new CGRect(x, y, width, height), View, 0, true);
		}

		/// <summary>
		/// Dismisses the specified picker.
		/// </summary>
		/// <param name="picker">The picker.</param>
		/// <param name="onDismiss">The on dismiss.</param>
		private void Dismiss(UIImagePickerController picker, Action onDismiss)
		{
			if (_viewController == null)
			{
				onDismiss();
			}
			else
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_observer);
				UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();

				_observer.Dispose();

				if (Popover != null)
				{
					Popover.Dismiss(true);
					Popover.Dispose();
					Popover = null;

					onDismiss();
				}
				else
				{
					picker.DismissViewController(true, onDismiss);
					picker.Dispose();
				}
			}
		}

		/// <summary>
		/// Dids the rotate.
		/// </summary>
		/// <param name="notice">The notice.</param>
		private void DidRotate(NSNotification notice)
		{
			var device = (UIDevice)notice.Object;
			if (!IsValidInterfaceOrientation(device.Orientation) || Popover == null)
			{
				return;
			}
			if (_orientation.HasValue && IsSameOrientationKind(_orientation.Value, device.Orientation))
			{
				return;
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
			{
				if (!GetShouldRotate6(device.Orientation))
				{
					return;
				}
			}
			else if (!GetShouldRotate(device.Orientation))
			{
				return;
			}

			var co = _orientation;
			_orientation = device.Orientation;

			if (co == null)
			{
				return;
			}

			DisplayPopover(true);
		}

		/// <summary>
		/// Gets the should rotate.
		/// </summary>
		/// <param name="orientation">The orientation.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool GetShouldRotate(UIDeviceOrientation orientation)
		{
			var iorientation = UIInterfaceOrientation.Portrait;
			switch (orientation)
			{
				case UIDeviceOrientation.LandscapeLeft:
					iorientation = UIInterfaceOrientation.LandscapeLeft;
					break;

				case UIDeviceOrientation.LandscapeRight:
					iorientation = UIInterfaceOrientation.LandscapeRight;
					break;

				case UIDeviceOrientation.Portrait:
					iorientation = UIInterfaceOrientation.Portrait;
					break;

				case UIDeviceOrientation.PortraitUpsideDown:
					iorientation = UIInterfaceOrientation.PortraitUpsideDown;
					break;

				default:
					return false;
			}

			return _viewController.ShouldAutorotateToInterfaceOrientation(iorientation);
		}

		/// <summary>
		/// Gets the should rotate6.
		/// </summary>
		/// <param name="orientation">The orientation.</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		private bool GetShouldRotate6(UIDeviceOrientation orientation)
		{
			if (!_viewController.ShouldAutorotate())
			{
				return false;
			}

			var mask = UIInterfaceOrientationMask.Portrait;
			switch (orientation)
			{
				case UIDeviceOrientation.LandscapeLeft:
					mask = UIInterfaceOrientationMask.LandscapeLeft;
					break;

				case UIDeviceOrientation.LandscapeRight:
					mask = UIInterfaceOrientationMask.LandscapeRight;
					break;

				case UIDeviceOrientation.Portrait:
					mask = UIInterfaceOrientationMask.Portrait;
					break;

				case UIDeviceOrientation.PortraitUpsideDown:
					mask = UIInterfaceOrientationMask.PortraitUpsideDown;
					break;

				default:
					return false;
			}

			return _viewController.GetSupportedInterfaceOrientations().HasFlag(mask);
		}

		/// <summary>
		/// Gets the picture media file.
		/// </summary>
		/// <param name="info">The information.</param>
		/// <returns>MediaFile.</returns>
        private void GetPictureMediaFile(NSDictionary info, Action<MediaFile> callback)
        {
            var image = (UIImage)info[UIImagePickerController.OriginalImage];

            var referenceURL = (NSUrl)info[UIImagePickerController.ReferenceUrl];
            if (referenceURL == null) //from camera
            {                
                var metadata = (NSDictionary) info[UIImagePickerController.MediaMetadata];
                ALAssetsLibrary library = new ALAssetsLibrary ();
                library.WriteImageToSavedPhotosAlbum (image.CGImage, metadata, (assetUrl, error) => {
                });
                GetPictureMediaFileWithMetadata(metadata, image, callback);
            }
            else // from albun
            {
                ALAssetsLibrary library = new ALAssetsLibrary();
                library.AssetForUrl(referenceURL, (ALAsset asset) =>
                    {
                        ALAssetRepresentation rep = asset.DefaultRepresentation;
                        NSDictionary metadata = rep.Metadata;
                        var cgimg = rep.GetFullScreenImage();
                        var img = new UIImage(cgimg);
                        GetPictureMediaFileWithMetadata(metadata, img, callback);
                    }, (e) =>
                    {
                    });
            }
        }

        private void GetPictureMediaFileWithMetadata(NSDictionary metadataIn, UIImage image, Action<MediaFile> callback)
        {   
            var metadata = new NSMutableDictionary(metadataIn);
            if (_options.MaxPixelDimension.HasValue)
            {
                float w = 0, h = 0;
                float coeff = Math.Max((float)image.Size.Width, (float)image.Size.Height) / (float)_options.MaxPixelDimension.Value; 
                w = (float)image.Size.Width / coeff;
                h = (float)image.Size.Height / coeff;
                image = MaxResizeImage(image, w, h);
                image = RotateImage (image, metadata);
            }

            //image = RotateImage(image, metadata);

			var path = GetOutputPath(
				MediaPicker.TypeImage,
				_options.Directory ?? ((IsCaptured) ? String.Empty : "temp"),
				_options.Name);

            var compress = _options.PercentQuality.HasValue ? _options.PercentQuality.Value / 100f : 1f;

            var data = GetDataWriteMetadata(image, metadata, compress);

			using (var fs = File.OpenWrite(path))
            using (Stream s = new NsDataStream(data))
			{
				s.CopyTo(fs);
				fs.Flush();
			}

			Action<bool> dispose = null;
			if (_source != UIImagePickerControllerSourceType.Camera)
			{
				dispose = d => File.Delete(path);
			}

            var mf = new MediaFile(path, () => File.OpenRead(path), dispose);
            callback(mf);
		}

        private NSData GetDataWriteMetadata(UIImage originalImage, NSDictionary imageMetadata, float compress)
        {
            //return originalImage.AsJPEG(compress); // turn off efix
            CGImageSource imgSrc = CGImageSource.FromData (originalImage.AsJPEG (compress));
            NSMutableData outImageData = new NSMutableData(); 
            CGImageDestination dest = CGImageDestination.Create(outImageData, MobileCoreServices.UTType.JPEG, 1, new CGImageDestinationOptions());
            dest.AddImage(imgSrc,0,imageMetadata); 
            dest.Close();
            return outImageData;
        }

        public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return sourceImage;
            /*
            float width = (float)(maxResizeFactor * sourceSize.Width);
            float height = (float)(maxResizeFactor * sourceSize.Height);
            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            */
            var resultImage = UIImage.FromImage(sourceImage.CGImage, (float)(1f/maxResizeFactor), sourceImage.Orientation);
            return resultImage;
        }

        public UIImage RotateImage(UIImage sourceImage, NSMutableDictionary imageMetadata)
        {
            try
            {
                CGImage imgRef = sourceImage.CGImage;
                float width = imgRef.Width;
                float height = imgRef.Height;
                CGAffineTransform transform = CGAffineTransform.MakeIdentity();
                RectangleF bounds = new RectangleF(0, 0, width, height);
                SizeF imageSize = new SizeF(width, height);
                float boundHeight;

                var orientation = ((NSNumber)imageMetadata[ImageIO.CGImageProperties.Orientation]).Int32Value;
                imageMetadata.Remove(ImageIO.CGImageProperties.Orientation);

                switch (orientation)
                {
                    case 1:                                        //EXIF = 1
                        return sourceImage;
                        break;

                    case 2:                                //EXIF = 2
                        transform = CGAffineTransform.MakeTranslation(imageSize.Width, 0f);
                        transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                        break;

                    case 3:                                      //EXIF = 3                    
                        transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
                        transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
                        break;

                    case 4:                              //EXIF = 4
                        transform = CGAffineTransform.MakeTranslation(0f, imageSize.Height);
                        transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
                        break;

                    case 5:                              //EXIF = 5
                        boundHeight = bounds.Height;
                        bounds.Height = bounds.Width;
                        bounds.Width = boundHeight;
                        transform = CGAffineTransform.MakeTranslation(imageSize.Height, imageSize.Width);
                        transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                        transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                        break;

                    case 6:                                      //EXIF = 6
                        boundHeight = bounds.Height;
                        bounds.Height = bounds.Width;
                        bounds.Width = boundHeight;
                        transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
                        transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                        break;

                    case 7:                             //EXIF = 7
                        boundHeight = bounds.Height;
                        bounds.Height = bounds.Width;
                        bounds.Width = boundHeight;
                        transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                        transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                        break;

                    case 8:                                     //EXIF = 8
                        boundHeight = bounds.Height;
                        bounds.Height = bounds.Width;
                        bounds.Width = boundHeight;
                        transform = CGAffineTransform.MakeTranslation(0.0f, imageSize.Width);
                        transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                        break;

                    default:
                        return sourceImage;
                        break;
                }

                UIGraphics.BeginImageContext(bounds.Size);

                CGContext context = UIGraphics.GetCurrentContext();

                if (orientation == 8 || orientation == 6)
                {
                    context.ScaleCTM(-1, 1);
                    context.TranslateCTM(-height, 0);
                }
                else
                {
                    context.ScaleCTM(1, -1);
                    context.TranslateCTM(0, -height);
                }

                context.ConcatCTM(transform);
                context.DrawImage(new RectangleF(0, 0, width, height), imgRef);

                UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                return imageCopy;
            }
            catch (Exception e)
            {
                return sourceImage;
            }
        }

        /*
        // resize the image (without trying to maintain aspect ratio)
        public UIImage ResizeImage(UIImage sourceImage, float width, float height)
        {
            UIGraphics.BeginImageContext(new SizeF(width, height));
            sourceImage.Draw(new RectangleF(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }

        // crop the image, without resizing
        private UIImage CropImage(UIImage sourceImage, int crop_x, int crop_y, int width, int height)
        {
            var imgSize = sourceImage.Size;
            UIGraphics.BeginImageContext(new SizeF(width, height));
            var context = UIGraphics.GetCurrentContext();
            var clippedRect = new RectangleF(0, 0, width, height);
            context.ClipToRect(clippedRect);
            var drawRect = new RectangleF(-crop_x, -crop_y, (float)imgSize.Width, (float)imgSize.Height);
            sourceImage.Draw(drawRect);
            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return modifiedImage;
        }
        */

		/// <summary>
		/// Gets the movie media file.
		/// </summary>
		/// <param name="info">The information.</param>
		/// <returns>MediaFile.</returns>
		private MediaFile GetMovieMediaFile(NSDictionary info)
		{
			var url = (NSUrl)info[UIImagePickerController.MediaURL];

			var path = GetOutputPath(
				MediaPicker.TypeMovie,
				_options.Directory ?? ((IsCaptured) ? String.Empty : "temp"),
				_options.Name ?? Path.GetFileName(url.Path));

			File.Move(url.Path, path);

			Action<bool> dispose = null;
			if (_source != UIImagePickerControllerSourceType.Camera)
			{
				dispose = d => File.Delete(path);
			}

			return new MediaFile(path, () => File.OpenRead(path), dispose);
		}

		/// <summary>
		/// Gets the unique path.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="name">The name.</param>
		/// <returns>System.String.</returns>
		private static string GetUniquePath(string type, string path, string name)
		{
			var isPhoto = (type == MediaPicker.TypeImage);
			var ext = Path.GetExtension(name);
			if (ext == String.Empty)
			{
				ext = ((isPhoto) ? ".jpg" : ".mp4");
			}

			name = Path.GetFileNameWithoutExtension(name);

			var nname = name + ext;
			var i = 1;
			while (File.Exists(Path.Combine(path, nname)))
			{
				nname = name + "_" + (i++) + ext;
			}

			return Path.Combine(path, nname);
		}

		/// <summary>
		/// Gets the output path.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		/// <param name="name">The name.</param>
		/// <returns>System.String.</returns>
		private static string GetOutputPath(string type, string path, string name)
		{
			path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);
			Directory.CreateDirectory(path);

			if (String.IsNullOrWhiteSpace(name))
			{
				var timestamp = DateTime.Now.ToString("yyyMMdd_HHmmss");
				if (type == MediaPicker.TypeImage)
				{
					name = "IMG_" + timestamp + ".jpg";
				}
				else
				{
					name = "VID_" + timestamp + ".mp4";
				}
			}

			return Path.Combine(path, GetUniquePath(type, path, name));
		}

		/// <summary>
		/// Determines whether [is valid interface orientation] [the specified self].
		/// </summary>
		/// <param name="self">The self.</param>
		/// <returns><c>true</c> if [is valid interface orientation] [the specified self]; otherwise, <c>false</c>.</returns>
		private static bool IsValidInterfaceOrientation(UIDeviceOrientation self)
		{
			return (self != UIDeviceOrientation.FaceUp && self != UIDeviceOrientation.FaceDown
			        && self != UIDeviceOrientation.Unknown);
	}

		/// <summary>
		/// Determines whether [is same orientation kind] [the specified o1].
		/// </summary>
		/// <param name="o1">The o1.</param>
		/// <param name="o2">The o2.</param>
		/// <returns><c>true</c> if [is same orientation kind] [the specified o1]; otherwise, <c>false</c>.</returns>
		private static bool IsSameOrientationKind(UIDeviceOrientation o1, UIDeviceOrientation o2)
		{
			if (o1 == UIDeviceOrientation.FaceDown || o1 == UIDeviceOrientation.FaceUp)
			{
				return (o2 == UIDeviceOrientation.FaceDown || o2 == UIDeviceOrientation.FaceUp);
			}
			if (o1 == UIDeviceOrientation.LandscapeLeft || o1 == UIDeviceOrientation.LandscapeRight)
			{
				return (o2 == UIDeviceOrientation.LandscapeLeft || o2 == UIDeviceOrientation.LandscapeRight);
			}
			if (o1 == UIDeviceOrientation.Portrait || o1 == UIDeviceOrientation.PortraitUpsideDown)
			{
				return (o2 == UIDeviceOrientation.Portrait || o2 == UIDeviceOrientation.PortraitUpsideDown);
			}

			return false;
		}

		/// <summary>
		/// Gets the device orientation.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <returns>UIDeviceOrientation.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		private static UIDeviceOrientation GetDeviceOrientation(UIInterfaceOrientation self)
		{
			switch (self)
			{
				case UIInterfaceOrientation.LandscapeLeft:
					return UIDeviceOrientation.LandscapeLeft;
				case UIInterfaceOrientation.LandscapeRight:
					return UIDeviceOrientation.LandscapeRight;
				case UIInterfaceOrientation.Portrait:
					return UIDeviceOrientation.Portrait;
				case UIInterfaceOrientation.PortraitUpsideDown:
					return UIDeviceOrientation.PortraitUpsideDown;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}