using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyFrameData
{
    /// <summary>
    /// セーブデータへのパス
    /// </summary>
    public static readonly string SaveFilePath = "Assets/Over/KeyFrameTool/SaveData/";

    /// <summary>
    /// セーブデータの拡張子
    /// </summary>
    public static readonly string SaveFileExtention = ".json";

    /// <summary>
    /// ファイル名
    /// </summary>
    public string FileName;

    /// <summary>
    /// 種類ごとのデータリスト
    /// </summary>
    public List<KeyFrameDataSegment> Segments = new List<KeyFrameDataSegment>();

    /// <summary>
    /// １つのモーションの個別保存を行う
    /// </summary>
    public void SaveDataToJson()
    {
        string jsonText = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(SaveFilePath + this.FileName + SaveFileExtention, jsonText);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        Debug.Log("セーブ完了");
    }

    /// <summary>
    /// 外部から読み込む
    /// </summary>
    public static KeyFrameData LoadDataFromJson(string name)
    {
        string fileName = SaveFilePath + name + SaveFileExtention;
        if (System.IO.File.Exists(fileName))
        {
            string json = System.IO.File.ReadAllText(fileName);
            var data = JsonUtility.FromJson<KeyFrameData>(json);
            return data;
        }
        else
        {
            Debug.LogWarning(fileName + "は存在しません");
            return null;
        }
    }
}
