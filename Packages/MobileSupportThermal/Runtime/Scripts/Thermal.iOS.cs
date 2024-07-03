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
        private static ThermalStatus? _latestThermalStatus;
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

        private static event Action<ThermalStatus> OnThermalStatusChangedInternal;
        private static event Action<BatteryStatus> OnBatteryStatusChangedInternal;
        private static event Action<float> OnBatteryLevelChangedInternal;

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
            _latestThermalStatus = (ThermalStatusIOS)status;

            // May be converted to an enum.
            OnThermalStatusChangedInternal?.Invoke((ThermalStatusIOS)status);
        }

        private static partial ThermalStatus? GetLatestThermalStatus()
        {
            return _latestThermalStatus;
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

        private static partial void AddOnThermalStatusChangedListener(Action<ThermalStatus> listener)
        {
            OnThermalStatusChangedInternal += listener;
        }

        private static partial void RemoveOnThermalStatusChangedListener(Action<ThermalStatus> listener)
        {
            OnThermalStatusChangedInternal -= listener;
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
}

#endif
