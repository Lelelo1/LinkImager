﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using FFImageLoading;
using FFImageLoading.Work;
using MR.Gestures;
using Plugin.Media.Abstractions;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;
namespace LinkImager.Items
{
    [Serializable()]
	public class MovableImage : ContainerMovableImage, ISerializable
    {
        public string appKey;
        public MovableImage owner;
        public string imageMediaPath;
        private string imageUrl;
        public string ImageUrl
        {
            get
            {
                return imageMediaPath == null ? imageUrl : imageMediaPath;

            }
            set
            {

                SetImageUrl(value);
                imageMediaPath = null;
                // imageUrl = value;

            }
        }
        private async void SetImageUrl(string value)
        {
            Console.WriteLine("Setting image url: " + value);
            imageUrl = value;

            try
            {
                bool exists = await Azure.Exists(imageUrl);
                if (!exists)
                {
                    imageUrl = StatusImages.ImageDeleted;
                }
                uriImageSource.Uri = new Uri(imageUrl);
                this.GetCachedImage().Source = imageUrl;           
                isVisible(AppBar.showState);

            }
            catch(Exception ex)
            {
                // imageUrl is mediaFile.Path (temporarily) or branch.jpg

            }
        }
        public async Task<string> GetImageUrlAsync()
        {

            if(imageUrl == StatusImages.ImageDeleted)
            {
                return imageUrl;
            }
            else
            {
                bool exists = await Azure.Exists(imageUrl);
                if(exists)
                {
                    return imageUrl;
                }
                else
                {
                    imageUrl = StatusImages.ImageDeleted;
                    return imageUrl;
                }
            }
        }
        private UriImageSource uriImageSource;
        public List<MovableImage> children = new List<MovableImage>();

        private Rectangle rectangle;
        public Rectangle Rectangle
        {
            get
            {
                Rectangle rect = new Rectangle(new Point(rectangle.X, rectangle.Y), new Size(rectangle.Width, rectangle.Height));
                var backgroundImage = MainPage.backgroundImage;
                SizeRequest sizeRequest = backgroundImage.Measure(backgroundImage.Bounds.Width, backgroundImage.Bounds.Height);

                var w = sizeRequest.Request.Width;
                var h = sizeRequest.Request.Height;

                rect.X *= w;
                var addX = (backgroundImage.Bounds.Width / 2) - (w / 2);
                rect.X += addX;
                rect.Y *= h;
                var addY = (backgroundImage.Bounds.Height / 2) - (h / 2);
                rect.Y += addY;
                rect.Width *= w;
                // rect.Width += (backgroundImage.Bounds.Width / 2) - (w / 2);
                rect.Height *= h;
                // rect.Height += (backgroundImage.Bounds.Height / 2) - (h / 2);



                return rect;
            }
            set
            {
                Rectangle rect = value;
                var backgroundImage = MainPage.backgroundImage;
                SizeRequest sizeRequest = backgroundImage.Measure(backgroundImage.Bounds.Width, backgroundImage.Bounds.Height);
                var w = sizeRequest.Request.Width;
                var h = sizeRequest.Request.Height;

                var addX = (backgroundImage.Bounds.Width / 2) - (w / 2);
                rect.X -= addX;
                rect.X /= w;
                rect.Width /= w;

                var addY = (backgroundImage.Bounds.Height / 2) - (h / 2);
                rect.Y -= addY;
                rect.Y /= h;
                rect.Height /= h;

                rectangle = rect;

            }
        }
        public MovableImage(string imageUrl)
        {
            this.imageUrl = imageUrl;
            SetAppKey();
        }

        public MovableImage(MR.Gestures.AbsoluteLayout absolute, MovableImage owner, Rectangle rectangle)
        {
            SetAppKey();
            MainPage.absolute = absolute;
            this.owner = owner;
            this.GetCachedImage().Source = "camera.png";
            Rectangle = rectangle;
            AssignEventHandlersWhenVisible();
        }

