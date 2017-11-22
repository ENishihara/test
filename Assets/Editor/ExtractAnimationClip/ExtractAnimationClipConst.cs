///
///  @brief アニメーションクリップの抽出関連定数
///

namespace ExtractAnimationClipTool
{
    /// <summary>
    /// アニメーションクリップの抽出関連定数
    /// </summary>
    public static class ExtractAnimationClipConst
    {
        /// <summary>
        /// ディレクトリ区切り文字
        /// </summary>
        public const string DirectorySeparator = "/";

        /// <summary>
        /// FBX検索フィルター
        /// </summary>
        public const string FbxFilter = "t:GameObject";

        /// <summary>
        /// FBX拡張子
        /// </summary>
        public const string FbxExtension = ".fbx";

        /// <summary>
        /// プレビュー用Prefix
        /// </summary>
        public const string PreviewClipPrefix = "__preview__";

        /// <summary>
        /// アニメーションクリップ検索用フィルター
        /// </summary>
        public const string AnimationClipFilter = "t:AnimationClip";

        /// <summary>
        /// アニメーション拡張子
        /// </summary>
        public const string AnimationClipExtension = ".anim";

        /// <summary>
        /// tempファイルのパス
        /// </summary>
        public const string TmpClipFilePath = "Assets/_tempClip" + AnimationClipExtension;

        /// <summary>
        /// 設定ファイル検索用フィルター
        /// </summary>
        public const string SettingsSearchFilter = "t:ExtractAnimationClipSettings";

        /// <summary>
        /// FBXからデータを抽出するパス
        /// </summary>
        public const string ExtractionDataFromFbxPath = "Assets/Resources/SvnRoot/3d/character/fbx";

        /// <summary>
        /// 抽出したデータを保存するパス
        /// </summary>
        public const string ExtractionDataSavePath = "Assets/Resources/SvnRoot/3d/character/motion_clip";
    }
}
