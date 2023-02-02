// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_ANDROID

using System;
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
        
        /// <summary>
        ///     Event that is sent when the thermal status is changed.
        /// </summary>
        /// <returns>The raw value of android.os.PowerManager.THERMAL_STATUS_XXX (0~7).</returns>
        public static event Action<int> OnThermalStatusChanged;

        private static bool IsMonitoring;
        
        /// <summary>
        ///     Start thermal status monitoring.
        /// </summary>
        public static void StartMonitoring()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return;
#endif

            if (IsMonitoring) return;

            CallPowerManagerMethod("addThermalStatusListener", JavaCallbackListener);
            IsMonitoring = true;
        }

        /// <summary>
        ///     Stop thermal status monitoring.
        /// </summary>
        public static void StopMonitoring()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return;
#endif

            if (IsMonitoring == false) return;
            
            CallPowerManagerMethod("removeThermalStatusListener", JavaCallbackListener);
            IsMonitoring = false;
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
            // May be converted to an enum.
            OnThermalStatusChanged?.Invoke(status);
        }
    }
}

#endif
