using System;
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
        public MovableImage(string imageUrl)
        {
            this.imageUrl = imageUrl;
        }

        public MovableImage(MovableImage owner, Rectangle rectangle)
        {
            this.owner = owner;
            this.Source = ImageSource.FromFile("camera.png");
            this.rectangle = rectangle;
            Xamarin.Forms.Button b = new Xamarin.Forms.Button();

            AssignEventHandlersWhenVisible();
        }

        // Differentiating when movableImage is visible or not
        private void AssignEventHandlersWhenVisible()
        {
            this.Tapped -= Handle_Tapped;
            this.LongPressed -= Handle_LongPressed;
            this.Panning -= Handle_Panning;
            this.Swiped -= Handle_Swiped;
            this.Tapped -= Handle_TappedWhenInVisible;

            this.Tapped += Handle_Tapped;
            this.LongPressed += Handle_LongPressed;
            this.Panning += Handle_Panning;
            this.Swiped += Handle_Swiped;

        }
        private void AssignEventHandlersWhenInVisible()
        {
            this.Tapped -= Handle_Tapped;
            this.LongPressed -= Handle_LongPressed;
            this.Panning -= Handle_Panning;
            this.Swiped -= Handle_Swiped;
            this.Tapped -= Handle_TappedWhenInVisible;

            this.Tapped += Handle_TappedWhenInVisible;
            this.LongPressed += null;
            this.Panning += null;
            this.Swiped += null;
        }
        // touch eventshandlers...
        private void Handle_TappedWhenInVisible(object sender, TapEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Tapped", " you tapped invisible", "ok");
            if(imageUrl != null)
                MainPage.Display(this);

        }

        void Handle_Tapped(object sender, TapEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Tapped", " you tapped", "ok");
        }

        void Handle_LongPressed(object sender, LongPressEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("LongPressed", " you longpressed", "ok");
        }

        void Handle_Panning(object sender, PanEventArgs e)
        {
            Size size = MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Size;
            Point point = new Point(MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).X, MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Y);
            Point newPoint = point.Offset(e.TotalDistance.X, e.TotalDistance.Y);
            MR.Gestures.AbsoluteLayout.SetLayoutBounds(this, new Rectangle(newPoint, size));
            rectangle = new Rectangle(newPoint, rectangle.Size);
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
                this.Source = ImageSource.FromFile("camera.png"); // or load imageURL
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
