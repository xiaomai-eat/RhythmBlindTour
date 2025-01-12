using MoonSharp.VsCodeDebugger.SDK;
using Qf.ClassDatas.AudioEdit;
using Qf.Events;
using Qf.Managers;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 该类是用于管理鼓点实体的
/// </summary>
public class CreateDrumsManager : ManagerBase
{
    [SerializeField]
    AudioSource audioSource;
    public override void Init()
    {
        CreateSetClass.Instance = new CreateSetClass(audioSource);
        Debug.Log("CreateDrumsManager 已加载...");
    }
    /// <summary>
    /// 添加鼓点实体
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="vector3"></param>
    public CreateSetClass CreateDrums(TheTypeOfOperation operation, Vector3 vector3)
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>(PathConfig.ProfabsOath + "InputMode"));
        InputMode mode = gameObject.GetComponent<InputMode>();
        CreateSetClass.Instance.SetInputMode(mode);
        mode.SetOperation(operation);
        gameObject.transform.position = vector3;
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
                if(instance == null)
                    instance = new CreateSetClass();
                return instance;
            }
            set
            {
                if(value != null)
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
        /// <summary>
        /// 设置触发成功音效(音效及延迟时间)
        /// </summary>
        public void SetSuccessSounds(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            _Mode.SuccessClip = Clip;
        }
        /// <summary>
        /// 设置来临前音效(音效及延迟时间)
        /// </summary>
        public void SetPreAdventSound(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            _Mode.PreAdventClip = Clip;
        }
        /// <summary>
        /// 设置失败音效(音效及延迟时间)
        /// </summary>
        public void SetFailureSound(AudioClip Clip, float DelayTime, ChannelPosition channelPosition = ChannelPosition.FullChannel)
        {
            _Mode.FailClip = Clip;
        }
        
    }

    public enum ChannelPosition //声道位置
    {
        LeftChannel,
        RightChannel,
        FullChannel
    }
}
