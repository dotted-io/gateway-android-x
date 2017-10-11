using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Net.Wifi;
using GatewayXNative.Services;
using Android.Bluetooth;
using System;
using Android.Locations;
using Java.Lang;

namespace GatewayXNative
{
    [Activity(Label = "GatewayXNative", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        private int logPostInterval = 300000; // 5 minutes by default, can be changed later
        private Handler handler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Setup our UI
            InitializeLayout();

            //Initialize our logging service
            LogService.SharedInstance();
            //Initialize our Settings
            Settings.SharedInstance();

            //Start the bluetooth service up
            StartBluetoothService();

            //Start the GPS service up
            StartGPSService();

            handler = new Handler();
            StartRepeatingTask();

        }

        private void StartGPSService()
        {
            LocationManager manager = (LocationManager)GetSystemService(Context.LocationService);
            GPSService service = new GPSService(manager);

            if(service.IsSupported)
            {
                service.StartGettingLocation();
            }
        }

        private void StartBluetoothService()
        {
            BluetoothManager manager = (BluetoothManager)GetSystemService(Context.BluetoothService);
            BluetoothService bService = new BluetoothService(manager);
            if (!bService.IsEnabled)
            {
                Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableBtIntent, 1);
            }

            bService.StartScanning();
        }

        private void InitializeLayout()
        {
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            //Build our Wifi Setup Intent (with back button)
            Intent settingsIntent = new Intent(WifiManager.ActionPickWifiNetwork);
            settingsIntent.PutExtra("only_access_points", true);
            settingsIntent.PutExtra("extra_prefs_show_button_bar", true);
            settingsIntent.PutExtra("wifi_enable_next_on_connect", true);

            button.Click += delegate { StartActivity(settingsIntent); };
        }

        //Called when an activity that's been started has a result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

        }
        Runnable StatusChecker;
        void StartRepeatingTask()
        {
            StatusChecker = new Runnable(() =>
            {
                try
                {
                    Intent uploadLogsIntent = new Intent(this, typeof(LogService));

                    uploadLogsIntent.PutExtra("dotLogs", LogService.SharedInstance().GetSerializedDotLogsForPosting());
                    uploadLogsIntent.PutExtra("positions", LogService.SharedInstance().GetSerializedPositionsForPosting());
                    StartService(uploadLogsIntent);
                }
                finally
                {
                    handler.PostDelayed(StatusChecker, logPostInterval);
                }
            }); 
            StatusChecker.Run();
        }

    //@Override
    //public void run()
    //    {
    //        try
    //        {
    //            updateStatus(); //this function can change value of mInterval.
    //        }
    //        finally
    //        {
    //            // 100% guarantee that this always happens, even if
    //            // your update method throws an exception
    //            mHandler.postDelayed(mStatusChecker, mInterval);
    //        }
    //    }
    //};
    }
}

