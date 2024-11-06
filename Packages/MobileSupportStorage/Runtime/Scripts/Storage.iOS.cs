// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine;

namespace MobileSupport
{
    public static partial class Storage
    {
        private const string DllName = "__Internal";

        [DllImport(DllName)]
        private static extern long storage_getFreeDiskSpace(bool includeDeletableCaches);

        /// <summary>
        /// Get usable space of internal storage.
        /// </summary>
        /// <param name="includeDeletableCaches">Set <c>false</c> to exclude size of deletable caches. To get value showed at System, set to <c>true</c>.</param>
        /// <returns>Bytes of usable space. It will return -1 for internal error and in Unity editor.</returns>
        public static long GetInternalUsableSpace(bool includeDeletableCaches = true)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                return -1;
            }
#endif

            return storage_getFreeDiskSpace(includeDeletableCaches);
        }

    }
}

#endif
