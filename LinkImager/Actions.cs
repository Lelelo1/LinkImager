using System;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
namespace LinkImager
{
    public class Actions
    {


        public static async Task<MediaFile> TakePhoto()
        {
            bool initialized = await CrossMedia.Current.Initialize();
            if (!initialized)
            {
                throw new Exception("Unable to initialize Plugin.Media CrossMedia.Current in TakePhoto method");
            }
            else
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("Unavailable camera", "Camera, picking photo, or take photo is not available on this device", "ok");
                    throw new Exception("Unavailable camera, picking or taking photo");
                }
                else
                {
                    var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                    {
                        // change this to random string
                        Name = "temp",
                        ModalPresentationStyle = MediaPickerModalPresentationStyle.OverFullScreen,
                        AllowCropping = false,
                        DefaultCamera = CameraDevice.Rear
                    });

                    if (file != null)
                    {
                        return file;
                    }
                }
            }
            return null;
        }
    }
}
