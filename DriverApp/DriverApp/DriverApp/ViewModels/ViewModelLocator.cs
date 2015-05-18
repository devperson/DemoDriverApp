using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverApp.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            this.MainViewModel = new MainViewModel();
        }
        public MainViewModel MainViewModel { get; set; }
    }
}
