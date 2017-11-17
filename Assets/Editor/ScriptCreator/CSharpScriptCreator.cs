///
///  @brief テンプレートから新しくスクリプト作成
///

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CSharpScriptCreator : EditorWindow
{
    //メニューのパス
    private const string MenuPath = "Assets/Create/";

    //テンプレートがあるディレクトリへのパス
    private const string TempleteScriptDirectoryPath = "Assets/Editor/ScriptCreator/CSharpTemplate/";

    //開発者名保存key
    private const string DeveloperNameKey = "DeveloperName";

    //テンプレート名とテンプレートメニュー記載項目
    private const string TemplateScriptMenuItemBasic = "C# Script(Inherit MonoBase)";
    private const string TemplateScriptMenuItemStatic = "C# Script Static";
    private const string TemplateScriptMenuItemSingleton = "C# Script Singleton(Inherit RQ_SingletonBehaviour)";

    //テンプレート名とテンプレート項目
    private const string TemplateScriptBasic = "Basic";
    private const string TemplateScriptStatic = "Static";
    private const string TemplateScriptSingleton = "Singleton";

    //テンプレート名とテンプレートの拡張子
    private const string TemplateScriptExtension = ".txt";

    //作成する元のテンプレート名
    private string templateScriptName_ = "";
    //新しく作成するスクリプト及びクラス名
    private string newScriptName_ = "";
    //スクリプトの説明文
    private string scriptSummary_ = "";

    /// <summary>
    /// メニューアイテムBasic
    /// </summary>
    [MenuItem(MenuPath + TemplateScriptMenuItemBasic)]
    private static void CreateBasicScript()
    {
        ShowWindow(TemplateScriptBasic);
    }

    /// <summary>
    /// メニューアイテムStatic
    /// </summary>
    [MenuItem(MenuPath + TemplateScriptMenuItemStatic)]
    private static void CreateStaticScript()
    {
        ShowWindow(TemplateScriptStatic);
    }

    /// <summary>
    /// メニューアイテムSingleton
    /// </summary>
    [MenuItem(MenuPath + TemplateScriptMenuItemSingleton)]
    private static void CreateSingletonScript()
    {
        ShowWindow(TemplateScriptSingleton);
    }

    /// <summary>
    /// ウィンドウ表示
    /// </summary>
    private static void ShowWindow(string templateScriptName)
    {
        //ウィンドウ作成
        EditorWindow.GetWindow<CSharpScriptCreator>("Create Script");

        //各項目を初期化
        EditorWindow.GetWindow<CSharpScriptCreator>().templateScriptName_ = templateScriptName;
        EditorWindow.GetWindow<CSharpScriptCreator>().newScriptName_ = "NewScript";
        EditorWindow.GetWindow<CSharpScriptCreator>().scriptSummary_ = "";
    }

    /// <summary>
    /// 表示するGUI処理
    /// </summary>
    private void OnGUI()
    {
        //テンプレート
        EditorGUILayout.LabelField("Template Script Name : " + templateScriptName_);
        GUILayout.Space(10);

        //スクリプト名
        GUILayout.Label("スクリプト名");
        newScriptName_ = GUILayout.TextField(newScriptName_);
        GUILayout.Space(10);

        //スクリプトの説明文
        GUILayout.Label("スクリプト説明");
        scriptSummary_ = GUILayout.TextArea(scriptSummary_);
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
        if (string.IsNullOrEmpty(EditorWindow.GetWindow<CSharpScriptCreator>().newScriptName_))
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

        //同名ファイルがあった場合はスクリプト作成失敗にする(上書きしてしまうため)
        string exportPath = directoryPath + "/" + EditorWindow.GetWindow<CSharpScriptCreator>().newScriptName_ + ".cs";

        if (System.IO.File.Exists(exportPath))
        {
            Debug.Log(exportPath + "が既に存在するため、スクリプトが作成できませんでした");
            return false;
        } 

        //テンプレートへのパスを作成しテンプレート読み込み
        string templatePath = TempleteScriptDirectoryPath + EditorWindow.GetWindow<CSharpScriptCreator>().templateScriptName_ + TemplateScriptExtension;
        StreamReader streamReader = new StreamReader(templatePath, Encoding.UTF8);
        string scriptText = streamReader.ReadToEnd();
        // 閉じる
        streamReader.Close();

        //各項目を置換
        scriptText = scriptText.Replace("#AUTHOR#", EditorPrefs.GetString(DeveloperNameKey, ""));      
        scriptText = scriptText.Replace("#SUMMARY#", EditorWindow.GetWindow<CSharpScriptCreator>().scriptSummary_.Replace("\n", "\n/// ")); //改行するとコメントアウトから外れるので修正     
        scriptText = scriptText.Replace("#SCRIPTNAME#", EditorWindow.GetWindow<CSharpScriptCreator>().newScriptName_);

        //スクリプトを書き出し
        File.WriteAllText(exportPath, scriptText, Encoding.UTF8);
        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        return true;
    }
}
