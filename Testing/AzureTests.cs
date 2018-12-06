using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using LinkImager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Testing
{
    [TestClass]
    public class AzureTests
    {

       static string url;

        [TestMethod]
        public async Task CanUploadImage()
        {
            string testImagePath = "/Users/hemma/Projects/LinkImager/Testing/ImageFromCamera/temp.jpg";
            Azure azure = new Azure();
            url = await azure.UploadFileToStorage(new Plugin.Media.Abstractions.MediaFile(testImagePath, () => GetStream(testImagePath)));
            System.Diagnostics.Debug.WriteLine(url);

            Assert.IsTrue(new Uri((url)).IsAbsoluteUri);
        }
        private static Stream GetStream(string testImagePath)
        {
            Stream stream = File.OpenRead(testImagePath);
            return stream;
        }
        [TestMethod]
        public async Task CanDelete()
        {
            Azure azure = new Azure();
            // need to extract name and use conteiner.getReference from url
            bool exists = await azure.DeleteFileFromStorage(url);

            Assert.IsFalse(exists);
        }

        [TestMethod] // temp
        public void CanUploadMediaToEasyTables()
        {
            Azure azure = new Azure();
            azure.UploadMediaReference();
        }
    }
}
