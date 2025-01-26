using Qf.ClassDatas.AudioEdit;
using Qf.Managers;
using Qf.Models.AudioEdit;
using Qf.Systems;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public class InputMode : MonoBehaviour, IController
{
    [SerializeField]
    TheTypeOfOperation operation;
    AudioEditModel editModel;
    DrumsLoadData drwmsData = new();//鼓点数据
    public DrumsLoadData DrwmsData { get { return drwmsData; } set { drwmsData = value; } }
    public float StartTime;
    public float EndTime;
    [SerializeField]
    float TimeOfExistence;//鼓点存在时间
    [SerializeField]
    AudioClip _PreAdventClip;//来临前播放的音频
    public AudioClip PreAdventClip { get { return _PreAdventClip; } set { _PreAdventClip = value; } }
    [SerializeField]
    AudioClip _SucceedClip;//成功时的音频
    public AudioClip SuccessClip { get { return _SucceedClip; } set { _SucceedClip = value; } }
    [SerializeField]
    AudioClip _LoseClip;//失败时的音频
    public AudioClip LoseClip { get { return _LoseClip; } set { _LoseClip = value; } }
    //[SerializeField]
    //bool isPlay;//是否被点击(用于处理同时出现的情况目前来说用不着)
    public SpriteRenderer SpriteRenderer;
    private void OnEnable()
    {
        TimeOfExistence = 0;
    }
    void Init()
    {
        editModel = this.GetModel<AudioEditModel>();
        StartTime = editModel.ThisTime;
        if (editModel.ThisTime + drwmsData.DrwmsData.TimeOfExistence > editModel.EditAudioClip.length)
        {
            EndTime = editModel.ThisTime + ((editModel.ThisTime + drwmsData.DrwmsData.TimeOfExistence) - editModel.EditAudioClip.length);
            return;
        }
        EndTime = editModel.ThisTime + drwmsData.DrwmsData.TimeOfExistence;
    }
    void Start()
    {
        Init();
    }
    void Update()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;
        TimeOfExistence += Time.deltaTime;
        if (TimeOfExistence >= drwmsData.DrwmsData.TimeOfExistence)
            Lose();
        InputRun();
    }
    void InputRun()
    {
        switch (operation)
        {
            case TheTypeOfOperation.SwipeUp:
                SwipeUp();
                break;
            case TheTypeOfOperation.SwipeDown:
                SwipeDown();
                break;
            case TheTypeOfOperation.SwipeLeft:
                SwipeLeft();
                break;
            case TheTypeOfOperation.SwipeRight:
                SwipeRight();
                break;
            case TheTypeOfOperation.Click:
                Click();
                break;
            default:
                break;
        }
    }
    void SwipeUp()
    {
        if (InputSystems.SwipeUp)
        {
            Debug.Log("上滑");
            Succeed();
            return;
        }
        if (!InputSystems.PlayClick) return;
        Lose();
    }
    void SwipeDown()
    {
        if (InputSystems.SwipeDown)
        {
            Debug.Log("下滑");
            Succeed();
            return;
        }
        if (!InputSystems.PlayClick) return;
        Lose();
    }
    void SwipeLeft()
    {
        if (InputSystems.SwipeLeft)
        {
            Debug.Log("左滑");
            Succeed();
            return;
        }
        if (!InputSystems.PlayClick) return;
        Lose();
    }
    void SwipeRight()
    {
        if (InputSystems.SwipeRight)
        {
            Debug.Log("右滑");
            Succeed();
            return;
        }
        if (!InputSystems.PlayClick) return;
        Lose();
    }
    void Click()
    {
        if (InputSystems.Click)
        {
            Debug.Log("点击");
            Succeed();
            return;
        }
        if (!InputSystems.PlayClick) return;
        Lose();
    }
    void Succeed()
    {
        AudioEditManager.Instance.Play(_SucceedClip);
        Destroy(gameObject);
    }
    void Lose()
    {
        AudioEditManager.Instance.Play(_LoseClip);
        Destroy(gameObject);
    }
    public void SetOperation(TheTypeOfOperation theTypeOfOperation)
    {
        operation = theTypeOfOperation;
    }

    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
/// <summary>
/// 交互操作
/// </summary>
public enum TheTypeOfOperation
{
    SwipeUp,
    SwipeDown,
    SwipeRight,
    SwipeLeft,
    Click
}
