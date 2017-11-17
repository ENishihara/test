using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Over.Unity.Importer
{
    /// <summary>
    /// FBXのインポート設定を管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "FbxImporterSettings", menuName = "Tools/FbxImporterSettings", order = 1000)]
    public class FbxImporterSettings : ScriptableObject
    {
        public List<FbxImportProperty> propertyList;
    }

    public enum FbxImportTarget
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

    public enum FbxType
    {
        Model,
        Motion
    }

    /// <summary>
    /// 設定対象とするディレクトリ及び、そのディレクトリで有効な設定を管理
    /// </summary>
    [System.Serializable]
    public class FbxImportProperty
    {
        public FbxType fbxType;
        public FbxImportTarget importTarget;
        public string[] targetPaths;

        [Header("モデル関連設定")]
        [ConditionalDisableAttribute("fbxType", (int)FbxType.Model)]
        public ModelImporterMaterialSearch materialSearch;

        [ConditionalDisableAttribute("fbxType", (int)FbxType.Model)]
        public bool importAnimation;

        [TooltipAttribute("AnimationTypeを変更するかどうか")]
        [ConditionalDisableAttribute("fbxType", (int)FbxType.Model)]
        public bool changeAnimationType;

        [TooltipAttribute("AnimationTypeを変更する判断をするオブジェクトの接頭辞（半角）")]
        [ConditionalDisableAttribute("fbxType", (int)FbxType.Model)]
        public string changeAnimationTypeIdentifier;

        [TooltipAttribute("設定するAnimationType")]
        [ConditionalDisableAttribute("fbxType", (int)FbxType.Model)]
        public ModelImporterAnimationType animationType;

        [Header("モーション関連設定")]
        [TooltipAttribute("アニメーションの圧縮オプション")]
        [ConditionalDisableAttribute("fbxType", (int)FbxType.Motion)]
        public ModelImporterAnimationCompression animationCompression;

        [ConditionalDisableAttribute("fbxType", (int)FbxType.Motion)]
        public float rotationError;

        [ConditionalDisableAttribute("fbxType", (int)FbxType.Motion)]
        public float positionError;

        [ConditionalDisableAttribute("fbxType", (int)FbxType.Motion)]
        public float scaleError;
    }

    /// <summary>
    /// FBXのインポート設定用Inspector
    /// </summary>
    [CustomEditor(typeof(FbxImporterSettings))]
    public class FbxImporterSettingsInspector : Editor
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