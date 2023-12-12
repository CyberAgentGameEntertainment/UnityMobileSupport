using NUnit.Framework;

namespace MobileSupport.PerformanceIndex.Editor.Tests
{
    public class HardwareInfoAndroidTests
    {
        [TestCase("Adreno (TM) 308", ExpectedResult = 308)]
        [TestCase("Adreno (TM) 418", ExpectedResult = 418)]
        [TestCase("Adreno (TM) 508", ExpectedResult = 508)]
        [TestCase("Adreno (TM) 630", ExpectedResult = 630)]
        [TestCase("Adreno (TM) 642L", ExpectedResult = 642)]
        [TestCase("Adreno (TM) 650 (RADV NAVI23)", ExpectedResult = 650)]
        [TestCase("Adreno (TM) 725", ExpectedResult = 725)]
        public int HardwareInfoAndroid_ParseAdrenoGpuSeriesNumber_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParseAdrenoGpuSeriesNumber(gpuName);
        }

        [TestCase("Mali-G52", ExpectedResult = 52)]
        [TestCase("Mali-G76", ExpectedResult = 76)]
        [TestCase("Mali-G610", ExpectedResult = 610)]
        [TestCase("Mali-G715-Immortalis", ExpectedResult = 715)]
        [TestCase("Mali-T720", ExpectedResult = 720)]
        [TestCase("Mali-T880", ExpectedResult = 880)]
        [TestCase("Mali-400", ExpectedResult = 400)]
        public int HardwareInfoAndroid_ParseMaliGpuSeriesNumber_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParseMaliGpuSeriesNumber(gpuName);
        }

        [TestCase("PowerVR Rogue GE8300", ExpectedResult = 8300)]
        [TestCase("PowerVR Rogue GE8320", ExpectedResult = 8320)]
        [TestCase("PowerVR Rogue GE8322", ExpectedResult = 8322)]
        [TestCase("PowerVR Rogue GM9446", ExpectedResult = 9446)]
        [TestCase("PowerVR Rogue GX6250", ExpectedResult = 6250)]
        public int HardwareInfoAndroid_ParsePowerVRGpuSeriesNumber_Tests(string gpuName)
        {
            return HardwareInfoAndroid.ParsePowerVRGpuSeriesNumber(gpuName);
        }
    }
}
