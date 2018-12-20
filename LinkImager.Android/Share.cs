using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Android.Support.V4.Content;
using Android.Webkit;
using LinkImager;
using LinkImager.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(Share))]
namespace LinkImager.Droid
{
    public class Share : IShare
    {
        private readonly Context _context;
        public Share()
        {
            _context = Android.App.Application.Context;
        }

        Task IShare.Show(string title, string message, string filePath)
        {
            var extension = filePath.Substring(filePath.LastIndexOf(".") + 1).ToLower();
            var contentType = string.Empty;

            // You can manually map more ContentTypes here if you want.
            switch (extension)
            {
                case "pdf":
                    contentType = "application/pdf";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                default:
                    // contentType = "application/octetstream";
                    contentType = "image/*";
                    break;
            }

            // var info = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension);

            var intent = new Intent(Intent.ActionSend);
            intent.SetType(contentType);
            var uri = Android.Net.Uri.Parse("file://" + filePath);

            intent.PutExtra(Intent.ExtraStream, uri);

            // Android.OS.Environment.g
            string authority = _context.PackageName + ".fileprovider";
            Java.IO.File file = new Java.IO.File(uri.Path);
            
            // var v = FileProvider.GetUriForFile(_context, authority , file);
           
            intent.PutExtra(Intent.ExtraText, string.Empty);
            intent.PutExtra(Intent.ExtraSubject, message ?? string.Empty);

            var chooserIntent = Intent.CreateChooser(intent, title ?? string.Empty);
            chooserIntent.SetFlags(ActivityFlags.ClearTop);
            chooserIntent.SetFlags(ActivityFlags.NewTask);
            _context.StartActivity(chooserIntent);

            return Task.FromResult(true);
        }
    }
}
