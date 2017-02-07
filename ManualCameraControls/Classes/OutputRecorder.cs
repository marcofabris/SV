using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreGraphics;
using CoreImage;
using System.Drawing;

namespace ManualCameraControls
{
	/// <summary>
	/// Helper class that pulls an image from the sample buffer and displays it in the <c>UIImageView</c>
	/// that it has been attached to.
	/// </summary>
	public class OutputRecorder : AVCaptureVideoDataOutputSampleBufferDelegate
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the display view.
		/// </summary>
		/// <value>The display view.</value>
		public UIImageView DisplayView { get; set; }
		#endregion
        public UIImageView SecondaryDisplayView { get; set; }

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ManualCameraControls.OutputRecorder"/> class.
		/// </summary>
		public OutputRecorder ()
		{

		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Gets a single image frame from sample buffer.
		/// </summary>
		/// <returns>The image from sample buffer.</returns>
		/// <param name="sampleBuffer">Sample buffer.</param>
		private UIImage GetImageFromSampleBuffer(CMSampleBuffer sampleBuffer) {

			// Get a pixel buffer from the sample buffer
			using (var pixelBuffer = sampleBuffer.GetImageBuffer () as CVPixelBuffer) {
				// Lock the base address
				pixelBuffer.Lock (CVOptionFlags.None);

				// Prepare to decode buffer
				var flags = CGBitmapFlags.PremultipliedFirst | CGBitmapFlags.ByteOrder32Little;

				// Decode buffer - Create a new colorspace
				using (var cs = CGColorSpace.CreateDeviceRGB ()) {

					// Create new context from buffer
					using (var context = new CGBitmapContext (pixelBuffer.BaseAddress,
						                     pixelBuffer.Width,
						                     pixelBuffer.Height,
						                     8,
						                     pixelBuffer.BytesPerRow,
						                     cs,
						                     (CGImageAlphaInfo)flags)) {

						// Get the image from the context
						using (var cgImage = context.ToImage ()) {

							// Unlock and return image
							pixelBuffer.Unlock (CVOptionFlags.None);
							return UIImage.FromImage (cgImage);
						}
					}
				}
			}
		}
		#endregion

		#region Override Methods
		/// <Docs>The capture output on which the frame was captured.</Docs>
		/// <param name="connection">The connection on which the video frame was received.</param>
		/// <remarks>Unless you need to keep the buffer for longer, you must call
		///  Dispose() on the sampleBuffer before returning. The system
		///  has a limited pool of video frames, and once it runs out of
		///  those buffers, the system will stop calling this method
		///  until the buffers are released.</remarks>
		/// <summary>
		/// Dids the output sample buffer.
		/// </summary>
		/// <param name="captureOutput">Capture output.</param>
		/// <param name="sampleBuffer">Sample buffer.</param>
		public override void DidOutputSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
			// Trap all errors
			try {
				// Grab an image from the buffer
                var image = ApplyFilter(GetImageFromSampleBuffer(sampleBuffer));

				// Display the image
				if (DisplayView !=null) {
					DisplayView.BeginInvokeOnMainThread(() => {
						// Set the image
						var oldImg = DisplayView.Image;
						oldImg?.Dispose ();

						DisplayView.Image = image;

						// Rotate image to the correct display orientation
						//DisplayView.Transform = CGAffineTransform.MakeRotation((float)Math.PI/2);
                        //DisplayView.Transform = CGAffineTransform.MakeRotation((float)Math.PI);


                        if (SecondaryDisplayView != null)
                        {
                            oldImg = SecondaryDisplayView.Image;
                            oldImg?.Dispose();

                            SecondaryDisplayView.Image = image;

                            // Rotate image to the correct display orientation
                            //SecondaryDisplayView.Transform = CGAffineTransform.MakeRotation((float)Math.PI);

                        }
					});
				}
                /*
                if (SecondaryDisplayView != null)
                {
                    SecondaryDisplayView.BeginInvokeOnMainThread(() =>
                    {
                        // Set the image
                        var oldImg = SecondaryDisplayView.Image;
                        oldImg?.Dispose();

                        SecondaryDisplayView.Image = image;

                        // Rotate image to the correct display orientation
                        SecondaryDisplayView.Transform = CGAffineTransform.MakeRotation((float)Math.PI);
                    });
                }*/

				// IMPORTANT: You must release the buffer because AVFoundation has a fixed number
				// of buffers and will stop delivering frames if it runs out.
				sampleBuffer.Dispose();
			}
			catch(Exception e) {
				// Report error
				Console.WriteLine ("Error sampling buffer: {0}", e.Message);
			}
		}
        #endregion

