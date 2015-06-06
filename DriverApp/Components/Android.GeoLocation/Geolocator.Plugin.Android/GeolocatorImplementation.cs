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
using Geolocator.Plugin.Abstractions;
using System.Threading;
using Android.Gms.Common.Apis;
using System.Threading.Tasks;
using Android.Gms.Location;
using Android.Gms.Common;

namespace Geolocator.Plugin
{
    public class GeolocatorImplementation : Java.Lang.Object, IGeolocator, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener, Android.Gms.Location.ILocationListener 
    {
        private IGoogleApiClient _googleAPI;
        LocationRequest locRequest;
        bool isListen = false;
        bool isGettingLastPos = false;
        TaskCompletionSource<Position> getPosisionTsc;
        public GeolocatorImplementation()
        {
            if (IsGooglePlayServicesInstalled())
            {
                var builder = new GoogleApiClientBuilder(this.Context);
                builder.AddApi(LocationServices.Api);
                builder.AddConnectionCallbacks(this);
                builder.AddOnConnectionFailedListener(this);
                _googleAPI = builder.Build();
                //_googleAPI = new GoogleApiClientBuilder(this.Context).AddApi(LocationServices.Api).AddConnectionCallbacks(this).AddOnConnectionFailedListener(this).Build(); 

                locRequest = new LocationRequest();
            }
            else
                Toast.MakeText(this.Context, "Google Play Services is not installed", ToastLength.Long).Show();
        }

        public Context Context
        {
            get
            {
                return Xamarin.Forms.Forms.Context;
            }
        }

        public event EventHandler<PositionErrorEventArgs> PositionError;

        public event EventHandler<PositionEventArgs> PositionChanged;

        public double DesiredAccuracy { get; set; }

        public bool IsListening
        {
            get { return this.isListen; }
        }

        public bool SupportsHeading
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsGeolocationAvailable
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsGeolocationEnabled
        {
            get { throw new NotImplementedException(); }
        }

        
        public Task<Position> GetPositionAsync(int timeout = Timeout.Infinite, CancellationToken? token = null, bool includeHeading = false)
        {
            if (!_googleAPI.IsConnected)
                this.connectGoogleAPI();

            isGettingLastPos = true;
            getPosisionTsc = new TaskCompletionSource<Position>();
            return getPosisionTsc.Task;            
        }
        

        public void StartListening(int minTime, double minDistance, bool includeHeading = false)
        {
            if (!_googleAPI.IsConnected)            
                this.connectGoogleAPI();
            
            isGettingLastPos = false;
            isListen = true;
        }

        public void StopListening()
        {
            if (_googleAPI.IsConnected)
            {                
                LocationServices.FusedLocationApi.RemoveLocationUpdates(_googleAPI, this);                
                disconnectGoogleAPI();
            }
            isListen = false;
        }

        public void OnConnected(Bundle connectionHint)
        {
            if (isGettingLastPos)
            {
                var location = LocationServices.FusedLocationApi.GetLastLocation(_googleAPI);
                getPosisionTsc.TrySetResult(new Position { Latitude = location.Latitude, Longitude = location.Longitude });

                if (_googleAPI.IsConnected)
                    this.disconnectGoogleAPI();
            }

            if (isListen)
                LocationServices.FusedLocationApi.RequestLocationUpdates(_googleAPI, locRequest, this);            
        }

        public void OnConnectionSuspended(int cause){}

        public void OnConnectionFailed(Android.Gms.Common.ConnectionResult result){}

        public void OnLocationChanged(Android.Locations.Location location)
        {
            if (this.PositionChanged != null)
                this.PositionChanged(this, new PositionEventArgs(new Position { Latitude = location.Latitude, Longitude = location.Longitude }));
        }

        private bool IsGooglePlayServicesInstalled()
        {
            int queryResult = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(this.Context);
            return queryResult == ConnectionResult.Success;            
        }       

        public void connectGoogleAPI()
        {
            System.Diagnostics.Debug.Assert(_googleAPI != null);

            if (!_googleAPI.IsConnectionCallbacksRegistered(this))
            {
                _googleAPI.RegisterConnectionCallbacks(this);
            }
            if (!_googleAPI.IsConnectionFailedListenerRegistered(this))
            {
                _googleAPI.RegisterConnectionFailedListener(this);
            }
            if (!_googleAPI.IsConnected || !_googleAPI.IsConnecting)
            {
                _googleAPI.Connect();
            }
        }

        public void disconnectGoogleAPI()
        {
            if (_googleAPI != null && _googleAPI.IsConnected)
            {
                if (_googleAPI.IsConnectionCallbacksRegistered(this))
                {
                    _googleAPI.UnregisterConnectionCallbacks(this);
                }
                if (_googleAPI.IsConnectionFailedListenerRegistered(this))
                {
                    _googleAPI.UnregisterConnectionFailedListener(this);
                }
                _googleAPI.Disconnect();
            }
        }
    }
}