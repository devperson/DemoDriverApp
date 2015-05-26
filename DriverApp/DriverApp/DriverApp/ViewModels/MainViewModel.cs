using CustomerApp.Models;
using DriverApp.Controls.Models;
using DriverApp.Models;
using Geolocator.Plugin;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace DriverApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {

        public string ApiUrl = "";
        public ObservableCollection<Order> Orders { get; set; }
        public Order ViewOrder { get; set; }
        public ObservableCollection<Menu> Menu { get; set; }
        public Driver Driver { get; set; }

        private IWebServiceClient _service;
        public IWebServiceClient WebService
        {
            get
            {
                if (_service == null)
                {
                    _service = DependencyService.Get<IWebServiceClient>();
                }
                return _service;
            }
        }
        
        public MainViewModel()
        {
            if (Debugger.IsAttached)
                this.ApiUrl = "http://localhost:1732/";
            else
                this.ApiUrl = "http://demowebserver.apphb.com/";

            this.Orders = new ObservableCollection<Order>();
            this.Menu = new ObservableCollection<Menu>();
            this.Driver = new Driver();


            Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
            {
                this.GetData();               
                return false;
            });
            //Menu menu1 = new Menu();
            //menu1.Name = "CHICKEN AND CHEESE ENCHILADAS";
            //menu1.Price = 8;
            //menu1.Description = "homemade chicken & cheese enchiladas with salsa roja, spanish rice, pinto beans & corn (~750 cal)";
            //menu1.Image = "img1.jpg";//"http://localhost:1732/Images/Products/img1.jpg";            
            //this.Menu.Add(menu1);

            //var menu2 = new Menu();
            //menu2.Name = "THAI STYLE PORK RICE BOWL";
            //menu2.Price = 10;
            //menu2.Description = "spicy minced pork with chilies, mint, lime, bell peppers, steamed jasmine rice & sauteed green beans (~425 cal)";
            //menu2.Image = "img2.jpg";//"http://localhost:1732/Images/Products/img2.jpg";
            
            //this.Menu.Add(menu2);

            //var menu3 = new Menu();
            //menu3.Name = "FOUR CHEESE RAVIOLI WITH WILD MUSHROOM SAUCE";
            //menu3.Price = 7;
            //menu3.Description = "four cheese ravioli with wild mushroom sauce, asparagus, peas, zucchini, sun dried tomatoes & fontina cheese (~700 cal)";
            //menu3.Image = "img3.jpg";//"http://localhost:1732/Images/Products/img3.jpg";            
            //this.Menu.Add(menu3);

            //Order or1 = new Order();
            //or1.Date = DateTime.Now;            
            //var meal1 = this.Menu[0].DeepClone();
            //meal1.Quantity = 2;
            //or1.Meals.Add(meal1);
            //or1.User = new User();
            //or1.User.FirstName = "Gary";
            //or1.User.LastName = "Ostman";
            //or1.User.Phone = "1234567";
            //or1.User.Gender = "Male";
            //or1.User.UserAddress = new Controls.Models.Address();
            //or1.User.UserAddress.AddressText = "1680 Union St San Francisco, CA 94123, USA";
            //or1.User.UserAddress.Position = new Position(41.259655, 69.184834);//new Position(37.798231, -122.426536);
            //this.Orders.Add(or1);

            //Order or2 = new Order();
            //or2.IsDelivered = true;
            //or2.Date = DateTime.Now;            
            //var meal2 = this.Menu[0].DeepClone();
            //meal2.Quantity = 3;
            //or2.Meals.Add(meal2);
            //or2.User = new User();
            //or2.User.FirstName = "Lucy";
            //or2.User.LastName = "Ostman";
            //or2.User.Phone = "1234567";
            //or2.User.Gender = "Female";
            //or2.User.UserAddress = new Controls.Models.Address();
            //or2.User.UserAddress.AddressText = "1816 Lacassie Ave Walnut Creek, CA 94596 USA";
            //or2.User.UserAddress.Position = new Position(37.903781, -122.067485);
            //this.Orders.Add(or2);

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(() => { 
                    var locator = CrossGeolocator.Current;
                    locator.PositionChanged += locator_PositionChanged;
                    locator.StartListening(15000, 10);
                });
                return false;
            });
        }

        private void GetData()
        {
            this.WebService.GetInventory(this.Driver.Id, (response) =>
            {
                this.Menu = new ObservableCollection<Menu>(response.Inventories);
                int lastOrderId = this.Orders.Any() ? this.Orders.Last().Id : 0;
                this.WebService.GetOrders(this.Driver.Id, lastOrderId, (res) =>
                {                    
                    this.Orders = new ObservableCollection<Order>(res.Orders);
                });
            });
        }

        private void locator_PositionChanged(object sender, Geolocator.Plugin.Abstractions.PositionEventArgs e)
        {
            var pos = new Position(e.Position.Latitude, e.Position.Longitude);
            Device.BeginInvokeOnMainThread(async () =>
            {
                var geo = new Geocoder();                
                var addresses = await geo.GetAddressesForPositionAsync(pos);
                var addr = addresses.First();

                var userAddress = new Address();
                userAddress.AddressText = addr;
                userAddress.Position = pos;
                this.Driver.Address = userAddress;
            });
        }
    }
}
