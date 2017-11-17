///
///  @brief アニメーションクリップの更新クラス
///

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExtractAnimationClipTool
{
    /// <summary>
    /// アニメーションクリップの更新クラス
    /// </summary>
    public static class ExtractAnimationClipUpdater
    {
        /// <summary>
        /// 設定
        /// </summary>
        private static ExtractAnimationClipSettings _settings;

        /// <summary>
        /// 指定されたソースをもとにアニメーションクリップの抽出、パラメータ設定を行う
        /// </summary>
        public static bool ExtractAnimationClip(ExtractAnimationClipEnum.MotionType motionType,
                                                List<MotionSearchResult> sourceList)
        {
            bool isProgressDisplayed = false;
            try
            {
                List<string> loopMotionNames = GetLoopMotionNames(GetSettings(), motionType);

                int counter = 1;
                int total = sourceList.Sum(result => result.ClipDataList.Where(clipData => clipData.IsTarget).Count());
                foreach (MotionSearchResult source in sourceList)
                {
                    foreach (ClipSearchResult clipData in source.ClipDataList)
                    {
                        if (false == clipData.IsTarget)
                        {
                            continue;
                        }
                        EditorUtility.DisplayProgressBar("AnimationClip抽出中", clipData.Clip.name, (float)counter / (float)total);
                        isProgressDisplayed = true;

                        AnimationClip copyClip = Object.Instantiate(clipData.Clip);

                        if (loopMotionNames.Contains(clipData.Clip.name))
                        {
                            var tmpSerialized = new SerializedObject(copyClip);
                            tmpSerialized.FindProperty("m_AnimationClipSettings.m_LoopTime").boolValue = true;
                            tmpSerialized.ApplyModifiedProperties();
                        }

                        AssetDatabase.CreateAsset(copyClip, ExtractAnimationClipConst.TmpClipFilePath);
                        string destPath = GetClipPath(clipData.Clip.name, motionType);
                        File.Copy(ExtractAnimationClipConst.TmpClipFilePath, destPath, true);
                        File.Delete(ExtractAnimationClipConst.TmpClipFilePath);
                        File.Delete(ExtractAnimationClipConst.TmpClipFilePath + ".meta");
                        AssetDatabase.Refresh();
                        AnimationClip saved = AssetDatabase.LoadAssetAtPath<AnimationClip>(destPath);
                        SerializedObject serialized = new SerializedObject(saved);
                        serialized.FindProperty("m_Name").stringValue = clipData.Clip.name;
                        serialized.ApplyModifiedProperties();
                        serialized.UpdateIfRequiredOrScript();
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        counter++;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            finally
            {
                if (isProgressDisplayed)
                {
                    EditorUtility.ClearProgressBar();
                }
            }
            return true;
        }

        /// <summary>
        /// 設定を取得
        /// </summary>
        static ExtractAnimationClipSettings GetSettings()
        {
            if (_settings == null)
            {
                _settings = LoadSettings();
            }
            return _settings;
        }

        /// <summary>
        /// 設定をロード
        /// </summary>
        public static ExtractAnimationClipSettings LoadSettings()
        {
            string[] guids = AssetDatabase.FindAssets(ExtractAnimationClipConst.SettingsSearchFilter);
            if (guids == null | guids.Length == 0)
            {
                return null;
            }

            if (guids.Length != 1)
            {
                Debug.LogError("Too many ExtractAnimationClipSettings.");
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<ExtractAnimationClipSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        /// <summary>
        /// 指定されたMotionTypeにおけるループモーション名のListを返す
        /// </summary>
        public static List<string> GetLoopMotionNames(ExtractAnimationClipSettings settings,
                                                      ExtractAnimationClipEnum.MotionType type)
        {
            switch (type)
            {
                case ExtractAnimationClipEnum.MotionType.InGame:
                    return settings.InGameLoopMotionNameList;
                case ExtractAnimationClipEnum.MotionType.OutGame:
                    return settings.OutGameLoopMotionNameList;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 指定されたクリップ名、MotionTypeのクリップのパスを返す
        /// </summary>
        public static string GetClipPath(string clipName, ExtractAnimationClipEnum.MotionType motionType)
        {
            string clipPath = ExtractAnimationClipWindow.ExtractFolderPath;
            return clipPath + ExtractAnimationClipConst.DirectorySeparator + clipName + ExtractAnimationClipConst.AnimationClipExtension;
        }
    }
}
