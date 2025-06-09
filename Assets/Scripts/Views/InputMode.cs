using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Managers;
using Qf.Models.AudioEdit;
using Qf.Systems;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public class InputMode : MonoBehaviour, IController
{
    [SerializeField]
    TheTypeOfOperation Operation;
    AudioEditModel editModel;
    DrumsLoadData drwmsData = new();//�ĵ�����
    public DrumsLoadData DrwmsData { get { return drwmsData; } set { drwmsData = value; } }
    public float StartTime;
    public float EndTime;
    [SerializeField]
    float TimeOfExistence;//�ĵ����ʱ��
    [SerializeField]
    AudioClip _PreAdventClip;//����ǰ���ŵ���Ƶ
    public AudioClip PreAdventClip { get { return _PreAdventClip; } set { _PreAdventClip = value; } }
    [SerializeField]
    AudioClip _SucceedClip;//�ɹ�ʱ����Ƶ
    public AudioClip SuccessClip { get { return _SucceedClip; } set { _SucceedClip = value; } }
    [SerializeField]
    AudioClip _LoseClip;//ʧ��ʱ����Ƶ
    public AudioClip LoseClip { get { return _LoseClip; } set { _LoseClip = value; } }
    //[SerializeField]
    //bool isPlay;//�Ƿ񱻵��(���ڴ���ͬʱ���ֵ����Ŀǰ��˵�ò���)
    public SpriteRenderer SpriteRenderer;
    private void OnEnable()
    {
        TimeOfExistence = 0;
    }
    void Init()
    {
        editModel = this.GetModel<AudioEditModel>();
        StartTime = editModel.ThisTime + drwmsData.DrwmsData.VPreAdventAudioClipOffsetTime;
        if (editModel.ThisTime + drwmsData.DrwmsData.VTimeOfExistence > editModel.EditAudioClip.length)
        {
            EndTime = editModel.ThisTime + ((editModel.ThisTime + drwmsData.DrwmsData.VTimeOfExistence) - editModel.EditAudioClip.length);
            return;
        }
        EndTime = editModel.ThisTime + drwmsData.DrwmsData.VTimeOfExistence ;
    }
    void Start()
    {
        Init();
    }
    void Update()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;
        TimeOfExistence += Time.deltaTime;
        if (TimeOfExistence >= drwmsData.DrwmsData.VTimeOfExistence)
            Lose();
        InputRun();
    }
    void InputRun()
    {
        switch (Operation)
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
            Debug.Log("�ϻ�");
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
            Debug.Log("�»�");
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
            Debug.Log("��");
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
            Debug.Log("�һ�");
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
            Debug.Log("���");
            Succeed();
            return;
        }
        if (!InputSystems.PlayClick) return;
        Lose();
    }
    void Succeed()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;
        AudioEditManager.Instance.Play(new AudioClip[] { _SucceedClip },new float[] { drwmsData.MusicData.SSucceedVolume });
        this.SendEvent<SucceedTrigger>();
        Destroy(gameObject);
    }
    void Lose()
    {
        if (!editModel.Mode.Equals(SystemModeData.PlayMode)) return;
        AudioEditManager.Instance.Play(new AudioClip[]{ _LoseClip}, new float[] { drwmsData.MusicData.SLoseVolume });
        this.SendEvent<LoseTrigger>();
        Destroy(gameObject);
    }
    public void SetOperation(TheTypeOfOperation theTypeOfOperation)
    {
        Operation = theTypeOfOperation;
    }
    public TheTypeOfOperation GetOperation()
    {
        return Operation;
    }


    public IArchitecture GetArchitecture()
    {
        return GameBody.Interface;
    }
}
/// <summary>
/// ��������
/// </summary>
public enum TheTypeOfOperation
{
  SwipeUp,    
  SwipeDown,    
  SwipeLeft,    
  SwipeRight,
  Click

  


}
