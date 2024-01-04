// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace MobileSupport.QualityTuner
{
    public class RuleBasedQualitySelector<T>
    {
        public RuleBasedQualitySelector(T defaultQualityLevel, IEnumerable<IMatcher> qualityLevelRuleMatchers)
        {
            DefaultQualityLevel = defaultQualityLevel;
            QualityLevelRuleMatchers = new List<IMatcher>(qualityLevelRuleMatchers);
        }

        public RuleBasedQualitySelector(QualityRuleData<T> data) : this(data.defaultQualityLevel,
            data.qualityLevelRuleMatchers)
        {
        }

        public List<IMatcher> QualityLevelRuleMatchers { get; }

        public T DefaultQualityLevel { get; set; }

        public bool TryGetQualityLevel(HardwareStats stats, out T qualityLevel)
        {
            if (QualityLevelRuleMatchers is not null)
                foreach (var matcher in QualityLevelRuleMatchers)
                {
                    if (matcher == null)
                        continue;

                    if (matcher.TryMatch<T>(stats, out var matchedQualityLevel))
                    {
                        qualityLevel = matchedQualityLevel;
                        return true;
                    }
                }

            qualityLevel = DefaultQualityLevel;
            return false;
        }
    }

    public abstract class QualityRuleData<T> : ScriptableObject
    {
        [Tooltip("Default quality level if no match")]
        public T defaultQualityLevel;

        [Tooltip("Combined performance index table")]
        [SerializeReference]
        [SelectableSerializeReference]
        public IMatcher[] qualityLevelRuleMatchers; // cannot use IMatcher<T> because of SerializeReference
    }
}
