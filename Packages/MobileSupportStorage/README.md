# Unity Mobile Support - Storage

[![license](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![license](https://img.shields.io/badge/PR-welcome-green.svg)](https://github.com/CyberAgentGameEntertainment/UnityMobileSupport/pulls)
[![license](https://img.shields.io/badge/Unity-2019.4-green.svg)](#Requirements)


:warning: This project is under hard construction. :warning:
They may be destructive change with future development.

To support iOS/Android native features in Unity, this will provide variety functions.
This package provide functions to interact with storage.

## Table of Contents

<details>
<summary>Details</summary>

- [Supported Functions](#supported-functions)
  - [Storage](#storage)
- [Setup](#setup)
  - [Requirements](#requirements)
  - [Install](#install)
- [Licenses](#licenses)

</details>

## Supported Functions

| Method | Description | Note | Editor Behaviour |
|--------|-------------|------|------------------|
| GetInternalUsableSpace | Get usable space of internal storage | "Usable" means to consider deletable caches and system's stability | -1 |

## Setup

### Requirements
This library is compatible with the following environments.

- Unity 2019.4 or higher

### Install

To install the software, follow the steps below.

1. Open the Package Manager from `Window > Package Manager`
2. `"+" button > Add package from git URL`
3. Enter the following
   * https://github.com/CyberAgentGameEntertainment/UnityMobileSupport.git?path=/Packages/MobileSupportStorage

<p align="center">
  <img width=500 src="https://user-images.githubusercontent.com/47441314/143533003-177a51fc-3d11-4784-b9d2-d343cc622841.png" alt="Package Manager">
</p>

Or, open `Packages/manifest.json` and add the following to the dependencies block.

```json
{
    "dependencies": {
        "jp.co.cyberagent.unity-mobile-support-storage": "https://github.com/CyberAgentGameEntertainment/UnityMobileSupport.git?path=/Packages/MobileSupportStorage"
    }
}
```

If you want to set the target version, write as follows.

- https://github.com/CyberAgentGameEntertainment/UnityMobileSupport.git?path=/Packages/MobileSupportStorage#0.1.0

Note that if you get a message like `No 'git' executable was found. Please install Git on your system and restart Unity`, you will need to set up Git on your machine.

To update the version, rewrite the version as described above.
If you don't want to specify a version, you can also update the version by editing the hash of this library in the package-lock.json file.

```json
{
  "dependencies": {
      "jp.co.cyberagent.unity-mobile-support-storage": {
      "version": "https://github.com/CyberAgentGameEntertainment/UnityMobileSupport.git?path=/Packages/MobileSupportStorage",
      "depth": 0,
      "source": "git",
      "dependencies": {},
      "hash": "..."
    }
  }
}
```

## Licenses
This software is released under the MIT license.
You are free to use it within the scope of the license, but the following copyright and license notices are required.

* [LICENSE](LICENSE)

