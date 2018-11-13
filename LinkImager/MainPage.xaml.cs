using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using LinkImager.Items;
using MR.Gestures;
using Xamarin.Forms;
namespace LinkImager
{
    public partial class MainPage : Xamarin.Forms.ContentPage
    {
        public static MainPage mainPage;
        static Xamarin.Forms.ContentPage contentPage;
        public static MR.Gestures.AbsoluteLayout absolute;
        public static MovableImage nowLinkImage;
        static MR.Gestures.Image backgroundImage;
        private static string standardImageName = "branch.jpg";
        public static string projectUrl;
        public MainPage()
        {
            InitializeComponent();
            contentPage = this;
            absolute = Absolute;
            // if not load project data file
            absolute.BackgroundColor = Color.Transparent; 
            nowLinkImage = new MovableImage(standardImageName);
            backgroundimage.Source = ImageSource.FromFile(nowLinkImage.imageUrl);
            backgroundImage = backgroundimage;
            this.BackgroundColor = Color.Black;
            AssignGestures();

            mainPage = this;
            // serialize
        }
        public MainPage(string projectUrl)
        {
            
            InitializeComponent();
            contentPage = this;
            absolute = Absolute;
            // if not load project data file
            absolute.BackgroundColor = Color.Transparent;
            nowLinkImage = Actions.ProjectFrom(projectUrl);
            /*
            nowLinkImage = new MovableImage(standardImageName);
            backgroundimage.Source = ImageSource.FromFile(nowLinkImage.imageUrl);
            */

            backgroundImage = backgroundimage;
            this.BackgroundColor = Color.Black;
            AssignGestures();

            mainPage = this;

            Display(nowLinkImage);

        }
        public static void Display(MovableImage movableImage)
        {
            absolute.Children.Clear();
            nowLinkImage = movableImage;
            // try catch since branch.jpg is not an uri
            try
            {
                backgroundImage.Source = ImageSource.FromUri(new Uri(movableImage.imageUrl));

                //backgroundImage.Source = 
            }
            catch(Exception ex)
            {
                backgroundImage.Source = ImageSource.FromFile(movableImage.imageUrl);
            }
            if(movableImage.children.Count > 0)
                movableImage.children.ForEach((MovableImage obj) =>
            {
                Paint(obj);
            });
        }
        private static void Paint(MovableImage child)
        {
            absolute.Children.Add(child, child.rectangle);
        }
        public static void Create(Rectangle rectangle)
        {
            /*
            MovableImage movableImage = new MovableImage(absolute, nowLinkImage, rectangle);
            MovableImage childImage = new MovableImage(absolute, movableImage, new Rectangle(new Point(200, 300), new Size(120, 120)));
            movableImage.children.Add(childImage);
            MovableImage otherChildImage = new MovableImage(absolute, movableImage, new Rectangle(new Point(80, 100), new Size(70, 60)));
            movableImage.children.Add(otherChildImage);
            */
            MovableImage child = new MovableImage(absolute, nowLinkImage, rectangle);
            nowLinkImage.children.Add(child);
            Paint(child);

        }
        /* refractored - added to icons to AppBar instead
        ShowState state = ShowState.IsHidden;
        void Button_Clicked(object sender, EventArgs e)
        {
            if (state.)
                isVisible = false;
            else
                isVisible = true;
            ToggleVisibilityOfMovableImages(isVisible);
        }
        */

