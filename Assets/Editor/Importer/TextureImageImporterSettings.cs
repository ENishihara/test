using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Over.Unity.Importer
{
    /// <summary>
    /// テクスチャのインポート設定を管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "TextureImageImporterSettings", menuName = "Tools/TextureImageImporterSettings", order = 1001)]
    public class TextureImageImporterSettings : ScriptableObject
    {
        public List<TextureImageImportProperty> propertyList;
    }

    public enum TextureImageImportTarget
    {
        // 全対象
        All,
        // ディレクトリが合致
        EqualsDir,
        // ディレクトリ名に含む
        ContainsDir,
        // ファイル名に含む
        ContainsName
    }

    /// <summary>
    /// 設定対象とするディレクトリ及び、そのディレクトリで有効な設定を管理
    /// </summary>
    [System.Serializable]
    public class TextureImageImportProperty
    {
        public TextureImageImportTarget importTarget;
        public string[] targetPaths;
        public TextureImporterType textureType;
        public TextureImporterNPOTScale npotScale;
        public bool alphaIsTransparent;
        public bool mipmapEnabled;
        public TextureWrapMode wrapMode;
    }

    /// <summary>
    /// テクスチャのインポート設定用Inspector
    /// </summary>
    [CustomEditor(typeof(TextureImageImporterSettings))]
    public class TextureImageImporterSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("SaveSettings"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }

}