        /*
        UIImage ScaleAndRotateImage(UIImage imageIn, UIImageOrientation orIn)
        {
            int kMaxResolution = 0;

            CGImage imgRef = imageIn.CGImage;
            float width = imgRef.Width;
            float height = imgRef.Height;
            CGAffineTransform transform = CGAffineTransform.MakeIdentity();
            RectangleF bounds = new RectangleF(0, 0, width, height);

            kMaxResolution = (int)width;
            if (DisplayView != null)
                kMaxResolution = 640;
            
            if (width > kMaxResolution || height > kMaxResolution)
            {
                float ratio = width / height;

                if (ratio > 1)
                {
                    bounds.Width = kMaxResolution;
                    bounds.Height = bounds.Width / ratio;
                }
                else
                {
                    bounds.Height = kMaxResolution;
                    bounds.Width = bounds.Height * ratio;
                }
            }

            float scaleRatio = bounds.Width / width;
            SizeF imageSize = new SizeF(width, height);
            UIImageOrientation orient = orIn;
            float boundHeight;

            switch (orient)
            {
                case UIImageOrientation.Up:                                        //EXIF = 1
                    transform = CGAffineTransform.MakeIdentity();
                    break;

                case UIImageOrientation.UpMirrored:                                //EXIF = 2
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, 0f);
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    break;

                case UIImageOrientation.Down:                                      //EXIF = 3
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
                    break;

                case UIImageOrientation.DownMirrored:                              //EXIF = 4
                    transform = CGAffineTransform.MakeTranslation(0f, imageSize.Height);
                    transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
                    break;

                case UIImageOrientation.LeftMirrored:                              //EXIF = 5
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, imageSize.Width);
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.Left:                                      //EXIF = 6
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(0.0f, imageSize.Width);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.RightMirrored:                             //EXIF = 7
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.Right:                                     //EXIF = 8
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                    break;

                default:
                    throw new Exception("Invalid image orientation");
                    break;
            }

            UIGraphics.BeginImageContext(bounds.Size);

            CGContext context = UIGraphics.GetCurrentContext();

            if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left)
            {
                context.ScaleCTM(-scaleRatio, scaleRatio);
                context.TranslateCTM(-height, 0);
            }
            else
            {
                context.ScaleCTM(scaleRatio, -scaleRatio);
                context.TranslateCTM(0, -height);
            }

            context.ConcatCTM(transform);
            context.DrawImage(new RectangleF(0, 0, width, height), imgRef);

            UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return imageCopy;
        }
*/
        private UIImage ApplyFilter(UIImage uiimage)
        {
            return uiimage;

            //need to scale it!
            var ciimage = new CIImage(uiimage);

            var ef1 = new CIHighlightShadowAdjust();
            ef1.Image = ciimage;
            ef1.HighlightAmount = 0.5f;


            /*
            var hueAdjust = new CIHueAdjust();   // first filter
            hueAdjust.Image = ciimage;
            hueAdjust.Angle = 2.094f;
            var sepia = new CISepiaTone();       // second filter
            sepia.Image = hueAdjust.OutputImage; // output from last filter, input to this one
            sepia.Intensity = 0.3f;*/
            CIFilter color = new CIColorControls()
            { // third filter
                Saturation = 0.1f,
                Brightness = 0.2f,
                Contrast = 0.3f,
                Image = ef1.OutputImage    // output from last filter, input to this one
            };
            var output = color.OutputImage;
            var context = CIContext.FromOptions(null);
            // ONLY when CreateCGImage is called do all the effects get rendered
            var cgimage = context.CreateCGImage(output, output.Extent);
            var uiImgOut = UIImage.FromImage(cgimage);
            return uiImgOut;
        }
	}
}

