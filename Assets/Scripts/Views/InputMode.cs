using Qf.ClassDatas.AudioEdit;
using Qf.Systems;
using UnityEngine;

public class InputMode : MonoBehaviour
{
    [SerializeField]
    TheTypeOfOperation operation;
    DrwmsData drwmsData = new();
    public DrwmsData DrwmsData { get { return drwmsData; } set { drwmsData = value; } }
    [SerializeField]
    float TimeOfExistence;//鼓点存在时间
    AudioClip _PreAdventClip;//来临前播放的音频
    public AudioClip PreAdventClip { get { return _PreAdventClip; } set { _PreAdventClip = value; } }
    AudioClip _SucceedClip;//成功时的音频
    public AudioClip SuccessClip { get { return _SucceedClip; } set { _SucceedClip = value; } }
    AudioClip _FailClip;//失败时的音频
    public AudioClip FailClip { get { return _FailClip; }  set { _FailClip = value; } }
    //[SerializeField]
    //bool isPlay;//是否被点击(用于处理同时出现的情况目前来说用不着)
    public SpriteRenderer SpriteRenderer;
    private void OnEnable()
    {
        TimeOfExistence = 0;
    }
    void Init()
    {

    }
    void Start()
    {
        Init();
    }
    void Update()
    {
        TimeOfExistence += Time.deltaTime;
        if (drwmsData.DelayTheTriggerTime < TimeOfExistence)
        {
            InputRun();
        }
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
        }
    }
    void SwipeDown()
    {
        if (InputSystems.SwipeDown)
        {
            Debug.Log("下滑");
        }
    }
    void SwipeLeft()
    {
        if (InputSystems.SwipeLeft)
        {
            Debug.Log("左滑");
        }
    }
    void SwipeRight()
    {
        if (InputSystems.SwipeRight)
        {
            Debug.Log("右滑");
        }
    }
    void Click()
    {
        if (InputSystems.Click)
        {
            Debug.Log("点击");
        }
    }
    public void SetOperation(TheTypeOfOperation theTypeOfOperation)
    {
        operation = theTypeOfOperation;
    }
}
public enum TheTypeOfOperation
{
    SwipeUp,
    SwipeDown,
    SwipeRight,
    SwipeLeft,
    Click
}
