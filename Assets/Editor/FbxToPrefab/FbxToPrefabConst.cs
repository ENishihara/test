///
///  @brief モデルの抽出関連定数
///

namespace FbxToPrefabTool
{
    /// <summary>
    /// モデルの抽出関連定数
    /// </summary>
    public static class FbxToPrefabToolConst
    {
        /// <summary>
        /// 検索用フィルター
        /// </summary>
        public const string FbxFilder = "t:GameObject";

        /// <summary>
        /// FBX拡張子
        /// </summary>
        public const string FbxExtention = ".fbx";

        /// <summary>
        /// ディレクトリ区切り文字
        /// </summary>
        public const string DirectorySeparator = "/";

        /// <summary>
        /// FBXからデータを抽出するパス
        /// </summary>
        public const string ExtractionDataFromFbxPath = "Assets/Resources/SvnRoot/3d/character/fbx";

        /// <summary>
        /// 抽出したデータを保存するパス
        /// </summary>
        public const string ExtractionDataSavePath = "Assets/Resources/SvnRoot/3d/character/prefab";

        /// <summary>
        /// Prefab張子
        /// </summary>
        public const string PrefabExtension = ".prefab";
       
        /// <summary>
        /// tempファイルのパス
        /// </summary>
        public const string TmpPrefabFilePath = "Assets/_tempPrefab" + PrefabExtension;

        /// <summary>
        /// fbxの共通の先頭名
        /// </summary>
        public const string FbxFileHeadCommonName = "char_mod";

        /// <summary>
        /// prefabの共通の先頭名
        /// </summary>
        public const string PrefabFileHeadCommonName = "char_pre";
    }
}
