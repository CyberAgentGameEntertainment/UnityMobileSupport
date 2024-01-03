// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using NUnit.Framework;

namespace MobileSupport.QualityTuner.Editor.Tests
{
    public class HardwareInfoIosTests
    {
        [TestCase("Apple A8 GPU", ExpectedResult = 8)]
        [TestCase("Apple A8X GPU", ExpectedResult = 8)]
        [TestCase("Apple A10 GPU", ExpectedResult = 10)]
        [TestCase("Apple A10X GPU", ExpectedResult = 10)]
        [TestCase("Apple A12Z GPU", ExpectedResult = 12)]
        [TestCase("Apple A17 Pro GPU", ExpectedResult = 17)]
        [TestCase("Apple M1 GPU", ExpectedResult = 1)]
        [TestCase("Apple M1", ExpectedResult = 1)]
        public int HardwareInfoAndroid_ParseAppleGpuSeriesNumber_Tests(string gpuName)
        {
            return HardwareInfoIos.ParseAppleGpuSeriesNumber(gpuName);
        }
    }
}
