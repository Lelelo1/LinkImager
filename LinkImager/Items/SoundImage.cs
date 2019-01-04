using System;
using MR.Gestures;
using Xamarin.Forms;

namespace LinkImager.Items
{

    public class SoundImage : MovableImage
    {
        public SoundImage(MR.Gestures.AbsoluteLayout absolute, MovableImage owner, Rectangle rectangle) : base(absolute, owner, rectangle)
        {

        }

        protected override void Handle_DowniOS(object sender, DownUpEventArgs e)
        {
            base.Handle_DowniOS(sender, e);
        }

    }
}
