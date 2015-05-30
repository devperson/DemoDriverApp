using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DriverApp.Models.Http;
using Microsoft.AspNet.SignalR.Client;
using Xamarin.Forms.Maps;

[assembly: Xamarin.Forms.Dependency(typeof(DriverApp.Hub.HubClient))]
namespace DriverApp.Hub
{
    public class HubClient : IHubClient
    {
        HubConnection connection;
        IHubProxy serverHub;
        public event EventHandler<OrderEventArgs> OnNewOrder;

        public async void Initialize(string host, string clientName)
        {
            connection = new HubConnection(host, string.Format("name={0}", clientName));
            serverHub = connection.CreateHubProxy("HubServer");
            serverHub.On<OrderEventArgs>("NewOrderPosted", (e) =>
            {
                if (this.OnNewOrder != null)
                    this.OnNewOrder(this, e);
            });
            await connection.Start();
        }

        public async void NotifyNewDriverLocation(MsgData data)
        {
            await serverHub.Invoke("Notify_NewDriverLocation", new object[] { data });
        }

        public async void NotifyOrderCompleted(MsgData data)
        {
            await serverHub.Invoke("Notify_OrderCompleted", new object[] { data });
        }
    }

    
}