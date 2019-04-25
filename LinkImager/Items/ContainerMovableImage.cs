using System;
using FFImageLoading.Forms;
namespace LinkImager.Items
{
    public class ContainerMovableImage : MR.Gestures.ContentView
    {
        CachedImage cachedImage;
        public CachedImage GetCachedImage()
        {
            if (this.cachedImage == null)
            {
                cachedImage = new CachedImage();
                this.Content = cachedImage;
            }
            return cachedImage;
        }
        public ContainerMovableImage()
        {
        }
    }
}
