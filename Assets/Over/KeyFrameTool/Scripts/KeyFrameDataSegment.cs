using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特定対象の全キーフレームデータ
/// </summary>
[System.Serializable]
public class KeyFrameDataSegment
{
    /// <summary>
    /// 種類
    /// </summary>
    public KeyFrameToolEnum.TargetType Target;

    /// <summary>
    /// 全キーフレームオブジェクト
    /// </summary>
    public List<KeyFrameDataOnce> AllKeyFrameData = new List<KeyFrameDataOnce>();

    /// <summary>
    /// 種類指定コンストラクタ
    /// </summary>
    public KeyFrameDataSegment(KeyFrameToolEnum.TargetType target)
    {
        Target = target;
    }
}
