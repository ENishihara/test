///
///  @brief アニメーションクリップの検索クラス
///

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExtractAnimationClipTool
{
    /// <summary>
    /// 検索結果のクリップに関するデータ
    /// </summary>
    [System.Serializable]
    public class ClipSearchResult
    {
        /// <summary>
        /// 新たに追加されるものかどうか
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// 抽出対象かどうか
        /// </summary>
        public bool IsTarget { get; set; }

        /// <summary>
        /// クリップ
        /// </summary>
        public AnimationClip Clip { get; set; }

        /// <summary>
        /// 元データ(FBX)へのパス
        /// </summary>
        public string SourceFbxPath { get; set; }
    }

    /// <summary>
    /// モーション検索結果
    /// </summary>
    [System.Serializable]
    public class MotionSearchResult
    {
        /// <summary>
        /// 抽出対象かどうか
        /// </summary>
        public bool IsTarget { get; set; }

        /// <summary>
        /// 元データ(FBX)へのパス
        /// </summary>
        public string SourceFbxPath { get; private set; }

        /// <summary>
        /// 元FBXファイル名
        /// </summary>
        public string FbxFileName { get; set; }

        /// <summary>
        /// クリップ検索結果リスト
        /// </summary>
        public List<ClipSearchResult> ClipDataList { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MotionSearchResult(string sourceFbxPath, string sourceFbxName)
        {
            SourceFbxPath = sourceFbxPath;
            FbxFileName = sourceFbxName;
            ClipDataList = new List<ClipSearchResult>();
        }
    }

    /// <summary>
    /// アニメーションクリップの検索クラス
    /// </summary>
    public class ExtractAnimationClipSearcher
    {
        /// <summary>
        /// 指定された種類のモーションクリップを検索して返す
        /// </summary>
        public static List<MotionSearchResult> SearchAnimationClipSources(ExtractAnimationClipEnum.MotionType type)
        {
            // ソースとなるファイルの検索
            string[] lookFor = { ExtractAnimationClipWindow.CurrentFolderPath };
            string[] fbxGuids = AssetDatabase.FindAssets(ExtractAnimationClipConst.FbxFilter, lookFor);

            var resultList = new List<MotionSearchResult>();
            foreach (string guid in fbxGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // 正規の拡張子だけを対象とする
                if (!Path.GetExtension(path).Equals(ExtractAnimationClipConst.FbxExtension))
                {
                    continue;
                }
                string fbxFileName = Path.GetFileNameWithoutExtension(path);

                var result = new MotionSearchResult(path, fbxFileName);
                Object[] files = AssetDatabase.LoadAllAssetsAtPath(path);

                List<Object> animationClips = files.Where(x => x is AnimationClip && !x.name.Contains(ExtractAnimationClipConst.PreviewClipPrefix)).ToList();
                if (animationClips == null || animationClips.Count <= 0)
                {
                    continue;
                }

                // 1クリップに関するデータを作る
                var castList = animationClips.Cast<AnimationClip>().ToList();
                foreach (AnimationClip clip in castList)
                {
                    ClipSearchResult clipResult = new ClipSearchResult();
                    clipResult.SourceFbxPath = result.SourceFbxPath;
                    clipResult.Clip = clip;
                    clipResult.IsNew = false == File.Exists(ExtractAnimationClipWindow.ExtractFolderPath + ExtractAnimationClipConst.DirectorySeparator + clip.name + ExtractAnimationClipConst.AnimationClipExtension);
                    clipResult.IsTarget = clipResult.IsNew;
                    result.ClipDataList.Add(clipResult);
                }

                // すべて新しいものならtrue
                result.IsTarget = result.ClipDataList.All(clipData => clipData.IsNew);
                resultList.Add(result);
            }
            return resultList;
        }
    }
}
