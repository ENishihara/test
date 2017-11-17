using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyFrameTool : MonoBehaviour
{
    /// <summary>
    /// 時間スライダー
    /// </summary>
    [SerializeField]
    private Slider _timeSlider;

    // MEMO:各KeyFrameGroupは将来的にKeyFrameGroup型のリストを持つだけにするかも
    /// <summary>
    /// hogeのキーフレームグループ
    /// </summary>
    [SerializeField]
    private HogeKeyFrameGroup _hogeKeyFrame;

    /// <summary>
    /// fugaのキーフレームグループ
    /// </summary>
    [SerializeField]
    private FugaKeyFrameGroup _fugaKeyFrame;

    /// <summary>
    /// ファイル名入力フィールド
    /// </summary>
    [SerializeField]
    private InputField _fileNameInput;

    /// <summary>
    /// セーブボタン
    /// </summary>
    [SerializeField]
    private Button _saveButton;

    /// <summary>
    /// ロードボタン
    /// </summary>
    [SerializeField]
    private Button _loadButton;

    /// <summary>
    /// 現在の編集ファイル
    /// </summary>
    private KeyFrameData _currentData;

    /// <summary>
    /// 生成直後の処理
    /// </summary>
    private void Awake()
    {
        _timeSlider.onValueChanged.AddListener(OnTimeSliderValueChanged);
        _saveButton.onClick.AddListener(OnSaveClicked);
        _loadButton.onClick.AddListener(OnLoadClicked);

        // MEMO:キーフレームグループが増えるたび増やす
        _hogeKeyFrame.TimeSliderRef = _timeSlider;
        _fugaKeyFrame.TimeSliderRef = _timeSlider;
    }

    /// <summary>
    /// 時間スライダーの値が変化したとき
    /// </summary>
    private void OnTimeSliderValueChanged(float value)
    {
        // MEMO:キーフレームグループが増えるたび増やす
        // すべてのキーフレームグループのトグルを解除する
        _hogeKeyFrame.ToggleGroup.SetAllTogglesOff();
        _fugaKeyFrame.ToggleGroup.SetAllTogglesOff();
    }

    /// <summary>
    /// セーブボタンを押した時
    /// </summary>
    private void OnSaveClicked()
    {
        if (_currentData == null || string.IsNullOrEmpty(_currentData.FileName))
        {
            Debug.LogWarning("データが読み込まれていません");
            return;
        }
        _currentData.Segments.Clear();
        // MEMO:キーフレームグループが増えるたび増やす
        _currentData.Segments.Add(_hogeKeyFrame.Data);
        _currentData.Segments.Add(_fugaKeyFrame.Data);
        _currentData.SaveDataToJson();
    }

    /// <summary>
    /// ロードボタンを押した時
    /// </summary>
    private void OnLoadClicked()
    {
        _currentData = KeyFrameData.LoadDataFromJson(_fileNameInput.text);
        if (_currentData == null)
        {
            return;
        }
        // MEMO:キーフレームグループが増えるたび増やす
        foreach (var segment in _currentData.Segments)
        {
            switch (segment.Target)
            {
                case KeyFrameToolEnum.TargetType.None:
                    break;
                case KeyFrameToolEnum.TargetType.Hoge:
                    _hogeKeyFrame.ApplyKeyFrameData(segment);
                    break;
                case KeyFrameToolEnum.TargetType.Fuga:
                    _fugaKeyFrame.ApplyKeyFrameData(segment);
                    break;
            }
        }
    }
}
