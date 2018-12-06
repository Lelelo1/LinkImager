using System;
using LinkImager.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ExternalStorage))]
namespace LinkImager.Droid
{
    public class ExternalStorage : IExternalStorage
    {
        public string Get()
        {
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).Path;
        }
    }
}
