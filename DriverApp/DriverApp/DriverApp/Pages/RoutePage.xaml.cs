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

            Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetupMap();
                });
                return false;
            });

            var btnComplete = new ToolbarItem() { Text = "Complete" };
            btnComplete.Clicked += btnComplete_Clicked;
            this.ToolbarItems.Add(btnComplete);     
        }

        private async void SetupMap()
        {
            var driver = App.Locator.MainViewModel.Driver;
            var driverPosition = new Position(driver.CurrentLatitude, driver.CurrentLongitude);
            var customerAddress = App.Locator.MainViewModel.ViewOrder.User.UserAddress;

            map.MoveToRegion(MapSpan.FromCenterAndRadius(driverPosition, Xamarin.Forms.Maps.Distance.FromMiles(0.5)));
            map.Pins.Clear();
            map.Pins.Add(new Pin { Label = "My Location", Address = driver.CurrentAddress, Position = driverPosition });
            map.Pins.Add(new Pin { Label = customerAddress.AddressText, Address = customerAddress.AddressText, Position = customerAddress.Position });

            await map.CreateRoute(driverPosition, customerAddress.Position);
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
