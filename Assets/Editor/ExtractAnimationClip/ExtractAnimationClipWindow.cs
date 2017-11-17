///
///  @brief アニメーションクリップ抽出EditorWindowクラス
///

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UniRx;

namespace ExtractAnimationClipTool
{
    /// <summary>
    /// アニメーションクリップ抽出EditorWindowクラス
    /// </summary>
    public class ExtractAnimationClipWindow : EditorWindow
    {
        /// <summary>
        /// windowサイズ
        /// </summary>
        private static readonly Vector2 WindowMinSize = new Vector2(700f, 300f);

        /// <summary>
        /// newアイコンのテクスチャ
        /// </summary>
        private static readonly Texture NewIconTexture = EditorGUIUtility.IconContent("AS Badge New").image;

        /// <summary>
        /// ボタンサイズ
        /// </summary>
        private static readonly float ButtonWidth = 120f;

        /// <summary>
        /// 抽出元FRX格納フォルダパス
        /// </summary>
        public static string CurrentFolderPath;

        /// <summary>
        /// Clip保存先フォルダパス
        /// </summary>
        public static string ExtractFolderPath;

        /// <summary>
        /// windowのインスタンス
        /// </summary>
        private static ExtractAnimationClipWindow _window;

        /// <summary>
        /// 今のページ
        /// </summary>
        private static ExtractAnimationClipEnum.Page _page;

        /// <summary>
        /// スクロール位置
        /// </summary>
        private static Vector2 _scrollPos;

        /// <summary>
        /// 検索終了状態かどうか
        /// </summary>
        private static bool _isSearched;

        /// <summary>
        /// モーションタイプ
        /// </summary>
        private static ExtractAnimationClipEnum.MotionType _motionType = ExtractAnimationClipEnum.MotionType.InGame;

        /// <summary>
        /// 検索結果格納
        /// </summary>
        private static List<MotionSearchResult> _searchResult;

        /// <summary>
        /// 抽出に成功したかどうか
        /// </summary>
        private static bool _isExtractSuccess;

        /// <summary>
        /// Initialize
        /// </summary>
        static void Initialize()
        {
            SwitchPage(ExtractAnimationClipEnum.Page.FolderConfirm);
            _isSearched = false;
            _isExtractSuccess = false;
            _searchResult = null;
        }

        /// <summary>
        /// 開始関数
        /// </summary>
        [MenuItem("Tools/ExtractAnimationClip")]
        static void Start()
        {
//            CurrentFolderPath = GetSelectFile();
//            if (!Directory.Exists(CurrentFolderPath))
//            {
//                Debug.LogWarning("抽出元フォルダを選択している状態で実行してください");
//                return;
//            }
//            ExtractFolderPath = SelectFolder();
//            if (string.IsNullOrEmpty(ExtractFolderPath))
//            {
//                // 空文字はキャンセルとみなす
//                return;
//            }
//            // 相対パスに変換
//            ExtractFolderPath = "Assets" + ExtractFolderPath.Substring(Application.dataPath.Length);

            // シンボリックリンクからパスを取得するとAssets以下にならなくなるので
            // AssetDatabaseが使えなくなってしまうので固定にする

            // 固定のディレクトリから抽出
            CurrentFolderPath = ExtractAnimationClipConst.ExtractionDataFromFbxPath;

            // 固定のディレクトリに保存
            ExtractFolderPath = ExtractAnimationClipConst.ExtractionDataSavePath;
            Open();
        }

        /// <summary>
        /// Open
        /// </summary>
        static void Open()
        {
            if (_window == null)
            {
                _window = CreateInstance<ExtractAnimationClipWindow>();
            }

            Initialize();

            _window.minSize = WindowMinSize;
            _window.ShowUtility();
        }

