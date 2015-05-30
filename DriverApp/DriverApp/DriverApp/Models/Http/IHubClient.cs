using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace DriverApp
{
    public interface IHubClient
    {        
        void Initialize(string host, string clientName);
        event EventHandler<OrderEventArgs> OnNewOrder;

        void NotifyNewDriverLocation(MsgData data);
        void NotifyOrderCompleted(MsgData data);
    }

    public class OrderEventArgs : EventArgs
    {
        public int OrderId { get; set; }
    }

    public class MsgData
    {
        public MsgData()
        {
            this.To = new List<string>();
        }
        public List<string> To { get; set; }
        public object Data { get; set; }
    }
}
