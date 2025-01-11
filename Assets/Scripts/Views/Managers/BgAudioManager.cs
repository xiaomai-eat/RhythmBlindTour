using Qf.ClassDatas;
using Qf.Commands.AudioEdit;
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
        [SerializeField]
        int Mode;
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
            this.RegisterEvent<OnEditMode>(v =>
            {
                Debug.Log("编辑模式");
                Mode = 0;
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnPlayMode>(v =>
            {
                Debug.Log("游玩模式");
                Mode = 1;
                PlayMode();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnRecordingMode>(v =>
            {
                Debug.Log("录制模式");
                Mode = 2;
                PlayMode();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ExitPlayMode>(v =>
            {
                Debug.Log("退出游玩模式");
                ExitPlayMode();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<ExitRecordingMode>(v =>
            {
                Debug.Log("退出录制模式");
                ExitPlayMode();
            });
        }
        void UpdateAll()
        {
            this.SendCommand(new SetAudioEditThisTimeCommand(audioSource.time));
        }
        void PlayMode()
        {
            UpdateData();
        }
        void ExitPlayMode()
        {
            audioSource.Pause();
        }
        void UpdateData()
        {
            audioSource.time = editModel.ThisTime;
            audioSource.clip = editModel.EditAudioClip;
        }
        private void Update()
        {
            if (Mode == 0)//编辑模式
            {

            }
            else if (Mode == 1) //游玩模式
            {
                UpdateAll();
            }
            else if (Mode == 2)//录制模式
            {
                UpdateAll();
            }
        }
        void Start()
        {
            Init();
            this.RegisterEvent<MainAudioChangeValue>(v => UpdateData()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        public IArchitecture GetArchitecture()
        {
            return GameBody.Interface;
        }
    }
}