// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using NUnit.Framework;

namespace MobileSupport.PerformanceIndex.Editor.Tests
{
    public class PerformanceIndexDataTests
    {
        [Test]
        [TestCase("iPhone 10", true, 3)]
        [TestCase("iPhone 12", true, 5)]
        [TestCase("iPhone 12 Pro Max", true, 1)]
        [TestCase("iPhone 13 Pro Max", true, 2)]
        [TestCase("iPhone 12 Pro", false, 0)]
        [TestCase("iPhone 1", false, 0)]
        [TestCase("iPhone 100", false, 0)]
        [TestCase("", false, 0)]
        public void PerformanceIndexDataTests_OnlyDeviceData(string deviceModel, bool expectedResult,
            int expectedPerformanceLevel)
        {
            var stats = new HardwareStats
            {
                DeviceModel = deviceModel
            };

            var matcher = new DeviceRuleMatcher<int>();
            matcher.rules = new[]
            {
                new DeviceRuleMatcher<int>.Rule
                {
                    deviceModel = "iPhone 10",
                    qualityLevel = 3
                },
                new DeviceRuleMatcher<int>.Rule
                {
                    deviceModel = "iPhone 12",
                    qualityLevel = 5
                },
                new DeviceRuleMatcher<int>.Rule
                {
                    deviceModel = "iPhone 12 Pro Max",
                    qualityLevel = 1
                },
                new DeviceRuleMatcher<int>.Rule
                {
                    deviceModel = "iPhone 13 Pro Max",
                    qualityLevel = 2
                }
            };

            var qualityLevel = 0;
            Assert.AreEqual(expectedResult, matcher.Match(stats, ref qualityLevel));
            Assert.AreEqual(expectedPerformanceLevel, qualityLevel);
        }


        [Test]
        [TestCase(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno500, 555, true, 5)]
        [TestCase(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno600, 600, true, 10)]
        [TestCase(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno200, 299, true, 1)]
        [TestCase(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno400, 400, true, 3)]
        [TestCase(GpuMajorSeries.Apple, GpuMinorSeries.AppleA, 12, false, 0)]
        public void PerformanceIndexDataTests_OnlyGpuData(GpuMajorSeries gpuMajorSeries, GpuMinorSeries gpuMinorSeries,
            int gpuSeriesNumber, bool expectedResult, int expectedPerformanceLevel)
        {
            var stats = new HardwareStats
            {
                GpuMajorSeries = gpuMajorSeries,
                GpuMinorSeries = gpuMinorSeries,
                GpuSeriesNumber = gpuSeriesNumber
            };

            var matcher = new GpuRuleMatcher<int>();
            matcher.gpuSeries = GpuSeriesEnumeration.AdrenoAny;
            matcher.rule = new[]
            {
                new GpuRuleMatcher<int>.Rule
                {
                    gpuSeriesNumberMin = 500,
                    gpuSeriesNumberMax = 599,
                    qualityLevel = 5
                },
                new GpuRuleMatcher<int>.Rule
                {
                    gpuSeriesNumberMin = 600,
                    gpuSeriesNumberMax = 799,
                    qualityLevel = 10
                },
                new GpuRuleMatcher<int>.Rule
                {
                    gpuSeriesNumberMin = 200,
                    gpuSeriesNumberMax = 299,
                    qualityLevel = 1
                },
                new GpuRuleMatcher<int>.Rule
                {
                    gpuSeriesNumberMin = 300,
                    gpuSeriesNumberMax = 499,
                    qualityLevel = 3
                }
            };

            var qualityLevel = 0;
            Assert.AreEqual(expectedResult, matcher.Match(stats, ref qualityLevel));
            Assert.AreEqual(expectedPerformanceLevel, qualityLevel);
        }
    }
}
