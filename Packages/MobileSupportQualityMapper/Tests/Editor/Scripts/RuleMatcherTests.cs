// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using NUnit.Framework;

namespace MobileSupport.QualityMapper.Editor.Tests
{
    public class RuleMatcherTests
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
        public void DeviceRuleMatcherTests(string deviceModel, bool expectedResult,
            int expectedQualityLevel)
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

            Assert.AreEqual(expectedResult, matcher.Match(stats, out var qualityLevel));
            Assert.AreEqual(expectedQualityLevel, qualityLevel);
        }


        [Test]
        [TestCase(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno500, 555, true, 5)]
        [TestCase(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno600, 600, true, 10)]
        [TestCase(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno200, 299, true, 1)]
        [TestCase(GpuMajorSeries.Adreno, GpuMinorSeries.Adreno400, 400, true, 3)]
        [TestCase(GpuMajorSeries.Apple, GpuMinorSeries.AppleA, 12, false, 0)]
        public void GpuRuleMatcherTests(GpuMajorSeries gpuMajorSeries, GpuMinorSeries gpuMinorSeries,
            int gpuSeriesNumber, bool expectedResult, int expectedQualityLevel)
        {
            var stats = new HardwareStats
            {
                GpuMajorSeries = gpuMajorSeries,
                GpuMinorSeries = gpuMinorSeries,
                GpuSeriesNumber = gpuSeriesNumber
            };

            var matcher = new GpuRuleMatcher<int, GpuSeriesEnumeration>();
            matcher.rules = new[]
            {
                new GpuRuleMatcher<int, GpuSeriesEnumeration>.Rule
                {
                    gpuSeries = GpuSeriesEnumeration.AdrenoAny,
                    gpuSeriesNumberMin = 500,
                    gpuSeriesNumberMax = 599,
                    qualityLevel = 5
                },
                new GpuRuleMatcher<int, GpuSeriesEnumeration>.Rule
                {
                    gpuSeries = GpuSeriesEnumeration.AdrenoAny,
                    gpuSeriesNumberMin = 600,
                    gpuSeriesNumberMax = 799,
                    qualityLevel = 10
                },
                new GpuRuleMatcher<int, GpuSeriesEnumeration>.Rule
                {
                    gpuSeries = GpuSeriesEnumeration.AdrenoAny,
                    gpuSeriesNumberMin = 200,
                    gpuSeriesNumberMax = 299,
                    qualityLevel = 1
                },
                new GpuRuleMatcher<int, GpuSeriesEnumeration>.Rule
                {
                    gpuSeries = GpuSeriesEnumeration.AdrenoAny,
                    gpuSeriesNumberMin = 300,
                    gpuSeriesNumberMax = 499,
                    qualityLevel = 3
                }
            };

            Assert.AreEqual(expectedResult, matcher.Match(stats, out var qualityLevel));
            Assert.AreEqual(expectedQualityLevel, qualityLevel);
        }

        // test for SystemMemoryRuleMatcher
        [Test]
        [TestCase(0, true, 1)]
        [TestCase(1024, true, 1)]
        [TestCase(1025, true, 5)]
        [TestCase(2048, true, 5)]
        [TestCase(4096, true, 5)]
        [TestCase(4097, true, 10)]
        [TestCase(8192, true, 10)]
        [TestCase(8193, false, 0)]
        public void SystemMemoryRuleMatcherTests(int systemMemorySize, bool expectedResult, int expectedQualityLevel)
        {
            var stats = HardwareStats.CreateDefault();
            stats.SystemMemorySizeMb = systemMemorySize;

            var matcher = new SystemMemoryRuleMatcher<int>();
            matcher.rules = new[]
            {
                new SystemMemoryRuleMatcher<int>.Rule
                {
                    systemMemoryMin = 0,
                    systemMemoryMax = 1024,
                    qualityLevel = 1
                },
                new SystemMemoryRuleMatcher<int>.Rule
                {
                    systemMemoryMin = 1025,
                    systemMemoryMax = 4096,
                    qualityLevel = 5
                },
                new SystemMemoryRuleMatcher<int>.Rule
                {
                    systemMemoryMin = 4097,
                    systemMemoryMax = 8192,
                    qualityLevel = 10
                }
            };


            Assert.AreEqual(expectedResult, matcher.Match(stats, out var qualityLevel));
            Assert.AreEqual(expectedQualityLevel, qualityLevel);
        }
    }
}
