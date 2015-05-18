using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms;
using MapKit;
using DriverApp.Controls;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.iOS;
using Xamarin;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CoreLocation;

[assembly: ExportRenderer(typeof(DriverApp.Controls.ExtendedMap), typeof(DriverApp.iOS.Renderers.ExtendedMapRenderer))]
namespace DriverApp.iOS.Renderers
{
    public class ExtendedMapRenderer : MapRenderer
    {
        private UITapGestureRecognizer _tapRecogniser;
        public ExtendedMapRenderer()
        {
            _tapRecogniser = new UITapGestureRecognizer(OnTap)
            {
                NumberOfTapsRequired = 1,
                NumberOfTouchesRequired = 1
            };
        }

        //public static void Init()
        //{
        //    FormsMaps.Init();
        //}

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {            
            var formsMap = Element as ExtendedMap;

            if (formsMap == null)
                return;

            if (Control != null)
                Control.RemoveGestureRecognizer(_tapRecogniser);
            base.OnElementChanged(e);
            if (Control != null)
                Control.AddGestureRecognizer(_tapRecogniser);

            var mapView = Control as MKMapView;
            var mapDelegate = new MapDelegate();
            mapView.Delegate = mapDelegate;

            ((ObservableCollection<Pin>)formsMap.Pins).CollectionChanged += OnPinsCollectionChanged;
            formsMap.polilenes.CollectionChanged += OnPolCollectionChanged;
        }

        private void OnTap(UITapGestureRecognizer recognizer)
        {
            var cgPoint = recognizer.LocationInView(Control);
            var location = ((MKMapView)Control).ConvertPoint(cgPoint, Control);
            ((ExtendedMap)Element).OnTap(new Position(location.Latitude, location.Longitude));
        }

        private void OnPolCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CreateLines();
        }

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePins();
        }

        private void CreateLines()
        {
            try
            {
                var mapView = Control as MKMapView;
                var formsMap = Element as ExtendedMap;

                var lineCords = new List<CLLocationCoordinate2D>();

                if (formsMap.polilenes.Count <= 0)
                {
                    return;
                }

                foreach (var item in formsMap.polilenes)
                {

                    lineCords.Add(new CLLocationCoordinate2D(item.Latitude, item.Longitude));
                }

                var line = MKPolyline.FromCoordinates(lineCords.ToArray());

                mapView.AddOverlay(line);

                mapView.SetVisibleMapRect(line.BoundingMapRect, true);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private void UpdatePins()
        {
            var mkMapView = Control as MKMapView;
            var formsMap = Element as ExtendedMap;

            if (mkMapView.Annotations.Length > 0)
            {
                mkMapView.RemoveAnnotations(mkMapView.Annotations);
            }

            var items = formsMap.Pins;

            foreach (var item in items)
            {
                var coord = new CLLocationCoordinate2D(item.Position.Latitude, item.Position.Longitude);

                
                var point = new MKPointAnnotation { Title = item.Label };
                
                point.SetCoordinate(coord);

                mkMapView.AddAnnotation(point);
            }
        }
    }

    class MapDelegate : MKMapViewDelegate
    {
        //Override OverLayRenderer to draw Polyline returned from directions
        public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (!(overlay is MKPolyline))
            {
                return null;
            }

            var route = (MKPolyline)overlay;
            var renderer = new MKPolylineRenderer(route) { StrokeColor = UIColor.Blue };

            return renderer;
        }

        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            string annotationIdentifier = @"annotationIdentifier";
            var pinView = mapView.DequeueReusableAnnotation(annotationIdentifier);
            if(pinView == null)
            {
                pinView = new MKAnnotationView(annotation, annotationIdentifier);

                var pointAnn = annotation as MKPointAnnotation;
                if (pointAnn != null)
                    pinView.Image = UIImage.FromBundle("pin.png");
            }
            else
            {
                pinView.Annotation = annotation;
            }

            return pinView;
        }
    }
}