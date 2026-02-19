// --------------------------------------------------------------
// Copyright 2026 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
using UnityEngine;

namespace MobileSupport
{
    public static class Storage
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(
            string lpDirectoryName,
            out ulong lpFreeBytesAvailable,
            out ulong lpTotalNumberOfBytes,
            out ulong lpTotalNumberOfFreeBytes);

        /// <summary>
        ///     Get usable space of internal storage.
        /// </summary>
        /// <returns>Bytes of usable space. It will return -1 for internal error and in Unity editor.</returns>
        public static long GetInternalUsableSpace()
        {
#if UNITY_EDITOR
            if (Application.isEditor) return -1;
#endif

            if (GetDiskFreeSpaceEx(Application.persistentDataPath,
                    out var freeBytesAvailable, out _, out _))
                return (long)freeBytesAvailable;

            return -1;
        }
    }
}
#endif