        void OnGUI()
        {
            switch (_page)
            {
                case ExtractAnimationClipEnum.Page.FolderConfirm:
                    ShowFolderConfirmPage();
                    break;
                case ExtractAnimationClipEnum.Page.Alert:
                    ShowAlertPage();
                    break;
                case ExtractAnimationClipEnum.Page.Select:
                    ShowSelectPage();
                    break;
                case ExtractAnimationClipEnum.Page.Confirm:
                    ShowConfirmPage();
                    break;
                case ExtractAnimationClipEnum.Page.Complete:
                    ShowCompletePage();
                    break;
            }
        }

        /// <summary>
        /// 対象フォルダ確認ページを表示
        /// </summary>
        static void ShowFolderConfirmPage()
        {
            EditorGUILayout.LabelField("対象フォルダ確認");
            EditorGUILayout.HelpBox("抽出元FBXフォルダ:" + CurrentFolderPath, MessageType.None);
            EditorGUILayout.HelpBox("Clip保存先フォルダ:" + ExtractFolderPath, MessageType.None);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MotionType");
            _motionType = (ExtractAnimationClipEnum.MotionType)System.Enum.Parse(typeof(ExtractAnimationClipEnum.MotionType), EditorGUILayout.EnumPopup(_motionType).ToString());
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("やめる", GUILayout.Width(ButtonWidth)))
            {
                _window.Close();
                GUIUtility.ExitGUI();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("進む", GUILayout.Width(ButtonWidth)))
            {
                ExtractAnimationClipEnum.Page nextPage = ExtractAnimationClipEnum.Page.Alert;
                SwitchPage(nextPage);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 異常ファイル確認ページを表示
        /// </summary>
        static void ShowAlertPage()
        {
            if (!_isSearched)
            {
                _searchResult = ExtractAnimationClipSearcher.SearchAnimationClipSources(_motionType);
                _isSearched = true;
            }

            // MEMO:何をもって異常とするか決めてない
            List<string> invalidList = new List<string>();

            ExtractAnimationClipEnum.Page nextPage = ExtractAnimationClipEnum.Page.Select;

            if (invalidList.Count == 0)
            {
                ShowSelectPage();
                SwitchPage(nextPage);
                return;
            }

            EditorGUILayout.LabelField("AnimationClip抽出");
            EditorGUILayout.HelpBox("不正なファイルが存在します。", MessageType.Warning);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUI.skin.scrollView);

            foreach (string path in invalidList)
            {
                GUILayout.Label(path);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("キャンセル", GUILayout.Width(ButtonWidth)))
            {
                _window.Close();
                GUIUtility.ExitGUI();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("無視して進む", GUILayout.Width(ButtonWidth)))
            {
                SwitchPage(nextPage);
            }
            EditorGUILayout.EndHorizontal();

        }

        /// <summary>
        /// 抽出するものを選択するページを表示
        /// </summary>
        static void ShowSelectPage()
        {
            List<MotionSearchResult> validList = _searchResult.ToList();

            string infoLabel = "追加、更新したいものにチェックを入れてください。";
            EditorGUILayout.LabelField("AnimationClip抽出");
            EditorGUILayout.HelpBox(infoLabel, MessageType.Info);

            EditorGUILayout.BeginVertical(GUI.skin.box);    
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUI.skin.scrollView);

