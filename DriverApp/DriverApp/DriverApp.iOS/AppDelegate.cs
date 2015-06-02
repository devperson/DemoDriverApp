using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace DriverApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UINavigationBar.Appearance.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0);
            UINavigationBar.Appearance.BarTintColor = UIColor.Red;
            UINavigationBar.Appearance.TintColor = UIColor.Black;
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                TextColor = UIColor.White
            });

            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();

            App.IsDevice = ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.DEVICE;
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
