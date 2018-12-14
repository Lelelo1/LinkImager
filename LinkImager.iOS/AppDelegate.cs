using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace LinkImager.iOS
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
            Xamarin.Calabash.Start();
            global::Xamarin.Forms.Forms.Init();
            MR.Gestures.iOS.Settings.LicenseKey = "QY37-QU4R-YH9Q-C9NA-6MTS-J5CV-3WE7-MVA4-7FDE-ZF8K-U8GP-FK5R-W6XN";
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // url.StartAccessingSecurityScopedResource();
            // check if url is image (user has edited image in another program
            App.Current.MainPage = new AppBar(new MainPage(url.Path));
            

            /*
            Xamarin.Calabash.Start();
            global::Xamarin.Forms.Forms.Init();
            MR.Gestures.iOS.Settings.LicenseKey = "QY37-QU4R-YH9Q-C9NA-6MTS-J5CV-3WE7-MVA4-7FDE-ZF8K-U8GP-FK5R-W6XN";
            LoadApplication(new App(url.Path));
            */
            // return base.OpenUrl(application, url, sourceApplication, annotation);
            return true;
        }
    }
}
