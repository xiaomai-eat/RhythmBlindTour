using UnityEngine;
using UnityEngine.UI;
using Qf.Events;
using Qf.Managers;
using QFramework;

public class mModeButtonsController : MonoBehaviour, IController
{
    public Button[] modeButtons; // 顺序为：编辑、游玩、录制
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private int currentSelectedIndex = -1;

    void Start()
    {
        if (modeButtons.Length >= 3)
        {
            modeButtons[0].onClick.AddListener(() => AudioEditManager.Instance?.EnterEditMode());
            modeButtons[1].onClick.AddListener(() => AudioEditManager.Instance?.EnterPlayMode());
            modeButtons[2].onClick.AddListener(() => AudioEditManager.Instance?.EnterRecordingMode());
        }

        this.RegisterEvent<OnEditMode>(v => SetSelectedMode(0)).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnPlayMode>(v => SetSelectedMode(1)).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnRecordingMode>(v => SetSelectedMode(2)).UnRegisterWhenGameObjectDestroyed(gameObject);

        // 初始设置为 EditMode
        AudioEditManager.Instance?.EnterEditMode(); // 触发模式切换逻辑
        SetSelectedMode(0);                         // 设置按钮视觉为选中 Edit
    }


    public void SetSelectedMode(int index)
    {
        currentSelectedIndex = index;

        for (int i = 0; i < modeButtons.Length; i++)
        {
            var colors = modeButtons[i].colors;
            colors.normalColor = (i == currentSelectedIndex) ? selectedColor : normalColor;
            modeButtons[i].colors = colors;

            if (modeButtons[i].targetGraphic != null)
                modeButtons[i].targetGraphic.color = (i == currentSelectedIndex) ? selectedColor : normalColor;
        }
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
