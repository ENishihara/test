using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Over.Unity.Importer
{
    /// <summary>
    /// OVER専用FbxImporter
    /// </summary>
    public class FbxImporter : AssetPostprocessor
    {
        const string targetExtension = ".fbx";
        const string invalidUpperExtension = ".FBX";

        /// <summary>
        /// fbxに対して設定した処理を実行する
        /// </summary>
        private void OnPreprocessModel()
        {
            if (!IsValidExtension())
            {
                return;
            }
			
            var importSettings = LoadImportSettings();
			
            if (importSettings == null || importSettings.propertyList == null || importSettings.propertyList.Count == 0)
            {
                Debug.LogWarning("FbxImportSettings is empty.");
                return;
            }

            // Model用設定がない場合は終了
            var modelSettings = importSettings.propertyList.Where(x => x.fbxType == FbxType.Model).ToList();
            if (modelSettings.Count == 0)
            {
                return;
            }

            foreach (FbxImportProperty prop in modelSettings)
            {
                var checkFunc = SelectCheckFunc(prop);
                if (checkFunc(prop.targetPaths, assetPath))
                {
                    var modelImporter = assetImporter as ModelImporter;
                    modelImporter.importAnimation = prop.importAnimation;
                    modelImporter.materialSearch = prop.materialSearch;
                }
            }
        }

        /// <summary>
        /// アニメーションに設定した処理の実行
        /// </summary>
        private void OnPreprocessAnimation()
        {
            if (!IsValidExtension())
            {
                return;
            }

            var importSettings = LoadImportSettings();

            if (importSettings == null || importSettings.propertyList == null || importSettings.propertyList.Count == 0)
            {
                Debug.LogWarning("FbxImportSettings is empty.");
                return;
            }

            // Motion用設定がない場合は終了
            var motionSetting = importSettings.propertyList.Where(x => x.fbxType == FbxType.Motion).ToList();
            if (motionSetting.Count == 0)
            {
                return;
            }

            foreach (FbxImportProperty prop in motionSetting)
            {
                var checkFunc = SelectCheckFunc(prop);
                if (checkFunc(prop.targetPaths, assetPath))
                {
                    var modelImporter = assetImporter as ModelImporter;
                    modelImporter.animationCompression = prop.animationCompression;
                    if (prop.animationCompression == ModelImporterAnimationCompression.KeyframeReduction)
                    {
                        modelImporter.animationPositionError = prop.positionError;
                        modelImporter.animationRotationError = prop.rotationError;
                        modelImporter.animationScaleError = prop.scaleError;
                    }
                }
            }

        }

        /// <summary>
        /// モデルに設定した処理の実行
        /// </summary>
        private void OnPostprocessModel(GameObject gameObject)
        {
            if (!IsValidExtension())
            {
                return;
            }

            var importSettings = LoadImportSettings();

            if (importSettings == null || importSettings.propertyList == null || importSettings.propertyList.Count == 0)
            {
                return;
            }

            // AnimationTypeの変更フラグが有効なものがない場合は終了
            var changeAnimationTypeList = importSettings.propertyList.Where(x => x.changeAnimationType).ToList();
            if (changeAnimationTypeList.Count == 0)
            {
                return;
            }

            foreach (FbxImportProperty prop in changeAnimationTypeList)
            {
                var checkFunc = SelectCheckFunc(prop);
                if (!checkFunc(prop.targetPaths, assetPath))
                {
                    return;
                }
                bool isTarget = false;
                foreach (Transform child in gameObject.transform)
                {
                    if (child.name.ToLower().StartsWith(prop.changeAnimationTypeIdentifier))
                    {
                        isTarget = true;
                        break;
                    }
                }
                if (isTarget)
                {
                    var modelImporter = assetImporter as ModelImporter;
                    if (!modelImporter.animationType.Equals(prop.animationType))
                    {
                        modelImporter.animationType = prop.animationType;
                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// 全てのアセットに対して設定した処理の実行
        /// </summary>
        static void OnPostprocessAllAssets(string[] importedAssets,
                                           string[] deletedAssets,
                                           string[] movedAssets,
                                           string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                // 拡張子が大文字のFBXファイルがあれば拡張子を小文字に修正する
                if (Path.GetExtension(path) == invalidUpperExtension)
                {
                    string dirName = Path.GetDirectoryName(path);
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    string newPath = Path.Combine(dirName, fileName + targetExtension);
                    AssetDatabase.MoveAsset(path, newPath);
                }
            }
        }

        /// <summary>
        /// 拡張したものかどうか
        /// </summary>
        private bool IsValidExtension()
        {
            return Path.GetExtension(assetPath).ToLower() == targetExtension;
        }

#region checkFunc
        /// <summary>
        /// ターゲットと同じディレクトリかどうか
        /// </summary>
        private bool ImportEqualsDir(string[] paths, string target)
        {
            if (paths == null || paths.Length == 0)
            {
                return false;
            }

            var targetPath = Path.GetDirectoryName(assetPath);
            foreach (string path in paths)
            {
                if (targetPath.Equals(path))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// ターゲットと一致するディレクトリかどうか
        /// </summary>
        private bool ImportContainsDir(string[] paths, string target)
        {
            if (paths == null || paths.Length == 0)
            {
                return false;
            }

            var targetPath = Path.GetDirectoryName(assetPath);
            foreach (string path in paths)
            {
                if (targetPath.Contains(path))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// ターゲットと一致する名前かどうか
        /// </summary>
        private bool ImportContainsName(string[] paths, string target)
        {
            if (paths == null || paths.Length == 0)
            {
                return false;
            }

            var targetFile = Path.GetFileNameWithoutExtension(assetPath);
            foreach (string path in paths)
            {
                if (targetFile.Contains(path))
                    return true;
            }

            return false;
        }
#endregion

        /// <summary>
        /// 指定されたFBXImportTargetによってチェック関数を切り替える
        /// </summary>
        private System.Func<string[], string, bool> SelectCheckFunc(FbxImportProperty prop)
        {
            // 指定されたFBXImportTargetによってチェック関数を切り替える
            System.Func<string[], string, bool> checkFunc;
            switch (prop.importTarget)
            {
                case FbxImportTarget.All:
                    checkFunc = (paths, target) => true;
                    break;
                case FbxImportTarget.EqualsDir:
                    checkFunc = ImportEqualsDir;
                    break;
                case FbxImportTarget.ContainsDir:
                    checkFunc = ImportContainsDir;
                    break;
                case FbxImportTarget.ContainsName:
                    checkFunc = ImportContainsName;
                    break;
                default:
                    checkFunc = (paths, target) => false;
                    break;
            }
            return checkFunc;
        }

        /// <summary>
        /// インポーター設定をロードする
        /// </summary>
        private FbxImporterSettings LoadImportSettings()
        {
            string[] guids = AssetDatabase.FindAssets("t:FbxImporterSettings");
            if (guids == null | guids.Length == 0)
            {
                return null;
            }
			
            if (guids.Length != 1)
            {
                Debug.LogError("Too many FbxImporterSettings.");
                return null;
            }
			
            return AssetDatabase.LoadAssetAtPath<FbxImporterSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
	
}