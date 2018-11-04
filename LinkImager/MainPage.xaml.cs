using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using LinkImager.Items;
namespace LinkImager
{
    public partial class MainPage : ContentPage
    {
        MediaFile mediaFile;

        public MainPage()
        {
            InitializeComponent();
            MovableImage movableImage = new MovableImage(Absolute, new Point(100, 400));
            movableImage.WidthRequest = 50;
            movableImage.HeightRequest = 50;

            MovableImage movableImage2 = new MovableImage(Absolute, new Point(180, 280));
            movableImage.WidthRequest = 50;
            movableImage.HeightRequest = 50;
        }


    }
}
