using DriverApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DriverApp.Pages
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();

            this.BindingContext = App.Locator.MainViewModel;

            App.Locator.MainViewModel.ShowAlert = this.DisplayAlert;
        }

        private void Submit_Clicked(object sender, EventArgs e)
        {
            errorMsg.IsVisible = false;
            var driver = App.Locator.MainViewModel.Driver;
            if (string.IsNullOrEmpty(driver.UserName) || string.IsNullOrEmpty(driver.Password))
            {
                errorMsg.IsVisible = true;
                lblErr.Text = "Please Fill all fields.";
                return;
            }

            App.Locator.MainViewModel.LoadingCount++;
            App.Locator.MainViewModel.WebService.RegisterUser(driver, (res) =>
            {
                App.Locator.MainViewModel.LoadingCount--;
                if (res.Success)
                {
                    App.Locator.MainViewModel.Driver.Id = res.DriverId;
                    App.Current.MainPage = new MainPage();
                }
                else
                {
                    errorMsg.IsVisible = true;
                    lblErr.Text = res.Error;
                }
            });
        }
    }
}
