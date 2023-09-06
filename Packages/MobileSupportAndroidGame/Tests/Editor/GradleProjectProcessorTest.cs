using System.IO;
using System.Xml;
using NUnit.Framework;

namespace MobileSupport.AndroidGame.Editor.Tests
{
    public class GradleProjectProcessorTest
    {
        // generate test for ProcessAndroidManifest
        [Test]
        [TestCase(@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
    <application>
    </application>
</manifest>",
            true,
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
  <application android:appCategory=""game"">
    <meta-data android:name=""android.game_mode_config"" android:resource=""@xml/game_mode_config"" />
  </application>
</manifest>")]
        [TestCase(@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
    <application>
    </application>
</manifest>",
            false,
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
  <application>
    <meta-data android:name=""android.game_mode_config"" android:resource=""@xml/game_mode_config"" />
  </application>
</manifest>")]
        [TestCase(@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
</manifest>",
            true,
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
</manifest>")]
        [TestCase(@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
</manifest>",
            false,
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
</manifest>")]
        // erase test
        [TestCase(@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
    <application android:appCategory=""game"">
    </application>
</manifest>",
            false,
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
  <application>
    <meta-data android:name=""android.game_mode_config"" android:resource=""@xml/game_mode_config"" />
  </application>
</manifest>")]
        // overwrite test
        [TestCase(@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
    <application android:appCategory=""game"">
        <meta-data android:name=""android.game_mode_config"" android:resource=""@xml/foo"" />
    </application>
</manifest>",
            true,
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
  <application android:appCategory=""game"">
    <meta-data android:name=""android.game_mode_config"" android:resource=""@xml/game_mode_config"" />
  </application>
</manifest>")]
        [TestCase(@"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
    <application android:appCategory=""game"">
        <meta-data android:name=""android.game_mode_config"" android:resource=""@xml/foo"" />
        <meta-data android:name=""android.game_mode_config"" android:resource=""@xml/bar"" />
    </application>
</manifest>",
            true,
            @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
  <application android:appCategory=""game"">
    <meta-data android:name=""android.game_mode_config"" android:resource=""@xml/game_mode_config"" />
  </application>
</manifest>")]
        public void ProcessAndroidManifestTest(string xml, bool enableGameApp, string expected)
        {
            var testAndroidManifest = new XmlDocument();
            testAndroidManifest.LoadXml(xml);

            GradleProjectProcessor.ProcessAndroidManifest(testAndroidManifest, enableGameApp);

            var actual = XmlDocumentToString(testAndroidManifest);
            Assert.AreEqual(expected, actual);
        }

        private static string XmlDocumentToString(XmlDocument doc)
        {
            using var writer = new StringWriter();
            using var xmlWriter = new XmlTextWriter(writer);
            xmlWriter.Formatting = Formatting.Indented;
            doc.WriteTo(xmlWriter);
            xmlWriter.Flush();
            return writer.ToString();
        }
    }
}
