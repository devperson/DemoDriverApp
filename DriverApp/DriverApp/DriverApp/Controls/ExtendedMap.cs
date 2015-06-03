﻿using DriverApp.Controls.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace DriverApp.Controls
{
    public class ExtendedMap : Map, INotifyPropertyChanged
    {

        public event EventHandler<TapEventArgs> Tap;
        /// <summary>
        /// Property for create Polilenes in Map to Route
        /// </summary>
        public ObservableRangeCollection<Position> polilenes { get; set; }

        /// <summary>
        /// Constructor MapExtend
        /// </summary>
        public ExtendedMap(): base()
        {
            polilenes = new ObservableRangeCollection<Position>();
        }

        /// <summary>
        /// Overload Contructor MapExtend
        /// </summary>
        /// <param name="mapSpan"></param>
        public ExtendedMap(MapSpan mapSpan): base(mapSpan)
        {
            polilenes = new ObservableRangeCollection<Position>();
        }


        private string getMapsApiDirectionsUrl(Position From, Position To)
        {
            String waypoints = string.Format("http://216.58.222.10/maps/api/directions/json?origin={0},{1}&destination={2},{3}&sensor=false", From.Latitude.ToString().Replace(',', '.'), From.Longitude.ToString().Replace(',', '.'), To.Latitude.ToString().Replace(',', '.'), To.Longitude.ToString().Replace(',', '.'));

            return waypoints;
        }

        private async Task<DriverApp.Controls.PlacesNs.Rootobject> DownloadPlaces(string url)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync(url);
            return (DriverApp.Controls.PlacesNs.Rootobject)JsonConvert.DeserializeObject(result, typeof(DriverApp.Controls.PlacesNs.Rootobject));
        }

        private async Task<DriverApp.Controls.RoutesNs.Rootobject> DownloadRoutes(string url)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync(url);
            return (DriverApp.Controls.RoutesNs.Rootobject)JsonConvert.DeserializeObject(result, typeof(DriverApp.Controls.RoutesNs.Rootobject));
        }

        private IEnumerable<Position> Decode(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                throw new ArgumentNullException("encodedPoints");

            char[] polylineChars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            while (index < polylineChars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                yield return new Position(Convert.ToDouble(currentLat) / 1E5, Convert.ToDouble(currentLng) / 1E5);
            }
        }


        /// <summary>
        /// Get Nearby Locals of Visible Region Of Map
        /// </summary>
        /// <param name="API_KEY">API KEY FROM GOOGLE PLACES API</param>
        public async Task NearbyLocations(string API_KEY, string types)
        {
            String PLACES_SEARCH_URL = "https://maps.googleapis.com/maps/api/place/search/json?location=" + this.VisibleRegion.Center.Latitude.ToString().Replace(',', '.') + "," + this.VisibleRegion.Center.Longitude.ToString().Replace(',', '.') + "&radius=" + this.VisibleRegion.Radius.Meters.ToString().Replace(',', '.');

            if (!string.IsNullOrEmpty(types))
                PLACES_SEARCH_URL += "&types=" + types;
            PLACES_SEARCH_URL += "&sensor=false&key=" + API_KEY;
            String PLACES_SEARCH_URL_NEXPAGE = "https://maps.googleapis.com/maps/api/place/search/json?pagetoken={0}&key=" + API_KEY;
            List<DriverApp.Controls.PlacesNs.Rootobject> locs = new List<DriverApp.Controls.PlacesNs.Rootobject>();
            var Locals = (await DownloadPlaces(PLACES_SEARCH_URL));
            locs.Add(Locals);


            while (!string.IsNullOrEmpty(Locals.next_page_token))
            {
                Locals = (await DownloadPlaces(string.Format(PLACES_SEARCH_URL_NEXPAGE, Locals.next_page_token)));
                locs.Add(Locals);
            }

            foreach (var lcAtual in locs)
            {

                foreach (var item in lcAtual.results)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.Pins.Add(new Pin()
                        {
                            Label = item.name,
                            Position = new Position(item.geometry.location.lat, item.geometry.location.lng)
                        });
                    });
                }
            }
        }

        /// <summary>
        /// Draw Route From Postion to another position
        /// </summary>
        /// <param name="From">Origin Position</param>
        /// <param name="To">Destinate Position</param>
        /// <returns></returns>
        public async Task<bool> CreateRoute(Position From, Position To)
        {
            var x = getMapsApiDirectionsUrl(From, To);

            var r = (await DownloadRoutes(x));

            var lstPos = new List<Position>();

            if (r.routes.Any())
            {
                var e = r.routes[0].legs[0].steps;

                foreach (var polys in e.Select(item => Decode(item.polyline.points)))
                {
                    lstPos.AddRange(polys);
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    polilenes.AddRange(lstPos);
                });

                return true;
            }
            else
                return false;
        }

        public double GetDistance(Position From, Position To, char unit = 'K')
        {
            double rlat1 = Math.PI * From.Latitude / 180;
            double rlat2 = Math.PI * To.Latitude / 180;
            double theta = From.Longitude - To.Longitude;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        /// <summary>
        /// Create Pin after find Location of Adress
        /// </summary>
        /// <param name="Adresss">Adress</param>
        /// <returns></returns>
        public async Task<Address> SearchAdress(string _address)
        {
            var pos = (await (new Geocoder()).GetPositionsForAddressAsync(_address)).ToList();
            if (!pos.Any())
                return null;

            var po = pos.First();

            this.MoveToRegion(MapSpan.FromCenterAndRadius(po, Xamarin.Forms.Maps.Distance.FromMiles(0.5)));
            this.Pins.Add(new Pin
            {
                Label = _address,
                Address = _address,
                Position = po,
                Type = PinType.SearchResult,
            });

            return new Address { AddressText = _address, Lat = po.Latitude, Lon = po.Longitude };
        }


        public void OnTap(Position coordinate)
        {
            OnTap(new TapEventArgs { Position = coordinate });
        }

        protected virtual void OnTap(TapEventArgs e)
        {
            var handler = Tap;
            if (handler != null) handler(this, e);
        }
    }
}
