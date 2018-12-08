﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MR.Gestures;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
namespace LinkImager.Items
{
    [Serializable()]
	public class MovableImage : MR.Gestures.Image, ISerializable
    {
        private string appKey;
        public MovableImage owner;
        private string imageUrl;
        public string ImageUrl
        {
            get
            {
                return imageUrl;
            }
            set
            {
                https://stackoverflow.com/questions/41487647/xamarin-forms-image-cache
                try
                {
                    // value is url
                    imageUrl = value;
                    if (uriImageSource == null)
                    {
                        uriImageSource = new UriImageSource();
                        uriImageSource.CachingEnabled = true;
                        uriImageSource.CacheValidity = new TimeSpan(1, 0, 0);
                    }
                    uriImageSource.Uri = new Uri(value);
                }
                catch(Exception ex)
                {
                    // value is local file set in MainPage contructor
                    imageUrl = value;
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
            this.Source = ImageSource.FromFile("camera.png");
            Rectangle = rectangle;
            AssignEventHandlersWhenVisible();
        }

        // Differentiating when movableImage is visible or not
        private void AssignEventHandlersWhenVisible()
        {
            if(Device.RuntimePlatform == Device.Android)
            {
                this.Down -= Handle_Down;
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
                this.Up -= Handle_UpiOS;
                this.Tapped -= Handle_Tapped;
                this.Tapped -= Handle_TappedWhenInVisible;
                this.LongPressed -= Handle_LongPressed;
                this.Panning -= Handle_Panning;
                this.Panned -= Handle_Panned;
                this.Swiped -= Handle_Swiped;

                this.Down += Handle_DowniOS;
                this.Up += Handle_UpiOS;
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

            if(imageUrl != null)
            {
                try
                {
                    MainPage.Display(this);
                }catch(InvalidOperationException ex)
                {
                    App.Current.MainPage.DisplayAlert("Error", ex.Message, "ok");
                }
            }
                
                
        }

        void Handle_DowniOS(object sender, DownUpEventArgs e)
        {

            MainPage.actionOrigin = this;

        }


        // dragging is listened for on absolute when down on this
        void Handle_Down(object sender, DownUpEventArgs e)
        {
            MainPage.actionOrigin = this;

            if(Device.RuntimePlatform == Device.Android)
            {
                MainPage.absolute.Panning += Absolute_Panning; // can be set to Handle_Panning directly?
                MainPage.absolute.Panned += Handle_Panned; // used to deselect actionorigin
                MainPage.absolute.Swiped += Absolute_Swiped;
                MainPage.absolute.Up += Absolute_Up;
            }


        }

        void Handle_UpiOS(object sender, DownUpEventArgs e)
        {
            // previously unset actionOrigin
            // but occurs prior to panned in absolute

        }


        // deselect
        void Handle_Up(object sender, DownUpEventArgs e)
        {

            MainPage.actionOrigin = null;
            this.Panning -= Handle_Panning;
            this.Swiped -= Handle_Swiped;
            // this.Up -= Handle_Up;
        }

        void Absolute_Up(object sender, DownUpEventArgs e)
        {
            MainPage.absolute.Panning -= Absolute_Panning;
            MainPage.absolute.Panned -= Handle_Panned;
            MainPage.absolute.Up -= Absolute_Up;
            MainPage.absolute.Swiped -= Absolute_Swiped;
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
                    Azure azure = new Azure();
                    string url = await azure.UploadFileToStorage(mediaFile);
                    // imageUrl = url;
                    // this.Source = ImageSource.FromUri(new Uri(url));
                    ImageUrl = url;
                    this.Source = uriImageSource;
                    isVisible(ShowState.IsHidden);
                }
            }
            else
            {
                // do what when tapping shown, display context menu - open image
                // in editor
            }
            MainPage.actionOrigin = null;
        }

        async void Handle_LongPressed(object sender, LongPressEventArgs e)
        {

            // create a context menu here when implementing like in MainPage when giving edit option etc
            // App.Current.MainPage.DisplayAlert("LongPressed", " you longpressed", "ok");
            MediaFile mediaFile = await Actions.PickPhoto();
            if(mediaFile != null)
            {
                Azure azure = new Azure();
                string url = await azure.UploadFileToStorage(mediaFile);
                ImageUrl = url;
                this.Source = uriImageSource;
                isVisible(ShowState.IsHidden);
            }
            MainPage.actionOrigin = null;
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

            }
        }
        // for deselection only
        void Handle_Panned(object sender, PanEventArgs e)
        {
            MainPage.actionOrigin = null;

        }

        void Absolute_Swiped(object sender, SwipeEventArgs e)
        {
            Handle_Swiped(sender, e);
        }


        async void Handle_Swiped(object sender, SwipeEventArgs e)
        {

            // App.Current.MainPage.DisplayAlert("Swiped", " you swiped", "ok");
            if(imageUrl == null || imageUrl == "camera.png")
            {
                MainPage.actionOrigin = null;
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

                MainPage.actionOrigin = null;
                this.Opacity = 0;
                MainPage.absolute.Children.Remove(this);
                Azure azure = new Azure();
                await azure.DeleteFileFromStorage(this.imageUrl);
                // need to be removed from parent
                if (owner != null)
                {
                    owner.children.Remove(this);
                }
            }

        }
        // other methods
        private async void SetAppKey()
        {
           appKey =  await App.GetAppKey();
        }
        public void isVisible(ShowState showState)
        {
            if(showState == ShowState.IsShown)
            {
                this.Opacity = 1;
                if(imageUrl == null)
                {
                    this.Source = ImageSource.FromFile("camera.png");
                }
                else
                {
                    try
                    {
                        // this.Source = ImageSource.FromUri(new Uri(imageUrl));
                        this.Source = uriImageSource;
                    }
                    catch(Exception ex)
                    {
                        // malformed uri when branch.jpg is shown
                    }

                }
                AssignEventHandlersWhenVisible();
            }
            else if(showState == ShowState.IsHinted)
            {
                if (imageUrl == null)
                {
                    this.Source = ImageSource.FromFile("camera.png");
                }
                else
                {   
                    try
                    {
                        // this.Source = ImageSource.FromUri(new Uri(imageUrl));
                        this.Source = uriImageSource;
                    }
                    catch(Exception ex)
                    {
                        // malformed uri when branch.jpg is shown
                    }
                }
                this.Opacity = 0.4;
                AssignEventHandlersWhenInVisible();
            }
            else if(showState == ShowState.IsHidden)
            {

                this.Source = ImageSource.FromFile("transparent.png");
                this.Opacity = 1;
                AssignEventHandlersWhenInVisible();
            }
             
        }

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
            // imageUrl = (string)info.GetValue("imageUrl", typeof(string));
            ImageUrl = (string)info.GetValue("imageUrl", typeof(string));
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
    }
}
