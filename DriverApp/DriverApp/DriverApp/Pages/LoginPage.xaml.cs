using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DriverApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            this.BindingContext = App.Locator.MainViewModel.Driver;
        }

        private void Submit_Clicked(object sender, EventArgs e)
        {
            errorMsg.IsVisible = false;
            var driver = App.Locator.MainViewModel.Driver;

            if (string.IsNullOrEmpty(driver.UserName) || string.IsNullOrEmpty(driver.Password))
            {
                errorMsg.IsVisible = true;
                return;
            }

            App.Locator.MainViewModel.WebService.Login(new { username = driver.UserName, password = driver.Password }, (res) =>
            {
                if (res.Success)
                {
                    driver.Id = res.DriverId;
                    App.Current.MainPage = new MainPage();
                }
                else
                {
                    errorMsg.IsVisible = true;
                    lbl.Text = res.Error;
                }
            }); 
        }
    }
}
