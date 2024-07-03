// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_IOS || UNITY_ANDROID

using System;
using UnityEngine;

namespace MobileSupport
{
    public static partial class Thermal
    {
        /// <summary>
        ///     The latest thermal status.
        ///     StartMonitoring() is required to get the latest value.
        /// </summary>
        public static ThermalStatus? LatestThermalStatus => GetLatestThermalStatus();

        /// <summary>
        ///     The latest battery status.
        ///     StartMonitoring() is required to get the latest value.
        /// </summary>
        public static BatteryStatus? LatestBatteryStatus => GetLatestBatteryStatus();

        /// <summary>
        ///     The latest battery level [0.0, 1.0].
        ///     StartMonitoring() is required to get the latest value.
        /// </summary>
        public static float? LatestBatteryLevel => GetLatestBatteryLevel();

        /// <summary>
        ///     Event that is sent when the thermal status is changed.
        ///     StartMonitoring() is required.
        /// </summary>
        /// <returns>The platform-dependent thermal status value.</returns>
        public static event Action<ThermalStatus> OnThermalStatusChanged
        {
            add => AddOnThermalStatusChangedListener(value);
            remove => RemoveOnThermalStatusChangedListener(value);
        }

        /// <summary>
        ///     Event that is sent when the battery status is changed.
        ///     StartMonitoring() is required.
        /// </summary>
        /// <returns>The battery status.</returns>
        public static event Action<BatteryStatus> OnBatteryStatusChanged
        {
            add => AddOnBatteryStatusChangedListener(value);
            remove => RemoveOnBatteryStatusChangedListener(value);
        }

        /// <summary>
        ///     Event that is sent when the battery level is changed.
        ///     StartMonitoring() is required.
        /// </summary>
        /// <returns>The battery temperature [0.0, 1.0].</returns>
        public static event Action<float> OnBatteryLevelChanged
        {
            add => AddOnBatteryLevelChangedListener(value);
            remove => RemoveOnBatteryLevelChangedListener(value);
        }

        /// <summary>
        ///     Start monitoring.
        /// </summary>
        public static partial void StartMonitoring();

        /// <summary>
        ///     Stop monitoring.
        /// </summary>
        public static partial void StopMonitoring();

        private static partial ThermalStatus? GetLatestThermalStatus();
        private static partial BatteryStatus? GetLatestBatteryStatus();
        private static partial float? GetLatestBatteryLevel();

        private static partial void AddOnThermalStatusChangedListener(Action<ThermalStatus> listener);
        private static partial void RemoveOnThermalStatusChangedListener(Action<ThermalStatus> listener);
        
        private static partial void AddOnBatteryStatusChangedListener(Action<BatteryStatus> listener);
        private static partial void RemoveOnBatteryStatusChangedListener(Action<BatteryStatus> listener);
        
        private static partial void AddOnBatteryLevelChangedListener(Action<float> listener);
        private static partial void RemoveOnBatteryLevelChangedListener(Action<float> listener);
    }
}

#endif
