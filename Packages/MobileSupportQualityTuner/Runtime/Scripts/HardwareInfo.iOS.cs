// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Text.RegularExpressions;

namespace MobileSupport.QualityTuner
{
    internal static class HardwareInfoIos
    {
        public static void SetIosHardwareStats(HardwareStats stats)
        {
            // only works with iOS, iPadOS and macOS, including editor
#if UNITY_EDITOR && !UNITY_EDITOR_OSX
            if (UnityEngine.Application.isEditor)
                return;
#endif
            stats.GpuMajorSeries = GpuMajorSeries.Apple;
            stats.GpuMinorSeries = ParseGpuMinorSeries(stats.GpuName);
            stats.GpuSeriesNumber = ParseAppleGpuSeriesNumber(stats.GpuName);
        }

        public static GpuMinorSeries ParseGpuMinorSeries(string gpuName)
        {
            // parse GPU series by StartsWith
            return gpuName switch
            {
                { } when gpuName.StartsWith("Apple A", StringComparison.Ordinal) => GpuMinorSeries.AppleA,
                { } when gpuName.StartsWith("Apple M", StringComparison.Ordinal) => GpuMinorSeries.AppleM,
                _ => GpuMinorSeries.Unknown
            };
        }

        public static int ParseAppleGpuSeriesNumber(string gpuName)
        {
            // parse Apple GPU series number by regex
            // ex: Apple A8 GPU, Apple A10 GPU, Apple M1
            var match = Regex.Match(gpuName, @"Apple [AM](\d+)");
            if (match.Success)
                if (int.TryParse(match.Groups[1].Value, out var number))
                    return number;

            return 0;
        }
    }
}
