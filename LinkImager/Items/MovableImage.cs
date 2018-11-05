﻿using System;
using MR.Gestures;
using Xamarin.Forms;
namespace LinkImager.Items
{
    public class MovableImage : MR.Gestures.Image
    {
        

        MR.Gestures.AbsoluteLayout layout;
        public MovableImage(MR.Gestures.AbsoluteLayout layout, Point intialPosition)
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
            this.Tapped += Handle_Tapped;
            this.LongPressed += Handle_LongPressed;
            this.Panning += Handle_Panning;
            this.Swiped += Handle_Swiped;
        }
        private void AssignListenersWhenInVisible()
        {
            this.Tapped += Handle_Tapped;
            this.LongPressed += null;;
            this.Panning += null;
            this.Swiped += null;
        }
        // touch eventshandlers...
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
        public bool isVisible()
        {
            double op = Math.Round(this.Opacity);
            int opacity = Convert.ToInt32(op);
            return opacity == 100 ? true : false;

        }
    }
}