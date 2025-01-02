using Qf.ClassDatas;
using Qf.Events;
using Qf.Models;
using Qf.Models.AudioEdit;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Qf.Managers
{
    public class BgAudioManager : MonoBehaviour , IController
    {
        [SerializeField]
        AudioSource audioSource;//音频源
        AudioEditModel editModel;

        private void Awake()
        {
            
        }

        void Init()
        {
            editModel = this.GetModel<AudioEditModel>();
            if(editModel.EditAudioClip != null)
            {
                audioSource.clip = editModel.EditAudioClip;
            }
            this.RegisterEvent<OnPlayMode>(v =>
            {
                Debug.Log("游玩模式");
                PlayMode();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ExitPlayMode>(v =>
            {
                Debug.Log("退出游玩模式");
                ExitPlayMode();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnRecordingMode>(v =>
            {
                Debug.Log("录制模式");
                PlayMode();
            });

            this.RegisterEvent<ExitRecordingMode>(v =>
            {
                Debug.Log("退出录制模式");
                ExitPlayMode();
            });
        }

        void PlayMode()
        {
            audioSource.time = editModel.ThisTime;
            audioSource.clip = editModel.EditAudioClip;
        }
        void ExitPlayMode()
        {
            audioSource.Pause();
        }

        void Start()
        {
            Init();
        }
        public IArchitecture GetArchitecture()
        {
            return GameBody.Interface;
        }
    }
}