using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
namespace LinkImager
{
    public partial class MainPage : ContentPage
    {
        MediaFile mediaFile;
        Button OpenCamera = new Button();
        Button UploadAzure = new Button();
        Button DownloadAzure = new Button();
        public MainPage()
        {
            OpenCamera.Text = "OpenCamera";
            UploadAzure.Text = "UploadAzure";
            DownloadAzure.Text = "DownloadAzure";
            InitializeComponent();
            StackLayout TestLayout = new StackLayout();
            TestLayout.Children.Add(OpenCamera);
            TestLayout.Children.Add(UploadAzure);
            TestLayout.Children.Add(DownloadAzure);
            TestLayout.Orientation = StackOrientation.Horizontal;
            Absolute.Children.Add(TestLayout, new Point(0, 500));
            OpenCamera.Clicked += Clicked_OpenCamera;
            UploadAzure.Clicked += Clicked_UploadAzure;
            DownloadAzure.Clicked += DownloadAzure_Clicked;
        }

        void DownloadAzure_Clicked(object sender, EventArgs e)
        {
            Image image = new Image();
            image.Source = ImageSource.FromUri(new Uri("https://linkimagerstorageaccount.blob.core.windows.net/images/DIYJKWVGAQCJVAOZZYOJ.jpg"));

        }


        private async void Clicked_UploadAzure(object sender, EventArgs e)
        {

            Azure azure = new Azure();
            await azure.UploadFileToStorage(mediaFile.GetStream());

        }



        private async void Clicked_OpenCamera(object sender, EventArgs e)
        {
            mediaFile = await Actions.TakePhoto();

        }
    }
}
