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
            
        }

        private async void SetupMap()
        {
            var driverAddress = App.Locator.MainViewModel.Driver.Address; //await App.Locator.MainViewModel.GetPosition();
            var customerAddress = App.Locator.MainViewModel.ViewOrder.User.UserAddress;

            map.MoveToRegion(MapSpan.FromCenterAndRadius(driverAddress.Position, Xamarin.Forms.Maps.Distance.FromMiles(0.5)));
            map.Pins.Clear();
            map.Pins.Add(new Pin { Label = "My Location", Address = driverAddress.AddressText, Position = driverAddress.Position });            
            map.Pins.Add(new Pin { Label = customerAddress.AddressText, Address = customerAddress.AddressText, Position = customerAddress.Position });

            await map.CreateRoute(driverAddress.Position, customerAddress.Position);
        }

        private void Call_Clicked(object sender, EventArgs e)
        {
           
        }
    }
}
