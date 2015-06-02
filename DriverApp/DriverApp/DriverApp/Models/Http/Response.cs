using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace DriverApp.Models.Http
{
    public class ResponseBase
    {        
        public bool Success { get; set; }
        public string Error { get; set; }
    }

    public class ErrorResponseModel
    {
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        //public string ExceptionType { get; set; }


        public bool HasErrorMessage
        {
            get
            {
                return !string.IsNullOrEmpty(this.Message) || !string.IsNullOrEmpty(this.ExceptionMessage);
            }
        }

        public string ErrorMessage
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Message))
                    return this.Message;
                else
                    return this.ExceptionMessage;
            }
        }
    }

    public class MenuResponse : ResponseBase
    {
        public List<Menu> Result { get; set; }
    }

    public class AuthResponse : ResponseBase
    {
        public int DriverId { get; set; }
    }

    public class OrderResponse : ResponseBase
    {
        public int OrderId { get; set; }
        public int DriverId { get; set; }
        public Position DriverPosition { get; set; }
    }

    public class OrdersResponse : ResponseBase
    {
        public List<Order> Orders { get; set; }
    }

    public class CustomersResponse : ResponseBase
    {
        public List<int> Ids { get; set; }
    }

    public class InventoryResponse : ResponseBase
    {
        public List<Menu> Inventories { get; set; }
    }
}
