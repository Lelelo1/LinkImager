using System;

using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Plugin.Media.Abstractions;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.Azure.Storage;
using LinkImager.Model;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
namespace LinkImager
{
    // handles uploading images to cloud
    public class Azure
    {
        // https://portal.azure.com/#@LeoWsehotmail.onmicrosoft.com/resource/subscriptions/3d0f6e68-d4c0-428b-b0f6-f4f869d6e86f/resourceGroups/LinkImager/providers/Microsoft.Storage/storageAccounts/linkimagerstorageaccount/keys
        private static string storageAccountName = "linkimagerstorageaccount";
        private static string storageKey1 = Secret.Secrets.LinkImager.AZURE_STORAGE_KEY;

        private CloudBlobContainer imageContainer;
        private CloudBlobContainer usersContainer;
        private static MobileServiceClient mobileServiceClient = new MobileServiceClient("https://linkimager.azurewebsites.net");
        public Azure()
        {
            // Create storagecredentials object by reading the values from the configuration (appsettings.json)
            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageKey1);
            // Create cloudstorage account by passing the storagecredentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
            imageContainer = blobClient.GetContainerReference("images");
            usersContainer = blobClient.GetContainerReference("users");
           
        }

        public async Task<string> UploadFileToStorage(MediaFile mediaFile)
        {
            // Get the reference to the block blob from the container
            CloudBlockBlob blockBlob = null;
            bool findUniqueName = true;
            while(findUniqueName)
            {
                string fileName = RandomString(20);
                fileName += ".jpg";
                blockBlob = imageContainer.GetBlockBlobReference(fileName);
                bool existed = await blockBlob.ExistsAsync();
                if (!existed)
                {
                    findUniqueName = false;

                }
            }

            // Upload the file

            await blockBlob.UploadFromStreamAsync(mediaFile.GetStreamWithImageRotatedForExternalStorage());
            string url = blockBlob.Uri.ToString();
            string appKey = App.GetApplicationKey();
            UploadMediaReference(appKey, url);
            return url;
        }
        // making uniqe filename for each imaag uploaded
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
        public async Task<bool> DeleteFileFromStorage(string url)
        {

            CloudBlockBlob blockBlob = imageContainer.GetBlockBlobReference(Path.GetFileName(new Uri(url).LocalPath));
            await blockBlob.DeleteAsync();
            return await blockBlob.ExistsAsync();
        }
        List<Media> MediaUploads() // media uploads
        {
            var text = usersContainer.GetBlockBlobReference("Medias.json").DownloadText();
            // Ôªø added when downloadtext: https://stackoverflow.com/questions/39861943/cloudblockblob-downloadtextasync-returns-invalid-text
            var startIndex = 0;
            while (char.GetUnicodeCategory(text, startIndex) == System.Globalization.UnicodeCategory.Format)
            {
                startIndex++;
            }
            var json = text.Substring(startIndex, text.Length - startIndex);
            var medias = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Media>>(json);
            return medias;
        }
        public void UploadMediaReference(string appKey, string url)
        {
            var medias = MediaUploads();
            medias.Add(new Media(appKey, url));
            var newJson = Newtonsoft.Json.JsonConvert.SerializeObject(medias);
            usersContainer.GetBlockBlobReference("Medias.json").UploadText(newJson);
        }
           
        public string GenerateAppKey()
        {
            var medias = MediaUploads();
            bool findUniqueName = true;
            string appKey = null;
            while (findUniqueName)
            {
                appKey = RandomString(20);
                bool existed = medias.Any(m => m.ApplicationKey == appKey);

                if (!existed)
                {
                    findUniqueName = false;
                }
            }
            return appKey;
        }

        public static async Task<bool> Exists(string url)
        {
            Uri uri = new Uri(url);
            CloudBlockBlob cloudBlockBlob = new CloudBlockBlob(uri);
            return await cloudBlockBlob.ExistsAsync();
        }
    }
}
