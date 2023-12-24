// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;

namespace MobileSupport.QualityMapper
{
    /// <summary>
    ///     Hardware stats to identify performance index of device
    /// </summary>
    public class HardwareStats
    {
        /// <summary>
        ///     Name of device model
        ///     <seealso cref="UnityEngine.SystemInfo.deviceModel" />
        /// </summary>
        public string DeviceModel { get; internal set; }

        /// <summary>
        ///     Memory size of GPU in MegaBytes
        ///     <remarks>Return zero on editor</remarks>
        /// </summary>
        public int GpuMemorySizeMb { get; internal set; }

        /// <summary>
        ///     Name of GPU
        ///     <seealso cref="UnityEngine.SystemInfo.graphicsDeviceName" />
        /// </summary>
        public string GpuName { get; internal set; }

        /// <summary>
        ///     Major series of GPU
        /// </summary>
        public GpuMajorSeries GpuMajorSeries { get; internal set; }

        /// <summary>
        ///     Major series of GPU
        /// </summary>
        public GpuMinorSeries GpuMinorSeries { get; internal set; }

        /// <summary>
        ///     Series number of GPU.
        ///     ex: Adreno 650 -> 650
        /// </summary>
        public int GpuSeriesNumber { get; internal set; }

        /// <summary>
        ///     Name of SoC
        ///     <remarks>Only available on Android12 or later</remarks>
        /// </summary>
        public string SocName { get; internal set; }

        /// <summary>
        ///     Memory size of system in MegaBytes
        ///     <remarks>Return zero on editor</remarks>
        /// </summary>
        public int SystemMemorySizeMb { get; internal set; }

        internal static HardwareStats CreateDefault()
        {
            return new HardwareStats
            {
                DeviceModel = SystemInfo.deviceModel,
                GpuName = SystemInfo.graphicsDeviceName,
                GpuMemorySizeMb = SystemInfo.graphicsMemorySize,
                GpuMajorSeries = GpuMajorSeries.Unknown,
                GpuMinorSeries = GpuMinorSeries.Unknown,
                GpuSeriesNumber = 0,
                SocName = "",
                SystemMemorySizeMb = SystemInfo.systemMemorySize
            };
        }
    }

    public static class HardwareInfo
    {
        // singleton cache
        private static HardwareStats _cachedHardwareStats;

        /// <summary>
        ///     Get hardware stats of running device
        /// </summary>
        /// <returns>HardwareStats</returns>
        public static HardwareStats GetHardwareStats()
        {
            if (_cachedHardwareStats != null)
                return _cachedHardwareStats;


            _cachedHardwareStats = HardwareStats.CreateDefault();


#if UNITY_EDITOR_OSX || UNITY_IOS
            HardwareInfoIos.SetIosHardwareStats(_cachedHardwareStats);
#elif UNITY_ANDROID
            HardwareInfoAndroid.SetAndroidHardwareStats(_cachedHardwareStats);
#endif
            return _cachedHardwareStats;
        }
    }
}
