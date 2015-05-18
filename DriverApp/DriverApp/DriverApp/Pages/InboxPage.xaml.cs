using DriverApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DriverApp.Pages
{
    public partial class InboxPage : ContentPage
    {
        public InboxPage()
        {
            InitializeComponent();

            this.BindingContext = App.Locator.MainViewModel;
            listView.ItemTapped += (s, e) =>
            {
                var order = listView.SelectedItem as Order;
                App.Locator.MainViewModel.ViewOrder = order;

                this.Navigation.PushAsync(new OrderPage());
            };         
        }
    }
}
