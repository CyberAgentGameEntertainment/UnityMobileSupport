using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MobileSupport
{
    internal static class HardwareInfoAndroid
    {
        // Build.VERSION_CODES.S (Android 12)
        private const int ANDROID_VERSION_CODES_S = 31;

        private static int _sdkVersion;

        public static void SetAndroidHardwareStats(HardwareStats stats)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                return;
#endif
            stats.GpuSeries = ParseGpuSeries(stats.GpuName);
            stats.GpuSeriesNumber = stats.GpuSeries switch
            {
                GpuSeries.Adreno => ParseAdrenoGpuSeriesNumber(stats.GpuName),
                GpuSeries.Mali or GpuSeries.MaliG or GpuSeries.MaliT => ParseMaliGpuSeriesNumber(stats.GpuName),
                _ => 0
            };
            stats.SocName = GetSocName();
        }

        public static GpuSeries ParseGpuSeries(string gpuName)
        {
            // parse GPU series by StartsWith
            return gpuName switch
            {
                { } when gpuName.StartsWith("Adreno") => GpuSeries.Adreno,
                { } when gpuName.StartsWith("Mali-G") => GpuSeries.MaliG,
                { } when gpuName.StartsWith("Mali-T") => GpuSeries.MaliT,
                { } when gpuName.StartsWith("Mali-") => GpuSeries.Mali,
                { } when gpuName.StartsWith("PowerVR") => GpuSeries.PowerVR,
                _ => GpuSeries.Unknown
            };
        }

        public static int ParseAdrenoGpuSeriesNumber(string gpuName)
        {
            // parse Adreno GPU series number by regex
            // Adreno (TM) xxx
            var match = Regex.Match(gpuName, @"Adreno \(TM\) (\d+)");
            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return 0;
        }

        public static int ParseMaliGpuSeriesNumber(string gpuName)
        {
            // parse Mali GPU series number by regex
            // Mali-Gxx, Mali-Txxx
            var match = Regex.Match(gpuName, @"Mali-[GT]*(\d+)");
            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return 0;
        }

        public static int ParsePowerVRGpuSeriesNumber(string gpuName)
        {
            // parse PowerVR GPU series number by regex
            // PowerVR Rogue GExxxx, PowerVR Rogue G?xxxx
            var match = Regex.Match(gpuName, @"PowerVR Rogue G[A-Z](\d+)");
            if (match.Success)
                return int.Parse(match.Groups[1].Value);

            return 0;
        }

        public static string GetSocName()
        {
            if (GetAndroidSdkVersion() < ANDROID_VERSION_CODES_S)
                return "";

            try
            {
                using var buildClass = new AndroidJavaClass("android.os.Build");
                var socModel = buildClass.GetStatic<string>("SOC_MODEL");
                return socModel;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return "";
            }
        }

        private static int GetAndroidSdkVersion()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                return 0;
#endif
#if UNITY_ANDROID
            if (_sdkVersion != 0)
                return _sdkVersion;

            using var versionCls = new AndroidJavaClass("android.os.Build$VERSION");
            _sdkVersion = versionCls.GetStatic<int>("SDK_INT");
            return _sdkVersion;
#else
            return 0;
#endif
        }
    }
}
