// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;

namespace MobileSupport.QualityMapper
{
    public abstract class QualityLevelSelector<T> : ScriptableObject
    {
        [Tooltip("Default quality level if no match")]
        public T defaultQualityLevel;

        [Tooltip("Combined performance index table")]
        [SerializeReference]
        [SelectableSerializeReference]
        public RuleMatcher[] qualityLevelRuleMatchers;

        public bool GetQualityLevel(HardwareStats stats, out T qualityLevel)
        {
#if UNITY_IOS || UNITY_ANDROID
            foreach (var matcher in qualityLevelRuleMatchers)
            {
                if (matcher == null)
                    continue;

                if (matcher.Match<T>(stats, out var matchedQualityLevel))
                {
                    qualityLevel = matchedQualityLevel;
                    return true;
                }
            }

            qualityLevel = defaultQualityLevel;
            return false;
#else
            return false;
#endif
        }
    }
}
