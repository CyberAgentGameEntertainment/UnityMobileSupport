using MobileSupport.PerformanceIndex;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SampleQualityLevelSelector), menuName = "Sample/Performance Index Data")]
public class SampleQualityLevelSelector : QualityLevelSelector<SampleQualityLevel>
{
}

public class SampleDeviceRuleMatcher : DeviceRuleMatcher<SampleQualityLevel>
{
}

public class SampleGpuRuleMatcher : GpuRuleMatcher<SampleQualityLevel>
{
}

public class SampleSystemMemoryRuleMatcher : SystemMemoryRuleMatcher<SampleQualityLevel>
{
}

public enum SampleQualityLevel
{
    Low,
    Medium,
    High
}
