using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms;
using DriverApp.Controls;
using Xamarin.Forms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps;

[assembly: ExportRenderer(typeof(DriverApp.Controls.ExtendedMap), typeof(DriverApp.Droid.Renderers.ExtendedMapRenderer))]
namespace DriverApp.Droid.Renderers
{
    public class ExtendedMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        private GoogleMap _map;


        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            if (_map != null)
                _map.MapClick -= googleMap_MapClick;

            base.OnElementChanged(e);

            var formsMap = (ExtendedMap)Element;
            var androidMapView = (MapView)Control;

            if (androidMapView != null)
                androidMapView.GetMapAsync(this);

            if (formsMap != null)
            {
                ((System.Collections.ObjectModel.ObservableCollection<Xamarin.Forms.Maps.Pin>)formsMap.Pins).CollectionChanged += OnPinsCollectionChanged;

                ((ObservableRangeCollection<Position>)formsMap.polilenes).CollectionChanged += OnPolCollectionChanged;

                androidMapView.Map.MarkerDragEnd += Map_MarkerDragEnd;
                androidMapView.Map.MapLongClick += (s, a) =>
                {
                    formsMap.Pins.Add(new Pin
                    {
                        Label = "Meu Endereço",
                        Position = new Position(a.Point.Latitude, a.Point.Longitude)
                    }
                    );
                };
            }
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            if (_map != null)
                _map.MapClick += googleMap_MapClick;
        }

        private void googleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            ((ExtendedMap)Element).OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
        }

        private void Map_MarkerDragEnd(object sender, Android.Gms.Maps.GoogleMap.MarkerDragEndEventArgs e)
        {

        }

        private void OnPolCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            createLines();
        }

        private void OnPinsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            updatePins();
        }

        private void createLines()
        {
            try
            {
                var formsMap = (ExtendedMap)Element;
                var androidMapView = (Android.Gms.Maps.MapView)Control;
                //androidMapView.Map.Clear();
                PolylineOptions line = new PolylineOptions();
                line.InvokeColor(global::Android.Graphics.Color.Blue);
                foreach (var item in formsMap.polilenes)
                {

                    LatLng pos = new LatLng(item.Latitude, item.Longitude);
                    line.Add(pos);
                }
                androidMapView.Map.AddPolyline(line);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private void updatePins()
        {
            var formsMap = (ExtendedMap)Element;
            var androidMapView = (Android.Gms.Maps.MapView)Control;

            androidMapView.Map.Clear();

            androidMapView.Map.MyLocationEnabled = formsMap.IsShowingUser;
            androidMapView.Map.MarkerClick += HandleMarkerClick;


            var items = formsMap.Pins;

            foreach (var item in items)
            {
                var markerWithIcon = new MarkerOptions();
                markerWithIcon.SetPosition(new LatLng(item.Position.Latitude, item.Position.Longitude));
                markerWithIcon.SetTitle(string.IsNullOrWhiteSpace(item.Label) ? "-" : item.Label);

                try
                {
                    markerWithIcon.InvokeIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
                }
                catch (Exception)
                {
                    markerWithIcon.InvokeIcon(BitmapDescriptorFactory.DefaultMarker());
                }

                //markerWithIcon.Draggable(true);

                androidMapView.Map.AddMarker(markerWithIcon);
            }
        }

        private void HandleMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            
        }

    }
}