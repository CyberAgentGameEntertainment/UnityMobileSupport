// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;

namespace MobileSupport.PerformanceIndex
{
    [Serializable]
    public sealed class DevicePerformanceIndex<T>
    {
        [Tooltip("Device model name returned by SystemInfo.deviceModel (exact match)")]
        public string deviceModel;

        [Tooltip("Performance level for the device that match")]
        public T performanceLevel;

        public bool Match(string deviceModel)
        {
            return this.deviceModel.Equals(deviceModel, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Serializable]
    public sealed class GpuPerformanceIndex<T>
    {
        [Tooltip("GPU series that match")]
        public GpuSeriesEnumeration gpuSeries;

        [Tooltip("Minimum of GPU series number that match (inclusive)")]
        public int gpuSeriesNumberMin;

        [Tooltip("Mamimum of GPU series number that match (inclusive)")]
        public int gpuSeriesNumberMax;

        [Tooltip("Performance level for the device that match")]
        public T performanceLevel;

        public bool Match(GpuMajorSeries gpuMajorSeries, GpuMinorSeries gpuMinorSeries, int gpuSeriesNumber)
        {
            if (gpuMajorSeries != gpuSeries.GpuMajorSeries)
                return false;
            // don't check gpuMinorSeries if it's unknown
            if (gpuSeries.GpuMinorSeries != GpuMinorSeries.Unknown && gpuMinorSeries != gpuSeries.GpuMinorSeries)
                return false;
            if (gpuSeriesNumber < gpuSeriesNumberMin || gpuSeriesNumber > gpuSeriesNumberMax)
                return false;
            return true;
        }
    }

    [Serializable]
    public sealed class CombinedPerformanceIndexData<T>
    {
        [Tooltip("Performance index table by device model name")]
        public DevicePerformanceIndex<T>[] devicePerformanceIndices;

        [Tooltip("Performance index table by GPU series")]
        public GpuPerformanceIndex<T>[] gpuPerformanceIndices;

        public bool GetPerformanceLevel(HardwareStats stats, ref T performanceLevel)
        {
            // search device name first
            if (devicePerformanceIndices != null)
                foreach (var devicePerformanceIndex in devicePerformanceIndices)
                    if (devicePerformanceIndex.Match(stats.DeviceModel))
                    {
                        performanceLevel = devicePerformanceIndex.performanceLevel;
                        return true;
                    }

            // search qpu series second
            if (gpuPerformanceIndices != null)
                foreach (var gpuPerformanceIndex in gpuPerformanceIndices)
                    if (gpuPerformanceIndex.Match(stats.GpuMajorSeries, stats.GpuMinorSeries, stats.GpuSeriesNumber))
                    {
                        performanceLevel = gpuPerformanceIndex.performanceLevel;
                        return true;
                    }

            return false;
        }
    }

    public abstract class PerformanceIndexData<T> : ScriptableObject
    {
        [Tooltip("Combined performance index table")]
        public CombinedPerformanceIndexData<T> performanceIndexData;

        public bool GetPerformanceLevel(HardwareStats stats, ref T performanceLevel)
        {
#if UNITY_IOS || UNITY_ANDROID
            return performanceIndexData.GetPerformanceLevel(stats, ref performanceLevel);
#else
            return false;
#endif
        }
    }
}