        /*
        public static void ToggleVisibilityOfMovableImages(ShowState state)
        {

            var list = absolute.Children.ToList().Select((arg) => (MovableImage)arg);
            list.ToList().ForEach((MovableImage obj) => {
                obj.isVisible(state);
            });
            // and also set visibility of those movableimages that arent in absolute
        }
        */
        public static void ToggleVisibilityOfMovableImages(ShowState showState)
        {
            Adjust(nowLinkImage.GetProject(), showState);
        }
        private static void Adjust(MovableImage movableImage, ShowState showState)
        {
            movableImage.isVisible(showState);
            foreach(MovableImage child in movableImage.children)
            {
                Adjust(child, showState);
            }
        }
        public static async void Remove(MovableImage movableImage)
        {
            string url = movableImage.imageUrl;
            nowLinkImage.imageUrl = standardImageName;
            Display(nowLinkImage);
            Azure azure = new Azure();
            await azure.DeleteFileFromStorage(url);
        }
        public static MovableImage actionOrigin = null;
        public void AssignGestures()
        {
            Absolute.Down += Absolute_Down;
            Absolute.Tapped += Absolute_Tapped;
            Absolute.DoubleTapped += Absolute_DoubleTapped;
            Absolute.LongPressed += Absolute_LongPressed;
            Absolute.Panned += Absolute_Panned;
            Absolute.Swiped += Absolute_Swiped;
        }
        // used to give panned a start x y position
        private DownUpEventArgs down;
        void Absolute_Down(object sender, DownUpEventArgs e)
        {
            down = e;

        }


        async void Absolute_Tapped(object sender, TapEventArgs e)
        {
            // await App.Current.MainPage.DisplayAlert("info", "url is: " + projectUrl, "ok");

            if (nowLinkImage.owner == null)
            {
                if(nowLinkImage.imageUrl == standardImageName)
                {
                    MediaFile mediaFile = await Actions.TakePhoto();
                    if(mediaFile != null)
                    {
                        Azure azure = new Azure();
                        string url = await azure.UploadFileToStorage(mediaFile);
                        nowLinkImage.imageUrl = url;
                        Display(nowLinkImage);
                    }
                }

            }

        }

        async void Absolute_DoubleTapped(object sender, TapEventArgs e)
        {
            if(nowLinkImage.imageUrl != standardImageName && nowLinkImage.owner == null)
            {

            }
        }


        async void Absolute_LongPressed(object sender, LongPressEventArgs e)
        {
            // await App.Current.MainPage.DisplayAlert("LongPressed", "LongPressed aboslute", "ok");
            /*
            if(nowLinkImage.owner == null)
            {
                if(nowLinkImage.imageUrl == startingImageName)
                {
                    MediaFile mediaFile = await Actions.PickPhoto();
                    if (mediaFile != null)
                    {
                        Azure azure = new Azure();
                        string url = await azure.UploadFileToStorage(mediaFile.GetStream());
                        nowLinkImage.imageUrl = url;
                        Display(nowLinkImage);
                    }
                }
            }
            */
            if(nowLinkImage.imageUrl != null)
            {
                if(nowLinkImage.imageUrl != standardImageName)
                {
                    string choice = await this.DisplayActionSheet("Image", "cancel", "Remove", "Edit");
                    if (choice != null)
                    {
                        if (choice == "Remove")
                        {
                            Remove(nowLinkImage);
                        }
                        else if (choice == "Edit")
                        {
                            // share as image, remember which image - returned image replace
                        }
                    }
                }

            }

        }

        void Absolute_Panned(object sender, PanEventArgs e)
        {
            if (actionOrigin == null)
            {
                // action occured on absolute
                Rectangle rectangle = new Rectangle(new Point(down.Touches[0].X, down.Touches[0].Y), new Size(e.TotalDistance.X, e.TotalDistance.Y));

                if(rectangle.Width > 30 && rectangle.Height > 30)
                {

                    Create(rectangle);
                }

            }
            else
            {
                // action occured on movable image and is handled there
            }
            actionOrigin = null;
        }
        void Absolute_Swiped(object sender, SwipeEventArgs e)
        {
            if(actionOrigin == null)
            {
                // action occured on absolute
                if(nowLinkImage.owner != null)
                {
                    Display(nowLinkImage.owner);
                }
            }
            else
            {
                // action occured on movable image and is handled there
            }
        }



    }
}
