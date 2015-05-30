using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverApp.Models
{
    public class Order : ObservableObject
    {
        public Order()
        {
            this.Meals = new List<Menu>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
        public List<Menu> Meals { get; set; }

        public bool IsDelivered { get; set; }
    }
}
