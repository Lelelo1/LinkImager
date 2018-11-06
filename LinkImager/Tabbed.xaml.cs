using System;
using System.Collections.Generic;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms;

namespace LinkImager
{
    public partial class Tabbed : Xamarin.Forms.TabbedPage
    {
        public Tabbed()
        {
            InitializeComponent();
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetBarItemColor(Color.Black);
            this.BarBackgroundColor = Color.Black;
        }
    }
}
