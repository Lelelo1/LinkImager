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

            this.Down -= Handle_Down;
            this.Tapped -= Handle_Tapped;
            this.LongPressed -= Handle_LongPressed;
            this.Tapped -= Handle_TappedWhenInVisible;

            this.Down += Handle_Down;
            this.Tapped += Handle_Tapped;
            this.LongPressed += Handle_LongPressed;

        }
        private void AssignEventHandlersWhenInVisible()
        {
            this.Down -= Handle_Down;
            this.Tapped -= Handle_Tapped;
            this.LongPressed -= Handle_LongPressed;
            this.Tapped -= Handle_TappedWhenInVisible;

            this.Tapped += Handle_TappedWhenInVisible;
            this.LongPressed += null;
            this.Swiped += null;
        }
        // touch eventshandlers...
        private void Handle_TappedWhenInVisible(object sender, TapEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Tapped", " you tapped invisible", "ok");
            /*
            if(imageUrl != null)
                MainPage.Display(this);
            */
            MainPage.Display(this);
        }

        // dragging is listened for on absolute when down on this
        void Handle_Down(object sender, DownUpEventArgs e)
        {
            MainPage.actionOrigin = this;
            absolute.Panning += Absolute_Panning;
            absolute.Swiped += Absolute_Swiped;
            absolute.Up += Absolute_Up;
        }
        // deselect
        void Absolute_Up(object sender, DownUpEventArgs e)
        {
            MainPage.actionOrigin = null;
            absolute.Panning -= Absolute_Panning;
            absolute.Up -= Absolute_Up;
            absolute.Swiped -= Absolute_Swiped;
            // App.Current.MainPage.DisplayAlert("Up", "Absolute up", "ok");
        }


        void Handle_Tapped(object sender, TapEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("Tapped", " you tapped", "ok");
        }

        void Handle_LongPressed(object sender, LongPressEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("LongPressed", " you longpressed", "ok");
        }

        //using absolute pan cordinate then used by this
        void Absolute_Panning(object sender, PanEventArgs e)
        {
            Handle_Panning(sender, e);
        }
        void Handle_Panning(object sender, PanEventArgs e)
        {

            Size size = MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Size;
            Point point = new Point(MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).X, MR.Gestures.AbsoluteLayout.GetLayoutBounds(this).Y);
            Point newPoint = point.Offset(e.DeltaDistance.X, e.DeltaDistance.Y);
            MR.Gestures.AbsoluteLayout.SetLayoutBounds(this, new Rectangle(newPoint, size));
            rectangle = new Rectangle(newPoint, rectangle.Size);
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
