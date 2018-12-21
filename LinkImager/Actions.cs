using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LinkImager.Items;
using Plugin.FilePicker;
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
        static byte[] key = { 23, 120, 78, 3, 4, 138, 45, 16};
        static byte[] iv = { 13, 34, 56, 4, 12, 89, 67, 62};
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
                    DESCryptoServiceProvider dESCrypto = new DESCryptoServiceProvider();
                    name += ".ii";
                    string fullPath = Path.Combine(tempDir, name);
                    Stream stream = File.Open(fullPath, FileMode.Create);
                    var d = dESCrypto.CreateEncryptor(key, iv); // can only use DES and 8 bytes for key and iv: https://stackoverflow.com/questions/53816508/system-security-cryptpgraphicunexpectedoperationexception-when-creating-icryptot
                    CryptoStream cryptoStream = new CryptoStream(stream, d, CryptoStreamMode.Write);
                    BinaryFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(cryptoStream, project);
                    cryptoStream.Close();
                    stream.Close();

                    bool existed = File.Exists(fullPath);
                    //CrossShareFile.Current.ShareLocalFile(fullPath); // does no work on old ipad iOS version 9
                    /*
                     * some mrgerstures issues on old ipad
                     *
                     *                    
                    */

                    if(Device.RuntimePlatform == Device.Android)
                    {
                        /*
                        // Plugin.FilePicker.CrossFilePicker.Current.SaveFile
                        FileData fileData = new FileData(tempDir, name, () => { return File.OpenRead(fullPath); }, null);
                        await CrossFilePicker.Current.SaveFile(fileData);
                        */
                        /*
                        string toPath =  DependencyService.Get<IExternalStorage>().Get(); // Documents
                        string sharePath = Path.Combine(toPath, name);
                        var b = File.Exists(toPath);
                        File.Create(sharePath);
                        var c = File.Exists(sharePath);
                        */
                        /*                       
                        if(!File.Exists(toPath))
                        {
                            Directory.CreateDirectory(toPath);
                        }
                        await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                        try
                        {
                            File.Create(sharePath);
                        }catch(Exception ex)
                        {

                        }
                        try
                        {
                            File.Copy(fullPath, sharePath);
                        }
                        catch (Exception ex)
                        {

                        }
                        */
                        // File.(fullPath, toPath);

                        await DependencyService.Get<IShare>().Show("", "", fullPath);
                        // delete sharepath
                        // File.Delete(sharePath);
                    }
                    else
                    {
                        await DependencyService.Get<IShare>().Show("", "", fullPath);
                    }


                }


            }
        }
        public static MovableImage ProjectFrom(string path)
        {
            DESCryptoServiceProvider dESCrypto = new DESCryptoServiceProvider();
            string tempName = "temp.ii";
            string tempDir = System.IO.Path.GetTempPath();
            string fullPath = Path.Combine(tempDir, tempName);
            File.Copy(path, fullPath, true);
            Stream stream = File.Open(fullPath, FileMode.Open);
            var d = dESCrypto.CreateDecryptor(key, iv);
            CryptoStream cryptoStream = new CryptoStream(stream, d, CryptoStreamMode.Read);
            // Stream stream = new FileStream(new, FileAccess.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            MovableImage project = (MovableImage)formatter.Deserialize(cryptoStream);
            cryptoStream.Close();
            stream.Close();
            return project;
        }
    }
}
