using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace FbxToPrefabTool
{
    /// <summary>
    /// 検索結果
    /// </summary>
    public class FbxSearchResult
    {
        /// <summary>
        /// パス
        /// </summary>
        public string Path;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName;

        /// <summary>
        /// 新規かどうか
        /// </summary>
        public bool IsNew;

        /// <summary>
        /// プレハブ化対象科同意あ
        /// </summary>
        public bool IsTarget;

        /// <summary>
        /// Fbxデータ
        /// </summary>
        public GameObject Fbx;
    }

    /// <summary>
    /// FBXをプレハブにするための変換ロジッククラス
    /// </summary>
    public class FbxToPrefabConverter
    {
        /// <summary>
        /// 定数
        /// </summary>
        public const string FbxFilder = "t:GameObject";
        public const string FbxExtention = ".fbx";
        public const string PrefabExtention = ".prefab";
        public const string DirectorySeparator = "/";

        /// <summary>
        /// 指定フォルダからFBXファイルを検索する
        /// </summary>
        public static List<FbxSearchResult> GetFbxPathList(string fbxFolderPath, string prefabFolderPath)
        {
            // ソースとなるファイルの検索
            string[] lookFor = { fbxFolderPath };
            string[] fbxGuids = AssetDatabase.FindAssets("t:GameObject", lookFor);

            if (fbxGuids == null || fbxGuids.Length <= 0)
            {
                Debug.LogWarning("fbxファイルが見つかりませんでした");
                return null;
            }

            // 結果格納用リスト
            List<FbxSearchResult> resultList = new List<FbxSearchResult>();

            foreach (string guid in fbxGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (false == Path.GetExtension(path).Equals(FbxExtention))
                {
                    continue;
                }
                string fileName = Path.GetFileNameWithoutExtension(path);

                var result = new FbxSearchResult();
                GameObject fbx = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                result.Fbx = fbx;
                result.Path = path;
                result.FileName = fileName;
                result.IsNew = false == File.Exists(prefabFolderPath + DirectorySeparator + fbx.name + FbxExtention);
                result.IsTarget = result.IsNew;
                resultList.Add(result);
            }

            return resultList;
        }

        /// <summary>
        /// FBXをプレハブ化する
        /// </summary>
        public static void FbxToPrefab(List<FbxSearchResult> searchList, string prefabFolderPath)
        {
            int progressCounter = 1;
            int totalCount = searchList.Where(fbx => fbx.IsTarget).Count();
            bool isProgressDisplayed = false;
            foreach (FbxSearchResult fbxData in searchList)
            {
                EditorUtility.DisplayProgressBar("AnimationClip抽出中", fbxData.Path, (float)progressCounter / (float)totalCount);
                isProgressDisplayed = true;

                if (fbxData.IsTarget)
                {
                    GameObject prefab = PrefabUtility.CreatePrefab(prefabFolderPath + DirectorySeparator + fbxData.FileName + PrefabExtention, fbxData.Fbx);

                    CustomizePrefab(prefab);

                    AssetDatabase.Refresh();
                    progressCounter++;
                }
            }
            if (isProgressDisplayed)
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// プレハブ化したfbxを加工する
        /// </summary>
        private static void CustomizePrefab(GameObject prefab)
        {
            
        }
    }
}
