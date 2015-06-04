using DriverApp.Pages;
using DriverApp.ViewModels;
using Geolocator.Plugin;
using Geolocator.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DriverApp
{
    public class App : Application
    {
        public static bool IsDevice { get; set; }
        public static ViewModelLocator Locator { get; set; }
        public static IGeolocator GeoLocator;

        public App()
        {
            Locator = new ViewModelLocator();
            // The root page of your application
            MainPage = new NavigationPage(new StartPage());
        }

        protected override void OnStart()
        {          
            // Handle when your app starts
        }

        protected override void OnResume()
        {
            if (Device.OS == TargetPlatform.Android && GeoLocator != null)
            {
                GeoLocator.StartListening(0, 0);
            }
        }


        protected override void OnSleep()
        {
            if (Device.OS == TargetPlatform.Android && GeoLocator!=null)
            {
                GeoLocator.StopListening();
            }
        }
    }
}
