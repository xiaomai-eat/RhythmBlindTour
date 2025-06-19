using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Managers;
using Qf.Models.AudioEdit;
using Qf.Systems;
using QFramework;
using UnityEngine;

public class InputMode : MonoBehaviour, IController
{
    [SerializeField] TheTypeOfOperation Operation;
    AudioEditModel editModel;

    DrumsLoadData drwmsData = new();
    public DrumsLoadData DrwmsData { get => drwmsData; set => drwmsData = value; }

    public float StartTime;
    public float EndTime;
    public float PreAdventTime;

    [SerializeField] float TimeOfExistence;
    [SerializeField] AudioClip _PreAdventClip;
    [SerializeField] AudioClip _SucceedClip;
    [SerializeField] AudioClip _LoseClip;

    public AudioClip PreAdventClip { get => _PreAdventClip; set => _PreAdventClip = value; }
    public AudioClip SuccessClip { get => _SucceedClip; set => _SucceedClip = value; }
    public AudioClip LoseClip { get => _LoseClip; set => _LoseClip = value; }

    public SpriteRenderer SpriteRenderer;
    public bool IsActive = true;
    public bool HasJudged = false;

    private void OnEnable()
    {
        TimeOfExistence = 0;
    }
    private static readonly System.Collections.Generic.Dictionary<TheTypeOfOperation, float> TypeYOffset = new()
{
    { TheTypeOfOperation.Click, 0f },
    { TheTypeOfOperation.SwipeUp, 1.5f },
    { TheTypeOfOperation.SwipeDown, -1.5f },
    { TheTypeOfOperation.SwipeLeft, 0.75f },
    { TheTypeOfOperation.SwipeRight, -0.75f }
};

    void Start()
    {
        editModel = this.GetModel<AudioEditModel>();

        float centerTime = drwmsData.DrwmsData.CenterTime;
        float existence = drwmsData.DrwmsData.VTimeOfExistence;
        float preOffset = drwmsData.DrwmsData.VPreAdventAudioClipOffsetTime;

        StartTime = centerTime - existence / 2f;
        EndTime = centerTime + existence / 2f;
        PreAdventTime = centerTime - preOffset;

        // 设置 Y 偏移（localPosition 不影响 world 路径）
        if (TypeYOffset.TryGetValue(Operation, out float offsetY))
        {
            Vector3 pos = transform.localPosition;
            pos.y += offsetY;
            transform.localPosition = pos;
        }

        if (PreAdventClip != null)
        {
            AudioEditManager.Instance.Play(new AudioClip[] { PreAdventClip }, new float[] { drwmsData.MusicData.SPreAdventVolume });
        }
    }

    public void InitializeTimes(float preAdventTime, float startTime, float endTime)
    {
        PreAdventTime = preAdventTime;
        StartTime = startTime;
        EndTime = endTime;
    }

    void Update()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode) || HasJudged)
            return;

        TimeOfExistence += Time.deltaTime;

        float now = editModel.ThisTime;

        // 超时未输入 → 自动失败
        if (now > EndTime)
        {
            Debug.Log($"[Timeout Fail] now: {now:F3}, StartTime: {StartTime:F3}, EndTime: {EndTime:F3}");
            LoseByManager();
            HasJudged = true;
        }
    }

    /// <summary>
    /// 由集中式输入控制器调用，进行输入判定
    /// </summary>
    public bool ReceiveInput(TheTypeOfOperation inputType)
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode) || HasJudged)
            return false;

        float now = editModel.ThisTime;

        if (now < StartTime)
        {

            // Debug.Log($"[Early Input Fail] now: {now:F3}, StartTime: {StartTime:F3}, Expected: {Operation}, Got: {inputType}");
            // LoseByManager();
            // HasJudged = true;
            return false;
        }
        else if (now >= StartTime && now <= EndTime)
        {
            if (inputType == Operation)
            {
                Debug.Log($"[Success] now: {now:F3}, StartTime: {StartTime:F3}, EndTime: {EndTime:F3}, Type: {Operation}");
                SucceedByManager();
                HasJudged = true;
                return true;
            }
            else
            {
                Debug.Log($"[Wrong Type Fail] now: {now:F3}, StartTime: {StartTime:F3}, Expected: {Operation}, Got: {inputType}");
                LoseByManager();
                HasJudged = true;
                return false;
            }
        }

        // 超过 EndTime 的输入交由 Update 判定，不在这里处理
        return false;
    }

    public void SucceedByManager()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;

        float now = editModel.ThisTime;
        Debug.Log($"[SucceedByManager] now: {now:F3}, StartTime: {StartTime:F3}, EndTime: {EndTime:F3}");

        AudioEditManager.Instance.Play(new AudioClip[] { _SucceedClip }, new float[] { drwmsData.MusicData.SSucceedVolume });
        this.SendEvent<SucceedTrigger>();
        Destroy(gameObject);
    }

    public void LoseByManager()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;

        float now = editModel.ThisTime;
        Debug.Log($"[LoseByManager] now: {now:F3}, StartTime: {StartTime:F3}, EndTime: {EndTime:F3}");

        AudioEditManager.Instance.Play(new AudioClip[] { _LoseClip }, new float[] { drwmsData.MusicData.SLoseVolume });
        this.SendEvent<LoseTrigger>();
        Destroy(gameObject);
    }

    public void SetOperation(TheTypeOfOperation type) => Operation = type;
    public TheTypeOfOperation GetOperation() => Operation;

    public IArchitecture GetArchitecture() => GameBody.Interface;
}


public enum TheTypeOfOperation
{
    SwipeUp,
    SwipeDown,
    SwipeLeft,
    SwipeRight,
    Click
}
