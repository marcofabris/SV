// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace ManualCameraControls
{
    [Register ("ExposureViewController")]
    partial class ExposureViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider Bias { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView CameraView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider Duration { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider ISO { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NoCamera { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider Offset { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl Segments { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Bias != null) {
                Bias.Dispose ();
                Bias = null;
            }

            if (CameraView != null) {
                CameraView.Dispose ();
                CameraView = null;
            }

            if (Duration != null) {
                Duration.Dispose ();
                Duration = null;
            }

            if (ISO != null) {
                ISO.Dispose ();
                ISO = null;
            }

            if (NoCamera != null) {
                NoCamera.Dispose ();
                NoCamera = null;
            }

            if (Offset != null) {
                Offset.Dispose ();
                Offset = null;
            }

            if (Segments != null) {
                Segments.Dispose ();
                Segments = null;
            }
        }
    }
}