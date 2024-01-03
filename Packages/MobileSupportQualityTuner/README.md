# Unity Mobile Support - QualityTuner <!-- omit in toc -->

[![license](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![license](https://img.shields.io/badge/PR-welcome-green.svg)](https://github.com/CyberAgentGameEntertainment/UnityMobileSupport/pulls)
[![license](https://img.shields.io/badge/Unity-2019.4-green.svg)](#Requirements)

It is often hard to decide quality level of game, since performance level of devices are various.
This package provide information and decision tool to decide quality level.

## Table of Contents <!-- omit in toc -->

<details>
<summary>Details</summary>

- [Supported Functions](#supported-functions)
- [Usage](#usage)
    - [Sample](#sample)
- [Setup](#setup)
    - [Requirements](#requirements)
    - [Install](#install)
- [Licenses](#licenses)

</details>

## Supported Functions

| Method                                                                               | Description                                             | Note                                                        | Editor Behaviour                                                           |
|--------------------------------------------------------------------------------------|---------------------------------------------------------|-------------------------------------------------------------|----------------------------------------------------------------------------|
| HardwareInfo.GetHardwareStats()                                                      | Get hardware stats which helps to decide quality level. | Many information is retrieved via `UnityEngine.SystemInfo`. | Same as mobile, but Windows Editor is unlikely not supported at this time. |
| RuleBasedQualitySelector<T>.GetQualityLevel(HardwareStats stats, out T qualityLevel) | Decide quality level by decision table you defined.     | You need to define your own QualityLevelSelector.           | Same as mobile.                                                            |

## Usage

1. Create your own QualityRuleData by inheriting `QualityRuleData<T>` class. T is your own quality
   level enum, integer, or something else you want to use for type of quality level.
2. Create asset of your own QualityRuleData on editor.
3. Set decision rules to asset as you like.
4. Instantiate RuleBasedQualitySelector<T> with your QualityRuleData. T should be same as your QualityRuleData.
5. Get HardwareStats and pass to RuleBasedQualitySelector<T>.GetQualityLevel() to decide quality level at runtime.

### Sample

[SampleQualityRuleData.cs](../../Assets/Scripts/SampleQualityRuleData.cs)

```csharp
using System.Text;
using MobileSupport;
using UnityEngine;

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
            // you can add new rule matcher at runtime, like deserializing from json
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
```

![Sample Settings](Documentation~/images/sample_settings.png)

### Customization

#### Creating your own rules

You can create your own rules by implementing `IMatcher` interface.
After you create it, you can add it to `QualityLevelRuleMatchers` of `RuleBasedQualitySelector<T>`.

## Setup

### Requirements

This library is compatible with the following environments.

- Unity 2021.3 or higher

### Install

To install the software, follow the steps below.

1. Open the Package Manager from `Window > Package Manager`
2. `"+" button > Add package from git URL`
3. Enter the following
    * https://github.com/CyberAgentGameEntertainment/UnityMobileSupport.git?path=/Packages/MobileSupportQualityTuner

<p align="center">
  <img width=500 src="https://user-images.githubusercontent.com/47441314/143533003-177a51fc-3d11-4784-b9d2-d343cc622841.png" alt="Package Manager">
</p>

Or, open `Packages/manifest.json` and add the following to the dependencies block.

```json
{
    "dependencies": {
        "jp.co.cyberagent.unity-mobile-support-quality-tuner": "https://github.com/CyberAgentGameEntertainment/UnityMobileSupport.git?path=/Packages/MobileSupportQualityTuner"
    }
}
```

If you want to set the target version, write as follows.

- https://github.com/CyberAgentGameEntertainment/UnityMobileSupport.git?path=/Packages/MobileSupportQualityTuner#qualitytuner/1.0.0

Note that if you get a message
like `No 'git' executable was found. Please install Git on your system and restart Unity`, you will need to set up Git
on your machine.

To update the version, rewrite the version as described above.
If you don't want to specify a version, you can also update the version by editing the hash of this library in the
package-lock.json file.

```json
{
  "dependencies": {
      "jp.co.cyberagent.unity-mobile-support-quality-tuner": {
      "version": "https://github.com/CyberAgentGameEntertainment/UnityMobileSupport.git?path=/Packages/MobileSupportQualityTuner",
      "depth": 0,
      "source": "git",
      "dependencies": {},
      "hash": "..."
    }
  }
}
```

## Licenses

This software is released under the MIT license.
You are free to use it within the scope of the license, but the following copyright and license notices are required.

* [LICENSE](LICENSE)

