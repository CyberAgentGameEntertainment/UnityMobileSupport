# Release notes

## v1.0.1 - 2025/10/14

- Bug Fixes :bug:
  - Android: updated `.androidlib` to make it compatible with Unity 6

## v1.0.0 - 2024/11/06

- Behaviour changes :warning:
  - iOS: includeDeletableCaches parameter of `GetInternalUsableSpace()` was previously set to `false` by default, but now set to `true` by default

## v0.2.0 - 2022/08/16

- New Features :rocket:
  - iOS: Added option to get free space with deletable caches

## v0.1.2 - 2022/08/10

- Bug Fixes :bug:
  - Downgraded Android library target api level to 30

## v0.1.1 - 2022/08/09

- Bug Fixes :bug:
  - Storage: Changed `GetInternalUsableSpace()` in iOS to public (accidently set to private)

## v0.1.0 - 2022/07/27

- New Features :rocket:
  - Storage: New function to get usable space of mobile storage 
