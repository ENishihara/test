/// 
///  @brief モーションクリップ抽出ツールの設定クラス
/// 

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ExtractAnimationClipTool
{
    /// <summary>
    /// モーションクリップ抽出ツールの設定クラス
    /// </summary>
    [CreateAssetMenu(fileName = "ExtractAnimationClipSettings", menuName = "Tool/ExtractAnimationClipSettings", order = 1000)]
    public class ExtractAnimationClipSettings : ScriptableObject
    {
        /// <summary>
        /// インゲームのものでループ設定が必要なモーション名リスト
        /// </summary>
        public List<string> InGameLoopMotionNameList;

        /// <summary>
        /// アウトゲームのものでループ設定が必要なモーション名リスト
        /// </summary>
        public List<string> OutGameLoopMotionNameList;
    }

    [CustomEditor(typeof(ExtractAnimationClipSettings))]
    public class ExtractAnimationClipSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("SaveSttings"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
