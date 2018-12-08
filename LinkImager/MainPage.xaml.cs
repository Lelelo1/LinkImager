﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using LinkImager.Items;
using MR.Gestures;
using Xamarin.Forms;
using FFImageLoading.Forms;
using FFImageLoading.Work;

namespace LinkImager
{
    public partial class MainPage : Xamarin.Forms.ContentPage
    {
        public static MainPage mainPage;
        static Xamarin.Forms.ContentPage contentPage;
        public static MR.Gestures.AbsoluteLayout absolute;
        public static MovableImage nowLinkImage;
        public static CachedImage backgroundImage;
        private static string standardImageName = "branch.jpg";
        public static string projectUrl;
        public MainPage()
        {
            InitializeComponent();
            contentPage = this;
            absolute = Absolute;
            // if not load project data file
            absolute.BackgroundColor = Color.Transparent; 
            nowLinkImage = new MovableImage(standardImageName);
            backgroundimage.Source = Xamarin.Forms.ImageSource.FromFile(nowLinkImage.ImageUrl);
            backgroundImage = backgroundimage;

            this.BackgroundColor = Color.Black;
            AssignGestures();

            mainPage = this;
            // serialize
        }
        public MainPage(string projectUrl)
        {
            
            InitializeComponent();
            contentPage = this;
            absolute = Absolute;
            // if not load project data file
            absolute.BackgroundColor = Color.Transparent;
            nowLinkImage = Actions.ProjectFrom(projectUrl);
            /*
            nowLinkImage = new MovableImage(standardImageName);
            backgroundimage.Source = ImageSource.FromFile(nowLinkImage.imageUrl);
            */
            backgroundImage = backgroundimage;
            this.BackgroundColor = Color.Black;
            AssignGestures();

            mainPage = this;

            Display(nowLinkImage);

        }
        public static async void Display(MovableImage movableImage)
        {
            absolute.Children.Clear();
            nowLinkImage = movableImage;

            // try catch since branch.jpg is not an uri
            Rectangle bounds = new Rectangle(new Point(0, 0), new Size(-1, -1));
            try
            {
                // bounds = await GetBoundsAsync(backgroundImage, movableImage);
                bounds = await ImageWaiter.WaitForBoundsAsync(backgroundImage, nowLinkImage);

            }
            catch(Exception ex)
            {

            }
            await Task.Delay(100); // so that the cachedimage get its properties correct, see movableImage
            var sizeRequest = backgroundImage.Measure(backgroundImage.Bounds.Width, backgroundImage.Bounds.Height);

            if(movableImage.children.Count > 0)
                movableImage.children.ForEach((MovableImage obj) =>
            {
                Paint(obj);
            });
        }
        
        // Paint from paint
        private static void Paint(MovableImage child)
        {


            absolute.Children.Add(child, child.Rectangle);

            // https://linkimagerstorageaccount.blob.core.windows.net/images/UOTZCNNZUZBUEMTJLTXF.jpg
            // https://linkimagerstorageaccount.blob.core.windows.net/images/IVKCMKTNHOVKZLVXNRJB.jpg
            //271 165 63 77
            //213 272 20 16
        }
        public static void Create(Rectangle rectangle)
        {
            /*
            MovableImage movableImage = new MovableImage(absolute, nowLinkImage, rectangle);
            MovableImage childImage = new MovableImage(absolute, movableImage, new Rectangle(new Point(200, 300), new Size(120, 120)));
            movableImage.children.Add(childImage);
            MovableImage otherChildImage = new MovableImage(absolute, movableImage, new Rectangle(new Point(80, 100), new Size(70, 60)));
            movableImage.children.Add(otherChildImage);
            */
            MovableImage child = new MovableImage(absolute, nowLinkImage, rectangle);
            nowLinkImage.children.Add(child);
            Paint(child);

        }
        /* refractored - added to icons to AppBar instead
        ShowState state = ShowState.IsHidden;
        void Button_Clicked(object sender, EventArgs e)
        {
            if (state.)
                isVisible = false;
            else
                isVisible = true;
            ToggleVisibilityOfMovableImages(isVisible);
        }
        */

        /*
        public static void ToggleVisibilityOfMovableImages(ShowState state)
        {

            var list = absolute.Children.ToList().Select((arg) => (MovableImage)arg);
            list.ToList().ForEach((MovableImage obj) => {
                obj.isVisible(state);
            });
            // and also set visibility of those movableimages that arent in absolute
        }
        */
        public static void ToggleVisibilityOfMovableImages(ShowState showState)
        {
            Adjust(nowLinkImage.GetProject(), showState);
        }
        private static void Adjust(MovableImage movableImage, ShowState showState)
        {
            movableImage.isVisible(showState);
            foreach(MovableImage child in movableImage.children)
            {
                Adjust(child, showState);
            }
        }
        public static async void Remove(MovableImage movableImage)
        {
            string url = movableImage.ImageUrl;
            nowLinkImage.ImageUrl = standardImageName;
            Display(nowLinkImage);
            // condition here
            string applicationKey = await App.GetApplicationKey();
            if(applicationKey == movableImage.appKey)
            {
                // Delete from cloud only if is author
                Azure azure = new Azure();
                await azure.DeleteFileFromStorage(url);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Not author", "Not author so image was not removed from cloud", "ok");
            }

        }

