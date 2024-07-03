// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_ANDROID

using System;
using System.Threading;
using UnityEngine;

namespace MobileSupport
{
    partial class Thermal
    {
        public static class Android
        {
            /// <summary>
            ///     The latest battery temperature multiplied by 100 in Celsius.
            ///     Thermal.StartMonitoring() is required to get the latest value.
            /// </summary>
            public static int? LatestBatteryTemperature { get; private set; }

            /// <summary>
            ///     The latest battery voltage in mV.
            ///     Thermal.StartMonitoring() is required to get the latest value.
            /// </summary>
            public static int? LatestBatteryVoltage { get; private set; }

            /// <summary>
            ///     Event that is sent when the battery temperature is changed.
            ///     Thermal.StartMonitoring() is required to get the latest value.
            /// </summary>
            /// <returns>The battery temperature multiplied by 100 in Celsius.</returns>
            public static event Action<int> OnBatteryTemperatureChanged;

            /// <summary>
            ///     Event that is sent when the battery voltage is changed.
            ///     Thermal.StartMonitoring() is required to get the latest value.
            /// </summary>
            /// <returns>The battery voltage in mV.</returns>
            public static event Action<int> OnBatteryVoltageChanged;

            /// <summary>
            ///     Tries to get the thermal headroom through android.os.PowerManager.getThermalHeadroom().
            ///     Calling this method multiple times within a second will return the cached value even if the forecastSeconds is
            ///     different.
            /// </summary>
            /// <param name="forecastSeconds">how many seconds in the future to forecast</param>
            /// <param name="result">the thermal headroom value</param>
            /// <param name="resultForecastSeconds">
            ///     the forecast seconds of the result. It maybe different from requested value when
            ///     the cached value is returned.
            /// </param>
            /// <param name="isLatestValue">whether if it succeeded to get the latest value.</param>
            public static void GetThermalHeadroom(int forecastSeconds, out float result, out int resultForecastSeconds,
                out bool isLatestValue)
            {
#if UNITY_EDITOR
                if (Application.isEditor)
                {
                    result = float.NaN;
                    resultForecastSeconds = forecastSeconds;
                    isLatestValue = true;
                    return;
                }
#endif
                var time = DateTime.UtcNow;

                lock (ThermalHeadroomLocker)
                {
                    if (_latestThermalHeadroom.HasValue &&
                        _latestThermalHeadroomTime.HasValue &&
                        time - _latestThermalHeadroomTime < TimeSpan.FromSeconds(1.0))
                    {
                        result = _latestThermalHeadroom.Value;
                        resultForecastSeconds = _latestThermalHeadroomForecastSeconds;
                        isLatestValue = false;
                        return;
                    }

                    var powerManager = _powerManager ??= new AndroidPowerManager();
                    result = powerManager.GetThermalHeadroom(forecastSeconds);

                    _latestThermalHeadroomTime = DateTime.UtcNow;
                    _latestThermalHeadroom = result;
                    resultForecastSeconds = _latestThermalHeadroomForecastSeconds = forecastSeconds;
                    isLatestValue = true;
                }
            }

            /// <summary>
            ///     The value of BatteryManager's BATTERY_PROPERTY_ENERGY_COUNTER.
            ///     Battery remaining energy in nanowatt-hours, as a long integer.
            ///     Returns long.MinValue on unsupported devices.
            /// </summary>
            /// <returns></returns>
            public static long GetBatteryEnergyCounter()
            {
                return (_batteryManager ??= new AndroidBatteryManager()).GetEnergyCounter();
            }

            /// <summary>
            ///     The value of BatteryManager's BATTERY_PROPERTY_CURRENT_NOW.
            ///     Instantaneous battery current in microamperes, as an integer. Positive values indicate net current entering the
            ///     battery from a charge source, negative values indicate net current discharging from the battery.
            ///     Returns int.MinValue on unsupported devices.
            /// </summary>
            /// <returns></returns>
            public static int GetBatteryCurrentNow()
            {
                return (_batteryManager ??= new AndroidBatteryManager()).GetCurrentNow();
            }

            /// <summary>
            ///     The value of BatteryManager's BATTERY_PROPERTY_CHARGE_COUNTER.
            ///     Battery capacity in microampere-hours, as an integer.
            ///     Returns int.MinValue on unsupported devices.
            /// </summary>
            /// <returns></returns>
            public static int GetBatteryChargeCounter()
            {
                return (_batteryManager ??= new AndroidBatteryManager()).GetChargeCounter();
            }

