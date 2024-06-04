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
                        return new AndroidJavaObject("java.lang.Boolean",equals((AndroidJavaObject)args[0]));
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

        public static float GetThermalHeadroom(int forecastSeconds)
        {
#if UNITY_EDITOR
            if (Application.isEditor) return float.NaN;
#endif

            using var playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            using var staticContext = new AndroidJavaClass("android.content.Context");
            using var service = staticContext.GetStatic<AndroidJavaObject>("POWER_SERVICE");
            using var powerManager = activity.Call<AndroidJavaObject>("getSystemService", service);

            return powerManager.Call<float>("getThermalHeadroom", forecastSeconds);
        }

        private static void StartReceiveBatteryTemperature()
        {
            using var playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = activity.Call<AndroidJavaObject>("getApplicationContext");

            BatteryChangedBroadcastReceiverInstance.Call("registerToContext", context);
            BatteryChangedBroadcastReceiverInstance.Call("addReceiver", BatteryTemperatureReceiverInstance);
        }

        private static void StopReceiveBatteryTemperature()
        {
            using var playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            using var context = activity.Call<AndroidJavaObject>("getApplicationContext");

            BatteryChangedBroadcastReceiverInstance.Call("removeReceiver", BatteryTemperatureReceiverInstance);
            BatteryChangedBroadcastReceiverInstance.Call("unregisterFromContext", context);
        }

        private static void CallPowerManagerMethod(string methodName, params object[] args)
        {
            using var playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            using var staticContext = new AndroidJavaClass("android.content.Context");
            using var service = staticContext.GetStatic<AndroidJavaObject>("POWER_SERVICE");
            using var powerManager = activity.Call<AndroidJavaObject>("getSystemService", service);
            
            powerManager.Call(methodName, args);
        }

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
