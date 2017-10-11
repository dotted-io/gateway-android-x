using System;
using System.Collections.Generic;
using System.Linq;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using GatewayXNative.Models;

namespace GatewayXNative.Services
{
    public class GPSService : Java.Lang.Object, ILocationListener
    {
        public Location CurrentLocation;
        LocationManager locationManager;
        string locationProvider;

        public bool IsSupported { get { return locationManager != null; } }
        public bool IsEnabled { get { return  locationManager.IsProviderEnabled(LocationManager.GpsProvider) || locationManager.IsProviderEnabled(LocationManager.NetworkProvider); } }
        public bool IsNetworkEnabled { get { return locationManager.IsProviderEnabled(LocationManager.NetworkProvider); } }
        public bool IsGPSEnabled { get { return locationManager.IsProviderEnabled(LocationManager.GpsProvider); } }

        //public IntPtr Handle => throw new NotImplementedException();
        public static GPSService instance = null;

        public static GPSService SharedInstance(LocationManager manager = null)
        {
            if (instance == null)
            {
                instance = new GPSService(manager);
            }
            return instance;
        }

        public GPSService(LocationManager manager)
        {
            this.locationManager = manager;

            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }
            Log.Debug("GPS", "Using " + locationProvider + ".");
        }

        public bool StartGettingLocation()
        {
            locationManager.RequestLocationUpdates(locationProvider, (long)0, (float)0, this);
            return true;
        }

        public void OnLocationChanged(Location location)
        {
            Position p = new Position(location);
            LogService.SharedInstance().Add(p);
            CurrentLocation = location;
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }
    }
}
