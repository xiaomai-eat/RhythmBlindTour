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
/// ????????????????????(????????????????��??????)
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
            if (editModel.TipsAudio.ContainsKey(v.ThisTime))
            {
                if (editModel.Mode.Equals(SystemModeData.PlayMode))
                    AudioEditManager.Instance.Play(editModel.TipsAudio[v.ThisTime].ToArray(), editModel.TipsVolume[v.ThisTime].ToArray());
            }
            if (editModel.TimeLineData.ContainsKey(v.ThisTime))
            {
                foreach (var i in editModel.TimeLineData[v.ThisTime])
                {
                    gameObjects.Add(CreateDrums(i.DrwmsData.DtheTypeOfOperation, i).GetInputMode());
                }
            }
            else if (!editModel.Mode.Equals(SystemModeData.PlayMode))//Bug:???????????????????(????????????????��??)
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
                foreach (var e in ls)
                {
                    gameObjects.Remove(e);
                }
            }


        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        Debug.Log("CreateDrumsManager ?????...");
    }
    /// <summary>
    /// ?????????
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="vector3"></param>
    public CreateSetClass CreateDrums(TheTypeOfOperation operation, DrumsLoadData drumsLoadData = null)
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>(PathConfig.ProfabsOath + "InputMode"));
        InputMode mode = gameObject.GetComponent<InputMode>();
        mode.DrwmsData = drumsLoadData;
        mode.PreAdventClip = cachingModel.GetAudioClip(drumsLoadData.DrwmsData.FPreAdventAudioClipPath);
        mode.LoseClip = cachingModel.GetAudioClip(drumsLoadData.DrwmsData.FLoseAudioClipPath);
        mode.SuccessClip = cachingModel.GetAudioClip(drumsLoadData.DrwmsData.FSucceedAudioClipPath);
        CreateSetClass.Instance.SetInputMode(mode);
        mode.SetOperation(operation);
        this.SendEvent(new DrumsGenerate()
        {
            InputMode = mode
        });
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
        /// ?????????InputMode
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
        /// ????????????��(??��????????)
        /// </summary>
        public void SetSuccessSounds(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            if (Clip != null)
                _Mode.SuccessClip = Clip;
            SetCpVector(channelPosition);
        }
        /// <summary>
        /// ???????????��(??��????????)
        /// </summary>
        public void SetPreAdventSound(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            if (Clip != null)
                _Mode.PreAdventClip = Clip;
            _Mode.DrwmsData.DrwmsData.VPreAdventAudioClipOffsetTime = DelayTime;
            SetCpVector(channelPosition);
        }
        /// <summary>
        /// ?????????��(??��????????)
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

    public enum ChannelPosition //????��??
    {
        LeftChannel,
        RightChannel,
        FullChannel
    }
}
