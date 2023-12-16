using System.Text;
using MobileSupport.PerformanceIndex;
using UnityEngine;

public class PerformanceIndexView : MonoBehaviour
{
    [SerializeField]
    private SamplePerformanceIndexData samplePerformanceIndexData;

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

        if (samplePerformanceIndexData == null)
        {
            Debug.LogError("SamplePerformanceIndexData is null");
        }
        else
        {
            var performanceLevel = SampleQualityLevel.Medium;
            if (samplePerformanceIndexData.GetPerformanceLevel(stats, ref performanceLevel))
                Debug.Log($"PerformanceLevel: {performanceLevel}");
            else
                Debug.Log("PerformanceLevel: Unknown");
        }
    }
}