        // Differentiating when movableImage is visible or not
        private void AssignEventHandlersWhenVisible()
        {
            if(Device.RuntimePlatform == Device.Android)
            {
                this.Down -= Handle_Down;
                this.Down -= Handle_DowniOS;
                this.Tapped -= Handle_Tapped;
                this.LongPressed -= Handle_LongPressed;
                this.Tapped -= Handle_TappedWhenInVisible;

                this.Down += Handle_Down;
                this.Tapped += Handle_Tapped;
                this.LongPressed += Handle_LongPressed;
            }
            else
            {
                this.Down -= Handle_DowniOS;
                // MainPage.absolute.Up -= Handle_UpiOS;
                this.Tapped -= Handle_Tapped;
                this.Tapped -= Handle_TappedWhenInVisible;
                this.LongPressed -= Handle_LongPressed;
                this.Panning -= Handle_Panning;
                this.Panned -= Handle_Panned;
                this.Swiped -= Handle_Swiped;

                this.Down += Handle_DowniOS;
                // MainPage.absolute.Up += Handle_UpiOS;
                this.Tapped += Handle_Tapped;
                this.LongPressed += Handle_LongPressed;
                this.Panning += Handle_Panning;
                this.Panned += Handle_Panned;
                this.Swiped += Handle_Swiped;
            }

        }


        private void AssignEventHandlersWhenInVisible()
        {
            this.Down -= Handle_Down;
            this.Tapped -= Handle_Tapped;
            this.LongPressed -= Handle_LongPressed;
            this.Tapped -= Handle_TappedWhenInVisible;
            this.Down -= Handle_DowniOS;
            // MainPage.absolute.Up -= Handle_UpiOS;
            this.Panning -= Handle_Panning;
            this.Panned -= Handle_Panned;
            this.Swiped -= Handle_Swiped;

            this.Tapped += Handle_TappedWhenInVisible;
            this.LongPressed += null;
            this.Swiped += null;
        }
        // touch eventshandlers...
        private void Handle_TappedWhenInVisible(object sender, TapEventArgs e)
        {
           // App.Current.MainPage.DisplayAlert("Tapped", " you tapped invisible", "ok");

            if(ImageUrl != null)
            {

                MainPage.Display(this);
            }
            // MainPage.actionOrigin = null;
            // MainPage.mainPage.AssignGestures(); // this and downiOS occur both on android
        }

        protected virtual void Handle_DowniOS(object sender, DownUpEventArgs e)
        {
            MainPage.mainPage.DeAssignGestures();
        }
        
        // dragging is listened for on absolute when down on this
        void Handle_Down(object sender, DownUpEventArgs e)
        {
            // MainPage.actionOrigin = this;
            MainPage.mainPage.DeAssignGestures();

            if(Device.RuntimePlatform == Device.Android)
            {
                // MainPage.absolute.Tapped += Handle_Tapped;
                MainPage.absolute.Panning += Absolute_Panning; // can be set to Handle_Panning directly?
                MainPage.absolute.Panned += Handle_Panned; // used to deselect actionorigin
                MainPage.absolute.Swiped += Absolute_Swiped;
                MainPage.absolute.Up += Absolute_Up;
            }


        }

        void Absolute_Up(object sender, DownUpEventArgs e)
        {
            MainPage.absolute.Tapped -= Handle_Tapped;
            MainPage.absolute.Panning -= Absolute_Panning;
            MainPage.absolute.Panned -= Handle_Panned;
            MainPage.absolute.Up -= Absolute_Up;
            MainPage.absolute.Swiped -= Absolute_Swiped;
            // MainPage.actionOrigin = null;
            // MainPage.mainPage.AssignGestures();
            // App.Current.MainPage.DisplayAlert("Up", "Absolute up", "ok");

        }


        async void Handle_Tapped(object sender, TapEventArgs e)
        {

            // App.Current.MainPage.DisplayAlert("Tapped", " you tapped", "ok");
            if(imageUrl == null)
            {
                Plugin.Media.Abstractions.MediaFile mediaFile = await Actions.TakePhoto();
                if (mediaFile != null)
                {
                    imageMediaPath = mediaFile.Path;
                    isVisible(ShowState.IsHidden);
                    Thread thread = new Thread(() =>
                    {
                        /*
                        Task<string> mediaUploadTask = MainPage.MediaUploadAsync(mediaFile, this);
                        MainPage.mediaUploadProccesses.Add(mediaUploadTask);
                        await mediaUploadTask;
                        MainPage.mediaUploadProccesses.Remove(mediaUploadTask);
                        return;
                        */
                        MediaUpload(mediaFile);                       
                    });
                    thread.Start();
                    // this.Source = ImageSource.FromUri(new Uri(url));

                }
            }
            else
            {
                // do what when tapping shown, display context menu - open image
                // in editor
            }
            // MainPage.actionOrigin = null;
            MainPage.mainPage.AssignGestures();
        }

