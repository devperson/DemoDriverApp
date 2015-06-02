using System;
using Xamarin.Forms;

namespace DriverApp
{
	public class BasePage : ContentPage
	{
		public BasePage ()
		{
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.MainViewModel.ShowAlert = this.DisplayAlert;
        }
	}
}

