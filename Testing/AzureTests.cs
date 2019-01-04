using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using LinkImager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;

namespace Testing
{
    [TestClass]
    public class AzureTests
    {

       static string url;

        [TestMethod]
        public async Task CanUploadImage()
        {

            FreshMvvm.FreshIOC.Container.Register<ISettings>(new Mock<Plugin.Settings.Abstractions.ISettings>().Object);
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
            try
            {

            }
            catch(Exception ex)
            {

            }
            Azure azure = new Azure();
            // need to extract name and use conteiner.getReference from url
            bool exists = await azure.DeleteFileFromStorage(url);

            Assert.IsFalse(exists);
        }

        /*
        static string exampleAppKey = "test";
        static string exampleUrl = "AUrl";
        Azure azure;
        [TestMethod] 
        public void CanUploadMediaToEasyTables()
        {
            azure = new Azure();
            azure.UploadMediaReference(exampleAppKey, exampleUrl);
        }
        */
                /*
                [TestMethod]
                public async Task CanGenerateUniqueAppKey()
                {

                    azure = new Azure();
                    string s = await azure.GenerateAppKey();
                    System.Diagnostics.Debug.WriteLine("generated appKey: " + s);
                    Assert.IsTrue(exampleAppKey != s);
                }
                */
                /*
                [TestMethod]
                public async Task CanGenerateUniqueAppKeyNotPresentInEasyTables()
                {
                    Azure azure = new Azure();
                    string appKey = await azure.GenerateAppKey();

                }
                */

            }
        }
