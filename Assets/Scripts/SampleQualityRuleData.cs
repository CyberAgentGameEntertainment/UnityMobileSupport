// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using MobileSupport.QualityTuner;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SampleQualityRuleData), menuName = "Sample/Quality Level Rule Data")]
public class SampleQualityRuleData : QualityRuleData<SampleQualityLevel>
{
}

public class SampleDeviceNameRuleMatcher : DeviceNameRuleMatcherBase<SampleQualityLevel>
{
}

public class SampleGpuSeriesRuleMatcher : GpuSeriesRuleMatcherBase<SampleQualityLevel, GpuSeriesEnumeration>
{
}

public class SampleSystemMemoryRuleMatcher : SystemMemoryRuleMatcherBase<SampleQualityLevel>
{
}

public enum SampleQualityLevel
{
    Low,
    Medium,
    High
}
