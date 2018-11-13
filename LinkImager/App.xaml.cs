using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LinkImager
{
    // solve issue with app restarting
    public partial class App : Application
    {
        
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
