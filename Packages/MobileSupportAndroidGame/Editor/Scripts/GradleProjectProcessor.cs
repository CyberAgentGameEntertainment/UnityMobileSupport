using System.IO;
using System.Xml;
using UnityEditor.Android;

namespace MobileSupport.AndroidGame.Editor
{
    public class GradleProjectProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var settings = AndroidGameEditorSettings.instance;
            if (!settings.enableAutomaticConfiguration) return;

            // set appCategory and game mode config
            ProcessAndroidManifest(path, settings.setGameAppCategoryGame);
            // write xml of game mode config
            WriteGameModeConfig(path, settings.supportsBatteryGameMode, settings.supportsPerformanceGameMode,
                settings.allowGameDownscaling, settings.allowGameFpsOverride);
        }

        private static void ProcessAndroidManifest(string path, bool enableGameApp)
        {
            var androidManifestPath = Path.Combine(path, "src/main/AndroidManifest.xml");
            // load xml
            var doc = new XmlDocument();
            doc.Load(androidManifestPath);
            if (!ProcessAndroidManifest(doc, enableGameApp)) return;

            // save xml
            doc.Save(androidManifestPath);
        }

        internal static bool ProcessAndroidManifest(XmlDocument doc, bool enableGameApp)
        {
            var manifest = doc.DocumentElement;
            var application = manifest.SelectSingleNode("application");
            if (application == null) return false;

            if (enableGameApp)
            {
                // create appCategory attribute
                var androidAppCategory = CreateAttribute(doc, "appCategory", "game");
                AddAttribute(application, androidAppCategory);
            }
            else
            {
                // remove appCategory attribute
                if (application.Attributes != null && application.Attributes["android:appCategory"] != null)
                    application.Attributes.Remove(application.Attributes["android:appCategory"]);
            }

            // create meta-data node for game mode config
            var metaData = doc.CreateElement("meta-data");
            var androidName = CreateAttribute(doc, "name", "android.game_mode_config");
            metaData.Attributes.Append(androidName);
            var androidResource = CreateAttribute(doc, "resource", "@xml/game_mode_config");
            metaData.Attributes.Append(androidResource);
            AddElement(application, metaData, androidName);
            return true;
        }

        private static XmlAttribute CreateAttribute(XmlDocument doc, string name, string value)
        {
            var attribute = doc.CreateAttribute(name, "http://schemas.android.com/apk/res/android");
            attribute.Value = value;
            return attribute;
        }

        private static void AddAttribute(XmlNode node, XmlAttribute attribute)
        {
            // check if attribute already exists
            if (node.Attributes != null && node.Attributes[attribute.Name] != null)
                node.Attributes[attribute.Name].Value = attribute.Value;
            else
                node.Attributes?.Append(attribute);
        }

        private static void AddElement(XmlNode node, XmlElement element, XmlAttribute nameAttribute)
        {
            // check if element already exists
            // select node with same attribute

            var prefix = node.GetPrefixOfNamespace(nameAttribute.NamespaceURI);
            var attributeName = string.IsNullOrEmpty(prefix) ? nameAttribute.Name : $"{prefix}:{nameAttribute.Name}";
            var xpath = @$"//{element.Name}[@{attributeName}='{nameAttribute.Value}']";

            var namespaceManager = new XmlNamespaceManager(node.OwnerDocument.NameTable);
            if (!string.IsNullOrEmpty(prefix))
                namespaceManager.AddNamespace(prefix, nameAttribute.NamespaceURI);

            var childNode = node.SelectSingleNode(xpath, namespaceManager);
            if (childNode != null) node.RemoveChild(childNode);

            node.AppendChild(element);
        }

        private static void WriteGameModeConfig(string path, bool supportsBatteryGameMode,
            bool supportsPerformanceGameModel, bool allowGameDownscaling, bool allowGameFpsOverride)
        {
            var xmlPath = Path.Combine(path, "src/main/res/xml/game_mode_config.xml");
            var dirName = Path.GetDirectoryName(xmlPath);
            if (dirName == null) return;
            Directory.CreateDirectory(dirName);

            var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<game-mode-config
    xmlns:android=""http://schemas.android.com/apk/res/android""
    android:supportsBatteryGameMode=""{supportsBatteryGameMode.ToString().ToLower()}""
    android:supportsPerformanceGameMode=""{supportsPerformanceGameModel.ToString().ToLower()}""
    android:allowGameDownscaling=""{allowGameDownscaling.ToString().ToLower()}""
    android:allowGameFpsOverride=""{allowGameFpsOverride.ToString().ToLower()}""
/>";
            File.WriteAllText(xmlPath, xml);
        }
    }
}
