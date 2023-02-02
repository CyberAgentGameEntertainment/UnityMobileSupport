// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using MobileSupport;
using UnityEngine;

public class ThermalView : MonoBehaviour
{
    private void Start()
    {
#if UNITY_ANDROID
        Thermal.OnThermalStatusChanged += status => Debug.Log($"Thermal Status: {status}");
        Thermal.StartMonitoring();
#endif
#if UNITY_IOS
        Thermal.OnThermalStatusChanged += status => Debug.Log($"Thermal Status: {status}");
        Thermal.StartMonitoring();
#endif
    }
}
