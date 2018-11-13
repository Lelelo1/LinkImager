﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using Android.Content;
using Java.IO;
using Android.Support.V4.Content;

namespace LinkImager.Droid
{
    [Activity(Label = "LinkImager", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static string projectUrl;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            MR.Gestures.Android.Settings.LicenseKey = "QY37-QU4R-YH9Q-C9NA-6MTS-J5CV-3WE7-MVA4-7FDE-ZF8K-U8GP-FK5R-W6XN";
            // File path = FileProvider.GetUriForFile(Context, this.PackageName, new File(new Uri("name")));
            // Android.Net.Uri uri = FileProvider.GetUriForFile(this, this.PackageName, new File(null));

            if (projectUrl != null)
            {
                LoadApplication(new App(projectUrl));
            }
            else
            {
                LoadApplication(new App());
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {

            base.OnActivityResult(requestCode, resultCode, data);
            projectUrl = data.Data.Path;
            OnCreate(null);

        }
    }
    [Activity]
    [IntentFilter(new string[] { "android.intent.action.VIEW" }, Categories = new string[] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" }, DataHost = "*", DataMimeType = "*/*", DataScheme = "file", DataPathPattern = ".*\\\\.ii")]
    public class LaunchProjectActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("bundle", savedInstanceState);
            MainActivity.projectUrl = Intent.Data.Path;

            StartActivity(intent);
            
        }
    }
}