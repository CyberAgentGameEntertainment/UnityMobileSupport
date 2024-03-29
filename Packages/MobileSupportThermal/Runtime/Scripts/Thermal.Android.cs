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

        private static readonly ThermalStatusListener JavaCallbackListener = new ThermalStatusListener(OnThermalStatusChangedCallback);

        private static SynchronizationContext _mainThreadContext;
        
        /// <summary>
        ///     Event that is sent when the thermal status is changed.
        /// </summary>
        /// <returns>The raw value of android.os.PowerManager.THERMAL_STATUS_XXX (0~7).</returns>
        public static event Action<int> OnThermalStatusChanged;

        /// <summary>
        ///     The latest thermal status.
        /// </summary>
        public static int? LatestThermalStatus { get; private set; }

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
            _isMonitoring = false;
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
    }
}

#endif
