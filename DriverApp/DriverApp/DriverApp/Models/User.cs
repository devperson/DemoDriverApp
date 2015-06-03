using DriverApp.Controls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }

        public string Prefix
        {
            get
            {
                return this.Gender == "Male" ? "Mr." : "Ms.";
            }
        }
        public string DisplayName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        public Address Address { get; set; }
    }
}
