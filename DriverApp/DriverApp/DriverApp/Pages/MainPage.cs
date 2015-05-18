using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DriverApp.Pages
{
    public class MainPage : MasterDetailPage
    {        
        public MainPage()
        {
            this.Title = "Meals";
            var sideList = new SideBarList();
            sideList.SideItemSelected += (s, e) =>
            {
                if (e == "Inbox" && !(this.Detail is InboxPage))
                {
                    this.Detail = new NavigationPage(new InboxPage());
                    this.Title = "Inbox";
                }
                if (e == "Today Meals" && !(this.Detail is MealsPage))
                {
                    this.Detail = new NavigationPage(new MealsPage());
                    this.Title = "Today Meals";
                }
                if (e == "My Account" && !(this.Detail is MyAccountPage))
                {
                    this.Detail = new NavigationPage(new MyAccountPage());
                    this.Title = "My Account";
                }
                if (e == "Logout")
                    App.Current.MainPage = new LoginPage();

                this.IsPresented = false;
            };
            sideList.Icon = "settings.png";

            this.Master = sideList;
            this.Detail = new NavigationPage(new InboxPage());
        }
    }
}
