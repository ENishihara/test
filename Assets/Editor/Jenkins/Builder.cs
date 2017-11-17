using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Over
{
    /// <summary>
    /// ジェンキンスビルド用スクリプト
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Android用ビルドスクリプト(デバッグ用ビルド)
        /// </summary>
        [UnityEditor.MenuItem("Tools/Build Project AllScene Android")]
        public static void BuildProjectAllSceneAndroid()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
            List<string> allScene = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    allScene.Add(scene.path);
                }
            }

            BuildOptions opt = BuildOptions.AllowDebugging |
                      BuildOptions.ConnectWithProfiler |
                      BuildOptions.Development;
            
            // http://tkhsken.hatenablog.com/entry/2016/05/23/075832
            // androidにはハイフンが使えない
            PlayerSettings.applicationIdentifier = "jp.co.rforce.over";
            PlayerSettings.statusBarHidden = true;
            BuildPipeline.BuildPlayer(
                allScene.ToArray(),
                "./android/overViewer.apk",
                BuildTarget.Android,
                opt
            );
        }

        /// <summary>
        /// iOS用ビルドスクリプト(デバッグ用ビルド)
        /// </summary>
        [UnityEditor.MenuItem("Tools/Build Project AllScene iOS")]
        public static void BuildProjectAllSceneiOS()
        {  
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
            List<string> allScene = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    allScene.Add(scene.path);
                }
            }

            BuildOptions opt = BuildOptions.SymlinkLibraries |
                      BuildOptions.AllowDebugging |
                      BuildOptions.ConnectWithProfiler |
                      BuildOptions.Development;

            //BUILD for Device
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
            PlayerSettings.applicationIdentifier = "jp.co.rforce.over";
            PlayerSettings.statusBarHidden = true;
            string errorMsg_Device = BuildPipeline.BuildPlayer(
                                allScene.ToArray(),
                                "iOS",
                                BuildTarget.iOS,
                                opt
                            );

            if (string.IsNullOrEmpty(errorMsg_Device))
            {
            }
            else
            {
                // エラー処理
            }
        }
    }
}
