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
    DrumsLoadData drwmsData = new();
    public DrumsLoadData DrwmsData { get { return drwmsData; } set { drwmsData = value; } }
    public float StartTime;
    public float EndTime;
    [SerializeField]
    float TimeOfExistence;
    [SerializeField]
    AudioClip _PreAdventClip;
    public AudioClip PreAdventClip { get { return _PreAdventClip; } set { _PreAdventClip = value; } }
    [SerializeField]
    AudioClip _SucceedClip;
    public AudioClip SuccessClip { get { return _SucceedClip; } set { _SucceedClip = value; } }
    [SerializeField]
    AudioClip _LoseClip;
    public AudioClip LoseClip { get { return _LoseClip; } set { _LoseClip = value; } }
   
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
            EndTime = editModel.ThisTime + (editModel.ThisTime + drwmsData.DrwmsData.VTimeOfExistence - editModel.EditAudioClip.length);
            return;
        }
        else
        {
        EndTime = editModel.ThisTime + drwmsData.DrwmsData.VTimeOfExistence;

        }
        
            float shift = (EndTime - StartTime) / 2f;
    StartTime -= shift;
    EndTime -= shift;
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

public enum TheTypeOfOperation
{
  SwipeUp,    
  SwipeDown,    
  SwipeLeft,    
  SwipeRight,
  Click

  


}
