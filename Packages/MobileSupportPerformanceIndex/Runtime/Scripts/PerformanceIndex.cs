using System;
using UnityEngine;

namespace MobileSupport.PerformanceIndex
{
    [Serializable]
    public sealed class DevicePerformanceIndex<T>
    {
        public string deviceModel;

        public T performanceLevel;

        public bool Match(string deviceModel)
        {
            return this.deviceModel.Equals(deviceModel, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Serializable]
    public sealed class GpuPerformanceIndex<T>
    {
        public GpuMajorSeries gpuMajorSeries;
        public GpuMinorSeries gpuMinorSeries;
        public int gpuSeriesNumberMin;
        public int gpuSeriesNumberMax;

        public T performanceLevel;

        public bool Match(GpuMajorSeries gpuMajorSeries, GpuMinorSeries gpuMinorSeries, int gpuSeriesNumber)
        {
            if (gpuMajorSeries != this.gpuMajorSeries)
                return false;
            // don't check gpuMinorSeries if it's unknown
            if (this.gpuMinorSeries != GpuMinorSeries.Unknown && gpuMinorSeries != this.gpuMinorSeries)
                return false;
            if (gpuSeriesNumber < gpuSeriesNumberMin || gpuSeriesNumber > gpuSeriesNumberMax)
                return false;
            return true;
        }
    }

    public abstract class PerformanceIndexData<T> : ScriptableObject
    {
        public DevicePerformanceIndex<T>[] devicePerformanceIndexIndices;
        public GpuPerformanceIndex<T>[] gpuPerformanceIndices;

        public bool GetPerformanceLevel(HardwareStats stats, ref T performanceLevel)
        {
            // search device name first
            if (devicePerformanceIndexIndices != null)
                foreach (var devicePerformanceIndex in devicePerformanceIndexIndices)
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
}
