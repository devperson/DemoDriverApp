using DriverApp.Pages;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DriverApp
{
	public partial class StartPage : BasePage
	{
		public StartPage ()
		{
			InitializeComponent ();
		}

        private void btnLogin_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new LoginPage());
        }

        private void btnReg_Clicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new RegistrationPage());
        }
	}
}