            /// <summary>
            ///     The value of BatteryManager's BATTERY_PROPERTY_CURRENT_AVERAGE.
            ///     Average battery current in microamperes, as an integer. Positive values indicate net current entering the battery
            ///     from a charge source, negative values indicate net current discharging from the battery. The time period over which
            ///     the average is computed may depend on the fuel gauge hardware and its configuration.
            ///     Returns int.MinValue on unsupported devices.
            /// </summary>
            /// <returns></returns>
            public static int GetBatteryCurrentAverage()
            {
                return (_batteryManager ??= new AndroidBatteryManager()).GetCurrentAverage();
            }

            /// <summary>
            ///     The value of BatteryManager's BATTERY_PROPERTY_CAPACITY.
            ///     Remaining battery capacity as an integer percentage of total capacity (with no fractional part).
            ///     Returns int.MinValue on unsupported devices.
            /// </summary>
            /// <returns></returns>
            public static int GetBatteryCapacity()
            {
                return (_batteryManager ??= new AndroidBatteryManager()).GetCapacity();
            }

            internal static void OnBatteryTemperatureChangedCallback(int value)
            {
                _mainThreadContext.Post(_ =>
                {
                    LatestBatteryTemperature = value;

                    OnBatteryTemperatureChanged?.Invoke(value);
                }, null);
            }

            internal static void OnBatteryVoltageChangedCallback(int value)
            {
                _mainThreadContext.Post(_ =>
                {
                    LatestBatteryVoltage = value;

                    OnBatteryVoltageChanged?.Invoke(value);
                }, null);
            }
        }

        #region Nested type: ThermalStatusListener

        private class ThermalStatusListener : AndroidJavaProxy
        {
            private readonly Action<ThermalStatusAndroid> _callback;

            public ThermalStatusListener(Action<ThermalStatusAndroid> callback)
                : base("android.os.PowerManager$OnThermalStatusChangedListener")
            {
                _callback = callback;
            }

            public override AndroidJavaObject Invoke(string methodName, object[] args)
            {
                // avoid reflection
                switch (methodName)
                {
                    case "hashCode":
                        return new AndroidJavaObject("java.lang.Integer", hashCode());
                    case "toString":
                        return new AndroidJavaObject("java.lang.String", toString());
                    case "equals":
                        return new AndroidJavaObject("java.lang.Boolean", equals((AndroidJavaObject)args[0]));
                    case "onThermalStatusChanged":
                        _callback?.Invoke((ThermalStatusAndroid)(int)args[0]);
                        return null;
                    default:
                        return base.Invoke(methodName, args);
                }
            }
        }

        #endregion

        #region Nested type: BatteryStatusReceiver

        private class BatteryStatusReceiver : AndroidJavaProxy
        {
            private readonly Action<float> _onLevelReceived;
            private readonly Action<int> _onStatusReceived;
            private readonly Action<int> _onTemperatureReceived;
            private readonly Action<int> _onVoltgageReceived;

            public BatteryStatusReceiver(Action<int> onTemperatureReceived, Action<int> onVoltgageReceived,
                Action<int> onStatusReceived, Action<float> onLevelReceived) : base(
                "jp.co.cyberagent.unitysupport.thermal.BatteryStatusReceiver")
            {
                _onTemperatureReceived = onTemperatureReceived;
                _onVoltgageReceived = onVoltgageReceived;
                _onStatusReceived = onStatusReceived;
                _onLevelReceived = onLevelReceived;
            }

            private void OnReceiveBatteryTemperature(int temperature)
            {
                _onTemperatureReceived?.Invoke(temperature);
            }

            private void OnReceiveVoltage(int voltage)
            {
                _onVoltgageReceived?.Invoke(voltage);
            }

            private void OnReceiveStatus(int status)
            {
                _onStatusReceived?.Invoke(status);
            }

            private void OnReceiveLevel(float level)
            {
                _onLevelReceived?.Invoke(level);
            }

            public override AndroidJavaObject Invoke(string methodName, object[] args)
            {
                // avoid reflection
                switch (methodName)
                {
                    case "hashCode":
                        return new AndroidJavaObject("java.lang.Integer", hashCode());
                    case "toString":
                        return new AndroidJavaObject("java.lang.String", toString());
                    case "equals":
                        return new AndroidJavaObject("java.lang.Boolean", equals((AndroidJavaObject)args[0]));
                    case "onReceiveBatteryTemperature":
                        OnReceiveBatteryTemperature((int)args[0]);
                        return null;
                    case "onReceiveVoltage":
                        OnReceiveVoltage((int)args[0]);
                        return null;
                    case "onReceiveStatus":
                        OnReceiveStatus((int)args[0]);
                        return null;
                    case "onReceiveLevel":
                        OnReceiveLevel((float)args[0]);
                        return null;
                    default:
                        return base.Invoke(methodName, args);
                }
            }
        }

