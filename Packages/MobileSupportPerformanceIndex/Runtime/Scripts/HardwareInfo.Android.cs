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
            var gpuName = stats.GpuName;
            stats.GpuMajorSeries = ParseGpuMajorSeries(gpuName);
            (stats.GpuMinorSeries, stats.GpuSeriesNumber) = stats.GpuMajorSeries switch
            {
                GpuMajorSeries.Adreno => ParseAdrenoGpuSeries(gpuName),
                GpuMajorSeries.Mali => ParseMaliGpuSeries(gpuName),
                GpuMajorSeries.PowerVR => ParsePowerVRGpuSeries(gpuName),
                GpuMajorSeries.Xclipse => ParseXclipseGpuSeries(gpuName),
                GpuMajorSeries.Maleoon => ParseMaleoonGpuSeries(gpuName),
                _ => (GpuMinorSeries.Unknown, 0)
            };
            stats.SocName = GetSocName();
        }

        public static GpuMajorSeries ParseGpuMajorSeries(string gpuName)
        {
            // parse GPU series by StartsWith
            return gpuName switch
            {
                { } when gpuName.StartsWith("Adreno", StringComparison.Ordinal) => GpuMajorSeries.Adreno,
                { } when gpuName.StartsWith("Mali", StringComparison.Ordinal) => GpuMajorSeries.Mali,
                { } when gpuName.StartsWith("PowerVR", StringComparison.Ordinal) => GpuMajorSeries.PowerVR,
                { } when gpuName.StartsWith("Samsung Xclipse", StringComparison.Ordinal) => GpuMajorSeries.Xclipse,
                { } when gpuName.StartsWith("Maleoon", StringComparison.Ordinal) => GpuMajorSeries.Maleoon,
                _ => GpuMajorSeries.Unknown
            };
        }

        public static (GpuMinorSeries, int) ParseAdrenoGpuSeries(string gpuName)
        {
            // parse Adreno GPU series number by regex
            // ex: Adreno (TM) xxx
            var match = Regex.Match(gpuName, @"Adreno \(TM\) (\d+)");
            if (match.Success)
            {
                var number = int.Parse(match.Groups[1].Value);
                switch (number / 100)
                {
                    case 1:
                        return (GpuMinorSeries.Adreno100, number);
                    case 2:
                        return (GpuMinorSeries.Adreno200, number);
                    case 3:
                        return (GpuMinorSeries.Adreno300, number);
                    case 4:
                        return (GpuMinorSeries.Adreno400, number);
                    case 5:
                        return (GpuMinorSeries.Adreno500, number);
                    case 6:
                        return (GpuMinorSeries.Adreno600, number);
                    case 7:
                        return (GpuMinorSeries.Adreno700, number);
                    case 8:
                        return (GpuMinorSeries.Adreno800, number);
                    case 9:
                        return (GpuMinorSeries.Adreno900, number);
                    default:
                        return (GpuMinorSeries.Unknown, number);
                }
            }

            return (GpuMinorSeries.Unknown, 0);
        }

        public static (GpuMinorSeries, int) ParseMaliGpuSeries(string gpuName)
        {
            // parse Mali GPU series number by regex
            // ex: Mali-Gxx, Mali-Txxx
            var match = Regex.Match(gpuName, @"Mali-([GT]?)(\d+)");
            if (match.Success)
            {
                var number = int.Parse(match.Groups[2].Value);
                return match.Groups[1].Value switch
                {
                    "G" => (GpuMinorSeries.MaliG, number),
                    "T" => (GpuMinorSeries.MaliT, number),
                    "" => (GpuMinorSeries.Mali, number),
                    _ => (GpuMinorSeries.Unknown, number)
                };
            }

            return (GpuMinorSeries.Unknown, 0);
        }

        public static (GpuMinorSeries, int) ParsePowerVRGpuSeries(string gpuName)
        {
            // parse PowerVR GPU series number by regex
            // ex: PowerVR Rogue GExxxx, PowerVR Rogue G?xxxx
            var match = Regex.Match(gpuName, @"PowerVR Rogue (G[A-Z])(\d)(\d+)");
            if (match.Success)
            {
                var number = int.Parse(match.Groups[2].Value + match.Groups[3].Value);
                return (match.Groups[1].Value + match.Groups[2].Value) switch
                {
                    "GX6" => (GpuMinorSeries.PowerVR6XT, number),
                    "GE8" => (GpuMinorSeries.PowerVR8XE, number),
                    "GM9" => (GpuMinorSeries.PowerVR9XM, number),
                    _ => (GpuMinorSeries.Unknown, number)
                };
            }


            return (GpuMinorSeries.Unknown, 0);
        }

        public static (GpuMinorSeries, int) ParseXclipseGpuSeries(string gpuName)
        {
            // parse Samsung Xclipse GPU series number by regex
            // ex: Samsung Xclipse xxx
            var match = Regex.Match(gpuName, @"Samsung Xclipse (\d+)");
            if (match.Success)
            {
                var number = int.Parse(match.Groups[1].Value);
                return (GpuMinorSeries.Xclipse, number);
            }

            return (GpuMinorSeries.Unknown, 0);
        }

        public static (GpuMinorSeries, int) ParseMaleoonGpuSeries(string gpuName)
        {
            // parse Huawei Maleoon GPU series number by regex
            // ex: Maleoon xxx
            var match = Regex.Match(gpuName, @"Maleoon (\d+)");
            if (match.Success)
            {
                var number = int.Parse(match.Groups[1].Value);
                return (GpuMinorSeries.Maleoon, number);
            }

            return (GpuMinorSeries.Unknown, 0);
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
