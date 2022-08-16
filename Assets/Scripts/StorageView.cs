// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using MobileSupport;
using UnityEngine;

public class StorageView : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
#if UNITY_ANDROID
        Debug.LogFormat("Available disk space: {0}", Storage.GetInternalUsableSpace());
        Debug.LogFormat("Available disk space (accurate): {0}", Storage.GetInternalUsableSpace(true));
#endif
#if UNITY_IOS
        Debug.LogFormat("Available disk space: {0}", Storage.GetInternalUsableSpace(false));
        Debug.LogFormat("Available disk space (includeDeletableCaches): {0}", Storage.GetInternalUsableSpace(true));
#endif
    }
}
