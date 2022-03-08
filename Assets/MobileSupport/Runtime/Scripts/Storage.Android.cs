// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

#if UNITY_ANDROID
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MobileSupport
{
    public static class Storage
    {
        private static readonly string JavaClassName = "jp.co.cyberagent.unitysupport.Storage";

        /// <summary>
        ///     Get usable space of internal storage.
        /// </summary>
        /// <param name="isAccurate">set true for safe and accurate calculation, but pretty slow (for Android 8.0 or later)</param>
        /// <param name="wantSpace">delete caches for desired space (should also set isAccurate to true)</param>
        /// <returns>bytes of usable space</returns>
        public static long GetInternalUsableSpace(bool isAccurate = false, long wantSpace = -1)
        {
#if UNITY_EDITOR
            if (Application.isEditor) return -1;
#endif

            using var activity = GetActivity();
            using var storageClass = new AndroidJavaClass(JavaClassName);
            return storageClass.CallStatic<long>("getInternalUsableSpace", activity, isAccurate, wantSpace);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static AndroidJavaObject GetActivity()
        {
            using var playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");
            return activity;
        }
    }
}
#endif
