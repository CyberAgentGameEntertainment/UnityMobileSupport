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
        private static extern long storage_getFreeDiskSpace();

        /// <summary>
        /// Get usable space of internal storage.
        /// </summary>
        /// <returns>Bytes of usable space. It will return -1 for internal error and in Unity editor.</returns>
        private static long GetInternalUsableSpace()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                return -1;
            }
#endif

            return storage_getFreeDiskSpace();
        }

    }
}

#endif
