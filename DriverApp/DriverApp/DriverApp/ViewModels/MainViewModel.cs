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
        bool isLocationListened;
        int _loadCount;
        public int LoadingCount
        {
            get { return _loadCount; }
            set
            {
                _loadCount = value;
                this.RaisePropertyChanged(p => p.IsLoading);
            }
        }
        public bool IsLoading
        {
            get
            {
                return this.LoadingCount > 0;
            }
        }
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

        private IHubClient _ntf;
        public IHubClient Notifier
        {
            get
            {
                if (_ntf == null)
                {
                    _ntf = DependencyService.Get<IHubClient>();                    
                }
                return _ntf;
            }
        }
        
        public MainViewModel()
        {
            if (App.IsDevice)
                this.ApiUrl = "http://demowebserver.apphb.com/";
            else
                this.ApiUrl = "http://xusanpc:1732/";

            this.Orders = new ObservableCollection<Order>();
            this.Menu = new ObservableCollection<Menu>();
            this.Driver = new Driver();            

            //Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            //{
            //    Device.BeginInvokeOnMainThread(() => { 
                   
            //    });
            //    return false;
            //});


        }

        public void ListenForLocationChanges()
        {
            if (!isLocationListened)
            {
                isLocationListened = true;
                var locator = CrossGeolocator.Current;
                locator.PositionChanged += locator_PositionChanged;
                locator.StartListening(5000, 10);
            }
        }

        public void OnDriverLogedIn()
        {
            this.Notifier.Initialize(this.ApiUrl, "Driver" + this.Driver.Id.ToString());
            this.Notifier.OnNewOrder += Notifier_OnNewOrder;
            this.GetData();

            if (!string.IsNullOrEmpty(this.Driver.CurrentAddress))
                this.WebService.UpdateDriverLocation(new { UserId = this.Driver.Id, Position = new Position(this.Driver.CurrentLatitude, this.Driver.CurrentLongitude), Address = this.Driver.CurrentAddress });
        }

        private void GetData()
        {
            this.LoadingCount++;
            this.WebService.GetInventory(this.Driver.Id, (response) =>
            {
                if (response.Success)
                {
                    this.Menu = new ObservableCollection<Menu>(response.Inventories);
                    this.RaisePropertyChanged(p => p.Menu);
                }
                else
                {                    
                    this.ShowError("Error on getting inventory data. " + response.Error);
                }
                
                int lastOrderId = this.Orders.Any() ? this.Orders.Last().Id : 0;
                this.WebService.GetOrders(this.Driver.Id, lastOrderId, (res) =>
                {
                    this.LoadingCount--;
                    if (res.Success)
                    {
                        this.Orders = new ObservableCollection<Order>(res.Orders);
                        this.RaisePropertyChanged(p => p.Orders);
                    }
                    else
                    {
                        this.ShowError("Error on getting orders. " + res.Error);
                    }
                });
            });
        }

        public void Notifier_OnNewOrder(object sender, OrderEventArgs e)
        {
            int lastOrderId = this.Orders.Any() ? this.Orders.Last().Id : 0;
            this.LoadingCount++;
            this.WebService.GetOrders(this.Driver.Id, lastOrderId, (res) =>
            {
                this.LoadingCount--;
                if (res.Orders.Count > 1)
                {
                    var orders = new List<Order>(this.Orders);
                    orders.AddRange(res.Orders);
                    this.Orders = new ObservableCollection<Order>(orders.OrderByDescending(o => o.Date));
                    this.RaisePropertyChanged(p => p.Orders);
                }
                else if (res.Orders.Count == 1)
                {
                    this.Orders.Insert(0, res.Orders.FirstOrDefault());                    
                }
            });
        }

        private async void locator_PositionChanged(object sender, Geolocator.Plugin.Abstractions.PositionEventArgs e)
        {
            this.ShowError(string.Format("Got location: {0},{1}", e.Position.Latitude, e.Position.Longitude));

            //var pos = new Position(e.Position.Latitude, e.Position.Longitude);
            //var geo = new Geocoder();
            //var addresses = await geo.GetAddressesForPositionAsync(pos);
            //var addr = addresses.First();

            //var isChanged = (this.Driver.CurrentAddress != addr || this.Driver.CurrentLatitude != pos.Latitude || this.Driver.CurrentLongitude != pos.Longitude);
            //this.Driver.CurrentAddress = addr;
            //this.Driver.CurrentLatitude = pos.Latitude;
            //this.Driver.CurrentLongitude = pos.Longitude;

            //if (isChanged)
            //{
            //    if (this.Driver.Id > 0)
            //        this.WebService.UpdateDriverLocation(new { UserId = this.Driver.Id, Position = new Position(this.Driver.CurrentLatitude, this.Driver.CurrentLongitude), Address = this.Driver.CurrentAddress });

            //    if (this.Orders.Where(o => !o.IsDelivered).Any()) //Has any customers.
            //    {
            //        var msgData = new MsgData();
            //        msgData.Data = new { DriverId = this.Driver.Id, Position = pos };
            //        msgData.To = this.Orders.Where(o => !o.IsDelivered).Select(o => "Customer" + o.User.Id).ToList();
            //        this.Notifier.NotifyNewDriverLocation(msgData);
            //    }
            //}
        }

        public void CompleteOrder()
        {
            this.LoadingCount++;
            this.WebService.CompleteOrder(this.ViewOrder.Id, (res) =>
            {
                this.LoadingCount--;
                if (res.Success)
                {
                    this.ViewOrder.IsDelivered = true;
                    this.ViewOrder.RaisePropertyChanged("IsDelivered");

                    var msgData = new MsgData();
                    msgData.Data = new { OrderId = this.ViewOrder.Id };
                    msgData.To.Add("Customer" + this.ViewOrder.User.Id);
                    this.Notifier.NotifyOrderCompleted(msgData);
                }
                else
                    this.ShowError("Error on completing order. Please try later again.");
            });
        }

        public Func<string, string, string, Task> ShowAlert;
        public async void ShowError(string errorMessage)
        {
            await ShowAlert("Error", errorMessage, "Close");
        }
    }
}
