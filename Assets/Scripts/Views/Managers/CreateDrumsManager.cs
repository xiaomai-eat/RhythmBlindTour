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

public class CreateDrumsManager : ManagerBase
{
    [SerializeField]
    AudioSource audioSource;
    AudioEditModel editModel;
    DataCachingModel cachingModel;
    List<InputMode> gameObjects = new();
    // 已实例化鼓点的中心时间（防止重复创建）
    private HashSet<float> activeDrumCenters = new();

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

    if (editModel.TimeLineData != null)
    {
        foreach (var kvp in editModel.TimeLineData)
        {
            float centerTime = kvp.Key;

            foreach (var data in kvp.Value)
            {
                float existence = data.DrwmsData.VTimeOfExistence;
                float start = centerTime - existence / 2f;
                float end = centerTime + existence / 2f;

                if (v.ThisTime >= start && v.ThisTime <= end)
                {
                    if (!activeDrumCenters.Contains(centerTime))
                    {
                        var inputMode = CreateDrums(data.DrwmsData.DtheTypeOfOperation, data).GetInputMode();
                        gameObjects.Add(inputMode);
                        activeDrumCenters.Add(centerTime);
                    }
                }
            }
        }
    }

    if (!editModel.Mode.Equals(SystemModeData.PlayMode))
    {
        List<InputMode> toRemove = new();

        foreach (var j in gameObjects)
        {
            if (j != null && (v.ThisTime > j.EndTime || v.ThisTime < j.StartTime))
            {
                toRemove.Add(j);
                Destroy(j.gameObject);

                // ❗ 移除对应鼓点标识，允许将来重新生成
                activeDrumCenters.Remove(j.DrwmsData.DrwmsData.CenterTime);
            }
        }

        foreach (var e in toRemove)
        {
            gameObjects.Remove(e);
        }
    }

}).UnRegisterWhenGameObjectDestroyed(gameObject);


        Debug.Log("CreateDrumsManager initialized...");
    }

    /// <summary>
    /// 清除所有活跃鼓点中心时间（允许再次实例化所有鼓点）
    /// 用于编辑模式下调试和手动重置
    /// </summary>
    public void ResetAllActiveCenters()
    {
        activeDrumCenters.Clear();
        Debug.Log("[CreateDrumsManager] 所有 activeDrumCenters 已清空。");
    }


    public CreateSetClass CreateDrums(TheTypeOfOperation operation, DrumsLoadData drumsLoadData = null)
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>(PathConfig.ProfabsOath + "InputMode"));
        InputMode mode = gameObject.GetComponent<InputMode>();

        // 如果未设置中心时间，使用当前 ThisTime 作为默认
        if (drumsLoadData != null && drumsLoadData.DrwmsData.CenterTime == 0f)
        {
            drumsLoadData.DrwmsData.CenterTime = this.GetModel<AudioEditModel>().ThisTime;
        }

        mode.DrwmsData = drumsLoadData;

        // 音效赋值
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

        public void SetSuccessSounds(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            if (Clip != null)
                _Mode.SuccessClip = Clip;
            SetCpVector(channelPosition);
        }

        public void SetPreAdventSound(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            if (Clip != null)
                _Mode.PreAdventClip = Clip;
            _Mode.DrwmsData.DrwmsData.VPreAdventAudioClipOffsetTime = DelayTime;
            SetCpVector(channelPosition);
        }

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

    public enum ChannelPosition
    {
        LeftChannel,
        RightChannel,
        FullChannel
    }
}
