using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace FbxToPrefabTool
{
    /// <summary>
    /// FBXをプレハブにするためのエディターウィンドウクラス
    /// </summary>
    public class FbxToPrefabWindow : EditorWindow
    {
        /// <summary>
        /// ページ
        /// </summary>
        private enum PageType
        {
            FolderConfirm,
            Select,
            SelectingConfirm,
            Complete,
        }

        /// <summary>
        /// windowサイズ
        /// </summary>
        private static readonly Vector2 WindowMinSize = new Vector2(700f, 300f);

        /// <summary>
        /// windowのインスタンス
        /// </summary>
        private static FbxToPrefabWindow _window;

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
        public static string FbxFolderPath;

        /// <summary>
        /// プレハブ保存先フォルダパス
        /// </summary>
        public static string PrefabFolderPath;

        /// <summary>
        /// ページ
        /// </summary>
        private static PageType _page;

        /// <summary>
        /// スクロール位置
        /// </summary>
        private static Vector2 _scrollPos;

        /// <summary>
        /// 検索結果キャッシュ
        /// </summary>
        private static List<FbxSearchResult> _searchList;

        /// <summary>
        /// GUI更新
        /// </summary>
        private void OnGUI()
        {
            switch (_page)
            {
                case PageType.FolderConfirm:
                    FolderConfirmPage();
                    break;
                case PageType.Select:
                    SelectPage();
                    break;
                case PageType.SelectingConfirm:
                    SelectingConfirmPage();
                    break;
                case PageType.Complete:
                    CompletePage();
                    break;
            }
        }

        [MenuItem("Tools/FbxToPrefab")]
        private static void FbxToPrefab()
        {
            FbxFolderPath = GetSelectFile();
            if (!Directory.Exists(FbxFolderPath))
            {
                Debug.LogWarning("抽出元フォルダを選択している状態で実行してください");
                return;
            }
            PrefabFolderPath = SelectFolder();
            if (string.IsNullOrEmpty(PrefabFolderPath))
            {
                // 空文字はキャンセルとみなす
                return;
            }
            // 相対パスに変換
            PrefabFolderPath = "Assets" + PrefabFolderPath.Substring(Application.dataPath.Length);

            _page = PageType.FolderConfirm;
            OpenWindow();
        }

        /// <summary>
        /// windowを開く
        /// </summary>
        private static void OpenWindow()
        {
            if (_window == null)
            {
                _window = CreateInstance<FbxToPrefabWindow>();
            }

            _searchList = null;

            _window.minSize = WindowMinSize;
            _window.ShowUtility();
        }

        /// <summary>
        /// FBX参照フォルダとプレハブ出力フォルダを確認するページ
        /// </summary>
        private static void FolderConfirmPage()
        {
            EditorGUILayout.LabelField("対象フォルダ確認");
            EditorGUILayout.HelpBox("FBX格納フォルダ" + FbxFolderPath, MessageType.None);
            EditorGUILayout.HelpBox("Prefab保存先フォルダ" + PrefabFolderPath, MessageType.None);

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("進む", GUILayout.Width(ButtonWidth)))
            {
                // 検索
                _searchList = FbxToPrefabConverter.GetFbxPathList(FbxFolderPath, PrefabFolderPath);
                SwitchPage(PageType.Select);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// プレハブ化するFBXを選択するページ
        /// </summary>
        private static void SelectPage()
        {
            EditorGUILayout.LabelField("対象FBX選択");
            EditorGUILayout.HelpBox("プレハブ化するFBXを選択してください", MessageType.Info);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUI.skin.scrollView);
            foreach (var searchData in _searchList)
            {
                EditorGUILayout.BeginHorizontal();
                searchData.IsTarget = EditorGUILayout.ToggleLeft("", searchData.IsTarget, GUILayout.Width(10f));
                searchData.IsTarget = GUILayout.Toggle(searchData.IsTarget, searchData.Path, "PR Label");
                if (searchData.IsNew)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(NewIconTexture);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("全てチェック"))
            {
                _searchList.ForEach(x => x.IsTarget = true);
            }
            if (GUILayout.Button("全てチェックを外す"))
            {
                _searchList.ForEach(x => x.IsTarget = false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(!_searchList.Any(x => x.IsTarget));
            if (GUILayout.Button("次へ", GUILayout.Width(ButtonWidth)))
            {
                SwitchPage(PageType.SelectingConfirm);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 選択したFBXを確認するページ
        /// </summary>
        private static void SelectingConfirmPage()
        {
            EditorGUILayout.LabelField("対象FBX確認");

            EditorGUILayout.BeginVertical(GUI.skin.box);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUI.skin.scrollView);
            foreach (FbxSearchResult searchData in _searchList)
            {
                if (searchData.IsTarget)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(searchData.Path);
                    if (searchData.IsNew)
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(NewIconTexture);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("戻る", GUILayout.Width(ButtonWidth)))
            {
                SwitchPage(PageType.Select);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("次へ", GUILayout.Width(ButtonWidth)))
            {
                // プレハブ化処理
                FbxToPrefabConverter.FbxToPrefab(_searchList, PrefabFolderPath);
                SwitchPage(PageType.Complete);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// FBX出力が終了したページ
        /// </summary>
        private static void CompletePage()
        {
            EditorGUILayout.LabelField("FBXプレハブ化");
            EditorGUILayout.HelpBox("完了しました", MessageType.Info);
        }

        /// <summary>
        /// ページ切り替え
        /// </summary>
        private static void SwitchPage(PageType page)
        {
            _page = page;
            _scrollPos = Vector2.zero;
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
