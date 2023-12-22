// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using UnityEngine;

namespace MobileSupport.PerformanceIndex
{
    [Serializable]
    public abstract class RuleMatcher
    {
        public abstract bool Match(HardwareStats stats, ref int qualityLevel);

        public bool Match<T>(HardwareStats stats, ref T qualityLevel)
        {
            var matchedQualityLevel = 0;
            var result = Match(stats, ref matchedQualityLevel);
            if (result)
            {
                if (typeof(T).IsEnum)
                    qualityLevel = (T)Enum.ToObject(typeof(T), matchedQualityLevel);
                else
                    qualityLevel = (T)Convert.ChangeType(matchedQualityLevel, typeof(T));
            }

            return result;
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

        public override bool Match(HardwareStats stats, ref int qualityLevel)
        {
            foreach (var rule in rules)
                if (rule.deviceModel.Equals(stats.DeviceModel, StringComparison.Ordinal))
                {
                    qualityLevel = ConvertToInt(rule.qualityLevel);
                    return true;
                }

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
    public class GpuRuleMatcher<T> : RuleMatcher
    {
        [Tooltip("GPU series that match")]
        public GpuSeriesEnumeration gpuSeries;

        public Rule[] rule;

        public override bool Match(HardwareStats stats, ref int qualityLevel)
        {
            if (stats.GpuMajorSeries != gpuSeries.GpuMajorSeries)
                return false;
            // don't check gpuMinorSeries if it's unknown
            if (gpuSeries.GpuMinorSeries != GpuMinorSeries.Unknown && stats.GpuMinorSeries != gpuSeries.GpuMinorSeries)
                return false;

            foreach (var range in rule)
            {
                if (stats.GpuSeriesNumber < range.gpuSeriesNumberMin ||
                    stats.GpuSeriesNumber > range.gpuSeriesNumberMax)
                    continue;

                qualityLevel = ConvertToInt(range.qualityLevel);
                return true;
            }

            return false;
        }

        [Serializable]
        public sealed class Rule
        {
            [Tooltip("Minimum of GPU series number that match (inclusive)")]
            public int gpuSeriesNumberMin;

            [Tooltip("Maximum of GPU series number that match (inclusive)")]
            public int gpuSeriesNumberMax;

            [Tooltip("Performance level for the device that match")]
            public T qualityLevel;
        }
    }

    [Serializable]
    public class SystemMemoryRuleMatcher<T> : RuleMatcher
    {
        public Rule[] rules;

        public override bool Match(HardwareStats stats, ref int qualityLevel)
        {
            foreach (var range in rules)
            {
                if (stats.SystemMemorySizeMb < range.systemMemoryMin ||
                    stats.SystemMemorySizeMb > range.systemMemoryMax)
                    continue;

                qualityLevel = ConvertToInt(range.qualityLevel);
                return true;
            }

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

    public abstract class QualityLevelSelector<T> : ScriptableObject
    {
        [Tooltip("Default quality level if no match")]
        public T defaultQualityLevel;

        [Tooltip("Combined performance index table")]
        [SerializeReference]
        [SelectableSerializeReference]
        public RuleMatcher[] qualityLevelRuleMatchers;

        public bool GetQualityLevel(HardwareStats stats, out T qualityLevel)
        {
#if UNITY_IOS || UNITY_ANDROID
            T matchedQualityLevel = default;
            foreach (var matcher in qualityLevelRuleMatchers)
            {
                if (matcher == null)
                    continue;

                if (matcher.Match(stats, ref matchedQualityLevel))
                {
                    qualityLevel = matchedQualityLevel;
                    return true;
                }
            }

            qualityLevel = defaultQualityLevel;
            return false;
#else
            return false;
#endif
        }
    }
}
