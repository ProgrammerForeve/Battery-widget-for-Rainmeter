using Rainmeter;
using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PluginBattery
{
    /// <summary>
    /// Основной класс, представляющий измерения батареи.
    /// </summary>
    internal class Measure
    {
        /// <summary>
        /// Типы измерений, доступные для отображения.
        /// </summary>
        private enum MeasureType
        {
            BatteryPercentage,              // Заряд в %
            BatteryStatus,                  // Статус (текст)
            BatteryStatusNumeric,           // Статус (номер)
            BatteryChargeStatus,            // Статус зарядки
            BatteryChargeStatusNumeric,     // Статус зарядки (номер)
            BatteryName,                    // Имя 
            BatteryID,                      // id

            BatteryChargeRate,              // Мощность зарядки mW
            BatteryDischargeRate,           // Мощность разрядки mW
            BatteryRemainingCapacity,       // Ёмкость до полной зарядки mWh
            BatteryVoltage,                 // Напряжение батареи В

            BatteryChargeRateStr,           // Мощность зарядки W (строка)
            BatteryDischargeRateStr,        // Мощность разрядки W (строка)
            BatteryRemainingCapacityStr,    // Ёмкость до полной зарядки Wh (строка)
            BatteryVoltageStr               // Напряжение батареи В (строка)

        }

        private MeasureType _type = MeasureType.BatteryPercentage;

        private string DLL_NAME = "PluginBattery.dll";

        /// <summary>
        /// Инициализация измерения с параметрами из Rainmeter.
        /// </summary>
        /// <param name="api">Объект API для взаимодействия с Rainmeter.</param>
        /// <param name="maxValue">Максимальное значение для измерения.</param>
        internal void Reload(Rainmeter.API api, ref double maxValue)
        {
            string type = api.ReadString("MeasureType", "").ToLowerInvariant();

            if (Enum.TryParse(type, true, out MeasureType parsedType))
            {
                _type = parsedType;
            }
            else
            {
                api.Log(API.LogType.Error, $"{DLL_NAME}: MeasureType={type} is not valid");
            }
        }

        /// <summary>
        /// Обновляет значение измерения.
        /// </summary>
        /// <returns>Числовое значение измерения.</returns>
        internal double Update()
        {
            return _type switch
            {
                MeasureType.BatteryPercentage => GetBatteryPercentage(),
                MeasureType.BatteryStatusNumeric => GetBatteryStatusNumeric(),
                MeasureType.BatteryChargeStatusNumeric => GetBatteryChargeStatusNumeric(),
                MeasureType.BatteryChargeRate => GetChargeRate(),
                MeasureType.BatteryDischargeRate => GetDischargeRate(),
                MeasureType.BatteryRemainingCapacity => GetRemainingCapacity(),
                MeasureType.BatteryVoltage => GetVoltage(),
                _ => 0.0
            };
        }

        /// <summary>
        /// Получает строковое значение измерения.
        /// </summary>
        /// <returns>Строковое значение измерения.</returns>
        internal string GetString()
        {
            return _type switch
            {
                MeasureType.BatteryStatus => GetBatteryStatusString(),
                MeasureType.BatteryChargeStatus => GetBatteryChargeStatusString(),
                MeasureType.BatteryName => GetBatteryName(),
                MeasureType.BatteryID => GetBatteryID(),
                MeasureType.BatteryChargeRateStr => GetBatteryChargeRateStr(),            // Мощность зарядки W (строка)
                MeasureType.BatteryDischargeRateStr => BatteryDischargeRateStr(),         // Мощность разрядки W (строка)
                MeasureType.BatteryRemainingCapacityStr => BatteryRemainingCapacityStr(), // Ёмкость до полной зарядки Wh (строка)
                MeasureType.BatteryVoltageStr => BatteryVoltageStr()                      // Напряжение батареи В (строка)
                    _ => null
            };
        }

        // Методы получения данных о батарее

        /// <summary>
        /// Получает строковый статус батареи.
        /// </summary>
        private string GetBatteryStatusString()
        {
            const string DEFAULT = "Unknown";
            var status = SystemInformation.PowerStatus.BatteryChargeStatus;
            return status switch
            {
                var s when s.HasFlag(BatteryChargeStatus.Charging) => "Charging",
                var s when s.HasFlag(BatteryChargeStatus.High) => "High",
                var s when s.HasFlag(BatteryChargeStatus.Low) => "Low",
                var s when s.HasFlag(BatteryChargeStatus.Critical) => "Critical",
                _ => DEFAULT
            };
        }

        /// <summary>
        /// Получает процент заряда батареи
        /// </summary>
        private double GetBatteryPercentage()
        {
            return SystemInformation.PowerStatus.BatteryLifePercent * 100;
        }

        /// <summary>
        /// Получает числовой статус батареи.
        /// </summary>
        private int GetBatteryStatusNumeric()
        {
            var status = SystemInformation.PowerStatus.BatteryChargeStatus;

            if (status.HasFlag(BatteryChargeStatus.High) && !status.HasFlag(BatteryChargeStatus.Charging)) return 1;
            if (status.HasFlag(BatteryChargeStatus.Low)) return 2;
            if (status.HasFlag(BatteryChargeStatus.Critical)) return 3;
            if (status.HasFlag(BatteryChargeStatus.Charging)) return 4;

            return 0; // Unknown
        }

        /// <summary>
        /// Получает строковый статус зарядки батареи.
        /// </summary>
        private string GetBatteryChargeStatusString()
        {
            return SystemInformation.PowerStatus.BatteryChargeStatus.HasFlag(BatteryChargeStatus.Charging)
                ? "Charging"
                : "Discharging";
        }

        /// <summary>
        /// Получает числовой статус зарядки батареи.
        /// </summary>
        private int GetBatteryChargeStatusNumeric()
        {
            return SystemInformation.PowerStatus.BatteryChargeStatus.HasFlag(BatteryChargeStatus.Charging) ? 1 : 0;
        }

        /// <summary>
        /// Получает имя батареи.
        /// </summary>
        private string GetBatteryName()
        {
            return QueryWmiValue("SELECT Name FROM Win32_Battery", "Name");
        }

        /// <summary>
        /// Получает ID батареи.
        /// </summary>
        private string GetBatteryID()
        {
            return QueryWmiValue("SELECT DeviceID FROM Win32_Battery", "DeviceID");
        }

        /// <summary>
        /// Получает скорость зарядки батареи.
        /// </summary>
        private double GetChargeRate()
        {
            return QueryWmiValueDouble("SELECT ChargeRate FROM BatteryStatus", "ChargeRate", "root\\wmi");
        }

        /// <summary>
        /// Получает скорость разрядки батареи.
        /// </summary>  
        private double GetDischargeRate()
        {
            return QueryWmiValueDouble("SELECT DischargeRate FROM BatteryStatus", "DischargeRate", "root\\wmi");
        }

        /// <summary>
        /// Получает оставшуюся емкость батареи.
        /// </summary>
        private double GetRemainingCapacity()
        {
            return QueryWmiValueDouble("SELECT RemainingCapacity FROM BatteryStatus", "RemainingCapacity", "root\\wmi");
        }

        /// <summary>
        /// Получает напряжение батареи.
        /// </summary>
        private double GetVoltage()
        {
            return QueryWmiValueDouble("SELECT Voltage FROM BatteryStatus", "Voltage", "root\\wmi");
        }

        /// <summary>
        /// Запрашивает значение из WMI.
        /// </summary>
        private string QueryWmiValue(string query, string property, string scope = "root\\cimv2")
        {
            const string DEFAULT = "Unknown";
            try
            {
                using var searcher = new ManagementObjectSearcher(new ManagementScope(scope), new SelectQuery(query));
                foreach (var obj in searcher.Get())
                {
                    return obj[property]?.ToString() ?? DEFAULT;
                }
            }
            catch
            {
                return DEFAULT;
            }
            return DEFAULT;
        }

        private double QueryWmiValueDouble(string query, string property, string scope = "root\\cimv2")
        {
            const double DEFAULT = 0.0;
            try
            {
                using var searcher = new ManagementObjectSearcher(new ManagementScope(scope), new SelectQuery(query));
                foreach (var obj in searcher.Get())
                {
                    return (double)(obj[property] ?? DEFAULT);
                }
            }
            catch
            {
                return DEFAULT;
            }
            return DEFAULT;
        }
    }

    /// <summary>
    /// Плагин для Rainmeter.
    /// </summary>
    public static class Plugin
    {
        private static IntPtr _stringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new Measure()));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            GCHandle.FromIntPtr(data).Free();
            if (_stringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_stringBuffer);
                _stringBuffer = IntPtr.Zero;
            }
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            var measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            var measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            var measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (_stringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_stringBuffer);
                _stringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                _stringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return _stringBuffer;
        }
    }
}
