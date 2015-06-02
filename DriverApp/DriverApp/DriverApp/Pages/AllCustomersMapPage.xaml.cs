using DriverApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace DriverApp.Pages
{
    public partial class AllCustomersMapPage : ContentPage
    {
        public AllCustomersMapPage()
        {
            InitializeComponent();

            Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetupMap();
                });
                return false;
            });
        }

        private void SetupMap()
        {
            var driver = App.Locator.MainViewModel.Driver;
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(driver.CurrentLatitude, driver.CurrentLongitude), Xamarin.Forms.Maps.Distance.FromMiles(0.5)));

            map.Pins.Clear();
            map.Pins.Add(new Pin { Label = "My Location", Address = driver.CurrentAddress, Position = new Position(driver.CurrentLatitude, driver.CurrentLongitude) });
            foreach (var order in App.Locator.MainViewModel.Orders.Where(o=>!o.IsDelivered).ToList())
            {
                var userAddress = order.User.Address;
                var pin = new Pin { Label = userAddress.AddressText, Address = userAddress.AddressText, Position = new Position(userAddress.Lat, userAddress.Lon) };
                pin.BindingContext = order;
                pin.Clicked += pin_Clicked;
                map.Pins.Add(pin);
            }            
            //await map.CreateRoute(driverAddress.Position, customerAddress.Position);
        }

        private void pin_Clicked(object sender, EventArgs e)
        {
            var pin = sender as Pin;
            App.Locator.MainViewModel.ViewOrder = pin.BindingContext as Order;
            this.Navigation.PushAsync(new OrderPage(true));
        }
    }
}
