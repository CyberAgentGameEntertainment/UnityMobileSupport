using UnityEditor;
using UnityEngine;

namespace MobileSupport.AndroidGame.Editor
{
    [FilePath("ProjectSettings/MobileSupport_AndroidGame.asset",
        FilePathAttribute.Location.ProjectFolder)]
    internal class AndroidGameEditorSettings : ScriptableSingleton<AndroidGameEditorSettings>
    {
        [Header("Opt-in")]
        [Tooltip("Enable automatic configuration of manifest when building Android app")]
        public bool enableAutomaticConfiguration = true;

        [Header("AndroidManifest.xml")]
        [Tooltip("Set app category to game, which is required for game mode")]
        public bool setGameAppCategoryGame = true;

        [Header("Game mode config")]
        [Tooltip("Enable battery mode")]
        public bool supportsBatteryGameMode = true;

        [Tooltip("Enable performance mode")]
        public bool supportsPerformanceGameMode = true;

        [Tooltip("Allow downscaling")]
        public bool allowGameDownscaling;

        [Tooltip("Allow FPS override")]
        public bool allowGameFpsOverride;

        public void Save()
        {
            Save(true);
        }
    }
}