        #endregion

        private static readonly ThermalStatusListener JavaCallbackListener = new(OnThermalStatusChangedCallback);

        private static readonly BatteryStatusReceiver BatteryTemperatureReceiverInstance =
            new(Android.OnBatteryTemperatureChangedCallback, Android.OnBatteryVoltageChangedCallback, OnBatteryStatusChangedCallback,
                OnBatteryLevelChangedCallback);

        private static readonly AndroidJavaObject BatteryChangedBroadcastReceiverInstance =
            new("jp.co.cyberagent.unitysupport.thermal.BatteryChangedBroadcastReceiver");

        private static AndroidPowerManager _powerManager;

        private static AndroidBatteryManager _batteryManager;

        private static SynchronizationContext _mainThreadContext;

        [ThreadStatic] private static jvalue[] _tempSingleJValueArray;
        [ThreadStatic] private static object[] _tempSingleObjectArray;

        private static float? _latestThermalHeadroom;
        private static DateTime? _latestThermalHeadroomTime;
        private static int _latestThermalHeadroomForecastSeconds;

        private static readonly object ThermalHeadroomLocker = new();

        private static bool _isMonitoring;
        private static jvalue[] TempSingleJValueArray => _tempSingleJValueArray ??= new jvalue[1];
        private static object[] TempSingleObjectArray => _tempSingleObjectArray ??= new object[1];

        private static ThermalStatus? _latestThermalStatus;
        private static BatteryStatus? _latestBatteryStatus;
        private static float? _latestBatteryLevel;
        
        private static event Action<ThermalStatus> OnThermalStatusChangedInternal;
        private static event Action<BatteryStatus> OnBatteryStatusChangedInternal;
        private static event Action<float> OnBatteryLevelChangedInternal;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            _mainThreadContext = SynchronizationContext.Current;
        }

        public static partial void StartMonitoring()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return;
#endif

            if (_isMonitoring) return;

            (_powerManager ??= new AndroidPowerManager()).AddThermalStatusListener(JavaCallbackListener);

            StartReceiveBatteryTemperature();

            _isMonitoring = true;
        }

        public static partial void StopMonitoring()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return;
