using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace MobileSupport.AndroidGame.Editor
{
    public class ProjectSettingProvider : SettingsProvider
    {
        private const string SettingPath = "Project/Mobile Support/Android Game";

        private UnityEditor.Editor _editor;

        public ProjectSettingProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(
            path, scopes, keywords)
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateProjectSettingsProvider()
        {
            var provider = new ProjectSettingProvider(SettingPath, SettingsScope.Project);
            return provider;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            var preferences = AndroidGameEditorSettings.instance;
            preferences.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;
            UnityEditor.Editor.CreateCachedEditor(preferences, null, ref _editor);
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.HelpBox(
                "These settings will automatically tweak AndroidManifest.xml and xml config for game mode.",
                MessageType.Info);
            EditorGUILayout.HelpBox(
                "Since setting AndroidManifest.xml is necessary, you should write by your self if you don't want to use this feature.",
                MessageType.Warning);

            EditorGUIUtility.labelWidth = 300;

            EditorGUI.BeginChangeCheck();
            _editor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck()) AndroidGameEditorSettings.instance.Save();
        }
    }
}
