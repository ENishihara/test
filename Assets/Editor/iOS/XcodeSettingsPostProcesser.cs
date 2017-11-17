using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;

public class PostprocessBuildPlayer
{
    [PostProcessBuildAttribute]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            string targetGUID = project.TargetGuidByName("Unity-iPhone");

            // Obj-Cの例外設定を有効にする（LUKeychainAccessで使用）
            project.SetBuildProperty(targetGUID, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");

            // Set Custom Link Flag
            project.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC -lz");

            File.WriteAllText(projectPath, project.WriteToString());
        }
    }
}

