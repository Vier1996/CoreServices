using Constants;
using UnityEditor;
using UnityEngine;

namespace ACS.Core
{
    public static class CoreConfigProvider
    {
        public static CoreConfig GetConfig()
        {
            var foundConfig = Resources.Load<CoreConfig>(ACSConst.ConfigsPath);
#if UNITY_EDITOR
            return foundConfig != null ? foundConfig : CreateConfig();
#endif
            return foundConfig;
        }

        private static CoreConfig CreateConfig()
        {
            var newConfig = ScriptableObject.CreateInstance<CoreConfig>();
            newConfig.name = nameof(CoreConfig);
            if (AssetDatabase.IsValidFolder(ACSConst.ConfigsPath) == false)
                CreateConfigsFolder();

            AssetDatabase.CreateAsset(newConfig, $"{ACSConst.ConfigsPath}/{newConfig.name}.asset");
            AssetDatabase.SaveAssets();
            return newConfig;
        }

        private static void CreateConfigsFolder()
        {
            if (AssetDatabase.IsValidFolder($"{ACSConst.AssetsFolderName}/{ACSConst.ResourcesFolderName}") == false)
                AssetDatabase.CreateFolder(ACSConst.AssetsFolderName, ACSConst.ResourcesFolderName);
            if (AssetDatabase.IsValidFolder(ACSConst.ConfigsPath) == false)
                AssetDatabase.CreateFolder($"{ACSConst.AssetsFolderName}/{ACSConst.ResourcesFolderName}", ACSConst.ConfigsFolderName);
        }
    }
}