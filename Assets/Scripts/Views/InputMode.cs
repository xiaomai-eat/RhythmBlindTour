using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Managers;
using Qf.Models.AudioEdit;
using Qf.Systems;
using QFramework;
using UnityEngine;



//解决鼓点重叠问题 !改为了统一管理鼓点的判定! 2025/06/13 - mixyao
public class InputMode : MonoBehaviour, IController
{
    [SerializeField] TheTypeOfOperation Operation;
    AudioEditModel editModel;

    DrumsLoadData drwmsData = new();
    public DrumsLoadData DrwmsData { get => drwmsData; set => drwmsData = value; }

    public float StartTime;
    public float EndTime;

    [SerializeField] float TimeOfExistence;
    [SerializeField] AudioClip _PreAdventClip;
    [SerializeField] AudioClip _SucceedClip;
    [SerializeField] AudioClip _LoseClip;

    public AudioClip PreAdventClip { get => _PreAdventClip; set => _PreAdventClip = value; }
    public AudioClip SuccessClip { get => _SucceedClip; set => _SucceedClip = value; }
    public AudioClip LoseClip { get => _LoseClip; set => _LoseClip = value; }

    public SpriteRenderer SpriteRenderer;
    public bool IsActive = true;

    private void OnEnable()
    {
        TimeOfExistence = 0;
    }

    void Start()
    {
        editModel = this.GetModel<AudioEditModel>();

        float centerTime = drwmsData.DrwmsData.CenterTime;
        float existence = drwmsData.DrwmsData.VTimeOfExistence;
        StartTime = centerTime - existence / 2f;
        EndTime = centerTime + existence / 2f;
    }

    void Update()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;

        TimeOfExistence += Time.deltaTime;

        // 超时自动失败
        if (TimeOfExistence >= drwmsData.DrwmsData.VTimeOfExistence && IsActive)
        {
            LoseByManager();
            IsActive = false;
        }

        // ❌ 禁用独立输入检查
    }

    public void SucceedByManager()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;
        AudioEditManager.Instance.Play(new AudioClip[] { _SucceedClip }, new float[] { drwmsData.MusicData.SSucceedVolume });
        this.SendEvent<SucceedTrigger>();
        Destroy(gameObject);
    }

    public void LoseByManager()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;
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
