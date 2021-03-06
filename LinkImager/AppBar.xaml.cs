﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LinkImager.Items;
using Xamarin.Forms;
using Plugin.ShareFile;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
namespace LinkImager
{
    // for text to speech
    // https://medium.com/@dev.aritradas/xamarin-forms-speech-recognition-c16f07cdf164
    public partial class AppBar : NavigationPage
    {
        public AppBar(Page page) : base(page)
        {
            InitializeComponent();
            this.BarBackgroundColor = Color.Black;
            AppBar.SetHasBackButton(this, true);
            this.BarTextColor = Color.White;


        }

        public static ShowState showState = ShowState.IsHidden;
        void Handle_Hint(object sender, System.EventArgs e)
        {
            if(showState == ShowState.IsHidden)
            {
                showState = ShowState.IsHinted;
                MainPage.ToggleVisibilityOfMovableImages(showState);
                hint.Icon = (Xamarin.Forms.FileImageSource)ImageSource.FromFile("hintfilled.png");
            }
            else if(showState == ShowState.IsHinted)
            {
                showState = ShowState.IsHidden;
                MainPage.ToggleVisibilityOfMovableImages(showState);
                hint.Icon = (FileImageSource)ImageSource.FromFile("hint.png");
            }
            else if(showState == ShowState.IsShown)
            {
                show.Icon = (FileImageSource)ImageSource.FromFile("show.png");
                showState = ShowState.IsHinted;
                MainPage.ToggleVisibilityOfMovableImages(showState);
                hint.Icon = (FileImageSource)ImageSource.FromFile("hintfilled.png");
            }

        }
        void Handle_Show(object sender, System.EventArgs e)
        {
            if(showState == ShowState.IsHidden)
            {
                showState = ShowState.IsShown;
                MainPage.ToggleVisibilityOfMovableImages(showState);
                show.Icon = (FileImageSource)ImageSource.FromFile("showfilled.png");
            }
            else if(showState == ShowState.IsHinted)
            {
                hint.Icon = (FileImageSource)ImageSource.FromFile("hint.png");
                showState = ShowState.IsShown;
                MainPage.ToggleVisibilityOfMovableImages(showState);
                show.Icon = (FileImageSource)ImageSource.FromFile("showfilled.png");
            }
            else if(showState == ShowState.IsShown)
            {
                showState = ShowState.IsHidden;
                MainPage.ToggleVisibilityOfMovableImages(showState);
                show.Icon = (FileImageSource)ImageSource.FromFile("show.png");
            }
        }
        void Handle_Share(object sender, System.EventArgs e)
        {

            MovableImage project = MainPage.nowLinkImage.GetProject();
            Actions.Share(project);

            // await App.Current.MainPage.DisplayAlert("info", "url is: " + MainPage.projectUrl, "ok");
        }
        // public static LinkType currentLinkType = LinkType.Image; // used to determine creating area results in camera microphone or video

    }
}
