// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MobileSupport.PerformanceIndex
{
    [Serializable]
    public abstract class RuleMatcher
    {
        public abstract bool Match(HardwareStats stats, out int qualityLevel);

        public bool Match<T>(HardwareStats stats, out T qualityLevel)
        {
            qualityLevel = default;
            if (Match(stats, out var matchedQualityLevel))
            {
                if (typeof(T).IsEnum)
                    qualityLevel = (T)Enum.ToObject(typeof(T), matchedQualityLevel);
                else
                    qualityLevel = (T)Convert.ChangeType(matchedQualityLevel, typeof(T));
                return true;
            }

            return false;
        }

        protected static int ConvertToInt(object value)
        {
            switch (value)
            {
                case int intValue:
                    return intValue;
                default:
                    return Convert.ToInt32(value);
            }
        }
    }

    [Serializable]
    public class DeviceRuleMatcher<T> : RuleMatcher
    {
        public Rule[] rules;

        public override bool Match(HardwareStats stats, out int qualityLevel)
        {
            foreach (var rule in rules)
                if (rule.deviceModel.Equals(stats.DeviceModel, StringComparison.Ordinal))
                {
                    qualityLevel = ConvertToInt(rule.qualityLevel);
                    return true;
                }

            qualityLevel = default;
            return false;
        }

        [Serializable]
        public class Rule
        {
            [Tooltip("Device model name returned by SystemInfo.deviceModel (exact match)")]
            public string deviceModel;

            [Tooltip("Quality level for the device that match")]
            public T qualityLevel;
        }
    }

    [Serializable]
    public class GpuRuleMatcher<TLevel, TEnum> : RuleMatcher where TEnum : GpuSeriesEnumeration
    {
        [FormerlySerializedAs("rule")]
        public Rule[] rules;

        public override bool Match(HardwareStats stats, out int qualityLevel)
        {
            qualityLevel = default;

            foreach (var rule in rules)
            {
                if (stats.GpuMajorSeries != rule.gpuSeries.GpuMajorSeries)
                    return false;
                // don't check gpuMinorSeries if it's unknown
                if (rule.gpuSeries.GpuMinorSeries != GpuMinorSeries.Unknown &&
                    stats.GpuMinorSeries != rule.gpuSeries.GpuMinorSeries)
                    return false;

                if (stats.GpuSeriesNumber < rule.gpuSeriesNumberMin ||
                    stats.GpuSeriesNumber > rule.gpuSeriesNumberMax)
                    continue;

                qualityLevel = ConvertToInt(rule.qualityLevel);
                return true;
            }

            return false;
        }

        [Serializable]
        public sealed class Rule
        {
            [Tooltip("GPU series that match")]
            public TEnum gpuSeries;

            [Tooltip("Minimum of GPU series number that match (inclusive)")]
            public int gpuSeriesNumberMin;

            [Tooltip("Maximum of GPU series number that match (inclusive)")]
            public int gpuSeriesNumberMax;

            [Tooltip("Performance level for the device that match")]
            public TLevel qualityLevel;
        }
    }

    [Serializable]
    public class SystemMemoryRuleMatcher<T> : RuleMatcher
    {
        public Rule[] rules;

        public override bool Match(HardwareStats stats, out int qualityLevel)
        {
            foreach (var rule in rules)
            {
                if (stats.SystemMemorySizeMb < rule.systemMemoryMin ||
                    stats.SystemMemorySizeMb > rule.systemMemoryMax)
                    continue;

                qualityLevel = ConvertToInt(rule.qualityLevel);
                return true;
            }

            qualityLevel = default;
            return false;
        }

        [Serializable]
        public sealed class Rule
        {
            [Tooltip("Minimum of system memory megabytes that match (inclusive)")]
            public int systemMemoryMin;

            [Tooltip("Maximum of system memory megabytes that match (inclusive)")]
            public int systemMemoryMax;

            [Tooltip("Performance level for the device that match")]
            public T qualityLevel;
        }
    }
}
