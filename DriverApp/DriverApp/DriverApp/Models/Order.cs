using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverApp.Models
{
    public class Order
    {
        public Order()
        {
            this.Meals = new List<Menu>();
        }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public List<Menu> Meals { get; set; }

        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus
    {
        Open,
        Closed
    }
}
