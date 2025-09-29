// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using NUnit.Framework;

namespace MobileSupport.QualityTuner.Editor.Tests
{
    public class HardwareInfoIosTests
    {
        [TestCase("Apple A8 GPU", ExpectedResult = GpuMinorSeries.AppleA)]
        [TestCase("Apple A8X GPU", ExpectedResult = GpuMinorSeries.AppleA)]
        [TestCase("Apple A10 GPU", ExpectedResult = GpuMinorSeries.AppleA)]
        [TestCase("Apple A10X GPU", ExpectedResult = GpuMinorSeries.AppleA)]
        [TestCase("Apple A12Z GPU", ExpectedResult = GpuMinorSeries.AppleA)]
        [TestCase("Apple A17 Pro GPU", ExpectedResult = GpuMinorSeries.AppleAPro)]
        [TestCase("Apple A15 Pro GPU", ExpectedResult = GpuMinorSeries.AppleAPro)]
        [TestCase("Apple M1 GPU", ExpectedResult = GpuMinorSeries.AppleM)]
        [TestCase("Apple M1", ExpectedResult = GpuMinorSeries.AppleM)]
        public GpuMinorSeries HardwareInfoIos_ParseGpuMinorSeries_Tests(string gpuName)
        {
            return HardwareInfoIos.ParseGpuMinorSeries(gpuName);
        }

        [TestCase("Apple A8 GPU", ExpectedResult = 8)]
        [TestCase("Apple A8X GPU", ExpectedResult = 8)]
        [TestCase("Apple A10 GPU", ExpectedResult = 10)]
        [TestCase("Apple A10X GPU", ExpectedResult = 10)]
        [TestCase("Apple A12Z GPU", ExpectedResult = 12)]
        [TestCase("Apple A17 Pro GPU", ExpectedResult = 17)]
        [TestCase("Apple M1 GPU", ExpectedResult = 1)]
        [TestCase("Apple M1", ExpectedResult = 1)]
        [TestCase("Apple A2147483647 GPU", ExpectedResult = int.MaxValue)]
        [TestCase("Apple A2147483648 GPU", ExpectedResult = 0)]
        public int HardwareInfoAndroid_ParseAppleGpuSeriesNumber_Tests(string gpuName)
        {
            return HardwareInfoIos.ParseAppleGpuSeriesNumber(gpuName);
        }
    }
}
