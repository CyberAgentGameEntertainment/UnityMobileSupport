// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MobileSupport.QualityTuner
{
    /// <summary>
    ///     Enum of GPU series
    /// </summary>
    public enum GpuMajorSeries
    {
        /// <summary>
        ///     Unknown
        /// </summary>
        Unknown,

        /// <summary>
        ///     Apple X-series
        /// </summary>
        Apple,

        /// <summary>
        ///     Qualcomm Adreno series
        /// </summary>
        Adreno,

        /// <summary>
        ///     ARM Mali series
        /// </summary>
        Mali,

        /// <summary>
        ///     PowerVR series
        /// </summary>
        PowerVR,

        /// <summary>
        ///     Samsung Xclipse series
        /// </summary>
        Xclipse,

        /// <summary>
        ///     Huawei Maleoon series
        /// </summary>
        Maleoon,

        /// <summary>
        ///     ARM Immortalis series
        /// </summary>
        Immortalis
    }

    public enum GpuMinorSeries
    {
        /// <summary>
        ///     Unknown
        /// </summary>
        Unknown,

        /// <summary>
        ///     Apple A-series
        /// </summary>
        AppleA = 11,

        /// <summary>
        ///     Apple M-series
        /// </summary>
        AppleM = 12,

        /// <summary>
        ///     Qualcomm Adreno 100 series
        /// </summary>
        Adreno100 = 21,

        /// <summary>
        ///     Qualcomm Adreno 200 series
        /// </summary>
        Adreno200 = 22,

        /// <summary>
        ///     Qualcomm Adreno 300 series
        /// </summary>
        Adreno300 = 23,

        /// <summary>
        ///     Qualcomm Adreno 400 series
        /// </summary>
        Adreno400 = 24,

        /// <summary>
        ///     Qualcomm Adreno 500 series
        /// </summary>
        Adreno500 = 25,

        /// <summary>
        ///     Qualcomm Adreno 600 series
        /// </summary>
        Adreno600 = 26,

        /// <summary>
        ///     Qualcomm Adreno 700 series
        /// </summary>
        Adreno700 = 27,

        /// <summary>
        ///     Qualcomm Adreno 800 series
        /// </summary
        Adreno800 = 28,

        /// <summary>
        ///     Qualcomm Adreno 900 series
        /// </summary>
        Adreno900 = 29,

        /// <summary>
        ///     ARM Mali series
        /// </summary>
        Mali = 31,

        /// <summary>
        ///     ARM Mali-T series
        /// </summary>
        MaliT = 32,

        /// <summary>
        ///     ARM Mali-G series
        /// </summary>
        MaliG = 33,

        /// <summary>
        ///     PowerVR 6XT series
        /// </summary>
        PowerVR6XT = 41,

        /// <summary>
        ///     PowerVR 8XE series
        /// </summary>
        PowerVR8XE = 42,

        /// <summary>
        ///     PowerVR 9XM series
        /// </summary>
        PowerVR9XM = 43,

        /// <summary>
        ///     PowerVR BXM series
        /// </summary>
        PowerVRBXM = 44,

        /// <summary>
        ///     PowerVR DXT series
        /// </summary>
        PowerVRDXT = 45,

        /// <summary>
        ///     Samsung Xclipse series
        /// </summary>
        Xclipse = 51,

        /// <summary>
        ///     Huawei Maleoon series
        /// </summary>
        Maleoon = 61,

        /// <summary>
        ///     ARM Immortalis-G series
        /// </summary>
        ImmortalisG = 71
    }

    [Serializable]
    public class GpuSeriesEnumeration : IComparable
    {
        public static readonly GpuSeriesEnumeration Unkown = new(GpuMajorSeries.Unknown, GpuMinorSeries.Unknown);

        // Apple
        public static readonly GpuSeriesEnumeration AppleAny = new(GpuMajorSeries.Apple, GpuMinorSeries.Unknown);
        public static readonly GpuSeriesEnumeration AppleA = new(GpuMajorSeries.Apple, GpuMinorSeries.AppleA);
        public static readonly GpuSeriesEnumeration AppleM = new(GpuMajorSeries.Apple, GpuMinorSeries.AppleM);

        // Adreno
        public static readonly GpuSeriesEnumeration AdrenoAny = new(GpuMajorSeries.Adreno, GpuMinorSeries.Unknown);
        public static readonly GpuSeriesEnumeration Adreno100 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno100);
        public static readonly GpuSeriesEnumeration Adreno200 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno200);
        public static readonly GpuSeriesEnumeration Adreno300 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno300);
        public static readonly GpuSeriesEnumeration Adreno400 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno400);
        public static readonly GpuSeriesEnumeration Adreno500 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno500);
        public static readonly GpuSeriesEnumeration Adreno600 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno600);
        public static readonly GpuSeriesEnumeration Adreno700 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno700);
        public static readonly GpuSeriesEnumeration Adreno800 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno800);
        public static readonly GpuSeriesEnumeration Adreno900 = new(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno900);

        // Mali
        public static readonly GpuSeriesEnumeration MaliAny = new(GpuMajorSeries.Mali, GpuMinorSeries.Unknown);
        public static readonly GpuSeriesEnumeration Mali = new(GpuMajorSeries.Mali, GpuMinorSeries.Mali);
        public static readonly GpuSeriesEnumeration MaliT = new(GpuMajorSeries.Mali, GpuMinorSeries.MaliT);
        public static readonly GpuSeriesEnumeration MaliG = new(GpuMajorSeries.Mali, GpuMinorSeries.MaliG);

        // ARM Immortalis
        public static readonly GpuSeriesEnumeration ImmortalisAny = new(GpuMajorSeries.Immortalis, GpuMinorSeries.Unknown);
        public static readonly GpuSeriesEnumeration ImmortalisG = new(GpuMajorSeries.Immortalis, GpuMinorSeries.ImmortalisG);

        // PowerVR
        public static readonly GpuSeriesEnumeration PowerVRAny = new(GpuMajorSeries.PowerVR, GpuMinorSeries.Unknown);
        public static readonly GpuSeriesEnumeration PowerVR6XT = new(GpuMajorSeries.PowerVR, GpuMinorSeries.PowerVR6XT);
        public static readonly GpuSeriesEnumeration PowerVR8XE = new(GpuMajorSeries.PowerVR, GpuMinorSeries.PowerVR8XE);
        public static readonly GpuSeriesEnumeration PowerVR9XM = new(GpuMajorSeries.PowerVR, GpuMinorSeries.PowerVR9XM);
        public static readonly GpuSeriesEnumeration PowerVRDXT = new(GpuMajorSeries.PowerVR, GpuMinorSeries.PowerVRDXT);
        public static readonly GpuSeriesEnumeration PowerVRBXM = new(GpuMajorSeries.PowerVR, GpuMinorSeries.PowerVRBXM);

        // Samsung
        public static readonly GpuSeriesEnumeration Xclipse = new(GpuMajorSeries.Xclipse, GpuMinorSeries.Xclipse);

        // Huawei
        public static readonly GpuSeriesEnumeration Maleoon = new(GpuMajorSeries.Maleoon, GpuMinorSeries.Maleoon);

        [SerializeField]
        protected int value;

        [SerializeField]
        protected GpuMajorSeries gpuMajorSeries;

        [SerializeField]
        protected GpuMinorSeries gpuMinorSeries;

        protected GpuSeriesEnumeration(GpuMajorSeries gpuMajorSeries, GpuMinorSeries gpuMinorSeries)
        {
            this.gpuMajorSeries = gpuMajorSeries;
            this.gpuMinorSeries = gpuMinorSeries;

            if (gpuMinorSeries == GpuMinorSeries.Unknown)
                value = (int)gpuMajorSeries * 10;
            else
                value = (int)gpuMinorSeries;
        }

        public GpuMajorSeries GpuMajorSeries => gpuMajorSeries;
        public GpuMinorSeries GpuMinorSeries => gpuMinorSeries;

        public int CompareTo(object other)
        {
            if (other is GpuSeriesEnumeration otherValue)
                return value.CompareTo(otherValue.value);
            return -1;
        }

        public static implicit operator int(GpuSeriesEnumeration e)
        {
            return e.value;
        }

        public static implicit operator GpuSeriesEnumeration(int value)
        {
            return FromValue<GpuSeriesEnumeration>(value);
        }

        public override string ToString()
        {
            if (gpuMajorSeries == GpuMajorSeries.Unknown)
                return "Any";
            if (gpuMinorSeries == GpuMinorSeries.Unknown)
                return gpuMajorSeries + "/Any";
            return gpuMajorSeries + "/" + gpuMinorSeries;
        }

        public static IEnumerable<T> GetAll<T>() where T : GpuSeriesEnumeration
        {
            return TypeCache<T>.Enums;
        }

        private static IEnumerable<T> _GetAll<T>() where T : GpuSeriesEnumeration
        {
            return typeof(T).GetFields(BindingFlags.Public |
                                       BindingFlags.Static |
                                       BindingFlags.DeclaredOnly)
                .Select(f => f.GetValue(null))
                .Cast<T>();
        }

        public static T FromValue<T>(int value) where T : GpuSeriesEnumeration
        {
            var matchingItem = TypeCache<T>.Enums.FirstOrDefault(item => item.value == value);

            if (matchingItem == null)
                throw new ArgumentException($"'{value}' is not a valid value in {typeof(T)}");

            return matchingItem;
        }

        public static T FromGpuSeries<T>(GpuMajorSeries gpuMajorSeries, GpuMinorSeries gpuMinorSeries)
            where T : GpuSeriesEnumeration
        {
            var matchingItem = TypeCache<T>.Enums.FirstOrDefault(item =>
                item.gpuMajorSeries == gpuMajorSeries && item.gpuMinorSeries == gpuMinorSeries);

            if (matchingItem == null)
                throw new ArgumentException(
                    $"'{gpuMajorSeries}/{gpuMinorSeries}' and  is not a valid GPU series in {typeof(T)}");

            return matchingItem;
        }

        public override bool Equals(object obj)
        {
            if (obj is not GpuSeriesEnumeration otherValue)
                return false;

            // type matches and value matches
            return GetType().Equals(obj.GetType()) && value.Equals(otherValue.value);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        protected static class TypeCache<T> where T : GpuSeriesEnumeration
        {
            public static readonly T[] Enums;

            static TypeCache()
            {
                Enums = _GetAll<T>().ToArray();
            }
        }
    }
}
