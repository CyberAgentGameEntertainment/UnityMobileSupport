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
        Thermal.Android.OnBatteryTemperatureChanged += value =>
        {
            Profiler.BeginSample("OnBatteryTemperatureChanged");
            Debug.Log($"Battery Temperature: {value}");
            Profiler.EndSample();
        };

        Thermal.Android.OnBatteryVoltageChanged += voltage =>
        {
            Profiler.BeginSample("OnBatteryVoltageChanged");
            Debug.Log($"Battery Voltage: {voltage} mV");
            Profiler.EndSample();
        };
#endif

#if UNITY_ANDROID || UNITY_IOS

        Thermal.OnBatteryLevelChanged += level => { Debug.Log($"Battery Level: {level}"); };

        Thermal.OnBatteryStatusChanged += status => { Debug.Log($"Battery Status: {status}"); };

        Thermal.OnThermalStatusChanged += status =>
        {
            Debug.Log($"Thermal Status: {status}");
        };
        Thermal.StartMonitoring();
#endif

#if UNITY_ANDROID
        StartCoroutine(GetValuesLooped());
#endif
    }

#if UNITY_ANDROID
    private IEnumerator GetValuesLooped()
    {
        Stopwatch sw = new();

        double elapsed;
        while (this)
        {
            yield return new WaitForSeconds(0.5f);

            sw.Restart();
            Profiler.BeginSample("Android.GetThermalHeadroom");
            Thermal.Android.GetThermalHeadroom(0, out var headroom, out var resultForecastSeconds,
                out var isLatestValue);
            Profiler.EndSample();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            Debug.Log(
                $"Thermal Headroom: {headroom}, isLatestValue: {isLatestValue}, resultForecastSeconds: {resultForecastSeconds} (obtained in {elapsed} ms)");

            sw.Restart();
            var counter = Thermal.Android.GetBatteryEnergyCounter();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            Debug.Log(
                $"Energy Counter: {counter} (obtained in {elapsed} ms)");

            sw.Restart();
            var currentNow = Thermal.Android.GetBatteryCurrentNow();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            Debug.Log(
                $"Current Now: {currentNow} (obtained in {elapsed} ms)");

            sw.Restart();
            var capacity = Thermal.Android.GetBatteryCapacity();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            Debug.Log(
                $"Capacity: {capacity} (obtained in {elapsed} ms)");

            sw.Restart();
            var chargeCounter = Thermal.Android.GetBatteryChargeCounter();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            Debug.Log(
                $"Charge Counter: {chargeCounter} (obtained in {elapsed} ms)");

            sw.Restart();
            var currentAverage = Thermal.Android.GetBatteryCurrentAverage();
            elapsed = (double)sw.ElapsedTicks / Stopwatch.Frequency * 1000;

            Debug.Log(
                $"Current Average: {currentAverage} (obtained in {elapsed} ms)");
        }
    }
#endif
}
