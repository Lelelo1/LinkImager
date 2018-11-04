using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using LinkImager.Items;
using MR.Gestures;
using Xamarin.Forms;

namespace LinkImager
{
    public partial class MainPage : Xamarin.Forms.ContentPage
    {
        static bool isHandlingMoveableImage = false;
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
        public void AssignGestures()
        {
            
        }
    }
}
