using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using LinkImager.Items;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.ShareFile;
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
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("Unavailable camera", "Camera, or take photo is not available on this device", "ok");
                    throw new Exception("Unavailable camera or taking photo");
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
        public static async Task<MediaFile> PickPhoto()
        {
            bool initialized = await CrossMedia.Current.Initialize();
            if (!initialized)
            {
                throw new Exception("Unable to initialize Plugin.Media CrossMedia.Current in TakePhoto method");
            }
            else
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("Photo picking","Picking of photo is not available on this device", "ok");
                    throw new Exception("Unavailable picking of photos");
                }
                else
                {
                    var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                    {
                        
                    });

                    if (file != null)
                    {
                        return file;
                    }
                }
            }
            return null;
        }
        public static async void Share(MovableImage project)
        {

            if (project.imageUrl == null)// || movableImage.imageUrl == "branch.jpg"
            {
                await App.Current.MainPage.DisplayAlert("Empty", "Build your project first before saving it or sending it", "ok");
            }
            else
            {
                string tempDir = System.IO.Path.GetTempPath();
                string name = await InputAlertBox.InputBox(App.Current.MainPage.Navigation);
                if (!string.IsNullOrEmpty(name))
                {
                    name += ".ii";
                    string fullPath = Path.Combine(tempDir, name);
                    Stream stream = File.Open(fullPath, FileMode.Create);
                    BinaryFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(stream, project);
                    stream.Close();

                    bool existed = File.Exists(fullPath);

                    CrossShareFile.Current.ShareLocalFile(fullPath, "Title");

                }
            }
        }
    }
}
