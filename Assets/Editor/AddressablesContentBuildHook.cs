using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

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
                throw new BuildFailedException("Нет настроек Addressables");
            }

            if (settings.BuildAddressablesWithPlayerBuild  == AddressableAssetSettings.PlayerBuildOption.DoNotBuildWithPlayer)
            {
                throw new BuildFailedException(
                    "Addressables не соберутся вместе с плеером.");
            }
        }
    }
}
