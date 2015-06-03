using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace DriverApp.Pages
{
    public partial class RoutePage : ContentPage
    {
        public RoutePage()
        {
           
            InitializeComponent();           
            this.BindingContext = App.Locator.MainViewModel.ViewOrder;

            //Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        SetupMap();
            //    });
            //    return false;
            //});

            var btnComplete = new ToolbarItem() { Text = "Complete" };
            btnComplete.Clicked += btnComplete_Clicked;
            this.ToolbarItems.Add(btnComplete);

            App.Locator.MainViewModel.ShowAlert = this.DisplayAlert;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.SetupMap();
        }
        private async void SetupMap()
        {
            var driver = App.Locator.MainViewModel.Driver;
            var driverPosition = new Position(driver.CurrentLatitude, driver.CurrentLongitude);
            var customerAddress = App.Locator.MainViewModel.ViewOrder.User.Address;
            var custPosition = new Position(customerAddress.Lat, customerAddress.Lon);
            map.Pins.Clear();

            if (driverPosition.Latitude != 0)
            {
                map.MoveToRegion(MapSpan.FromCenterAndRadius(driverPosition, Xamarin.Forms.Maps.Distance.FromMiles(0.5)));
                map.Pins.Add(new Pin { Label = "My Location", Address = driver.CurrentAddress, Position = driverPosition });
            }
            map.Pins.Add(new Pin { Label = customerAddress.AddressText, Address = customerAddress.AddressText, Position = custPosition });

            var result = await map.CreateRoute(driverPosition, custPosition);
            if (!result)
                this.DisplayAlert("Error", "Couldn't get map route for Customer", "Close");
        }

        private void btnComplete_Clicked(object sender, EventArgs e)
        {
            App.Locator.MainViewModel.CompleteOrder();            
            this.Navigation.PopToRootAsync(true);
        }

       

        private void Call_Clicked(object sender, EventArgs e)
        {
           
        }
    }
}
