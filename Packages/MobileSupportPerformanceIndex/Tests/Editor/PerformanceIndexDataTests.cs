using NUnit.Framework;
using UnityEngine;

namespace MobileSupport.PerformanceIndex.Editor.Tests
{
    // need to define a class to test generic ScriptableObject
    internal class TestPerformanceIndexData : PerformanceIndexData<int>
    {
    }

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

            var data = ScriptableObject.CreateInstance<TestPerformanceIndexData>();
            data.devicePerformanceIndexIndices = new[]
            {
                new DevicePerformanceIndex<int>
                {
                    deviceModel = "iPhone 10",
                    performanceLevel = 3
                },
                new DevicePerformanceIndex<int>
                {
                    deviceModel = "iPhone 12",
                    performanceLevel = 5
                },
                new DevicePerformanceIndex<int>
                {
                    deviceModel = "iPhone 12 Pro Max",
                    performanceLevel = 1
                },
                new DevicePerformanceIndex<int>
                {
                    deviceModel = "iPhone 13 Pro Max",
                    performanceLevel = 2
                }
            };

            var performanceLevel = 0;
            Assert.AreEqual(expectedResult, data.GetPerformanceLevel(stats, ref performanceLevel));
            Assert.AreEqual(expectedPerformanceLevel, performanceLevel);
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

            var data = ScriptableObject.CreateInstance<TestPerformanceIndexData>();
            data.gpuPerformanceIndices = new[]
            {
                new GpuPerformanceIndex<int>
                {
                    gpuMajorSeries = GpuMajorSeries.Adreno,
                    gpuSeriesNumberMin = 500,
                    gpuSeriesNumberMax = 599,
                    performanceLevel = 5
                },
                new GpuPerformanceIndex<int>
                {
                    gpuMajorSeries = GpuMajorSeries.Adreno,
                    gpuSeriesNumberMin = 600,
                    gpuSeriesNumberMax = 799,
                    performanceLevel = 10
                },
                new GpuPerformanceIndex<int>
                {
                    gpuMajorSeries = GpuMajorSeries.Adreno,
                    gpuSeriesNumberMin = 200,
                    gpuSeriesNumberMax = 299,
                    performanceLevel = 1
                },
                new GpuPerformanceIndex<int>
                {
                    gpuMajorSeries = GpuMajorSeries.Adreno,
                    gpuSeriesNumberMin = 300,
                    gpuSeriesNumberMax = 499,
                    performanceLevel = 3
                }
            };

            var performanceLevel = 0;
            Assert.AreEqual(expectedResult, data.GetPerformanceLevel(stats, ref performanceLevel));
            Assert.AreEqual(expectedPerformanceLevel, performanceLevel);
        }
    }
}
