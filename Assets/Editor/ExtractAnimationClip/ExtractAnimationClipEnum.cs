///
///  @brief アニメーションクリップ抽出関連Enum
///

namespace ExtractAnimationClipTool
{
    /// <summary>
    /// アニメーションクリップ抽出関連Enum
    /// </summary>
    public class ExtractAnimationClipEnum
    {
        /// <summary>
        /// モーションタイプ
        /// </summary>
        public enum MotionType
        {
            InGame,
            OutGame,
        }

        /// <summary>
        /// Window内の状態管理
        /// </summary>
        public enum Page
        {
            // 選択フォルダ確認
            FolderConfirm,
            // 異常ファイル確認
            Alert,
            // 抽出対象を選択
            Select,
            // 抽出開始確認
            Confirm,
            // 終了画面
            Complete
        }
    }
}
