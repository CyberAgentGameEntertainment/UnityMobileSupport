// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MobileSupport.QualityTuner
{
    public interface IMatcher
    {
        bool TryMatch<T>(HardwareStats stats, out T qualityLevel);
    }

    public interface IMatcher<T>
    {
        bool TryMatch(HardwareStats stats, out T matchedQualityLevel);
    }

    public abstract class RuleMatcherBase<T> : IMatcher
    {
        protected abstract IEnumerable<IMatcher<T>> Rules { get; }

        bool IMatcher.TryMatch<TOut>(HardwareStats stats, out TOut qualityLevel)
        {
            Assert.AreEqual(typeof(T), typeof(TOut), "type mismatch");

            var matched = TryMatch(stats, out var matchedQualityLevel);
            qualityLevel = (TOut)Convert.ChangeType(matchedQualityLevel, typeof(TOut));
            return matched;
        }

        public bool TryMatch(HardwareStats stats, out T qualityLevel)
        {
            if (Rules is not null)
                foreach (var rule in Rules)
                    if (rule.TryMatch(stats, out var matchedQualityLevel))
                    {
                        qualityLevel = matchedQualityLevel;
                        return true;
                    }

            qualityLevel = default;
            return false;
        }
    }

    [Serializable]
    public class DeviceNameRuleMatcherBase<T> : RuleMatcherBase<T>
    {
        public Rule[] rules;

        protected override IEnumerable<IMatcher<T>> Rules => rules;

        [Serializable]
        public class Rule : IMatcher<T>
        {
            [Tooltip("Device model name returned by SystemInfo.deviceModel (exact match)")]
            public string deviceModel;

            [Tooltip("Quality level for the device that match")]
            public T qualityLevel;

            public bool TryMatch(HardwareStats stats, out T matchedQualityLevel)
            {
                if (deviceModel.Equals(stats.DeviceModel, StringComparison.Ordinal))
                {
                    matchedQualityLevel = qualityLevel;
                    return true;
                }

                matchedQualityLevel = default;
                return false;
            }
        }
    }

    [Serializable]
    public class GpuSeriesRuleMatcherBase<TLevel, TEnum> : RuleMatcherBase<TLevel>
        where TEnum : GpuSeriesEnumeration
    {
        public Rule[] rules;

        protected override IEnumerable<IMatcher<TLevel>> Rules => rules;

        [Serializable]
        public sealed class Rule : IMatcher<TLevel>
        {
            [Tooltip("GPU series that match")]
            public TEnum gpuSeries;

            [Tooltip("Minimum of GPU series number that match (inclusive)")]
            public int gpuSeriesNumberMin;

            [Tooltip("Maximum of GPU series number that match (inclusive)")]
            public int gpuSeriesNumberMax;

            [Tooltip("Performance level for the device that match")]
            public TLevel qualityLevel;

            public bool TryMatch(HardwareStats stats, out TLevel matchedQualityLevel)
            {
                matchedQualityLevel = default;
                if (stats.GpuMajorSeries != gpuSeries.GpuMajorSeries)
                    return false;
                // don't check gpuMinorSeries if it's unknown
                if (gpuSeries.GpuMinorSeries != GpuMinorSeries.Unknown &&
                    stats.GpuMinorSeries != gpuSeries.GpuMinorSeries)
                    return false;

                if (stats.GpuSeriesNumber < gpuSeriesNumberMin ||
                    stats.GpuSeriesNumber > gpuSeriesNumberMax)
                    return false;

                matchedQualityLevel = qualityLevel;
                return true;
            }
        }
    }

    [Serializable]
    public class SystemMemoryRuleMatcherBase<T> : RuleMatcherBase<T>
    {
        public Rule[] rules;

        protected override IEnumerable<IMatcher<T>> Rules => rules;

        [Serializable]
        public sealed class Rule : IMatcher<T>
        {
            [Tooltip("Minimum of system memory megabytes that match (inclusive)")]
            public int systemMemoryMin;

            [Tooltip("Maximum of system memory megabytes that match (inclusive)")]
            public int systemMemoryMax;

            [Tooltip("Performance level for the device that match")]
            public T qualityLevel;

            public bool TryMatch(HardwareStats stats, out T matchedQualityLevel)
            {
                matchedQualityLevel = default;
                if (stats.SystemMemorySizeMb < systemMemoryMin ||
                    stats.SystemMemorySizeMb > systemMemoryMax)
                    return false;

                matchedQualityLevel = qualityLevel;
                return true;
            }
        }
    }
}
