// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Collections;
using System.Diagnostics;
using MobileSupport;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ThermalView : MonoBehaviour
{
    private void Start()
    {
#if UNITY_ANDROID
        Thermal.OnBatteryTemperatureChanged += value => Debug.Log($"Battery Temperature: {value}");
#endif

#if UNITY_ANDROID || UNITY_IOS
        Thermal.OnThermalStatusChanged += status => Debug.Log($"Thermal Status: {status}");
        Thermal.StartMonitoring();
#endif

#if UNITY_ANDROID
        StartCoroutine(GetTemperaturesLooped());
#endif
    }

    private IEnumerator GetTemperaturesLooped()
    {
        Stopwatch sw = new();

        while (this)
        {
            yield return new WaitForSeconds(1);

            sw.Restart();
            var headroom = Thermal.GetThermalHeadroom(0);
            var elapsed = sw.ElapsedMilliseconds;

            Debug.Log($"Thermal Headroom: {headroom} (obtained in {elapsed} ms)");
        }
    }
}
