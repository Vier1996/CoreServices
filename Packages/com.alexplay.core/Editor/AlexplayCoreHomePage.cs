using Constants;
using Sirenix.OdinInspector;
using UnityEditor.PackageManager;

namespace ACS.CoreEditor.Editor
{
    public class AlexplayCoreHomePage
    {
        [Title("Alexplay Core", "Made by 'Bochek potik' & team", TitleAlignment = TitleAlignments.Centered)]
        
        [Button(ButtonSizes.Large)]
        public void OpenFcknAwesomeDocs() => AlexplayEditor.OpenDocumentation();
        
        [Button] 
        private void UpdatePackage() => Client.Add(ACSConst.CorePackageURL);
    }
}
