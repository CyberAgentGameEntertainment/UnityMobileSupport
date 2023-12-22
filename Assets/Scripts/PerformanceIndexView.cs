using System.Text;
using MobileSupport.PerformanceIndex;
using UnityEngine;
using UnityEngine.Serialization;

public class PerformanceIndexView : MonoBehaviour
{
    [FormerlySerializedAs("sampleQualityLevelMatcher")]
    [SerializeField]
    private SampleQualityLevelSelector sampleQualityLevelSelector;

    private void Start()
    {
        var stats = HardwareInfo.GetHardwareStats();
        var sb = new StringBuilder();
        sb.AppendLine($"DeviceModel: {stats.DeviceModel}");
        sb.AppendLine($"GpuName: {stats.GpuName}");
        sb.AppendLine($"GpuMajorSeries: {stats.GpuMajorSeries}");
        sb.AppendLine($"GpuMinorSeries: {stats.GpuMinorSeries}");
        sb.AppendLine($"GpuSeriesNumber: {stats.GpuSeriesNumber}");
        sb.AppendLine($"SocName: {stats.SocName}");
        sb.AppendLine($"GpuMemorySizeMb: {stats.GpuMemorySizeMb}");
        sb.AppendLine($"SystemMemorySizeMb: {stats.SystemMemorySizeMb}");
        Debug.Log(sb.ToString());

        if (sampleQualityLevelSelector == null)
        {
            Debug.LogError("SampleQualityLevelSelector is null");
        }
        else
        {
            if (sampleQualityLevelSelector.GetQualityLevel(stats, out var qualityLevel))
                Debug.Log($"PerformanceLevel: {qualityLevel}");
            else
                Debug.Log("PerformanceLevel: Unknown");
        }
    }
}
