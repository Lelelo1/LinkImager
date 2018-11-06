using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LinkImager.Items;
using Xamarin.Forms;
using Plugin.ShareFile;
namespace LinkImager
{
    public partial class AppBar : NavigationPage
    {
        public AppBar(Page page) : base(page)
        {
            InitializeComponent();
            this.BarBackgroundColor = Color.Black;
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            /* for testing
            MovableImage project = new MovableImage("branch.jpg");
            MovableImage movableImage = new MovableImage(project, new Xamarin.Forms.Rectangle(new Point(0, 0), new Size(40, 40)));
            project.children.Add(movableImage);
            MovableImage childImage = new MovableImage(movableImage, new Rectangle(new Point(200, 300), new Size(120, 120)));
            movableImage.children.Add(childImage);
            MovableImage otherChildImage = new MovableImage(movableImage, new Rectangle(new Point(80, 100), new Size(70, 60)));
            movableImage.children.Add(otherChildImage);
            */

            MovableImage movableImage = MainPage.project;
            if(movableImage.imageUrl == null)
            {
                App.Current.MainPage.DisplayAlert("Empty", "Build your project firs before saving it or sending it", "ok");
            }
            else
            {
                string tempDir = System.IO.Path.GetTempPath();
                string name = await InputAlertBox.InputBox(this.Navigation);
                if(!string.IsNullOrEmpty(name))
                {
                    name += ".ii";
                    string fullPath = Path.Combine(tempDir, name);
                    Stream stream = File.Open(fullPath, FileMode.Create);
                    BinaryFormatter formatter = new BinaryFormatter();

                    formatter.Serialize(stream, movableImage);
                    stream.Close();

                    bool existed = File.Exists(fullPath);

                    CrossShareFile.Current.ShareLocalFile(fullPath, "Title");
                }


            }
        }
        public void setBackButton()
        {

        }
    }
}