        async void Handle_LongPressed(object sender, LongPressEventArgs e)
        {

            // create a context menu here when implementing like in MainPage when giving edit option etc
            // App.Current.MainPage.DisplayAlert("LongPressed", " you longpressed", "ok");
            MediaFile mediaFile = await Actions.PickPhoto();
            if(mediaFile != null)
            {
                imageMediaPath = mediaFile.Path;
                isVisible(ShowState.IsHidden);
                Thread thread = new Thread(() =>
                {

                    /*
                    Task<string> mediaUploadTask = MainPage.MediaUploadAsync(mediaFile, this);
                    MainPage.mediaUploadProccesses.Add(mediaUploadTask);
                    await mediaUploadTask;
                    MainPage.mediaUploadProccesses.Remove(mediaUploadTask);
                    return;
                    */
                    MediaUpload(mediaFile);                  
                });
                thread.Start();

            }
            // MainPage.actionOrigin = null;
            MainPage.mainPage.AssignGestures();
        }

        //using absolute pan cordinate then used by this
        void Absolute_Panning(object sender, PanEventArgs e)
        {
            Handle_Panning(sender, e);
        }
        void Handle_Panning(object sender, PanEventArgs e)
        {


            if(Device.RuntimePlatform == Device.Android)
            {
                // Size size = MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Size;
                Size size = Rectangle.Size;
                // Point point = new Point(MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).X, MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Y);
                Point point = Rectangle.Location;
                Point newPoint = point.Offset(e.DeltaDistance.X, e.DeltaDistance.Y);
                Rectangle = new Rectangle(newPoint, size);
                MR.Gestures.AbsoluteLayout.SetLayoutBounds(this, Rectangle);

                MainPage.absolute.RaiseChild(this);
                // Rectangle = new Rectangle(newPoint, rectangle.Size); //
            }
            else if(Device.RuntimePlatform == Device.iOS)
            {
                // Size size = MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Size;
                Size size = Rectangle.Size;
                // Point point = new Point(MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).X, MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Y);
                Point point = Rectangle.Location;
                Point newPoint = point.Offset(e.TotalDistance.X, e.TotalDistance.Y);
                Rectangle = new Rectangle(newPoint, size);
                MR.Gestures.AbsoluteLayout.SetLayoutBounds(this, Rectangle);

                MainPage.absolute.RaiseChild(this);
                // Rectangle = new Rectangle(newPoint, rectangle.Size); //
                // MainPage.actionOrigin = null;
                // MainPage.mainPage.AssignGestures(); // maybe not here
            }
        }
        // for deselection only
        void Handle_Panned(object sender, PanEventArgs e)
        {
            // MainPage.actionOrigin = null;
            MainPage.mainPage.AssignGestures();
        }

        void Absolute_Swiped(object sender, SwipeEventArgs e)
        {
            Handle_Swiped(sender, e);
        }


