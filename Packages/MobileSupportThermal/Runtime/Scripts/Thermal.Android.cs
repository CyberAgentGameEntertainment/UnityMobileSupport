// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_ANDROID

using System;
using System.Threading;
using UnityEngine;

namespace MobileSupport
{
    public static class Thermal
    {
        private class ThermalStatusListener : AndroidJavaProxy
        {
            private readonly Action<int> _callback;
            
            public ThermalStatusListener(Action<int> callback)
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
                        _callback?.Invoke((int)args[0]);
                        return null;
                    default:
                        return base.Invoke(methodName, args);
                }
            }
        }

        private class BatteryTemperatureReceiver : AndroidJavaProxy
        {
            private readonly Action<int> _callback;

            public BatteryTemperatureReceiver(Action<int> callback) : base(
                "jp.co.cyberagent.unitysupport.thermal.BatteryTemperatureReceiver")
            {
                _callback = callback;
            }

            private void OnReceiveBatteryTemperature(int temperature)
            {
                _callback?.Invoke(temperature);
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
                    default:
                        return base.Invoke(methodName, args);
                }
            }
        }

        private static readonly ThermalStatusListener JavaCallbackListener =
            new ThermalStatusListener(OnThermalStatusChangedCallback);

        private static readonly BatteryTemperatureReceiver BatteryTemperatureReceiverInstance =
            new(OnBatteryTemperatureChangedCallback);

        private static readonly AndroidJavaObject BatteryChangedBroadcastReceiverInstance =
            new("jp.co.cyberagent.unitysupport.thermal.BatteryChangedBroadcastReceiver");

        private static AndroidPowerManager _powerManager;

        private static SynchronizationContext _mainThreadContext;
        
        /// <summary>
        ///     Event that is sent when the thermal status is changed.
        /// </summary>
        /// <returns>The raw value of android.os.PowerManager.THERMAL_STATUS_XXX (0~7).</returns>
        public static event Action<int> OnThermalStatusChanged;

        /// <summary>
        ///     Event that is sent when the battery temperature is changed.
        /// </summary>
        /// <returns>The battery temperature multiplied by 100 in Celsius.</returns>
        public static event Action<int> OnBatteryTemperatureChanged;

        /// <summary>
        ///     The latest thermal status.
        /// </summary>
        public static int? LatestThermalStatus { get; private set; }

        /// <summary>
        ///     The latest battery temperature.
        /// </summary>
        public static int? LatestBatteryTemperature { get; private set; }

        private static float? _latestThermalHeadroom;
        private static DateTime? _latestThermalHeadroomTime;
        private static int _latestThermalHeadroomForecastSeconds;

        private static readonly object ThermalHeadroomLocker = new();

        private static bool _isMonitoring;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            _mainThreadContext = SynchronizationContext.Current;
        }

        /// <summary>
        ///     Start thermal status monitoring.
        /// </summary>
        public static void StartMonitoring()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return;
#endif

            if (_isMonitoring) return;

            (_powerManager ??= new AndroidPowerManager()).AddThermalStatusListener(JavaCallbackListener);

            StartReceiveBatteryTemperature();

            _isMonitoring = true;
        }

        /// <summary>
        ///     Stop thermal status monitoring.
        /// </summary>
        public static void StopMonitoring()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return;
#endif

            if (_isMonitoring == false) return;

            _powerManager.RemoveThermalStatusListener(JavaCallbackListener);

            StopReceiveBatteryTemperature();

            _isMonitoring = false;
        }

        /// <summary>
        ///     Tries to get the thermal headroom through android.os.PowerManager.getThermalHeadroom().
        ///     Calling this method multiple times within a second will return the cached value even if the forecastSeconds is different.
        /// </summary>
        /// <param name="forecastSeconds">how many seconds in the future to forecast</param>
        /// <param name="result">the thermal headroom value</param>
        /// <param name="resultForecastSeconds">the forecast seconds of the result. It maybe different from requested value when the cached value is returned.</param>
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

        private static AndroidJavaClass GetUnityPlayerClass() => new("com.unity3d.player.UnityPlayer");

        private static AndroidJavaObject GetCurrentActivity(AndroidJavaClass unityPlayerClass) =>
            unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        private static AndroidJavaObject GetApplicationContext(AndroidJavaObject activity) =>
            activity.Call<AndroidJavaObject>("getApplicationContext");

        private static void OnThermalStatusChangedCallback(int status)
        {
            _mainThreadContext.Post(_ =>
            {
                LatestThermalStatus = status;
                
                // May be converted to an enum.
                OnThermalStatusChanged?.Invoke(status);
            }, null);
        }

        private static void OnBatteryTemperatureChangedCallback(int value)
        {
            _mainThreadContext.Post(_ =>
            {
                LatestBatteryTemperature = value;

                OnBatteryTemperatureChanged?.Invoke(value);
            }, null);
        }


        private sealed class AndroidPowerManager : IDisposable
        {
            [ThreadStatic] private static jvalue[] _tempSingleJValueArray;
            [ThreadStatic] private static object[] _tempSingleObjectArray;

            private static jvalue[] TempSingleJValueArray => _tempSingleJValueArray ??= new jvalue[1];
            private static object[] TempSingleObjectArray => _tempSingleObjectArray ??= new object[1];

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

            public float GetThermalHeadroom(int forecastSeconds)
            {
                var getThermalHeadroomMethodId = _getThermalHeadroomMethodId ??=
                    AndroidJNIHelper.GetMethodID(_powerManager.GetRawClass(), "getThermalHeadroom", "(I)F");

                TempSingleJValueArray[0] = new jvalue()
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

            public void Dispose()
            {
                DisposeCore();
                GC.SuppressFinalize(this);
            }

            ~AndroidPowerManager()
            {
                DisposeCore();
            }
        }
    }
}

#endif
