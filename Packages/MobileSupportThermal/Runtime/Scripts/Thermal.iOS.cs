// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Threading;
using AOT;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace MobileSupport
{
    partial class Thermal
    {
        private const string DllName = "__Internal";

        private static bool _isMonitoring;
        private static BatteryStatus? _latestBatteryStatus;
        private static float? _latestBatteryLevel;

        private static SynchronizationContext _mainThread;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterUpdate()
        {
            _mainThread = SynchronizationContext.Current;

            using var modifier = PlayerLoopModifier.Create();
            modifier.InsertBefore<Update.ScriptRunBehaviourUpdate>(new PlayerLoopSystem
            {
                type = typeof(ThermalBatteryUpdate),
                updateDelegate = () =>
                {
                    if (!_isMonitoring) return;
                    if (OnBatteryStatusChangedInternal != null) GetLatestBatteryStatus();
                    if (OnBatteryLevelChangedInternal != null) GetLatestBatteryLevel();
                }
            });
        }

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void thermal_startMonitoring(CallBackDelegate callback);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void thermal_stopMonitoring();

        /// <summary>
        ///     The latest thermal status.
        ///     StartMonitoring() is required to get the latest value.
        /// </summary>
        public static event Action<ThermalStatusIOS> OnThermalStatusChanged;

        private static event Action<BatteryStatus> OnBatteryStatusChangedInternal;
        private static event Action<float> OnBatteryLevelChangedInternal;

        /// <summary>
        ///     The latest thermal status.
        ///     StartMonitoring() is required to get the latest value.
        /// </summary>
        public static ThermalStatusIOS? LatestThermalStatus { get; private set; }

        public static partial void StartMonitoring()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return;
#endif

            if (_isMonitoring) return;

            thermal_startMonitoring(OnThermalStatusChangedCallback);
            _isMonitoring = true;
        }

        public static partial void StopMonitoring()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return;
#endif

            if (_isMonitoring == false) return;

            thermal_stopMonitoring();
            _isMonitoring = false;
        }

        [MonoPInvokeCallback(typeof(CallBackDelegate))]
        private static void OnThermalStatusChangedCallback(int status)
        {
            LatestThermalStatus = (ThermalStatusIOS)status;

            // May be converted to an enum.
            OnThermalStatusChanged?.Invoke((ThermalStatusIOS)status);
        }

        private static partial BatteryStatus? GetLatestBatteryStatus()
        {
            if (_mainThread == null || _mainThread != SynchronizationContext.Current)
                throw new InvalidOperationException("This method must be called from the main thread.");

            if (!_isMonitoring) return _latestBatteryStatus;

            var status = SystemInfo.batteryStatus;

            if (_latestBatteryStatus != status)
            {
                _latestBatteryStatus = status;
                OnBatteryStatusChangedInternal?.Invoke(status);
            }

            return status;
        }

        private static partial float? GetLatestBatteryLevel()
        {
            if (_mainThread == null || _mainThread != SynchronizationContext.Current)
                throw new InvalidOperationException("This method must be called from the main thread.");

            if (!_isMonitoring) return _latestBatteryLevel;

            var level = SystemInfo.batteryLevel;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_latestBatteryLevel != level)
            {
                _latestBatteryLevel = level;
                OnBatteryLevelChangedInternal?.Invoke(level);
            }

            return level;
        }

        private static partial void AddOnBatteryStatusChangedListener(Action<BatteryStatus> listener)
        {
            OnBatteryStatusChangedInternal += listener;
        }

        private static partial void RemoveOnBatteryStatusChangedListener(Action<BatteryStatus> listener)
        {
            OnBatteryStatusChangedInternal -= listener;
        }

        private static partial void AddOnBatteryLevelChangedListener(Action<float> listener)
        {
            OnBatteryLevelChangedInternal += listener;
        }

        private static partial void RemoveOnBatteryLevelChangedListener(Action<float> listener)
        {
            OnBatteryLevelChangedInternal -= listener;
        }

        #region Nested type: CallBackDelegate

        private delegate void CallBackDelegate(int status);

        #endregion

        #region Nested type: ThermalBatteryUpdate

        private struct ThermalBatteryUpdate
        {
        }

        #endregion
    }

    /// <summary>
    ///     Equivalent to
    ///     <see href="https://developer.apple.com/documentation/foundation/nsprocessinfothermalstate">NSProcessInfoThermalState</see>.
    /// </summary>
    public enum ThermalStatusIOS
    {
        /// <summary>
        ///     The thermal state is within normal limits.
        /// </summary>
        Nominal,

        /// <summary>
        ///     The thermal state is slightly elevated.
        /// </summary>
        Fair,

        /// <summary>
        ///     The thermal state is high.
        /// </summary>
        Serious,

        /// <summary>
        ///     The thermal state is significantly impacting the performance of the system and the device needs to cool down.
        /// </summary>
        Critical
    }
}

#endif
