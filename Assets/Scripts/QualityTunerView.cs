// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Text;
using MobileSupport.QualityTuner;
using UnityEngine;

public class QualityTunerView : MonoBehaviour
{
    [SerializeField]
    private SampleQualityRuleData sampleQualityRuleData;

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

        if (sampleQualityRuleData == null)
        {
            Debug.LogError("SampleQualityLevelSelector is null");
        }
        else
        {
            var sampleQualityLevelSelector = new RuleBasedQualitySelector<SampleQualityLevel>(sampleQualityRuleData);
            var newMatcher =
                JsonUtility.FromJson<SampleDeviceNameRuleMatcher>(
                    @"{""rules"":[{""deviceModel"":""MacBookPro18,2"",""qualityLevel"":2}]}");
            sampleQualityLevelSelector.QualityLevelRuleMatchers.Add(newMatcher);
            if (sampleQualityLevelSelector.GetQualityLevel(stats, out var qualityLevel))
                Debug.Log($"QualityLevel: {qualityLevel}");
            else
                Debug.Log("QualityLevel: Unknown");
        }
    }
}
