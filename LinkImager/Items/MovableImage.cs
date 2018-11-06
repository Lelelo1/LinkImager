﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MR.Gestures;
using Xamarin.Forms;
namespace LinkImager.Items
{
    [Serializable()]
	public class MovableImage : MR.Gestures.Image, ISerializable
    {

        MovableImage owner;
        public string imageUrl;
        public List<MovableImage> children = new List<MovableImage>();
        public Rectangle rectangle;
        MR.Gestures.AbsoluteLayout absolute;
        public MovableImage(string imageUrl)
        {
            this.imageUrl = imageUrl;
        }

        public MovableImage(MR.Gestures.AbsoluteLayout absolute, MovableImage owner, Rectangle rectangle)
        {
            this.absolute = absolute;
            this.owner = owner;
            this.Source = ImageSource.FromFile("camera.png");
            this.rectangle = rectangle;
            Xamarin.Forms.Button b = new Xamarin.Forms.Button();

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
                this.Swiped -= Handle_Swiped;

                this.Down += Handle_DowniOS;
                this.Up += Handle_UpiOS;
                this.Tapped += Handle_Tapped;
                this.LongPressed += Handle_LongPressed;
                this.Panning += Handle_Panning;
                this.Swiped += Handle_Swiped;
            }

        }

        void Handle_Up2(object sender, DownUpEventArgs e)
        {
        }


        private void AssignEventHandlersWhenInVisible()
        {
            this.Down -= Handle_Down;
            this.Tapped -= Handle_Tapped;
            this.LongPressed -= Handle_LongPressed;
            this.Tapped -= Handle_TappedWhenInVisible;
            this.Down -= Handle_DowniOS;

            this.Panning -= Handle_Panning;
            this.Swiped -= Handle_Swiped;



            this.Tapped += Handle_TappedWhenInVisible;
            this.LongPressed += null;
            this.Swiped += null;
        }
        // touch eventshandlers...
        private void Handle_TappedWhenInVisible(object sender, TapEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Tapped", " you tapped invisible", "ok");

            if(imageUrl != null)
            {
                MainPage.Display(this);
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
                absolute.Panning += Absolute_Panning;
                absolute.Swiped += Absolute_Swiped;
                absolute.Up += Absolute_Up;
            }


        }

        void Handle_UpiOS(object sender, DownUpEventArgs e)
        {
            MainPage.actionOrigin = null;
        }


        // deselect
        void Handle_Up(object sender, DownUpEventArgs e)
        {
            MainPage.actionOrigin = null;
            this.Panning -= Handle_Panning;
            this.Swiped -= Handle_Swiped;
            this.Up -= Handle_Up;
        }

        void Absolute_Up(object sender, DownUpEventArgs e)
        {
            MainPage.actionOrigin = null;
            absolute.Panning -= Absolute_Panning;
            absolute.Up -= Absolute_Up;
            absolute.Swiped -= Absolute_Swiped;
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
                    string url = await azure.UploadFileToStorage(mediaFile.GetStream());
                    this.imageUrl = url;
                    this.Source = ImageSource.FromUri(new Uri(url));
                    isVisible(false);
                }
            }
            else
            {
                // do what when tapping shown, display context menu - open image
                // in editor
            }
        }

        void Handle_LongPressed(object sender, LongPressEventArgs e)
        {
            // App.Current.MainPage.DisplayAlert("LongPressed", " you longpressed", "ok");
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
                Size size = MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Size;
                Point point = new Point(MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).X, MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Y);
                Point newPoint = point.Offset(e.DeltaDistance.X, e.DeltaDistance.Y);
                MR.Gestures.AbsoluteLayout.SetLayoutBounds(this, new Rectangle(newPoint, size));
                absolute.RaiseChild(this);
                rectangle = new Rectangle(newPoint, rectangle.Size);
            }
            else if(Device.RuntimePlatform == Device.iOS)
            {
                Size size = MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Size;
                Point point = new Point(MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).X, MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Y);
                Point newPoint = point.Offset(e.TotalDistance.X, e.TotalDistance.Y);
                MR.Gestures.AbsoluteLayout.SetLayoutBounds(this, new Rectangle(newPoint, size));
                absolute.RaiseChild(this);
                rectangle = new Rectangle(newPoint, rectangle.Size);
            }
        }

        void Absolute_Swiped(object sender, SwipeEventArgs e)
        {
            Handle_Swiped(sender, e);
        }


        void Handle_Swiped(object sender, SwipeEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Swiped", " you swiped", "ok");
        }
        // other methods
 
        public void isVisible(bool show)
        {
            if(show)
            {
                AssignEventHandlersWhenVisible();
                if(imageUrl == null)
                {
                    this.Source = ImageSource.FromFile("camera.png");
                }
                else
                {
                    this.Source = ImageSource.FromUri(new Uri(imageUrl));
                }
            }
            else
            {
                // this.Opacity = 0.01;
                this.Source = ImageSource.FromFile("transparent.png");
                AssignEventHandlersWhenInVisible();

            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("owner", owner);
            info.AddValue("imageUrl", imageUrl);
            info.AddValue("children", children);
            info.AddValue("x", rectangle.X);
            info.AddValue("y", rectangle.Y);
            info.AddValue("width", rectangle.Width);
            info.AddValue("height", rectangle.Height);
        }
        public MovableImage(SerializationInfo info, StreamingContext context)
        {
            owner = (MovableImage)info.GetValue("owner", typeof(MovableImage));
            imageUrl = (string)info.GetValue("imageUrl", typeof(string));
            children = (List<MovableImage>)info.GetValue("children", typeof(List<MovableImage>));
            double x = (double)info.GetValue("x", typeof(double));
            double y = (double)info.GetValue("y", typeof(double));
            double width = (double)info.GetValue("width", typeof(double));
            double height = (double)info.GetValue("height", typeof(double));
            rectangle = new Rectangle(new Point(x, y), new Size(width, height));
        }
    }
}
