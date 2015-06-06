using System;
using Xamarin.Forms;

namespace DriverApp
{
	public class BasePage : ContentPage
	{
		public BasePage ()
		{
            App.Locator.MainViewModel.ShowAlert = this.DisplayAlert;
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
	}
}

