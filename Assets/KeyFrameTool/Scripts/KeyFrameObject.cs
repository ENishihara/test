using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キーフレーム用オブジェクト
/// </summary>
public class KeyFrameObject : MonoBehaviour
{
    /// <summary>
    /// プレハブへのパス
    /// </summary>
    public static readonly string PrefabPath = "Prefabs/KeyFrameObject";

    /// <summary>
    /// トグル
    /// </summary>
    [SerializeField]
    public Toggle ToggleButton;

    /// <summary>
    /// 種類
    /// </summary>
    public KeyFrameToolEnum.TargetType Target;

    /// <summary>
    /// データ
    /// </summary>
    public KeyFrameDataOnce Data = new KeyFrameDataOnce();

    /// <summary>
    /// 生成用static関数
    /// </summary>
    public static KeyFrameObject CreateKeyFrameObject(KeyFrameToolEnum.TargetType target, Transform parent)
    {
        var obj = Instantiate<KeyFrameObject>(Resources.Load<KeyFrameObject>(PrefabPath), parent);
        obj.Target = target;
        return obj;
    }
}
