// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_IOS

using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace MobileSupport
{
    public static class Thermal
    {
        private const string DllName = "__Internal";
        
        private delegate void CallBackDelegate(int status);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void thermal_startMonitoring(CallBackDelegate callback);
        
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void thermal_stopMonitoring();

        /// <summary>
        ///     Event that is sent when the thermal status is changed.
        /// </summary>
        /// <returns>The raw value of android.os.PowerManager.THERMAL_STATUS_XXX (0~3).</returns>
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

            thermal_startMonitoring(OnThermalStatusChangedCallback);
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

            thermal_stopMonitoring();
            IsMonitoring = false;
        }
        
        [MonoPInvokeCallback(typeof(CallBackDelegate))]
        private static void OnThermalStatusChangedCallback(int status)
        {
            // May be converted to an enum.
            OnThermalStatusChanged?.Invoke(status);
        }
    }
}

#endif
