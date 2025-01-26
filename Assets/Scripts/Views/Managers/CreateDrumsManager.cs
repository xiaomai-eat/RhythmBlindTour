using MoonSharp.VsCodeDebugger.SDK;
using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Managers;
using Qf.Models;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 该类是用于管理鼓点实体的(可通过链式方法去设置鼓点数据)
/// </summary>
public class CreateDrumsManager : ManagerBase
{
    [SerializeField]
    AudioSource audioSource;
    AudioEditModel editModel;
    DataCachingModel cachingModel;
    List<InputMode> gameObjects = new();
    public override void Init()
    {
        editModel = this.GetModel<AudioEditModel>();
        cachingModel = this.GetModel<DataCachingModel>();
        CreateSetClass.Instance = new CreateSetClass(audioSource);
        this.RegisterEvent<OnUpdateThisTime>(v =>
        {
            Debug.Log(v.ThisTime);
            if (editModel.TipsAudio.ContainsKey(v.ThisTime))
            {
                AudioEditManager.Instance.Play(editModel.TipsAudio[v.ThisTime].ToArray());
            }
            if (editModel.TimeLineData.ContainsKey(v.ThisTime))
            {
                foreach (var i in editModel.TimeLineData[v.ThisTime])
                {
                    gameObjects.Add(CreateDrums(i.DrwmsData.theTypeOfOperation, default, i).GetInputMode());
                }
            }
            else if (!editModel.Mode.Equals(SystemModeData.PlayMode))
            {
                List<InputMode> ls = new();
                foreach (var j in gameObjects)
                {
                    if (j != null)
                    {
                        if (v.ThisTime > j.EndTime || v.ThisTime < j.StartTime)
                        {
                            ls.Add(j);
                            Destroy(j.gameObject);
                        }
                    }
                }
                foreach(var e in ls)
                {
                    gameObjects.Remove(e);
                }
            }
            

        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        Debug.Log("CreateDrumsManager 已加载...");
    }
    /// <summary>
    /// 添加鼓点实体
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="vector3"></param>
    public CreateSetClass CreateDrums(TheTypeOfOperation operation, Vector3 vector3 = default, DrumsLoadData drumsLoadData = null)
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>(PathConfig.ProfabsOath + "InputMode"));
        InputMode mode = gameObject.GetComponent<InputMode>();
        mode.DrwmsData = drumsLoadData;
        mode.LoseClip = cachingModel.GetAudioClip(drumsLoadData.DrwmsData.LoseAudioClipPath);
        mode.SuccessClip = cachingModel.GetAudioClip(drumsLoadData.DrwmsData.SucceedAudioClipPath);
        CreateSetClass.Instance.SetInputMode(mode);
        mode.SetOperation(operation);
        if (vector3.Equals(default))
            mode.transform.position = new Vector3(0, 0, 0);
        // gameObject.transform.position = vector3;
        return CreateSetClass.Instance;
    }

    public class CreateSetClass
    {
        AudioSource _AudioSource;
        InputMode _Mode;
        public CreateSetClass() { }
        public CreateSetClass(AudioSource audioSource)
        {
            _AudioSource = audioSource;
        }
        static CreateSetClass instance;
        public static CreateSetClass Instance
        {
            get
            {
                if (instance == null)
                    instance = new CreateSetClass();
                return instance;
            }
            set
            {
                if (value != null)
                    instance = value;
            }
        }
        /// <summary>
        /// 设置被影响的InputMode
        /// </summary>
        /// <param name="inputMode"></param>
        public void SetInputMode(InputMode inputMode)
        {
            _Mode = inputMode;
        }
        public InputMode GetInputMode()
        {
            return _Mode;
        }
        public InputMode SetData(DrumsLoadData drumsLoadData)
        {
            _Mode.DrwmsData = drumsLoadData;
            return _Mode;
        }
        /// <summary>
        /// 设置触发成功音效(音效及延迟时间)
        /// </summary>
        public void SetSuccessSounds(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            if (Clip != null)
                _Mode.SuccessClip = Clip;
            SetCpVector(channelPosition);
        }
        /// <summary>
        /// 设置来临前音效(音效及延迟时间)
        /// </summary>
        public void SetPreAdventSound(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            if (Clip != null)
                _Mode.PreAdventClip = Clip;
            _Mode.DrwmsData.DrwmsData.PreAdventAudioClipOffsetTime = DelayTime;
            SetCpVector(channelPosition);
        }
        /// <summary>
        /// 设置失败音效(音效及延迟时间)
        /// </summary>
        public void SetFailureSound(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            if (Clip != null)
                _Mode.LoseClip = Clip;
            SetCpVector(channelPosition);
        }
        void SetCpVector(ChannelPosition channelPosition)
        {
            switch (channelPosition)
            {
                case ChannelPosition.FullChannel:
                    _AudioSource.panStereo = 0;
                    break;
                case ChannelPosition.LeftChannel:
                    _AudioSource.panStereo = -1;
                    break;
                case ChannelPosition.RightChannel:
                    _AudioSource.panStereo = 1;
                    break;
                default:
                    break;
            }
        }
    }

    public enum ChannelPosition //声道位置
    {
        LeftChannel,
        RightChannel,
        FullChannel
    }
}