        public static MovableImage actionOrigin = null;
        public void AssignGestures()
        {
            Absolute.Down += Absolute_Down;
            Absolute.Tapped += Absolute_Tapped;
            Absolute.DoubleTapped += Absolute_DoubleTapped;
            Absolute.LongPressed += Absolute_LongPressed;
            Absolute.Panned += Absolute_Panned;
            Absolute.Swiped += Absolute_Swiped;
        }
        // used to give panned a start x y position
        private DownUpEventArgs down;
        void Absolute_Down(object sender, DownUpEventArgs e)
        {
            down = e;
            // actionOrigin = null; // using this creates panned down left create bug

        }


        async void Absolute_Tapped(object sender, TapEventArgs e)
        {
            // await App.Current.MainPage.DisplayAlert("info", "url is: " + projectUrl, "ok");

            if(actionOrigin == null)
            {
                if (nowLinkImage.owner == null)
                {
                    if (nowLinkImage.ImageUrl == standardImageName)
                    {
                        MediaFile mediaFile = await Actions.TakePhoto();
                        if (mediaFile != null)
                        {
                            Azure azure = new Azure();
                            string url = await azure.UploadFileToStorage(mediaFile);
                            nowLinkImage.ImageUrl = url;
                            Display(nowLinkImage);
                        }
                    }
                }
            }

        }

        async void Absolute_DoubleTapped(object sender, TapEventArgs e)
        {
            if(nowLinkImage.ImageUrl != standardImageName && nowLinkImage.owner == null)
            {


            }

            string appKey = await App.GetApplicationKey();
            await App.Current.MainPage.DisplayAlert("info", "Your app key is " + appKey,  "ok");
        }


        async void Absolute_LongPressed(object sender, LongPressEventArgs e)
        {

            // await App.Current.MainPage.DisplayAlert("LongPressed", "LongPressed aboslute", "ok");
            /*
            if(nowLinkImage.owner == null)
            {
                if(nowLinkImage.imageUrl == startingImageName)
                {
                    MediaFile mediaFile = await Actions.PickPhoto();
                    if (mediaFile != null)
                    {
                        Azure azure = new Azure();
                        string url = await azure.UploadFileToStorage(mediaFile.GetStream());
                        nowLinkImage.imageUrl = url;
                        Display(nowLinkImage);
                    }
                }
            }
            */
            if(actionOrigin == null)
            {
                if (nowLinkImage.ImageUrl != standardImageName)
                {
                    // string choice = await this.DisplayActionSheet("Image", "Cancel", "Remove", "Edit");
                    string choice = await this.DisplayActionSheet("Image", "Cancel", "Remove");
                    if (choice != null)
                    {
                        if (choice == "Remove")
                        {
                            Remove(nowLinkImage);
                        }
                        else if (choice == "Edit")
                        {
                            // share as image, remember which image - returned image replace
                        }
                    }
                }
                else
                {
                    string choice = await this.DisplayActionSheet("Image", "Cancel", null, "Photos");
                    if (choice != null)
                    {
                        if(choice == "Photos")
                        {
                            MediaFile mediaFile = await Actions.PickPhoto();
                            if(mediaFile != null)
                            {
                                Azure azure = new Azure();
                                string url = await azure.UploadFileToStorage(mediaFile);
                                nowLinkImage.ImageUrl = url;
                                Display(nowLinkImage);
                            }
                        }
                    }
                }
            }
        }

        void Absolute_Panned(object sender, PanEventArgs e)
        {

            if (actionOrigin == null)
            {
                // action occured on absolute
                Rectangle rectangle = new Rectangle(new Point(down.Touches[0].X, down.Touches[0].Y), new Size(e.TotalDistance.X, e.TotalDistance.Y));

                if(rectangle.Width > 30 && rectangle.Height > 30)
                {

                    Create(rectangle);
                }


            }
            else
            {
                // action occured on movable image and is handled there
            }
        }
        void Absolute_Swiped(object sender, SwipeEventArgs e)
        {

            if(actionOrigin == null)
            {
                // action occured on absolute
                if(nowLinkImage.owner != null)
                {

                    Display(nowLinkImage.owner);
                }
            }
            else
            {

                // action occured on movable image and is handled there
            }
        }



    }
    public class ImageWaiter
    {
        // private Func<Task<Rectangle>> func;

        private static TaskCompletionSource<Rectangle> BoundsAwaiter;
        private static EventHandler<CachedImageEvents.SuccessEventArgs> Handle_Success = (s, e) =>
        {
            Rectangle rect = ((CachedImage)s).Bounds;
            var r = e.LoadingResult;
            BoundsAwaiter.SetResult(((CachedImage)s).Bounds);

        };
        public static Task<Rectangle> WaitForBoundsAsync(CachedImage cachedImage, MovableImage displayLinkImage)
        {
            // unregister so that eventhandlers doesn't stack up
            cachedImage.Success -= Handle_Success;
            BoundsAwaiter = new TaskCompletionSource<Rectangle>();
            cachedImage.Success += Handle_Success;

            cachedImage.Source = displayLinkImage.ImageUrl;
            
            return BoundsAwaiter.Task;
        }
    }

}

