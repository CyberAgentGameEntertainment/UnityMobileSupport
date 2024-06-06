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
                "jp.co.cyberagent.unitysupport.BatteryTemperatureReceiver")
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
            new("jp.co.cyberagent.unitysupport.BatteryChangedBroadcastReceiver");

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
            
            CallPowerManagerMethod("addThermalStatusListener", JavaCallbackListener);

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
            
            CallPowerManagerMethod("removeThermalStatusListener", JavaCallbackListener);

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
                resultForecastSeconds = 0;
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

                using var playerClass = GetUnityPlayerClass();
                using var activity = GetCurrentActivity(playerClass);
                using var staticContext = GetContextClass();
                using var service = GetPowerService(staticContext);
                using var powerManager = GetSystemService(activity, service);

                result = powerManager.Call<float>("getThermalHeadroom", forecastSeconds);

                _latestThermalHeadroomTime = DateTime.UtcNow;
                _latestThermalHeadroom = result;
                resultForecastSeconds = _latestThermalHeadroomForecastSeconds = forecastSeconds;
                isLatestValue = true;

                return;
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

        private static void CallPowerManagerMethod(string methodName, params object[] args)
        {
            using var playerClass = GetUnityPlayerClass();
            using var activity = GetCurrentActivity(playerClass);
            using var staticContext = GetContextClass();
            using var service = GetPowerService(staticContext);
            using var powerManager = GetSystemService(activity, service);

            powerManager.Call(methodName, args);
        }

        private static AndroidJavaClass GetUnityPlayerClass() => new("com.unity3d.player.UnityPlayer");

        private static AndroidJavaObject GetCurrentActivity(AndroidJavaClass unityPlayerClass) =>
            unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        private static AndroidJavaClass GetContextClass() => new("android.content.Context");

        private static AndroidJavaObject GetPowerService(AndroidJavaClass contextClass) =>
            contextClass.GetStatic<AndroidJavaObject>("POWER_SERVICE");

        private static AndroidJavaObject GetSystemService(AndroidJavaObject activity, AndroidJavaObject service) =>
            activity.Call<AndroidJavaObject>("getSystemService", service);

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
    }
}

#endif