            foreach (MotionSearchResult source in validList)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("a", GUILayout.Width(16f), GUILayout.Height(16f)))
                {
                    validList.ForEach(result =>
                    {
                        bool allCancel = !result.ClipDataList.Any(clipData => clipData.IsTarget);
                        result.ClipDataList.ForEach(clipData =>
                        {
                            if (clipData.SourceFbxPath == source.SourceFbxPath)
                            {
                                clipData.IsTarget = allCancel;
                            }
                        });
                    });
                }
                GUILayout.Label(source.SourceFbxPath);
                EditorGUILayout.EndHorizontal();
                foreach (ClipSearchResult clipData in source.ClipDataList)
                {
                    EditorGUILayout.BeginHorizontal();
                    clipData.IsTarget = EditorGUILayout.ToggleLeft("", clipData.IsTarget, GUILayout.Width(10f));
                    clipData.IsTarget = GUILayout.Toggle(clipData.IsTarget, clipData.Clip.name, "PR Label");
                    if (clipData.IsNew)
                    {
                        GUILayout.Label(NewIconTexture);
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("全てチェック"))
            {
                validList.ForEach(result =>
                {
                    result.ClipDataList.ForEach(clipData =>
                    {
                        clipData.IsTarget = true;
                    });
                });
            }
            if (GUILayout.Button("全てチェックを外す"))
            {
                validList.ForEach(result =>
                {
                    result.ClipDataList.ForEach(clipData =>
                    {
                        clipData.IsTarget = false;
                    });
                });
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("キャンセル", GUILayout.Width(ButtonWidth)))
            {
                _window.Close();
                GUIUtility.ExitGUI();
                return;
            }

            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(!validList.Any(result => result.ClipDataList.Any(clipData => clipData.IsTarget)));
            if (GUILayout.Button("次へ", GUILayout.Width(ButtonWidth)))
            {
                SwitchPage(ExtractAnimationClipEnum.Page.Confirm);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

        }

        /// <summary>
        /// 抽出確認ページを表示
        /// </summary>
        static void ShowConfirmPage()
        {
            string infoLabel = "以下のファイルのAnimationClipを抽出します。";
            EditorGUILayout.LabelField("AnimationClip抽出");
            EditorGUILayout.HelpBox(infoLabel, MessageType.Info);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUI.skin.scrollView);

            int clipCount = 0;
            foreach (MotionSearchResult result in _searchResult)
            {
                GUILayout.Label(result.SourceFbxPath);
                foreach (ClipSearchResult clipData in result.ClipDataList)
                {
                    if (clipData.IsTarget)
                    {
                        GUILayout.Label("    " + clipData.Clip.name);
                        clipCount++;
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("戻る", GUILayout.Width(ButtonWidth)))
            {
                SwitchPage(ExtractAnimationClipEnum.Page.Select);
                return;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("抽出実行", GUILayout.Width(ButtonWidth)))
            {
                if (EditorUtility.DisplayDialog("抽出実行確認", clipCount + "項目が対象になります。\r\n抽出を開始しますか？", "実行", "キャンセル"))
                {
                    Observable.FromCoroutine(ExtractAnimationClips).Subscribe();
                }
            }
            EditorGUILayout.EndHorizontal();
            
        }

        /// <summary>
        /// 抽出終了ページを表示
        /// </summary>
        static void ShowCompletePage()
        {
            if (_isExtractSuccess)
            {
                EditorGUILayout.HelpBox("AnimationClipの抽出が成功しました。", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("AnimationClipの抽出に失敗しました。", MessageType.Warning);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("閉じる", GUILayout.Width(ButtonWidth)))
            {
                _window.Close();
                GUIUtility.ExitGUI();
                return;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 抽出開始
        /// </summary>
        static IEnumerator ExtractAnimationClips()
        {
            _isExtractSuccess = ExtractAnimationClipUpdater.ExtractAnimationClip(_motionType, _searchResult);
            if (!_isExtractSuccess)
            {
                SwitchPage(ExtractAnimationClipEnum.Page.Complete);
                yield break;
            }

            SwitchPage(ExtractAnimationClipEnum.Page.Complete);
            yield return null;

        }

        /// <summary>
        /// ページ切り替え
        /// </summary>
        static void SwitchPage(ExtractAnimationClipEnum.Page nextPage)
        {
            _scrollPos = Vector2.zero;
            _page = nextPage;
        }

        /// <summary>
        /// 選択中のファイルパス取得
        /// </summary>
        public static string GetSelectFile()
        {
            // ファイルが選択されている時
            if (Selection.assetGUIDs != null && Selection.assetGUIDs.Length > 0)
            {
                // 選択されているパスの取得
                string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs.First());
                return path;
            }
            return null;
        }

        /// <summary>
        /// フォルダを選択させ、そのパスを返す
        /// </summary>
        public static string SelectFolder()
        {
            string path = EditorUtility.OpenFolderPanel("フォルダ選択", "Assets/", "");
            if (path.Length != 0)
            {
                Debug.Log(path);
            }
            return path;
        }
    }
}
