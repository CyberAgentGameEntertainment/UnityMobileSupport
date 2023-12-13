using MobileSupport.PerformanceIndex;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SamplePerformanceIndexData), menuName = "Sample/Performance Index Data")]
public class SamplePerformanceIndexData : PerformanceIndexData<SampleQualityLevel>
{
}

public enum SampleQualityLevel
{
    Low,
    Medium,
    High
}
