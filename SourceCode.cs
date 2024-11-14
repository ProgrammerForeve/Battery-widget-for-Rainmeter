using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Management;
using Rainmeter;

namespace PluginBattery 
{
    internal class Measure 
    {
        enum MeasureType 
        {
            BatteryPercentage,
            BatteryStatus,
            BatteryStatusNumeric,
            BatteryChargeStatus,
            BatteryChargeStatusNumeric,
            BatteryName,
            BatteryID
        }

        private MeasureType Type = MeasureType.BatteryPercentage;

        internal void Reload(Rainmeter.API api, ref double maxValue)
        {
            string type = api.ReadString("MeasureType", "");
            switch (type.ToLowerInvariant())
            {
                case "batterypercentage":
                    Type = MeasureType.BatteryPercentage;
                    break;
                case "batterystatus":
                    Type = MeasureType.BatteryStatus;
                    break;
                case "batterystatusnumeric":
                    Type = MeasureType.BatteryStatusNumeric;
                    break;
                case "batterychargestatus":
                    Type = MeasureType.BatteryChargeStatus;
                    break;
                case "batterychargestatusnumeric":
                    Type = MeasureType.BatteryChargeStatusNumeric;
                    break;
                case "batteryname":
                    Type = MeasureType.BatteryName;
                    break;
                case "batteryid":
                    Type = MeasureType.BatteryID;
                    break;
                default:
                    api.Log(API.LogType.Error, "PluginBattery.dll: MeasureType=" + type + " is not valid");
                    break;
            }
        }

        internal double Update()
        {
            switch (Type)
            {
                case MeasureType.BatteryPercentage:
                    return SystemInformation.PowerStatus.BatteryLifePercent * 100;

                case MeasureType.BatteryStatusNumeric:
                    return GetBatteryStatusNumeric();

                case MeasureType.BatteryChargeStatusNumeric:
                    return GetBatteryChargeStatusNumeric();
            }

            return 0.0;
        }

        internal string GetString()
        {
            switch (Type)
            {
                case MeasureType.BatteryStatus:
                    return GetBatteryStatusString();

                case MeasureType.BatteryChargeStatus:
                    return GetBatteryChargeStatusString();

                case MeasureType.BatteryName:
                    return GetBatteryName();

                case MeasureType.BatteryID:
                    return GetBatteryID();
            }
            return null;
        }

        private string GetBatteryStatusString()
        {
            var status = SystemInformation.PowerStatus.BatteryChargeStatus;
            if (status.HasFlag(BatteryChargeStatus.Charging))
                return "Charging";
            else if (status.HasFlag(BatteryChargeStatus.High))
                return "High";
            else if (status.HasFlag(BatteryChargeStatus.Low))
                return "Low";
            else if (status.HasFlag(BatteryChargeStatus.Critical))
                return "Critical";
            else
                return "Unknown";
        }

        private int GetBatteryStatusNumeric()
        {
            var status = SystemInformation.PowerStatus.BatteryChargeStatus;
            if (status.HasFlag(BatteryChargeStatus.Critical))
                return 3;
            else if (status.HasFlag(BatteryChargeStatus.Low))
                return 2;
            else if (status.HasFlag(BatteryChargeStatus.High) && !status.HasFlag(BatteryChargeStatus.Charging))
                return 1;
            else if (status.HasFlag(BatteryChargeStatus.Charging))
                return 4;
            else
                return 0; // Unknown or default state
        }

        private string GetBatteryChargeStatusString()
        {
            return SystemInformation.PowerStatus.BatteryChargeStatus.HasFlag(BatteryChargeStatus.Charging) ? "Charging" : "Discharging";
        }

        private int GetBatteryChargeStatusNumeric()
        {
            return SystemInformation.PowerStatus.BatteryChargeStatus.HasFlag(BatteryChargeStatus.Charging) ? 1 : 0;
        }

        private string GetBatteryName()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Battery"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return obj["Name"]?.ToString() ?? "Unknown";
                    }
                }
            }
            catch
            {
                return "Unknown";
            }
            return "Unknown";
        }

        private string GetBatteryID()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT DeviceID FROM Win32_Battery"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return obj["DeviceID"]?.ToString() ?? "Unknown";
                    }
                }
            }
            catch
            {
                return "Unknown";
            }
            return "Unknown";
        }
    }

    public static class Plugin
    {
        static IntPtr StringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            GCHandle.FromIntPtr(data).Free();

            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }
    }
}
