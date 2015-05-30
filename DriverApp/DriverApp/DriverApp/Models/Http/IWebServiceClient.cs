using DriverApp.Models;
using DriverApp.Models.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverApp
{
    public interface IWebServiceClient
    {
        void PostObject<T>(string requestUrl, T obj, Action<ResponseBase> onCompleted = null);

        void RegisterUser(Driver user, Action<AuthResponse> action = null);
        void Login(object obj, Action<AuthResponse> action = null);

        void GetOrders(int driverId, int lastOrderId, Action<OrdersResponse> onCompleted = null);
        void GetInventory(int driverId, Action<InventoryResponse> onCompleted = null);

        void CompleteOrder(int orderId, Action<ResponseBase> onCompleted = null);
    }

   
}
