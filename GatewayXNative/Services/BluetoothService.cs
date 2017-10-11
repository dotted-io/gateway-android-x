using System.Collections.Generic;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.OS;
using GatewayXNative.Models;

namespace GatewayXNative.Services
{
    public class BluetoothService
    {
        BluetoothLeScanner scanner;
        BluetoothAdapter adapter;
        BluetoothManager bluetoothManager;

        public static ParcelUuid DottedUUID = ParcelUuid.FromString("0000FE57-0000-1000-8000-00805F9B34FB");
        BluetoothScanCallback scanCallback;

        private bool isScanning;
        private Handler scanHandler = new Handler();

        public bool IsSupported { get { return adapter != null; }}
        public bool IsEnabled { get { return adapter != null && adapter.IsEnabled; } }

        public BluetoothService(BluetoothManager manager)
        {
            bluetoothManager = manager;
            adapter = manager.Adapter;
            scanner = adapter.BluetoothLeScanner;
            scanCallback = new BluetoothScanCallback(this);
        }

        public bool StartScanning()
        {
            if (isScanning)
                return true;
            
            if (IsEnabled)
            {
                // Stops scanning after a pre-defined scan period.
                //scanHandler.PostDelayed(new Runnable(() =>
                //{
                //    isScanning = false;

                //    scanner.StopScan(scanCallback);
                //}), 10000);

                isScanning = true;
                ScanFilter.Builder builder = new ScanFilter.Builder();
                builder.SetServiceUuid(DottedUUID);
                ScanSettings.Builder sBuilder = new ScanSettings.Builder();
                sBuilder.SetScanMode(Android.Bluetooth.LE.ScanMode.LowPower);
                scanner.StartScan(new List<ScanFilter>{ builder.Build() },
                                  sBuilder.Build(),
                                  scanCallback);
                return true;
            } 
            else 
            {
                isScanning = false;
                scanner.StopScan(scanCallback);
                return false;
            }
        }

        public void DotDiscovered(ScanResult result)
        {
            DotLog log = new DotLog(result);
            LogService.SharedInstance().Add(log);
        }

        private void ScanCallback()
        {
            
        }
    }

    public class BluetoothScanCallback : ScanCallback
    {
        BluetoothService service;
        public BluetoothScanCallback(BluetoothService service)
        {
            this.service = service;
        }

        public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
        {
            base.OnScanResult(callbackType, result);
            if (result.ScanRecord != null && result.ScanRecord.ServiceData != null)
            {
                service.DotDiscovered(result);
            }
        }

        public override void OnBatchScanResults(System.Collections.Generic.IList<ScanResult> results)
        {
            base.OnBatchScanResults(results);
        }

        public override void OnScanFailed(ScanFailure errorCode)
        {
            base.OnScanFailed(errorCode);
        }
    }
}