#endif

            if (_isMonitoring == false) return;

            _powerManager.RemoveThermalStatusListener(JavaCallbackListener);

            StopReceiveBatteryTemperature();

            _isMonitoring = false;
        }

        private static partial ThermalStatus? GetLatestThermalStatus() => _latestThermalStatus;

        private static partial BatteryStatus? GetLatestBatteryStatus() => _latestBatteryStatus;

        private static partial float? GetLatestBatteryLevel() => _latestBatteryLevel;

        private static void StartReceiveBatteryTemperature()
        {
            using var playerClass = GetUnityPlayerClass();
            using var activity = GetCurrentActivity(playerClass);
            using var context = GetApplicationContext(activity);

            BatteryChangedBroadcastReceiverInstance.Call("registerToContext", context);
            BatteryChangedBroadcastReceiverInstance.Call("addReceiver", BatteryTemperatureReceiverInstance);
        }

        private static void StopReceiveBatteryTemperature()
        {
            using var playerClass = GetUnityPlayerClass();
            using var activity = GetCurrentActivity(playerClass);
            using var context = GetApplicationContext(activity);

            BatteryChangedBroadcastReceiverInstance.Call("removeReceiver", BatteryTemperatureReceiverInstance);
            BatteryChangedBroadcastReceiverInstance.Call("unregisterFromContext", context);
        }

        private static AndroidJavaClass GetUnityPlayerClass()
        {
            return new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        }

        private static AndroidJavaObject GetCurrentActivity(AndroidJavaClass unityPlayerClass)
        {
            return unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        }

        private static AndroidJavaObject GetApplicationContext(AndroidJavaObject activity)
        {
            return activity.Call<AndroidJavaObject>("getApplicationContext");
        }

        private static void OnThermalStatusChangedCallback(ThermalStatusAndroid status)
        {
            _mainThreadContext.Post(_ =>
            {
                _latestThermalStatus = status;

                // May be converted to an enum.
                OnThermalStatusChangedInternal?.Invoke(status);
            }, null);
        }

        private static void OnBatteryStatusChangedCallback(int value)
        {
            var status = value switch
            {
                2 => BatteryStatus.Charging,
                3 => BatteryStatus.Discharging,
                4 => BatteryStatus.NotCharging,
                5 => BatteryStatus.Full,
                _ => BatteryStatus.Unknown
            };

            _mainThreadContext.Post(_ =>
            {
                _latestBatteryStatus = status;

                OnBatteryStatusChangedInternal?.Invoke(status);
            }, null);
        }

        private static void OnBatteryLevelChangedCallback(float value)
        {
            _mainThreadContext.Post(_ =>
            {
                _latestBatteryLevel = value;

                OnBatteryLevelChangedInternal?.Invoke(value);
            }, null);
        }

        private static partial void AddOnThermalStatusChangedListener(Action<ThermalStatus> listener) => OnThermalStatusChangedInternal += listener;
        private static partial void RemoveOnThermalStatusChangedListener(Action<ThermalStatus> listener) => OnThermalStatusChangedInternal -= listener;
        private static partial void AddOnBatteryStatusChangedListener(Action<BatteryStatus> listener) => OnBatteryStatusChangedInternal += listener;
        
        private static partial void RemoveOnBatteryStatusChangedListener(Action<BatteryStatus> listener) => OnBatteryStatusChangedInternal -= listener;
        private static partial void AddOnBatteryLevelChangedListener(Action<float> listener) => OnBatteryLevelChangedInternal += listener;
        
        private static partial void RemoveOnBatteryLevelChangedListener(Action<float> listener) => OnBatteryLevelChangedInternal -= listener;

        #region Nested type: AndroidPowerManager

        private sealed class AndroidPowerManager : IDisposable
        {
            private static IntPtr? _getThermalHeadroomMethodId;
            private static IntPtr? _addThermalStatusListenerMethodId;
            private static IntPtr? _removeThermalStatusListenerMethodId;

            private AndroidJavaObject _powerManager;

            public AndroidPowerManager()
            {
                using var playerClass = GetUnityPlayerClass();
                using var activity = GetCurrentActivity(playerClass);
                using var staticContext = new AndroidJavaClass("android.content.Context");
                using var service = staticContext.GetStatic<AndroidJavaObject>("POWER_SERVICE");
                _powerManager = activity.Call<AndroidJavaObject>("getSystemService", service);
            }

            #region IDisposable Members

            public void Dispose()
            {
                DisposeCore();
                GC.SuppressFinalize(this);
            }

            #endregion

            public float GetThermalHeadroom(int forecastSeconds)
            {
                var getThermalHeadroomMethodId = _getThermalHeadroomMethodId ??=
                    AndroidJNIHelper.GetMethodID(_powerManager.GetRawClass(), "getThermalHeadroom", "(I)F");

                TempSingleJValueArray[0] = new jvalue
                {
                    i = forecastSeconds
                };

                return AndroidJNI.CallFloatMethod(_powerManager.GetRawObject(), getThermalHeadroomMethodId,
                    TempSingleJValueArray);
            }

            public void AddThermalStatusListener(ThermalStatusListener listener)
            {
                var addThermalStatusListenerMethodId = _addThermalStatusListenerMethodId ??=
                    AndroidJNIHelper.GetMethodID(_powerManager.GetRawClass(), "addThermalStatusListener",
                        "(Landroid/os/PowerManager$OnThermalStatusChangedListener;)V");

                // AndroidJavaProxy doesn't expose the raw object pointer
                TempSingleObjectArray[0] = listener;
                var argArray = AndroidJNIHelper.CreateJNIArgArray(TempSingleObjectArray);
                AndroidJNI.CallVoidMethod(_powerManager.GetRawObject(), addThermalStatusListenerMethodId, argArray);

                AndroidJNIHelper.DeleteJNIArgArray(TempSingleObjectArray, argArray);
            }

            public void RemoveThermalStatusListener(ThermalStatusListener listener)
            {
                var removeThermalStatusListenerMethodId = _removeThermalStatusListenerMethodId ??=
                    AndroidJNIHelper.GetMethodID(_powerManager.GetRawClass(), "removeThermalStatusListener",
                        "(Landroid/os/PowerManager$OnThermalStatusChangedListener;)V");

                // AndroidJavaProxy doesn't expose the raw object pointer
                TempSingleObjectArray[0] = listener;
                var argArray = AndroidJNIHelper.CreateJNIArgArray(TempSingleObjectArray);
                AndroidJNI.CallVoidMethod(_powerManager.GetRawObject(), removeThermalStatusListenerMethodId, argArray);

                AndroidJNIHelper.DeleteJNIArgArray(TempSingleObjectArray, argArray);
            }

            private void DisposeCore()
            {
                var powerManager = _powerManager;
                if (powerManager == null) return;

                // thread safety
                if (Interlocked.CompareExchange(ref _powerManager, null, powerManager) == null) return;

                powerManager.Dispose();
            }

            ~AndroidPowerManager()
            {
                DisposeCore();
            }
        }

        #endregion

        #region Nested type: AndroidBatteryManager

        private sealed class AndroidBatteryManager : IDisposable
        {
            private static IntPtr? _getLongPropertyMethodId;
            private static IntPtr? _getIntPropertyMethodId;
            private static int? _batteryPropertyChargeCounter;
            private static int? _batteryPropertyCurrentNow;
            private static int? _batteryPropertyCurrentAverage;
            private static int? _batteryPropertyCapacity;
            private static int? _batteryPropertyEnergyCounter;

            private AndroidJavaObject _batteryManager;

            public AndroidBatteryManager()
            {
                using var playerClass = GetUnityPlayerClass();
                using var activity = GetCurrentActivity(playerClass);
                using var staticContext = new AndroidJavaClass("android.content.Context");
                using var service = staticContext.GetStatic<AndroidJavaObject>("BATTERY_SERVICE");
                _batteryManager = activity.Call<AndroidJavaObject>("getSystemService", service);
            }

            private int BatteryPropertyEnergyCounter => _batteryPropertyEnergyCounter ??=
                _batteryManager.GetStatic<int>("BATTERY_PROPERTY_ENERGY_COUNTER");

            private int BatteryPropertyCurrentNow => _batteryPropertyCurrentNow ??=
                _batteryManager.GetStatic<int>("BATTERY_PROPERTY_CURRENT_NOW");

            private int BatteryPropertyChargeCounter => _batteryPropertyChargeCounter ??=
                _batteryManager.GetStatic<int>("BATTERY_PROPERTY_CHARGE_COUNTER");

            private int BatteryPropertyCurrentAverage => _batteryPropertyCurrentAverage ??=
                _batteryManager.GetStatic<int>("BATTERY_PROPERTY_CURRENT_AVERAGE");

            private int BatteryPropertyCapacity => _batteryPropertyCapacity ??=
                _batteryManager.GetStatic<int>("BATTERY_PROPERTY_CAPACITY");

            #region IDisposable Members

            public void Dispose()
            {
                DisposeCore();
                GC.SuppressFinalize(this);
            }

            #endregion

            private long GetLongProperty(int id)
            {
                var getLongPropertyMethodId = _getLongPropertyMethodId ??=
                    AndroidJNIHelper.GetMethodID(_batteryManager.GetRawClass(), "getLongProperty", "(I)J");

                TempSingleJValueArray[0] = new jvalue
                {
                    i = id
                };

                return AndroidJNI.CallLongMethod(_batteryManager.GetRawObject(), getLongPropertyMethodId,
                    TempSingleJValueArray);
            }

            private int GetIntProperty(int id)
            {
                var getIntPropertyMethodId = _getIntPropertyMethodId ??=
                    AndroidJNIHelper.GetMethodID(_batteryManager.GetRawClass(), "getIntProperty", "(I)I");

                TempSingleJValueArray[0] = new jvalue
                {
                    i = id
                };

                return AndroidJNI.CallIntMethod(_batteryManager.GetRawObject(), getIntPropertyMethodId,
                    TempSingleJValueArray);
            }

            public long GetEnergyCounter()
            {
                return GetLongProperty(BatteryPropertyEnergyCounter);
            }

            public int GetCurrentNow()
            {
                return GetIntProperty(BatteryPropertyCurrentNow);
            }

            public int GetChargeCounter()
            {
                return GetIntProperty(BatteryPropertyChargeCounter);
            }

            public int GetCurrentAverage()
            {
                return GetIntProperty(BatteryPropertyCurrentAverage);
            }

            public int GetCapacity()
            {
                return GetIntProperty(BatteryPropertyCapacity);
            }

            public int GetVoltage()
            {
                return GetIntProperty(BatteryPropertyCapacity);
            }

            private void DisposeCore()
            {
                var batteryManager = _batteryManager;
                if (batteryManager == null) return;

                // thread safety
                if (Interlocked.CompareExchange(ref _batteryManager, null, batteryManager) == null) return;

                batteryManager.Dispose();
            }

            ~AndroidBatteryManager()
            {
                DisposeCore();
            }
        }

        #endregion
    }
}

#endif
