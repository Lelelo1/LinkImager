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
            AppBar.SetHasBackButton(this, true);
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            MovableImage project = MainPage.nowLinkImage.GetProject();
            Actions.Share(project);
        }
    }
}
