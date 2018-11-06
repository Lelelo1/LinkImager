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
        static Xamarin.Forms.ContentPage contentPage;
        static MR.Gestures.AbsoluteLayout absolute;
        public static MovableImage project;
        static MovableImage nowLinkImage;
        public MainPage()
        {
            InitializeComponent();
            contentPage = this;
            absolute = Absolute;
            // if not load project data file
            project = new MovableImage("branch.jpg");
            contentPage.BackgroundImage = project.imageUrl;
            absolute.BackgroundColor = Color.Transparent; 
            nowLinkImage = project;

            AssignGestures();

            // serialize


        }
        public static void Display(MovableImage movableImage)
        {
            absolute.Children.Clear();
            nowLinkImage = movableImage;
            contentPage.BackgroundImage = nowLinkImage.imageUrl;
            if(nowLinkImage.children.Count > 0)
                nowLinkImage.children.ForEach((MovableImage obj) =>
            {
                Paint(obj);
            });
        }
        private static void Paint(MovableImage child)
        {
            absolute.Children.Add(child, child.rectangle);
        }
        public void Create(Rectangle rectangle)
        {

            MovableImage movableImage = new MovableImage(absolute, nowLinkImage, rectangle);
            MovableImage childImage = new MovableImage(absolute, movableImage, new Rectangle(new Point(200, 300), new Size(120, 120)));
            movableImage.children.Add(childImage);
            MovableImage otherChildImage = new MovableImage(absolute, movableImage, new Rectangle(new Point(80, 100), new Size(70, 60)));
            movableImage.children.Add(otherChildImage);

            Paint(movableImage);
        }

        bool isVisible = true;
        void Button_Clicked(object sender, EventArgs e)
        {
            if (isVisible == true)
                isVisible = false;
            else
                isVisible = true;
            ToggleVisibilityOfMovableImages(isVisible);
        }

        private void ToggleVisibilityOfMovableImages(bool show)
        {

            var list = Absolute.Children.ToList().Select((arg) => (MovableImage)arg);
            list.ToList().ForEach((MovableImage obj) => {
                obj.isVisible(show);
            });
            // and also set visibility of those movableimages that arent in absolute
        }

        public static MovableImage actionOrigin = null;
        public void AssignGestures()
        {
            Absolute.LongPressed += Absolute_LongPressed;
        }

        void Absolute_LongPressed(object sender, LongPressEventArgs e)
        {
            App.Current.MainPage.DisplayAlert("LongPressed", "LongPressed aboslute", "ok");
            Create(new Rectangle(new Point(100, 300), new Size(120, 120)));
        }

    }
}
