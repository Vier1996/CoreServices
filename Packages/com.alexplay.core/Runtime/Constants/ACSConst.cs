using UnityEngine;

namespace Constants
{
    public static class ACSConst
    {
        public const string UnityMenuPath = "Tools/Alexplay/";
        public const string CoreObjectName = "CoreBootstrapper";
        public const string AssetsFolderName = "Assets";
        public const string ResourcesFolderName = "Resources";
        public const string ConfigsFolderName = "Alexplay";
        public const string ConfigsPath = AssetsFolderName + "/" + ResourcesFolderName + "/" + ConfigsFolderName;
        public const string AssetMenuRootName = "ACS Configs";
        public const string ConfigName = "ACS Config";
        
        public static readonly string AssetSignature = "ACS Config.asset";
        public static readonly string ResourcesFolder = "Assets/Resources";
        public static readonly string AbsoluteResourcesFolder = Application.dataPath + ResourcesFolder.Replace("Assets", "");
        public static readonly string SourcePath = ResourcesFolder + "/" + AssetSignature;
        
        public const string CorePackageAssetPath = "Packages/com.alexplay.core";
        public const string CorePackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.core";
       
        public const string AdsPackageAssetPath = "Packages/com.alexplay.advertisement";
        public const string AudioPackageAssetPath = "Packages/com.alexplay.audio";
        public const string DataPackageAssetPath = "Packages/com.alexplay.data";
        public const string DialogPackageAssetPath = "Packages/com.alexplay.dialog";
        public const string GDPRPackageAssetPath = "Packages/com.alexplay.gdpr";
        public const string ObjectPoolPackageAssetPath = "Packages/com.alexplay.object-pool";
        public const string SignalBusPackageAssetPath = "Packages/com.alexplay.signal_bus";
        public const string PurchasePackageAssetPath = "Packages/com.alexplay.iap";
        public const string AnalyticsPackageAssetPath = "Packages/com.alexplay.analytics";
        public const string FBRCPackageAssetPath = "Packages/com.alexplay.fbrc";
        public const string CheatTrackerPackageAssetPath = "Packages/com.alexplay.cheat_tracker";
    }
}