using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DriverApp.Pages
{
    public partial class MealsPage : BasePage
    {
        public MealsPage()
        {
            InitializeComponent();

            this.BindingContext = App.Locator.MainViewModel;
        }
    }
}
