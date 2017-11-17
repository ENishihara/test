using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// キーフレームグループのベースクラス
/// </summary>
public abstract class KeyFrameGroup : MonoBehaviour
{
    /// <summary>
    /// 時間スライダー参照
    /// </summary>
    public Slider TimeSliderRef;

    /// <summary>
    /// キーフレーム登録/解除ボタン
    /// </summary>
    [SerializeField]
    private Button _registerButton;

    /// <summary>
    /// キーフレーム登録/解除ボタンテキスト
    /// </summary>
    [SerializeField]
    private Text _registerButtonText;

    /// <summary>
    /// キーフレームオブジェクト生成root
    /// </summary>
    [SerializeField]
    protected RectTransform KeyFrameObjectRoot;

    /// <summary>
    /// トグルグループ
    /// </summary>
    public ToggleGroup ToggleGroup;

    /// <summary>
    /// 一つ前のAnyToggleOn
    /// </summary>
    private bool _prevAnyToggleOn;

    /// <summary>
    /// データ
    /// </summary>
    public KeyFrameDataSegment Data;

    /// <summary>
    /// 全キーフレームオブジェクトリスト
    /// </summary>
    protected List<KeyFrameObject> AllKeyFrameObjectList = new List<KeyFrameObject>();

    /// <summary>
    /// キーフレーム対象
    /// </summary>
    public abstract KeyFrameToolEnum.TargetType Target { get; }

    /// <summary>
    /// 生成直後の処理
    /// </summary>
    protected virtual void Awake()
    {
        Data = new KeyFrameDataSegment(Target);

        _registerButton.onClick.AddListener(() =>
        {
            if (ToggleGroup.AnyTogglesOn())
            {
                UnregisterKeyFrameData(ToggleGroup.ActiveToggles().First().GetComponentInParent<KeyFrameObject>());
            }
            else
            {
                RegisterKeyFrameData();
            }
        });
    }

    /// <summary>
    /// 毎フレーム更新処理
    /// </summary>
    protected virtual void Update()
    {
        _registerButtonText.text = ToggleGroup.AnyTogglesOn() ? "解除" : "登録";
    }

    /// <summary>
    /// キーフレームを新規に登録する
    /// </summary>
    public virtual void RegisterKeyFrameData()
    {
        // 生成
        KeyFrameObject obj = KeyFrameObject.CreateKeyFrameObject(Target, KeyFrameObjectRoot);
        obj.transform.SetPositionX(TimeSliderRef.handleRect.position.x);
        obj.transform.SetLocalPositionY(0);

        // トグル登録
        ToggleGroup.RegisterToggle(obj.ToggleButton);
        obj.ToggleButton.group = ToggleGroup;

        //　データ登録
        obj.Data.SettingTime = TimeSliderRef.value;
        Data.AllKeyFrameData.Add(obj.Data);
        AllKeyFrameObjectList.Add(obj);
    }

    /// <summary>
    /// データを渡して反映する
    /// 編集中のデータは破棄する
    /// </summary>
    public virtual void ApplyKeyFrameData(KeyFrameDataSegment segment)
    {
        ClearTempKeyFrameData();
        foreach (var frame in segment.AllKeyFrameData)
        {
            // 生成
            KeyFrameObject obj = KeyFrameObject.CreateKeyFrameObject(Target, KeyFrameObjectRoot);
            obj.transform.position = KeyFrameObjectRoot.position;
            // スライダーの部分のサイズ変更に対応できるように、スライダーの幅*設定時間(0~1)で座標を設定
            obj.transform.SetLocalPositionX(TimeSliderRef.handleRect.parent.GetComponent<RectTransform>().rect.width * frame.SettingTime);

            // トグル登録
            ToggleGroup.RegisterToggle(obj.ToggleButton);
            obj.ToggleButton.group = ToggleGroup;

            //　データ登録
            obj.Data.SettingTime = frame.SettingTime;
            Data.AllKeyFrameData.Add(obj.Data);
            AllKeyFrameObjectList.Add(obj);
        }
        ToggleGroup.SetAllTogglesOff();
    }

    /// <summary>
    /// キーフレームを解除する
    /// </summary>
    public virtual void UnregisterKeyFrameData(KeyFrameObject keyFrameObject)
    {
        // データから削除
        Data.AllKeyFrameData.Remove(keyFrameObject.Data);
        AllKeyFrameObjectList.Remove(keyFrameObject);

        // トグル解除
        ToggleGroup.UnregisterToggle(keyFrameObject.ToggleButton);

        // 削除
        Destroy(keyFrameObject.gameObject);
        keyFrameObject = null;
    }

    /// <summary>
    /// キーフレームグループのデータをクリアする
    /// </summary>
    protected virtual void ClearTempKeyFrameData()
    {
        for (int i = 0; i < AllKeyFrameObjectList.Count; i++)
        {
            Destroy(AllKeyFrameObjectList[i].gameObject);
            AllKeyFrameObjectList[i] = null;
        }
        AllKeyFrameObjectList.Clear();
        Data = new KeyFrameDataSegment(Target);
    }
}
