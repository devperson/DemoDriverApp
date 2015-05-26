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

        void RegisterUser(Driver user, Action<AuthResponse> action);
        void Login(object obj, Action<AuthResponse> action);

        void GetOrders(int driverId, int lastOrderId, Action<OrdersResponse> onCompleted);
        void GetInventory(int driverId, Action<InventoryResponse> onCompleted);

        void CompleteOrder(int orderId, Action<ResponseBase> onCompleted);
    }

   
}
