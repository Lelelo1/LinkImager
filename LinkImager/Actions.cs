using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using LinkImager.Items;
using Plugin.FilePicker.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.ShareFile;
using Xamarin.Forms;
namespace LinkImager
{
    public class Actions
    {


        public static async Task<MediaFile> TakePhoto()
        {
            bool initialized = await CrossMedia.Current.Initialize();
            if (!initialized)
            {
                // throw new Exception("Unable to initialize Plugin.Media CrossMedia.Current in TakePhoto method");
            }
            else
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("Unavailable camera", "Camera, or take photo is not available on this device", "ok");
                    // throw new Exception("Unavailable camera or taking photo");
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
                // throw new Exception("Unable to initialize Plugin.Media CrossMedia.Current in TakePhoto method");
            }
            else
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("Photo picking","Picking of photo is not available on this device", "ok");
                    // throw new Exception("Unavailable picking of photos");
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

            if (project.ImageUrl == null)// || movableImage.imageUrl == "branch.jpg"
            {
                await App.Current.MainPage.DisplayAlert("Empty", "Build your project first before saving it or sending it", "ok");
            }
            else
            {
                // FileData fileData = await Plugin.FilePicker.CrossFilePicker.Current();
                // string tempDir = Xamarin.Essentials.FileSystem.AppDataDirectory;
                // string tempDir = System.IO.Path.GetTempPath();
                /*
                string tempDir = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                tempDir = Path.Combine(tempDir, "Pictures");
                */
                /*
                var x = Plugin.FileSystem.CrossFileSystem.Current.LocalStorage;
                tempDir = x.FullName;
                */
                string tempDir = Xamarin.Essentials.FileSystem.CacheDirectory;
                // Xamarin.Essentials.FileSystem.
                // var appdata = Xamarin.Essentials.FileSystem.AppDataDirectory;
                // var appcache = Xamarin.Essentials.FileSystem.CacheDirectory;
                // Environment.
                string name = await InputAlertBox.InputBox(App.Current.MainPage.Navigation);

                if (!string.IsNullOrEmpty(name))
                {
                    if(MainPage.mediaUploadProccesses.Count >= 1)
                    {
                        await MainPage.mediaUploadProccesses[MainPage.mediaUploadProccesses.Count - 1];
                    }
                    name += ".ii";
                    string fullPath = Path.Combine(tempDir, name);
                    Stream stream = File.Open(fullPath, FileMode.Create);
                    BinaryFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(stream, project);
                    stream.Close();

                    bool existed = File.Exists(fullPath);
                    // CrossShareFile.Current.ShareLocalFile(fullPath);
                    if(Device.RuntimePlatform == Device.iOS)
                    {
                        CrossShareFile.Current.ShareLocalFile(fullPath);
                    }
                    else
                    {
                        // https://xamarinhelp.com/share-dialog-xamarin-forms/
                        string p = DependencyService.Get<IExternalStorage>().Get();

                        await DependencyService.Get<IShare>().Show("title", "message", p + System.IO.Path.DirectorySeparatorChar + name);
                    }



                    
                }


            }
        }
        public static MovableImage ProjectFrom(string path)
        {
            string tempName = "temp.ii";
            string tempDir = System.IO.Path.GetTempPath();
            string fullPath = Path.Combine(tempDir, tempName);
            File.Copy(path, fullPath, true);
            Stream stream = File.Open(fullPath, FileMode.Open);
            // Stream stream = new FileStream(new, FileAccess.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            MovableImage project = (MovableImage)formatter.Deserialize(stream);
            stream.Close();
            return project;
        }
    }
}
