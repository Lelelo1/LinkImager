using System;
using Xamarin.Forms;
namespace LinkImager.Items
{
    public class MovableImage : Image
    {
        AbsoluteLayout layout;
        public MovableImage(AbsoluteLayout layout, Point intialPosition)
        {
            this.layout = layout;
            layout.Children.Add(this, intialPosition);
            this.Source = ImageSource.FromFile("camera.png");


            TapGestureRecognizer tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += TapRecognizer_Tapped;
            this.GestureRecognizers.Add(tapRecognizer);

            PanGestureRecognizer panRecognizer = new PanGestureRecognizer();
            panRecognizer.PanUpdated += PanRecognizer_PanUpdated;
            this.GestureRecognizers.Add(panRecognizer);
        }

        void TapRecognizer_Tapped(object sender, EventArgs e)
        {


        }


        void PanRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            layout.RaiseChild(this);
            Point point = new Point(AbsoluteLayout.GetLayoutBounds(this).X, AbsoluteLayout.GetLayoutBounds(this).Y);
            point.Offset(e.TotalX, e.TotalY);
            AbsoluteLayout.SetLayoutBounds(this, new Rectangle(point, new Size(this.WidthRequest, this.HeightRequest)));
        }

    }
}
