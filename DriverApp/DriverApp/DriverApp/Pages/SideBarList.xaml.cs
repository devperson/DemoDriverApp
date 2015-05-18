using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DriverApp.Pages
{
    public partial class SideBarList : ContentPage
    {
        public event EventHandler<string> SideItemSelected;
        public SideBarList()
        {
            InitializeComponent();

            listView.ItemsSource = new List<string> { "Inbox", "Today Meals", "My Account", "Logout" };
            listView.ItemSelected += (s, e) =>
            {
                if (SideItemSelected != null)
                    SideItemSelected(this, listView.SelectedItem.ToString());
            };
        }
    }
}
