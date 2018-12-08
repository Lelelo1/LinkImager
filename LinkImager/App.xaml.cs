using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Settings;
using System.Threading.Tasks;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LinkImager
{
    // solve issue with app restarting
    public partial class App : Application
    {
        // used to have one uniqe string per app and mark/sign all resources/images
        private static string applicationKey;
        public static async Task<string> GetApplicationKey()
        {
            if(applicationKey == null)
            {
                // string appkey = Preferences.Get("appKey", null);
                string applicationkey = CrossSettings.Current.GetValueOrDefault("appKey", null);

                if(applicationkey == null)
                {
                    Azure azure = new Azure();
                    applicationkey = await azure.GenerateAppKey();

                    azure.UploadMediaReference(applicationkey, "key");
                    // Preferences.Set("appKey", appKey);
                    CrossSettings.Current.AddOrUpdateValue("appKey", applicationkey);
                }
                applicationKey = applicationkey;
            }
            // tested works. however
            return applicationKey;
        }
        public App()
        {
            InitializeComponent();
            MainPage = new AppBar(new MainPage());
        }
        public App(string projectUrl)
        {
            // check if image - then launch with first image as the image
            InitializeComponent();
            MainPage = new AppBar(new MainPage(projectUrl));
        }
        // launch when appliation is still running - wditing the selected image in
        // project
        public static void LaunchWithImage(string url)
        {

        }
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
 
        }
    }
}
