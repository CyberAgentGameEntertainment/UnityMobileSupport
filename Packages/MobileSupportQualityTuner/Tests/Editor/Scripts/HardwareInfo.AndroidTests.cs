// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Collections;
using NUnit.Framework;

namespace MobileSupport.QualityTuner.Editor.Tests
{
    public class HardwareInfoAndroidTests
    {
        private static IEnumerable AdrenoTestCases
        {
            get
            {
                yield return new TestCaseData("Adreno (TM) 308").Returns((GpuMinorSeries.Adreno300, 308));
                yield return new TestCaseData("Adreno (TM) 418").Returns((GpuMinorSeries.Adreno400, 418));
                yield return new TestCaseData("Adreno (TM) 508").Returns((GpuMinorSeries.Adreno500, 508));
                yield return new TestCaseData("Adreno (TM) 630").Returns((GpuMinorSeries.Adreno600, 630));
                yield return new TestCaseData("Adreno (TM) 642L").Returns((GpuMinorSeries.Adreno600, 642));
                yield return new TestCaseData("Adreno (TM) 650 (RADV NAVI23)").Returns((GpuMinorSeries.Adreno600, 650));
                yield return new TestCaseData("Adreno (TM) 725").Returns((GpuMinorSeries.Adreno700, 725));
                yield return new TestCaseData("Adreno (TM) 1000").Returns((GpuMinorSeries.Unknown, 1000));
                yield return new TestCaseData("Adreno (TM) 2147483648").Returns((GpuMinorSeries.Unknown, 0));
            }
        }

        private static IEnumerable MaliTestCases
        {
            get
            {
                yield return new TestCaseData("Mali-G52").Returns((GpuMinorSeries.MaliG, 52));
                yield return new TestCaseData("Mali-G76").Returns((GpuMinorSeries.MaliG, 76));
                yield return new TestCaseData("Mali-G610").Returns((GpuMinorSeries.MaliG, 610));
                yield return new TestCaseData("Mali-G715-Immortalis").Returns((GpuMinorSeries.MaliG, 715));
                yield return new TestCaseData("Mali-T720").Returns((GpuMinorSeries.MaliT, 720));
                yield return new TestCaseData("Mali-T880").Returns((GpuMinorSeries.MaliT, 880));
                yield return new TestCaseData("Mali-400").Returns((GpuMinorSeries.Mali, 400));
                yield return new TestCaseData("Mali-2147483648").Returns((GpuMinorSeries.Unknown, 0));
            }
        }

        private static IEnumerable PowerVRTestCases
        {
            get
            {
                yield return new TestCaseData("PowerVR Rogue GX6250").Returns((GpuMinorSeries.PowerVR6XT, 6250));
                yield return new TestCaseData("PowerVR Rogue GE8300").Returns((GpuMinorSeries.PowerVR8XE, 8300));
                yield return new TestCaseData("PowerVR Rogue GE8320").Returns((GpuMinorSeries.PowerVR8XE, 8320));
                yield return new TestCaseData("PowerVR Rogue GE8322").Returns((GpuMinorSeries.PowerVR8XE, 8322));
                yield return new TestCaseData("PowerVR Rogue GM9446").Returns((GpuMinorSeries.PowerVR9XM, 9446));
                yield return new TestCaseData("PowerVR B-Series BXM-8-256").Returns((GpuMinorSeries.Unknown, 0));
                yield return new TestCaseData("PowerVR Rogue GM2147483648").Returns((GpuMinorSeries.Unknown, 0));
            }
        }

        private static IEnumerable XclipseTestCases
        {
            get
            {
                yield return new TestCaseData("Samsung Xclipse 920").Returns((GpuMinorSeries.Xclipse, 920));
                yield return new TestCaseData("Samsung Xclipse 2147483647").Returns((GpuMinorSeries.Xclipse,
                    int.MaxValue));
                yield return new TestCaseData("Samsung Xclipse 2147483648").Returns((GpuMinorSeries.Unknown, 0));
            }
        }

        private static IEnumerable MaleoonTestCases
        {
            get
            {
                yield return new TestCaseData("Maleoon 910").Returns((GpuMinorSeries.Maleoon, 910));
                yield return new TestCaseData("Maleoon 2147483647").Returns((GpuMinorSeries.Maleoon, int.MaxValue));
                yield return new TestCaseData("Maleoon 2147483648").Returns((GpuMinorSeries.Unknown, 0));
            }
        }

        private static IEnumerable ImmortalisTestCases
        {
            get
            {
                yield return new TestCaseData("ARM Immortalis-G720 MP12").Returns((GpuMinorSeries.ImmortalisG, 720));
                yield return new TestCaseData("ARM Immortalis-G720 MP7").Returns((GpuMinorSeries.ImmortalisG, 720));
                yield return new TestCaseData("ARM Immortalis-G925 MP16").Returns((GpuMinorSeries.ImmortalisG, 925));
                yield return new TestCaseData("ARM Immortalis-G925 MP12").Returns((GpuMinorSeries.ImmortalisG, 925));
                yield return new TestCaseData("ARM Immortalis-G2147483647").Returns((GpuMinorSeries.ImmortalisG, int.MaxValue));
                yield return new TestCaseData("ARM Immortalis-G2147483648").Returns((GpuMinorSeries.Unknown, 0));
            }
        }

        [TestCaseSource(typeof(HardwareInfoAndroidTests), nameof(AdrenoTestCases))]
        public (GpuMinorSeries, int) ParseAdrenoGpuSeries_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParseAdrenoGpuSeries(gpuName);
        }

        [TestCaseSource(typeof(HardwareInfoAndroidTests), nameof(MaliTestCases))]
        public (GpuMinorSeries, int) ParseMaliGpuSeries_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParseMaliGpuSeries(gpuName);
        }

        [TestCaseSource(typeof(HardwareInfoAndroidTests), nameof(PowerVRTestCases))]
        public (GpuMinorSeries, int) ParsePowerVRGpuSeries_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParsePowerVRGpuSeries(gpuName);
        }

        [TestCaseSource(typeof(HardwareInfoAndroidTests), nameof(XclipseTestCases))]
        public (GpuMinorSeries, int) ParseXclipseGpuSeries_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParseXclipseGpuSeries(gpuName);
        }

        [TestCaseSource(typeof(HardwareInfoAndroidTests), nameof(MaleoonTestCases))]
        public (GpuMinorSeries, int) ParseMaleoonGpuSeries_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParseMaleoonGpuSeries(gpuName);
        }

        [TestCaseSource(typeof(HardwareInfoAndroidTests), nameof(ImmortalisTestCases))]
        public (GpuMinorSeries, int) ParseImmortalisGpuSeries_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParseImmortalisGpuSeries(gpuName);
        }
    }
}
