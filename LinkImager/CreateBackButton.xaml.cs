using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace LinkImager
{
    public partial class CreateBackButton : ContentPage
    {
        public CreateBackButton()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            Navigation.PushAsync(new MainPage(), false);
        }
    }
}
