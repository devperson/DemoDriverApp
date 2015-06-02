using DriverApp.Controls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverApp.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        //address
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Postal { get; set; }
        public string Country { get; set; }

        //Driver License
        public string LicenseNumber { get; set; }
        public string StateIssued { get; set; }

        public string CurrentAddress { get; set; }
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }        
    }
}
