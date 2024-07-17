// --------------------------------------------------------------
// Copyright 2024 CyberAgent, Inc.
// --------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.LowLevel;

namespace MobileSupport
{
    internal struct PlayerLoopModifier : IDisposable
    {
        private PlayerLoopSystem _root;

        private PlayerLoopModifier(in PlayerLoopSystem root)
        {
            _root = root;
        }

        public static PlayerLoopModifier Create()
        {
            return new PlayerLoopModifier(PlayerLoop.GetCurrentPlayerLoop());
        }

        public bool InsertBefore<T>(in PlayerLoopSystem subSystem) where T : struct
        {
            return InsertBefore<T>(subSystem, ref _root);
        }

        public void Dispose()
        {
            PlayerLoop.SetPlayerLoop(_root);
            _root = default;
        }

        private static bool InsertBefore<T>(in PlayerLoopSystem subSystem, ref PlayerLoopSystem parentSystem)
            where T : struct
        {
            var subSystems = parentSystem.subSystemList?.ToList();
            if (subSystems == default) return false;

            var found = false;
            for (var i = 0; i < subSystems.Count; i++)
            {
                var s = subSystems[i];
                if (s.type == typeof(T))
                {
                    found = true;
                    subSystems.Insert(i, subSystem);
                    break;
                }
            }

            if (!found)
                for (var i = 0; i < subSystems.Count; i++)
                {
                    var s = subSystems[i];
                    if (InsertBefore<T>(subSystem, ref s))
                    {
                        found = true;
                        subSystems[i] = s;
                        break;
                    }
                }

            if (found)
                parentSystem.subSystemList = subSystems.ToArray();

            return found;
        }
    }
}
