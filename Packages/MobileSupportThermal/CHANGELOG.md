# Release notes

## v2.0.0 - 2024/07/11

- New Features :rocket:
    - Thermal: New apis to get battery voltage and power consumption on Android
- Changed
    - Thermal: Android-specific APIs are moved to nested class `Thermal.Android`
    - Thermal: The type of `OnThermalStatusChanged` and `LatestThermalStatus` changed to platform-specific types `ThermalStatusIOS` `ThermalStatusAndroid`

## v1.0.0 - 2024/06/26

- New Features :rocket:
    - Thermal: New apis to get battery temperature and thermal headroom on Android

## v0.1.0 - 2023/02/10

- New Features :rocket:
  - Thermal: New feature to get thermal status of mobile 
