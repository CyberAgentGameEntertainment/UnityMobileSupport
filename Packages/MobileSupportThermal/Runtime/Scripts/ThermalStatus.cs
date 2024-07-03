// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;

namespace MobileSupport
{
    /// <summary>
    ///     Platform-specific thermal status.
    /// </summary>
    public readonly struct ThermalStatus
    {
        public bool IsIOS => !IsAndroid;
        public bool IsAndroid { get; }

        private readonly int _status;

        public ThermalStatus(ThermalStatusAndroid status)
        {
            IsAndroid = true;
            _status = (int)status;
        }

        public ThermalStatus(ThermalStatusIOS status)
        {
            IsAndroid = false;
            _status = (int)status;
        }

        public static implicit operator ThermalStatus(ThermalStatusAndroid status)
        {
            return new ThermalStatus(status);
        }

        public static implicit operator ThermalStatus(ThermalStatusIOS status)
        {
            return new ThermalStatus(status);
        }

        public ThermalStatusAndroid UnwrapAsAndroid()
        {
            if (!IsAndroid) throw new InvalidOperationException("This status is not for Android.");
            return (ThermalStatusAndroid)_status;
        }

        public ThermalStatusIOS UnwrapAsIOS()
        {
            if (IsAndroid) throw new InvalidOperationException("This status is not for iOS.");
            return (ThermalStatusIOS)_status;
        }
    }

    /// <summary>
    ///     Equivalent to <see href="https://developer.android.com/reference/android/os/PowerManager">PowerManager</see>'s
    ///     THERMAL_STATUS_* values.
    /// </summary>
    public enum ThermalStatusAndroid
    {
        /// <summary>
        ///     Not under throttling.
        /// </summary>
        None,

        /// <summary>
        ///     Light throttling where UX is not impacted.
        /// </summary>
        Light,

        /// <summary>
        ///     Moderate throttling where UX is not largely impacted.
        /// </summary>
        Moderate,

        /// <summary>
        ///     Severe throttling where UX is largely impacted.
        /// </summary>
        Severe,

        /// <summary>
        ///     Platform has done everything to reduce power.
        /// </summary>
        Critical,

        /// <summary>
        ///     Key components in platform are shutting down due to thermal condition. Device functionalities will be limited.
        /// </summary>
        Emergency,

        /// <summary>
        ///     Need shutdown immediately.
        /// </summary>
        Shutdown
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
