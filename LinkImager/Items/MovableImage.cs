using System;
using System.Runtime.Serialization;
using MR.Gestures;
using Xamarin.Forms;
namespace LinkImager.Items
{
    
	public class MovableImage : MR.Gestures.Image, ISerializable
    {

        string imageUrl;

        MR.Gestures.AbsoluteLayout layout;
        public MovableImage(MR.Gestures.AbsoluteLayout layout, Point intialPosition, MovableImage owner)
        {
            this.layout = layout;
            layout.Children.Add(this, intialPosition);
            this.Source = ImageSource.FromFile("camera.png");
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
            throw new NotImplementedException();
        }
    }
}
