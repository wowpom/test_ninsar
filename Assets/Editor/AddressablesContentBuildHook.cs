using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Game.Editor
{
    public sealed class AddressablesContentBuildHook : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                throw new BuildFailedException(
                    "Нет настроек Addressables.\nWindow → Asset Management → Addressables → Groups — создайте их там.");
            }

            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);

            if (!string.IsNullOrEmpty(result.Error))
            {
                throw new BuildFailedException($"Addressables не собрались: {result.Error}");
            }

            Debug.Log($"Контент Addressables собран за {result.Duration:F1} с.");
        }
    }
}
