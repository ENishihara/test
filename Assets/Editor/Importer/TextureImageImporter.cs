using UnityEngine;
using UnityEditor;
using System.IO;

namespace Over.Unity.Importer
{
    /// <summary>
    /// Texture image importer.
    /// </summary>
    public class TextureImageImporter : AssetPostprocessor
    {
        static readonly string[] targetExtensions = { ".tga", ".png" };

        /// <summary>
        /// textureに対して設定した処理を実行する
        /// </summary>
        private void OnPreprocessTexture()
        {
            bool isValidExtension = false;
            foreach (var extension in targetExtensions)
            {
                if (Path.GetExtension(assetPath).ToLower().Equals(extension))
                {
                    isValidExtension = true;
                    break;
                }
            }

            if (!isValidExtension)
            {
                return;
            }

            var settings = LoadImporterSettings();

            if (settings == null || settings.propertyList == null || settings.propertyList.Count == 0)
            {
                Debug.LogWarning("TextureImageImporterSettings is empty.");
                return;
            }

            foreach (TextureImageImportProperty prop in settings.propertyList)
            {
                // 指定されたImportTargetによってチェック関数を切り替える
                System.Func<string[], string, bool> checkFunc;
                switch (prop.importTarget)
                {
                    case TextureImageImportTarget.All:
                        checkFunc = (paths, target) => true;
                        break;
                    case TextureImageImportTarget.EqualsDir:
                        checkFunc = ImportEqualsDir;
                        break;
                    case TextureImageImportTarget.ContainsDir:
                        checkFunc = ImportContainsDir;
                        break;
                    case TextureImageImportTarget.ContainsName:
                        checkFunc = ImportContainsName;
                        break;
                    default:
                        checkFunc = (paths, target) => false;
                        break;
                }

                if (checkFunc(prop.targetPaths, assetPath))
                {
                    var importer = assetImporter as TextureImporter;
                    importer.textureType = prop.textureType;
                    importer.npotScale = prop.npotScale;
                    importer.alphaIsTransparency = prop.alphaIsTransparent;
                    importer.mipmapEnabled = prop.mipmapEnabled;
                    importer.wrapMode = prop.wrapMode;
                }
            }
        }

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
                {
                    return true;
                }
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
                {
                    return true;
                }
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
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// インポーター設定をロードする
        /// </summary>
        private TextureImageImporterSettings LoadImporterSettings()
        {
            string[] guids = AssetDatabase.FindAssets("t:TextureImageImporterSettings");
            if (guids == null | guids.Length == 0)
            {
                return null;
            }

            if (guids.Length != 1)
            {
                Debug.LogError("Too many TextureImageImporterSettings.");
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<TextureImageImporterSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
    }
}