        async void Handle_Swiped(object sender, SwipeEventArgs e)
        {

            // App.Current.MainPage.DisplayAlert("Swiped", " you swiped", "ok");
            if(imageUrl == null || imageUrl == "camera.png" || imageUrl == StatusImages.ImageDeleted)
            {
                // MainPage.actionOrigin = null;
                this.Opacity = 0;
                MainPage.absolute.Children.Remove(this);
                // need to be removed from parent
                if(owner != null)
                {
                    owner.children.Remove(this);
                }
            }
            else
            {

                // MainPage.actionOrigin = null;
                this.Opacity = 0;
                MainPage.absolute.Children.Remove(this);
                string applicationKey = App.GetApplicationKey();
                // if is author
                if (applicationKey == this.appKey)
                {
                    /*
                    if (MainPage.mediaUploadProccesses.Count >= 1)
                    {
                        await MainPage.mediaUploadProccesses[MainPage.mediaUploadProccesses.Count - 1];
                    }*/
                    // await Task.WhenAll(MainPage.mediaUploadProccesses);
                    if(MediaUploadTask != null)
                    {
                        await MediaUploadTask;
                    }

                    Azure azure = new Azure();
                    await azure.DeleteFileFromStorage(this.ImageUrl);
                }
                else
                {
                    // is not author - don't remove from cloud
                    // await App.Current.MainPage.DisplayAlert("Not author", "Not author so image was not removed from cloud", "ok");
                }
                // need to be removed from parent
                if (owner != null)
                {
                    owner.children.Remove(this);
                }
            }
            // MainPage.actionOrigin = null;
            MainPage.mainPage.AssignGestures();
        }
        // other methods
        private void SetAppKey()
        {
           appKey = App.GetApplicationKey();
        }
        public void isVisible(ShowState showState)
        {
            if(showState == ShowState.IsShown)
            {
                this.Opacity = 1;
                if(ImageUrl == null)
                {
                    // this.Source = Xamarin.Forms.ImageSource.FromFile("camera.png");
                    this.GetCachedImage().Source = "camera.png";
                }
                else
                {
                    this.GetCachedImage().Source = ImageUrl;
                }
                AssignEventHandlersWhenVisible();
            }
            else if(showState == ShowState.IsHinted)
            {
                if (ImageUrl == null)
                {
                    this.GetCachedImage().Source = "camera.png"; // check selected LinkType, image, audio video
                }
                else
                {
                    this.GetCachedImage().Source = ImageUrl;
                }
                this.Opacity = 0.4;
                AssignEventHandlersWhenInVisible();
            }
            else if(showState == ShowState.IsHidden)
            {

                this.GetCachedImage().Source = "transparent.png";
                this.Opacity = 1;
                AssignEventHandlersWhenInVisible();
            }
             
        }
        /*
        public void isLinkType(LinkType linkTo)
        {
            if(LinkType.Image == linkTo)
            {
                if (this.Source == Xamarin.Forms.ImageSource.FromFile("camera.png"))
                {

                }
                else if(this.Source == Xamarin.Forms.ImageSource.FromFile("audio.png") || this.Source == Xamarin.Forms.ImageSource.FromFile("video.png"))
                {
                    this.Source = Xamarin.Forms.ImageSource.FromFile("camera.png");
                }
            }
            else if(LinkType.Sound == linkTo)
            {
                if(this.Source == Xamarin.Forms.ImageSource.FromFile("audio.png"))
                {

                }
                else if(this.Source == Xamarin.Forms.ImageSource.FromFile("camera.png") || this.Source == Xamarin.Forms.ImageSource.FromFile("video.png"))
                {
                    this.Source = Xamarin.Forms.ImageSource.FromFile("audio.png");
                }
            }
            else if(LinkType.Video == linkTo)
            {
                if(this.Source == Xamarin.Forms.ImageSource.FromFile("video.png"))
                {

                }
                else if(this.Source == Xamarin.Forms.ImageSource.FromFile("camera.png") || this.Source == Xamarin.Forms.ImageSource.FromFile("audio.png"))
                {
                    this.Source = Xamarin.Forms.ImageSource.FromFile("video.png");
                }
            }
        }
        */
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("appKey", appKey);
            info.AddValue("owner", owner);
            info.AddValue("imageUrl", ImageUrl);
            info.AddValue("children", children);
            info.AddValue("x", rectangle.X);
            info.AddValue("y", rectangle.Y);
            info.AddValue("width", rectangle.Width);
            info.AddValue("height", rectangle.Height);


        }
        public MovableImage(SerializationInfo info, StreamingContext context)
        {
            appKey = (string)info.GetValue("appKey", typeof(string));
            owner = (MovableImage)info.GetValue("owner", typeof(MovableImage));
            string url = (string)info.GetValue("imageUrl", typeof(string));
            Console.WriteLine("Deserilizing");
            imageUrl = url;
            new Thread(() => SetImageUrl(url)).Start();
            children = (List<MovableImage>)info.GetValue("children", typeof(List<MovableImage>));
            double x = (double)info.GetValue("x", typeof(double));
            double y = (double)info.GetValue("y", typeof(double));
            double width = (double)info.GetValue("width", typeof(double));
            double height = (double)info.GetValue("height", typeof(double));
            rectangle = new Rectangle(new Point(x, y), new Size(width, height));

            AssignEventHandlersWhenInVisible();

        }

            // getting the ultimate owner
            public MovableImage GetProject()
        {
            MovableImage temp = this;
            while(temp.owner != null)
            {
                temp = temp.owner;
            }
            return temp;
        }
        // if is displayed and to delete a child
        public void Remove(MovableImage directChild)
        {
            MainPage.absolute.Children.Remove(directChild);
            this.children.Remove(directChild);
        }
        public Task<string> MediaUploadTask;
        public void MediaUpload(MediaFile mediaFile) // await MediaUploadTask
        {
            MediaUploadTask = mediaUploadTask(mediaFile);
        }
        public static List<Task<string>> mediaUploadProccesses = new List<Task<string>>();
        private async Task<string> mediaUploadTask(MediaFile mediaFile)
        {
            Azure azure = new Azure();
            MediaUploadTask = azure.UploadFileToStorage(mediaFile);
            mediaUploadProccesses.Add(MediaUploadTask);
            ImageUrl = await MediaUploadTask;
            mediaUploadProccesses.Remove(MediaUploadTask);
            return ImageUrl;
        }
    }
}
