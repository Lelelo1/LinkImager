using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Plugin.Media.Abstractions;
using System.Linq;
using System.Text;

namespace LinkImager
{
    // handles uploading images to cloud


    public class Azure
    {
        // https://portal.azure.com/#@LeoWsehotmail.onmicrosoft.com/resource/subscriptions/3d0f6e68-d4c0-428b-b0f6-f4f869d6e86f/resourceGroups/LinkImager/providers/Microsoft.Storage/storageAccounts/linkimagerstorageaccount/keys
        private static string storageAccountName = "linkimagerstorageaccount";
        private static string storageKey1 = "puQEUrYllHVatpZsIdyqiDy1B9TLXBOMFZMU8/W5w/W5wH0UvI6AxYkei30y8X7lEzC6aFG/a6mUdue4rrrmCg==";
        private static string containerName = "images";

        private CloudBlobContainer container;

        public Azure()
        {
            // Create storagecredentials object by reading the values from the configuration (appsettings.json)
            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageKey1);
            // Create cloudstorage account by passing the storagecredentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
            container = blobClient.GetContainerReference(containerName);


        }

        public async Task<string> UploadFileToStorage(Stream stream)
        {

            // Get the reference to the block blob from the container
            CloudBlockBlob blockBlob = null;
            bool findUniqueName = true;
            while(findUniqueName)
            {
                string fileName = RandomString(20);
                fileName += ".jpg";
                blockBlob = container.GetBlockBlobReference(fileName);
                bool existed = await blockBlob.ExistsAsync();
                if (!existed)
                {
                    findUniqueName = false;

                }

            }

            // Upload the file
            await blockBlob.UploadFromStreamAsync(stream);

            return blockBlob.Uri.ToString();
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
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(Path.GetFileName(new Uri(url).LocalPath));
            await blockBlob.DeleteAsync();
            return await blockBlob.ExistsAsync();
        }
    }
}
