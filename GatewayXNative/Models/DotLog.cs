using System;
using Android.Bluetooth.LE;
using GatewayXNative.Services;
using Java.Nio;
using Java.Util;

namespace GatewayXNative.Models
{
    public class DotLog
    {
        private ScanResult result;

        public int? logTypeId { get; set; }
        public int? capabilityId { get; set; }
        public string baseId { get; set; }
        public string dotId { get; set; }
        public int? rSSI { get; set; }
        public int? txPower { get; set; }
        public Position position { get; set; }
        public DateTime detectedAt { get; set; }
        public string value { get; set; }
        public string description { get; set; }

        public DotLog()
        {
            
        }

        public DotLog(ScanResult result)
        {
            this.result = result;

            byte[] value;
            if (result.ScanRecord.ServiceData.TryGetValue(BluetoothService.DottedUUID, out value))
            {
                if (value.Length == 16)
                {
                    this.dotId = GetGuidFromByteArray(value);
                    System.Diagnostics.Debug.WriteLine(dotId + " - " + result.Rssi.ToString());

                    this.rSSI = result.Rssi;
                    this.baseId = Settings.SharedInstance().BaseId;
                    this.logTypeId = (int)LogTypeEnum.Capability;
                    this.capabilityId = (int)CapabilityEnum.Proximity;
                    this.detectedAt = DateTime.UtcNow;
                    this.txPower = result.ScanRecord.TxPowerLevel;
                }
            }
        }

        public static string GetGuidFromByteArray(byte[] bytes)
        {
            ByteBuffer bb = ByteBuffer.Wrap(bytes);
            UUID uuid = new UUID(bb.Long, bb.Long);
            return uuid.ToString();
        }
    }

    public class DotLogMinified
    {
        //logTypeId
        public int? t { get; set; }
        //capabilityId
        public int? c { get; set; }
        //baseId
        public string b { get; set; }
        //dotId
        public string d { get; set; }
        //rSSI
        public int? r { get; set; }
        //txPower
        public int? x { get; set; }
        //position
        public PositionMinified p { get; set; }
        //detectedAt
        public DateTime a { get; set; }
        //value
        public string v { get; set; }
        //description
        public string s { get; set; }

        public DotLogMinified(DotLog log)
        {
            t = log.logTypeId;
            c = log.capabilityId;
            b = log.baseId;
            d = log.dotId;
            r = log.rSSI;
            x = log.txPower;
            a = log.detectedAt;
            v = log.value;
            s = log.description;

            if (log.position != null)
                p = new PositionMinified(log.position);

        }
    }

    public enum LogTypeEnum
    {
        Capability = 1,
        Position = 2,
        Audit = 3
    }

    public enum CapabilityEnum
    {
        Proximity = 1,
        Find = 2,
        Battery = 3,
        PowerProfile = 4
    }
}
