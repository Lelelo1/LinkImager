using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Settings;
using System.Threading.Tasks;
using Plugin.Settings.Abstractions;
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
                string applicationkey = FreshMvvm.FreshIOC.Container.Resolve<ISettings>().GetValueOrDefault("appKey", null);
                if(applicationkey == null)
                {
                    Azure azure = new Azure();
                    applicationkey = await azure.GenerateAppKey();
                    // Preferences.Set("appKey", appKey);
                    FreshMvvm.FreshIOC.Container.Resolve<ISettings>().AddOrUpdateValue("appKey", applicationkey);
                }
                applicationKey = applicationkey;
            }
            // tested works. however
            return applicationKey;
        }
        public App()
        {
            InitializeComponent();
            IoCSetUp();
            if (null == FreshMvvm.FreshIOC.Container.Resolve<ISettings>().GetValueOrDefault("initialized", null)) // showing a example project if i'ts the first startup
            {
                MainPage = new AppBar(new MainPage("Example.ii"));
                FreshMvvm.FreshIOC.Container.Resolve<ISettings>().AddOrUpdateValue("initialized", "yes");
            }
            else
            {
                MainPage = new AppBar(new MainPage());
            }
           

        }
        public App(string projectUrl)
        {
            // check if image - then launch with first image as the image
            InitializeComponent();
            IoCSetUp();
            MainPage = new AppBar(new MainPage(projectUrl));
        }
        // launch when appliation is still running - wditing the selected image in
        // project
        private void IoCSetUp()
        {
            FreshMvvm.FreshIOC.Container.Register<Plugin.Settings.Abstractions.ISettings>(Plugin.Settings.CrossSettings.Current);
        }
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
