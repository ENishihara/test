///
///  @brief MVPの新しくスクリプト作成
///

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public class MVPScriptCreator : EditorWindow
{
    /// <summary>
    /// メニューのパス
    /// </summary>
    private const string MenuPath = "Assets/Create/";

    /// <summary>
    /// テンプレートがあるディレクトリへのパス
    /// </summary>
    private const string TempleteScriptDirectoryPath = "Assets/Editor/ScriptCreator/CSharpTemplate/";

    /// <summary>
    /// 開発者名保存key
    /// </summary>
    private const string DeveloperNameKey = "DeveloperName";

    /// <summary>
    /// テンプレート名とテンプレートメニュー記載項目
    /// </summary>
    private const string TemplateScriptMenuItemMVP = "C# MVP Script";

    /// <summary>
    /// スクリプトMAX
    /// </summary>
    private const int ScriptMax = 3;

    /// <summary>
    /// テンプレート名とテンプレートの拡張子
    /// </summary>
    private const string TemplateScriptExtension = ".txt";


    /// <summary>
    /// テンプレートファイル名
    /// </summary>
    static private string[] TemplateScriptMVP = {
        "MVPPresenter",
        "MVPView",
        "MVPModel"
    };
	
    /// <summary>
    /// 作成する元のテンプレート名
    /// </summary>
    private string _templateScriptName = "";

    /// <summary>
    /// 新しく作成するスクリプト及びクラス名
    /// </summary>
    private string _newScriptName = "";

    /// <summary>
    /// スクリプトの説明文
    /// </summary>
    private string _scriptSummary = "";

    /// <summary>
    /// 出力するスクリプト及びクラス名
    /// </summary>
    private string[] _outNewScriptName = new string[ScriptMax];

    /// <summary>
    /// スクリプトの説明文
    /// </summary>
    private string[] _outScriptSummary = new string[ScriptMax];

    /// <summary>
    /// メニューアイテムMVP
    /// </summary>
    [MenuItem(MenuPath + TemplateScriptMenuItemMVP)]
    private static void CreateMVPScript()
    {
        ShowWindow();
    }

    /// <summary>
    /// ウィンドウ表示
    /// </summary>
    private static void ShowWindow()
    {
        //ウィンドウ作成
        EditorWindow.GetWindow<MVPScriptCreator>("MVP Create Script");

        //各項目を初期化
        EditorWindow.GetWindow<MVPScriptCreator>()._newScriptName = "NewScript";
        EditorWindow.GetWindow<MVPScriptCreator>()._scriptSummary = "";
    }

    /// <summary>
    /// 表示するGUI処理
    /// </summary>
    private void OnGUI()
    {
        //テンプレート
        EditorGUILayout.LabelField("Template Script Name : " + _templateScriptName);
        GUILayout.Space(10);

        //スクリプト名
        GUILayout.Label("スクリプト名");
        EditorWindow.GetWindow<MVPScriptCreator>()._newScriptName = GUILayout.TextField(EditorWindow.GetWindow<MVPScriptCreator>()._newScriptName);
        GUILayout.Space(10);

        //スクリプトの説明文
        GUILayout.Label("スクリプト説明");
        EditorWindow.GetWindow<MVPScriptCreator>()._scriptSummary = GUILayout.TextArea(EditorWindow.GetWindow<MVPScriptCreator>()._scriptSummary);
        GUILayout.Space(30);

        //作成ボタン
        if (GUILayout.Button("Create"))
        {
            if (!CreateScript())
            {
                Debug.LogError("作成エラー");
            }
            //ウィンドウを閉じる
            this.Close();
        }
    }

    /// <summary>
    /// スクリプト作成
    /// </summary>
    private static bool CreateScript()
    {
        //スクリプト名が空欄の場合は作成失敗
        if (string.IsNullOrEmpty(EditorWindow.GetWindow<MVPScriptCreator>()._newScriptName))
        {
            Debug.Log("スクリプト名が入力されていないため、スクリプトが作成できませんでした");
            return false;
        }

        //現在選択しているファイルのパスを取得、選択されていない場合はスクリプト作成失敗
        string directoryPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(directoryPath))
        {
            Debug.Log("作成場所が選択されていないため、スクリプトが作成できませんでした");
            return false;
        }

        //選択されているファイルに拡張子がある場合(ディレクトリでない場合)は一つ上のディレクトリ内に作成する
        if (!string.IsNullOrEmpty(new System.IO.FileInfo(directoryPath).Extension))
        {
            directoryPath = System.IO.Directory.GetParent(directoryPath).FullName;
        }

        // ファイル名確定
        EditorWindow.GetWindow<MVPScriptCreator>()._outNewScriptName[0] = EditorWindow.GetWindow<MVPScriptCreator>()._newScriptName + "Presenter";
        EditorWindow.GetWindow<MVPScriptCreator>()._outNewScriptName[1] = EditorWindow.GetWindow<MVPScriptCreator>()._newScriptName + "View";
        EditorWindow.GetWindow<MVPScriptCreator>()._outNewScriptName[2] = EditorWindow.GetWindow<MVPScriptCreator>()._newScriptName + "Model";

        //説明確定
        EditorWindow.GetWindow<MVPScriptCreator>()._outScriptSummary[0] = EditorWindow.GetWindow<MVPScriptCreator>()._scriptSummary + "プレゼンタークラス";
        EditorWindow.GetWindow<MVPScriptCreator>()._outScriptSummary[1] = EditorWindow.GetWindow<MVPScriptCreator>()._scriptSummary + "ビュークラス";
        EditorWindow.GetWindow<MVPScriptCreator>()._outScriptSummary[2] = EditorWindow.GetWindow<MVPScriptCreator>()._scriptSummary + "モデルクラス";

        for (int i = 0; i < ScriptMax; i++)
        {
            //同名ファイルがあった場合はスクリプト作成失敗にする(上書きしてしまうため)
            string exportPath = directoryPath + "/" + EditorWindow.GetWindow<MVPScriptCreator>()._outNewScriptName[i] + ".cs";

            if (System.IO.File.Exists(exportPath))
            {
                Debug.Log(exportPath + "が既に存在するため、スクリプトが作成できませんでした");
                return false;
            } 

            //テンプレートへのパスを作成しテンプレート読み込み
            string templatePath = TempleteScriptDirectoryPath + TemplateScriptMVP[i] + TemplateScriptExtension;
            StreamReader streamReader = new StreamReader(templatePath, Encoding.UTF8);
            string scriptText = streamReader.ReadToEnd();
            // 閉じる
            streamReader.Close();

            //各項目を置換
            scriptText = scriptText.Replace("#AUTHOR#", EditorPrefs.GetString(DeveloperNameKey, ""));      
            scriptText = scriptText.Replace("#SUMMARY#", EditorWindow.GetWindow<MVPScriptCreator>()._outScriptSummary[i].Replace("\n", "\n/// ")); //改行するとコメントアウトから外れるので修正     
            scriptText = scriptText.Replace("#SCRIPTNAME#", EditorWindow.GetWindow<MVPScriptCreator>()._outNewScriptName[i]);
            scriptText = scriptText.Replace("#INPUTSCRIPTNAME#", EditorWindow.GetWindow<MVPScriptCreator>()._newScriptName);

            //スクリプトを書き出し
            File.WriteAllText(exportPath, scriptText, Encoding.UTF8);
        }

        //AssetDatabaseリフレッシュ
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        return true;
    }

}