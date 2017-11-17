using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HogeKeyFrameGroup : KeyFrameGroup
{
    /// <summary>
    /// キーフレーム対象
    /// </summary>
    public override KeyFrameToolEnum.TargetType Target { get { return KeyFrameToolEnum.TargetType.Hoge; } }
}
