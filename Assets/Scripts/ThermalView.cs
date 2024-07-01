// --------------------------------------------------------------
// Copyright 2023 CyberAgent, Inc.
// --------------------------------------------------------------

using System.Collections;
using System.Diagnostics;
using MobileSupport;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public class ThermalView : MonoBehaviour
{
    private void Start()
    {
#if UNITY_ANDROID
        Thermal.OnBatteryTemperatureChanged += value =>
        {
            Profiler.BeginSample("OnBatteryTemperatureChanged");
            Debug.Log($"Battery Temperature: {value}");
            Profiler.EndSample();
        };

        Thermal.OnBatteryLevelChanged += levelPercentage =>
        {
            Profiler.BeginSample("OnBatteryLevelChanged");
            Debug.Log($"Battery Level: {levelPercentage}");
            Profiler.EndSample();
        };

        Thermal.OnBatteryStatusChanged += status =>
        {
            Profiler.BeginSample("OnBatteryStatusChanged");
            Debug.Log($"Battery Level: {status}");
            Profiler.EndSample();
        };

        Thermal.OnBatteryVoltageChanged += voltage =>
        {
            Profiler.BeginSample("OnBatteryVoltageChanged");
            Debug.Log($"Battery Voltage: {voltage} mV");
            Profiler.EndSample();
        };
#endif

#if UNITY_ANDROID || UNITY_IOS
        Thermal.OnThermalStatusChanged += status =>
        {
            Profiler.BeginSample("OnThermalStatusChanged");
            Debug.Log($"Thermal Status: {status}");
            Profiler.EndSample();
        };
        Thermal.StartMonitoring();
#endif

        StartCoroutine(GetValuesLooped());
    }

    private IEnumerator GetValuesLooped()
    {
        Stopwatch sw = new();

        double elapsed;
        while (this)
        {
            yield return new WaitForSeconds(0.5f);

#if UNITY_ANDROID
            sw.Restart();
            Profiler.BeginSample("GetThermalHeadroom");
            Thermal.GetThermalHeadroom(0, out var headroom, out var resultForecastSeconds, out var isLatestValue);
            Profiler.EndSample();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            Debug.Log(
                $"Thermal Headroom: {headroom}, isLatestValue: {isLatestValue}, resultForecastSeconds: {resultForecastSeconds} (obtained in {elapsed} ms)");
#endif

            sw.Restart();
            var counter = Thermal.GetEnergyCounter();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            sw.Restart();
            var currentNow = Thermal.GetCurrentNow();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            sw.Restart();
            var capacity = Thermal.GetCapacity();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            sw.Restart();
            var chargeCounter = Thermal.GetChargeCounter();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            sw.Restart();
            var currentAverage = Thermal.GetCurrentAverage();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            Debug.Log(
                $"Energy Counter: {counter}, Current Now: {currentNow}, Capacity: {capacity}, Charge Counter: {chargeCounter}, Current Average: {currentAverage} (obtained in {elapsed} ms)");
        }
    }
}
